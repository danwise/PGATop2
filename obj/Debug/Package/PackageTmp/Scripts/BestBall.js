function BestBallThurs(xml) {
    // debugger;
    $(xml).find("GolfTeam").sort(function (a, b) {
        var aScore = parseInt(a.getElementsByTagName("BestBallScore_Thurs")[a.getElementsByTagName("BestBallScore_Thurs").length - 1].childNodes[0].nodeValue);
        var bScore = parseInt(b.getElementsByTagName("BestBallScore_Thurs")[b.getElementsByTagName("BestBallScore_Thurs").length - 1].childNodes[0].nodeValue);
        if (aScore > bScore) return 1;
        if (aScore < bScore) return -1;
        return 0
    }).each
                            (
                                function (index) {
                                    if (index == 0 && $(this).find("BestBallScore_Thurs").text() != "0") {
                                        //$("#BBThurLeader").append($('<p/>',{'text':$(this).find("GolfTeamName").text() + " @" + $(this).find("BestBallScore_Thurs").text()}));
                                        $("#BBThurLeader").text($(this).find("GolfTeamName").text() + " @" + $(this).find("BestBallScore_Thurs").text());
                                    }

                                    var $newli = $("#csBBThurs")
                                    .append($('<div/>',
                                        {
                                            'data-role': 'collapsible',
                                            'data-theme': 'a'
                                        })
                                        .append($('<h3/>',
                                        {
                                            'text': $(this).find("BestBallScore_Thurs").text() + " - " + $(this).find("GolfTeamName").text()

                                        }))
                                        .append($('<ul/>',
                                        {
                                            'data-role': 'listview',
                                            'data-inset': 'true',
                                            'id': 'bestballTeam_thurs' + $(this).find("GolfTeamId").first().text()
                                        })));

                                    $(this).find("BestBallHoles_Thurs").each
                                    (
                                        function () {
                                            $(this).find("Hole").sort(function (a, b) {
                                                var aHoleNumber = parseInt(a.getElementsByTagName("HoleNumber")[a.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue);
                                                var bHoleNumber = parseInt(b.getElementsByTagName("HoleNumber")[b.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue);
                                                if (aHoleNumber > bHoleNumber) return 1;
                                                if (aHoleNumber < bHoleNumber) return -1;
                                                return 0
                                            }).each(
                                            function () {
                                                $('#bestballTeam_thurs' + $(this).find("GolfTeamId").first().text())
                                                 .append($('<li/>', {})
                                                     .append($('<h3/>',
                                                        {
                                                            'text': 'Hole# ' + $(this).find("HoleNumber").text() + " - " + $(this).find("ScoreName").last().text()
                                                        }))
                                                            .append($('<p/>',
                                                        {
                                                            'text': 'Par: ' + $(this).find("Par").text()
                                                        }))
                                                        .append($('<p/>',
                                                        {
                                                            'text': 'Handicap Rank: ' + $(this).find("CalculatedHandicapRank").text()
                                                        })));
                                            })
                                        }
                                    )
                                }
                            )

    //                            $('#teamTop2').listview('refresh');

}