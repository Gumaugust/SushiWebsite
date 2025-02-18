// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
<script>
    document.addEventListener("DOMContentLoaded", function () {
        var navbarToggler = document.querySelector(".navbar-toggler");
    var navbarCollapse = document.querySelector("#navbarNav");

    navbarToggler.addEventListener("click", function () {
            if (navbarCollapse.classList.contains("show")) {
        navbarCollapse.style.maxHeight = "0px";
                setTimeout(() => {
        navbarCollapse.classList.remove("show");
                }, 300); // Đợi 300ms để hiệu ứng chạy xong rồi mới ẩn
            } else {
        navbarCollapse.classList.add("show");
    navbarCollapse.style.maxHeight = navbarCollapse.scrollHeight + "px";
            }
        });
    });
</script>





