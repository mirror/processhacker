<?php include "config.php"; 
    $conn = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbNameRo); 
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8"/>
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <meta name="description" content="Process Hacker, A free and open source process viewer with powerful process termination and memory searching/editing capabilities."/>

        <link rel="shortcut icon" href="favicon.ico">
        <!-- Bootstrap CSS -->
        <link href="css/bootstrap.css" rel="stylesheet">
        <!-- Custom template CSS -->
        <link href="css/custom.css" rel="stylesheet">

        <title><?php echo $pagetitle ?> - Process Hacker</title>
    </head>
<body>

<div class="navbar navbar-inverse navbar-fixed-top">
    <div class="container">
        <div class="navbar-collapse collapse">
            <ul class="nav navbar-nav">
                <li <?php if ($pagetitle == 'Overview') echo "class=\"active\"" ?>><a href="index.php">Overview</a></li>
                <li <?php if ($pagetitle == 'Features') echo "class=\"active\"" ?>><a href="features.php">Features</a></li>
                <li <?php if ($pagetitle == 'Downloads') echo "class=\"active\"" ?>><a href="downloads.php">Downloads</a></li>
                <li <?php if ($pagetitle == 'FAQ') echo "class=\"active\"" ?>><a href="faq.php">FAQ</a></li>
                <li <?php if ($pagetitle == 'About') echo "class=\"active\"" ?>><a href="about.php">About</a></li>
                <li><a href="/forums">Forum</a></li>
                
                <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown">Other pages <b class="caret"></b></a>
                    <ul class="dropdown-menu unstyled pull-left dropdown-features">
                        <li <?php if ($pagetitle == 'News') echo "class=\"active\"" ?>><a href="news.php">Project News</a></li>
                        <li <?php if ($pagetitle == 'Changelog') echo "class=\"active\"" ?>><a href="changelog.php">Project Changelog</a></li>
                        <li <?php if ($pagetitle == 'Members') echo "class=\"active\"" ?>><a href="members.php">Project Members</a></li>
                        <li <?php if ($pagetitle == 'Plugins') echo "class=\"active\"" ?>><a href="plugins.php">Project Plugins</a></li>
                    </ul>
                </li>
            </ul>
          
            <form class="navbar-form navbar-right">
                <!-- AddThis Button BEGIN -->
                <div class="addthis_toolbox addthis_default_style addthis_32x32_style" style="padding-top: 2px;" addthis:url="http://processhacker.sourceforge.net">
                    <a class="addthis_button_facebook"></a>
                    <a class="addthis_button_twitter"></a>
                    <a class="addthis_button_google_plusone_share"></a>
                    <a class="addthis_button_compact"></a>
                </div>
                <script type="text/javascript" src="http://s7.addthis.com/js/300/addthis_widget.js#pubid=dmex"></script>
                <!-- AddThis Button END -->
            </form>
            
            <div style="float: right;padding-top: 12px;">
                <a href="http://sourceforge.net/project/project_donations.php?group_id=242527">
                    <img src="../img/donate.png" alt="Donate" width="92" height="26">
                </a>
            </div>
        </div>
    </div>
</div>

<div class="container">
