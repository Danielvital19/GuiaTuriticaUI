var sitesPopUp = document.getElementById('sites-PopUp');

function openSite(id) {
    openSitePopUp();

    $.ajax({
        url: "Administration/GetAllPlace",
        data: {id: id},
        type: "GET"
    }).done(function (response) {
        console.log(response);
    });
}

function openSitePopUp() {
    sitesPopUp.style.display = "block";
    sitesPopUp.classList.remove('animated', 'fadeOut');
    sitesPopUp.classList.add('animated', 'fadeIn');
}

function closeSitePopUp() {
    sitesPopUp.classList.remove('animated', 'fadeIn');
    sitesPopUp.classList.add('animated', 'fadeOut');
    sitesPopUp.style.display = "none";
}