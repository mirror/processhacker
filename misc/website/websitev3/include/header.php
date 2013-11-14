<?php include "config.php"; 
   $conn = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbNameRo);  
?>
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8"/>
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <meta name="description" content="Process Hacker, A free, powerful, multi-purpose tool that helps you monitor system resources, debug software and detect malware."/>
                
        <link href="http://netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
        <link href="http://netdna.bootstrapcdn.com/bootstrap/3.0.1/css/bootstrap.min.css" rel="stylesheet" type="text/css">
        
        <link href='http://fonts.googleapis.com/css?family=Open+Sans' rel='stylesheet' type='text/css'>
        
        <link href="favicon.ico" rel="shortcut icon">
        <link href="css/custom.css" rel="stylesheet" type="text/css">
        
        <script src="http://code.jquery.com/jquery-2.0.3.min.js"></script>
        <script src="http://netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
        <script src="http://code.jquery.com/jquery-2.0.3.min.js"></script>
        
        <!--[if lt IE 9]>
            <script src="js/html5shiv.js"></script>
            <script src="js/respond.min.js"></script>
        <![endif]-->

    <?php if ($pagetitle == 'Overview') {
            echo "<script type=\"text/javascript\" src=\"http://www.google.com/jsapi\"></script>
        <script type=\"text/javascript\">google.load(\"feeds\", \"1\");</script>
        <script src=\"js/feed.js\"></script>
        <script src=\"js/moment.js\"></script>";
    } ?>
        <title><?php echo $pagetitle ?> - Process Hacker</title>
    </head>
<body>

<nav class="navbar navbar-inverse navbar-fixed-top" role="navigation">
    <div class="container">
        <div class="row"> 
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
            </div>
            <div class="collapse navbar-collapse" id="collapse">
                <ul class="nav navbar-nav">
                    <li <?php if ($pagetitle == 'Overview') echo "class=\"active\"" ?>><a href="index.php">Overview</a></li>
                    <li <?php if ($pagetitle == 'Features') echo "class=\"active\"" ?>><a href="features.php">Features</a></li>
                    <li <?php if ($pagetitle == 'Downloads') echo "class=\"active\"" ?>><a href="downloads.php">Downloads</a></li>
                    <li <?php if ($pagetitle == 'FAQ') echo "class=\"active\"" ?>><a href="faq.php">FAQ</a></li>
                    <li <?php if ($pagetitle == 'About') echo "class=\"active\"" ?>><a href="about.php">About</a></li>
                    <li><a href="/forums">Forum</a></li>
                </ul>
                <ul class="nav navbar-nav navbar-right">
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle navbar-brand" data-toggle="dropdown"> Process Hacker <b class="caret"></b></a>
                        <ul class="dropdown-menu">
                            <li <?php if ($pagetitle == 'Changelog') echo "class=\"active\"" ?>><a href="changelog.php"><i class="glyphicon glyphicon-cog" style="font-size: smaller"></i> Changelog</a></li>
                            <li <?php if ($pagetitle == 'Members') echo "class=\"active\"" ?>><a href="members.php"><i class="glyphicon glyphicon-user" style="font-size: smaller"></i> Members</a></li>
                            <li <?php if ($pagetitle == 'News') echo "class=\"active\"" ?>><a href="news.php"><i class="glyphicon glyphicon-star" style="font-size: smaller"></i> News</a></li>
                            <li <?php if ($pagetitle == 'Plugins') echo "class=\"active\"" ?>><a href="plugins.php"><i class="glyphicon glyphicon-th" style="font-size: smaller"></i> Plugins</a></li>
                            <li class="divider"></li>
                            <li><a href="../forums/viewforum.php?f=5"><i class="glyphicon glyphicon-comment" style="font-size: smaller"></i> Ask a question</a></li>
                            <li><a href="../forums/viewforum.php?f=24"><i class="glyphicon glyphicon-fire" style="font-size: smaller"></i> Report a bug</a></li> 
                        </ul>
                    </li>
                </ul>   
            </div>
        </div>
    </div>
</nav>

<div class="container">