<?php $pagetitle = "News"; include "include/header.php"; include "include/phpbb.php"; ?>

<div class="row">
    <h1 class="page-header"><small>News</small></h1>
</div>

<div class="row">
    <div class="col-lg-8">
        <?php
            // Check connection
            if (mysqli_connect_errno())
            {
                echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."</p>";
            }
                
            $sql = 
                "SELECT t.topic_id, 
                        t.topic_title, 
                        t.topic_last_post_id,
                        t.forum_id, 
                        t.topic_views,
                        t.topic_first_poster_name,
                        p.post_id, 
                        p.poster_id, 
                        p.post_time, 
                        p.post_text,
                        p.bbcode_bitfield,
                        p.bbcode_uid, 
                        u.user_id, 
                        u.user_colour, 
                        u.user_avatar,
                        u.user_avatar_type, 
                        u.user_avatar_width, 
                        u.user_avatar_height
                FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
                WHERE 	t.topic_id = p.topic_id AND
                        t.topic_approved = 1 AND
                        t.forum_id = 1 AND
                        t.topic_status <> 2 AND
                        f.forum_id = t.forum_id AND
                        p.post_approved = 1 AND
                        p.post_id = t.topic_last_post_id AND
                        p.poster_id = u.user_id
                ORDER BY p.post_id DESC LIMIT $topicnumber";

                if ($result = mysqli_query($conn, $sql))
                {
                    // Fetch one and one row
                    while ($row = mysqli_fetch_array($result))
                    {
                        $topic_title = $row["topic_title"];
                        $post_text = $row['post_text'];
                        $post_time = $row["post_time"];
                        $author_avatar = $row["user_avatar"];
                        $author_name = $row['topic_first_poster_name'];
                        $author_colour = $row['user_colour'];
                        $topic_views = $row['topic_views'];
                        
                        $post_local_time = date('F jS, Y, g:i a', $post_time);
                        $author_link = "http://processhacker.sourceforge.net/forums/memberlist.php?mode=viewprofile&u=".$row['user_id'];
                        $post_link = "http://processhacker.sourceforge.net/forums/viewtopic.php?p=".$row['post_id']."#p".$row['post_id'];
                        
                        // Second parse bbcode here
                        if ($row['bbcode_bitfield'])
                        {
                            $bbcode = new bbcode(base64_encode($row['bbcode_bitfield']));
                            $bbcode->bbcode_second_pass($post_text, $row['bbcode_uid'], $row['bbcode_bitfield']);
                        }
                        
                        // censor_text();
                        $post_text = bbcode_nl2br($post_text);
                        $post_text = smiley_text($post_text);
                        
                        echo 
                          "<h1><a href=\"{$post_link}\">{$topic_title}</a></h1>
                          <p class=\"lead\">by <a href=\"{$author_link}\">{$author_name}</a></p>
                          <hr>
                          <p><i class=\"icon-time\"></i>{$post_local_time}</p>
                          <hr>
                          <p>{$post_text}</p>
                          <a class=\"btn btn-primary\" href=\"{$post_link}\">Read More <i class=\"icon-angle-right\"></i></a>
                          <hr>";		
                    }
                    
                    mysqli_free_result($result);
                }	
                else
                {
                    echo "<p>Failed to query database: ".mysqli_error($con)."</p>";
                }
            ?>
            
            <hr>
          
            <ul class="pager">
                <li class="previous"><a href="#">&larr; Older</a></li>
                <li class="next"><a href="#">Newer &rarr;</a></li>
            </ul>
        </div>

        <div class="col-lg-4">
          <div class="well">
            <h4>News Search</h4>
            <div class="input-group">
              <input type="text" class="form-control">
              <span class="input-group-btn">
                <button class="btn btn-default" type="button"><i class="icon-search"></i></button>
              </span>
            </div><!-- /input-group -->
          </div><!-- /well -->
          <div class="well">
            <div id="content_right">
                <a class="twitter-timeline" width="336" height="500" data-dnt="true" href="https://twitter.com/search?q=%23processhacker&f=realtime" data-widget-id="364356491609776128">Tweets about "#processhacker"</a>
                <script type="text/javascript">
                    (function() {
                    var s = document.createElement('SCRIPT');
                    var c = document.getElementsByTagName('script')[0];
                    s.type = 'text/javascript';
                    s.async = true;
                    s.src = 'http://platform.twitter.com/widgets.js';
                    c.parentNode.insertBefore(s, c);
                    })();
                </script>
            </div>
        </div>
    </div>
</div>

<?php include "include/footer.php" ?>