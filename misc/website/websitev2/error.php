<?php
$errorcode = $_SERVER['REDIRECT_STATUS'];
$pagetitle = "Error ".$errorcode;

include("header.php");

function curPageURL()
{
    $pageURL = 'http';

    if (!empty($_SERVER['HTTPS']))
    {
        if ($_SERVER['HTTPS'] == 'on')
        {
            $pageURL .= "s";
        }
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
    // the .htaccess file redirects all PHP site errors to this page.
    // config.php includes the forum phpbb functions for the index page forum activity/news query,
    // one of these forum functions is add_log and we can log errors into the mysql error database.
    // the error log is availabe on the Maintenance tab > Error Log in the forum Admin Control Panel
    // this way all site errors get logged instead of just phpbb errors
    // it works exactly like printf however the string must be defined in /forums/language/en/acp/common.php
    // 1st param is the log type
    // 2nd param is the string defined in /forums/language/en/acp/common.php as 'LOG_ERROR_PAGE' => '<strong>ERROR PAGE</strong> - %d<br/>» %s',
    // 3rd param is %d for errorcode - defined above
    // 4th param is %s for the current page - defined above

    if ($errorcode != 403)
    {
        if (!empty($_SERVER['HTTP_REFERER']))
        {
            $referringSite = $_SERVER['HTTP_REFERER'];
            // this is a second type for logging the referer if the request come from another site
            add_log('critical', 'LOG_ERROR_PAGE_REF', $errorcode, curPageURL(), $referringSite);
        }
        else
        {
            add_log('critical', 'LOG_ERROR_PAGE', $errorcode, curPageURL());
        }
    }
?>

<div class="page">
    <div class="yui-d0">
        <nav>
            <div class="flowed-block">
                <img src="/images/logo_64x64.png" alt="Project Logo" width="64" height="64">
            </div>

            <div class="flowed-block">
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
        </nav>

        <div class="yui-t4">
            <div class="summary center">
                <p><strong>ERROR <?php echo $errorcode ?>:</strong> <?php echo curPageURL(); ?></p>
                <p><strong>Please notify the team about this error or try again later.</strong></p>
                <p>Contact information is available on the About page.</p>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>
