<div class="center">
    <img src="/images/sflogo.png" width="120" height="30" alt="SourceForge logo" title="Process Hacker is hosted by SourceForge.net">
    <br>
    Copyright &copy; 2008-2012 wj32
</div>

<?php
if (@$includejs)
{
    echo "<script src=\"http://www.google.com/jsapi\"></script><script>google.load(\"feeds\", \"1\")</script>
    <script src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js\"></script>
    <script src=\"/js/pack.js\"></script>
    <script>window.jQuery || document.write('<script src=\"js/jquery-1.8.3.min.js\"><\/script>')</script>";
    if ($pagetitle == "Overview") {
        echo
        "<script src=\"http://s7.addthis.com/js/300/addthis_widget.js#pubid=dmex\"></script>";
    }
    echo
    "<script>
    $(document).ready(function() {
        $(\".fancybox\").fancybox({});
    });
    </script>";
}
?>

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
</body>
</html>