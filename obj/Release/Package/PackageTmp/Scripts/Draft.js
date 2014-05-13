function draftGolfer(Id) {
    debugger;
    $.ajax(
                        {
                            type: "POST",
                            datatype: "xml",
                            url: "http://localhost:62133/webservice1.asmx/DraftGolfer",
                            data: { EventOddsId: Id },
                            success: function (xml) { xmlParser(xml); },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert(errorThrown);
                            }
                        });
                    }

function successFunc(data, status){  15.        // parse it as object  16.        var lankanListArray = JSON.parse(data.d);  17.  18.        // creating html string  19.        var listString = '<ul data-role="listview" id="customerList">';  20.  21.        // running a loop  22.        $.each(lankanListArray, function(index,value){  23.         listString += '<li><a href="#" >'+this.Name+'</a></li>';  24.        });  25.        listString +='</ul>';  26.  27.        //appending to the div  28.        $('#LankanLists').html(listString);  29.  30.        // refreshing the list to apply styles  31.        $('#LankanLists ul').listview();  32.    }  