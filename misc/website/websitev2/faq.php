<?php
$pagetitle = "FAQ";

include("header.php");
?>

<div class="page">
    <div class="yui-d0">
        <div id="watermark" class="watermark-apps-portlet">
            <div class="flowed-block">
                <img alt="ProjectLogo" width="64" height="64" src="/images/logo_64x64.png">
            </div>

            <div class="flowed-block wide">
                <h2>Process Hacker</h2>
                <ul class="facetmenu">
                    <li><a href="/">Overview</a></li>
                    <li><a href="/features.php">Features</a></li>
                    <li><a href="/screenshots.php">Screenshots</a></li>
                    <li><a href="/downloads.php">Downloads</a></li>
                    <li class="overview active"><a href="/faq.php">FAQ</a></li>
                    <li><a href="/about.php">About</a></li>
                    <li><a href="/forums/">Forum</a></li>
                </ul>
            </div>
        </div>

        <div class="yui-t4">
            <div class="yui-b side">
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
                    <p><strong>Why should I use Process Hacker? Why not Process Explorer or some other program?</strong></p>
                    <p>
                        Process Hacker offers some pretty unique features, like an awesome run-as tool and
                        the ability to find and terminate hidden processes. The intended users are developers and
                        people interested in Windows internals. This tool is NOT intended for general system
                        optimization.
                    </p>

                    <p><strong>Why is there annoying bug X in Process Hacker? Why is Process Hacker missing feature Y?</strong></p>
                    <p>Please report any bugs or feature requests in the <a href="/forums">forums</a>.</p>

                    <p><strong>"Process Hacker"? Is this a dangerous "hacking" tool?</strong></p>
                    <p>Please read about the <a href="http://catb.org/~esr/faqs/hacker-howto.html#what_is">correct definition of "hacker"</a>.</p>

                    <p><strong>Process Hacker can kill my anti-virus software! Is this a bug in the anti-virus software?</strong></p>
                    <p>No. Please do not report these incidents as bugs because you will be wasting their time.</p>
                    <p><strong>My anti-virus software reports Process Hacker as a threat!</strong></p>
                    <p>
                        This is an expected part of the FUD (fear, uncertainty and doubt) campaigns of anti-virus companies
                        like Kaspersky Lab, who want to scare their users as much as possible. This is completely
                        misleading and irresponsible, but is typical of the "security industry". The source code of
                        Process Hacker is available; verify and build the code yourself if you do not trust the binary releases.
                    </p>
                    <p><strong>Anti-cheat software reports Process Hacker as a game cheating tool!</strong></p>
                    <p>As with anti-virus FUD campaigns, this is similarly ridiculous. Unfortunately there is nothing much that can be done about this.</p>

                    <p><strong>Is Process Hacker a portable application?</strong></p>
                    <p>Yes. If you want settings to be saved on your USB drive, create a file named "ProcessHacker.exe.settings.xml" in the same directory as ProcessHacker.exe. Settings will then automatically be saved there.</p>

                    <p><strong>Symbols don't work properly!</strong></p>
                    <p>
                        Firstly, you need the latest <code>dbghelp.dll</code> version. Install the latest Windows SDK (information and links are located below)</a>, open Process Hacker options, click on Symbols, and locate <code>dbghelp.dll</code>.
                        It is usually in <code>C:\Program Files\Debugging Tools for Windows (x86)</code> or if you're using the Windows 8 SDK it'll be located at <code>C:\Program Files (x86)\Windows Kits\8.0\Debuggers</code>. Next, you need
                        to configure the search path. If you don't know what to do, enter
                        <code>SRV*SOME_FOLDER*http://msdl.microsoft.com/download/symbols</code>.
                        Replace <code>SOME_FOLDER</code> with any folder you can write to, like <code>D:\Symbols</code>.
                        Now you can restart Process Hacker and view full symbols.
                    </p>

                    <p><strong>Why can't I build Process Hacker?</strong></p>
                    <p>The most likely problem is that you do not have the latest Windows SDK installed.</p>
                    <p>For Windows XP, Vista and Windows 7 you'll need the <a href="http://msdn.microsoft.com/en-us/windows/bb980924.aspx">Windows SDK for Windows 7 and .NET Framework 4</a></p>
                    <p>For Windows Vista, Windows 7 and Windows 8 you'll need the <a href="http://msdn.microsoft.com/en-US/windows/hardware/hh852363">Windows 8 SDK</a></p>
                </div>
            </div>
        </div>
    </div>
</div>

<?php include("footer.php"); ?>