function EventInfo(xml) {
   // debugger;
    if ($(xml).find("EventId").text() != $.cookie("GolfEventId")) {
        if ($.cookie("GolfEventId") != null)
            $.cookie("GolfEventId", null);

        $.cookie("GolfEventId", $(xml).find("EventId").text(), { expires: 1000, path: '/' });
    }

    if ($(xml).find("EventName").text() != $.cookie("GolfEventName")) {
        if ($.cookie("GolfEventName") != null)
            $.cookie("GolfEventName", null);

        $.cookie("GolfEventName", $(xml).find("EventName").text(), { expires: 1000, path: '/' });
    }

    //$('#footerRefresh').text("Last Update: " + $.cookie("LastRefreshTime"));

    if ($(xml).find("GolfTeamOnTheClock").first().text() != "")
        $('#DraftRoomOnTheClock').text("On the clock: " + $(xml).find("GolfTeamOnTheClock").first().text());

    $('#LastDraftPick').text($(xml).find("LastDraftPick").first().text());
    $('#Title').text($(xml).find("EventName").text());
    
}