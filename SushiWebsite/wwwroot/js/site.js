// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelector('.navbar-toggler').addEventListener('click', function () {
    var navbarCollapse = document.getElementById('navbarNav');
    navbarCollapse.classList.toggle('show');
});
document.addEventListener("scroll", function () {
    const navbar = document.querySelector(".navbar");
    if (window.scrollY > 50) {
        navbar.classList.add("shrink");
    } else {
        navbar.classList.remove("shrink");
    }
});





