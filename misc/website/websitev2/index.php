<?php
$pagetitle = "Overview";
include "header.php";

// Connect to DB
$conn = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbNameRo);
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
                        <li class="active"><a href="/">Overview</a></li>
                        <li><a href="downloads.php">Downloads</a></li>
                        <li><a href="faq.php">FAQ</a></li>
                        <li><a href="about.php">About</a></li>
                        <li><a href="forums/">Forum</a></li>
                    </ul>
                </div>
            </nav>

            <p class="headline main-headline">A <strong>free</strong>, powerful, multi-purpose tool that helps you <strong>monitor system resources</strong>,<br>
            <strong>debug software</strong> and <strong>detect malware</strong>.</p>

            <div class="pre-section">
                <!-- Ad Unit 3 - DO NOT CHANGE THIS CODE -->
                <div style="float: left; width: 336px; height: 280px;">
                    <?php ad_unit_3(); ?>
                </div>

                <div class="yui-b side">
                    <div class="portlet downloads">
                        <div class="version">
                            <ul>
                                <li>Process Hacker <?php echo $LATEST_PH_VERSION ?></li>
                                <li>Released <?php echo $LATEST_PH_RELEASE_DATE ?></li>
                            </ul>
                        </div>
                        <ul>
                            <li><a href="downloads.php">Download v<?php echo $LATEST_PH_VERSION." (r".$LATEST_PH_BUILD.")" ?></a></li>
                        </ul>
                        <div class="center donate">
                            <a href="http://sourceforge.net/project/project_donations.php?group_id=242527">
                                <img src="img/donate.png" alt="Donate" width="92" height="26">
                            </a>
                        </div>
                    </div>

                    <div class="portlet quick-links">
                        <h2 class="center">Quick Links</h2>
                        <ul class="involvement">
                            <li><a href="http://sourceforge.net/projects/processhacker/">SourceForge project page</a></li>
                            <li><a href="forums/viewforum.php?f=5">Ask a question</a></li>
                            <li><a href="forums/viewforum.php?f=24">Report a bug</a></li>
                            <li><a href="http://sourceforge.net/p/processhacker/code/">Browse source code</a></li>
                            <li><a href="doc/">Source code documentation</a></li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="main-section">
                <p class="section-header">Main features</p>
                <p class="headline">A detailed overview of system activity with highlighting.</p>
                <img src="img/screenshots/main_window.png" alt="Main window" width="676" height="449">

                <p class="headline">Graphs and statistics allow you quickly to track down resource hogs and runaway processes.</p>
                <img src="img/screenshots/sysinfo_trimmed_1.png" alt="System information summary" width="700" height="347">
                <p class="tip">Tip: Use Ctrl+I to view system performance information.
                Move your cursor over a graph to get a tooltip with information about the data point under your cursor.
                You can double-click the graph to see information about the process at that data point, even if the process is no longer running.</p>

                <p class="headline">Can't edit or delete a file? Discover which processes are using that file.</p>
                <img src="img/screenshots/find_handles.png" alt="Find handles" width="650" height="199">
                <p class="tip">Tip: Use Ctrl+F to search for a handle or DLL.
                If all else fails, you can right-click an entry and close the handle associated with the file. However, this
                should only be used as a last resort and can lead to data loss and corruption.</p>

                <p class="headline">See what programs have active network connections, and close them if necessary.</p>
                <img src="img/screenshots/network.png" alt="Network connections" width="641" height="296">

                <p class="headline">Get real-time information on disk access.</p>
                <img src="img/screenshots/disk_tab.png" alt="Disk tab" width="621" height="325">
                <p class="tip">Tip: This may look very similar to the Disk Activity feature in Resource Monitor, but Process Hacker has a few more features!</p>

                <p class="section-header">Advanced features</p>
                <p class="headline">View detailed stack traces with kernel-mode, WOW64 and .NET support.</p>
                <img src="img/screenshots/thread_stack.png" alt="Stack trace" width="524" height="384">
                <p class="tip">Tip: Hover your cursor over the first column (with the numbers) to view parameter and line number information when available.</p>

                <p class="headline">Go beyond services.msc: create, edit and control services.</p>
                <img src="img/screenshots/services.png" alt="Service properties" width="604" height="468">
                <p class="tip">Tip: By default, Process Hacker shows entries for drivers in addition to normal user-mode services. You can turn this off
                by checking <strong>View &gt; Hide Driver Services</strong>.</p>

                <p class="headline">And much more...</p>
                <img src="img/screenshots/menu.png" alt="Service properties" width="637" height="518">

                <p class="headline">Other additions</p>
                <p class="normal">Many of you have probably used Process Explorer in the past. Process Hacker has several advantages:</p>
                <ul class="normal">
                    <li>Process Hacker allows you to copy data by simply pressing Ctrl+C.</li>
                    <li>Process Hacker is open source and can be modified or redistributed.</li>
                    <li>Process Hacker does not have several year old bugs that still remain unfixed.</li>
                    <li>Process Hacker is more customizable.</li>
                    <li>Process Hacker shows symbolic access masks (e.g. <code>Read</code>, <code>Write</code>), rather than just numbers (e.g. <code>0x12019f</code>).</li>
                </ul>

                <p class="headline bottom-download"><strong><a href="downloads.php?bottom=1">Download &gt;</a></strong></p>
            </div>
            <br/> <!-- this br div is a placeholder -->

            <div class="yui-g">
                <div class="yui-u first">
                    <div class="portlet">
                        <p><strong>Latest News</strong></p>
                        <?php
                            // Check connection
                            if (mysqli_connect_errno())
                            {
                                echo "<p>Failed to connect to MySQL: " . mysqli_connect_error()."</p>";
                            }
                            else
                            {
                                $sql =
                                    "SELECT t.topic_id,
                                        t.topic_title,
                                        t.topic_last_post_id,
                                        t.forum_id,
                                        p.post_id,
                                        p.poster_id,
                                        p.post_time,
                                        u.user_id,
                                        u.username,
                                        u.user_colour
                                    FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
                                    WHERE t.topic_id = p.topic_id AND
                                        f.forum_id = t.forum_id AND
                                        t.forum_id = 1 AND
                                        t.topic_status <> 2 AND
                                        p.post_id = t.topic_last_post_id AND
                                        p.poster_id = u.user_id
                                    ORDER BY p.post_id DESC LIMIT $topicnumber";

                                if ($result = mysqli_query($conn, $sql))
                                {
                                    while ($row = mysqli_fetch_array($result))
                                    {
                                        $topic_title = $row["topic_title"];
                                        $author_name = $row["username"];
                                        $author_colour = $row["user_colour"];

                                        $post_time = $row["post_time"];
                                        $post_local_time = date("F jS, Y, g:i a", $post_time);
                                        $post_date = get_time_ago($post_time);
                                        $post_id = $row["post_id"];

                                        $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$row["user_id"];
                                        $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$post_id."#p".$post_id;

                                        echo
                                        "<div>
                                            <a href=\"{$post_link}\">{$topic_title}</a>
                                            <span class=\"forumdate\"> by <span style=\"color:#{$author_colour}\">{$author_name}</span></span>
                                            <div class=\"forumdate\">{$post_date} - {$post_local_time}</div>
                                        </div>";
                                    }

                                    mysqli_free_result($result);
                                }
                            }
                        ?>
                    </div>
                </div>

                <div class="yui-g">
                    <div class="portlet">
                        <p><strong>Forum Activity</strong></p>
                        <?php
                            // Check connection
                            if (mysqli_connect_errno())
                            {
                                echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."</p>";
                            }
                            else
                            {
                                $sql =
                                    "SELECT t.topic_id,
                                        t.topic_title,
                                        t.topic_last_post_id,
                                        t.forum_id,
                                        p.post_id,
                                        p.poster_id,
                                        p.post_time,
                                        u.user_id,
                                        u.username,
                                        u.user_colour
                                    FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
                                    WHERE t.topic_id = p.topic_id AND
                                        t.topic_approved = 1 AND
                                        f.forum_id = t.forum_id AND
                                        t.forum_id != 1 AND
                                        t.forum_id != 7 AND
                                        t.topic_status <> 2 AND
                                        p.post_approved = 1 AND
                                        p.post_id = t.topic_last_post_id AND
                                        p.poster_id = u.user_id
                                    ORDER BY p.post_id DESC LIMIT $topicnumber";

                                if ($result = mysqli_query($conn, $sql))
                                {
                                    while ($row = mysqli_fetch_array($result))
                                    {
                                        $topic_title = $row["topic_title"];
                                        $author_name = $row["username"];
                                        $author_colour = $row["user_colour"];

                                        $post_time = $row["post_time"];
                                        $post_local_time = date("F jS, Y, g:i a", $post_time);
                                        $post_date = get_time_ago($post_time);
                                        $post_id = $row["post_id"];

                                        $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$row["user_id"];
                                        $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$post_id."#p".$post_id;

                                        echo
                                        "<div>
                                            <a href=\"{$post_link}\">{$topic_title}</a>
                                            <span class=\"forumdate\"> by <span style=\"color:#{$author_colour}\">{$author_name}</span></span>
                                            <div class=\"forumdate\">{$post_date} - {$post_local_time}</div>
                                        </div>";
                                    }

                                    mysqli_free_result($result);
                                }
                            }
                        ?>
                    </div>
                </div>
                <div class="yui-u">
                    <div id="structural-subscription-content-box"></div>
                </div>
            </div>

            <div class="yui-g">
                <div class="yui-u">
                    <div class="portlet">
                        <p><strong>SVN Activity</strong></p>
                        <div id="feeddiv"></div>
                        <script src="js/moment.min.js"></script>
                        <script>
                            var feedcontainer=document.getElementById("feeddiv");
                            var rssoutput = "";

                            feedcontainer.innerHTML = "<div id=\"feeddiv2\">Loading commit history...</div>";

                            function rssfeedsetup() {
                                var feedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
                                feedpointer.setNumEntries(5);
                                feedpointer.load(displayfeed);
                            }

                            function displayfeed(result) {
                                if (!result.error) {
                                    var thefeeds = result.feed.entries;
                                    for (var i = 0; i < thefeeds.length; i++) {
                                        rssoutput += "<div>";
                                        rssoutput += "<a href=\" " + thefeeds[i].link + " \">" +
                                                        thefeeds[i].title.replace("/p/processhacker/code/", "http://sourceforge.net/p/processhacker/code/") +
                                                     "</a>";
                                        rssoutput += "<span class=\"forumdate\"> by <span style=\"color:#A00\">" + thefeeds[i].author + "</span></span>";
                                        rssoutput += "<div class=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
                                        rssoutput += "</div>";
                                    }

                                    feedcontainer.innerHTML = rssoutput;
                                } else {
                                    feedcontainer.innerHTML = "Error fetching feeds!";
                                }
                            }

                            window.onload = function() {
                                rssfeedsetup();
                            };
                        </script>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- AddThis Button BEGIN -->
    <div class="addthis_toolbox addthis_default_style" style="position:absolute; top:10px; right:0" addthis:url="http://processhacker.sourceforge.net">
        <a class="addthis_button_facebook_like" fb:like:layout="button_count"></a>
        <a class="addthis_button_tweet"></a>
        <a class="addthis_button_google_plusone" g:plusone:size="medium"></a>
        <a class="addthis_counter addthis_pill_style"></a>
    </div>
    <!-- AddThis Button END -->
</div>

<?php
if ($conn)
{
    mysqli_close($conn);
}

include "footer.php";
?>
