﻿function Skins(xml, day) {
    // debugger;
    $(xml).find("Skins_" + day).find("Hole")
//    .sort(function (a, b) {
//        var aTB = (parseInt(a.getElementsByTagName("HoleNumber")[a.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue));
//        var bTB = (parseInt(b.getElementsByTagName("HoleNumber")[b.getElementsByTagName("HoleNumber").length - 1].childNodes[0].nodeValue));
//        if (aTB > bTB) return 1;
//        if (aTB < bTB) return -1;

//        return 0
//    })
    .each(function (index) {
        $("#SkinsTotal" + day).text($(xml).find("Skins_" + day).find("Hole").length.toString());

       // debugger;
        var totalActive = $(xml).find("Skins_" + day).find("ActiveGolferCount").first().text().toString();

        $('#ulSkins' + day).append($('<li/>', {})
                                .append($('<a/>',
                                {
                                    'href': '#' + 'pSkins_' + day + $(this).find("HoleNumber").first().text(),
                                    'data-transition': 'slide'
                                })
                                .append($('<h3/>',
                                        {
                                            'text': "Hole #" + $(this).find("HoleNumber").first().text() + " " + $(this).find("ScoreName").first().text() + " - " + $(this).find("HoleGolfTeam").first().text()
                                        }))
                                .append($('<p/>',
                                        {
                                            'text': ($(this).find("HoleGolfTeam").first().text() != "Tied" ? $(this).find("HoleGolfer").first().text() : "Eagles:" + $(this).find("SkinEagleCount_" + day).text() + " Birdies:" + $(this).find("SkinBirdieCount_" + day).text() + " Pars:" + $(this).find("SkinParCount_" + day).text() + " Bogeys:" + $(this).find("SkinBogeyCount_" + day).text())
                                        }))
                                .append($('<p/>',
                                        {
                                            'text': "Handicap:" + $(this).find("CalculatedHandicapRank").first().text() + " Par:" + $(this).find("Par").first().text()
                                        }))
                                .append($('<span/>',
                                {
                                    'class': 'ui-li-count',
                                    'text': $(this).find("EventPlays").find("PossibleHole").length + "/" + totalActive
                                }))));

        $("#body").append($('<div/>',
                                        {
                                            'data-role': 'page',
                                            'id': 'pSkins_' + day + $(this).find("HoleNumber").first().text(),
                                            'data-theme': 'c'
                                        })
                                         .append($('<div/>',
                                        {
                                            'data-role': 'header',
                                            'data-theme': 'e'
                                        })
                                        .append($(jQuery(jQuery("#headerTemplate").html()))))
                                        .append($('<div/>',
                                        {
                                            'data-role': 'content'
                                        })
                                        .append($('<h3/>',
                                        {
                                            'text': "Hole# " + $(this).find("HoleNumber").first().text()
                                        }))
                                        .append($('<ul/>',
                                        {
                                            'data-inset': 'true',
                                            'data-role': 'listview',
                                            'id': 'SkinHoles_' + day + $(this).find("pHoleNumber").first().text()
                                        }))));

        $(this).find("EventPlays").each(function () {
            $(this).find("PossibleHole")
//            .sort(function (a, b) {
//                var aTB = (parseInt(a.getElementsByTagName("pHoleShots")[a.getElementsByTagName("pHoleShots").length - 1].childNodes[0].nodeValue));
//                var bTB = (parseInt(b.getElementsByTagName("pHoleShots")[b.getElementsByTagName("pHoleShots").length - 1].childNodes[0].nodeValue));
//                if (aTB > bTB) return 1;
//                if (aTB < bTB) return -1;

//                return 0
//            })
            .each(function () {
                $('#SkinHoles_' + day + $(this).find("pHoleNumber").first().text())
                                                            .append($('<li/>', {})
                                                                .append($('<h3/>',
                                                                {
                                                                    'text': $(this).find("pHoleScoreName").first().text() + " - " + $(this).find("pHoleHoleGolfer").first().text()
                                                                }).css('background-color', ($(this).find("pHoleSkin").first().text() == "true" ? 'red' : '')))
                                                            .append($('<p/>',
                                                                {
                                                                    'text': $(this).find("pHoleGolfTeam").first().text()
                                                                })));
            })
        })
                                                    })
                                                    $('.ui-page-active .ui-listview').listview('refresh');
}