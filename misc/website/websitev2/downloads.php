<?php
$pagetitle = "Downloads";
include "header.php";

function sf_link($type)
{
    global $LATEST_PH_VERSION;
    return "http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-$LATEST_PH_VERSION-$type/download?use_mirror=autoselect";
}

function wj32_link($type)
{
    global $LATEST_PH_VERSION;
    return "http://wj32.org/processhacker/rel/processhacker-$LATEST_PH_VERSION-$type";
}

$load_number = 5;
$which_server = rand(1, $load_number);
$show_mirror = true;

if ($which_server == 1) {
    $show_mirror = false;
    $link_setup = wj32_link("setup.exe");
    $link_bin = wj32_link("bin.zip");
    $link_src = wj32_link("src.zip");
} else {
    $link_setup = sf_link("setup.exe");
    $link_bin = sf_link("bin.zip");
    $link_src = sf_link("src.zip");
}
?>

<div class="page">
    <div class="yui-d0">
        <div class="yui-t4">
            <nav>
                <div class="logo">
                    <a href="/"><img class="flowed-block" src="img/logo_64x64.png" alt="Project Logo" width="64" height="64"></a>
                </div>

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

                <!-- Ad Unit 4 - DO NOT CHANGE THIS CODE -->
                <div style="clear: both; margin-left: auto; margin-right: auto; margin-bottom: 20px; width: 728px; height: 90px;">
                    <?php ad_unit_4(); ?>
                </div>

                <div class="side">
                    <div class="portlet downloads">
                        <h2 class="center">Download</h2>
                        <div class="downloads">
                            <ul>
                                <li><a href="<?php echo $link_setup ?>" title="Installer (recommended)">Installer</a></li>
                                <li><a href="<?php echo $link_bin ?>">Binaries (portable)</a></li>
                                <li><a href="<?php echo $link_src ?>">Source code</a></li>
                            </ul>
                        </div>
                        <div class="all-downloads">
                            <?php if($show_mirror) : ?><a href="http://wj32.org/processhacker/rel/">Mirror</a><br><?php endif; ?>
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

                <!-- Ad Unit 6 - DO NOT CHANGE THIS CODE -->
                <div style="clear: both; margin-left: auto; margin-right: auto; margin-top: 20px; width: 336px; height: 280px;">
                    <?php ad_unit_6(); ?>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include "footer.php"; ?>
