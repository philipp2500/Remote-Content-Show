var paramether;

$(document).ready(function ()
    {
        paramether = getUrlParameter('room');
        if (paramether != null)
        {
            document.getElementById('saal').innerHTML = paramether;
            getData(paramether);
        }
    });function getData(sParam){
    $.ajax({
        type: 'GET',
        url: 'http://127.0.0.1:1053/room=' + sParam,
        success: 
        function(data)
        {
            DisplayResult(data);
        },
        error:
        function(data)
        {
            alert("dsErrad");
        },
        contentType: "application/json",
        dataType: 'json'
    });
}

function DisplayResult(data)
{
    var count = 0;
    var html = "";
    for (i in data)
    {
        if (count == 0)
        {
            document.getElementById('saalbezeichnung').innerHTML = data[i]["Bezeichnung"];
            if ()
        }
        else
        {
            html += '<tr> \
                    <td> \
                        <div class="nextLecture"> \
                            <table class="lectureTable"> \
                                <tr> \
                                    <td class="nextLecturebezeichnung">' + data[i]["LVBezeichnung"] + '</td> \
                                    <td class="nextLecturelvart">' + data[i]["LVArt"] + '</td> \
                                    <td class="nextLectureDateum">' + getDate(data[i]["Datum"]) + '</td> \
                                    <td class="nextLectureVonBis">' + getTime(data[i]["Von"]) + "<br />" + getTime(data[i]["Von"]) + '</td> \
                                </tr> \
                            </table> \
                        </div> \
                    </td> \
                </tr>';
        }
        count++;
    }

    document.getElementById('contentTable').innerHTML = html;
}

function getTime(sParam)
{
    return sParam.substring(0, 5);
}

function getDate(sParam)
{
   return sParam.substring(8, 10) + "." + sParam.substring(5, 7)  + "." + sParam.substring(0, 4);
}

function getUrlParameter(sParam)
{
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};