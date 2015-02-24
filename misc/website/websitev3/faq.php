<?php $pagetitle = "FAQ"; include "include/header.php"; ?>

<div class="container">
    <div class="row row-offcanvas row-offcanvas-right">
        <div class="col-xs-12 col-sm-9">
            <h1 class="page-header" id="page-padding"><small>Frequently Asked Questions</small></h1>

            <h3>Is "Process Hacker" a dangerous "hacking" tool?</h3>
                No. Please read about the <a href="http://catb.org/~esr/faqs/hacker-howto.html#what_is">correct definition of "hacker"</a>.

            <hr>
            <h3>Is Process Hacker a portable application?</h3>
                Yes. In the same directory as <code>ProcessHacker.exe</code>, create a file named <code>ProcessHacker.exe.settings.xml</code>, settings will then be automatically saved here.

            <hr>
            <h3>Process Hacker can kill my anti-virus software! Is this a bug in the anti-virus software?</h3>
                No. Please do not report these incidents as bugs because you will be wasting their time.

            <hr>
                <h3>Why is Process Hacker able to kill processes that no other tools can kill?</h3>
                    Process Hacker loads a driver that searches for an internal Microsoft kernel function and uses it for process termination. 
                    This function is not known to be hooked by malware or security software.

            <hr>
            <h3>Why is there annoying bug X in Process Hacker? Why is Process Hacker missing feature Y?</h3>
                    Please report any bugs or feature requests in the <a href="/forums">forums</a>.

            <hr>
                <h3>Why can't I build Process Hacker?</h3>
                    The most likely problem is that you do not have the latest Windows SDK installed.<br>
                    Windows 7, Windows 8 and 8.1 SDK: <a href="http://msdn.microsoft.com/en-US/windows/desktop/bg162891">Windows 8.1 SDK</a>

            <hr>
                <h3>Symbols don't work properly!</h3>
      			Firstly, you need the latest <code>dbghelp.dll</code> version:<br>
                <br>
                1) Install the latest Windows SDK.<br>
                2) Open Process Hacker options via the main menu: Hacker &gt; Options<br>
                3) Click Symbols, and locate <code>dbghelp.dll</code><br><br>
                <dl>
                    <dd>
                        Windows XP, Vista and Windows 7 SDK:<br>
                        <code>\Program Files\Debugging Tools for Windows (x86)\</code><br><br>
                        Windows 8 or above SDK:<br>
                        32bit Windows: <code>\Program Files (x86)\Windows Kits\8.x\Debuggers\x86\</code><br>
                        64bit Windows: <code>\Program Files (x86)\Windows Kits\8.x\Debuggers\x64\</code><br>
                    </dd>
                </dl>
                Secondly, you need to configure the search path. If you don't know what to do, enter:<br>
                <code>SRV*SOME_FOLDER*http://msdl.microsoft.com/download/symbols</code><br><br>
                Replace <code>SOME_FOLDER</code> with any folder you can write to, like <code>D:\Symbols</code>.
                Now you can restart Process Hacker and view full symbols.

            <hr>
                <h3>Why can't I debug my plugin?</h3>
      			The most likely problem is that you have not configured the plugin Solution properties > Debugger options.<br>
                <br>
                For example; Debugging current plugins can be configured using the following settings:<br>
                Debugger Command: <code>$(SolutionDir)..\bin\$(Configuration)$(PlatformArchitecture)\ProcessHacker.exe</code><br>
                Working Directory: <code>$(SolutionDir)..\bin\$(Configuration)$(PlatformArchitecture)\</code>

            <hr>
            <h3>Anti-cheat software reports Process Hacker as a game cheating tool!</h3>
      		Unfortunately there is nothing much that can be done about this. Report issues with Anti-cheat software to <a href="about.php">dmex</a>.
        </div>

        <div class="col-xs-6 col-sm-3 sidebar-offcanvas" id="page-padding">
            <div class="list-group">
                <a href="http://processhacker.sourceforge.net/forums/viewforum.php?f=5" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-comment"></i> Ask a question</a>
                <a href="http://processhacker.sourceforge.net/forums/viewforum.php?f=24" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-fire"></i> Report a bug</a>
                <a href="http://sourceforge.net/projects/processhacker/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-globe"></i> SourceForge project page</a>
                <a href="http://sourceforge.net/p/processhacker/code/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-check"></i> Browse source code</a>
                <a href="http://processhacker.sourceforge.net/doc/" target="_blank" class="list-group-item"><i class="glyphicon glyphicon-edit"></i> Source code documentation</a>
            </div>
        </div>
    </div>
</div>

<?php include "include/footer.php"; ?>