<?php 
$pagetitle = "Features"; 

include("header.php"); 
?>

<div class="page">
    <div class="yui-d0">
        <div id="watermark" class="watermark-apps-portlet">
            <div class="flowed-block">
                <img alt="" width="64" height="64" src="./images/logo_64x64.png" />
            </div>
            
            <div class="flowed-block wide">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">					
                    <li><a href="/">Overview</a></li>							
                    <li class="overview active"><a href="./features.php">Features</a></li>
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
                <div class="portlet" >
                    <h2 class="center">Quick Links</h2>
                    <ul class="involvement">



                    </ul>
                </div>
                
                <div class="portlet" >
                    <h2 class="center">Get Involved</h2>
                    <ul class="involvement">
                        <li><a href="/forums/viewforum.php?f=24">Report a bug</a></li>
                        <li><a href="/forums/viewforum.php?f=5">Ask a question</a></li>
                    </ul>
                </div>
            </div>
        
            <div class="top-portlet">
                <div class="summary">
                    <p>A very incomplete feature list for Process Hacker 2:</p>		
                    <div class="description">
                        <h3>Processes</h3>
                        <li>&#160;•&#160;View processes in a tree view with highlighting</li>
                        <li>&#160;•&#160;View detailed process statistics and performance graphs</li>
                        <li>&#160;•&#160;Process tooltips are detailed and show context-specific information</li>
                        <li>&#160;•&#160;Select multiple processes and terminate, suspend or resume them</li>
                        <li>&#160;•&#160;(32-bit only) Bypass almost all forms of process protection</li>
                        <li>&#160;•&#160;Restart processes</li>
                        <li>&#160;•&#160;Empty the working set of processes</li>
                        <li>&#160;•&#160;Set affinity, priority and virtualization</li>
                        <li>&#160;•&#160;Create process dumps</li>
                        <li>&#160;•&#160;Use over a dozen methods to terminate processes</li>
                        <li>&#160;•&#160;Detach processes from debuggers</li>
                        <li>&#160;•&#160;View process heaps</li>
                        <li>&#160;•&#160;View GDI handles</li>
                        <li>&#160;•&#160;Inject DLLs</li>
                        <li>&#160;•&#160;View DEP status, and even enable/disable DEP</li>
                        <li>&#160;•&#160;View environment variables</li>
                        <li>&#160;•&#160;View and edit process security descriptors</li>
                        <li>&#160;•&#160;View image properties such as imports and exports</li>

                        <h3>Threads</h3>
                        <li>&#160;•&#160;View thread start addresses and stacks with symbols</li>
                        <li>&#160;•&#160;Threads are highlighted if suspended, or are GUI threads</li>
                        <li>&#160;•&#160;Select multiple threads and terminate, suspend or resume them</li>
                        <li>&#160;•&#160;Force terminate threads</li>
                        <li>&#160;•&#160;View TEB addresses and view TEB contents</li>
                        <li>&#160;•&#160;(32-bit only) Find out what a thread is doing, and what objects it is waiting on</li>
                        <li>&#160;•&#160;View and edit thread security descriptors</li>

                        <h3>Tokens</h3>
                        <li>&#160;•&#160;View full token details, including user, owner, primary group, session ID, elevation status, and more</li>
                        <li>&#160;•&#160;View token groups</li>
                        <li>&#160;•&#160;View privileges and even enable, disable or remove them</li>
                        <li>&#160;•&#160;View and edit token security descriptors</li>
                        
                        <h3>Modules</h3>
                        <li>&#160;•&#160;View modules and mapped files in one list</li>
                        <li>&#160;•&#160;Unload DLLs</li>
                        <li>&#160;•&#160;View file properties and open them in Windows Explorer</li>
                        
                        <h3>Memory</h3>
                        <li>&#160;•&#160;View a virtual memory list</li>
                        <li>&#160;•&#160;Read and modify memory using a hex editor</li>
                        <li>&#160;•&#160;Dump memory to a file</li>
                        <li>&#160;•&#160;Free or decommit memory</li>
                        <li>&#160;•&#160;Scan for strings</li>

                        <h3>Handles</h3>
                        <li>&#160;•&#160;View process handles, complete with highlighting for attributes</li>
                        <li>&#160;•&#160;Search for handles (and DLLs and mapped files)</li>
                        <li>&#160;•&#160;Close handles</li>
                        <li>&#160;•&#160;(32-bit only) Set handle attributes - Protected and Inherit</li>
                        <li>&#160;•&#160;Granted access of handles can be viewed symbolically instead of plain hex numbers</li>
                        <li>&#160;•&#160;View detailed object properties when supported</li>
                        <li>&#160;•&#160;View and edit object security descriptors</li>

                        <h3>Services</h3>
                        <li>&#160;•&#160;View a list of all services</li>
                        <li>&#160;•&#160;Create services</li>
                        <li>&#160;•&#160;Start, stop, pause, continue or delete services</li>
                        <li>&#160;•&#160;Edit service properties</li>
                        <li>&#160;•&#160;View service dependencies and dependents</li>
                        <li>&#160;•&#160;View and edit service security descriptors</li>

                        <h3>Network</h3>
                        <li>&#160;•&#160;View a list of network connections</li>
                        <li>&#160;•&#160;Close network connections</li>
                        <li>&#160;•&#160;Use tools such as whois, traceroute and ping</li>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>