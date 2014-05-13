function refreshXml() {
   // debugger;
                        $.ajax(
                        {
                            //                            beforeSend: function () { $.mobile.showPageLoadingMsg(); }, //Show spinner
                            //                            complete: function () { $.mobile.hidePageLoadingMsg() }, //Hide spinner
                            async: false,
                            cache: false,
                            type: "POST",
                            datatype: "text",
                            url: "../Golf.EventDraft.asmx/PlayerMatchupsRefresh",
                            data: {},
                            success: function () { },
                            error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
                        });
                    }