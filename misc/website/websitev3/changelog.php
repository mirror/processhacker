<?php $pagetitle = "Changelog"; include "include/header.php"; ?>

<div class="row">
    <h1 class="page-header"><small>Changelog</small></h1>
</div>

<div class="row row-offcanvas row-offcanvas-right">
    <div class="col-xs-12 col-sm-9">            
        <div class="row">
            <p>This is the changelog from Process Hacker's SVN repository and may contain information about unreleased versions of Process Hacker.</p>
            <iframe class="changelog" src="http://svn.code.sf.net/p/processhacker/code/2.x/trunk/CHANGELOG.txt"></iframe>
        </div>
    </div>
    
    <div class="col-xs-6 col-sm-3 sidebar-offcanvas" id="sidebar" role="navigation">
        <div class="well sidebar-nav">
            <ul class="nav">
                <?php
                    // Check connection
                    if (mysqli_connect_errno())
                    {
                        echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."<p>";
                    }
                    else
                    {
                        $sql = "SELECT t.topic_id,
                                                t.topic_title,
                                                t.topic_last_post_id,
                                                t.forum_id,
                                                p.post_id,
                                                p.poster_id,
                                                p.post_time,
                                                u.user_id
                                FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
                                WHERE t.topic_id = p.topic_id AND
                                                t.topic_approved = 1 AND
                                                f.forum_id = t.forum_id AND
                                                t.forum_id = 6 AND
                                                t.topic_status <> 2 AND
                                                p.post_approved = 1 AND
                                                p.post_id = t.topic_last_post_id AND
                                                p.poster_id = u.user_id
                                ORDER BY t.topic_status DESC";
                        if ($result = mysqli_query($conn, $sql))
                        {
                            while ($row = mysqli_fetch_array($result))
                            {
                                $topic_title = $row["topic_title"];
                                $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$row["post_id"]."#p".$row["post_id"];
                                echo "<li><a href=\"{$post_link}\">{$topic_title}</a></li>";
                            }
                            mysqli_free_result($result);
                        }
                    }
                ?>
            </ul>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>
