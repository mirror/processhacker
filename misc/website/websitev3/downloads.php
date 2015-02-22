<?php $pagetitle = "Downloads"; include "include/header.php"; ?>

<div class="jumbotron">
    <div class="container">
      	<h1>Process Hacker <?php echo $LATEST_PH_VERSION ?> <small class="footer">r<?php echo $LATEST_PH_BUILD ?></small></h1>
        <div class="footer">Released <?php echo $LATEST_PH_RELEASE_DATE ?></div>
    </div>
</div>

<div class="container">
    <div class="alert alert-info" role="alert">
        <h4>Supported Operating Systems:</h4>
        <p>
            <ul>
                <li>Windows XP, Vista, 7, 8, 10, 32-bit or 64-bit.</li>
                <li>ARM and Itanium platforms are not supported.</li>
            </ul>
        </p>
    </div>
</div>

<div class="container">
    <div class="row">
        <div class="col-sm-8">
            <h3 class="media-heading">Installer (recommended)</h3>
            <p>Prebuilt installer for easy and simple software deployment.</p>
            <p>
                <a href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_EXE');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe
                </a>
            </p>
            <!--<p><div class="label label-success">SHA1: <?php echo $LATEST_PH_SETUP_SHA1 ?></div></p>-->
            <hr>
        </div>
        
        <div class="col-sm-8">
            <h3 class="media-heading">Binaries (portable)</h3>
            <p>Compiled and zipped executable (includes plugins) without installer for advanced portable software deployment.</p>
            <p>
                <a href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_BIN');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip
                </a>
            </p>
            <!--<p><div class="label label-primary">SHA1: <?php echo $LATEST_PH_BIN_SHA1 ?></div></p>-->
            <hr>
        </div>
        
        <div class="col-sm-8">
            <h3 class="media-heading">Source code</h3>
            <p>Raw source code; Requires a compiler such as Visual Studio and some configuration.</p>
            <p>
                <a href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_SRC');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip
                </a>
            </p>
            <!--<p><div class="label label-danger">SHA1: <?php echo $LATEST_PH_SOURCE_SHA1 ?></div></p>-->
            <hr>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>