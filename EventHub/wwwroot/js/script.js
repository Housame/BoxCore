$(document).ready(function() {
    $('.signInBtn').prop('disabled', true);

    $('.input').keyup(function () {
        var inputName = $('#input-name').val();
        var inputPassword = $('#input-password').val();
        if (inputName !== '' && inputPassword !== '') {
            $('.signInBtn').attr('disabled', false);
        } else {
            $('.signInBtn').attr('disabled', true);
        }
    });
});