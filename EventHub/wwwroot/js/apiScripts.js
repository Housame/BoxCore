$(document).ready(function () {
    var adminSaveButton = $("#adminSaveButton button");
    var boxesSelect = $("#boxesSelect");
    var claimsSelect = $('#sel1');
    var userForm = $("#userForm");
    var activeUser = $("#active-user");
    var firstName = $("#firstName");
    var lastName = $("#lastName");
    var gender = $("#gender");
    var dateOfBirth = $("#dateOfBirth");
    var location = $("#location");
    var size = $("#size");
    var email = $("#email");
    //var box = $("#box");
    var team = $("#team");
    var role = $("#role");
    var claim = $("#claim");
    var searchInput = $("#searchInput");

    var saveBtn = $(".admin-edit-button");
    var userToEditId = "";
    getClaims();
    getBoxes();
    editUsers();
    searchInput.on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#myTable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });


    function getClaims() {
        $.ajax({
            url: '/api/admin/getClaims',
            type: 'Get'
        }).done(function (result) {
            console.log('get claims success');
            console.log(result);
            $.each(result, function (key, value) {
                claimsSelect
                    .append($("<option></option>")
                        .attr("value", key)
                        .text(value));
            });
        }).fail(function (xhr, status, error) {
            console.log('error');
        });
    }

    function editUsers() {
        console.log("get users with their claims");
        $.ajax({
            url: '/api/admin/getusers',
            method: 'GET'
        }).done(function (result) {
            console.log(result);
            $('#customerTable').empty();
            $.each(result, function (i, item) {
                var $tr = $('<tr>').append(
                    $('<td>').text(item.user.email),
                    $('<td>').append(
                        $('<span>').text(item.role[0]),
                    ),

                    $('<td>').text(claimsGet(item.claims, item.boxName)),
                    $('<td>').addClass('admin-button-table').append(
                        $('<button>').attr('id', item.user.id).addClass('admin-edit-button btn btn-warning glyphicon glyphicon-pencil')
                    ),
                    $('<td>').addClass('admin-button-table').append(
                        $('<button>').attr('id', item.user.id).addClass('admin-remove-button btn btn-danger glyphicon glyphicon-trash')
                    )
                ).appendTo('#customerTable');

            });
        }).fail(function (xhr, status, error) {
            console.log('error');
        });
    }

    function getBoxes() {
        $.ajax({
            url: '/api/admin/getboxes',
            type: 'Get'
        }).done(function (result) {
            console.log('get boxes success');
            console.log(result);
            $.each(result, function (key, value) {
                boxesSelect
                    .append($("<option></option>")
                        .attr("value", key)
                        .attr("id", value.id)
                        .text(value.name));
            });
        }).fail(function (xhr, status, error) {
            console.log('error');
        });
    }

    // "Squeezing" out the claims from array to a string to fit the <td>
    function claimsGet(claims, boxName) {
        var strToReturn = "";

        $.each(claims, function (i, claim) {
            //if (claim.type === "Box" || claim.type === "EventManager") {
            //    strToReturn = claim.type + ": " + boxName;
            //}
            //else {
            if (i === 0) {
                if (claim.type === "EventManager" || claim.type == "Box") {
                    strToReturn = claim.type + ": " + boxName;
                }
                else {
                    strToReturn = claim.type + ": " + claim.value;
                }
            }
            else {

                if (claims[i - 1].type === claim.type) {
                    if (claim.type === "EventManager" || claim.type == "Box") {
                        strToReturn = strToReturn + ", " + ": " + boxName;
                    }
                    else {
                        strToReturn = strToReturn + ", " + claim.value;
                    }

                }
                else {
                    if (claim.type === "EventManager" || claim.type == "Box") {
                        strToReturn = strToReturn + " // " + claim.type + ": " + boxName;
                    }
                    else {
                        strToReturn = strToReturn + " // " + claim.type + ": " + claim.value;
                    }


                }
                //}
            }

        });
        //if (strToReturn.startsWith("//")) {
        //    strToReturn =  strToReturn.substr(0, 2);
        //}
        return strToReturn;
    }

    //Enable/Disable the second dropdownMenu when selecting EventManager role and th submit btn
    //$(document).on('change', '#role', function () {
    //    $(':input[type="submit"]').prop('disabled', true);
    //    if ($(this).val() === "Client") {
    //        $(':input[type="submit"]').prop('disabled', false);
    //        $('#claim').prop('disabled', false);
    //        console.log("claims menu enabled");
    //    }
    //    else {
    //        $('#claim').prop('disabled', true);
    //        $(':input[type="submit"]').prop('disabled', false);
    //        //$(".admin-edit-button").prop('disabled', false);
    //        console.log("claims menu disabled");
    //        console.log("save btn enabled");
    //    }

    //});

    //Getting user for changes
    $(document).on("click", ".admin-edit-button", function () {
        resetForm();
        $('#role').prop('disabled', false);
        var id = $(this).attr("id");
        console.log(id);
        console.log(activeUser);
        $.ajax({
            url: "/api/admin/" + id,
            type: "GET",
        }).done(function (user) {
            activeUser.text(user.user.email);
            if (user.bcUser != null) {
                user.bcUser.firstName != null ? firstName.val(user.bcUser.firstName) : firstName.val();
                user.bcUser.lastName != null ? lastName.val(user.bcUser.lastName) : lastName.val();
                user.bcUser.gender != null ? gender.val(user.bcUser.gender) : gender.val();
                var date;
                user.bcUser.dateOfBirth != null ? date = user.bcUser.dateOfBirth.split("T") : date = "";
                dateOfBirth.val(date[0]) != null ? dateOfBirth.val(date[0]) : dateOfBirth.val();
                user.bcUser.location != null ? location.val(user.bcUser.location) : location.val();
                user.bcUser.size != null ? size.val(user.bcUser.size) : size.val();
                user.bcUser.email != null ? email.val(user.bcUser.email) : email.val(user.user.email);
                user.bcUser.boxId != null ? boxesSelect.val(user.bcUser.boxId) : boxesSelect.val();
                user.bcUser.team != null ? team.val(user.bcUser.team) : team.val();
            }
            userToEditId = id;
            console.log(userToEditId);
            role.val(user.role[0]);
            if (user.claims.length > 0) {
                claim.val(user.claims[0].type);
            }
            else {
                claim.val("");
            }
            editUser();
        }).fail(function (xhr, status, error) {
            console.log(error);
        });
    });

    function editUser() {
        adminSaveButton.text("Spara ändring").removeClass("create").addClass("edit");
    }

    var claims = [];
    //Filling claims[] from the drop-down checkboxes
    $('.dropdown-menu a').on('click', function (event) {
        //$(':input[type="submit"]').prop('disabled', false);
        var $target = $(event.currentTarget),
            val = $target.attr('data-value'),
            $inp = $target.find('input'),
            idx;

        if ((idx = claims.indexOf(val)) > -1) {
            claims.splice(idx, 1);
            setTimeout(function () { $inp.prop('checked', false) }, 0);
        } else {
            claims.push(val);
            setTimeout(function () { $inp.prop('checked', true) }, 0);
        }

        $(event.target).blur();

        console.log(claims);
        return false;
    });

    $(document).on("click", ".create", function () {
        //var claims_to_send = $(claims).serializeObject();
        var user = {};
        user.firstName = firstName.val();
        user.lastName = lastName.val();
        user.gender = gender.val();
        user.dateOfBirth = dateOfBirth.val();
        user.location = location.val();
        user.size = size.val();
        user.email = email.val();
        user.boxId = boxesSelect.val();
        user.team = team.val();
        user.role = role.val();
        user.claims = claims;
        console.log(user);
        $.ajax({
            url: "/api/admin/",
            type: "POST",
            data: user
        }).done(function (result) {
            console.log(result);
        }).fail(function (xhr, status, error) {
            console.log(error);
        });
    });

    //Submitting changes
    $(document).on("click", ".edit", function () {
        var user = {};
        user.firstName = firstName.val();
        user.lastName = lastName.val();
        user.gender = gender.val();
        user.dateOfBirth = dateOfBirth.val();
        user.location = location.val();
        user.size = size.val();
        user.email = email.val();
        user.boxId = boxesSelect.find('option:selected').attr('id');;
        user.team = team.val();
        user.role = role.val();
        user.claims = claims;
        user.id = userToEditId;
        console.log(user);
        $.ajax({
            url: "/api/admin/",
            type: "PUT",
            data: user
        }).done(function (result) {
            console.log(result);
            adminSaveButton.text("Skapa användare").removeClass("edit").addClass("create");
            resetForm();
            editUsers();
        }).fail(function (xhr, status, error) {
            console.log(error);
        });
    });

    $(document).on("click", ".admin-remove-button", function () {
        var id = $(this).attr("id");
        $.ajax({
            url: "/api/admin/",
            type: "DELETE",
            data: { "id": id }
        }).done(function (result) {
            console.log(result);
            editUsers();
        }).fail(function (xhr, status, error) {
            console.log(error);
        });
    });

    function resetForm() {
        activeUser.text("Ingen användare vald.");
        userForm.find("input").val("");
        boxesSelect.val("- Välj box -");
        gender.val("- Välj kön -");
        size.val("- Välj storlek -");
        role.val("- Välj roll -");
    }
});