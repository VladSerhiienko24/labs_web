$(function () {

    $("#startNewGame").click(function () {
        $('#newGameForm').validate({
            rules: {
                nickName: {
                    required: true,
                    minlength: 2
                },
                countBots: {
                    required: true,
                    regex: "^[1-9]+[0-9]*$",
                    minCountBots: true
                },
                maxCountRounds: {
                    required: true,
                    regex: "^[1-9]+[0-9]*$"
                },
                coinsAtStart: {
                    required: true,
                    regex: "^[1-9]+[0-9]*$"
                },
                reward: {
                    required: true,
                    regex: "^[1-9]+[0-9]*$",
                    minReward: true
                }
            },
            messages: {
                nickName: {
                    required: "Field is required",
                    minlength: "Enter at least 2 characters"
                },
                countBots: {
                    required: "Field is required",
                    regex: 'Value must be integer',
                    minCountBots: "Min count 1 and max 5"
                },
                maxCountRounds: {
                    required: "Field is required",
                    regex: 'Value must be greater than 0 and integer'
                },
                coinsAtStart: {
                    required: "Field is required",
                    regex: 'Value must be greater than 0 and integer'
                },
                reward: {
                    required: "Field is required",
                    regex: 'Value must be greater than 0 and integer',
                    minReward: 'Min reward 50 coins'
                }
            },
            submitHandler: function (form) {

                let object = {
                    nickName: $("#NickName").val(),
                    maxCountRounds: $("#MaxCountRounds").val(),
                    countBots: $("#CountBots").val(),
                    coinsAtStart: $("#CoinsAtStart").val(),
                    reward: $("#Reward").val()
                }
                
                $.ajax({
                    type: "POST",
                    url: "/api/GameAPI/CreateNewGame",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(object),
                    success: function (data) {
                        window.location = 'Table?gameId=' + data;
                    },
                    error: function (data) {
                        showErrorModal(data.responseJSON.exceptionMessage);
                    }
                });

                return false;
            }
        });

    });

    //--------------#region VALIDATORS-------------------

    $.validator.addMethod("regex", function (value, element, regexp) {

        var re = new RegExp(regexp);

        return this.optional(element) || re.test(value);
    });

    $.validator.addMethod("minCountBots", function (value, element) {

        let check = parseInt(value);

        if (check > 0 && check <= 5) {
            return true;
        }

        return false;
    });

    $.validator.addMethod("minReward", function (value, element) {

        let check = parseInt(value);

        if (check >= 50) {
            return true;
        }

        return false;
    });

    //--------------#endregion VALIDATORS-------------------

    function showErrorModal(errorMessage) {
        $(".alert").find(".alert-txt").text(errorMessage);

        $('#errorModal').modal({
            backdrop: 'static',
            keyboard: true,
            show: true
        });
    }
});