function BestBall(xml, day) {
    var LeaderSet = false;
    // debugger;
     $(xml).find("GolfTeam")
    .each(
                                function (index) {
                                   // debugger;
                                    $('#ulbb' + day).append($('<li/>',
                                    {
                                        //'data-theme': ($(this).find("BestBallHoles_" + day).find("TeamPlays").find("PossibleHole").length == ($(this).find("ActiveGolfers").text() * 18) ? "c" : "a")
                                    }).append($('<a/>',
                                                {
                                                    'href': '#' + 'bestballTeam_' + day + $(this).find("GolfTeamId").first().text(),
                                                    'data-transition': 'slide'
                                                })
                                                .append($('<h3/>',
                                                {
                                                    'text': ($(this).find("BestBallScore_" + day).text() == "" || $(this).find("BestBallScore_" + day).text() == "0" ? "" : +$(this).find("BestBallScore_" + day).text() + " ") + $(this).find("GolfTeamName").text() + ($(this).find("BestBallTieBreaker_" + day).first().text() == "true" ? " *TB" : "")
                                                }))
                                                .append($('<p/>',
                                                {
                                                    'text': "Eagles:" + $(this).find("BestBallEagleCount_" + day).text() + " Birdies:" + $(this).find("BestBallBirdieCount_" + day).text() + " Pars:" + $(this).find("BestBallParCount_" + day).text() + " Bogeys:" + $(this).find("BestBallBogeyCount_" + day).text()
                                                }))
                                                .append($('<span/>',
                                                {
                                                    'class': 'ui-li-count',
                                                    'text': $(this).find("BestBallHoles_" + day).find("TeamPlays").find("PossibleHole").length + "/" + $(this).find("ActiveGolfers").text() * 18
                                                }))));
                                    var $teamPage =
                               $("#body").append($('<div/>',
                                        {
                                            'data-role': 'page',
                                            'id': 'bestballTeam_' + day + $(this).find("GolfTeamId").first().text(),
                                            'data-theme': 'b'
                                        })
                                         .append($('<div/>',
                                        {
                                            'data-role': 'header',
                                            'data-theme': 'a'
                                        })
                                        .append($(jQuery(jQuery("#headerTemplate").html()))))
                                        .append($('<div/>',
                                        {
                                            'data-role': 'content'
                                        })
                                        .append($('<h3/>',
                                        {
                                            'text': $(this).find("GolfTeamName").first().text()
                                        })
                                        .append($('<p/>',
                                        {
                                            'text': $(this).find("BestBallHoles_" + day).find("TeamPlays").find("PossibleHole").length + " of " + $(this).find("ActiveGolfers").text() * 18 + " holes played"
                                        }))
                                        .append($('<ul/>',
                                        {
                                            'data-inset': 'true',
                                            'data-role': 'listview',
                                            'id': 'bestballTeamHoles_' + day + $(this).find("GolfTeamId").first().text()
                                        })))));

                                    $(this).find("BestBallHoles_" + day).each
                                    (
                                        function () {
                                            $(this).find("Hole")
                                            .each(
                                            function (index) {
                                                $('#bestballTeamHoles_' + day + $(this).find("GolfTeamId").first().text())
                                                 .append($('<li/>', {})
                                                    .append($('<a/>',
                                                    {
                                                        'href': '#' + 'bestballTeamHole_' + day + $(this).find("HoleId").first().text(),
                                                        'data-transition': 'slide'
                                                    })
                                                     .append($('<h3/>',
                                                        {
                                                            'text': 'Hole# ' + $(this).find("HoleNumber").text() + " - " + $(this).find("ScoreName").last().text()
                                                            //'color': ($(this).find("BestBallTieBreaker").first().text() == "true" ? 'red' : 'black')
                                                        })).css('background-color', ($(this).find("BestBallTieBreaker").first().text() == "true" ? 'red' : ''))
                                                        .append($('<p/>',
                                                        {
                                                            'text': $(this).find("HoleGolfer").last().text()
                                                        }))
                                                        .append($('<p/>',
                                                        {
                                                            'text': 'Par: ' + $(this).find("Par").text()
                                                        }))
                                                        .append($('<p/>',
                                                        {
                                                            'text': 'Handicap Rank: ' + $(this).find("CalculatedHandicapRank").first().text()
                                                        }))
                                                        .append($('<span/>',
                                                        {
                                                            'class': 'ui-li-count',
                                                            'text': $(this).find("TeamPlays").find("PossibleHole").length + "/" + $(this).find("pActiveGolfers").first().text() 
                                                        }))));


                                                $("#body").append($('<div/>',
                                                        {
                                                            'data-role': 'page',
                                                            'id': 'bestballTeamHole_' + day + $(this).find("HoleId").first().text(),
                                                        })
                                                         .append($('<div/>',
                                                            {
                                                                'data-role': 'header',
                                                                'data-theme': 'a'
                                                            })
                                                        .append($(jQuery(jQuery("#headerTemplate").html()))))
                                                        .append($('<div/>',
                                                        {
                                                            'data-role': 'content'
                                                        })
                                                        .append($('<h3/>',
                                                        {
                                                            'text': 'Hole# ' + $(this).find("HoleNumber").text() + " - " + $(this).find("ScoreName").last().text()
                                                        }))
                                                        .append($('<ul/>',
                                                        {
                                                            'data-inset': 'true',
                                                            'data-role': 'listview',
                                                            'id': 'bestballTeamHolesPlayed_' + day + $(this).find("HoleId").first().text(),
                                                        }))));

                                                $(this).find("TeamPlays").each
                                                        (
                                                        function () {
                                                            $(this).find("PossibleHole").each(
                                                            function () {
                                                                $('#bestballTeamHolesPlayed_' + day + $(this).find("pHoleParentHoleId").first().text())
                                                                 .append($('<li/>', {})
                                                                     .append($('<h3/>',
                                                                        {
                                                                            'text': $(this).find("pHoleHoleGolfer").text() + " - " + $(this).find("pHoleScoreName").last().text()
                                                                        })));
                                                            })
                                                        })
                                            })
                                        }
                                    )
                                }
                            )

                         $('.ui-page-active .ui-listview').listview('refresh');

}