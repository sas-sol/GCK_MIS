function SASLoad(elem) {

    if (SASLoaded(elem)) {
        return;
    }

    let __sas_loader_structure__ = `<div class="loader-parent sas-loader-box">
                <img src="/assets/static/images/loader.gif" class="loader-img-style">
            </div>`;

    $(elem).html(__sas_loader_structure__);
    let wid = $(elem).width();
    let hei = $(elem).height();
    if (hei < 80) {
        hei = 100;
    }
    $(elem).find('.sas-loader-box').width(wid);
    $(elem).find('.sas-loader-box').height(hei);
}

function SASUnLoad(elem) {
    $(elem).find('.sas-loader-box').remove();
}

function SASLoaded(elem) {
    return ($(elem).find('.sas-loader-box').length > 0);
}

function ShowFullScreenLoader() {
    $('#loader').removeClass('fadeOut');
}

function HideFullScreenLoader() {
    $('#loader').addClass('fadeOut');
}