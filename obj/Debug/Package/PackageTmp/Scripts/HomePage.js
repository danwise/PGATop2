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
                            $('.ui-page-active .ui-listview').listview('refresh');
                        });
}


function LoadMenu2(xml) {
   // debugger;
    $(xml).find("LeagueProduct")
     .each(function () {
         switch ($(this).find("ProductNumber").text()) {
             default:
                 $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('" + $(this).find("url").text() + "')"
                                 }).append($('<a/>',
                                 {
                                     "href": $(this).find("url").text(),
                                     "data-transition": "slide",
                                     "text": $(this).find("ProductName").text(),
                                     "rel": "external"
                                 })).append($('<p/>',
                                 {
                                     "text": $(this).find("Leader").text()
                                 }).css("padding-left", "30px")));

                 break;

             case "SkinsSat":
             case "SkinsSun":
                 $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('" + $(this).find("url").text() + "')"
                                 }).append($('<a/>',
                                 {
                                     "href": $(this).find("url").text(),
                                     "data-transition": "slide",
                                     "text": $(this).find("ProductName").text(),
                                     "rel": "external"
                                 }))
                                 .append($('<p/>',
                                 {
                                     "text": $(this).find("Leader").text()
                                 }).css("padding-left", "30px")
                                 .append($('<span/>',
                                 {
                                     "text": $(this).find("Count").text(),
                                     "class": "ui-li-count"
                                 }).css("padding-left", "10px"))));

                 break;
             case "36Ind":
             case "36TT2":
                 $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('" + $(this).find("url").text() + "')"
                                 }).append($('<h1/>',
                                 {
                                     "text": $(this).find("ProductName").text()
                                 }))
                                 .append($('<p/>',
                                 {
                                     "text": $(this).find("Leader").text()
                                 }).css("padding-left", "30px")));

                 break;

         }
     });
    
    $('.ui-page-active .ui-listview').listview('refresh');
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
                                     "text": "Draft Room",
                                     "rel" : "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "DraftRoomOnTheClock"
                                 }).css("padding-left", "30px")));
                break;
            case "DraftResults":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('DraftResults.htm')"
                                 }).append($('<a/>',
                                 {
                                     "href": "DraftResults.htm",
                                     "data-transition": "slide",
                                     "text": "Draft Results", 
                                     "rel" : "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "LastDraftPick"
                                 }).css("padding-left", "30px")));

//                                 $.mobile.loadPage('DraftResults.htm', { showLoadMsg: false });

                break;
            case "Leaderboard":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('Leaderboard.htm')"
                                 }).append($('<a/>',
                                 {
                                     "href": "Leaderboard.htm",
                                     "data-transition": "slide",
                                     "text": "Leaderboard",
                                     "rel" : "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "LeaderboardLeader"
                                 }).css("padding-left", "30px")));

//                                  $.mobile.loadPage('Leaderboard.htm', { showLoadMsg: false });
                break;
            case "TeamTop2":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('teamTop2.htm')"
                                 }).append($('<a/>',
                                 {
                                     "href": "teamTop2.htm",
                                     "data-transition": "slide",
                                     "text": "Team Top 2",
                                     "rel": "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "TeamTop2Leader"
                                 }).css("padding-left", "30px")));

//                                 $.mobile.loadPage('teamTop2.htm', { showLoadMsg: false });
                                 break;

                            case "Top4":
                                $('#HomeMenu').append($('<li/>',
                                {
                                    "onclick": "javascript:liClick('Top4.htm')"
                                }).append($('<a/>',
                                {
                                    "href": "Top4.htm",
                                    "data-transition": "slide",
                                    "text": "Top 4",
                                    "rel": "external"
                                })).append($('<p/>',
                                {
                                    "id": "Top4Leader"
                                }).css("padding-left", "30px")));
                                break;

            case "BBThurs":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('BestBall.htm?Day=Thurs')"
                                 }).append($('<a/>',
                                 {
                                     "href": "BestBall.htm?Day=Thurs",
                                     "data-transition": "slide",
                                     "text": "Best Ball Thursday",
                                     "rel" : "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "BBThursLeader"
                                 }).css("padding-left", "30px")));

                                 
                break;

            case "BBFri":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('BestBall.htm?Day=Fri')"
                                 }).append($('<a/>',
                                 {
                                     "href": "BestBall.htm?Day=Fri",
                                     "data-transition": "slide",
                                     "text": "Best Ball Friday",
                                     "rel": "external"
                                 })).append($('<p/>',
                                 {
                                     "id": "BBFriLeader"
                                 }).css("padding-left", "30px")));

                break;

            case "SkinsSat":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('Skins.htm?Day=Sat')"
                                 }).append($('<a/>',
                                 {
                                     "href": "Skins.htm?Day=Sat",
                                     "data-transition": "slide",
                                     "text": "Skins Saturday",
                                     "rel": "external"
                                 })).append($('<span/>',
                                 {
                                     "id": "SkinsTotalSat",
                                     "class": "ui-li-count"
                                 }).css("padding-left", "10px")));

                break;

            case "SkinsSun":
                $('#HomeMenu').append($('<li/>',
                                 {
                                     "onclick": "javascript:liClick('Skins.htm?Day=Sun')"
                                 }).append($('<a/>',
                                 {
                                     "href": "Skins.htm?Day=Sun",
                                     "data-transition": "slide",
                                     "text": "Skins Sunday",
                                     "rel": "external"
                                 })).append($('<span/>',
                                 {
                                     "id": "SkinsTotalSun",
                                     "class": "ui-li-count"
                                 }).css("padding-left", "10px")));

                break;

        }
        //$('#HomeMenu').listview('refresh');
        $('.ui-page-active .ui-listview').listview('refresh');
    }
}
function GetMenuOrder(xml) {
    var d = new Date();
    var MenuOrder = new Array(8);
    var EventStatus = $(xml).find("EventStatusName").first().text();
    var LeagueStatus = $(xml).find("LeagueStatusName").first().text();
    var SkinsAmount = $(xml).find(" SkinsAmount").first().text();
  //debugger;
    //Play Delayed
    if (EventStatus == "Play Suspended") {
        $("#DelayNotification").css("visibility", "visible").css("display", "inline");
        $("#DelayNotification").text($(xml).find("PlaySuspendedReason").first().text());
    }
   // debugger;
    //check to see if the draft is over, if so don't show the draft room.
    switch (d.getDay()) {
        case 0:
            if (SkinsAmount != "0.0000") {
                if (EventStatus == "Active") {
                    MenuOrder[0] = "DraftRoom";
                    MenuOrder[1] = "DraftResults";
                }
                else {
                    MenuOrder[0] = "SkinsSun";
                    MenuOrder[1] = "TeamTop2";
                    MenuOrder[2] = "Leaderboard";
                    MenuOrder[3] = "SkinsSat";
                    MenuOrder[4] = "BBThurs";
                    MenuOrder[5] = "BBFri";
                }
            }
            else {
                if (EventStatus == "Active") {
                    MenuOrder[0] = "DraftRoom";
                    MenuOrder[1] = "DraftResults";
                }
                else {
                    MenuOrder[0] = "TeamTop2";
                    MenuOrder[1] = "Leaderboard";
                    MenuOrder[2] = "BBThurs";
                    MenuOrder[3] = "BBFri";
                }
            }
            //  MenuOrder[6] = "DraftRoom";
            //MenuOrder[7] = "DraftResults";
            break;
        case 1:
            if (SkinsAmount != "0.0000") {
                if (EventStatus == "Active") {
                    MenuOrder[0] = "DraftRoom";
                    MenuOrder[1] = "DraftResults";
                }
                else {
                    MenuOrder[0] = "SkinsSun";
                    MenuOrder[1] = "TeamTop2";
                    MenuOrder[2] = "Leaderboard";
                    MenuOrder[3] = "SkinsSat";
                    MenuOrder[4] = "BBThurs";
                    MenuOrder[5] = "BBFri";
                }
            }
            else {
                if (EventStatus == "Active") {
                    MenuOrder[0] = "DraftRoom";
                    MenuOrder[1] = "DraftResults";
                }
                else {
                    MenuOrder[0] = "TeamTop2";
                    MenuOrder[1] = "Leaderboard";
                    MenuOrder[2] = "BBThurs";
                    MenuOrder[3] = "BBFri";
                }   
            }
            break;
        case 2:
            if (LeagueStatus != "Draft Completed")
                MenuOrder[0] = "DraftRoom";

            MenuOrder[1] = "DraftResults";
            
//            MenuOrder[0] = "TeamTop2";
//            MenuOrder[1] = "Leaderboard";
//            MenuOrder[2] = "SkinsSat";
//            MenuOrder[3] = "SkinsSun";
//            MenuOrder[4] = "BBThurs";
//            MenuOrder[5] = "BBFri";
            break;
        case 3:
            if (LeagueStatus != "Draft Completed")
                MenuOrder[0] = "DraftRoom";
            MenuOrder[1] = "DraftResults";
            MenuOrder[2] = "Leaderboard";
            MenuOrder[3] = "TeamTop2";
            
            break;
        case 4:
            
            MenuOrder[0] = "BBThurs";
            MenuOrder[1] = "TeamTop2";
            MenuOrder[2] = "Leaderboard";
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
                MenuOrder[0] = "TeamTop2";
                MenuOrder[1] = "Leaderboard";
                MenuOrder[2] = "BBFri";
                MenuOrder[3] = "BBThurs";
            }
            //MenuOrder[5] = "SkinsSun";
            //MenuOrder[6] = "DraftRoom";
            //MenuOrder[7] = "DraftResults";
            break;
        default:
            if (SkinsAmount != "0.0000") {
                MenuOrder[0] = "SkinsSat";
                MenuOrder[1] = "TeamTop2";
                MenuOrder[2] = "Leaderboard";
                MenuOrder[3] = "BBFri";
                MenuOrder[4] = "BBThurs";
                MenuOrder[5] = "SkinsSun";
                MenuOrder[6] = "DraftRoom";
                MenuOrder[7] = "DraftResults";
            }
            else {
                MenuOrder[0] = "TeamTop2";
                MenuOrder[1] = "Leaderboard";
                MenuOrder[2] = "BBFri";
                MenuOrder[3] = "BBThurs";
                MenuOrder[4] = "DraftRoom";
                MenuOrder[5] = "DraftResults";
            }
            break;
    }
    return MenuOrder;
}