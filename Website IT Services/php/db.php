<?php
    $myServer = "datafhdbF1";
    $myUser = "www_stuplan";
    $myPass = "Export_stuplan";
    $myDB = "FHDB"; 

    //connection to the database
    $dbhandle = mssql_connect($myServer, $myUser, $myPass)
      or die("Couldn't connect to SQL Server on $myServer"); 

    //select a database to work with
    $selected = mssql_select_db($myDB, $dbhandle)
      or die("Couldn't open database $myDB"); 

    //declare the SQL statement that will query the database
    $query = "Select TOP(3) * from V_STUPLAN;";

    //execute the SQL query and return records
    $result = mssql_query($query);

    $numRows = mssql_num_rows($result); 
    echo "<h1>" . $numRows . " Row" . ($numRows == 1 ? "" : "s") . " Returned </h1>"; 

    //display the results 
    while($row = mssql_fetch_array($result))
    {
      echo "<li>" . $row["Datum"] . $row["Saal"] . $row["Bezeichnung"] . "</li>";
    }
    //close the connection
    mssql_close($dbhandle);
?>