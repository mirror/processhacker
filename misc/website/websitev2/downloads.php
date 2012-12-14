<?php $pagetitle = "Downloads"; include "header.php"; ?>

<div id="page">
    <div class="yui-d0">
        <div class="yui-t4">
            <nav>
                <img class="flowed-block" src="images/logo_64x64.png" alt="Project Logo" width="64" height="64">

                <div class="flowed-block">
                    <h2>Process Hacker</h2>
                    <ul class="facetmenu">
                        <li><a href="/">Overview</a></li>
                        <li class="active"><a href="downloads.php">Downloads</a></li>
                        <li><a href="faq.php">FAQ</a></li>
                        <li><a href="about.php">About</a></li>
                        <li><a href="forums/">Forum</a></li>
                    </ul>
                </div>
            </nav>
            <!--<div class="yui-b side">
                <div class="portlet">
                    <h2 class="center">Quick Links</h2>
                    <ul class="involvement">
                        <li><a href="changelog.php">SVN Changelog</a></li>
                        <li><a href="http://sourceforge.net/projects/processhacker/files/">Sourceforge Downloads</a></li>
                    </ul>
                </div>
                <div class="portlet">
                    <h2 class="center">Get Involved</h2>
                    <ul class="involvement">
                        <li><a href="forums/viewforum.php?f=24">Report a bug</a></li>
                        <li><a href="forums/viewforum.php?f=5">Ask a question</a></li>
                    </ul>
                </div>
            </div>-->

            <div class="main-section-2">
                <p>The latest stable version of Process Hacker is <strong><?php echo $LATEST_PH_VERSION." (r".$LATEST_PH_BUILD.")" ?></strong>.
                The <a href="http://www.reactos.org/wiki/Driver_Signing">ReactOS Foundation</a> has very kindly signed the driver for 64-bit systems.</p>

                <!-- Ad Unit 4 - DO NOT CHANGE THIS CODE -->
                <div style="clear: both; margin-left: auto; margin-right: auto; margin-bottom: 20px; width: 728px; height: 90px;">
                    <?php ad_unit_4(); ?>
                </div>

                <div class="side">
                    <div class="portlet" id="downloads">
                        <h2 class="center">Download</h2>
                        <div id="downloads">
                            <ul>
                                <li><a href="http://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe" title="Installer (recommended)">Installer</a></li>
                                <li><a href="http://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip">Binaries (portable)</a></li>
                                <li><a href="http://processhacker.googlecode.com/files/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip">Source code</a></li>
                            </ul>
                        </div>
                        <div id="all-downloads">
                            <a href="http://sourceforge.net/projects/processhacker/files/processhacker2/">See all downloads</a>
                        </div>
                    </div>
                </div>

                <p><strong>System requirements:</strong></p>
                <ul>
                    <li>Windows XP (SP2)/Vista/7/8, 32-bit or 64-bit.</li>
                    <li>Intel Itanium platforms are not supported.</li>
                </ul>
                <p>If you like this software, please
                <a href="http://sourceforge.net/project/project_donations.php?group_id=242527"><strong>Donate</strong></a>
                to show your support!</p>
            </div>
        </div>
    </div>
</div>

<?php include "footer.php"; ?>
