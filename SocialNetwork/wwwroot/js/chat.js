const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", (senderId, content) => {
    const messageElement = document.createElement("div");
    messageElement.textContent = `User ${senderId}: ${content}`;
    document.getElementById("messagesList").appendChild(messageElement);
});

connection.start().catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", () => {
    const chatId = document.getElementById("chatId").value;
    const senderId = document.getElementById("senderId").value;
    const content = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", chatId, senderId, content).catch(err => console.error(err.toString()));
});
