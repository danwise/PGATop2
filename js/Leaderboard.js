function gotoGolfer(GolferId,GolfTeamId,GolferName) {
   // debugger;
    window.localStorage["GolferId"] = GolferId;
    window.localStorage["GolferGolfTeamId"] = GolfTeamId;
    window.localStorage["GolferName"] = GolferName;
    var url = "Golfer.htm";
    $(location).attr('href', url);
}
function Leaderboard(xml) {
    $(xml).find("Golfer")
    .each
                            (
                                function (index) {
//                                    if (index == 0 && $(this).find("GolferScore").first().text() != "") {
//                                        $("#LeaderboardLeader").text($(this).find("GolferName").first().text() + "/" + $(this).find("GolferGolfTeam").first().text() + " @ " + $(this).find("GolferScore").last().text());
//                                    }

                                    $("#ulLeaderboard")
                                    .append($('<li/>', {})
                                        .append($('<a/>',
                                        {
                                            'href': "javascript:gotoGolfer('" + $(this).find("GolferId").first().text() + "','" + $(this).find("GolfTeamId").first().text() + "','" + $(this).find("GolferName").first().text() + "')",
                                            //'href' :$(this).find("ShotTrackerUrl").first().text(),
                                            'data-transition': 'slide',
                                            'rel' : 'external'
                                        })
                                        .append($('<img/>',
                                        {
                                            'src': $(this).find("GolferImgUrl").first().text()
                                        })).css("height","60px")
                                        .append($('<h5/>',
                                        {
                                            //'text': (index + 1) + ". " + $(this).find("GolferName").first().text() + ($(this).find("TotalScore").last().text() == "" ? "" : " @ " + $(this).find("TotalScore").last().text())
                                            //'text': "(" + ($(this).find("GolferScore").first().text() == "" ? "" : ($(this).find("GolferScore").first().text() == "0" ? "E" : $(this).find("GolferScore").first().text())) + ") " + $(this).find("GolferName").first().text() 
                                            'text': golferText($(this))
                                        }))
                                        .append($('<p/>',
                                        {
                                            'text': $(this).find("GolferGolfTeam").first().text() + " " + ($(this).find("DraftPickNumber").first().text() != "0"?"Pick #" + $(this).find("DraftPickNumber").first().text():"")
                                        }))
                                        .append($('<p/>',
                                        {
                                            //'text': "Today: " + ($(this).find("RoundScore").first().text()=="0"?"Even":$(this).find("RoundScore").first().text())
                                            'text': todayText($(this))
                                        }))
                                        .append($('<p/>',
                                        {
                                            //'text': "Today: " + ($(this).find("RoundScore").first().text()=="0"?"Even":$(this).find("RoundScore").first().text())
                                            'text': ($(this).find("TournamentRank").first().text() != ""?"Rank:" + $(this).find("TournamentRank").first().text():"")
                                        }))
                                        .append($('<span/>',
                                        {
                                            'class': 'ui-li-count'
                                            ,'text': statusText($(this))
                                        }))));
                                })

                                    $('.ui-page-active .ui-listview').listview('refresh');

                                }

                                function todayText(xml) {
    //                                 debugger;
                                    var rtn = "";
                                    if ($(xml).find("TodaysRoundStarted").first().text() == "true") {
                                        if ($(xml).find("RoundScore").first().text() == "0")
                                            rtn = "Today (E)";
                                        else
                                            rtn = "Today (" + $(xml).find("RoundScore").first().text() + ")";
                                    }
                                    else {
                                        if ($(xml).find("dtNextTeeTime").first().text() != "0001-01-01T00:00:00")
                                        {
                                            if ($(xml).find("RoundInProgress").first().text() != "Rnd 1") {
//                                                if ($(xml).find("TournamentRank").first().text() != "0")
//                                                    rtn = "Tournament Rank:" + $(xml).find("TournamentRank").first().text();
//                                                else {
                                                    if ($(xml).find("GolferScore").first().text() == 0)
                                                        rtn = "Score (Even)";
                                                    else
                                                        rtn = "Score (" + $(xml).find("GolferScore").first().text() + ")";
//                                                }
                                            }
                                        }
                                    }
                                    return rtn;
                                }

                                function golferText(xml) {
                                    // debugger;
                                    var rtn = "";

                                    //if (($(xml).find("RoundInProgress").first().text() == "Rnd 1" || $(xml).find("dtNextTeeTime").first().text() == "0001-01-01T00:00:00") && $(xml).find("TodaysRoundStarted").first().text() == "false")
                                    if ($(xml).find("RoundInProgress").first().text() == "Rnd 1" && $(xml).find("TodaysRoundStarted").first().text() == "false")
                                        rtn = $(xml).find("GolferName").first().text();
                                    else {
                                        if ($(xml).find("GolferScore").first().text() == 0)
                                            rtn = "(Even) " + $(xml).find("GolferName").first().text();
                                        else
                                            rtn = "(" + $(xml).find("GolferScore").first().text() + ") " + $(xml).find("GolferName").first().text();
                                    }
                                    return rtn;
                                }

                                function statusText(xml) {
                                   // debugger;
                                    var rtn = "";
                                    if ($(xml).find("isCutLine").first().text() == "true")
                                        rtn = "Cut Line";
                                    else
                                    {
                                        if ($(xml).find("Thru").first().text() == "0" || $(xml).find("Thru").first().text() == "")
                                            rtn = $(xml).find("NextTeeTime").first().text();
                                        else
                                            rtn = $(xml).find("Thru").first().text() + "/18";

                                        if ($(xml).find("GolferStatus").first().text() == 'Missed Cut')
                                            rtn = "Missed Cut";
                                        if ($(xml).find("GolferStatus").first().text() == 'Withdraw')
                                            rtn = "Withdraw";
                                    }
                                    return rtn;
                                }