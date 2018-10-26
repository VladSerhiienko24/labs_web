$(document).ready(function () {

    var myVar;

    function myFunction() {
        myVar = setTimeout(showPage, 500);
    }

    function showPage() {
        document.getElementById("loader").style.display = "none";
        document.getElementById("container-players").style.display = "block";
    }

    myFunction();

    $.ajax({
        type: "POST",
        url: "/api/PlayerAPI/GetAllPlayers",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.each(data.players, function (i, item) {
                var rows = "<tr>" +
                    "<td id='Id'>" + item.id + "</td>" +
                    "<td id='FirstName'>" + item.nickName + "</td>" +
                    "<td id='IsBot'>" + item.playerRole + "</td>" +
                    "<td id='Coins'>" + item.coins + "</td>" +
                    "</tr>";
                $('#Table').append(rows);
            });
        },
        failure: function (data) {
            alert(data.responseText);
        },
        error: function (data) {
            alert(data.responseText);
        }
    })
});