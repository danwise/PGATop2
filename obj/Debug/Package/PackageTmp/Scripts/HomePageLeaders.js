//NOT IN USE ANYMORE!!

function HomePageLeader_TeamTop2(xml) {
    //debugger;
//Team Top 2
    $(xml).find("GolfTeam").sort(function (a, b) {
        var aScore = parseInt(a.getElementsByTagName("TeamTop2Score")[a.getElementsByTagName("TeamTop2Score").length - 1].childNodes[0].nodeValue);
        var bScore = parseInt(b.getElementsByTagName("TeamTop2Score")[b.getElementsByTagName("TeamTop2Score").length - 1].childNodes[0].nodeValue);
        if (aScore > bScore) return 1;
        if (aScore < bScore) return -1;
        return 0
    }).each(
                function (index) {
                    if (index == 0 && $(this).find("TeamTop2Score").text() != "0") {
                        $("#TeamTop2Leader").text($(this).find("GolfTeamName").text() + " @ " + $(this).find("TeamTop2Score").text());
                        return false;
                    }
                }
            )
}

function HomePageLeader_BestBall(xml,day) {              
        $(xml).find("GolfTeam").sort(function (a, b) {
            var aTB = (a.getElementsByTagName("BestBallTieBreaker_" + day)[a.getElementsByTagName("BestBallTieBreaker_" + day).length - 1].childNodes[0].nodeValue == "true" ? parseInt(a.getElementsByTagName("BestBallScore_" + day)[a.getElementsByTagName("BestBallScore_" + day).length - 1].childNodes[0].nodeValue) - 10 : parseInt(a.getElementsByTagName("BestBallScore_" + day)[a.getElementsByTagName("BestBallScore_" + day).length - 1].childNodes[0].nodeValue));
            var bTB = (b.getElementsByTagName("BestBallTieBreaker_" + day)[b.getElementsByTagName("BestBallTieBreaker_" + day).length - 1].childNodes[0].nodeValue == "true" ? parseInt(b.getElementsByTagName("BestBallScore_" + day)[b.getElementsByTagName("BestBallScore_" + day).length - 1].childNodes[0].nodeValue) - 10 : parseInt(b.getElementsByTagName("BestBallScore_" + day)[b.getElementsByTagName("BestBallScore_" + day).length - 1].childNodes[0].nodeValue));
            if (aTB > bTB) return 1;
            if (aTB < bTB) return -1;

            return 0
        }).each(
                        function (index) {
                            if ($(this).find("BestBallScore_" + day).text() != "0" && $(this).find("BestBallHoles_" + day).find("Hole").length == 18) {
                                if ($(this).find("BestBallTieBreaker_" + day).first().text() == "true")
                                    $("#BB" + day + "Leader").text($(this).find("GolfTeamName").text() + " @ " + $(this).find("BestBallScore_" + day).text() + " *TB");
                                else
                                    $("#BB" + day + "Leader").text($(this).find("GolfTeamName").text() + " @ " + $(this).find("BestBallScore_" + day).text());

                                return false;

                            }
                        }
                )
  }

  function HomePageLeader_Skins(xml,day) {

            $("#SkinsTotal" + day).text($(xml).find("Skins_" + day).find("Hole").length.toString());
}

function HomePageLeader_Leaderboard(xml) {
            //Leaderboard
            $(xml).find("Golfer").sort(function (a, b) {
                var aScore = parseInt(a.getElementsByTagName("GolferScore")[a.getElementsByTagName("GolferScore").length - 1].childNodes[0].nodeValue);
                var bScore = parseInt(b.getElementsByTagName("GolferScore")[b.getElementsByTagName("GolferScore").length - 1].childNodes[0].nodeValue);
                if (aScore > bScore) return 1;
                if (aScore < bScore) return -1;
                return 0
            }).each
                            (
                                function (index) {
                                    if (index == 0 && $(this).find("GolferScore").first().text() != "") {
                                        $("#LeaderboardLeader").text($(this).find("GolferName").first().text() + "/" + $(this).find("GolferGolfTeam").first().text() + " @ " + $(this).find("GolferScore").last().text());

                                        return false;
                                    }

                                }
                                    )
}