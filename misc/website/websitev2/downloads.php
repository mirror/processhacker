<?php $pagetitle = "Downloads"; include("header.php"); ?>

<div class="page">
    <div class="yui-d0">
        <div class="watermark-apps-portlet">
            <div class="flowed-block">
                <img src="/images/logo_64x64.png" alt="Project Logo" width="64" height="64">
            </div>

            <div class="flowed-block">
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
                <div class="summary">
                    <p>The latest stable version of Process Hacker is <strong><?php echo $LATEST_PH_VERSION." (r".$LATEST_PH_BUILD.")" ?></strong></p>
                    <p>The <a href="http://www.reactos.org/wiki/Driver_Signing">ReactOS Foundation</a> has very kindly signed the driver so it works on 64-bit systems.</p>
                    <p><strong>System Requirements:</strong></p>
                    <ul>
                        <li>Microsoft Windows XP SP2 or above, 32-bit or 64-bit.</li>
                        <li>Intel Itanium platforms are not supported.</li>
                    </ul>
                </div>
            </div>

            <div class="yui-g">
                <div class="yui-u first">
                    <div class="portlet">
                        <p><strong>Setup (recommended)</strong></p>
                        <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe/download" title="Setup (recommended)">
                            processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe (<?php echo $LATEST_PH_SETUP_SIZE ?>)
                        </a>
                        <p>SHA1: <?php echo $LATEST_PH_SETUP_SHA1 ?></p>
                    </div>
                    <div class="portlet">
                        <p><strong>Binaries (portable)</strong></p>
                        <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip/download" title="Binaries (portable)">
                            processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip (<?php echo $LATEST_PH_BIN_SIZE ?>)
                        </a>
                        <p>SHA1: <?php echo $LATEST_PH_BIN_SHA1 ?></p>
                    </div>
                    <div class="portlet">
                        <p><strong>Source Code</strong></p>
                        <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip/download" title="Source Code">
                            processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip (<?php echo $LATEST_PH_SOURCE_SIZE ?>)
                        </a>
                        <p>SHA1: <?php echo $LATEST_PH_SOURCE_SHA1 ?></p>
                    </div>
                    <div class="portlet">
                        <p><strong>Plugins SDK</strong></p>
                        <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip/download" title="Plugins SDK">
                            processhacker-<?php echo $LATEST_PH_VERSION ?>-sdk.zip (<?php echo $LATEST_PH_SDK_SIZE ?>)
                        </a>
                        <p>SHA1: <?php echo $LATEST_PH_SDK_SHA1 ?></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>
