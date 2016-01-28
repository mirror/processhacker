<?php $pagetitle = "Downloads"; include "include/header.php"; ?>

<div class="jumbotron">
    <div class="container">
      	<h1>Process Hacker <?php echo $LATEST_PH_VERSION ?> <small class="footer">r<?php echo $LATEST_PH_BUILD ?></small></h1>
        <div class="footer">Released <?php echo $LATEST_PH_RELEASE_DATE ?></div>
    </div>
</div>

<!-- <div class="container">
    <div class="col-sm-12">
        <div class="alert alert-danger">
            <h4></h4>
        </div>
    </div>
</div> -->

<div class="container">

    <div class="row row-offcanvas row-offcanvas-right">
        <div class="col-xs-12 col-sm-9">
        
        <div class="col-sm-12">
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
        
        <div class="col-sm-12">
            <h3 class="media-heading">Installer (recommended)</h3>
            <p>Prebuilt installer for easy and simple software deployment.</p>
            <p>
                <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe/download?use_mirror=autoselect" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_EXE');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe
                </a>
            </p>
            <!--<p><div class="label label-success">SHA1: <?php echo $LATEST_PH_SETUP_SHA1 ?></div></p>-->
            <hr>
        </div>
        
        <div class="col-sm-12">
            <h3 class="media-heading">Binaries (portable)</h3>
            <p>Compiled and zipped executable (includes plugins) without installer for advanced portable software deployment.</p>
            <p>
                <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip/download?use_mirror=autoselect" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_BIN');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip
                </a>
            </p>
            <!--<p><div class="label label-primary">SHA1: <?php echo $LATEST_PH_BIN_SHA1 ?></div></p>-->
            <hr>
        </div>
        
        <div class="col-sm-12">
            <h3 class="media-heading">Source code</h3>
            <p>Raw source code; Requires a compiler such as Visual Studio and some configuration.</p>
            <p>
                <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip/download?use_mirror=autoselect" class="btn btn-primary" role="button" onclick="ga('send', 'event', 'DownloadPage', 'click', 'Download_SRC');">
                    <i class="glyphicon glyphicon-download-alt"></i> processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip
                </a>
            </p>
            <!--<p><div class="label label-danger">SHA1: <?php echo $LATEST_PH_SOURCE_SHA1 ?></div></p>-->
            <hr>
        </div>
        
        </div>
    
        <div class="col-xs-6 col-sm-3 sidebar-offcanvas">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Quick Links</h3>
                </div>
                <div class="list-group">
                    <a href="<?php echo $LATEST_PH_RELEASE_NEWS; ?>" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-comment"></i> Release Notes</a>
                    <!--<a href="http://svn.code.sf.net/p/processhacker/code/2.x/trunk/LICENSE.txt" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-comment"></i> End User License Agreement</a>
                    <a href="" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-comment"></i> Checksums</a>-->
                    <a href="forums/viewforum.php?f=5" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-comment"></i> Ask a question</a>
                    <a href="forums/viewforum.php?f=24" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-fire"></i> Report a bug</a>
                    <a href="http://sourceforge.net/projects/processhacker/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-globe"></i> SourceForge project page</a>
                    <a href="http://sourceforge.net/p/processhacker/code/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-check"></i> Browse source code</a>
                    <a href="http://processhacker.sourceforge.net/doc/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-edit"></i> Source code documentation</a>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>