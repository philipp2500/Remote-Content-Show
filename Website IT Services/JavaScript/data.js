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
        url: '127.0.0.1:1053/room=' + sParam,
        success: 
        function(data)
        { 
            DisplayResult(data);
        },
        contentType: "application/json",
        dataType: 'json'
    });
}

function DisplayResult(data)
{

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