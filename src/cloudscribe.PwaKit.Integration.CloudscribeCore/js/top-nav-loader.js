navLoaded = function (xhr) {
    jQuery.SmartMenus.Bootstrap.init();
    var logoutLink = document.getElementById("lnkLogout");
    if (logoutLink) {
        logoutLink.addEventListener('click', function (event) {
            var logoutForm = document.getElementById("logoutForm");
            if (logoutForm) {
                logoutForm.submit();
            }
            event.preventDefault();
        }, false);
    }
};
navLoadFail = function (xhr) {
    console.log('nav menu ajax failed');
    var loginNav = document.getElementById("ulLoginMenu");
    if (loginNav) {
        loginNav.style.display = 'none';
    }
    var navOffline = document.getElementById('navMenuOffline');
    if (navOffline) {
        navMenuOffline.style.display = 'block';
    }
};
document.addEventListener("DOMContentLoaded", function () {
    var btnNav = document.getElementById("btnGetNav");
    if (btnNav) {
        btnNav.click();
    }
});