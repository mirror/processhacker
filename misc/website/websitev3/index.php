<?php $pagetitle = "Overview"; include "include/header.php"; ?>

<div class="row row-offcanvas row-offcanvas-right">
    <div class="col-xs-12 col-sm-9">            
        <div class="container">
            <div class="jumbotron">
                <h1>Process Hacker</h1>
                <p class="headline main-headline">
                    A <strong>free</strong>, powerful, multi-purpose tool that helps you <strong>monitor system resources</strong>, <strong>debug software</strong> and <strong>detect malware</strong>.
                </p>
                <p><a href="features.php" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
            </div>
        </div>
    </div>
</div>

<div class="container">
    <div class="well">
        <ul class="nav nav-list">
            <li class="nav-header">Latest Posts</li>
            <?php
                if (mysqli_connect_errno())
                {
                    echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."</p>";
                }
                else
                {
                    $sql = "SELECT 
                                t.topic_id,
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
                            // Query fields
                            $topic_title = $row["topic_title"];
                            $author_name = $row["username"];
                            $author_colour = $row["user_colour"];
                            $post_time = $row["post_time"];
                            $post_id = $row["post_id"];
                            $author_link = $row["user_id"];
                               
                            // Convert values
                            $post_local_time = date("F jS, Y, g:i a", $post_time);
                            $post_date = get_time_ago($post_time);
                            $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$post_id."#p".$post_id;
                            $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$author_link; 
                            
                            echo
                                "<div><li id=\"forumitem\">
                                    <a href=\"".htmlspecialchars($post_link)."\">".$topic_title."</a>
                                    <p id=\"forumdate\">".$post_date.", ".$post_local_time."
                                        <span id=\"forumdate\"> by <a href=\"".htmlspecialchars($author_link)."\"><span style=\"color:#".$author_colour."\">".$author_name."</span></a></span>
                                    </p>
                                </li><div>";
                        }
                        
                        mysqli_free_result($result);
                    }
                }
                ?>
        </ul>
    </div>
</div>

<div class="container">
    <div class="well">
        <ul class="nav nav-list">
            <li class="nav-header">Latest Releases</li>
            <?php
                    if (mysqli_connect_errno())
                    {
                        echo "<p>Failed to connect to MySQL: " . mysqli_connect_error()."</p>";
                    }
                    else
                    {
                        $sql = 
                            "SELECT 
                                t.topic_id,
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
                                // Query fields
                                $topic_title = $row["topic_title"];
                                $author_name = $row["username"];
                                $author_colour = $row["user_colour"];
                                $post_time = $row["post_time"];
                                $post_id = $row["post_id"];
                                $author_link = $row["user_id"];
                                
                                // Convert values
                                $post_local_time = date("F jS, Y", $post_time);
                                $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$post_id."#p".$post_id;
                                $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$author_link; 
                                
                                echo
                                "<li><div id='forumitem'>
                                    <a href=\"".htmlspecialchars($post_link)."\">".$topic_title."</a>
                                    <span id=\"forumdate\"> by <a href=\"".htmlspecialchars($author_link)."\"><span style=\"color:#".$author_colour."\">".$author_name."</span></a></span>
                                    <div id=\"forumdate\">".$post_local_time."</div>
                                </div></li>";
                            }
                            mysqli_free_result($result);
                        }
                    }
                ?>
        </ul>
    </div>
</div>

<div class="container">
    <div class="well">
        <ul class="nav nav-list">
            <li class="nav-header">Latest Source</li>
            <div id="feeddiv"></div>
        </ul>
    </div>
</div>

<?php include "include/footer.php"; ?>