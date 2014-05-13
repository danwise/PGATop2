        $("input[type='radio']").bind("change", function (event, ui) {
                //debugger;
                //Parse out PlayerMatchupId from Id for this
                var i = 0;
                switch ($(this)[0].id) {
                    case "radio-choice-h-6a":
                        i = 1;
                        $('#iAmount').val(i);
                        break;
                    case "radio-choice-h-6b":
                        i = 5;
                        $('#iAmount').val(i);
                        break;
                    case "radio-choice-h-6c":
                        i = 10;
                        $('#iAmount').val(i);
                        break;
                    case "radio-choice-h-6d":
                        $('#Baseline').val($("#Baseline_Original").val());
                        CalculateBetAmounts();
                        $("#counterOffer").addClass('ui-state-disabled');
                        break;
                }
            });



            function BtnPlus_Click(PlayerMatchupId) {
              //  debugger;
                var increment_value = $("#iAmount_" + PlayerMatchupId).val();
                var baseline_value_pre = $("#Baseline_" + PlayerMatchupId).val();
                $("#Baseline_" + PlayerMatchupId).val(parseInt(baseline_value_pre) + parseInt(increment_value));
                CalculateBetAmounts();
            }

            function BtnMinus_Click(PlayerMatchupId) {
               // debugger;
                var increment_value = $("#iAmount_" + PlayerMatchupId).val();
                var baseline_value_pre = $("#Baseline_" + PlayerMatchupId).val();
                var amt = parseInt(baseline_value_pre) - parseInt(increment_value);
             //   debugger;
                if (amt < 0)
                    amt = 0;
                $("#Baseline_" + PlayerMatchupId).val(amt);
                CalculateBetAmounts();
            }

            function CalculateBetAmounts(PlayerMatchupId) {
              //  debugger;
                var baseline_value = $("#Baseline_" + PlayerMatchupId).val();
                var underdogPerDollar_value = $("#UnderdogAmountPerDollar_" + PlayerMatchupId).val();
                var favoritePerDollar_value = $("#FavoriteAmountPerDollar_" + PlayerMatchupId).val();
                var underdogAmount = parseInt(baseline_value) * underdogPerDollar_value;
                underdogAmount = underdogAmount.toFixed(2);
                var favoriteAmount = parseInt(baseline_value) * favoritePerDollar_value;
                favoriteAmount.toFixed(2);
                //debugger;
                $("#underdogAmount_" + PlayerMatchupId).text(underdogAmount);
                $("#favoriteAmount_" + PlayerMatchupId).text(favoriteAmount);

                $("#counterOffer_" + PlayerMatchupId).removeClass('ui-state-disabled');
            }
            
           