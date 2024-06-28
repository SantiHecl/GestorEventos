// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    var currentUrl = window.location.href;
    var specificUrl = "https://localhost:7233/Home";

    if (currentUrl === specificUrl) {
        var element = document.getElementById("foto-logo");
        if (element) {
            element.style.display = "block";
        }
    }
});

