function loadHomePage() {

    $.ajax(
                            {
                                type: "GET",
                                datatype: "xml",
                                url: "xmlConfig.xml",
                                data: {},
                                async: false,
                                cache: false,
                                success: function (xml) {
                                   // var output = $(xml).find("CurrentXML").first().text();
                                    //set last updated cookie
                                    if ($.cookie("LastRefreshTime") != null)
                                        $.cookie("LastRefreshTime", null);

                                    $.cookie("LastRefreshTime", $(xml).find("CreatedOn").text(), { expires: 1000, path: '/' });

                                    buildHomePage(xml);
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    alert(errorThrown);
                                }
                            });

                        }

                        function buildHomePage(xml) {
                            try {
                                var xmlLocation = $(xml).find("CurrentXML").first().text();
                                //get current XML File
                                buildScorecardMenu(xml);
                                $.ajax(
                                {
                                    type: "GET",
                                    datatype: "xml",
                                    async: false,
                                    cache: false,
                                    url: xmlLocation,
                                    data: {},
                                    success: function (xml) { xmlParser(xml); },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        alert(errorThrown);
                                    }
                                });
                            }
                            catch (e) {
                                window.alert("Unable to load the requested file.");
                                return;
                            }
                        }

                        
function xmlParser(xml) {
    LoadMenu(xml);
    EventInfo(xml);
    TeamsGolfersRoundsHoles(xml);
    Leaderboard(xml);
    TeamTop2(xml);
    BestBall(xml, 'Thurs');
    BestBall(xml, 'Fri');
    Skins(xml, 'Sat');
    Skins(xml, 'Sun');
    buildDraftResults(xml);
         
}

function buildScorecardMenu(xml) {
    var EventId = $(xml).find("EventId").first().text();
    if ($(xml).find("Scorecard").length == 0)
        $('#scorecardHeader').text("No Scorecards Posted");

    $(xml).find("Scorecard").each(
                        function () {
                            $('#ulpopupScorecards').append($('<li/>', {})
                                             .append($('<a/>',
                                                {
                                                    'href': $(this).find("url").text(),
                                                    'data-transition': 'slide',
                                                    'data-rel': 'dialog',
                                                    'data-position-to': 'window',
                                                    'data-theme': 'b',
                                                    'data-inline': 'true',
                                                    'text': $(this).find("DayName").text()
                                                })));
                        });
}

function LoadMenu(xml) {
    var MenuOrder = GetMenuOrder(xml);
    for (var i = 0; i < MenuOrder.length; i++) {
        switch (MenuOrder[i]) {
            case "DraftRoom":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('/DraftRoom/Draft.html')"
                                 }).append($('<a/>',
                                 {
                                     "href": "/DraftRoom/Draft.html",
                                     "data-transition": "fade",
                                     "text": "Draft Room"
                                 })).append($('<p/>',
                                 {
                                     "id": "DraftRoomOnTheClock"
                                 }).css("padding-left", "30px")));
                break;
            case "DraftResults":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#draftResults')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#draftResults",
                                     "data-transition": "slide",
                                     "text": "Draft Results"
                                 })).append($('<p/>',
                                 {
                                     "id": "LastDraftPick"
                                 }).css("padding-left", "30px")));

                break;
            case "Leaderboard":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#leaderboard')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#leaderboard",
                                     "data-transition": "slide",
                                     "text": "Leaderboard"
                                 })).append($('<p/>',
                                 {
                                     "id": "LeaderboardLeader"
                                 }).css("padding-left", "30px")));

                break;
            case "TeamTop2":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#teamTop2')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#teamTop2",
                                     "data-transition": "slide",
                                     "text": "Team Top 2"
                                 })).append($('<p/>',
                                 {
                                     "id": "TeamTop2Leader"
                                 }).css("padding-left", "30px")));

                break;

            case "BBThurs":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#bbThurs')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#bbThurs",
                                     "data-transition": "slide",
                                     "text": "Best Ball Thursday"
                                 })).append($('<p/>',
                                 {
                                     "id": "BBThursLeader"
                                 }).css("padding-left", "30px")));

                break;

            case "BBFri":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#bbFri')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#bbFri",
                                     "data-transition": "slide",
                                     "text": "Best Ball Friday"
                                 })).append($('<p/>',
                                 {
                                     "id": "BBFriLeader"
                                 }).css("padding-left", "30px")));

                break;

            case "SkinsSat":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#SkinsSat')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#SkinsSat",
                                     "data-transition": "slide",
                                     "text": "Skins Saturday"
                                 })).append($('<span/>',
                                 {
                                     "id": "SkinsTotalSat",
                                     "class": "ui-li-count"
                                 }).css("padding-left", "30px")));

                break;

            case "SkinsSun":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('#SkinsSun')"
                                 }).append($('<a/>',
                                 {
                                     "href": "#SkinsSun",
                                     "data-transition": "slide",
                                     "text": "Skins Sunday"
                                 })).append($('<span/>',
                                 {
                                     "id": "SkinsTotalSun",
                                     "class": "ui-li-count"
                                 }).css("padding-left", "30px")));

                break;

        }
    }
}
function GetMenuOrder(xml) {
    //debugger;
    var d = new Date();
    var MenuOrder = new Array(8);
    var EventStatus = $(xml).find("EventStatusName").first().text();
    var SkinsAmount = $(xml).find("SkinsAmount").first().text();
    //check to see if the draft is over, if so don't show the draft room.
    switch (d.getDay()) {
        case 0:
            if (SkinsAmount != "0.0000") {
                MenuOrder[0] = "SkinsSun";
                MenuOrder[1] = "TeamTop2";
                MenuOrder[2] = "Leaderboard";
                MenuOrder[3] = "SkinsSat";
                MenuOrder[4] = "BBThurs";
                MenuOrder[5] = "BBFri";
            }
            else {
                MenuOrder[1] = "TeamTop2";
                MenuOrder[2] = "Leaderboard";
                MenuOrder[4] = "BBThurs";
                MenuOrder[5] = "BBFri";
            }
            //  MenuOrder[6] = "DraftRoom";
            //MenuOrder[7] = "DraftResults";
            break;
        case 1:
            if (EventStatus != "Draft Completed")
                MenuOrder[0] = "DraftRoom";

            MenuOrder[1] = "DraftResults";
            break;
        case 2:
        case 3:
            if (EventStatus != "Draft Completed")
                MenuOrder[0] = "DraftRoom";

            MenuOrder[1] = "DraftResults";
            break;
        case 4:
            //MenuOrder[0] = "DraftRoom";
            //MenuOrder[1] = "DraftResults";
            MenuOrder[2] = "BBThurs";
            MenuOrder[3] = "TeamTop2";
            MenuOrder[4] = "Leaderboard";
            //                                 MenuOrder[5] = "BBFri";
            //                                 MenuOrder[6] = "SkinsSat";
            //                                 MenuOrder[7] = "SkinsSun";
            break;
        case 5:
            MenuOrder[0] = "BBFri";
            MenuOrder[1] = "TeamTop2";
            MenuOrder[2] = "Leaderboard";
            MenuOrder[3] = "BBThurs";
            //                                 MenuOrder[4] = "SkinsSat";
            //                                 MenuOrder[5] = "SkinsSun";
            //MenuOrder[6] = "DraftRoom";
            //MenuOrder[7] = "DraftResults";
            break;
        case 6:
            if (SkinsAmount != "0.0000") {
                MenuOrder[0] = "SkinsSat";
                MenuOrder[1] = "TeamTop2";
                MenuOrder[2] = "Leaderboard";
                MenuOrder[3] = "BBFri";
                MenuOrder[4] = "BBThurs";
            }
            else {
                MenuOrder[1] = "TeamTop2";
                MenuOrder[2] = "Leaderboard";
                MenuOrder[3] = "BBFri";
                MenuOrder[4] = "BBThurs";
            }
            //MenuOrder[5] = "SkinsSun";
            //MenuOrder[6] = "DraftRoom";
            //MenuOrder[7] = "DraftResults";
            break;
    }
    return MenuOrder;
}