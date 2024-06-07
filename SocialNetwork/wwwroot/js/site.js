$(document).ready(function () {
    // Navbar active link
    $('.navbar .nav-link').on('click', function () {
        $('.navbar .nav-link').removeClass('active');
        $(this).addClass('active');
    });

    // Friend and user actions
    $('.friend-link, .user-link, .request-link').on('click', function () {
        var userId = $(this).data('id');
        $.get('/Friendship/GetUserDetails', { id: userId }, function (data) {
            $('#userDetails').html(data);
        });
    });

    $('#acceptRequest').on('click', function () {
        var userId = $(this).data('id');
        $.post('/Friendship/AcceptRequest', { id: userId }, function () {
            location.reload();
        });
    });

    $('#declineRequest').on('click', function () {
        var userId = $(this).data('id');
        $.post('/Friendship/DeclineRequest', { id: userId }, function () {
            location.reload();
        });
    });

    $('#removeFriend').on('click', function () {
        var userId = $(this).data('id');
        $.post('/Friendship/RemoveFriend', { id: userId }, function () {
            location.reload();
        });
    });

    $('#sendRequest').on('click', function () {
        var userId = $(this).data('id');
        $.post('/Friendship/SendRequest', { id: userId }, function () {
            location.reload();
        });
    });

    // Search functionality
    $('#searchFriends').on('keyup', function () {
        var searchTerm = $(this).val().toLowerCase();
        $('.friends-list .list-group-item').each(function () {
            var friendName = $(this).text().toLowerCase();
            $(this).toggle(friendName.indexOf(searchTerm) !== -1);
        });
    });

    $('#searchAllUsers').on('keyup', function () {
        var searchTerm = $(this).val().toLowerCase();
        $('.all-users-list .list-group-item').each(function () {
            var Username = $(this).text().toLowerCase();
            $(this).toggle(Username.indexOf(searchTerm) !== -1);
        });
    });
});
