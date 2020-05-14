$(document).ready(function () {

    //Show or hide description.
    $("#switcherController").click(function () {
        if ($(this).hasClass("expand")) {
            $(this).removeClass('expand');
            $(this).addClass("collapse");
            $("#switcherContent").hide();
        } else {
            $(this).removeClass('collapse');
            $(this).addClass("expand");
            $("#switcherContent").show();
        }
    });
});
