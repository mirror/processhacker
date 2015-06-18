<?php include "config.php"; include "helpers.php"; /*ob_start('removeWhitespace');*/ ?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8"/>
        <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
        <meta name="viewport" content="width=device-width, initial-scale=1"/>
        <meta name="description" content="Process Hacker, A free, powerful, multi-purpose tool that helps you monitor system resources, debug software and detect malware."/>
        <meta name="author" content="wj32"/>

        <link rel="shortcut icon" href="favicon.ico"/>

        <link rel="stylesheet" type="text/css" href="//cdn.jsdelivr.net/bootstrap/3.3.4/css/bootstrap.min.css"/>
        <link rel="stylesheet" type="text/css" href="//processhacker.sourceforge.net/v3/css/lightbox.css"/>
        <link rel="stylesheet" type="text/css" href="//processhacker.sourceforge.net/v3/css/custom.css"/>

        <link rel="alternate" type="application/atom+xml" href="http://processhacker.sourceforge.net/forums/feed.php?f=1" title="Process Hacker - News"/>
        <link rel="alternate" type="application/atom+xml" href="http://sourceforge.net/p/processhacker/code/feed" title="Process Hacker - SVN"/>

        <title><?php echo $pagetitle ?> - Process Hacker</title>

        <!--[if lt IE 9]>
            <script src="//cdn.jsdelivr.net/html5shiv/3.7.2/html5shiv.min.js"></script>
            <script src="//cdn.jsdelivr.net/respond/1.4.2/respond.min.js"></script> 
        <![endif]-->
        
        <?php if ($pagetitle == 'Overview') {
                echo "<script type=\"text/javascript\" src=\"http://www.google.com/jsapi\"></script>
        <script type=\"text/javascript\">google.load(\"feeds\", \"1\");</script>
        <script type=\"text/javascript\" src=\"js/feed.js\"></script>
        <script type=\"text/javascript\" src=\"http://cdn.jsdelivr.net/momentjs/2.10.3/moment.min.js\"></script>";
        } ?>
    </head>
<body>

<nav class="navbar navbar-inverse navbar-fixed-top">
    <div class="container">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar" aria-expanded="false" aria-controls="navbar">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="index.php">
                <img style="float:left" alt="SourceForge logo" title="" width="24" height="24" src="/img/logo_64x64.png">
                <span style="padding-left:5px;">Process Hacker</span>
            </a>
        </div>
        <div id="navbar" class="navbar-collapse collapse">
            <ul class="nav navbar-nav navbar-right">
                <li <?php if ($pagetitle == 'Overview') echo "class=\"active\"" ?>><a href="index.php">Overview</a></li>
                <li <?php if ($pagetitle == 'Downloads') echo "class=\"active\"" ?>><a href="downloads.php">Downloads</a></li>
                <li <?php if ($pagetitle == 'FAQ') echo "class=\"active\"" ?>><a href="faq.php">FAQ</a></li>
                <li <?php if ($pagetitle == 'About') echo "class=\"active\"" ?>><a href="about.php">About</a></li>
                <li><a href="/forums">Forum</a></li>
            </ul> 
        </div>
    </div>
</nav>