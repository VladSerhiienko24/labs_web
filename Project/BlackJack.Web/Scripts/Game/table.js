$(function () {

    showRule();

    //--------------#region MODALS-------------------

    function showRule() {

        $.ajax({
            type: "GET",
            async: false,
            url: "/api/GameAPI/CheckRound",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                "gameId": $('#gameId').val()
            },
            success: function (data) {
                if (!data) {
                    $('#ruleModal').modal({
                        backdrop: 'static',
                        keyboard: false,
                        show: true
                    });
                }

                if (data) {
                    showTable();
                }
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });
    }

    function showRound() {
        return new Promise(resolve => {

            $.ajax({
                type: "GET",
                async: false,
                url: "/api/GameAPI/GetRound",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {
                    "gameId": $('#gameId').val()
                },
                success: function (data) {
                    $('.round').empty();

                    $('#stepId').val(data.roundId);
                    $('.round').append(data.numberOfRound);
                },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });

            $('#stepModal').modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });

            setTimeout(resolve, 2000);
        });
    }

    function showResult() {
        return new Promise(resolve => {
            getWinners();

            $('#resultModal').modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });

        });
    }

    function showHistory() {

        getHistoryOfWins();

        $('#historyModal').modal({
            show: true
        });
    }

    function showErrorModal(errorMessage) {

        $(".alert").find(".alert-txt").text(errorMessage);

        $('#errorModal').modal({
            backdrop: 'static',
            keyboard: true,
            show: true
        });

    }

    function getHistoryOfWins() {

        $.ajax({
            type: "GET",
            url: "/api/GameAPI/GetHistoryOfWins",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                "gameId": $('#gameId').val()
            },
            success: function (data) {
                $('.winners-container').empty();

                $.each(data.winners, function (i, item) {
                    let round = parseInt($('.round').text());

                    if (item.roundNumber != round) {

                        let row;

                        if (item.players.length == 0) {
                            row = '<div class="round-box"><div class="round-box-number"><h4>ROUND ' + item.roundNumber + '</h4></div>' +
                                '<div class="round-box-winners"><h4>Nobody wins</h4></div></div>';
                        }

                        if (item.players.length != 0) {

                            let winners = [];

                            $.each(item.players, function (i, player) {

                                let element = '<h4>' + player.nickName + ' ' + player.points + '</h4>';

                                winners.push(element);
                            });

                            row = '<div class="round-box"><div class="round-box-number"><h4>ROUND ' + item.roundNumber + '</h4></div>' +
                                '<div class="round-box-winners">' + winners.join(" ") + '</div ></div > ';
                        }

                        $('.winners-container').append(row);
                    }
                });

            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });
    }

    $('.history-btn').click(function () {
        showHistory();
    });

    //------------------------#endregion MODALS------------------

    //------------------------#region INITIALIZE GAME TABLE------------------

    $("#startGame").click(function (event) {
        showTable();
    });

    function showTable() {
        setTimeout(function () {
            $('#game-container').show();

            $.ajax({
                type: "GET",
                async: false,
                url: "/api/GameAPI/GetGame",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: { "gameId": $('#gameId').val() },
                success: function (data) {

                    $('#maxRounds').append(data.game.maxCountRounds);
                    $('#reward').append(data.game.reward);

                    $.each(data.players, function (i, item) {

                        let role;
                        let box;
                        let headClass;
                        let container;

                        if (item.playerRole == "Player") {
                            container = ".players-container";
                            role = "player";
                            box = " col-md-6 player-box";
                            headClass = "player-head";

                            let deal = '<div class="player-info-box">' +
                                '<input type="hidden" id="playerId" value="' + item.id + '">' +
                                '<h3>Your cash: <span class="player-cash">' + item.coins + '</span></h3>' +
                                '<h3>Your deal: <span class="player-deal">' + 0 + '</span></h3>' +
                                '</div>';

                            $(".player-info").append(deal);
                        }

                        if (item.playerRole == "Bot") {
                            container = ".players-container";
                            role = "bot";
                            box = " col-md-6 bot-box";
                            headClass = "bot-head";
                        }

                        if (item.playerRole == "Dealler") {
                            container = ".dealler-container";
                            role = "dealler";
                            box = " col-md-12 dealler-box";
                            headClass = "dealler-head";
                        }

                        var row = '<div class="player-info-' + i + box + '">' +
                            '<input type="hidden" id="playerId" value="' + item.id + '">' +
                            '<input type="hidden" class="playerHandId" value="">' +
                            '<input type="hidden" class="playerRole" value="' + role + '">' +
                            '<h4 class="player-name ' + headClass + '">' + item.nickName + '</h4>' +
                            '<div><span class="playerCards"></span></div>' +
                            '<div class="round-info">' +
                            '<ul class="list-group">' +
                            '<li class="list-group-item">' +
                            '<span class="badge playerPoints">0</span>Points</li>' +
                            '</ul>';

                        $(container).append(row);
                    });

                    checkGameIsFinished();
                },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });
        }, 200);
    }

    function checkGameIsFinished() {

        $.ajax({
            type: "GET",
            async: false,
            url: "/api/GameAPI/CheckGameIsFinished",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                "gameId": $('#gameId').val()
            },
            success: function (data) {
                if (!data) {
                    gamelogic();
                }

                if (data) {
                    $('#finishedGameModal').modal({
                        backdrop: 'static',
                        keyboard: false,
                        show: true
                    });
                }
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });
    }

    //------------------------#endregion INITIALIZE GAME TABLE------------------

    //------------------------#region ROUND LOGIC-------------------

    async function gamelogic() {
        await showRound();

        $('#stepModal').modal('hide');

        await playDealsLogic();

        await getPlayersTwoCards();

        await getDeallerCard();
        
        await checkDeallerFirstCard();

        await getDeallerCard();

        extraCards();
    }

    //------------------------#endregion ROUND LOGIC-------------------

    //------------------------#region DEAL LOGIC--------------------

    function playDealsLogic() {
        return new Promise(resolve => {
            setTimeout(async function () {
                let playersId = [];

                let players = $('.players-container').children('[class^=player-info-]');

                for (var i = 0; i < players.length; i++) {
                    let el = $(players[i]);

                    let role = el.find(".playerRole").val();

                    let playerId = el.find("#playerId").val();

                    if (role == "player") {
                        await setDealPlayer(el, playerId);

                        await delay();
                    }

                    if (role == "bot") {
                        await SetDealBotAndDealler(playerId);
                    }

                    playersId.push(playerId);
                }

                let deallerId = $('.dealler-box').find("#playerId").val();

                await SetDealBotAndDealler(deallerId);

                return resolve();
            }, 200);
        });
    }

    function setDealPlayer(el, playerId) {
        return new Promise(resolve => {

            $('.makeDeal').show();

            setTimeout(resolve, 15500);

            timerDealPlayer("on", resolve);

            validateDealPlayerField(playerId, resolve);

        });
    }

    function validateDealPlayerField(playerId, resolve) {

        $('#makeDealForm').validate({
            rules: {
                deal: {
                    required: true,
                    regex: "^[1-9]+[0-9]*$",
                    customValid: true
                }
            },
            messages: {
                deal: {
                    required: "Field is required",
                    regex: "Value must be greater than 0 and integer",
                    customValid: "Don't enough the money!"
                }
            },
            submitHandler: function (form) {

                let deal = parseInt($("#dealCount").val());

                $.ajax({
                    type: "POST",
                    async: false,
                    url: "/api/GameAPI/SetDealPlayer",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({
                        roundId: $("#stepId").val(),
                        playerId: playerId,
                        coins: $("#dealCount").val()
                    }),
                    success: function () {
                        $(".player-deal").empty();

                        $(".player-deal").text(deal);

                        $('.makeDeal').hide();

                        return timerDealPlayer("off", resolve);
                    },
                    error: function () {
                        showErrorModal(data.responseJSON.message);
                    }
                });
            }
        });

        $.validator.addMethod("regex", function (value, element, regexp) {

            var re = new RegExp(regexp);

            return this.optional(element) || re.test(value);
        }
        );

        $.validator.addMethod("customValid", function (value, element) {

            if (!value || value.trim().length === 0) {
                return false;
            }

            let response;

            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/CheckDealPlayer",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    roundId: $("#stepId").val(),
                    playerId: $('.player-info-box').children('#playerId').val(),
                    coins: $("#dealCount").val()
                }),
                dataFilter: function (data) {
                    response = JSON.parse(data);
                }
            });

            return response;
        });
    }

    function SetDealBotAndDealler(playerId) {
        return new Promise(resolve => {
            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/SetDealBotAndDealler",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    roundId: $("#stepId").val(),
                    playerId: playerId
                }),
                success: function () {
                },
                error: function () {
                    showErrorModal(data.responseJSON.message);
                }
            });

            setTimeout(resolve, 100);
        });
    }

    //------------------------#endregion DEAL LOGIC--------------------

    //------------------------#region LOGIC FOR TWO CARDS-------------------

    function getPlayersTwoCards() {
        return new Promise(resolve => {
            let playersId = [];

            let players = $('.players-container').children('[class^=player-info-]');

            for (var i = 0; i < players.length; i++) {
                let el = $(players[i]);

                let playerId = el.find("#playerId").val();

                playersId.push(playerId);
            }

            var dataObj = {
                roundId: $('#stepId').val(),
                players: playersId
            };

            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/GetPlayersTwoCards",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(dataObj),
                success: function (data) {
                    showTwoCards(data.listPlayersWithCards);

                    setTimeout(resolve, 1000 * players.length);
                },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });
        });
    }

    function showTwoCards(data) {
        let delay = 1000;
        let delay2 = 500;

        let boxes = $('.players-container').find('[class^=player-info-]');

        $.each(data, function (i, dataItem) {

            (function (index) {

                setTimeout(function () {

                    for (let i = 0; i <= boxes.length; i++) {
                        let tmp = $('.players-container').find('[class^=player-info-' + i + ']');

                        let id = parseInt(tmp.find('#playerId').val());

                        if (dataItem.playerId == id) {
                            tmp.find('.playerPoints').empty();

                            tmp.find('.playerHandId').val(dataItem.handId);

                            $.each(dataItem.cards, function (y, item) {
                                (function (index) {

                                    setTimeout(function () {

                                        let row = '<section class="cards">' +
                                            '<section class="card" value="' + item.face + '">' +
                                            '<div class="card__inner card__inner--centered">' +
                                            '<div class="card__column">' +
                                            '<div class="card__symbol">' + item.suit + '</div>' +
                                            '</div> </div>' + '</section> </section>';

                                        tmp.find('.playerCards').append(row).show('slow');

                                    }, delay2 * y);

                                })(y);
                            });

                            tmp.find('.playerPoints').append(dataItem.summary);

                            break;
                        }
                    }
                }, delay * index);

            })(i);

        });

    }

    function getDeallerCard() {
        return new Promise(resolve => {

            var dataObj = {
                gameId: parseInt($('#gameId').val()),
                roundId: $('#stepId').val(),
                playerId: $('.dealler-box').find("#playerId").val()
            };

            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/GetDeallerCard",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(dataObj),
                success: function (data) {

                    let delay = 1000;
                    let delay2 = 500;

                    let deallerBox = $('.dealler-box');

                    deallerBox.find('.playerPoints').empty();

                    deallerBox.find('.playerHandId').val(data.handId);

                    let row = '<section class="cards">' +
                        '<section class="card" value="' + data.card.face + '">' +
                        '<div class="card__inner card__inner--centered">' +
                        '<div class="card__column">' +
                        '<div class="card__symbol">' + data.card.suit + '</div>' +
                        '</div> </div>' + '</section> </section>';

                    deallerBox.find('.playerCards').append(row).show('slow');

                    deallerBox.find('.playerPoints').append(data.summary);

                    setTimeout(function () {
                        return resolve();
                    }, 1000);
                },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });
        });
    }

    //------------------------#endregion LOGIC FOR TWO CARDS-------------------

    //------------------------#region LOGIC FOR INSURANCE-----------------

    function checkDeallerFirstCard() {
        return new Promise(resolve => {
            var dataObj = {
                roundId: $('#stepId').val(),
                playerId: $('.dealler-box').find("#playerId").val()
            };

            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/CheckDeallerFirstCard",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(dataObj),
                success: function (data) {
                    
                    if (data) {
                        insuranceLogic(resolve);
                    }

                    if (!data) {
                        return resolve();
                    }
                },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });
        });
    }

    async function insuranceLogic(resolve) {
        let players = $('.players-container').children('[class^=player-info-]');

        for (var i = 0; i < players.length; i++) {
            let el = $(players[i]);

            let playerId = el.find("#playerId").val();
            let role = el.find(".playerRole").val();

            if (role == "player") {
                
                await playerInsurance(playerId, el);
            }

            if (role == "bot") {
                await setBotInsure(playerId);
            }
        }

        return resolve();
    }

    function playerInsurance(playerId, el) {
        return new Promise(resolve => {

            let ability = checkAbilityToInsure(playerId);

            if (ability) {

                let insuranceCoins = getInsureCoins(playerId);

                $('.modalPlayerInsurance').text(insuranceCoins);

                $('.insurance').show();

                timerInsurance("on", resolve);

                let button;

                $(".modalPlayerInsuranceButton").click(function (event) {

                    button = parseInt($(this).val());

                    if (button == 1) {
                        timerInsurance("off", resolve);
                    }

                    if (button == 2) {

                        setPlayerInsure(playerId);

                        timerInsurance("off", resolve);
                    }

                });
            }

            if (!ability) {

                let coins = parseInt(el.find(".playerPoints").text());

                if (coins <= 0) {
                    return resolve();
                }

                $('.insurance-error').show();

                setTimeout(function () {
                    $('.insurance-error').hide();

                    return resolve();
                }, 5000);
            }
        });
    }

    function checkAbilityToInsure(playerId) {

        let dataObj = {
            roundId: $('#stepId').val(),
            playerId: playerId
        };

        let ability;

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/CheckAbilityToInsure",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(dataObj),
            success: function (data) {
                ability = data;
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });

        return ability;
    }

    function getInsureCoins(playerId) {

        let dataObj = {
            roundId: $('#stepId').val(),
            playerId: playerId
        };

        let coins;

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/GetInsureCoins",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(dataObj),
            success: function (data) {
                coins = data;
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });

        return coins;
    }

    function setPlayerInsure(playerId) {

        var dataObj = {
            roundId: $('#stepId').val(),
            playerId: playerId
        };
        
        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/SetInsure",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(dataObj),
            success: function (data) { },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });

    }

    function setBotInsure(playerId) {
        return new Promise(resolve => {

            var dataObj = {
                roundId: $('#stepId').val(),
                playerId: playerId
            };
            
            $.ajax({
                type: "POST",
                async: false,
                url: "/api/GameAPI/SetInsure",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(dataObj),
                success: function (data) { },
                error: function (data) {
                    showErrorModal(data.responseJSON.message);
                }
            });

            setTimeout(resolve, 1000);
        });
    }

    //------------------------#endregion LOGIC FOR INSURANCE-----------------

    //------------------------#region LOGIC FOR EXTRA CARDS-----------------

    async function extraCards() {
        let delay = 1000;

        var ul_players_list = document.querySelector('.players-container').querySelectorAll("[class^=player-info-]");
        var ul_players_array = Array.prototype.slice.call(ul_players_list);

        var ul_dealler_list = document.querySelector('.dealler-container').querySelectorAll("[class^=player-info-]");
        var ul_dealler_array = Array.prototype.slice.call(ul_dealler_list);

        var game_players_list = Array.prototype.concat.call(ul_players_array, ul_dealler_array);

        for (var i = 0; i < game_players_list.length; i++) {
            let el = $(game_players_list[i]);

            let role = el.find(".playerRole").val();
            let playerId = el.find("#playerId").val();

            if (role == "player") {
                let playerPoints = parseInt(el.find(".playerPoints").text());

                await playerExtraCard(el, i, playerId, playerPoints);

                $('.extraCard').hide();
            }

            if (role == "bot") {
                await botExtraCard(el, i, playerId);
            }

            if (role == "dealler") {
                await deallerExtraCard(el, i, playerId);
            }
        }

        await showResult();
    }

    function delay() {
        return new Promise(resolve => setTimeout(resolve, 1000));
    }

    function checkPointsPlayer(playerId) {

        let result;

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/CheckPointsPlayer",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                roundId: $("#stepId").val(),
                playerId: playerId
            }),
            success: function (data) {
                result = data;
            },
            error: function () {
                showErrorModal(data.responseJSON.message);
            }
        });

        return result;
    }

    function playerExtraCard(el, i, playerId, playerPoints) {
        return new Promise(resolve => {
            if (checkPointsPlayer(playerId)) {
                $(".modalPlayerPoints").text(playerPoints);

                $('.extraCard').show();

                timerExtraCard("on");

                let timeout = setTimeout(resolve, 5500);

                let button;

                $(".modalPlayerButton").click(function (event) {
                    button = parseInt($(this).val());

                    if (button == 1) {

                        $('.extraCard').hide();

                        timerExtraCard("off");

                        return resolve();
                    }

                    if (button == 2) {

                        timerExtraCard("reset");

                        getExtraCard(el, i);

                        playerPoints = parseInt(el.find(".playerPoints").text());

                        if (!checkPointsPlayer(playerId)) {

                            $('.extraCard').hide();

                            timerExtraCard("off");

                            return resolve();
                        }

                        $(".modalPlayerPoints").text(playerPoints);

                        clearTimeout(timeout);

                        timeout = setTimeout(resolve, 5500);

                    }

                });
            }
            if (!checkPointsPlayer(playerId)) {
                return resolve();
            }
        });
    }

    function getExtraCard(el, i) {
        let delay = 200;

        var obj = {
            gameId: parseInt($('#gameId').val()),
            handId: parseInt(el.find('.playerHandId').val())
        };

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/GetExtraCard",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(obj),
            success: function (data) {

                let row = '<section class="cards">' +
                    '<section class="card" value="' + data.card.face + '">' +
                    '<div class="card__inner card__inner--centered">' +
                    '<div class="card__column">' +
                    '<div class="card__symbol">' + data.card.suit + '</div>' +
                    '</div> </div>' + '</section> </section>';

                el.find('.playerCards').append(row);

                el.find('.playerPoints').empty();

                el.find('.playerPoints').append(data.summary);

            },
            error: function () {
                showErrorModal(data.responseJSON.message);
            }

        });
    }

    async function botExtraCard(el, i, playerId) {
        await delay();

        var obj = {
            gameId: parseInt($('#gameId').val()),
            handId: parseInt(el.find('.playerHandId').val())
        };

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/GetBotExtraCard",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(obj),
            success: function (data) {
                console.log(data);
                
                let delay2 = 1000;

                if (jQuery.isEmptyObject(data.extraCards)) {
                    return;
                }

                if (!jQuery.isEmptyObject(data)) {

                    $.each(data.extraCards, function (y, item) {

                        (function (index) {

                            setTimeout(function () {
                                let row = '<section class="cards">' +
                                    '<section class="card" value="' + item.card.face + '">' +
                                    '<div class="card__inner card__inner--centered">' +
                                    '<div class="card__column">' +
                                    '<div class="card__symbol">' + item.card.suit + '</div>' +
                                    '</div> </div>' + '</section> </section>';

                                el.find('.playerCards').append(row);

                                el.find('.playerPoints').empty();

                                el.find('.playerPoints').append(item.summary);
                            }, delay2 * y);

                        })(y);

                    });
                }
            },
            error: function (data) {
                
                console.log(data);
                showErrorModal(data.responseJSON.message);
            }
        });

        await delay();
    }

    async function deallerExtraCard(el, i, playerId) {
        await delay();

        var obj = {
            gameId: parseInt($('#gameId').val()),
            handId: parseInt(el.find('.playerHandId').val())
        };

        $.ajax({
            type: "POST",
            async: false,
            url: "/api/GameAPI/GetDeallerExtraCard",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(obj),
            success: function (data) {

                let delay2 = 1000;

                if (jQuery.isEmptyObject(data.extraCards)) {
                    return;
                }

                if (!jQuery.isEmptyObject(data)) {

                    $.each(data.extraCards, function (y, item) {

                        (function (index) {

                            setTimeout(function () {
                                let row = '<section class="cards">' +
                                    '<section class="card" value="' + item.card.face + '">' +
                                    '<div class="card__inner card__inner--centered">' +
                                    '<div class="card__column">' +
                                    '<div class="card__symbol">' + item.card.suit + '</div>' +
                                    '</div> </div>' + '</section> </section>';

                                el.find('.playerCards').append(row);

                                el.find('.playerPoints').empty();

                                el.find('.playerPoints').append(item.summary);
                            }, delay2 * y);

                        })(y);

                    });
                }
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });

        await delay();
    }

    //------------------------#endregion LOGIC FOR EXTRA CARDS-----------------

    //------------------------#region LOGIC FOR WINNERS------------------

    function getWinners() {
        
        $.ajax({
            type: "GET",
            async: false,
            url: "/api/GameAPI/GetWinners",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            traditional: true,
            data: {
                "roundId": $("#stepId").val()
            },
            success: function (data) {
                console.log(data);
                
                if (data.nickName.length == 0) {
                    let nameTable = '<tr><th class="winners"><h3>Nobody wins</h3></th></tr >';

                    $('#resultModal').find('#Table').append(nameTable);
                }

                if (data.nickName.length != 0) {
                    let nameTable = '<tr><th class="winners"><h3>WINNER</h3></th></tr >';

                    $('#resultModal').find('#Table').append(nameTable);

                    let row = '<tr><td><h4>' + data.nickName + ' ' + data.points + ' points</h4></td></tr>';

                    $('#resultModal').find('#Table').append(row);
                }

                if (data.winnersOfGame.length != 0) {
                    let table = '<table class="table" id="tableRewardsWinners"><tr><th><span class="winners winners-reward">REWARDING</span></th></tr></table >';

                    $('#resultModal').find('.rewards').append(table);

                    for (var i = 0; i < data.winnersOfGame.length; i++) {
                        let field = '<tr><td><span>' + data.winnersOfGame[i].nickName + ' +' + data.winnersOfGame[i].cash + '</span></td></tr>'

                        $('#resultModal').find('#tableRewardsWinners').append(field);
                    }

                }
            },
            error: function (data) {
                
                showErrorModal(data.responseJSON.message);
            }
        });

    }

    function updateFiledCoins() {

        $.ajax({
            type: "GET",
            async: false,
            url: "/api/GameAPI/UpdateFiledCoins",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                "playerId": $(".player-info-box").find("#playerId").val()
            },
            success: function (data) {
                $(".player-info-box").find(".player-cash").html("");

                $(".player-info-box").find(".player-cash").text(data);
            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });
    }

    $("#nextRound").click(function (event) {

        setTimeout(function () {
            getHistoryOfWins();
            clearAll();
            updateFiledCoins();
            checkGameIsFinished();
        }, 200);

    });

    $("#finishGame").on("click", function (e) {
        e.preventDefault();

        $.ajax({
            type: "GET",
            url: "/api/GameAPI/FinishGame",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                "gameId": $('#gameId').val()
            },
            success: function (data) {

                if (data) {
                    setTimeout(clearAll, 200);

                    window.location.href = '/Home/Index';
                }

            },
            error: function (data) {
                showErrorModal(data.responseJSON.message);
            }
        });
    });

    function clearAll() {
        $(".modalPlayerButton").unbind("click");
        $(".modalPlayerInsuranceButton").unbind("click");

        $('.playerCards').empty();
        $(".playerCards").html("");
        $(".playerPoints").html("");
        $(".player-deal").html("");
        $("#dealCount").val("");
        $('.playerCards').empty();

        $('#resultModal').find('#Table').html("");
        $('.timerDeal').empty();
        $('.insuranceTimer').empty();
        $('.timer').empty();
    }

    //------------------------#endregion LOGIC FOR WINNERS------------------

    //----------------------#region TIMERS-------------------------------

    let timeExtraCard;
    let interval;

    function timerExtraCard(mode) {

        if (mode == "reset") {
            timeExtraCard = 5;
        }

        if (mode == "on") {
            timeExtraCard = 5;

            interval = setInterval(function () {

                $('.timer').text(timeExtraCard);

                timeExtraCard--;

                if (timeExtraCard === 0) {
                    clearInterval(interval);

                    $('.extraCard').modal('hide');

                    return;
                }

            }, 1000);
        }

        if (mode == "off") {
            clearInterval(interval);

            return;
        }
    }

    function timerDealPlayer(mode, resolve) {
        let timeDealPlayer;
        let intervalDealPlayer;

        if (mode == "on") {
            timeDealPlayer = 15;
            $('.timerDeal').text(timeDealPlayer);

            intervalDealPlayer = setInterval(function () {
                timeDealPlayer--;

                $('.timerDeal').text(timeDealPlayer);

                if (timeDealPlayer === 0) {
                    clearInterval(intervalDealPlayer);

                    $('.makeDeal').hide();

                    $('.timerDeal').empty();

                    return resolve();
                }
            }, 1000);
        }

        if (mode == "off") {
            clearInterval(intervalDealPlayer);

            $('.timerDeal').empty();

            return resolve();
        }
    }

    function timerInsurance(mode, resolve) {
        let timeInsurance;
        let interval;

        if (mode == "on") {
            timeInsurance = 15;

            setTimeout(resolve, 15500);

            interval = setInterval(function () {
                timeInsurance--;

                $('.insuranceTimer').text(timeInsurance);

                if (timeInsurance === 0) {
                    clearInterval(interval);

                    $('.insurance').hide();

                    return resolve();
                }

            }, 1000);
        }

        if (mode == "off") {
            clearInterval(interval);

            $('.insurance').hide();

            return resolve();
        }
    }

    //----------------------#endregion TIMERS-------------------------------
});