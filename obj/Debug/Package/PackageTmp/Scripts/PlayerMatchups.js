function loadMatchups(golfTeamId) 
{
    //      debugger;
    $.ajax(
            {
                beforeSend: function () { $.mobile.loading("show"); }, //Show spinner
                complete: function () { $.mobile.loading("hide"); }, //Hide spinner
                type: "POST",
                datatype: "xml",
                url: "../Golf.EventDraft.asmx/PlayerMatchups",
                data: { GolfTeamId: golfTeamId },
                success: function (xml) { xmlParser(xml, golfTeamId); },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
}

function xmlParser(xml, golfTeamId) {
    //var golfTeamId = $.cookie("GolfTeamId");
//    debugger;
    $(xml).find("PlayerMatchups").each(function (index) {
        //        $("#dcolapsibleTeams").append($('<div/>',
        //                    {
        //                        //'data-role': 'list-divider',
        //                        'data-role': 'collapsible',
        //                        'data-theme': 'e',
        //                        'data-collapsed-icon': 'arrow-r',
        //                        'data-expanded-icon': 'arrow-d'
        //                    })
        //                    .append($('<h3/>',
        //                    {
        //                        'text': $(this).find("MatchupGolfTeamName").text()
        //                    })
        //                    .append($('<p/>',
        //                    {
        //                        "class": "ui-li-aside",
        //                        'text': $(this).find("MatchupGolfTeamWinLoss").text()
        //                    })))
        //                    .append($('<ul/>',
        //                    {
        //                        'id': $(this).find("MatchupGolfTeamId").first().text(),
        //                        'data-role': 'listview',
        //                        'data-inset': 'true'

        //                    })));

        //        $("#dBets").append($('<ul/>',
        //                    {
        //                        'id': $(this).find("MatchupGolfTeamId").first().text(),
        //                        'data-role': 'listview',
        //                        'data-inset': 'true'
        //                    }));

        // debugger;
        //        var golfTeamId = $.cookie("GolfTeamId");
        $("#dBets").empty();
        $("#dBets").append($('<h3/>',
                    {
                        'text': "Win/Loss: " + $(this).find("MatchupGolfTeamWinLoss").text()
                    }));

        var MenuOrder = GetMenuOrder();
        for (var i = 0; i < MenuOrder.length; i++) {
            if (MenuOrder[i] != null)
                buildMatchups($(this), MenuOrder[i], golfTeamId,  $(this).find("MatchupGolfTeamId").first().text(), (i < 2 ? true : true));
        }
    });
}

function GetMenuOrder() {
    var d = new Date();
    var MenuOrder = new Array(5);
   // debugger;
    switch (d.getDay()) {
        case 0: //Sunday
            MenuOrder[0] = "EventMatchups";
            MenuOrder[1] = "SundayMatchups";
            MenuOrder[2] = "SaturdayMatchups";
            MenuOrder[3] = "FridayMatchups";
            MenuOrder[4] = "ThursdayMatchups";
            break;
        case 1:
        case 2:
            MenuOrder[0] = "EventMatchups";
            MenuOrder[1] = "SundayMatchups";
            MenuOrder[2] = "SaturdayMatchups";
            MenuOrder[3] = "FridayMatchups";
            MenuOrder[4] = "ThursdayMatchups";
            break;
        case 3: //Wed
            MenuOrder[0] = "EventMatchups";
            MenuOrder[1] = "ThursdayMatchups";
            break;
        case 4: //Thursday
            MenuOrder[0] = "FridayMatchups";
            MenuOrder[1] = "EventMatchups";
            MenuOrder[2] = "ThursdayMatchups";
            break;
        case 5: //Friday
            MenuOrder[0] = "SaturdayMatchups";
            MenuOrder[1] = "FridayMatchups";
            MenuOrder[2] = "EventMatchups";
            MenuOrder[3] = "ThursdayMatchups";
                    
            break;
        case 6: //Saturday
            MenuOrder[0] = "SundayMatchups";
            MenuOrder[1] = "EventMatchups";
            MenuOrder[2] = "SaturdayMatchups";
            MenuOrder[3] = "FridayMatchups";
            MenuOrder[4] = "ThursdayMatchups";
            break;
    }
    return MenuOrder;
}

function buildMatchups(xml, matchupType, golfTeamId, loggedInGolfTeamId, expanded) {
    //debugger;
    
               if($(xml).find(matchupType).find("PlayerMatchup").length > 0)
               {
                var DayStatus = dayStatus($(xml), golfTeamId, matchupType);

                $("#dBets").append($('<div/>',
                    {
                        //'data-role': 'list-divider',
                        'data-role': 'collapsible',
                        'data-theme': 'e',
                        'data-collapsed-icon': 'arrow-r',
                        'data-expanded-icon': 'arrow-d',
                        'data-collapsed': expanded
                    })
                    .append($('<h3/>',
                    {
                        'text': matchupType
                    })
                    .append($('<img/>',
                    {
                        "class": "ui-li-aside",
                        'src': DayStatus[1],
                        'height': '16px',
                        'width': '16px'
                    }).css("visibility",DayStatus[2]))
                    .append($('<p/>',
                    {
                        "class": "ui-li-aside",
                        'text': DayStatus[0]
                    })))
                    .append($('<ul/>',
                    {
                        'id': matchupType,
                        'data-role': 'listview',
                        'data-inset': 'true'
                     })));

                    // debugger;
                     $(xml).find(matchupType).each(
                        function () {
                            $(this).find("PlayerMatchup").each(
                                        function () {

                                            var showButton_Favorite = "hidden";
                                            var showButton_Underdog = "hidden";
                                            var betAccepted_Underdog = "false";
                                            var betAccepted_Favorite = "false";

                                            if ($(this).find("UnderdogAccepted").first().text() == "true")
                                                betAccepted_Underdog = "true";

                                            if ($(this).find("FavoriteAccepted").first().text() == "true")
                                                betAccepted_Favorite = "true";

                                            if (loggedInGolfTeamId.toUpperCase() == $(this).find("FavoriteGolfTeamId").first().text().toUpperCase() && betAccepted_Favorite == "false" && $(this).find("BetStatus").text() != "Closed" && $(this).find("BetStatus").text() != "Declined") {
                                                //                                                debugger;
                                                showButton_Favorite = "visible";
                                            }

                                            //&& ($(this).find("Favorite").first().find("Round").first().find("Thru").text() == 0
                                            if (loggedInGolfTeamId.toUpperCase() == $(this).find("UnderdogGolfTeamId").first().text().toUpperCase() && betAccepted_Underdog == "false" && $(this).find("BetStatus").text() != "Closed" && $(this).find("BetStatus").text() != "Declined") {
                                                //                                                debugger;
                                                showButton_Underdog = "visible";
                                            }
                                            //                                            //debugger;
                                            //                                            if ($(this).find("BetStatus").text() == "Closed") {
                                            //                                                //debugger;
                                            //                                                showButton_Favorite = "hidden";
                                            //                                                showButton_Underdog = "hidden";
                                            //                                            }
                                            // debugger;

                                            var status = betStatus($(this), golfTeamId);

                                            $("#" + matchupType)
                                           .append($('<div/>',
                                                {
                                                    //'data-role': 'list-divider',
                                                    'data-role': 'collapsible',
                                                    'data-theme': 'c',
                                                    'data-collapsed-icon': 'arrow-r',
                                                    'data-expanded-icon': 'arrow-d'
                                                })
                                                .append($('<h3/>',
                                                {
                                                    'text': $(this).find("PlayerMatchupTitle").text(),
                                                    'class': 'smallHeader'
                                                }).css("font-size", "10px")
//                                                .append($('<div/',
//                                                {
//                                                    'class':'imgContainer'
                                                //                                                })).css("align","right")
                                                .append($('<img/>',
                                                    {
                                                        "class": "ui-li-aside",
                                                        'id': $(this).find("PlayerMatchupId").first().text() + "_statuspic",
                                                        'src': status[1],
                                                        'height': '16px',
                                                        'width': '16px'
                                                    }).css("visibility", status[2])))
//                                                .append($('<p/>',
//                                                    {
//                                                        "class": "ui-li-aside",
//                                                        //'text': betStatus($(this), golfTeamId)
//                                                        'text': status[0]
//                                                    })))
                                                .append($('<ul/>',
                                                {
                                                    'id': $(this).find("PlayerMatchupId").first().text() + "_" + golfTeamId,
                                                    'data-role': 'listview',
                                                    'data-inset': 'true'

                                                })));

                                            $('#' + $(this).find("PlayerMatchupId").text() + "_" + golfTeamId)
                                                .append($('<li/>', {})
                                                    .append($('<a/>',
                                                    {
                                                        'href': '#',
                                                        'data-transition': 'slide'
                                                    })
                                                    .append($('<h3/>',
                                                    {
                                                        'text': $(this).find("UnderdogGolfer").first().text() + " " + ($(this).find("UnderdogOdds").text() > 0 ? "(+" + $(this).find("UnderdogOdds").text() + ")" : "(" + ($(this).find("UnderdogOdds").text() == "0" ? "Even" : $(this).find("UnderdogOdds").text()) + ")")
                                                    }))
//                                                    .append($('<div/',
//                                                    {
//                                                        'class' : 'ui-grid-b'
//                                                    }))
                                                    .append($('<p/>',
                                                    {
//                                                        "class": "ui-block-a",
                                                        'text': $(this).find("UnderdogGolfTeam").text()

                                                    }))
                                                    .append($('<p/>',
                                                    {
                                                        //"class": "ui-block-b",
                                                        'text': "$" + $(this).find("UnderdogAmount").text()
                                                    }).css("float", "right").css("overflow", "hidden").css("white-space", "nowrap").css("text-overflow", "ellipsis"))
                                                    .append($('<p/>',
                                                    {
                                                        'text': formatScore(
                                                            $(this).find("Underdog").first().find("Round").first().find("RoundScore").text()
                                                            , $(this).find("Underdog").first().find("Round").first().find("TotalScore").text()
                                                            , $(this).find("Underdog").first().find("Round").first().find("TeeTime").text()
                                                            , $(this).find("Underdog").first().find("Round").first().find("Thru").text()
                                                            , $(this).find("Underdog").first().find("Round").first().find("RoundName").text()
                                                            , $(this).find("Underdog").first().find("Round").first().find("RoundGolferEventStatus").text()
                                                            , matchupType
                                                            )
                                                    })))
                                                    .append($('<a/>',
                                                    {
                                                        'href': "javascript:betPopup('" + $(this).find("PlayerMatchupId").text() + "','" + $(this).find("UnderdogGolfTeamId").first().text().toUpperCase() + "')",
                                                        'data-icon': (betAccepted_Favorite == "false" ? 'plus' : 'check'),
                                                        'text': (betAccepted_Favorite == "false" ? "Propose Bet" : "Accept Bet")
                                                    }).css("visibility", showButton_Underdog)));


                                            $('#' + $(this).find("PlayerMatchupId").text() + "_" + golfTeamId)
                                                .append($('<li/>', {})
                                                    .append($('<a/>',
                                                    {
                                                        'href': '#',
                                                        'data-transition': 'slide'
                                                    })
                                                    .append($('<h3/>',
                                                    {
                                                        'text': $(this).find("FavoriteGolfer").first().text() + " " + ($(this).find("FavoriteOdds").text() > 0 ? "(+" + $(this).find("FavoriteOdds").text() + ")" : "(" + ($(this).find("FavoriteOdds").text() == "0" ? "Even" : $(this).find("FavoriteOdds").text()) + ")")
                                                    }))
//                                                    .append($('<div/',
//                                                    {
//                                                        'class': 'ui-grid-b'
//                                                                                                        }))
                                                    .append($('<p/>',
                                                    {
//                                                        "class": "ui-block-a",
                                                        'text': $(this).find("FavoriteGolfTeam").text()
                                                        
                                                    }))
                                                    .append($('<p/>',
                                                    {
//                                                        "class": "ui-block-b",
                                                        'text': "$" + $(this).find("FavoriteAmount").text()
                                                    }).css("float", "right").css("overflow", "hidden").css("white-space", "nowrap").css("text-overflow", "ellipsis"))
                                                    
//                                                    .append($('<p/>',
//                                                    {
//                                                       // "class": "ui-li-aside",
//                                                        'text': "$" + $(this).find("FavoriteAmount").text()
//                                                    }).css("float", "right").css("overflow", "hidden").css("white-space","nowrap").css("text-overflow", "ellipsis"))
//                                                    .append($('<p/>',
//                                                    {
//                                                        'text': $(this).find("FavoriteGolfTeam").text() + " $" + $(this).find("FavoriteAmount").text()
//                                                    }))
                                                    .append($('<p/>',
                                                    {
                                                        'text': formatScore(
                                                            $(this).find("Favorite").first().find("Round").first().find("RoundScore").text()
                                                            , $(this).find("Favorite").first().find("Round").first().find("TotalScore").text()
                                                            , $(this).find("Favorite").first().find("Round").first().find("TeeTime").text()
                                                            , $(this).find("Favorite").first().find("Round").first().find("Thru").text()
                                                            , $(this).find("Favorite").first().find("Round").first().find("RoundName").text()
                                                            ,$(this).find("Favorite").first().find("Round").first().find("RoundGolferEventStatus").text()
                                                            , matchupType
                                                            )
                                                    })))
                                                    .append($('<a/>',
                                                    {
                                                        'href': "javascript:betPopup('" + $(this).find("PlayerMatchupId").text() + "','" + $(this).find("FavoriteGolfTeamId").first().text().toUpperCase() + "')",
                                                        'data-icon': (betAccepted_Underdog == "false" ? 'plus' : 'check'),
                                                        'text': (betAccepted_Underdog == "false" ? "Propose Bet" : "Accept Bet Or Make Counter Offer")
                                                    }).css("visibility", showButton_Favorite)));

                                                })
                                                $('.ui-page-active .ui-listview').listview('refresh');
                                            })
                                            $('.ui-page-active :jqmData(role=content)').trigger('create');
                }
            }

            function formatScore(rndScore, totalScore, teeTime, thru, roundName, golferStatus, matchupType) 
            {
                  //   debugger;
                    var rtn = ""
                    if (matchupType == "EventMatchups")
                    {
                        if (thru != 0) {
                            if (thru == 18)
                                if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                                    rtn = rtn + golferStatus + " - Event:" + totalScore;
                                else
                                    rtn = rtn + "\n" + roundName + " complete. Event: " + totalScore;
                            else {
                                if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                                    rtn = rtn + golferStatus + " - Event:" + totalScore;
                                else
                                    rtn = rtn + "Event :" + totalScore + " " + roundName + " (" + rndScore + ") Thru: " + thru;
                            }
                        }
                        else
                            if (roundName == "Rnd 1")
                                rtn = rtn + roundName + " Tee Time: " + teeTime;
                            else
                                if (teeTime == "" && totalScore == "")
                                    rtn = rtn + " ";
                                else {
                                    if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                                        rtn = rtn + golferStatus + " - Event:" + totalScore;
                                    else
                                        rtn = rtn + " Tee Time: " + teeTime + " Event: " + totalScore;
                                }
                    } 
                    else
                        rtn = roundName + " Score: " + rndScore + " " + (thru != 0 ? "Thru: " + thru : "Tee Time: " + teeTime);
                                                
                    return rtn;
                }

                                        function dayStatus(xml, golfTeamId,matchupType) {
                                            var result = [];
                                            result[2] = "hidden";
                                            var bolBetsNotAccepted = false;
                                            var proposedCount = 0;
                                            $(xml).find(matchupType).find("PlayerMatchup").each(
                                            function () {
                                                //   debugger;
                                                var sbetStatus = $(this).find("BetStatus").text();
                                                var underdogAccepted = $(this).find("UnderdogAccepted").text();
                                                var underdogGolfTeamId = $(this).find("UnderdogGolfTeamId").first().text().toUpperCase();
                                                var favoriteGolfTeamId = $(this).find("FavoriteGolfTeamId").first().text().toUpperCase();
                                                var favoriteAccepted = $(this).find("FavoriteAccepted").text();

                                                if (sbetStatus == "Proposed") {
                                                    //Bet Proposed 
                                                    proposedCount = proposedCount + 1;

                                                    if (underdogAccepted == "false" && golfTeamId.toUpperCase() == underdogGolfTeamId) {
                                                        result[0] = "Proposed";
                                                        bolBetsNotAccepted = true;
                                                        // result[2] = "hidden";
                                                        return false;
                                                    }
                                                    if (favoriteAccepted == "false" && golfTeamId.toUpperCase() == favoriteGolfTeamId) {
                                                        result[0] = "Proposed";
                                                        bolBetsNotAccepted = true;
                                                        //result[2] = "hidden";
                                                        return false;

                                                    }
                                                }
                                            });

                                            

                                            // debugger;
                                            switch (true) {
                                                case (parseFloat($(xml).find(matchupType + "WinLoss").text().replace("$", "")) == 0):
                                                    if (bolBetsNotAccepted == true)
                                                    //result[1] = "../images/red_light.png";
                                                        result[1] = "../images/flashing_red.gif";

                                                    if (bolBetsNotAccepted == false) {
                                                        if (proposedCount > 0) {
                                                            result[0] = "Offered ";
                                                            result[1] = "../images/flashing_yellow.gif";
                                                        }
                                                        else
                                                            result[1] = "../images/green_light.png";
                                                        // result[1] = "../images/flashing_green.jpg";
                                                    }
                                                    result[2] = "visible";
                                                    break;
                                                case (parseFloat($(xml).find(matchupType + "WinLoss").text().replace("$", "")) > 0):
                                                    result[1] = "../images/thumbs_up.png";
                                                    result[2] = "visible";
                                                    result[0] = $(xml).find(matchupType + "WinLoss").text();
                                                    break;
                                                case (parseFloat($(xml).find(matchupType + "WinLoss").text().replace("$", "")) < 0):
                                                    result[1] = "../images/thumbs_down.png";
                                                    result[2] = "visible";
                                                    result[0] = $(xml).find(matchupType + "WinLoss").text();
                                                    break;
                                            }
                                            if (parseFloat($(xml).find(matchupType + "WinLoss").text().replace("$", "")) != parseFloat("0")) {
                                                result[0] = $(xml).find(matchupType + "WinLoss").text();
                                            }
                                            
                                            return result;
                                        }


                                        function betStatus(xml, golfTeamId) {
                                         //   debugger;
                                            var sbetStatus = $(xml).find("BetStatus").text();
                                            var isTie = $(xml).find("isTie").text();
                                            var playerMatchupId = $(xml).find("PlayerMatchupId").text();
                                            var underdogGolfTeam = $(xml).find("UnderdogGolfTeam").text();
                                            var underdogGolfTeamId = $(xml).find("UnderdogGolfTeamId").first().text().toUpperCase();
                                            var underdogAccepted = $(xml).find("UnderdogAccepted").text();
                                            var underdogAmount = $(xml).find("UnderdogAmount").text();
                                            var underdogWon = $(xml).find("UnderdogWon").text();

                                            var favoriteGolfTeam = $(xml).find("FavoriteGolfTeam").text();
                                            var favoriteGolfTeamId = $(xml).find("FavoriteGolfTeamId").first().text().toUpperCase();
                                            var favoriteAccepted = $(xml).find("FavoriteAccepted").text();
                                            var favoriteAmount = $(xml).find("FavoriteAmount").text();
                                            var favoriteWon = $(xml).find("FavoriteWon").text();

                                            var isUnderdog = (golfTeamId.toUpperCase() == underdogGolfTeamId ? "true" : "false");
                                            var isFavorite = (golfTeamId.toUpperCase() == favoriteGolfTeamId ? "true" : "false");
                                            var isEventMatchup = $(xml).find("isEventMatchup").text();
                                            var favoriteScore = parseInt((isEventMatchup=="true"? ($(xml).find("Favorite").first().find("Round").first().find("TotalScore").text() == ""?0:$(xml).find("Favorite").first().find("Round").first().find("TotalScore").text()) : $(xml).find("Favorite").first().find("Round").first().find("RoundScore").text()));
                                            var underdogScore = parseInt((isEventMatchup=="true"? ($(xml).find("Underdog").first().find("Round").first().find("TotalScore").text() ==""?0:$(xml).find("Underdog").first().find("Round").first().find("TotalScore").text()) : $(xml).find("Underdog").first().find("Round").first().find("RoundScore").text()));
                                            //$(xml).find("Favorite").first().find("GolferScore").text()
//                                            if (playerMatchupId.toUpperCase() == "51C42E24-0A10-E311-9307-86AF9538D0B1")
//                                                debugger;

                                            var result = [];
                                            result[2] = "hidden";
                                            //debugger;
                                            //Bet Proposed
                                            if (sbetStatus == "Proposed") {
                                                result[2] = "visible";
                                                //Bet Proposed
                                                if (underdogAccepted == "true" && golfTeamId.toUpperCase() == underdogGolfTeamId) {
                                                    result[1] = "../images/flashing_yellow.gif";
                                                    //result[0] = "Proposed";
                                                    //                                                    result[0] = "Bet Proposed To " + favoriteGolfTeam;
                                                    
                                                }
                                                if (underdogAccepted == "false" && golfTeamId.toUpperCase() == underdogGolfTeamId) {
                                                    result[1] = "../images/flashing_red.gif";
                                                   // result[0] = "Offered";
                                                    //result[0] = "Bet Offered By " + favoriteGolfTeam;
                                                }

                                                if (favoriteAccepted == "true" && golfTeamId.toUpperCase() == favoriteGolfTeamId) {
                                                    result[1] = "../images/flashing_yellow.gif";
                                                   // result[0] = "Proposed";
                                                    //result[0] = "Bet Proposed To " + underdogGolfTeam;
                                                }
                                                if (favoriteAccepted == "false" && golfTeamId.toUpperCase() == favoriteGolfTeamId) {
                                                    result[1] = "../images/flashing_red.gif";
                                                   // result[0] = "Offered";
                                                    //result[0] = "Bet Offered By " + underdogGolfTeam;
                                                }
                                                    
                                            }

                                            //Bet Accepted, show winner if there is one.
                                            if (sbetStatus == "Accepted") {
                                               // debugger;

                                                result[0] = "Bet Accepted";
                                                result[2] = "visible";
                                                if (isUnderdog == "true" && underdogScore < favoriteScore) {
                                                    result[0] = abs(underdogScore - favoriteScore);
                                                    result[1] = "../images/Number-" + abs(underdogScore - favoriteScore) + "-icon-green.png";
                                                }
                                                if (isUnderdog == "false" && underdogScore < favoriteScore) {
                                                    result[0] = abs(underdogScore - favoriteScore);
                                                    //result[1] = "../images/thumbs_down.png";
                                                    result[1] = "../images/Number-" + abs(underdogScore - favoriteScore) + "-icon-red.png";
                                                }
                                                if (isFavorite == "true" && underdogScore > favoriteScore) {
                                                    result[0] = abs(favoriteScore - underdogScore);
                                                    //result[1] = "../images/thumbs_up.png";
                                                    if (abs(favoriteScore - underdogScore) > 10)
                                                        result[1] = "../images/lock.png";
                                                    else
                                                        result[1] = "../images/Number-" + abs(underdogScore - favoriteScore) + "-icon-green.png";
                                                }
                                                if (isFavorite == "false" && underdogScore > favoriteScore) {
                                                    result[0] = abs(favoriteScore - underdogScore);
                                                    //result[1] = "../images/thumbs_down.png";
                                                    if (abs(favoriteScore - underdogScore) > 10)
                                                        result[1] = "../images/poop.png";
                                                    else
                                                        result[1] = "../images/Number-" + abs(underdogScore - favoriteScore) + "-icon-red.png";
                                                }

                                                if (isTie == "true" && underdogScore == favoriteScore) {
                                                    result[0] = "Even";
                                                    result[1] = "../images/Number-0-icon.png";
                                                    
                                                }

                                                if (favoriteWon == "true") {
                                                    result[0] = favoriteGolfTeam + " won $" + favoriteAmount;
                                                    result[2] = "hidden";
                                                }

                                                if (underdogWon == "true") {
                                                    result[0] = underdogGolfTeam + " won $" + underdogAmount;
                                                    result[2] = "hidden";
                                                }
                                                
                                            }

                                            //Bet Active
                                            if (sbetStatus == "Active") {
                                                //result[0] = "Waiting for Proposal";
                                                result[1] = "../images/green_light.png";
                                                result[2] = "visible";
                                            }
                                            //Bet Declined
                                            if (sbetStatus == "Declined") {
                                                result[0] = "Bet Declined";
                                                result[1] = "../images/cancel16x16.gif";
                                                result[2] = "visible";

                                                if (underdogAccepted == "true")
                                                    result[0] = "Bet Declined By " + favoriteGolfTeam;
                                                if (favoriteAccepted == "true")
                                                    result[0] = "Bet Declined By " + underdogGolfTeam;
                                                
                                            }

                                            //Underdog Won
                                            if (underdogWon == "true" && sbetStatus == "Completed" && isUnderdog == "true") {
                                                result[0] = "$" + underdogAmount;
                                                result[1] = "../images/thumbs_up.png";
                                                result[2] = "visible";
                                            }
                                            //Underdog Lost
                                            if (underdogWon == "false" && sbetStatus == "Completed" && isUnderdog == "true") {
                                                result[0] = "$" + underdogAmount;
                                                result[1] = "../images/thumbs_down.png";
                                                result[2] = "visible";
                                            }

                                            //Favorite Won
                                            if (favoriteWon == "true" && sbetStatus == "Completed" && isFavorite == "true") {
                                                result[0] = "$" + favoriteAmount;
                                                result[1] = "../images/thumbs_up.png";
                                                result[2] = "visible";
                                            }

                                            //Favorite Lost
                                            if (favoriteWon == "false" && sbetStatus == "Completed" && isFavorite == "true") {
                                                result[0] = "$" + favoriteAmount;
                                                result[1] = "../images/thumbs_down.png";
                                                result[2] = "visible";
                                            }

                                            //tied bet
                                            if (favoriteWon == "false" && sbetStatus == "Completed" && underdogWon == "false") {
                                                result[0] = "$" + favoriteAmount;
                                                result[1] = "../images/Number-0-icon.png";
                                                result[2] = "visible";
                                            }

                                            if (sbetStatus == "Closed") {
                                                result[0] = "Closed Not Accepted";
                                                result[1] = "../images/cancel16x16.gif";
                                                result[2] = "visible";
                                            }
                                             //   debugger;
                                            if (isTie == "true" && sbetStatus == "Completed")
                                                result[0] = "Tie";
                                            
                                            return result;
                                        }

                                        function abs(x) {
                                            return Number(x < 0 ? x * -1 : x);
                                        }