// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const multipleItemCarousel = document.querySelector('#carouselExampleControls');

if (window.matchMedia("(min-width:576px)").matches) {
    const carousel = new bootstrap.Carousel(multipleItemCarousel,
        {
            interval: false
        })

    var carouselWidth = $('.carousel-inner')[0].scrollWidth;
    var cardWidth = $('.carousel-item').width();

    var scrollPosition = 0;

    $('.carousel-control-next').on('click', function () {
        if (scrollPosition < (carouselWidth - (cardWidth * 4))) {
            console.log('next');
            scrollPosition = scrollPosition + cardWidth;
            $('.carousel-inner').animate({scrollLeft: scrollPosition}, 600);
        }
    });

    $('.carousel-control-prev').on('click', function () {
        if (scrollPosition > 0) {
            console.log('prev');
            scrollPosition = scrollPosition - cardWidth;
            $('.carousel-inner').animate({scrollLeft: scrollPosition}, 600);
        }
    });
} else {
    $(multipleItemCarousel).addClass('slide');
}
$(document).ready(function () {
    // Number of cards to show initially
    var initialCards = 3;

    // Select the card container and the "See More" button
    var cardContainer = $('#cardContainer');
    var seeMoreBtn = $('#seeMoreBtn');

    // Hide extra cards initially
    cardContainer.find('.card:gt(' + (initialCards - 1) + ')').hide();

    // Show/hide cards on "See More" button click
    seeMoreBtn.click(function () {
        var hiddenCards = cardContainer.find('.card:hidden');

        if (hiddenCards.length === 0) {
            // No hidden cards, toggle back to initial display
            cardContainer.find('.card:gt(' + (initialCards - 1) + ')').hide();
            seeMoreBtn.text('See More');
        } else {
            // Toggle visibility of the hidden cards
            hiddenCards.show();

            // Update "See More" button text based on hidden cards
            if (hiddenCards.length > initialCards) {
                seeMoreBtn.text('See Less');
            } else {
                seeMoreBtn.text('See More');
            }
        }
    });
});

function goToTop() {
    window.scrollTo({top: 0, behavior: 'smooth'});
}

window.onscroll = function () {
    var topButton = document.getElementById("topButton");
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        topButton.style.display = "block";
    } else {
        topButton.style.display = "none";
    }
};