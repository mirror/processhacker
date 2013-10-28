<?php $pagetitle = "Downloads"; include "include/header.php"; ?>

<div class="row row-offcanvas row-offcanvas-right">
    <div class="col-xs-12 col-sm-9">             
        <div class="jumbotron">
            <h1>Process Hacker <?php echo $LATEST_PH_VERSION ?></h1>
            <p class="headline main-headline">
                Released <?php echo $LATEST_PH_RELEASE_DATE ?>
            </p>
        </div>

        <div class="row">
            <small>The <a href="http://www.reactos.org/wiki/Driver_Signing">ReactOS Foundation</a> has very kindly signed the driver for 64-bit systems.</small>
        </div>

        <div class="row">
            <div class="col-md-6">
                <div class="row">
                    <hr>
                    <div class="col-md-12">
                        <div class="btn-group pull-right">
                            <a class="btn btn-primary" href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe">
                                Download
                            </a>
                        </div>
                        <div class="media-body">
                            <h4 class="media-heading">Installer (recommended)</h4>
                            <small><cite title="Source Title">processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe</cite></small>
                            <p><span class="label label-primary"><?php echo $LATEST_PH_SETUP_SHA1 ?></span></p>
                        </div>
                    </div>
                </div>
                
                <br/>
                
                <div class="row">
                    <hr>            
                    <div class="col-md-12">
                        <div class="btn-group pull-right">
                            <a class="btn btn-primary" href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.exe" title="Binaries (portable)">
                                Download
                            </a>
                        </div>
                        <div class="media-body">
                            <h4 class="media-heading">Binaries (portable)</h4>
                            <small><cite title="Source Title">processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip</cite></small>
                            <p><span class="label label-success"><?php echo $LATEST_PH_BIN_SHA1 ?></span></p>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="col-md-6">
                <div class="row">
                    <hr>            
                    <div class="col-md-12">
                        <div class="btn-group pull-right">
                            <a class="btn btn-primary" href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip" title="Source code">
                                Download
                            </a>
                        </div>
                        <div class="media-body">
                            <h4 class="media-heading">Source code</h4>
                            <small><cite title="Source Title">processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip</cite></small>
                            <p><span class="label label-primary"><?php echo $LATEST_PH_SOURCE_SHA1 ?></span></p>
                        </div>
                    </div>
                </div>
                
                <br/>
                
                <div class="row">
                    <hr>            
                    <div class="col-md-12">
                        <div class="btn-group pull-right">
                            <a class="btn btn-primary" href="https://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip" title="Plugin SDK">
                                Download
                            </a>
                        </div>
                        <div class="media-body">
                            <h4 class="media-heading">Plugin SDK</h4>
                            <small><cite title="Source Title">processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip</cite></small>
                            <p><span class="label label-danger"><?php echo $LATEST_PH_SDK_SHA1 ?></span></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xs-6 col-sm-3 sidebar-offcanvas" id="sidebar" role="navigation">
          <div class="well sidebar-nav">
            <ul class="nav">
                <li><a href="http://sourceforge.net/projects/processhacker/">SourceForge project page</a></li>
                <li><a href="forums/viewforum.php?f=5">Ask a question</a></li>
                <li><a href="forums/viewforum.php?f=24">Report a bug</a></li>
                <li><a href="http://sourceforge.net/p/processhacker/code/">Browse source code</a></li>
                <li><a href="doc/">Source code documentation</a></li>
            </ul>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>