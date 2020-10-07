const io = require('./index.js').io
const Game = require('./Game');

var exec = require('child_process').execFile;
var assert = require('assert');

const { VERIFY_USER, USER_CONNECTED, USER_DISCONNECTED, 
		LOGOUT, COMMUNITY_CHAT, MESSAGE_RECIEVED, MESSAGE_SENT,
		TYPING, CREATE_GAME, GAME_OVER, SERVER_UPDATE, GAME_UPDATE,
		PLAYER_INPUT, ACTIVE_GAME, START_GAME, CLIENT_ADDED,
		UPDATE_LIST, WINNER, LOSER,LEAVE
	} = require('../Events')

const { createUser, createMessage, createChat, createGame } = require('../Factories')

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

	//User Connects with username
	socket.on(USER_CONNECTED, (user)=>{
		connectedUsers = addUser(connectedUsers, user)
		socket.user = user

		sendMessageToChatFromUser = sendMessageToChat(user.name)
		sendTypingFromUser = sendTypingToChat(user.name)

		io.emit(USER_CONNECTED, connectedUsers)
		io.emit(SERVER_UPDATE, getUpdatedList())
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
					io.emit(`${LEAVE}-${id}`, getUpdatedList())
				}
				games.delete(game);
			}
		}
		connectedUsers = removeUser(connectedUsers, socket.user.name)
		io.emit(USER_DISCONNECTED, connectedUsers)
		io.emit(SERVER_UPDATE, getUpdatedList())
		console.log("Disconnect", connectedUsers);
	})

	socket.on(ACTIVE_GAME, ({gameId, previousGame, user},callback)=>{
		for (const game of games) {
			if(game.getId()===gameId){
				game.addPlayer(user)
				callback(getUpdatedList())
				break				
			} else if (game.getId()===previousGame) {
					if(game.isStarted()){
						let id = game.getOpponent(user.id)
						io.emit(`${LEAVE}-${id}`, getUpdatedList())
					}
				games.delete(game);				
			}
		}
		io.emit(SERVER_UPDATE, getUpdatedList())		
	})

	socket.on(START_GAME, ({gameId, user},callback)=>{
		for (const game of games) {
			if(game.getId()===gameId){				
				var rport = game.getServerInfo().port
				var isWin = process.platform === "win32"
				var processName = isWin ? "broyal-server.exe" : "broyal-server.x86_64"

				const lateServStarter = later(15000)
						.then(msg => 
						{
							execute(processName, ['--port', rport], "../server-linux")
							.then((info) => { 
								console.log("Server started: ", info)
								return info
							})
							.catch(error => {console.log(error.message)})

							console.log(processName + " on port " + rport + " started")
						})
						.catch(() => { console.log("cancelled"); })

							

				game.startGame(10);
				io.emit(GAME_UPDATE, getGameInfo(game))	
				callback({time:10, address:"127.0.0.1", port:rport})
				break						
			}
		}
		io.emit(SERVER_UPDATE, getUpdatedList())		
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

	socket.on(CREATE_GAME, ({gameName, user}, callback)=>{
		counter++
		let game = Game.create({counter,user})
		game.addPlayer(user)
		socket.emit(`${CLIENT_ADDED}-${user.id}`, {
			id: game.getId(),
			name: game.getName(),
			gameStarted: game.isStarted(),
			turn: game.getTurn(),
			users: game.getUsersToJSON(),
			gameState: game.getCurrentStateToJSON()
		})
		games.add(game)
		io.emit(SERVER_UPDATE, getUpdatedList())
		callback(game)
	})

	socket.on(UPDATE_LIST, ()=>{
		socket.emit(UPDATE_LIST, getUpdatedList())
	})
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
		gameStarted: game.isStarted(),
		turn: game.getTurn(),
		serverInfo: game.getServerInfo(),
		users: JSON.parse(game.getUsersToJSON()),
		gameState: game.getCurrentStateToJSON()
	}
}

/*
* Creates a json object that contains the state of all current
* games
*/
function getUpdatedList() {
	return {
		games: Array.from(games).map(game => {
				return {
					id: game.getId(),
					owner: game.getOwner(),
					name: game.getName(),
					gameStarted: game.isStarted(),
					turn: game.getTurn(),
					serverInfo: game.getServerInfo(),
					users: JSON.parse(game.getUsersToJSON()),
					gameState: game.getCurrentStateToJSON()
				}
		})
	}
}