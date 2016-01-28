$(document).ready(function ()
    {
        startTime();        insertDate();    });function insertDate()
{
    var today = new Date();
    var t = today.getDay();
    var m = today.getMonth();
    var j = today.getFullYear();
    document.getElementById('datum').innerHTML = t + "." + m + "." + j;
}function startTime() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    m = checkTime(m);
    s = checkTime(s);
    document.getElementById('uhrzeit').innerHTML = h + ":" + m + ":" + s;
    var t = setTimeout(startTime, 500);
}function checkTime(i)
{
    if (i < 10)
    {
        i = "0" + i
    };
    return i;
}