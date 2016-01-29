var paramether;

$(document).ready(function ()
    {
        paramether = getUrlParameter('room');
        if (paramether != null)
        {
            document.getElementById('saal').innerHTML = paramether;
            getData(paramether);
        }

        setInterval(function () {
            window.location.reload();
        }, 5 * 60000);
    }
);

function getData(sParam)
{
    $.ajax({
        type: 'GET',
        url: 'http://10.100.105.27:1053/room=' + sParam,
        success: 
        function(data)
        {
            DisplayResult(data);
        },
        error:
        function(data)
        {

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
            if (isCurrent(data[i]))
            {
                html += '<tr> \
                        <td> \
                            <div class="currenLecture"> \
                                <table class="lectureTable"> \
                                    <tr> \
                                        <td class="currenLecturelvbezeichnung" colspan="2">' + data[i]["LVBezeichnung"] + '</td> \
                                    </tr> \
                                    <tr> \
                                        <td class="currenLectureVonBis">' + getTime(data[i]["Von"]) + "<br />" + getTime(data[i]["Bis"]) + '</td> \
                                        <td class="currenLecturelvart">' + data[i]["LVArt"] + '</td> \
                                    </tr> \
                                </table> \
                            </div> \
                        </td> \
                    </tr>';
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
                                        <td class="nextLectureVonBis">' + getTime(data[i]["Von"]) + "<br />" + getTime(data[i]["Bis"]) + '</td> \
                                    </tr> \
                                </table> \
                            </div> \
                        </td> \
                    </tr>';
            }
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
                                    <td class="nextLectureVonBis">' + getTime(data[i]["Von"]) + "<br />" + getTime(data[i]["Bis"]) + '</td> \
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

function isCurrent(data)
{
    var now = new Date();
    var start = new Date(data["Datum"].substring(0, 4), data["Datum"].substring(5, 7) - 1, data["Datum"].substring(8, 10), data["Von"].substring(0, 2), data["Von"].substring(3, 5), "00");
    var end = new Date(data["Datum"].substring(0, 4), data["Datum"].substring(5, 7) - 1, data["Datum"].substring(8, 10), data["Bis"].substring(0, 2), data["Bis"].substring(3, 5), "00");

    if (start <= now && now <= end)
    {
        return true;
    }

    return false;
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