function TeamTop2(xml) {
    // debugger;
    $(xml).find("GolfTeam").sort(function (a, b) {
        var aScore = parseInt(a.getElementsByTagName("TeamTop2Score")[a.getElementsByTagName("TeamTop2Score").length - 1].childNodes[0].nodeValue);
        var bScore = parseInt(b.getElementsByTagName("TeamTop2Score")[b.getElementsByTagName("TeamTop2Score").length - 1].childNodes[0].nodeValue);
        if (aScore > bScore) return 1;
        if (aScore < bScore) return -1;
        return 0
    }).each
                            (
                                function (index) {
                                    if (index == 0 && $(this).find("TeamTop2Score").text() != "0") {
                                        $("#TeamTop2Leader").text($(this).find("GolfTeamName").text() + " @ " + $(this).find("TeamTop2Score").text());
                                    }
                                    var $newli = $("#dcolapsibleTeamTop2")
                                    .append($('<div/>',
                                        {
                                            //'data-role': 'list-divider',
                                            'data-role': 'collapsible',
                                          //  'data-theme': 'e',
                                            'data-collapsed-icon': 'arrow-r',
                                            'data-expanded-icon': 'arrow-d'
                                        })
                                        .append($('<h3/>',
                                        {
                                            'text': ($(this).find("TeamTop2Score").text() == "" ? "" : $(this).find("TeamTop2Score").text()) + "  " + $(this).find("GolfTeamName").text()
                                        }))
                                        .append($('<ul/>',
                                        {
                                            'id': 'teamTop2_' + $(this).find("GolfTeamId").first().text(),
                                            'data-role': 'listview',
                                            'data-inset': 'true'

                                        })));

                                    $(this).find("Golfer").sort(function (a, b) {
                                        var aScore = parseInt(a.getElementsByTagName("GolferScore")[a.getElementsByTagName("GolferScore").length - 1].childNodes[0].nodeValue);
                                        var bScore = parseInt(b.getElementsByTagName("GolferScore")[b.getElementsByTagName("GolferScore").length - 1].childNodes[0].nodeValue);
                                        if (aScore > bScore) return 1;
                                        if (aScore < bScore) return -1;
                                        return 0
                                    }).each(
                                        function () {
                                            $('#teamTop2_' + $(this).find("GolfTeamId").first().text())
                                            .append($('<li/>', {})
                                             .append($('<a/>',
                                                {
                                                    'rel' : 'external',
                                                    'href': $(this).find("GolferPageUrl").first().text(),
                                                    'data-transition': 'fade'
                                                })
                                                .append($('<h3/>',
                                                {
                                                    'text': golferText($(this))
                                                }))
                                                .append($('<p/>',
                                                {
                                                    'text': todayText($(this))
                                                })
                                                 .append($('<span/>',
                                                {
                                                    'class': 'ui-li-count',
                                                    'text': statusText($(this))
                                                })))));
                                        }
                                    )
                                            
                                }
                            )
                                        $('.ui-page-active :jqmData(role=collapsible)').collapsible();
                                        $('.ui-page-active :jqmData(role=content)').trigger('create');
                                        
                                    }


                                    function golferText(xml) {
                                         //debugger;
                                        var rtn = "";

                                        if (($(xml).find("RoundInProgress").first().text() == "Rnd 1" || $(xml).find("RoundInProgress").first().text() == "") && $(xml).find("TodaysRoundStarted").first().text() == "false") 
                                                    rtn =  $(xml).find("GolferName").first().text();
                                                else {
                                                    if ($(xml).find("GolferScore").first().text() == 0)
                                                        rtn = "(Even) " + $(xml).find("GolferName").first().text();
                                                    else
                                                        rtn = "(" + $(xml).find("GolferScore").first().text() + ") " + $(xml).find("GolferName").first().text();
                                                }
                                        return rtn;
                                    }


                                    function todayText(xml) {
//                                    var golferId = "4c82fbfc-d5c7-e211-9de3-46f9930cc812"
//                                    if ($(xml).find("GolferId").first().text() == "4c82fbfc-d5c7-e211-9de3-46f9930cc812")
//                                        debugger;

                                        var rtn = "";
                                        if ($(xml).find("TodaysRoundStarted").first().text() == "true") {
                                            if ($(xml).find("RoundScore").first().text() == "0")
                                                rtn = "Today (E)";
                                            else
                                                rtn = "Today (" + $(xml).find("RoundScore").first().text() + ")";
                                        }
                                        else {
                                            if ($(xml).find("RoundInProgress").first().text() != "Rnd 1") {
                                                if ($(xml).find("TournamentRank").first().text() != "0")
                                                    rtn = $(xml).find("TournamentRank").first().text();
                                                else {
                                                    if ($(xml).find("GolferScore").first().text() == 0)
                                                        if ($(xml).find("TodaysRoundStarted").first().text() == "true")
                                                            rtn = "Tournament Score (Even)";
                                                        else
                                                            if ($(xml).find("Round").length > 1)
                                                                rtn = "Tournament Score (Even)";
                                                            else
                                                                rtn = "";
                                                    else
                                                        rtn = "Tournament Score (" + $(xml).find("GolferScore").first().text() + ")";
                                                }
                                            }                                                
                                        }
                                        return rtn;
                                    }

                                    function statusText(xml) {
                                        var rtn = "";
                                        if ($(xml).find("Thru").first().text() == "0" || $(xml).find("Thru").first().text() == "")
                                            rtn = $(xml).find("NextTeeTime").first().text();
                                        else
                                            rtn = $(xml).find("Thru").first().text() + "/18";

                                        if ($(xml).find("GolferStatus").first().text() == 'Missed Cut')
                                            rtn = "Missed Cut";
                                        if ($(xml).find("GolferStatus").first().text() == 'Withdraw')
                                            rtn = "Withdraw";

                                        return rtn;
                                    }