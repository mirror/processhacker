<?php $pagetitle = "Overview"; include("header.php"); ?>

<div class="page" style="position:relative">
    <div class="yui-d0">
        <div class="watermark-apps-portlet">
            <div class="flowed-block">
                <img src="/images/logo_64x64.png" alt="Project Logo" width="64" height="64">
            </div>

            <div class="flowed-block wide">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">
                    <li class="overview active"><a href="/">Overview</a></li>
                    <li><a href="/features.php">Features</a></li>
                    <li><a href="/screenshots.php">Screenshots</a></li>
                    <li><a href="/downloads.php">Downloads</a></li>
                    <li><a href="/faq.php">FAQ</a></li>
                    <li><a href="/about.php">About</a></li>
                    <li><a href="/forums/">Forum</a></li>
                </ul>
            </div>
        </div>

        <div class="yui-t4">
            <div class="yui-b side">
                <div class="portlet">
                    <h2 class="center">Downloads</h2>
                    <div class="downloads">
                        <div class="version">
                            Latest version is <?php echo $LATEST_PH_VERSION." (r".$LATEST_PH_BUILD.")" ?>
                        </div>
                        <ul style="list-style-type: none; padding: 0; margin: 0">
                            <li><a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-setup.exe/download" title="Setup (recommended)">Installer</a></li>
                            <li><a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-bin.zip/download" title="Binaries (portable)">Binaries (portable)</a></li>
                            <li><a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo $LATEST_PH_VERSION ?>-src.zip/download" title="Source code">Source code</a></li>
                        </ul>
                        <div class="released">
                            Released <?php echo $LATEST_PH_RELEASE_DATE ?>
                        </div>
                    </div>
                    <div class="center">
                        <a href="http://sourceforge.net/project/project_donations.php?group_id=242527">
                            <img alt="Donate" width="92" height="26" src="/images/donate.png">
                        </a>
                    </div>
                </div>

                <div class="portlet">
                    <h2 class="center">Quick Links</h2>
                    <ul class="involvement">
                        <li><a href="http://sourceforge.net/projects/processhacker/?source=directory">Sourceforge Project Page</a></li>
                        <li><a href="/forums/viewforum.php?f=5">Ask a question</a></li>
                        <li><a href="/forums/viewforum.php?f=24">Report a bug</a></li>
                        <li><a href="http://sourceforge.net/p/processhacker/code/">Browse source code</a></li>
                        <li><a href="http://processhacker.sourceforge.net/doc/">Source code documentation</a></li>
                    </ul>
                </div>
            </div>

            <div class="top-portlet">
                <div class="summary">
                    <p>Process Hacker is a free and open source process viewer. This multi-purpose tool will assist you with debugging, malware detection and system monitoring. It includes powerful process termination, memory viewing/editing and other unique and specialized features.</p>
                    <p><strong>Key features of Process Hacker:</strong></p>
                    <ul>
                        <li> • A simple, customizable tree view with highlighting showing you the processes running on your computer.</li>
                        <li> • Detailed system statistics with graphs.</li>
                        <li> • Advanced features not found in other programs, such as detaching from debuggers, viewing GDI handles, viewing heaps, injecting and unloading DLLs, and more.</li>
                        <li> • Powerful process termination that bypasses security software and rootkits.</li>
                        <li> • View, edit and control services, including those not shown by the Services console.</li>
                        <li> • View and close network connections.</li>
                        <li> • Starts up almost instantly, unlike other programs.</li>
                        <li> • <a href="./features.php">Many more features...</a></li>
                    </ul>
                    <p><strong>Compared with Process Explorer, Process Hacker:</strong></p>
                    <ul>
                        <li> • Implements all of the functionality offered by Process Explorer, plus more advanced features.</li>
                        <li> • Allows you to see what a thread is waiting on.</li>
                        <li> • Has advanced string scanning capabilities, as well as regular expression filtering.</li>
                        <li> • Highlights both relocated and .NET DLLs.</li>
                        <li> • Shows symbolic access masks (e.g. <code>Read, Write</code>), rather than just numbers (e.g. <code>0x12019f</code>).</li>
                        <li> • Shows names for transaction manager objects and ETW registration objects.</li>
                        <li> • Shows detailed token information, as well as allowing privileges to be enabled and disabled.</li>
                    </ul>
                </div>
                <br/> <!-- this br div is a placeholder -->
            </div>

            <div class="yui-g">
                <div class="yui-u first">
                    <div class="portlet">
                        <p><strong>Latest News</strong></p>
                        <?php
                            $sql = "SELECT
                            t.topic_id, t.topic_title, t.topic_last_post_id, t.forum_id,
                            p.post_id, p.poster_id, p.post_time,
                            u.user_id, u.username, u.user_colour, u.user_avatar, u.user_avatar_type, u.user_avatar_width, u.user_avatar_height
                            FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
                            WHERE t.topic_id = p.topic_id AND
                            t.topic_approved = 1 AND
                            f.forum_id = t.forum_id AND
                            t.forum_id = 1 AND
                            t.topic_status <> 2 AND
                            p.post_approved = 1 AND
                            p.post_id = t.topic_last_post_id AND
                            p.poster_id = u.user_id
                            ORDER BY p.post_id DESC LIMIT $topicnumber";

                            // Check if we have a valid database connection, preform the query if we do.
                            if (!empty($db) && ($query = $db->sql_query($sql)))
                            {
                                while ($row = $db->sql_fetchrow($query))
                                {
                                    $topic_title = $row['topic_title'];
                                    //$post_text = $row['post_text'];
                                    $author_avatar = $row['user_avatar'];

                                    $post_time = $row["post_time"];
                                    $post_local_time = date('F jS, Y, g:i a', $post_time);

                                    $post_author = get_username_string('full', $row['poster_id'], $row['username'], $row['user_colour']);
                                    $post_date = get_time_ago($post_time);
                                    $post_link = append_sid("{$phpbb_root_path}viewtopic.php", "p=" . $row['post_id'] . "#p" . $row['post_id']);

                                    //$bbcode = new bbcode(base64_encode($row['bbcode_bitfield']));
                                    //$bbcode->bbcode_second_pass($post_text, $row['bbcode_uid'], $row['bbcode_bitfield']);
                                    //$post_text = smiley_text($post_text);
                                    //$post_text = str_replace('&nbsp;','',$post_text);
                                    //$post_text = str_replace('./forums','http://processhacker.sourceforge.net/forums/',$post_text);
                                    //$post_text = substr($post_text, 0, 300);
                                    //if ($author_avatar) $avatar = get_user_avatar($author_avatar, $row['user_avatar_type'], 16, 16);

                                    echo
                                    "<div class=\"ft\">
                                        <a href=\"{$post_link}\">{$topic_title}</a>
                                        <span style='color:#C0C0C0'>by
                                            <span>{$post_author}</span>
                                        </span>
                                        <div class='forumdate'>{$post_date} - {$post_local_time}</div>
                                    </div>";
                                }
                                $db->sql_freeresult($query);
                            }
                            else
                            {
                                // Check if we have a valid database connection.
                                if (!empty($db))
                                {
                                    $error = $db->sql_error();

                                    echo "<p>Query failed: ".$error['message']."</p>";
                                }
                                else
                                {
                                    echo "<p>Query failed: Unknown error.</p>";
                                }
                            }
                        ?>
                    </div>
                </div>

                <div class="yui-g">
                    <div class="portlet">
                        <p><strong>Forum Activity</strong></p>
                        <?php
                            $sql = "SELECT t.topic_id, t.topic_title, t.topic_last_post_id, t.forum_id, p.post_id, p.poster_id, p.post_time, u.user_id, u.username, u.user_colour, u.user_avatar, u.user_avatar_type, u.user_avatar_width, u.user_avatar_height
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

                            // Check if we have a valid database connection, preform the query if we do.
                            if (!empty($db) && ($query = $db->sql_query($sql)))
                            {
                                while ($row = $db->sql_fetchrow($query))
                                {
                                    $topic_title = $row['topic_title'];
                                    //$post_text = nl2br($row['post_text']);
                                    $author_avatar = $row['user_avatar'];

                                    $post_time = $row["post_time"];
                                    $post_local_time = date('F jS, Y, g:i a', $post_time);

                                    $post_author = get_username_string('full', $row['poster_id'], $row['username'], $row['user_colour']);
                                    $post_date = get_time_ago($post_time);
                                    $post_link = append_sid("{$phpbb_root_path}viewtopic.php", "p=" . $row['post_id'] . "#p" . $row['post_id']);
                                    //if ($author_avatar) $avatar = get_user_avatar($author_avatar, $row['user_avatar_type'], 16, 16);

                                    echo
                                    "<div class=\"ft\">
                                        <a href=\"{$post_link}\">{$topic_title}</a>
                                        <span style='color:#C0C0C0'>by
                                            <span>{$post_author}</span>
                                        </span>
                                        <div class='forumdate'>{$post_date} - {$post_local_time}</div>
                                    </div>";
                                }
                                $db->sql_freeresult($query);
                            }
                            else
                            {
                                // Check if we have a valid database connection.
                                if (!empty($db))
                                {
                                    $error = $db->sql_error();

                                    echo "<p>Query failed: ".$error['message']."</p>";
                                }
                                else
                                {
                                    echo "<p>Query failed: Unknown error.</p>";
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
                <div class="yui-u first">
                    <div class="portlet">
                        <a href="/screenshots.php"><p><strong>Screenshots</strong></p></a>
                        <a class="fancybox" data-fancybox-group="gallery" href="/images/screenshots/processes_tab_large.png" title="Main window">
                            <img src="/images/screenshots/processhacker_small.png" alt="Main window" width="200" height="107"/>
                        </a>
                        <a class="fancybox" data-fancybox-group="gallery" href="/images/screenshots/sysinfo_large.png" title="Sysinfo window">
                            <img src="/images/screenshots/sysinfo_small.png" alt="Sysinfo" width="200" height="107"/>
                        </a>
                    </div>
                </div>
                <div class="yui-u">
                    <div class="portlet">
                        <div id="feeddiv"></div>

                        <script>
                            var feedcontainer=document.getElementById("feeddiv")
                            var rssoutput = "<p><strong>SVN Activity</strong></p>"

                            feedcontainer.innerHTML = "<div id=\"feeddiv2\">Loading feed...</div>"

                            function rssfeedsetup()
                            {
                                var feedpointer=new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed")
                                feedpointer.setNumEntries(3)
                                feedpointer.load(displayfeed)
                            }

                            function displayfeed(result)
                            {
                                if (!result.error)
                                {
                                    var thefeeds = result.feed.entries
                                    for (var i = 0; i < thefeeds.length; i++)
                                    {
                                        rssoutput += "<p><div class='ft'>"
                                            rssoutput += "<div style=\"color:#333\">" + thefeeds[i].content.replace("/p/processhacker/code/", "http://sourceforge.net/p/processhacker/code/"); + "</div>"
                                            rssoutput += "<span style=\"color:#C0C0C0\"> by </span>"
                                            rssoutput += "<span style=\"color:#A00\">" + thefeeds[i].author + "</span>"
                                            rssoutput += "<div style=\"color:#C0C0C0\">" + new Date(thefeeds[i].publishedDate).toString() + "</div>"
                                        rssoutput += "</div></p>";
                                    }

                                    feedcontainer.innerHTML = rssoutput
                                }
                                else
                                {
                                    feedcontainer.innerHTML = "Error fetching feeds!"
                                }
                            }

                            window.onload=function()
                            {
                                rssfeedsetup()
                            }
                        </script>

                    </div>
                </div>
            </div>
        </div>
    </div>

 <!-- AddThis Button BEGIN -->
 <div class="addthis_toolbox addthis_default_style" style="position:absolute; top:10px; right:0;" addthis:url="http://processhacker.sourceforge.net">
    <a class="addthis_button_facebook_like" fb:like:layout="button_count"></a>
    <a class="addthis_button_tweet"></a>
    <a class="addthis_button_google_plusone" g:plusone:size="medium"></a>
    <a class="addthis_counter addthis_pill_style"></a>
 </div>
 <!-- AddThis Button END -->
</div>

<?php $includejs = true; include("footer.php"); ?>
