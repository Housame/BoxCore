var input = "";

$(document).ready(function () {

    $("#search-input").keyup(function (e) {

        if (e.keyCode === 8 && input !== "")
            input = input.substring(0, input.length - 1);
        else if (e.keyCode === 8 && input.length === 0)
            input = "";
        else
            input += String.fromCharCode(e.which);

        $.ajax({
            url: "UserClient/FilterUser",
            type: "GET",
            data: { "input": input },
            success: function (users)
            {
                $("#filter-users").html(users);

                //$("#filter-users").empty();

                //if (users.length == 0)
                //{
                //    $("#filter-users").append('<a href="#" data-toggle="modal" data-target="#login-modal" class="list-group-item list-group-item-action">' + "No user found" + '</a>');
                //}
                //else
                //{
                //    $.each(users, function (i, user) {
                //        $("#filter-users").append('<a href="#" data-toggle="modal" data-target="#login-modal" class="list-group-item list-group-item-action">' + user.firstName + " " + user.lastName + '</a>');
                //    });
                //}

            },
            error: function () {
                alert("Error loading users");
            },
        });
    });

});