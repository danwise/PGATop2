function BtnAccept_Click(matchupId, golfTeamId) {
    //            debugger;
    $.ajax(
            {
                //                beforeSend: function () { $.mobile.showPageLoadingMsg("e", "Accepting Bet..."); }, //Show spinner
                //                complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
                type: "POST",
                datatype: "xml",
                async: false,
                cache: false,
                url: "http://www.dwise.net/Golf.EventDraft.asmx/AcceptBet",
                data: { PlayerMatchupId: matchupId, GolfTeamId: golfTeamId },
                success: function (xml) {
                    /*disable accept button*/
                    $("#Accept_" + matchupId).addClass('ui-state-disabled');

                    //set flag so user can't reactivate accept button.
                    $("#isProposed_" + matchupId).val("true");
                    //Deactive ate Plus and Minus buttons
                    $("#btnPlus_" + matchupId).addClass('ui-state-disabled');
                    $("#btnMinus_" + matchupId).addClass('ui-state-disabled');


                    //Close Pannel
                    $("#panel_" + matchupId).panel("close");
                    var style = $("#dAccepted")[0].attributes["style"].value;
                    if (style.indexOf("hidden") > -1 || $("#dAccepted")[0].children.length == 0) {
                        $("#dAccepted").css("visibility", "visible").css("display", "inline");
                        $("#dAccepted").append($('<h3/>',
                                {
                                    'text': "Accepted"
                                }));
                    }

                    var proposedAnchor = $("#ProposedToYou_" + matchupId);
                    $("#dAccepted").append($('<a/>',
                                {
                                    'text': proposedAnchor.text(),
                                    'href': proposedAnchor[0].attributes['href'].value,
                                    'data-ajax': "false",
                                    'class': "jqm-deeplink jqm-open-quicklink-panel ui-icon-carat-l ui-alt-icon"
                                }));

                    //hide old anchor tag
                    $("#ProposedToYou_" + matchupId).remove();


                    Success(matchupId, golfTeamId);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
}

function BtnCounter_Click(matchupId, golfTeamId) {
    var amount = $("#Baseline_" + matchupId).val();

    $.ajax(
            {
                //  beforeSend: function () { $.mobile.showPageLoadingMsg("e", "Making Counter Offer..."); }, //Show spinner
                // complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
                type: "POST",
                datatype: "xml",
                async: false,
                cache: false,
                url: "http://www.dwise.net/Golf.EventDraft.asmx/CounterOfferBet",
                data: { PlayerMatchupId: matchupId, GolfTeamId: golfTeamId, Amount: amount },
                success: function (xml) {
                    debugger;
                    /*disable accept and counter buttons*/
                    $("#Accept_" + matchupId).addClass('ui-state-disabled');
                    $("#counterOffer_" + matchupId).addClass('ui-state-disabled');
                    //Close Pannel
                    $("#panel_" + matchupId).panel("close");


                    var style = $("#dProposed")[0].attributes["style"].value;
                    if (style.indexOf("hidden") > -1 || $("#dProposed")[0].children.length == 0) {
                        $("#dProposed").css("visibility", "visible").css("display", "inline");
                        $("#dProposed").append($('<h3/>',
                                {
                                    'text': "Proposed"
                                }));
                    }

                    var proposedAnchor = $("#ProposedToYou_" + matchupId);
                    $("#dProposed").append($('<a/>',
                                {
                                    'text': proposedAnchor.text(),
                                    'href': proposedAnchor[0].attributes['href'].value,
                                    'data-ajax': "false",
                                    'class': "jqm-deeplink jqm-open-quicklink-panel ui-icon-carat-l ui-alt-icon"
                                }));

                    //hide old anchor tag
                    $("#ProposedToYou_" + matchupId).remove();


                    Success(matchupId, golfTeamId);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
}


function BtnPropose_Click(matchupId, golfTeamId) {
    // debugger;
    var amount = $("#Baseline_" + matchupId).val();
    $.ajax(
            {
                // beforeSend: function () { $.mobile.showPageLoadingMsg("e", "Accepting Bet..."); }, //Show spinner
                //complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
                type: "POST",
                datatype: "xml",
                // async: false,
                //cache: false,
                url: "http://www.dwise.net/Golf.EventDraft.asmx/ProposeBet",
                data: { PlayerMatchupId: matchupId, GolfTeamId: golfTeamId, Amount: amount },
                success: function (xml) {
                    //debugger;
                    /*deactive propose button*/
                    $("#Propose_" + matchupId).addClass('ui-state-disabled');

                    //set flag so user can't reactivate accept button.
                    $("#isProposed_" + matchupId).val("true");
                    //Deactive ate Plus and Minus buttons
                    $("#btnPlus_" + matchupId).addClass('ui-state-disabled');
                    $("#btnMinus_" + matchupId).addClass('ui-state-disabled');

                    $("#panel_" + matchupId).panel("close");

                    var style = $("#dProposed")[0].attributes["style"].value;
                    if (style.indexOf("hidden") > -1 || $("#dProposed")[0].children.length == 0) {
                        $("#dProposed").css("visibility", "visible").css("display", "inline");
                        $("#dProposed").append($('<h3/>',
                                {
                                    'text': "Proposed"
                                }));
                    }


                    //Move from active to proposed div?
                    var proposedAnchor = $("#Active_" + matchupId);
                    $("#dProposed").append($('<a/>',
                                {
                                    'text': proposedAnchor.text(),
                                    'href': proposedAnchor[0].attributes['href'].value,
                                    'data-ajax': "false",
                                    'class': "jqm-deeplink jqm-open-quicklink-panel ui-icon-carat-l ui-alt-icon"
                                }));

                    //hide old anchor tag
                    $("#Active_" + matchupId).remove();


                    //Check to see if there are any more "Active" bets if not then clear "#dActive"
                    //if($("#dActive").



                    Success(matchupId, golfTeamId);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
}

function Success(PlayerMatchupId, golfTeamId) {

    //debugger;
    if ($("#dActive")[0].children.length == 1)
        $("#dActive").css("visibility", "hidden").css("display", "none");

    //Check to see if there are any more "Active" bets if not then clear "#dProposedToYou"
    if ($("#dProposedToYou")[0].children.length == 1)
        $("#dProposedToYou").css("visibility", "hidden").css("display", "none");

    if ($("#dProposed")[0].children.length == 1)
        $("#dProposed").css("visibility", "hidden").css("display", "none");

    $('.ui-page-active :jqmData(role=content)').trigger('create');
}

//function declineBet(matchupId, golfTeamId) {
//    $.ajax(
//            {
//                beforeSend: function () { $.mobile.showPageLoadingMsg("e", "Declining Bet..."); }, //Show spinner
//                complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
//                type: "POST",
//                datatype: "xml",
//                async: false,
//                cache: false,
//                url: "http://www.dwise.net/Golf.EventDraft.asmx/DeclineBet",
//                data: { PlayerMatchupId: matchupId, GolfTeamId: golfTeamId },
//                success: function (xml) { $.mobile.hidePageLoadingMsg(); Success(); },
//                error: function (XMLHttpRequest, textStatus, errorThrown) {
//                    alert(errorThrown);
//                }
//            });
//}


function loadMatchups(golfTeamId) {
          //debugger;
          $.ajax(
            {
                //   beforeSend: function () { $.mobile.loading("show"); }, //Show spinner
                //  complete: function () { $.mobile.loading("hide"); }, //Hide spinner
                type: "GET",
                datatype: "xml",
                url: "http://www.dwise.net/Golf.EventDraft.asmx/PlayerMatchups2",
                data: { GolfTeamId: golfTeamId },
                success: function (xml) {
                    xmlParser(xml, golfTeamId);
                    $.mobile.loading("hide");
                    },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                  ///  debugger;
                    // alert(errorThrown);
                }
            });
}

function xmlParser(xml, golfTeamId) {
    //var golfTeamId = $.cookie("GolfTeamId");
    // debugger;


    

    //      if (parseInt($(xml).find("TotalWL").first().text()) < 0)
    //          $("#TotalWL").css("color", "#e60811");



    $(xml).find("PlayerMatchups").each(function (index) {

        $("#dActive").empty();
        $("#dProposed").empty();
        $("#dProposedToYou").empty();
        $("#dAccepted").empty();
        $("#dCompleted").empty();
        $("#panels").empty();

        buildMatchups($(this), golfTeamId, $(this).find("MatchupGolfTeamId").first().text());

        if (parseInt($(xml).find("TotalWL").first().text()) > 0)
            $("#TotalWL").addClass("ui-li-count-win");

        if (parseInt($(xml).find("TotalWL").first().text()) < 0)
            $("#TotalWL").addClass("ui-li-count-loss");

        $("#TotalWL").text(Math.abs($(xml).find("TotalWL").first().text()));
        $("#AcceptedW").text($(xml).find("AcceptedW").first().text());
        $("#AcceptedL").text(Math.abs($(xml).find("AcceptedL").first().text()));
    });
}

function buildMatchups(xml, golfTeamId, loggedInGolfTeamId) {
    // debugger;
    var iProposed = 0;
    var iProposedToYou = 0;
    var iInProcess = 0;
    var iCompleted = 0;
    var iActive = 0;



    $(xml).find("PlayerMatchup").each(
                function () {
                    var PlayerMatchupId = $(this).find("PlayerMatchupId").first().text();
                    var BetStatus = $(this).find("BetStatus").first().text()
                    // debugger;
                    switch (BetStatus) {
                        case "Active": //Proposed
                        case "Proposed": //Proposed

                            var showDiv = "";
                            if (BetStatus == "Active") {
                                iActive++;
                                showDiv = "Active";
                                if (iActive == 1) {
                                    $("#d" + showDiv).append($('<h3/>', { 'text': "Available" }));
                                    $("#d" + BetStatus).css("visibility", "visible").css("display", "inline");
                                }
                            }
                            else {
                                //                                iActive = 0;
                                //debugger;
                                var ProposedToYou = "true";
                                if ($(this).find("Proposed").first().text() == "true")
                                    ProposedToYou = "false";


                                if (ProposedToYou == "true") {
                                    //   debugger;
                                    showDiv = "ProposedToYou";
                                    iProposedToYou++;
                                    if (iProposedToYou == 1) {
                                        $("#d" + showDiv).append($('<h3/>', { 'text': "Proposed To You" }));
                                        $("#d" + showDiv).css("visibility", "visible").css("display", "inline");
                                    }
                                }
                                else {
                                    showDiv = "Proposed";
                                    iProposed++;
                                    if (iProposed == 1) {
                                        $("#d" + showDiv).append($('<h3/>', { 'text': showDiv }));
                                        $("#d" + showDiv).css("visibility", "visible").css("display", "inline");
                                    }
                                }
                            }

                            $("#d" + showDiv).append($('<a/>',
                            {
                                'text': $(this).find("PlayerMatchupTitle").first().text(),
                                'href': "#panel_" + PlayerMatchupId,
                                'data-ajax': "false",
                                'class': "jqm-deeplink jqm-open-quicklink-panel ui-icon-carat-l ui-alt-icon",
                                'id': showDiv + "_" + PlayerMatchupId
                            }));


                             // debugger;
                            $('#panels').append($('<div/>',
                            {
                                'data-role': "panel",
                                'id': "panel_" + PlayerMatchupId,
                                'data-position': "right",
                                'data-display': "overlay",
                                //'data-theme': "d"
                            })
                            //                             .append($('<script/>',
                            //                            {
                            //                                'src': "js/PlayerMatchups2_panel.js"
                            //                            }))
                            //                            .append($('<h3/>',
                            //                            {
                            //                                'data-rel': "close",
                            //                                //'class' : "ui-btn ui-icon-delete ui-btn-icon-left",
                            //                                'text': $(this).find("PlayerMatchupTitle").first().text()
                            //                            }))
                            .append($('<h3/>',
                            {
                                'text': ($(this).find("isEventMatchup").first().text() == "true" ? "Event" + ($(this).find("BetClosed").first().text() == "false" ? " @ " + ($(this).find("CloseTime").first().text() == "" ? " Tee Time Not Posted" : $(this).find("CloseTime").first().text()) : " - Closed") : $(this).find("DayName").first().text() + ($(this).find("BetClosed").first().text() == "false" ? " @ " + ($(this).find("CloseTime").first().text() == "" ? " Tee Time Not Posted" : $(this).find("CloseTime").first().text()) : " - Closed"))
                            })).css("color", "#ffefaa")
                            .append($('<fieldset/>',
                            {
                                'data-role': "controlgroup",
                                'data-type': "horizontal",
                                'data-mini': "true"
                            })
                            .append($('<input/>',
                            {
                                'type': "radio",
                                'name': "radio-choice-h-6_" + PlayerMatchupId,
                                'id': "radio-choice-h-6a_" + PlayerMatchupId,
                                'value': "on",
                                'checked': "checked",
                                'onchange': "OptionSet_onChange('" + PlayerMatchupId + "','1')",
                                'class' : "dwise-panel",
                            }))
                            .append($('<label/>',
                            {
                                'for': "radio-choice-h-6a_" + PlayerMatchupId,
                                'text': "$1"
                            }))
                            .append($('<input/>',
                            {
                                'type': "radio",
                                'name': "radio-choice-h-6_" + PlayerMatchupId,
                                'id': "radio-choice-h-6b_" + PlayerMatchupId,
                                'value': "on",
                                'onchange': "OptionSet_onChange('" + PlayerMatchupId + "','5')"
                            }))
                            .append($('<label/>',
                            {
                                'for': "radio-choice-h-6b_" + PlayerMatchupId,
                                'text': "$5"
                            }))
                            .append($('<input/>',
                            {
                                'type': "radio",
                                'name': "radio-choice-h-6_" + PlayerMatchupId,
                                'id': "radio-choice-h-6c_" + PlayerMatchupId,
                                'value': "on",
                                'onchange': "OptionSet_onChange('" + PlayerMatchupId + "','10')"
                            }))
                            .append($('<label/>',
                            {
                                'for': "radio-choice-h-6c_" + PlayerMatchupId,
                                'text': "$10"
                            }))
                            .append($('<input/>',
                            {
                                'type': "radio",
                                'name': "radio-choice-h-6_" + PlayerMatchupId,
                                'id': "radio-choice-h-6d_" + PlayerMatchupId,
                                'value': "on",
                                'onchange': "OptionSet_onChange('" + PlayerMatchupId + "','RST')"
                            }))
                            .append($('<label/>',
                            {
                                'for': "radio-choice-h-6d_" + PlayerMatchupId,
                                'text': "RST"
                            })))
                            .append($('<div/>',
                            {
                                'data-role': "navbar",
                                'data-grid': "c"
                            })
                            .append($('<ul/>', {})
                            .append($('<li/>', {})
                            .append($('<a/>',
                            {
                                'text': "+",
                                'class': "ui-btn ui-shadow ui-btn-inline",
                                'href': "javascript:BtnPlus_Click('" + PlayerMatchupId + "')",
                                'id': "btnPlus_" + PlayerMatchupId
                            })))
                            .append($('<li/>', {})
                            .append($('<a/>',
                            {
                                'text': "-",
                                'class': "ui-btn ui-shadow ui-btn-inline",
                                'href': "javascript:BtnMinus_Click('" + PlayerMatchupId + "')",
                                'id': "btnMinus_" + PlayerMatchupId
                            })))
                            .append($('<li/>', {}))
                            .append($('<li/>', {})
                            .append($('<input/>',
                                {
                                    // 'class': "ui-state-disabled",
                                    'id': "Baseline_" + PlayerMatchupId,
                                    'value': $(this).find("BetAmount").first().text()
                                })))))
                            .append($('<div/>',
                            {
                                'data-role': "content",
                                'id': "content_" + PlayerMatchupId,
                                'class': "ui-body"
                            })
                            .append($('<ul/>',
                            {
                                'data-role': "listview",
                                'id': "ulAmounts_" + PlayerMatchupId,
                                'class': "ui-body"
                            })
                            .append($('<li/>', {})
                            .append($('<p/>',
                            {
                                'text': "$"
                            })
                            .append($('<span/>',
                            {
                                'id': "underdogAmount_" + PlayerMatchupId,
                                'text': $(this).find("UnderdogAmount").first().text() + " " + $(this).find("UnderdogGolfer").first().text() + " (" + ($(this).find("UnderdogOdds").first().text() > 0 ? "+" : "") + $(this).find("UnderdogOdds").first().text() + ")"
                            })).css("text-align", "right")
                            .append($('<h5/>',
                            {
                                'text': $(this).find("UnderdogGolfTeam").first().text()
                            }))))
                            .append($('<li/>', {})
                            .append($('<p/>',
                            {
                                'text': "$",
                                'class': "none"
                            })
                            .append($('<span/>',
                            {
                                'id': "favoriteAmount_" + PlayerMatchupId,
                                'text': $(this).find("FavoriteAmount").first().text() + " " + $(this).find("FavoriteGolfer").first().text() + " (" + ($(this).find("FavoriteOdds").first().text() > 0 ? "+" : "") + $(this).find("FavoriteOdds").first().text() + ")"
                            })).css("text-align", "right")
                            .append($('<h5/>',
                            {
                                'text': $(this).find("FavoriteGolfTeam").first().text()
                            }))))))
                            .append($('<div/>',
                            {
                                'data-role': "navbar",
                                'id': "buttonGroup_" + PlayerMatchupId
                            }))
                             .append($('<div/>',
                            {
                                'data-role': "collapsible",
                                'data-collapsed': "true",
                                'data-collapsed-icon': "carat-d",
                                'data-expanded-icon': "carat-u",
                                'data-iconpos': "right",
                                'id': "dbetHistory_" + PlayerMatchupId
                            })
                            .append($('<h5/>',
                            {
                                'text': "Bet History"
                            }))
                            .append($('<ul/>',
                            {
                                'data-role': "listview",
                                'id': "betHistory_" + PlayerMatchupId
                            })))
                            .append($('<a/>',
                            {
                                'data-rel': "close",
                                'class': "ui-btn ui-icon-delete ui-btn-icon-left",
                                'text': "Close"
                            })))
                            .append($('<div/>',
                            {
                                'id': "Hidden_" + PlayerMatchupId
                            }));


                            /*Button Group*/
                            var state = "";
                            if (BetStatus == "Proposed") {
                                 // debugger;
                                if ($(this).find("Proposed").first().text() == "true")
                                    state = "ui-state-disabled";

                                if ($(this).find("BetClosed").first().text() == "true")
                                    state = "ui-state-disabled";

                                $("#buttonGroup_" + PlayerMatchupId)
                                    .append($('<ul/>', {})
                                    .append($('<li/>', {})
                                    .append($('<a/>',
                                    {
                                        'text': "Accept",
                                        'class': "ui-btn ui-shadow ui-btn-inline " + state,
                                        'href': "javascript:BtnAccept_Click('" + PlayerMatchupId + "','" + golfTeamId + "')",
                                        'id': "Accept_" + PlayerMatchupId
                                    })))
                                    .append($('<li/>', {})
                                    .append($('<a/>',
                                    {
                                        'text': "Counter",
                                        'class': "ui-btn ui-shadow ui-btn-inline ui-state-disabled",
                                        'href': "javascript:BtnCounter_Click('" + PlayerMatchupId + "','" + golfTeamId + "')",
                                        'id': "counterOffer_" + PlayerMatchupId
                                    }))));
                            }
                            else {
                                if ($(this).find("BetClosed").first().text() == "true")
                                    state = "ui-state-disabled";

                                $("#buttonGroup_" + PlayerMatchupId)
                                    .append($('<ul/>', {})
                                    .append($('<li/>', {})
                                    .append($('<a/>',
                                    {
                                        'text': "Propose Bet",
                                        'class': "ui-btn ui-shadow ui-btn-inline " + state,
                                        'href': "javascript:BtnPropose_Click('" + PlayerMatchupId + "','" + golfTeamId + "')",
                                        'id': "Propose_" + PlayerMatchupId
                                    }))));
                                }
                                
                             //disable +/- buttons
                                if (state == "ui-state-disabled") {
                                    $("#btnPlus_" + PlayerMatchupId).addClass('ui-state-disabled');
                                    $("#btnMinus_" + PlayerMatchupId).addClass('ui-state-disabled');
                                }
                            /*Insert hidden text boxes to hold values*/
                            $("#Hidden_" + PlayerMatchupId)
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "UnderdogAmountPerDollar_" + PlayerMatchupId,
                                    'value': $(this).find("UnderdogAmountPerDollar").first().text()
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "FavoriteAmountPerDollar_" + PlayerMatchupId,
                                    'value': $(this).find("FavoriteAmountPerDollar").first().text()
                                }))
                            //                                .append($('<input/>',
                            //                                {
                            //                                    'type': "hidden",
                            //                                    'id': "Baseline_" + PlayerMatchupId,
                            //                                    'value': $(this).find("BetAmount").first().text()
                            //                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "Baseline_Original_" + PlayerMatchupId,
                                    'value': $(this).find("BetAmount").first().text()
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "iAmount_" + PlayerMatchupId,
                                    'value': "1.00"
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "underdogGolfer_" + PlayerMatchupId,
                                    'value': $(this).find("UnderdogGolfer").first().text() + " (" + ($(this).find("UnderdogOdds").first().text() > 0 ? "+" : "") + $(this).find("UnderdogOdds").first().text() + ")"
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "favoriteGolfer_" + PlayerMatchupId,
                                    'value': $(this).find("FavoriteGolfer").first().text() + " (" + ($(this).find("FavoriteOdds").first().text() > 0 ? "+" : "") + $(this).find("FavoriteOdds").first().text() + ")"
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "underdogTeam_" + PlayerMatchupId,
                                    'value': $(this).find("UnderdogGolfTeam").first().text()
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "favoriteTeam_" + PlayerMatchupId,
                                    'value': $(this).find("FavoriteGolfTeam").first().text()
                                }))
                                .append($('<input/>',
                                {
                                    'type': "hidden",
                                    'id': "isProposed_" + PlayerMatchupId,
                                    'value': (showDiv == "Proposed" ? "true" : "false")
                                }));



                            //Bet History
                            var i = 0;
                            $(this).find("PlayerMatchupProposal").each(
                            function () {
                                // debugger;
                                i++;
                                $("#betHistory_" + PlayerMatchupId)
                                .append
                                (
                                    $('<li/>', {})
                                    .append
                                    (
                                        $('<p/>',
                                        {
                                            'text': $(this).find("Name").text(),
                                            'class': 'smallHeader'
                                        }).css("font-size", "10px")
                                    )
                                );
                            });

                            // debugger;
                            if (i == 0)
                                $("#dbetHistory_" + PlayerMatchupId).css("visibility", "hidden").css("display", "none");
                            break;

                        case "Completed":
                        case "Closed":
                        case "Accepted": //Accepted
                            var status = betStatus($(this), golfTeamId);
                            $("#d" + BetStatus).css("visibility", "visible").css("display", "inline");
                            // debugger;
                            switch (BetStatus) {
                                case "Completed":
                                    iCompleted++;
                                    if (iCompleted == 1) {
                                        $("#d" + BetStatus)
                                        .append($('<h3/>',
                                        {
                                            'text': BetStatus
                                        })
                                        .append($('<span/>',
                                        {
                                            'id': "TotalWL",
                                            'class': "ui-li-count-total"
                                        })));
                                    }
                                    // debugger;
                                    $("#d" + BetStatus)
                                    .append($('<a/>',
                                    {
                                        'text': ($(this).find("DayNumber").first().text() != "0" ? $(this).find("DayName").first().text().substring(0, 2) + "- " : "E- ") + $(this).find("PlayerMatchupTitle").first().text(),
                                        'href': "#inprogress_" + PlayerMatchupId,
                                        'data-ajax': "false",
                                        'class': "jqm-deeplink jqm-open-quicklink-panel"
                                    })
                                    .append($('<span/>',
                                    {
                                        'text': status[0],
                                        'class': status[1]
                                    })));
                                    break;
                                case "Accepted":
                                    iInProcess++;
                                    if (iInProcess == 1) {
                                        $("#d" + BetStatus)
                                        .append($('<h3/>',
                                        {
                                            'text': BetStatus
                                        })
                                        .append($('<span/>',
                                        {
                                            'id': "AcceptedL",
                                            'class': "ui-li-count-loss"
                                        }))
                                        .append($('<span/>',
                                        {
                                            'id': "AcceptedW",
                                            'class': "ui-li-count-win"
                                        })));
                                    }

                                    $("#d" + BetStatus)
                                    .append($('<a/>',
                                    {
                                        'text': ($(this).find("DayNumber").first().text() != "0" ? $(this).find("DayName").first().text().substring(0, 1) + "- " : "E- ") + $(this).find("PlayerMatchupTitle").first().text(),
                                        'href': "#inprogress_" + PlayerMatchupId,
                                        'data-ajax': "false",
                                        'class': "jqm-deeplink jqm-open-quicklink-panel ui-alt-icon " + status[1]
                                    }));
                                    break;
                            }

                            //                            // debugger;
                            //                            $("#d" + BetStatus)
                            //                                .append($('<a/>',
                            //                                {
                            //                                    'text': ($(this).find("DayNumber").first().text() != "0" ? $(this).find("DayName").first().text().substring(0, 2) + " - " : "Ev - ") + $(this).find("PlayerMatchupTitle").first().text(),
                            //                                    'href': "#inprogress_" + PlayerMatchupId,
                            //                                    'data-ajax': "false",
                            //                                    'class': "jqm-deeplink jqm-open-quicklink-panel"
                            //                                })
                            //                                .append($('<span/',
                            //                                {
                            //                                    'text': status[0],
                            //                                    'class': status[1]
                            //                                })));


                            //debugger;
                            $('#panels').append($('<div/>',
                            {
                                'data-role': "panel",
                                'id': "inprogress_" + PlayerMatchupId,
                                'data-position': "right",
                                'data-display': "overlay",
                                'data-theme': "d"
                            })
                            .append($('<h3/>',
                            {
                                'data-rel': "close",
                                //'class' : "ui-btn ui-icon-delete ui-btn-icon-left",
                                'text': ($(this).find("isEventMatchup").first().text() == "true" ? "Event Matchup" : $(this).find("DayName").first().text() + " Matchup")
                            })).css("color", "#ffefaa")
                            .append($('<ul/>',
                            {
                                'data-role': "listview"
                            })
                            .append($('<li/>', {})
                            .append($('<h3/>',
                            {
                                'text': "$" + $(this).find("UnderdogAmount").first().text() + " " + $(this).find("UnderdogGolfTeam").first().text() //"$7.60 Mrazek"
                            }))
                            .append($('<p/>',
                            {
                                'text': $(this).find("UnderdogGolfer").first().text() + " (" + ($(this).find("UnderdogOdds").first().text() > 0 ? "+" : "") + $(this).find("UnderdogOdds").first().text() + ")"
                            }))
                            .append($('<p/>',
                            {
                                'text': ($(this).find("isEventMatchup").first().text() == "true" ? "Event (" + $(this).find("Underdog").first().find("Round").first().find("TotalScore").text() + ")" : "")
                            }))
                            .append($('<div/>',
                            {
                                'id': "underdogRounds_" + PlayerMatchupId
                            })))
                            .append($('<li/>', {})
                            .append($('<h3/>',
                            {
                                'text': "$" + $(this).find("FavoriteAmount").first().text() + " " + $(this).find("FavoriteGolfTeam").first().text()
                            }))
                            .append($('<p/>',
                            {
                                'text': $(this).find("FavoriteGolfer").first().text() + " (" + ($(this).find("FavoriteOdds").first().text() > 0 ? "+" : "") + $(this).find("FavoriteOdds").first().text() + ")"
                            }))
                            .append($('<p/>',
                            {
                                'text': ($(this).find("isEventMatchup").first().text() == "true" ? "Event (" + $(this).find("Favorite").first().find("Round").first().find("TotalScore").text() + ")" : "")
                            }))
                            .append($('<div/>',
                            {
                                'id': "favoriteRounds_" + PlayerMatchupId
                            })))
                            .append($('<a/>',
                            {
                                'data-rel': "close",
                                'class': "ui-btn ui-icon-delete ui-btn-icon-left",
                                'text': "Close"
                            }))));

                            //debugger;
                            var isEventMatchup = $(this).find("isEventMatchup").first().text();
//                            var DayName = $(this).find("DayName").first().text();
                            $(this).find("Favorite").find("Round").each(
                                function () {
                                   // debugger;
                                    $('#favoriteRounds_' + PlayerMatchupId).append($('<p/>',
                                                            {
                                                                'text': formatScore(
                                                                    $(this).find("RoundScore").text()
                                                                    , $(this).find("TotalScore").text()
                                                                    , $(this).find("TeeTime").text()
                                                                    , $(this).find("Thru").text()
                                                                    , $(this).find("RoundName").text()
                                                                    , $(this).find("DayName").first().text()
                                                                    , $(this).find("RoundGolferEventStatus").text()
                                                                    , (isEventMatchup == "true" ? "EventMatchups" : "")
                                                                    )
                                                            }));
                                });

                            $(this).find("Underdog").find("Round").each(
                                function () {
                                    //debugger;
                                    $('#underdogRounds_' + PlayerMatchupId).append($('<p/>',
                                                            {
                                                                'text': formatScore(
                                                                    $(this).find("RoundScore").text()
                                                                    , $(this).find("TotalScore").text()
                                                                    , $(this).find("TeeTime").text()
                                                                    , $(this).find("Thru").text()
                                                                    , $(this).find("RoundName").text()
                                                                    , $(this).find("DayName").first().text()
                                                                    , $(this).find("RoundGolferEventStatus").text()
                                                                    , (isEventMatchup == "true" ? "EventMatchups" : "")
                                                                    )
                                                            }));
                                });
                            break;
                    }
                    $('.ui-page-active :jqmData(role=content)').trigger('create');
                });

}

function formatScore(rndScore, totalScore, teeTime, thru, roundName, dayName, golferStatus, matchupType) {
      // debugger;
    var rtn = ""
    if (matchupType == "EventMatchups") {
        if (thru != 0) {
            if (thru == 18) {
                if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                    rtn = rtn + golferStatus;
                else
                    rtn = rtn + "\n" + roundName + " (" + rndScore + ")";
            }
            else {
                if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                    rtn = rtn + golferStatus;
                else
                    rtn = rtn + roundName + " (" + rndScore + ") Thru: " + thru;
            }
        }
        else {
            if (roundName == "Rnd 1")
                rtn = rtn + roundName + " Tee Time: " + teeTime;
            else {
                if (teeTime == "" && totalScore == "")
                    rtn = rtn + " ";
                else {
                    if (golferStatus == "Withdraw" || golferStatus == "Missed Cut")
                        rtn = rtn + golferStatus;
                    else
                        rtn = rtn + roundName + ": " + teeTime;
                }
            }
        }
    }
    else {
        if (thru == 18)
            rtn = dayName + " (" + rndScore + ")";
        else
            rtn = dayName + " (" + rndScore + ") " + (thru != 0 ? "Thru: " + thru : "Tee Time: " + teeTime);
    }
    return rtn;
}

function dayStatus(xml, golfTeamId, matchupType) {
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
    var isTie = $(xml).find("MatchupInProgress_Tied").text();
    var isWinning = $(xml).find("MatchupInProgress_Winning").text();
    var scoreDelta = parseInt($(xml).find("MatchupInProgress_ScoreDelta").text());
    var wonMatchup = $(xml).find("WonMatchup").text();
    var LossAmount = $(xml).find("LossAmount").text();
    var WinAmount = $(xml).find("WinAmount").text();

    //                    var playerMatchupId = $(xml).find("PlayerMatchupId").text();
    //                    var underdogGolfTeam = $(xml).find("UnderdogGolfTeam").text();
    //                    var underdogGolfTeamId = $(xml).find("UnderdogGolfTeamId").first().text().toUpperCase();
    //                    var underdogAccepted = $(xml).find("UnderdogAccepted").text();
    //                    var underdogAmount = $(xml).find("UnderdogAmount").text();
    //                    var underdogWon = $(xml).find("UnderdogWon").text();

    //                    var favoriteGolfTeam = $(xml).find("FavoriteGolfTeam").text();
    //                    var favoriteGolfTeamId = $(xml).find("FavoriteGolfTeamId").first().text().toUpperCase();
    //                    var favoriteAccepted = $(xml).find("FavoriteAccepted").text();
    //                    var favoriteAmount = $(xml).find("FavoriteAmount").text();
    //                    var favoriteWon = $(xml).find("FavoriteWon").text();

    //                    var isUnderdog = (golfTeamId.toUpperCase() == underdogGolfTeamId ? "true" : "false");
    //                    var isFavorite = (golfTeamId.toUpperCase() == favoriteGolfTeamId ? "true" : "false");
    //                    var isEventMatchup = $(xml).find("isEventMatchup").text();
    //                    var favoriteScore = parseInt((isEventMatchup=="true"? ($(xml).find("Favorite").first().find("Round").first().find("TotalScore").text() == ""?0:$(xml).find("Favorite").first().find("Round").first().find("TotalScore").text()) : $(xml).find("Favorite").first().find("Round").first().find("RoundScore").text()));
    //                    var underdogScore = parseInt((isEventMatchup=="true"? ($(xml).find("Underdog").first().find("Round").first().find("TotalScore").text() ==""?0:$(xml).find("Underdog").first().find("Round").first().find("TotalScore").text()) : $(xml).find("Underdog").first().find("Round").first().find("RoundScore").text()));
    //$(xml).find("Favorite").first().find("GolferScore").text()
    //                                            if (playerMatchupId.toUpperCase() == "51C42E24-0A10-E311-9307-86AF9538D0B1")
    //                                                debugger;

    var result = [];
    result[2] = "hidden";
    //debugger;
    //                    //Bet Proposed
    //                    if (sbetStatus == "Proposed") {
    //                        result[2] = "visible";
    //                        //Bet Proposed
    //                        if (underdogAccepted == "true" && golfTeamId.toUpperCase() == underdogGolfTeamId) {
    //                            result[1] = "ui-icon-flashing-yellow";
    //                            //result[0] = "Proposed";
    //                            //                                                    result[0] = "Bet Proposed To " + favoriteGolfTeam;
    //                                                    
    //                        }
    //                        if (underdogAccepted == "false" && golfTeamId.toUpperCase() == underdogGolfTeamId) {
    //                            result[1] = ".ui-icon-flashing-red";
    //                            // result[0] = "Offered";
    //                            //result[0] = "Bet Offered By " + favoriteGolfTeam;
    //                        }

    //                        if (favoriteAccepted == "true" && golfTeamId.toUpperCase() == favoriteGolfTeamId) {
    //                            result[1] = "ui-icon-flashing-yellow";
    //                            // result[0] = "Proposed";
    //                            //result[0] = "Bet Proposed To " + underdogGolfTeam;
    //                        }
    //                        if (favoriteAccepted == "false" && golfTeamId.toUpperCase() == favoriteGolfTeamId) {
    //                            result[1] = "ui-icon-flashing-red";
    //                            // result[0] = "Offered";
    //                            //result[0] = "Bet Offered By " + underdogGolfTeam;
    //                        }
    //                                                    
    //                    }

    //Bet Accepted, show winner if there is one.
    if (sbetStatus == "Accepted") {
        // debugger;

        result[0] = "Bet Accepted";
        result[2] = "visible";
        if (isWinning == "false") {
            result[0] = abs(scoreDelta);
            result[1] = "ui-icon-number-" + scoreDelta + "-icon-red";
        }

        if (isWinning == "true") {
            result[0] = abs(scoreDelta);
            result[1] = "ui-icon-number-" + scoreDelta + "-icon-green";
        }

        if (isTie == "true") {
            result[0] = "Even";
            result[1] = "ui-icon-number-0";

        }

    }

    //                    //Bet Active
    //                    if (sbetStatus == "Active") {
    //                        //result[0] = "Waiting for Proposal";
    //                        result[1] = "ui-icon-green-light";
    //                        result[2] = "visible";
    //                    }
    //                    //Bet Declined
    //                    if (sbetStatus == "Declined") {
    //                        result[0] = "Bet Declined";
    //                        result[1] = "ui-icon-cancel";
    //                        result[2] = "visible";

    //                        if (underdogAccepted == "true")
    //                            result[0] = "Bet Declined By " + favoriteGolfTeam;
    //                        if (favoriteAccepted == "true")
    //                            result[0] = "Bet Declined By " + underdogGolfTeam;
    //                                                
    //                    }


    //Won
    if (wonMatchup == "true" && sbetStatus == "Completed") {
        result[0] =  Math.abs(WinAmount);
        result[1] = "ui-li-count-win-small";
        result[2] = "visible";
    }
    //Lost
    if (wonMatchup == "false" && sbetStatus == "Completed") {
        result[0] = Math.abs(LossAmount);
        result[1] = "ui-li-count-loss-small";
        result[2] = "visible";
    }

    //tied bet
    if (isTie == "true" && sbetStatus == "Completed") {
        result[0] = "Tie";
        result[1] = "ui-li-count-win-small";
        result[2] = "visible";
    }

    //Bet Closed Not Accepted
    if (sbetStatus == "Closed") {
        result[0] = "Closed Not Accepted";
        result[1] = "ui-icon-cancel";
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