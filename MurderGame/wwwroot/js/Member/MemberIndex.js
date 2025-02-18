//document.addEventListener("DOMContentLoaded", function (event) {

//    const showNavbar = (toggleId, navId, bodyId, headerId) => {
//        const toggle = document.getElementById(toggleId),
//            nav = document.getElementById(navId),
//            bodypd = document.getElementById(bodyId),
//            headerpd = document.getElementById(headerId)

//        // Validate that all variables exist
//        if (toggle && nav && bodypd && headerpd) {
//            toggle.addEventListener('click', () => {
//                // show navbar
//                nav.classList.toggle('show')
//                // change icon
//                toggle.classList.toggle('bx-x')
//                // add padding to body
//                bodypd.classList.toggle('body-pd')
//                // add padding to header
//                headerpd.classList.toggle('body-pd')
//            })
//        }
//    }

//    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

//    /*===== LINK ACTIVE =====*/
//    const linkColor = document.querySelectorAll('.nav_link')

//    function colorLink() {
//        if (linkColor) {
//            linkColor.forEach(l => l.classList.remove('active'))
//            this.classList.add('active')
//        }
//    }
//    linkColor.forEach(l => l.addEventListener('click', colorLink))

//    // Your code to run since DOM is loaded and ready
//});

const mobileScreen = window.matchMedia("(max-width: 990px )");
$(document).ready(function () {
    $(".dashboard-nav-dropdown-toggle").click(function () {
        $(this).closest(".dashboard-nav-dropdown")
            .toggleClass("show")
            .find(".dashboard-nav-dropdown")
            .removeClass("show");
        $(this).parent()
            .siblings()
            .removeClass("show");
    });
    $(".menu-toggle").click(function () {
        if (mobileScreen.matches) {
            $(".dashboard-nav").toggleClass("mobile-show");
        } else {
            $(".dashboard").toggleClass("dashboard-compact");
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    document.body.addEventListener("click", function (event) {
        let card = event.target.closest(".clickable-card"); // En yakın clickable-card'ı bul
        if (card && !event.target.closest("a")) { // Eğer bir linke tıklanmadıysa
            window.location.href = card.getAttribute("data-url"); // Kartın yönlendirme URL'sine git
        }
    });
});
