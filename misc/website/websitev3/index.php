<?php $pagetitle = "Overview"; include "include/header.php"; ?>

 <div class="jumbotron">
    <div class="container">
      	<h1>Process Hacker</h1>
		<p class="headline main-headline">
			A <strong>free</strong>, powerful, multi-purpose tool that helps you <strong>monitor system resources</strong>, <strong>debug software</strong> and <strong>detect malware</strong>.
		</p>
		<p><a href="downloads.php" class="btn btn-primary btn-lg">Download Process Hacker &raquo;</a></p>
    </div>
</div>

<div class="container">
    <div class="row">
        <div class="col-sm-4">
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/main_window.png" data-lightbox="image-screenshot" data-title="Main Window">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/main_window.png">
            </a>
        </div>
        <div class="col-sm-8">
            <h2>A detailed overview of system activity with highlighting.</h2>
            <p>Tip: Add extra columns to show detailed system activity.</p>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-8">
            <h2>Graphs and statistics allow you quickly to track down resource hogs and runaway processes.</h2>
            <p>Tip: Use Ctrl+I to view system performance information.
            Move your cursor over a graph to get a tooltip with information about the data point under your cursor.
            You can double-click the graph to see information about the process at that data point, even if the process is no longer running.</p>
        </div>
        <div class="col-sm-4">
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/sysinfo_trimmed_1.png" data-lightbox="image-screenshot" data-title="System Information Window">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/sysinfo_trimmed_1.png">
            </a>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/find_handles.png" data-lightbox="image-screenshot" data-title="Find Handles Window">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/find_handles.png">
            </a>
        </div>
        <div class="col-sm-8">
            <h2>Can't edit or delete a file? Discover which processes are using that file.</h2>
            <p>Tip: Use Ctrl+F to search for a handle or DLL.
            If all else fails, you can right-click an entry and close the handle associated with the file. However, this
            should only be used as a last resort and can lead to data loss and corruption.</p>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-8">
            <h2>See what programs have active network connections, and close them if necessary.</h2>
        </div>
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/network.png" data-lightbox="image-screenshot" data-title="Network Tab">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/network.png">
            </a>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">    
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/disk_tab.png" data-lightbox="image-screenshot" data-title="Disk Tab">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/disk_tab.png">
            </a>
        </div>
        <div class="col-sm-8"><h2>Get real-time information on disk access.</h2>
            <p>Tip: This may look very similar to the Disk Activity feature in Resource Monitor, but Process Hacker has a few more features!</p>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-8">
            <h2>View detailed stack traces with kernel-mode, WOW64 and .NET support.</h2>
            <p>Tip: Hover your cursor over the first column (with the numbers) to view parameter and line number information when available.</p>
        </div>     
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/thread_stack.png" data-lightbox="image-screenshot" data-title="Thread Stack">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/thread_stack.png">
            </a>
        </div>
    </div>

    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/services.png" data-lightbox="image-screenshot" data-title="services Tab">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/services.png">
            </a>
        </div>
        <div class="col-sm-8">
            <h2>Go beyond services.msc: create, edit and control services.</h2>
            <p>Tip: By default, Process Hacker shows entries for drivers in addition to normal user-mode services. You can turn this off by checking <strong>View &gt; Hide Driver Services</strong>.</p>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-sm-8">
            <h2>And much more...</h2>
            <p>Capabilities, Claims, Permissions, Integrity Control, Symbols, Statistics, Peformance, Threads, Tokens, Modules, Memory, Environement Variables, Handles, Services, GPU, Disk, Network, Terminator, DLL Injector, VirusTotal Uploader, Jotti Uploader, Comodo Uploader, Priority, Protection, Ping, Tracert, Whois... And much more!</p>
        </div>     
        <div class="col-sm-4">          
            <a class="thumbnail" href="http://processhacker.sourceforge.net/img/screenshots/menu.png" data-lightbox="image-screenshot" data-title="Process Menu">
                <img class="img-responsive" src="http://processhacker.sourceforge.net/img/screenshots/menu.png">
            </a>
        </div>
    </div>
    
    <hr class="col-sm-12">

    <div class="row">
        <div class="col-md-4">
            <h2>Latest News</h2>
                <?php
						// Connect to DB
						$conn = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbNameRo);
						if (mysqli_connect_errno())
						{
							echo "<p>Failed to connect to MySQL: " . mysqli_connect_error()."</p>";
						}
						else
						{
							$sql = "SELECT 
										t.topic_id,
										t.topic_title,
										t.topic_views,
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
									$topic_views = $row["topic_views"];
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
										"<a href=\"".htmlspecialchars($post_link)."\" class=\"list-group-item\"><span class=\"badge\">{$topic_views}</span>
											<h4 class=\"list-group-item-heading\">".$topic_title."</h4>
											<p class=\"list-group-item-text\">
												<span class=\"text-muted\">".$post_local_time." by <span style=\"color:#".$author_colour."\">".$author_name."</span></span>
											</p>
										</a>";
								}
								
								mysqli_free_result($result);
							}
							mysqli_close($conn);
						}
				s?>
        </div>
        <div class="col-md-4">
            <h2>Latest Posts</h2>
    		<?php
										// Connect to DB
						$conn = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbNameRo);
						if (mysqli_connect_errno())
						{
							echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."</p>";
						}
						else
						{
							$sql = "SELECT 
										t.topic_id,
										t.topic_title,
										t.topic_views,
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
										t.topic_visibility = 1 AND
										f.forum_id = t.forum_id AND
										t.forum_id != 1 AND
										t.forum_id != 7 AND
										t.topic_status <> 2 AND
										p.post_visibility = 1 AND
										p.post_id = t.topic_last_post_id AND
										p.poster_id = u.user_id
									ORDER BY p.post_id DESC LIMIT $topicnumber";
							
							if ($result = mysqli_query($conn, $sql))
							{
								while ($row = mysqli_fetch_array($result))
								{
									// Query fields
									$topic_title = $row["topic_title"];
									$topic_views = $row["topic_views"];
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
										"<a href=\"".htmlspecialchars($post_link)."\" class=\"list-group-item\"><span class=\"badge\">{$topic_views}</span>
											<h4 class=\"list-group-item-heading\">{$topic_title}</h4>
											<p class=\"list-group-item-text\">
												<span class=\"text-muted\">".$post_date.", ".$post_local_time." by <span style=\"color:#".$author_colour."\">".$author_name."</span></span>
											</p>
										</a>";
								}
								
								mysqli_free_result($result);
							}
							mysqli_close($conn);
						}
						?>
       </div>
        <div class="col-md-4">
            <h2>Latest Source</h2>
            <div id="feeddiv"></div>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>