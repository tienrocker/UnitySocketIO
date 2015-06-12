var io = require('socket.io')({
    transports: ['websocket'],
});

var i = 0;
var users = [];
var messageType = {
    ERROR : 0,
    CONNECT : 1,
    LOGIN : 2,
    NOTIFY : 3,
    CHAT : 4
};

function message(messagetype, from, content, log) {
    
    var result = {
        messagetype : null,
        from : null,
        content : null,
        log : null
    };
    
    result.messagetype = messagetype;
    result.from = from;
    result.content = content;
    result.log = log;
    
    return result;
}

io.on('connection', function (socket) {
    
    socket.on('login', function (user) {
        users[socket.id] = user;
        socket.emit('message', message(messageType.LOGIN, socket.id, user.username, '[login] ' + user.username));
        socket.broadcast.emit('message', message(messageType.LOGIN, null, user.username, '[login] ' + user.username));
    });
    
    socket.on('chat', function (content) {
        if (users.hasOwnProperty(socket.id)) {
			i++; 
			var messcontent = message(messageType.CHAT, users[socket.id].username, content.text, '[chat-no] ' + i);
			socket.emit('message', messcontent);
			socket.broadcast.emit('message', messcontent);
        } else {
            socket.emit('message', message(messageType.ERROR, socket.id, socket.id, '[chat-error] ' + socket.id));
        }
    });
    
    socket.on('disconnect', function () {
        if (users.hasOwnProperty(socket.id)) {
            delete users[socket.id];
        }
    });
});

io.attach(4567);
console.log('Server running @4567');