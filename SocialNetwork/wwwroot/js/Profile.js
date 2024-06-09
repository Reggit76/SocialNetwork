// profile.js

$(document).ready(function () {
    // Edit profile AJAX
    $('#editProfileForm').submit(function (event) {
        event.preventDefault();
        $.ajax({
            url: '/Profile/Edit',
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error updating profile.');
                }
            }
        });
    });
});
