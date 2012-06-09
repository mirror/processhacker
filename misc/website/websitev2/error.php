<?php

$errorcode = $_SERVER['REDIRECT_STATUS'];
$pagetitle = "Error ".$errorcode; 

include("header.php");

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

<div class="page">
    <div class="yui-d0">
        <div class="watermark-apps-portlet">
            <div class="flowed-block">
                <img alt="ProjectLogo" width="64" height="64" src="/images/logo_64x64.png">
            </div>
            
            <div class="flowed-block wide">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">
                    <li><a href="/">Overview</a></li>
                    <li><a href="/features.php">Features</a></li>
                    <li><a href="/screenshots.php">Screenshots</a></li>
                    <li><a href="/downloads.php">Downloads</a></li>
                    <li><a href="/faq.php">FAQ</a></li>
                    <li><a href="/about.php">About</a></li>
                    <li><a href="/forums/">Forum</a></li>
                </ul>
            </div>
        </div>
        
        <div class="yui-t4">
            <div class="top-portlet">
                <div class="summary center">
                    <p><strong>ERROR <?php echo $errorcode ?>:</strong> <?php echo curPageURL(); ?></p>
                    <p><strong>Please notify the team about this error or try again later.</strong></p>
                    <p>Contact information is available on the About page.</p>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>