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
                                function (index) 
                                {
                                    if(index==0 && $(this).find("TeamTop2Score").text() != "0")
                                    {
                                     $("#TeamTop2Leader").text($(this).find("GolfTeamName").text() + " @ " + $(this).find("TeamTop2Score").text());
                                    }
                                    var $newli =   $("#ulTeamTop2")
                                    .append($('<li/>',
                                        { 
                                            'data-role': 'list-divider',
                                            'data-theme' : 'e',
                                        })
                                        .append($('<a/>',
                                        {
                                            'href': '#' + $(this).find("GolfTeamId").first().text(),
                                            'data-transition': 'slide'
                                        }).css("color","black")
                                        .append($('<h3/>',
                                        {
                                            'text': ($(this).find("TeamTop2Score").text() == "" ?"": $(this).find("TeamTop2Score").text()) + "  " + $(this).find("GolfTeamName").text()
                                        }))
                                        .append($('<ul/>',
                                        {
                                            'id': $(this).find("GolfTeamId").text(),
                                            'data-role' :'listview',
                                            'data-inset':'true'
                                            
                                        }))));

                                    $(this).find("Golfer").sort(function (a, b) {
                                     var aScore = parseInt(a.getElementsByTagName("TotalScore")[a.getElementsByTagName("TotalScore").length - 1].childNodes[0].nodeValue);
                                     var bScore = parseInt(b.getElementsByTagName("TotalScore")[b.getElementsByTagName("TotalScore").length - 1].childNodes[0].nodeValue);
                                     if (aScore > bScore) return 1;
                                     if (aScore < bScore) return -1;
                                     return 0
                                    }).each(
                                        function () 
                                        {
                                            $newli.append($('<li/>', {})
                                             .append($('<a/>',
                                                {
                                                    'href': '#' + $(this).find("GolferId").first().text(),
                                                    'data-transition': 'slide'
                                                })
                                                .append($('<h3/>',
                                                {
                                                    'text': $(this).find("GolferName").text() + ($(this).find("TotalScore").last().text() == ""?"": " @ " +$(this).find("TotalScore").last().text())
                                                })
                                                 .append($('<span/>',
                                                {
                                                    'class' : 'ui-li-count',
                                                    'text': ($(this).find("Thru").last().text() == "0" || $(this).find("Thru").last().text() == ""? $(this).find("TeeTime").last().text() : $(this).find("Thru").last().text() + "/18") 
                                                })))));
                                        }
                                    )
                                        }
                            )

                         

                        }