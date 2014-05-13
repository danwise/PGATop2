function loadXml(data,filter) {

    $.ajax(
        {
            type: "GET",
            datatype: "xml",
            url: "../xmlConfig.xml",
            data: {},
            async: false,
            cache: false,
            success: function (xml) {
                // var output = $(xml).find("CurrentXML").first().text();
                //set last updated cookie
                if ($.cookie("LastRefreshTime") != null)
                    $.cookie("LastRefreshTime", null);

                $.cookie("LastRefreshTime", $(xml).find("CreatedOn").text(), { expires: 1000, path: '/' });

                if (data == "Event")
                    xmlParser(xml, filter);
                else
                    getEventData(xml, data, filter);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
               // alert(errorThrown);
            }
        });

}

function getEventData(xml, data, filter) {
    try {
        var xmlLocation = "../" +  $(xml).find("CurrentXML").first().text();

        //debugger;
        switch (data) {
            case "Leaderboard":
                xmlLocation = "../xml/" + $.cookie("GolfEventId") + "_Golfers.xml";
                break;
            case "BestBall":
                if(filter == "Thurs")
                    xmlLocation = "../xml/" + $.cookie("GolfEventId") + "_GolfTeams_BestBallThurs.xml";
                else
                    xmlLocation = "../xml/" + $.cookie("GolfEventId") + "_GolfTeams_BestBallFri.xml";
                break;
            case "TeamTop2":
                xmlLocation = "../xml/" + $.cookie("GolfEventId") + "_GolfTeams.xml";
                break;
            case "Golfer":
                xmlLocation = "../xml/golfer/" + $.cookie("GolfEventId") + "_Golfer_" + filter + ".xml";
                break;
            case "Skins":
                xmlLocation = "../xml/" + $.cookie("GolfEventId") + "_Skins.xml";
                break;
        }
        
        $.ajax(
            {
                type: "GET",
                datatype: "xml",
                async: false,
                cache: false,
                url:  xmlLocation,
                data: {},
                success: function (xml) { xmlParser(xml, filter); },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
    }
    catch (e) {
        window.alert("Unable to load the requested file.");
        return;
    }
}