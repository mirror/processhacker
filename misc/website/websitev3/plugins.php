<?php $pagetitle = "Plugins"; include "include/header.php"; ?>

<div class="row">
    <h1 class="page-header"><small>Plugins</small></h1>
</div>

<div class="row">
    <div class="col-xs-12 col-sm-9">
        <div class="row">
            <div class="col-6 col-sm-6 col-lg-4">
                <?php
                    // Check connection
                    if (mysqli_connect_errno())
                    {
                        echo "Failed to connect to MySQL: " . mysqli_connect_error();
                    }
                    
                    $sql = "SELECT t.topic_id, 
                                t.topic_title, 
                                t.topic_last_post_id,
                                    t.forum_id, 
                                    p.post_id, 
                                    p.poster_id, 
                                    p.post_time, 
                                    p.bbcode_bitfield,
                                    p.bbcode_uid, 
                                    u.user_id, 
                                    u.username,
                                    u.user_colour, 
                                    u.user_avatar,
                                    u.user_avatar_type, 
                                    u.user_avatar_width, 
                                    u.user_avatar_height
                                FROM phpbb_topics t, phpbb_forums f, phpbb_posts p, phpbb_users u
                                WHERE t.topic_id = p.topic_id AND
                                    t.topic_approved = 1 AND
                                    f.forum_id = t.forum_id AND
                                    t.forum_id = 18 AND
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
                            $author_avatar = $row["user_avatar"];
                            $author_name = $row['username'];
                            $author_colour = $row['user_colour'];
                            $post_time = $row["post_time"];
                            
                            $post_local_time = date('F jS, Y, g:i a', $post_time);
                            $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$row['user_id'];
                            $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$row['post_id']."#p".$row['post_id'];
                            
                            echo 
                            "<div class=\"row\"><div id=\"forumitem\">
                                <a href=\"{$post_link}\">{$topic_title}</a>
                                <span id=\"forumdate\" > by 
                                    <a href=\"{$author_link}\" style=\"color:#{$author_colour}\">{$author_name}</a>
                                    <a class=\"thumbnail pull-right\" href=\"#\">
                                        <img class=\"media-object\" src=\"http://processhacker.sourceforge.net/forums/download/file.php?avatar={$author_avatar}\">
                                    </a>
                                </span>
                            </div></div><hr>";
                        }
                        
                        mysqli_free_result($result);
                    }
                ?>
            </div>
        </div>
    </div>
</div>
<?php include "include/footer.php"; ?>