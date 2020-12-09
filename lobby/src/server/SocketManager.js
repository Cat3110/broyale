const io = require('./index.js').io
const Game = require('./Game');

var exec = require('child_process').execFile;
var assert = require('assert');

const { VERIFY_USER, LOGIN_WITH_DEVICEID, GET_CHARACTERS, SET_CHARACTER, USER_CONNECTED, USER_DISCONNECTED, 
		LOGOUT, COMMUNITY_CHAT, MESSAGE_RECIEVED, MESSAGE_SENT,
		TYPING, CREATE_GAME, GAME_OVER, SERVER_UPDATE, GAME_UPDATE,
		PLAYER_INPUT, START_GAME, JOIN_GAME, CLIENT_ADDED,
		GET_GAMES, WINNER, LOSER, LEAVE, GAME_STARTED, GAME_START_FAILED
	} = require('../Events')

const { createUser, createMessage, createChat, createGame } = require('../Factories');
const { db } = require('./index.js');

const GameStartDelay = 15

let connectedUsers = { }
let counter = 0
let games = new Set()

let communityChat = createChat()

module.exports = function(socket){
					
	// console.log('\x1bc'); //clears console
	console.log("Socket Id:" + socket.id);

	let sendMessageToChatFromUser;

	let sendTypingFromUser;

	//Verify Username
	socket.on(VERIFY_USER, (nickname, callback)=>{
		if(isUser(connectedUsers, nickname)){
			callback({ isUser:true, user:null })
		}else{
			callback({ isUser:false, user:createUser({name:nickname})})
		}
	})

	socket.on(LOGIN_WITH_DEVICEID, async (deviceId, callback)=>{
		var user = await User.findOne({ device_id: deviceId });		
		if(user == null){
			const newUser = { device_id: deviceId }
			user = await new User(newUser).save()
			const character =  await new Character({user_id:user._id}).save()
			callback({ status: 200, data:{ is_new:true, user:user } })
		}else callback({ status: 200,  data:{ is_new:false, user:user} })

		socket.dbUser = user

		user.last_login_at = Date.now()
		user.save()
	})

	//With verified user
	socket.on(GET_CHARACTERS, async(character, callback)=>{
		if("dbUser" in socket){
			var characters = await Character.find({ user_id: socket.dbUser._id });
			callback({ status: 200, data:{ characters: characters } })
		}else{
			callback({ status: 401 })
		}
	})

	socket.on(SET_CHARACTER, async(character, callback)=>{
		if("dbUser" in socket){
			var dbCharacter = await Character.findOne({ user_id: socket.dbUser._id, _id: character._id });
			dbCharacter.sex = character.sex
			dbCharacter.skin_id = character.skin_id

			dbCharacter.head_type = character.head_type
			dbCharacter.body_type = character.body_type
			dbCharacter.pants_type = character.pants_type

			dbCharacter.head_color = character.head_color
			dbCharacter.body_color = character.body_color
			dbCharacter.pants_color = character.pants_color

			dbCharacter.skill_set = character.skill_set

			dbCharacter.save()

			callback({ status: 200, data:{ character: dbCharacter } })
		}else{
			callback({ status: 401 })
		}
	})

	//User Connects with username
	socket.on(USER_CONNECTED, (user)=>{
		connectedUsers = addUser(connectedUsers, user)
		socket.user = user

		sendMessageToChatFromUser = sendMessageToChat(user.name)
		sendTypingFromUser = sendTypingToChat(user.name)

		io.emit(USER_CONNECTED, connectedUsers)
		io.emit(SERVER_UPDATE, getGamesList())
		console.log(connectedUsers);
	})
	
	//User disconnects
	socket.on('disconnect', ()=>{
		if("user" in socket){
			connectedUsers = removeUser(connectedUsers, socket.user.name)

			io.emit(USER_DISCONNECTED, connectedUsers)
			console.log("Disconnect", connectedUsers);
		}
	})

	//User logsout
	// socket.on(LOGOUT, ()=>{
	// 	connectedUsers = removeUser(connectedUsers, socket.user.name)
	// 	io.emit(USER_DISCONNECTED, connectedUsers)
	// 	console.log("Disconnect", connectedUsers);
	// })

	//Get Community Chat
	socket.on(COMMUNITY_CHAT, (callback)=>{
		callback(communityChat)
	})

	socket.on(MESSAGE_SENT, ({chatId, message})=>{
		sendMessageToChatFromUser(chatId, message)
	})

	socket.on(TYPING, ({chatId, isTyping})=>{
		sendTypingFromUser(chatId, isTyping)
	})

	//User logsout
	socket.on(LOGOUT, ({user})=>{
		for (const game of games) {
			if(game.isUserInGame(user)){
				game.addPlayer(user)
				if(game.isStarted()){
					let id = game.getOpponent(user.id)
					io.emit(`${LEAVE}-${id}`, games)
				}
				games.delete(game);
			}
		}
		connectedUsers = removeUser(connectedUsers, socket.user.name)
		io.emit(USER_DISCONNECTED, connectedUsers)
		io.emit(SERVER_UPDATE, getGamesList())
		console.log("Disconnect", connectedUsers);
	})

	socket.on(JOIN_GAME, async (gameId,callback)=>{			
		if("dbUser" in socket){
			const user = socket.dbUser
			for (const game of games) {
				if(game.getId()===gameId){
					if( game.isGameStarted() ) {
						callback({ status: 200 })
						break
					}

					game.addPlayer(user)

					await Games.deleteOne( {gid:gameId })

					var dbGame = getGameInfo(game);	
					dbGame.gid = dbGame.id
					var newGame = await new Games(dbGame)
					//newGame.anything = game
					//newGame.markModified('anything');
					newGame.save()

					// var dbGame = await Games.findOne( {gid:gameId })
					// dbGame._doc.players.push(user)
					// dbGame._doc.save()

					callback({ status: 200, data: { game: game, eta: game.getETATime() }})
					io.emit(SERVER_UPDATE, getGamesList())
					break				
				}
			}			
			callback({ status: 404 })
		}else{
			callback({ status: 401 })
		}			
	})
/*
	socket.on(START_GAME, async (gameId,callback)=>{
		if("dbUser" in socket)
		{
			const user = socket.dbUser
			for (const game of games) {
				if(game.getId()===gameId){
					if( !game.isUserInGame(user) ){
						callback({ status: 401 })
						break
					}	

					if( game.isGameStarted() ) {
						callback({ status: 200 })
						break
					}

					var rport = game.getServerInfo().port
					var isWin = process.platform === "win32"
					var processName = isWin ? "broyal-server.exe" : "broyal-server.x86_64"

					const lateServStarter = later(GameStartDelay * 1000)
							.then(msg => 
							{
								execute(processName, ['--port', rport, '--gameid', gameId], "../server-linux")
								.then((info) => { 
									console.log("Server started: ", info)
									return info
								})
								.catch(error => {console.log(error.message)})

								console.log(processName + " on port " + rport + " started")
							})
							.catch(() => { 
								console.log("cancelled"); 
							})								
				
					game.startGame(10);					

					var dbGame = getGameInfo(game);	
					dbGame.gid = dbGame.id
					var newGame = await new Games(dbGame)
					//newGame.anything = game
					//newGame.markModified('anything');
					newGame.save()

					callback({ status: 200 })
					io.emit(GAME_UPDATE, game)	
					io.emit(SERVER_UPDATE, getGamesList())
					break						
				}
			}			
		}else callback({ status: 401 })		
	})

	socket.on(PLAYER_INPUT, ({gameId, message, user})=>{
		for (const game of games) {
			if(game.getId()===gameId){
				if(game.processInput(message, user)){
					io.emit(SERVER_UPDATE, getUpdatedList())
					io.emit(`${WINNER}-${user.id}`, getUpdatedList())
					let id = game.getOpponent(user.id)
					io.emit(`${LOSER}-${id}`, getUpdatedList())
					games.delete(game);
				}
				io.emit(SERVER_UPDATE, getUpdatedList())
			}
		}
	})
*/
	// socket.on(CREATE_GAME, (gameName, callback)=>{
	// 	if("dbUser" in socket){
	// 		const user = socket.dbUser
	// 		counter++
	// 		let game = Game.create(counter, user, GameStartDelay)
	// 		game.addPlayer(user)
	// 		socket.emit(`${CLIENT_ADDED}-${user.id}`, { game: game })
	// 		games.add(game)
	// 		io.emit(SERVER_UPDATE, getGamesList())
	// 		callback({ status: 200, data:{ game: game } })
	// 	}else{
	// 		callback({ status: 401 })
	// 	}		
	// })

	socket.on(CREATE_GAME, async (gameName, callback)=>{
		if("dbUser" in socket){
			const user = socket.dbUser
			counter++

			let game = Game.create(counter, user, GameStartDelay)
			game.addPlayer(user)
			games.add(game)

			var rport = game.getServerInfo().port
			var isWin32 = process.platform === "win32"
			var processName = isWin32 ? "broyal-server.exe" : "../server-linux/broyal-server.x86_64"
			var path = isWin32 ? "../bin/server/" : "../server-linux"

			const lateServStarter = later(GameStartDelay * 1000)
					.then(msg => 
					{
						execute(processName, ['--port', rport, '--gameid', game.getId()], path )
						.then((info) => { 	
							console.log(processName + " on port " + rport + " started")
							return info
						})
						.catch(error => {
							io.emit(GAME_START_FAILED, game)
							console.log(error.message)})

						game.startGame()
						io.emit(GAME_STARTED, game)
						console.log(processName + " on port " + rport + " started")					
					})
					.catch((error) => { 
						io.emit(GAME_START_FAILED, game)
						console.log("cancelled " + error.message); 
					})			

			var dbGame = getGameInfo(game);	
			dbGame.gid = dbGame.id
			var newGame = await new Games(dbGame)
			//newGame.anything = game
			//newGame.markModified('anything');
			newGame.save()
			io.emit(SERVER_UPDATE, getGamesList())
			callback({ status: 200, data:{ game: game, eta: GameStartDelay} })
		}else{
			callback({ status: 401 })
		}		
	})

	socket.on(GET_GAMES, (game,callback) => {callback({ status: 200, data:getGamesList()})})
}
/*
* Returns a function that will take a chat id and a boolean isTyping
* and then emit a broadcast to the chat id that the sender is typing
* @param sender {string} username of sender
* @return function(chatId, message)
*/
function sendTypingToChat(user){
	return (chatId, isTyping)=>{
		io.emit(`${TYPING}-${chatId}`, {user, isTyping})
	}
}

/**
 * Function to execute exe
 * @param {string} fileName The name of the executable file to run.
 * @param {string[]} params List of string arguments.
 * @param {string} path Current working directory of the child process.
 */
function execute(fileName, params, path) {
    let promise = new Promise((resolve, reject) => {
        exec(fileName, params, { cwd: path }, (err, data) => {
            if (err) reject(err);
			else 
			{
				console.log(`stdout:\n${data}`);
				resolve(data);
			}			
        });

    });
    return promise;
}

function later(delay) {
    return new Promise(function(resolve) {
        setTimeout(resolve, delay);
    });
}

/*
* Returns a function that will take a chat id and message
* and then emit a broadcast to the chat id.
* @param sender {string} username of sender
* @return function(chatId, message)
*/
function sendMessageToChat(sender){
	return (chatId, message)=>{
		io.emit(`${MESSAGE_RECIEVED}-${chatId}`, createMessage({message, sender}))
	}
}

/*
* Adds user to list passed in.
* @param userList {Object} Object with key value pairs of users
* @param user {User} the user to added to the list.
* @return userList {Object} Object with key value pairs of Users
*/
function addUser(userList, user){
	let newList = Object.assign({}, userList)
	newList[user.name] = user
	return newList
}

/*
* Removes user from the list passed in.
* @param userList {Object} Object with key value pairs of Users
* @param username {string} name of user to be removed
* @return userList {Object} Object with key value pairs of Users
*/
function removeUser(userList, username){
	let newList = Object.assign({}, userList)
	delete newList[username]
	return newList
}

/*
* Checks if the user is in list passed in.
* @param userList {Object} Object with key value pairs of Users
* @param username {String}
* @return userList {Object} Object with key value pairs of Users
*/
function isUser(userList, username){
  	return username in userList
}

function getGameInfo(game) {
	return {
		id: game.getId(),
		owner: game.getOwner(),
		name: game.getName(),
		isStarted: game.isGameStarted(),
		turn: game.getTurn(),
		serverInfo: game.getServerInfo(),
		players: JSON.parse(game.getUsersToJSON())	
	}
}

/*
* Creates a json object that contains the state of all current
* games

function getGamesList() {
	return {
		games: Array.from(games).map(game => {
				return getGameInfo(game)
		})
	}
}
*/
function getGamesList() {
	return { games: Array.from(games) }
}