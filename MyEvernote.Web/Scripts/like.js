$(function () {
    var noteids = [];
    $("div[data-note-id]").each(function (i, e) {
        noteids.push($(e).data("note-id"));
    });
    $.ajax({
        method: "POST",
        url: "/Note/GetLiked",
        data: { ids: noteids }
    }).done(function (data) {
        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                var likedNote = $("div[data-note-id=" + id + "]");
                var btn = likedNote.find("button[data-liked]");
                var span = btn.children().first();
                btn.data("liked", true);
                span.removeClass("fa-thumbs-o-up");
                span.addClass("fa-thumbs-up");
            }
        }
    }).fail(function () {

    });


    $("button[data-liked]").click(function () {
        var btn = $(this);
        var liked = btn.data("liked");
        var noteid = btn.data("note-id");
        var spanStar = btn.find(".like-star");
        var spancount = btn.find("span.like-count");

        $.ajax({
            method: "POST",
            url: "/Note/SetLikeState",
            data: { "noteid": noteid, "liked": !liked }
        }).done(function (data) {

            if (data.hasError) {
                alert(data.errorMesssage);
            } else {
                liked = !liked;
                btn.data("liked", liked);
                spancount.text(data.result);

                spanStar.removeClass("fa fa-thumbs-o-up");
                spanStar.removeClass("fa fa-thumbs-up");

                if (liked) {
                    spanStar.addClass("fa fa-thumbs-up");
                } else {
                    spanStar.addClass("fa fa-thumbs-o-up");
                }
            }

        }).fail(function () {
            alert("Beğenmek için önce giriş yapmalısınız..");
        });
    });

});
