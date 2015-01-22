<?php
$errorcode = $_SERVER['REDIRECT_STATUS'];
$pagetitle = "Error ".$errorcode;

function curPageURL()
{
    $pageURL = 'http';

    if (!empty($_SERVER['HTTPS'])) {
        if ($_SERVER['HTTPS'] == 'on') {
            $pageURL .= "s";
        }
    }
    $pageURL .= "://";

    if ($_SERVER["SERVER_PORT"] != "80") {
        $pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
    } else {
        $pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
    }

    return $pageURL;
}
?>
<?php include "/home/project-web/processhacker/htdocs/v3/include/header.php"; ?>
<div class="container" style="padding: 30px 15px;">
   <div class="alert alert-danger" role="alert">
        <h4><strong>ERROR <?php echo $errorcode ?>:</strong> <?php echo curPageURL(); ?></h4>
        <ul>
            <li>Please notify the team about this error or try again later.</li>
        </ul>
    </div>
</div>
<?php include "/home/project-web/processhacker/htdocs/v3/include/footer.php"; ?>