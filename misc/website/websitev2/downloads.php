<?php
$pagetitle = "Downloads";

include("header.php");
?>

<div class="page">
    <div class="yui-d0">
        <div class="watermark-apps-portlet">
            <div class="flowed-block">
                <img alt="ProjectLogo" width="64" height="64" src="/images/logo_64x64.png">
            </div>
            
            <div class="flowed-block wide">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">
                    <li><a href="/">Overview</a></li>
                    <li><a href="/features.php">Features</a></li>
                    <li><a href="/screenshots.php">Screenshots</a></li>
                    <li class="overview active"><a href="/downloads.php">Downloads</a></li>
                    <li><a href="/faq.php">FAQ</a></li>
                    <li><a href="/about.php">About</a></li>
                    <li><a href="/forums/">Forum</a></li>
                </ul>
            </div>
        </div>
        
        <div class="yui-t4">
            <div class="yui-b side">
                <div class="portlet" >
                    <h2 class="center">Quick Links</h2>
                    <ul class="involvement">
                        <li><a href="/changelog.php">SVN Changelog</a></li>
                        <li><a href="http://sourceforge.net/projects/processhacker/files/">Sourceforge Downloads</a></li>
                    </ul>
                </div>
                <div class="portlet" >
                    <h2 class="center">Get Involved</h2>
                    <ul class="involvement">
                        <li><a href="/forums/viewforum.php?f=24">Report a bug</a></li>
                        <li><a href="/forums/viewforum.php?f=5">Ask a question</a></li>
                    </ul>
                </div>
            </div>
            
            <div class="top-portlet">
                <div class="summary" style="padding-bottom:40px;">
                    <p>The latest stable version of Process Hacker is <strong><?php echo $LATEST_PH_VERSION ?></strong></p>
                    <p>The <a href="http://www.reactos.org/wiki/Driver_Signing">ReactOS Foundation</a> has very kindly signed the driver, so it works on 64-bit systems.</p>
                    <p><strong>System Requirements:</strong></p>
                    <ul>
                        <li> • Microsoft Windows XP SP2 or above, 32-bit or 64-bit.</li>
                        <li> • Intel Itanium Platforms are not supported.</li>
                    </ul>
                    <p><strong>Licence:</strong></p>
                    <ul><li> • GNU General Public License version 3.0 <a href="http://processhacker.svn.sourceforge.net/viewvc/processhacker/2.x/trunk/LICENSE.txt">GPLv3</a></li></ul>
                </div>
            </div>
            
            <div class="yui-g">
                <div class="yui-u first">
                    <div class="portlet">
                        <p><strong>Setup (recommended)</strong></p>
                        <p>Size: <?php echo $LATEST_PH_SETUP_SIZE ?></p>
                        <p>SHA1: <?php echo $LATEST_PH_SETUP_SHA1 ?></p>
                        <p>MD5: <?php echo $LATEST_PH_SETUP_MD5 ?></p>
                        <ul style="width: 175px;" class="downloads">
                            <li>
                                <a style="width: 200px;" href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe/download" title="Setup (recommended)">
                                    processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>

                <div class="yui-g">
                    <div class="portlet">
                        <p><strong>Source Code</strong></p>
                        <p>Size: <?php echo $LATEST_PH_SOURCE_SIZE ?></p>
                        <p>SHA1: <?php echo $LATEST_PH_SOURCE_SHA1 ?></p>
                        <p>MD5: <?php echo $LATEST_PH_SOURCE_MD5 ?></p>
                        <ul style="width: 175px;" class="downloads">
                            <li>
                                <a style="width: 200px;" href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip/download" title="Source Code">
                                    processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
                <div class="yui-u">
                    <div id="structural-subscription-content-box"></div>
                </div>
            </div>
            
            <div class="yui-g">
                <div class="yui-u first">
                    <div class="portlet">
                        <p><strong>Binaries (portable)</strong></p>
                        <p>Size: <?php echo $LATEST_PH_BIN_SIZE ?></p>
                        <p>SHA1: <?php echo $LATEST_PH_BIN_SHA1 ?></p>
                        <p>MD5: <?php echo $LATEST_PH_BIN_MD5 ?></p>
                        <ul style="width: 150px;" class="downloads">
                            <li>
                                <a style="width: 200px;" href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip/download" title="Binaries (portable)">
                                    processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip
                                </a>
                            </li>
                        </ul>
                    </div>	
                </div>
                
                <div class="yui-u">
                    <div class="portlet">
                        <p><strong>Plugins SDK</strong></p>
                        <p>Size: <?php echo $LATEST_PH_SDK_SIZE ?></p>
                        <p>SHA1: <?php echo $LATEST_PH_SDK_SHA1 ?></p>
                        <p>MD5: <?php echo $LATEST_PH_SDK_MD5 ?></p> 
                        <ul style="width: 150px;" class="downloads">
                            <li>
                                <a style="width: 200px;" href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip/download" title="Setup (recommended)">
                                    processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>