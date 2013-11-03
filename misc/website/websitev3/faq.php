<?php $pagetitle = "FAQ"; include "include/header.php"; ?>

<div class="col-sm-9">
        <h1 class="page-header"><small>Frequently Asked Questions</small></h1>
        
            <h3>Is "Process Hacker" a dangerous "hacking" tool?</h3>
                No. Please read about the <a href="http://catb.org/~esr/faqs/hacker-howto.html#what_is">correct definition of "hacker"</a>.
              
            <hr class="col-sm-12">
            <h3>Is Process Hacker a portable application?</h3>
                Yes. In the same directory as <code>ProcessHacker.exe</code>, create a file named <code>ProcessHacker.exe.settings.xml</code>, settings will then be automatically saved here.
              
            <hr class="col-sm-12">
            <h3>Process Hacker can kill my anti-virus software! Is this a bug in the anti-virus software?</h3>
      			No. Please do not report these incidents as bugs because you will be wasting their time.
              
            <hr class="col-sm-12">
            <h3>Why is Process Hacker able to kill processes that no other tools can kill?</h3>
      			Process Hacker loads a driver that searches for an internal Microsoft kernel function and uses it for process termination. 
                This function is not known to be hooked by malware or security software.
              
            <hr class="col-sm-12">
            <h3>Why is there annoying bug X in Process Hacker? Why is Process Hacker missing feature Y?</h3>
      			Please report any bugs or feature requests in the <a href="/forums">forums</a>.
              
            <hr class="col-sm-12">
            <h3>Why can't I build Process Hacker?</h3>
                The most likely problem is that you do not have the latest Windows SDK installed.<br>
                Windows XP, Vista and Windows 7 SDK: <a href="http://msdn.microsoft.com/en-us/windows/bb980924.aspx">Windows SDK</a><br>
                Windows 7, Windows 8 SDK: <a href="http://msdn.microsoft.com/en-US/windows/hardware/hh852363">Windows 8 SDK</a><br>
                Windows 7, Windows 8 and 8.1 SDK: <a href="http://msdn.microsoft.com/en-US/windows/desktop/bg162891">Windows 8.1 SDK</a>
              	
            <hr class="col-sm-12">
            <h3>Symbols don't work properly!</h3>
      			Firstly, you need the latest <code>dbghelp.dll</code> version:<br>
                <br>
                1) Install the latest Windows SDK. (links are below)<br>
                2) Open Process Hacker options via the main menu: Hacker &gt; Options<br>
                3) Click Symbols, and locate <code>dbghelp.dll</code><br>
                <dl>
                    <dd>
                                            Windows XP, Vista and Windows 7 SDK:<br>
                                            <code>C:\Program Files\Debugging Tools for Windows (x86)\</code><br><br>
                                            Windows 8 or above SDK:<br>
                                            32bit: <code>\Program Files (x86)\Windows Kits\8.x\Debuggers\x86\</code><br>
                                            64bit: <code>\Program Files (x86)\Windows Kits\8.x\Debuggers\x64\</code><br>
                    </dd>
                </dl>
                Secondly, you need to configure the search path. If you don't know what to do, enter:<br>
                <code>SRV*SOME_FOLDER*http://msdl.microsoft.com/download/symbols</code><br><br>
                Replace <code>SOME_FOLDER</code> with any folder you can write to, like <code>D:\Symbols</code>.
                Now you can restart Process Hacker and view full symbols.
                
            <hr class="col-sm-12">
            <h3>Why can't I debug my plugin?</h3>
      			The most likely problem is that you have not configured the plugin Solution properties > Debugger options.<br>
                <br>
                For example; Debugging current plugins can be configured using the following settings:<br>
                Debugger Command: <code>$(SolutionDir)..\bin\$(Configuration)$(PlatformArchitecture)\ProcessHacker.exe</code><br>
                Working Directory: <code>$(SolutionDir)..\bin\$(Configuration)$(PlatformArchitecture)\</code>
                
            <hr class="col-sm-12">
            <h3>Anti-cheat software reports Process Hacker as a game cheating tool!</h3>
      			Unfortunately there is nothing much that can be done about this. Report issues with Anti-cheat software to <a href="about.php">dmex</a>.
        </div> 
 
<div class="col-sm-3">
            <div class="well">
                <ul class="nav nav-list">
                    <li class="nav-header">Tips and Tutorials</li>
                    <?php
                        // Check connection
                        if (mysqli_connect_errno())
                        {
                            echo "<p>Failed to connect to MySQL: ".mysqli_connect_error()."<p>";
                        }
                        else
                        {
                            $sql =  "SELECT t.topic_id,
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
                                    ORDER BY t.topic_id ASC";
                             
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
    
<?php include "include/footer.php"; ?>