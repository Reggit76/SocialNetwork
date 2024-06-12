$(document).ready(function () {
    // Обработка открытия модального окна для редактирования чата
    $('#editChatModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var chatId = button.data('chat-id');
        var chatName = button.data('chat-name');
        var chatDescription = button.data('chat-description');
        var chatIconUrl = button.data('chat-icon-url');

        var modal = $(this);
        modal.find('#editChatId').val(chatId);
        modal.find('#editChatName').val(chatName);
        modal.find('#editChatDescription').val(chatDescription);
        modal.find('#editChatIconUrl').val(chatIconUrl);
    });

    // Обработка создания нового чата
    $('#createChatForm').on('submit', function (event) {
        event.preventDefault();

        var formData = $(this).serialize();

        $.ajax({
            url: '/Chats/Create',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    $('#createChatModal').modal('hide');
                    location.reload();
                } else {
                    alert("Failed to create chat.");
                }
            }
        });
    });

    // Обработка сохранения изменений в чате
    $('#editChatForm').on('submit', function (event) {
        event.preventDefault();

        var formData = $(this).serialize();

        $.ajax({
            url: '/Chats/Edit',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    $('#editChatModal').modal('hide');
                    location.reload();
                } else {
                    alert("Failed to edit chat.");
                }
            }
        });
    });

    // Обработка удаления чата
    window.deleteChat = function (chatId) {
        if (confirm("Are you sure you want to delete this chat?")) {
            $.ajax({
                url: '/Chats/Delete',
                type: 'POST',
                data: { id: chatId },
                success: function (result) {
                    if (result.success) {
                        location.reload();
                    } else {
                        alert("Failed to delete chat.");
                    }
                }
            });
        }
    }
});
