function showRightPopup(heading) {
    $('.right-popup').show();
    $('.right-pop-head').text(heading);
    $('.right-pop-body').empty();
    const element = document.querySelector('.right-popup');
    element.classList.add('animate__animated', 'animate__fadeInRight');
    element.addEventListener('animationend', shownRightPopUp, true);
}

function closeRightPopup() {
    const element = document.querySelector('.right-popup');
    element.classList.add('animate__animated', 'animate__fadeOutRight');
    element.addEventListener('animationend', hideRightPopUp, true);
}

function shownRightPopUp() {
    const element = document.querySelector('.right-popup');
    element.removeEventListener('animationend', shownRightPopUp, true);
    $(element).removeClass('animate__animated').removeClass('animate__fadeInRight');
}

function hideRightPopUp() {
    $('.right-popup').hide();
    const element = document.querySelector('.right-popup');
    element.removeEventListener('animationend', hideRightPopUp, true);
    $(element).removeClass('animate__animated').removeClass('animate__fadeOutRight');
}