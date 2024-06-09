// main.js

$(document).ready(function () {
    // Initialize Select2
    $('#participantSelect').select2({
        width: '100%',
        placeholder: 'Select Participants'
    });

    // Submit create chat form via AJAX
    $('#createChatForm').submit(function (event) {
        event.preventDefault();
        $.ajax({
            url: '/Chats/Create',
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#createChatModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error creating chat.');
                }
            }
        });
    });

    // Like post
    $('.like-post-btn').click(function () {
        const postId = $(this).data('post-id');
        $.post('/News/LikePost', { postId: postId }, function (response) {
            if (response.success) {
                $(`#likes-count-${postId}`).text(response.newLikesCount);
            } else {
                alert('An error occurred while liking the post.');
            }
        });
    });

    // Dislike post
    $('.dislike-post-btn').click(function () {
        const postId = $(this).data('post-id');
        $.post('/News/DislikePost', { postId: postId }, function (response) {
            if (response.success) {
                $(`#likes-count-${postId}`).text(response.newLikesCount);
            } else {
                alert('An error occurred while disliking the post.');
            }
        });
    });

    // Submit comment form via AJAX
    $('.comment-form').submit(function (event) {
        event.preventDefault();
        const form = $(this);
        const postId = form.data('post-id');
        const content = form.find('textarea[name="Content"]').val();

        $.post('/News/AddComment', { postId: postId, content: content }, function (response) {
            if (response.success) {
                const comment = `
                    <div class="comment mb-2">
                        <img src="${response.comment.UserProfilePictureUrl ?? '~/images/default-avatar.jpg'}" alt="Avatar" class="post-avatar-small">
                        <strong>${response.comment.UserFullName}</strong>
                        <small>${new Date(response.comment.DatePosted).toLocaleString()}</small>
                        <p>${response.comment.Content}</p>
                    </div>`;
                form.before(comment);
                form.find('textarea[name="Content"]').val('');
            } else {
                alert('An error occurred while adding the comment.');
            }
        });
    });
});
