<?php
$errorcode = $_SERVER['REDIRECT_STATUS'];
$pagetitle = "Error ".$errorcode;

include "config.php";

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

    if ($errorcode != 403) {
        if (!empty($_SERVER['HTTP_REFERER'])) {
            $referringSite = $_SERVER['HTTP_REFERER'];
            // this is a second type for logging the referer if the request come from another site
            add_log('critical', 'LOG_ERROR_PAGE_REF', $errorcode, curPageURL(), $referringSite);
        } else {
            add_log('critical', 'LOG_ERROR_PAGE', $errorcode, curPageURL());
        }
    }
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <title><?php echo $pagetitle ?> - Process Hacker</title>
    <meta charset="utf-8"/>
    <meta name="description" content="A free and open source process viewer with powerful process termination and memory searching/editing capabilities."/>
    <link rel="icon" type="image/ico" href="/favicon.ico">
    <link rel="stylesheet" href="/css/pack.css"/>
    <link rel="alternate" type="application/atom+xml" href="http://processhacker.sourceforge.net/forums/feed.php?f=1" title="Process Hacker - News"/>
    <link rel="alternate" type="application/atom+xml" href="http://sourceforge.net/p/processhacker/code/feed" title="Process Hacker - SVN"/>
    <!--[if lt IE 9]>
    <script src="js/html5shiv.js"></script>
    <![endif]-->
</head>
<body>
<div class="page">
    <div class="yui-d0">
        <nav>
            <div class="logo">
                <a href="/"><img class="flowed-block" src="img/logo_64x64.png" alt="Project Logo" width="64" height="64"></a>
            </div>

            <div class="flowed-block">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">
                    <li><a href="/">Overview</a></li>
                    <li><a href="features.php">Features</a></li>
                    <li><a href="screenshots.php">Screenshots</a></li>
                    <li><a href="downloads.php">Downloads</a></li>
                    <li><a href="faq.php">FAQ</a></li>
                    <li><a href="about.php">About</a></li>
                    <li><a href="forums/">Forum</a></li>
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

<footer>
    <a href="http://sourceforge.net/projects/processhacker/"><img src="img/sflogo.png" alt="SourceForge logo" title="Process Hacker is hosted by SourceForge.net" width="120" height="30"></a>
    <br>
    <a href="privacy.php">Privacy Policy</a>
    <br>
    Copyright &copy; 2008-2012 wj32

<!-- Google Analytics (Async)-->
<script>
    var _gaq = _gaq || [];
    _gaq.push(['_setAccount', 'UA-22023876-1']);
    _gaq.push(['_trackPageview']);
    (function() {
        var ga = document.createElement('script');
        ga.type = 'text/javascript';
        ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0];
        s.parentNode.insertBefore(ga, s);
    })();
</script>
<!-- End Google Analytics -->
</footer>
</body>
</html>