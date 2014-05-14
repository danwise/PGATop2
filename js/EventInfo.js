function EventInfo(xml) {
   // debugger;
    if ($(xml).find("EventId").text() != window.localStorage["GolfEventId"]) {
        window.localStorage["GolfEventId"] = $(xml).find("EventId").text();
    }

    if ($(xml).find("EventName").text() != window.localStorage["GolfEventName"]) {
        window.localStorage["GolfEventName"] = $(xml).find("EventName").text();
    }

    //$('#footerRefresh').text("Last Update: " + $.cookie("LastRefreshTime"));

    if ($(xml).find("GolfTeamOnTheClock").first().text() != "")
        $('#DraftRoomOnTheClock').text("On the clock: " + $(xml).find("GolfTeamOnTheClock").first().text());

    $('#LastDraftPick').text($(xml).find("LastDraftPick").first().text());
    $('#Title').text($(xml).find("EventName").text());
    
}