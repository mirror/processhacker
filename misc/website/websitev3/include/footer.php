    <hr>
    <div style="float:right">
        <a href="http://sourceforge.net/projects/processhacker/">
            <img src="/img/sflogo.png" alt="SourceForge logo" title="Process Hacker is hosted by SourceForge.net" width="120" height="30">
        </a>
    </div>
</div>
    
    <script src="js/jquery-2.0.3.min.js"></script>
    <script src="js/bootstrap.js"></script>
    <!--[if lt IE 9]>
        <script src="js/html5shiv.js"></script>
        <script src="js/respond.min.js"></script>
    <![endif]-->
    <?php if ($pagetitle == 'Overview') {
        echo "<script type=\"text/javascript\" src=\"https://www.google.com/jsapi\"></script>
    <script type=\"text/javascript\">google.load(\"feeds\", \"1\");</script>
    <script src=\"js/feed.js\"></script>
    <script src=\"js/moment.js\"></script>";
    } ?>
    
    </body>
</html>

<?php
    if (!empty($conn))
    {
        mysqli_close($conn);
    }
?>