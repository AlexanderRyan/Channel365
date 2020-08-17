var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var _connectionId = '';

connection.on("RecieveMessage", function (data) {

    console.log(data);

    var message = document.createElement("div")
    message.classList.add('message')


    var header = document.createElement("div")
    header.appendChild(document.createTextNode(data.name))


    var p = document.createElement("p")
    p.appendChild(document.createTextNode(data.commentBody))

    var footer = document.createElement("footer")
    footer.appendChild(document.createTextNode(data.postedAt))

    message.appendChild(header);
    message.appendChild(p);
    message.appendChild(footer);

    document.querySelector('.chat-body').append(message);


})



var joinRoom = function () {
    var url = '/Chat/WatchVideo/' + _connectionId + '/@Model.Name'
    axios.post(url, null)
        .then(res => {
            console.log('Room Joined!', res);
        })
        .catch(err => {
            Console.log('Failed to join');
        })
}

connection.start()
    .then(function () {
        connection.invoke('getConnectionId')
            .then(function (connectionId) {
                _connectionId = connectionId
                joinRoom();
            })
    })
    .catch(function (err) {
        console.log(err)
    })


var sendMessage = function (event) {
    event.preventDefault();
    form = event.target;

    var data = new FormData(event.target);
    axios.post("/Chat/SendMessage", data)
        .then(res => {
            console.log("Message sent", res)
        })
        .catch(err => {
            console.log("It aint working")
        })
}

