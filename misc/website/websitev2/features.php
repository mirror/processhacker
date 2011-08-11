<?php $pagetitle = "Features"; include("header.php"); ?>

<div class="page">
	<div class="yui-d0">
		<div id="watermark" class="watermark-apps-portlet">
			<div class="flowed-block">
				<img alt="" width="64" height="64" src="./images/logo.png" />
			</div>
			
			<div class="flowed-block wide">
				<h2>Process Hacker</h2>
				<ul class="facetmenu">					
					<li><a href="./">Overview</a></li>							
					<li class="overview active"><a href="./features.php">Features</a></li>
					<li><a href="./screenshots.php">Screenshots</a></li>
					<li><a href="./downloads.php">Downloads</a></li>
					<li><a href="./faq.php">FAQ</a></li>
					<li><a href="./about.php">About</a></li>
					<li><a href="./forums/" target="_blank">Forum</a></li>
				</ul>
			</div>
		</div>	

		<div class="yui-t4">
			<div class="top-portlet">
				<div class="summary">
					<p>A very incomplete feature list for Process Hacker 2:</p>		
					<div class="description">
						<h3>Processes</h3>
						<li>&nbsp;•&nbsp;View processes in a tree view with highlighting</li>
						<li>&nbsp;•&nbsp;View detailed process statistics and performance graphs</li>
						<li>&nbsp;•&nbsp;Process tooltips are detailed and show context-specific information</li>
						<li>&nbsp;•&nbsp;Select multiple processes and terminate, suspend or resume them</li>
						<li>&nbsp;•&nbsp;(32-bit only) Bypass almost all forms of process protection</li>
						<li>&nbsp;•&nbsp;Restart processes</li>
						<li>&nbsp;•&nbsp;Empty the working set of processes</li>
						<li>&nbsp;•&nbsp;Set affinity, priority and virtualization</li>
						<li>&nbsp;•&nbsp;Create process dumps</li>
						<li>&nbsp;•&nbsp;Use over a dozen methods to terminate processes</li>
						<li>&nbsp;•&nbsp;Detach processes from debuggers</li>
						<li>&nbsp;•&nbsp;View process heaps</li>
						<li>&nbsp;•&nbsp;View GDI handles</li>
						<li>&nbsp;•&nbsp;Inject DLLs</li>
						<li>&nbsp;•&nbsp;View DEP status, and even enable/disable DEP</li>
						<li>&nbsp;•&nbsp;View environment variables</li>
						<li>&nbsp;•&nbsp;View and edit process security descriptors</li>
						<li>&nbsp;•&nbsp;View image properties such as imports and exports</li>

						<h3>Threads</h3>
						<li>&nbsp;•&nbsp;View thread start addresses and stacks with symbols</li>
						<li>&nbsp;•&nbsp;Threads are highlighted if suspended, or are GUI threads</li>
						<li>&nbsp;•&nbsp;Select multiple threads and terminate, suspend or resume them</li>
						<li>&nbsp;•&nbsp;Force terminate threads</li>
						<li>&nbsp;•&nbsp;View TEB addresses and view TEB contents</li>
						<li>&nbsp;•&nbsp;(32-bit only) Find out what a thread is doing, and what objects it is waiting on</li>
						<li>&nbsp;•&nbsp;View and edit thread security descriptors</li>

						<h3>Tokens</h3>
						<li>&nbsp;•&nbsp;View full token details, including user, owner, primary group, session ID, elevation status, and more</li>
						<li>&nbsp;•&nbsp;View token groups</li>
						<li>&nbsp;•&nbsp;View privileges and even enable, disable or remove them</li>
						<li>&nbsp;•&nbsp;View and edit token security descriptors</li>
						
						<h3>Modules</h3>
						<li>&nbsp;•&nbsp;View modules and mapped files in one list</li>
						<li>&nbsp;•&nbsp;Unload DLLs</li>
						<li>&nbsp;•&nbsp;View file properties and open them in Windows Explorer</li>
						
						<h3>Memory</h3>
						<li>&nbsp;•&nbsp;View a virtual memory list</li>
						<li>&nbsp;•&nbsp;Read and modify memory using a hex editor</li>
						<li>&nbsp;•&nbsp;Dump memory to a file</li>
						<li>&nbsp;•&nbsp;Free or decommit memory</li>
						<li>&nbsp;•&nbsp;Scan for strings</li>

						<h3>Handles</h3>
						<li>&nbsp;•&nbsp;View process handles, complete with highlighting for attributes</li>
						<li>&nbsp;•&nbsp;Search for handles (and DLLs and mapped files)</li>
						<li>&nbsp;•&nbsp;Close handles</li>
						<li>&nbsp;•&nbsp;(32-bit only) Set handle attributes - Protected and Inherit</li>
						<li>&nbsp;•&nbsp;Granted access of handles can be viewed symbolically instead of plain hex numbers</li>
						<li>&nbsp;•&nbsp;View detailed object properties when supported</li>
						<li>&nbsp;•&nbsp;View and edit object security descriptors</li>

						<h3>Services</h3>
						<li>&nbsp;•&nbsp;View a list of all services</li>
						<li>&nbsp;•&nbsp;Create services</li>
						<li>&nbsp;•&nbsp;Start, stop, pause, continue or delete services</li>
						<li>&nbsp;•&nbsp;Edit service properties</li>
						<li>&nbsp;•&nbsp;View service dependencies and dependents</li>
						<li>&nbsp;•&nbsp;View and edit service security descriptors</li>

						<h3>Network</h3>
						<li>&nbsp;•&nbsp;View a list of network connections</li>
						<li>&nbsp;•&nbsp;Close network connections</li>
						<li>&nbsp;•&nbsp;Use tools such as whois, traceroute and ping</li>
					</div>
				</div>
			</div>
		</div>
	</div>
</center>

<?php include("footer.php"); ?>