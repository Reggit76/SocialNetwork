// chats.js

$(document).ready(function () {
    // Join chat group via SignalR
    const chatId = $('#chatId').val();
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    connection.start().then(function () {
        connection.invoke("JoinChat", chatId).catch(function (err) {
            return console.error(err.toString());
        });
    });

    // Send message
    $('#sendMessageForm').submit(function (event) {
        event.preventDefault();
        const message = $('#messageInput').val();
        connection.invoke("SendMessage", chatId, message).catch(function (err) {
            return console.error(err.toString());
        });
        $('#messageInput').val('');
    });

    // Receive message
    connection.on("ReceiveMessage", function (user, message) {
        const msg = `<div class="chat-message">
            <strong>${user}</strong>
            <p>${message}</p>
        </div>`;
        $('#chatMessages').append(msg);
    });
});
