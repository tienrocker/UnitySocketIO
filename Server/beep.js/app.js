var io = require('socket.io')({ transports: ['websocket'] }),
	users = [];

function message(username, message) {
    
    var result = {
        username : null,
        message : null
    };
    
    result.username = username;
    result.message = message;
    
    return result;
}

io.on('connection', function (socket) {

	socket.on('error', function (err) { console.error(err.stack);});
	
    socket.on('login', function (user, callback) {
		if(users[user.username] === undefined){
		
			socket.username = user.username;
			users[user.username] = socket;
		
			socket.broadcast.emit('join', message( user.username, "joined"));
			if(callback) callback(true);
		} else {
		
			if(callback) callback('Error! Username existed');
		}
    });
    
    socket.on('chat', function (data, callback) {
        if (socket.username !== undefined) {
			var messcontent = message(socket.username, data.text);
			io.sockets.emit('message', messcontent);	// send to all user include logged user
			if(callback) callback(true);
        } else {
            if(callback) callback('Error! Username not logged');
        }
    });

    socket.on('disconnect', function () {
		if(socket.username !== undefined && users[socket.username] === undefined) return;
		delete users[socket.username];
    });
	
});

io.attach(4567);
console.log('Server running @4567');