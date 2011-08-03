<?php $pagetitle = "Overview"; include("header.php"); include("config.php"); ?>

<link rel="stylesheet" media="screen" href="/css/lytebox.css">
<script type="text/javascript" language="javascript" src="/scripts/lytebox.js"></script>
	
	<div style="max-width: 80em; margin: 0 auto;"> 
			<div class="yui-d0">
				<div id="watermark" class="watermark-apps-portlet">
					<div class="flowed-block">
						<img alt="" width="64" height="64" src="/images/logo.png" />
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
					<div id="side-portlets" class="yui-b side">
						<div class="portlet"> 
							<div id="downloads" class="top-portlet downloads">
								<h2 style="text-align: center;">
									Downloads
								</h2>
								
								<div class="version">
									Latest version is <?php echo LATEST_PH_VERSION ?>
								</div>

								<li>
									<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo LATEST_PH_VERSION ?>-setup.exe/download" title="Setup (recommended)">
										Installer
									</a>
								</li>
								<li>
									<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo LATEST_PH_VERSION ?>-bin.zip/download" title="Binaries (portable)">
										Binaries (portable)
									</a>
								</li>
								<li>
									<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-<?php echo LATEST_PH_VERSION ?>-src.zip/download" title="Source code">
										Source code
									</a>
								</li>
						
								<div class="released">
									Released <?php echo LATEST_PH_RELEASE_DATE ?>
								</div>
                                
                                <div style="text-align:center;">
                                    <a href="http://sourceforge.net/project/project_donations.php?group_id=242527">
                                        <img alt="Donate" src="/images/donate.gif" />
                                    </a>
                                </div>
							</div>
						</div>
						
						<div id="involvement" class="portlet">
						<ul class="involvement">
							<li>
								<a href="/downloads.php">All downloads</a>
							</li>				  
							<li>
								<a href="http://processhacker.sourceforge.net/forums/viewforum.php?f=24">Report a bug</a>
							</li>
							<li>
								<a href="http://processhacker.sourceforge.net/forums/viewforum.php?f=5">Ask a question</a>
							</li>
							<li style="margin-top: 1em;">
								<a href="http://processhacker.svn.sourceforge.net/viewvc/processhacker/2.x/trunk/">Browse source code</a>
							</li>
							<li>
								<a href="http://processhacker.sourceforge.net/doc">Source code documentation</a>
							</li>
							</ul>
						</div>
					</div>
					
					<div class="top-portlet" style="width: 70%;">	
						<div class="summary">
							<p>Process Hacker is a feature-packed tool for manipulating processes and services on your computer.</p>

							<p><strong>System Requirements:</strong></p>
							<li>&nbsp;•&nbsp;Microsoft Windows XP SP2 or above, 32-bit or 64-bit.</li>

							<li>&nbsp;Intel Itanium Platforms are not supported.</li>

							<p><strong>Key features of Process Hacker:</strong></p>
							<li>&nbsp;•&nbsp;A simple, customizable tree view with highlighting showing you the processes running on your computer.</li>
							<li>&nbsp;•&nbsp;Detailed system statistics with graphs.</li>
							<li>&nbsp;•&nbsp;Advanced features not found in other programs, such as detaching from debuggers, viewing GDI handles, viewing heaps, injecting and unloading DLLs, and more.</li>
							<li>&nbsp;•&nbsp;Powerful process termination that bypasses security software and rootkits.</li>
							<li>&nbsp;•&nbsp;View, edit and control services, including those not shown by the Services console.</li>
							<li>&nbsp;•&nbsp;View and close network connections.</li>
							<li>&nbsp;•&nbsp;Starts up almost instantly, unlike other programs.</li>
							<li>&nbsp;•&nbsp;<a href="./features.php">Many more features</a>...</li>
							
							<p><strong>Compared with Process Explorer, Process Hacker:</strong></p>
							
							<li>&nbsp;•&nbsp;Implements almost all of the functionality offered by Process Explorer, plus more advanced features.</li>
							<li>&nbsp;•&nbsp;Has advanced string scanning capabilities, as well as regular expression filtering.</li>
							<li>&nbsp;•&nbsp;Allows you to see what a thread is waiting on.</li>
							<li>&nbsp;•&nbsp;Highlights both relocated and .NET DLLs.</li>
							<li>&nbsp;•&nbsp;Allows you to connect to other sessions, just like Windows Task Manager can.</li>
							<li>&nbsp;•&nbsp;Shows symbolic access masks (e.g. <code>Read, Write</code>), rather than just numbers (e.g. <code>0x12019f</code>).</li>
							<li>&nbsp;•&nbsp;Shows names for transaction manager objects and ETW registration objects.</li>
							<li>&nbsp;•&nbsp;Shows detailed token information, as well as allowing privileges to be enabled and disabled.</li>
							<li>&nbsp;•&nbsp;Shows information for POSIX processes.</li>
						</div>
					</div>

					<div class="yui-g">
						<div class="yui-u first">
							<div class="portlet" id="portlet-latest-faqs">
								<p><strong>Latest News</strong></p>

								<?php
									// How Many Topics you want to display?
									$topicnumber = 5;
									// Change this to your phpBB path
									$urlPath = "./forums";
								 
									// Database Configuration (Where your phpBB config.php file is located)
									include './forums/config.php';
								 
									$table_topics = $table_prefix. "topics";
									$table_forums = $table_prefix. "forums";
									$table_posts = $table_prefix. "posts";
									$table_users = $table_prefix. "users";

									if ($link = mysql_connect("$dbhost", "$dbuser", "$dbpasswd"))
									{
										if (mysql_select_db("$dbname"))
										{
											$query = "SELECT t.topic_id, t.topic_title, t.topic_last_post_id, t.forum_id, p.post_id, p.poster_id, p.post_time, u.user_id, u.username, u.user_colour
											FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
											WHERE t.topic_id = p.topic_id AND
											f.forum_id = t.forum_id AND
											t.forum_id = 1 AND
											t.topic_status <> 2 AND
											p.post_id = t.topic_last_post_id AND
											p.poster_id = u.user_id
											ORDER BY p.post_id DESC LIMIT $topicnumber";
											
											if ($result = mysql_query($query))
											{			
												print "<table width=\"100%\" border=\"1\">";
											
												while ($row = mysql_fetch_array($result, MYSQL_ASSOC)) 
												{
													echo  
													"<tr>
														<font face=\"Verdana, Arial, Helvetica, sans-serif\" size=\"1\">
															<b>
																<a href=\"$urlPath/viewtopic.php?f=$row[forum_id]&t=$row[topic_id]&p=$row[post_id]#p$row[post_id]\" TARGET=\"_blank\">".$row["topic_title"]."</a>
															</b>
															
															<font color=\"#C0C0C0\">
																by <a href=\"$urlPath/memberlist.php?mode=viewprofile&u=$row[user_id]\" TARGET=\"_blank\">
																	<strong><font color=\"$row[user_colour]\">".$row["username"]."</font></strong>
																</a>
															
																<li>"
																	.date('F j, Y, g:i a', $row["post_time"])."
																</li>
															</font>
														</font>
													</tr>";
												}
												
												print "</table>";
											}
											else
											{
												print "<h2>Query failed.</h2>";
											}
										}
										else
										{
											print "<h2>Could not select database.</h2>";
										}
									}
									else
									{
										print "<h2>Could not connect.</h2>";
									}
									
									mysql_free_result($result);
									mysql_close($link);
									?>
							</div>
						</div>

						<div class="yui-g">
							<div class="portlet" id="portlet-latest-faqs">
								<p><strong>Forum Activity</strong></p>

								<?php
									// How Many Topics you want to display?
									$topicnumber = 5;
									// Change this to your phpBB path
									$urlPath = "./forums";
								 
									// Database Configuration (Where your phpBB config.php file is located)
									include './forums/config.php';
								 
									$table_topics = $table_prefix. "topics";
									$table_forums = $table_prefix. "forums";
									$table_posts = $table_prefix. "posts";
									$table_users = $table_prefix. "users";
									
									if ($link = mysql_connect("$dbhost", "$dbuser", "$dbpasswd"))
									{
										if (mysql_select_db("$dbname"))
										{
											$query = "SELECT t.topic_id, t.topic_title, t.topic_last_post_id, t.forum_id, p.post_id, p.poster_id, p.post_time, u.user_id, u.username, u.user_colour
											FROM $table_topics t, $table_forums f, $table_posts p, $table_users u
											WHERE t.topic_id = p.topic_id AND
											f.forum_id = t.forum_id AND
											t.forum_id != 1 AND 
											t.forum_id != 7 AND 
											t.topic_status <> 2 AND
											p.post_id = t.topic_last_post_id AND
											p.poster_id = u.user_id
											ORDER BY p.post_id DESC LIMIT $topicnumber";
											
											if ($result = mysql_query($query))
											{							
												print "<table width=\"100%\" border=\"1\">";
												
												while ($row = mysql_fetch_array($result, MYSQL_ASSOC)) 
												{
													echo  
													"<tr>
														<font face=\"Verdana, Arial, Helvetica, sans-serif\" size=\"1\">
															<b>	
																<a href=\"$urlPath/viewtopic.php?f=$row[forum_id]&t=$row[topic_id]&p=$row[post_id]#p$row[post_id]\" TARGET=\"_blank\">".$row["topic_title"]."</a>
															</b>
															<font color=\"#C0C0C0\">
																by <a href=\"$urlPath/memberlist.php?mode=viewprofile&u=$row[user_id]\" TARGET=\"_blank\">
																	<strong><font color=\"$row[user_colour]\">".$row["username"]."</font></strong>
																</a>
																<li>"
																	.date('F j, Y, g:i a', $row["post_time"])."
																</li>
															</font>
														</font>
													</tr>";
												}
												
												print "</table>";
											}
											else
											{
												print("<h2>Query failed.</h2>");
											}
										}
										else
										{
											print("<h2>Could not select database.</h2>");
										}
									}
									else
									{
										print("<h2>Could not connect.</h2>");
									}
									
									mysql_free_result($result);
									mysql_close($link);
									?>
							</div>
						</div>
						
						<div class="yui-u">
							<div id="structural-subscription-content-box"></div>
						</div>
					</div>

					<div class="yui-g">
						<div class="yui-u first">
							<div class="portlet" id="portlet-latest-faqs">
								<p><strong>Screenshots</strong></p>
								<a href="/images/screenshots/processes_tab_large.png" rel="lytebox[group1]" style="margin-left: 1em; margin-right: 1em;">
									<img border="0" width="200" height="107" src="/images/screenshots/processhacker_small.png" />
								</a>
								
								<a href="/images/screenshots/sysinfo_large.png" rel="lytebox[group1]" style="margin-left: 1em; margin-right: 1em;">
									<img border="0" width="200" height="107" src="/images/screenshots/sysinfo_small.png" />
								</a>
								
								<h2>
									<span class="see-all">
										<a href="./screenshots.php" class="menu-link-list_all sprite info">All Screenshots</a>
									</span>
								</h2>
							</div>	
						</div>

						<div class="yui-u">
							<div class="portlet" id="portlet-latest-questions">
								<p><strong>Statistics</strong></p>
								<script type="text/javascript" src="http://www.ohloh.net/p/329666/widgets/project_basic_stats.js"></script>
							</div>      
						</div>
			
						<div class="yui-u">
						  <div id="structural-subscription-content-box"></div>
						</div>
					</div>	
				</div>
			</div>
		</div>

<?php include("footer.php"); ?>