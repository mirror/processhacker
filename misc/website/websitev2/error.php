<?php
$errorcode = $_SERVER['REDIRECT_STATUS'];

header("HTTP/1.0 200 OK");

$pagetitle = "Error ".$errorcode; include("header.php"); 

function curPageURL() 
{
    $pageURL = 'http';
    
    if ($_SERVER["HTTPS"] == "on") 
    {
        $pageURL .= "s";
    }
    
    $pageURL .= "://";
     
    if ($_SERVER["SERVER_PORT"] != "80") 
    {
        $pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
    } 
    else 
    {
        $pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
    }
    return $pageURL;
}

?>

<div class="center"> 	
    <div class="top-portlet">
        
        <br>
        <p class="neg">ERROR <?php echo $errorcode ?> : <?php echo curPageURL(); ?></p>

        <p>Send us an e-mail and notify us about this error and try again later.</p>
        
        <p><?php echo $php_errormsg ?></p>
            
        <div class="menu">
            <p><a href="/">Home</a></p>
        </div>
    
    </div>
</div>

<?php include("footer.php"); ?>