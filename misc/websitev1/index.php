<?php
    $title = "Home";
    require("header.php");
?>

    <div id="content">
		
		<div class="downloadbox1">		
			<!-- AddThis Button BEGIN -->
			<div class="addthis_toolbox addthis_default_style ">
				<a class="addthis_counter addthis_pill_style"></a>
			</div>
	
			<script type="text/javascript">var addthis_config = {"data_track_clickback":true};</script>
			<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=dmex"></script>
			<!-- AddThis Button END -->
		</div>
		
		<div class="downloadbox">
			<p class="downloadversion">
				<img src="images/go-down.png" alt="Download" /> Process Hacker 2.18
			</p>
			
			<p class="downloadmain">
				<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-2.18-bin.zip/download">Download binaries (.zip)</a>
			</p>
			
			<p class="downloadalt">
				<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-2.18-setup.exe/download">Download installer (.exe)</a>
			</p>
			
			<p class="downloadalt">
				<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/processhacker-2.18-src.zip/download">Download source code (.zip)</a>
			</p>
			
			<p class="downloadalt">
				<a href="http://sourceforge.net/projects/processhacker/files/processhacker2/">All files</a>
			</p>
        <p style="margin-top: 10px; font-size: 8pt;"><em>.NET Framework is NOT required.</em></p>
      </div>
      
		<div class="additionalbox">
			<p><a href="http://sourceforge.net/project/project_donations.php?group_id=242527">Donate</a> to support Process Hacker!</p>
		</div>
		
		<p>Process Hacker is a feature-packed tool for manipulating processes and services on your computer.</p>
      	  
		<p>Key features of Process Hacker:</p>
		<ul>
        <li><strong>A simple, customizable tree view with highlighting</strong> showing you the 
        processes running on your computer.</li>
        <li><strong>Detailed system statistics</strong> with graphs.</li>
        <li><strong>Advanced features not found in other programs</strong>, such as detaching from debuggers, viewing 
        GDI handles, viewing heaps, injecting and unloading DLLs, and more.</li>
        <li>Powerful process termination that bypasses security software and rootkits.</li>
        <li>View, edit and control services, including those not shown by the Services console.</li>
        <li>View and close network connections.</li>
        <li>Starts up almost instantly, unlike other programs.</li>
        <li><a href="/features.php">Many more features</a>...</li>
      </ul>
      
		<p>Compared with Process Explorer, Process Hacker:</p>
		<ul style="margin-bottom: 20px;">
        <li>Implements almost all of the functionality offered by Process Explorer, plus more 
        advanced features.</li>
        <li>Has advanced string scanning capabilities, as well as regular expression filtering.</li>
        <li>Allows you to see what a thread is waiting on.</li>
        <li>Highlights both relocated and .NET DLLs.</li>
        <li>Allows you to connect to other sessions, just like Windows Task Manager can.</li>
        <li>Shows symbolic access masks (e.g. <code>Read, Write</code>), rather than just numbers (e.g. <code>0x12019f</code>).</li>
        <li>Shows names for transaction manager objects and ETW registration objects.</li>
        <li>Shows detailed token information, as well as allowing privileges to be enabled and disabled.</li>
        <li>Shows information for POSIX processes.</li>
      </ul>
	  
	  	<p>The <a href="http://www.reactos.org">ReactOS Foundation</a> has very 
			kindly signed the driver, so it works on 64-bit systems.</p>
			
		<h2 style="margin-top: 10px;">System Requirements</h2>
		<ul>
			<li>Microsoft Windows XP SP2 or above, 32-bit or 64-bit.</li> 
		</ul>
	  
		<h2 style="margin-top: 20px;">Screenshots</h2>
	
		<a href="./images/processhacker_small.png" rel="lytebox" style="margin-left: 1em; margin-right: 1em;">
			<img border="0" height="107" src="./images/processhacker_small.png" width="200" />
		</a>
		
		<a href="./images/sysinfo_small.png" rel="lytebox" style="margin-left: 1em; margin-right: 1em;">
			<img border="0" height="107" src="./images/sysinfo_small.png" width="200" />
		</a>
	  
		<h2 style="margin-top: 20px;">Statistics</h2>
        <script type="text/javascript" src="http://www.ohloh.net/p/329666/widgets/project_basic_stats.js"></script>
	
	</div>

<?php include("footer.php"); ?>
