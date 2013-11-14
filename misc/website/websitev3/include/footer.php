</div>

<div class="container footer">
    <hr>
    <div class="pull-right">
        <a class="thumbnail" href="http://sourceforge.net/projects/processhacker/">
            <img src="/img/sflogo.png" alt="SourceForge logo" title="Process Hacker is hosted by SourceForge.net" width="120" height="30">
        </a>
    </div>
    <div class="pull-left">
        <p><small>Copyright &copy; 2008-2013 - Wen Jia Liu (wj32)</small></p>
        <a href="index.php">Home</a> | 
        <a href="about.php">About</a> | 
        <a href="members.php">Contact Us</a> | 
        <a href="privacy.php">Privacy Policy</a>
    </div>
</div>
</body>
</html>

<?php
    if (!empty($conn))
    {
        mysqli_close($conn);
    }
?>