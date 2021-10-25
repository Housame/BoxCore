$(document).ready(function () {
    $(".social-media").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '1' }, "slow");
    });
    $(".social-media").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '0.5' }, "slow");
    });
    $("#syntax-copy").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '1' }, "slow");
    });
    $("#syntax-copy").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '0.5' }, "slow");
    });
    $("#logo").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '1' }, "slow");
    });
    $("#logo").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '0.5' }, "slow");
    });
    $("#indexDiv").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '1' }, "slow");
    });
    $("#indexDiv").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '0.5' }, "slow");
    });
    //$("#scroll-down").mouseenter(function () {
    //    var div = $(this);
    //    div.stop(true, false);
    //    div.animate({ opacity: '1' }, "slow");
    //});
    //$("#scroll-down").mouseleave(function () {
    //    var div = $(this);
    //    div.stop(true, false);
    //    div.animate({ opacity: '0.5' }, "slow");
    //});
    $("#scroll-up").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '1' }, "slow");
    });
    $("#scroll-up").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        div.animate({ opacity: '0.5' }, "slow");
    });
    $("#contact-glyph, #event-glyph").mouseenter(function () {
        var div = $(this);
        div.stop(true, false);
        $(div).animate({
            "font-size": "115px",
        }, 200);
    });
    $("#contact-glyph, #event-glyph").mouseleave(function () {
        var div = $(this);
        div.stop(true, false);
        $(div).animate({
            "font-size": "80px",
        }, 200);
    });
    $(document).on('click', '.navbar-collapse.in', function (e) {
        if ($(e.target).is('a')) {
            $(this).collapse('hide');
        }
    });
    $(".navbar a, footer a[href='#top']").on('click', function (event) {
        if (this.hash !== "") {
            event.preventDefault();
            var hash = this.hash;
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, 900, function () {
                window.location.hash = hash;
            });
        }
    });
    $(window).scroll(function () {
        $(".slideanim").each(function () {
            var pos = $(this).offset().top;

            var winTop = $(window).scrollTop();
            if (pos < winTop + 600) {
                $(this).addClass("slide");
            }
        });
    });
    $('a[href^="#"], #indexDiv').on('click', function (event) {

        var target = $(this.getAttribute('href'));

        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top
            }, 1000);
        }

    });
})
