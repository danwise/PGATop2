function loadGolfer(xml, GolferId) {
    //debugger;
    $(xml).find("Golfer").filter(function () {
        return $(this).find('GolferId').first().text() == GolferId;
    }).each(
                            function () {
                                $('#hGolfer').text($(this).find('GolferName').first().text());

                                $(this).find("Round")
//                                .sort(function (a, b) {
//                                    var aScore = parseInt(a.getElementsByTagName("DayNumber")[a.getElementsByTagName("DayNumber").length - 1].childNodes[0].nodeValue);
//                                    var bScore = parseInt(b.getElementsByTagName("DayNumber")[b.getElementsByTagName("DayNumber").length - 1].childNodes[0].nodeValue);
//                                    if (aScore > bScore) return -1;
//                                    if (aScore < bScore) return 1;
//                                    return 0
//                                })
                                .each(
                                function () {
                                    $('#ulRounds').append($('<li/>', {})
                                        .append($('<a/>',
                                        {
                                            'href': '#' + $(this).find("RoundId").first().text(),
                                            'data-transition': 'slide'
                                        })
                                        .append($('<h3/>',
                                        {
                                            'text': $(this).find("RoundName").first().text() //+ ($(this).find("RoundScore").last().text() == "" ? "" : " @ " + ($(this).find("RoundScore").last().text()=="0"?"E":$(this).find("RoundScore").last().text()))
                                        }))
                                        .append($('<p/>',
                                        {
                                            //'text': "Score: " + ($(this).find("RoundScore").text() == "0" ? "E" : $(this).find("RoundScore").text()),
                                            'text' : todayText($(this)),
                                            'font-weight': 'bold'
                                        }))
                                        .append($('<p/>',
                                        {
                                            'text': "Shots: " + $(this).find("RoundShots").first().text()
                                        }))
                                            .append($('<span/>',
                                        {
                                            'class': 'ui-li-count',
                                            //'text': ($(this).first().parent().first().parent().find("MissedCut").first().text() == 'true' ? 'Missed Cut' : ($(this).find("Thru").first().text() == "0" || $(this).find("Thru").first().text() == "" ? $(this).find("NextTeeTime").first().text() : $(this).find("Thru").first().text() + "/18"))
                                            'text': statusText($(this))
                                        }))));

                                    var $roundPage =
                                            $("#body").append($('<div/>',
                                                    {
                                                        'data-role': 'page',
                                                        'id': $(this).find("RoundId").first().text(),
                                                        'data-theme': 'c'
                                                    })
                                                        .append($('<div/>',
                                                    {
                                                        'data-role': 'header',
                                                        'data-theme': 'e'
                                                    })
                                                    .append($(jQuery(jQuery("#headerTemplate").html().replace("pageHeaderHeader1", "pageHeaderHeader" + $(this).find("RoundId").first().text()).replace("pageHeaderFooter1", "pageHeaderFooter" + $(this).find("RoundId").first().text())))))
                                                    .append($('<div/>',
                                                    {
                                                        'data-role': 'content'
                                                    })
                                                    .append($('<h3/>',
                                                    {
                                                        'text': $(this).find("HoleGolfer").first().text() + " - " + $(this).find("RoundName").first().text()
                                                    }))
                                                    .append($('<ul/>',
                                                    {
                                                        'data-inset': 'true',
                                                        'data-role': 'listview',
                                                        'id': 'roundListing' + $(this).find("RoundId").first().text()
                                                    }))));


                                    $roundPage.bind('pageinit', function () {
                                        //                                                                $('#' + 'roundListing' + $(this).find("RoundId").first().text()).listview('refresh');
                                        $('#' + 'pageHeaderHeader' + $(this).find("RoundId").first().text()).append($('<h4/>', { 'text': $.cookie("GolfEventName") }).css("text-align", "left"));
                                        $('#' + 'pageHeaderFooter' + $(this).find("RoundId").first().text()).append($('<h6/>', { 'text': $.cookie("GolfTeamName") }).css("text-align", "right").css("padding-right", ".5cm").css("text-valign", "top"));
                                    });

                                    $(this).find("Hole")
//                                    .sort(function (a, b) {
//                                        var aHole = parseInt(a.getElementsByTagName("HoleNumber")[a.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue);
//                                        var bHole = parseInt(b.getElementsByTagName("HoleNumber")[b.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue);
//                                        if (aHole > bHole) return 1;
//                                        if (aHole < bHole) return -1;
//                                        return 0
//                                    })
                                    .each(
                                                            function () {
                                                                $('#roundListing' + $(this).find("RoundId").text()).append($('<li/>', {})
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
                                                            }
                                                     );
                                                            });
                                                            $('.ui-page-active .ui-listview').listview('refresh');
                            });
    //            });

                                                    }

                                                    function todayText(xml) {
                                                        // debugger;
                                                        var rtn = "";
                                                        if ($(xml).find("Thru").first().text() != "0" && $(xml).find("Thru").first().text() != "" ) {
                                                            if ($(xml).find("RoundScore").first().text() == "0")
                                                                rtn = "Score: (E)";
                                                            else
                                                                rtn = "Score: (" + $(xml).find("RoundScore").first().text() + ")";
                                                        }
                                                        else
                                                            rtn = "Not Started";

                                                        return rtn;
                                                    }

                                                    function statusText(xml) {
                                                        //debugger;
                                                        var rtn = "";
                                                        if ($(xml).find("Thru").first().text() == "0" || $(xml).find("Thru").first().text() == "")
                                                            rtn = $(xml).find("TeeTime").first().text();
                                                        else
                                                            rtn = $(xml).find("Thru").first().text() + "/18";

                                                        if ($(xml).find("MissedCut").first().text() == 'true')
                                                            rtn = "Missed Cut";
                                                        if ($(xml).find("RoundEventStatus").first().text() == 'Withdraw')
                                                            rtn = "Withdraw";

                                                        return rtn;
                                                    }