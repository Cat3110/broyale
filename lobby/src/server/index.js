var app = require('http').createServer()
var io = module.exports.io = require('socket.io')(app)

const PORT = process.env.PORT || 3231

const SocketManager = require('./SocketManager')

io.on('connection', SocketManager)

app.listen(PORT, ()=>{
	console.log("Connected to port:" + PORT);
})

const express = require("express")
const bodyParser = require("body-parser")

const expressApp = express()
expressApp.use(bodyParser.json())


//expressApp.use("/admin", require("./admin"))

const httpServerPort = 3000
expressApp.listen(httpServerPort, () => {
  console.log(`🔥  Server started on PORT: ${httpServerPort}`)
})

const mongoose = require("mongoose")
const uniqueValidator = require('mongoose-unique-validator')

const userSchema = mongoose.Schema({
	name: { type: String},

	email: { type: String },
	password: { type: String },

	avatar_url: { type: String },

	bio: { type: String },	

	device_id: { type: String, required: true, unique: true },

	level: { type: Number, default: 0 },
	progress: { type: Number, default: 0.0, min: 0.0, max: 1.0 },

	coins: { type: Number, default: 0 },
	crystals: { type: Number, default: 0 },

	role: { type: String, enum: ['user','admin'], default: 'user' },

	last_login_at: { type: Date, default: Date.now() },

	created_at: { type: Date, default: Date.now() },
	updated_at: { type: Date, default: Date.now() }	
  })

// Update the updated_at field on save
userSchema.pre("save", next => {
  this.updated_at = Date.now()
  next()
})

userSchema.plugin(uniqueValidator)

const characterSchema = mongoose.Schema({
	user_id: { type: Object },

	sex: { type: String, enum : ['male','female'], default: 'male' },	
	skin_id: { type: String },

	head_type: { type: Number, default: 0 },
	body_type: { type: Number, default: 0 },
	pants_type: { type: Number, default: 0 },

	head_color: { type: String, default: "#000000" },
	body_color: { type: String, default: "#000000" },
	pants_color: { type: String, default: "#000000" },

	skill_set: { type: Map,	of: String, default: { main_skill: "ID_Melee" }  },	

	created_at: { type: Date, default: Date.now() },
	updated_at: { type: Date, default: Date.now() },
  })

// Update the updated_at field on save
characterSchema.pre("save", next => {
  this.updated_at = Date.now()
  next()
})

const gameSchema = mongoose.Schema({any: {}},{ strict:false })

// Update the updated_at field on save
gameSchema.pre("save", next => {
  this.updated_at = Date.now()
  next()
})

module.exports = User = mongoose.model("User", userSchema)
module.exports = Character = mongoose.model("Character", characterSchema)
module.exports = Games = mongoose.model("Game", gameSchema)

// Import MongoDB
//const MONGODB_URI = "mongodb://127.0.0.1:44158/e38700ac-908e-49e7-af5d-75af91a32445?";
const MONGODB_URI = "mongodb://127.0.0.1:27017";

// Connect to MongoDB
mongoose
  .connect(MONGODB_URI, { useNewUrlParser: true })
  .then(() => console.log("🔥  MongoDB Connected..."))
  .catch(err => console.log(err))

  expressApp.get('/games', async (req, res) => {
	const games = await Games.find({})
	res.send(games)
  })

expressApp.get('/game/:id/short', async (req, res) => {
	const game = await Games.find( {gid:req.params.id })
	res.send(game)
})

expressApp.get('/game/:id/full', async (req, res) => {
	var game = await Games.findOne( {gid:req.params.id })
	var characters = []
	for (let index = 0; index < game._doc.players.length;index++) {
		var user = game._doc.players[index]
		var id = mongoose.Types.ObjectId(user._id);
		const dbcharacters = await Character.find({user_id:id})	
		characters.push(dbcharacters[0])
	}
	game.characters = characters
	res.send({game: game, characters: characters})
})


expressApp.get('/characters', async (req, res) => {
	const characters = await Character.find({})
	res.send(characters)
 })

 expressApp.get('/character/:id', async (req, res) => {
	const characters = await Character.find({_id: req.params.id})
	res.send(characters)
 })



 expressApp.get('/users', async (req, res) => {
	const users = await User.find({})
	res.send(users)
  })

expressApp.get('/user/:device_id', async (req, res) => {
	const users = await User.find({ device_id: req.params.device_id })	
	res.send(users)
  })

  expressApp.post('/users', async (req, res) => {
	var dummyUser = { device_id: req.body.device_id }
	const user = await new User(dummyUser).save()
	res.send(user)
  })

