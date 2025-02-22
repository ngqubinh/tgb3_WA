const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", (email, message) => {
    const msg = document.createElement("div");
    msg.classList.add("list-group-item");
    msg.innerHTML = `<strong>${email}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(msg);
});

connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(err => console.error("Connection start error:", err.toString()));

document.getElementById("messageForm").addEventListener("submit", event => {
    event.preventDefault();

    const messageInput = document.getElementById("messageInput").value;
    const receiverEmail = "nguyenbinh031104@gmail.com"; // Admin email

    if (!messageInput) {
        console.error("Message input is empty.");
        return;
    }

    connection.invoke("SendMessage", receiverEmail, messageInput)
        .then(() => {
            console.log("Message sent successfully.");
        })
        .catch(err => {
            console.error("SendMessage error:", err.toString());
        });

    document.getElementById("messageInput").value = ''; // Clear the input field after sending
});
