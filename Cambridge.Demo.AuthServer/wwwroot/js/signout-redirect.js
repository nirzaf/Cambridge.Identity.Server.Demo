$(function () {
    var redirectAfterLogoutUrl = $('#PostLogoutRedirectUri');
    window.location = redirectAfterLogoutUrl[0].value;
});