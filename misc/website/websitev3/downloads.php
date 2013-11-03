<?php $pagetitle = "Downloads"; include "include/header.php"; ?>

<div class="jumbotron">
    <h1>Process Hacker <?php echo $LATEST_PH_VERSION ?></h1>
    <p class="headline main-headline">
        Released <?php echo $LATEST_PH_RELEASE_DATE ?>
    </p>
</div>

<div class="row">
    <div class="col-md-6">
        <div class="row">
                    <hr>            
                    <div class="col-md-12">
                        <div class="media-body">
                            <h4 class="media-heading"><p>Installer (recommended)</p></h4>
                        </div>
                        
                        <button class="btn btn-primary btn-lg" data-toggle="modal" data-target="#myModal">
                            processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip <i class="glyphicon glyphicon-download"></i>
                        </button>
                        
                        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                        <h4 class="modal-title" id="myModalLabel">Installer (recommended)</h4>
                                    </div>
                                    <div class="modal-body">
                                        <p><span class="label label-info"><?php echo $LATEST_PH_SETUP_SHA1 ?></span></p>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                        <a class="btn btn-primary" href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe">
                                            Download <i class="glyphicon glyphicon-download-alt"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <br/>
                
                <div class="row">
                    <hr>            
                    <div class="col-md-12">
                        <div class="btn-group pull-right">
                            <a class="btn btn-primary" href="https://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.exe" title="Binaries (portable)">
                                Download <i class="glyphicon glyphicon-download-alt"></i>
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
                                Download <i class="glyphicon glyphicon-download-alt"></i>
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
                        Download <i class="glyphicon glyphicon-download-alt"></i>
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


<?php include "include/footer.php"; ?>