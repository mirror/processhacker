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
        <div class="yui-t4">
            <nav>
                <div class="logo">
                    <a href="/"><img class="flowed-block" src="/img/logo_64x64.png" alt="Project Logo" width="64" height="64"></a>
                </div>

                <div class="flowed-block">
                    <h2>Process Hacker</h2>
                    <ul class="facetmenu">
                        <li class="active"><a href="/">Overview</a></li>
                        <li><a href="/downloads.php">Downloads</a></li>
                        <li><a href="/faq.php">FAQ</a></li>
                        <li><a href="/about.php">About</a></li>
                        <li><a href="/forums/">Forum</a></li>
                    </ul>
                </div>
            </nav>
			
            <div class="summary center">
                <p><strong>ERROR <?php echo $errorcode ?>:</strong> <?php echo curPageURL(); ?></p>
                <p><strong>Please notify the team about this error or try again later.</strong></p>
                <p>Contact information is available on the About page.</p>
            </div>
        </div>
    </div>
</div>

<footer>
    <a href="http://sourceforge.net/projects/processhacker/"><img src="/img/sflogo.png" alt="SourceForge logo" title="Process Hacker is hosted by SourceForge.net" width="120" height="30"></a>
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