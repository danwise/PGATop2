function loadTeam(Id) {

    if ($.cookie("GolfTeamId") != null)
        $.cookie("GolfTeamId", null);

    if ($.cookie("GolfTeamName") != null)
        $.cookie("GolfTeamName", null);

    $.cookie("GolfTeamId", Id, { expires: 1000, path: '/' });

    $.ajax(
                        {
//                            complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
                            type: "POST",
                            datatype: "xml",
                            url: "../Golf.EventDraft.asmx/GolfTeamIdToName",
                            data: { gtId: Id },
                            success: function (xml) { updateGolfTeam(xml); },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert(errorThrown);
                            }
                        });
}

function updateGolfTeam(xml) {
    $.cookie("GolfTeamName", $(xml).find("string").text(), { expires: 1000, path: '/' });
    $("#teamName").text($.cookie("GolfTeamName"));
}