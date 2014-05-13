function buildDraftResults(xml) {
    var EventStatus = $(xml).find("EventStatusName").first().text();
    $(xml).find("EventDraft").sort(function (a, b) {
        var aDraftPick = parseInt(a.getElementsByTagName("EventDraftPickNumber")[a.getElementsByTagName("EventDraftPickNumber").length - 1].childNodes[0].nodeValue);
        var bDraftPick = parseInt(b.getElementsByTagName("EventDraftPickNumber")[b.getElementsByTagName("EventDraftPickNumber").length - 1].childNodes[0].nodeValue);

        if (aDraftPick > bDraftPick) return (EventStatus == "Draft Completed" ? 1 : -1);
        if (aDraftPick < bDraftPick) return (EventStatus == "Draft Completed" ? -1 : 1);
        return 0
    }).each(
                        function () {
                            $("#ulDraftResults")
                            .append($('<li/>', {})
//                                .append($('<a/>',
//                                {
//                                    'href': '#',
//                                    'data-transition': 'slide'
//                                })
                                .append($('<img/>',
                                {
                                    'src': $(this).find("GolferImgUrl").text()
                                }))
                                .append($('<h3/>',
                                {
                                    'text': $(this).find("EventDraftPickNumber").text() + ". " + $(this).find("EventDraftGolfer").text() + " (" + $(this).find("Odds").text() + "/1" + ")"
                                }))
                                .append($('<p/>',
                                {
                                    'text': $(this).find("EventDraftGolfTeam").text()
                                }))
                                .append($('<p/>',
                                {
                                    'text': "Pick Duration: " + $(this).find("EventDraftPickDuration").text()
                                })));

                            $("#ulDraftResults").listview('refresh');
                        })
}