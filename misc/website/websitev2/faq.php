<?php $pagetitle = "FAQ"; include "header.php"; ?>

<div class="page">
    <div class="yui-d0">
        <div class="yui-t4">
            <div class="summary">
                <nav>
                    <div class="logo">
                        <a href="/"><img class="flowed-block" src="images/logo_64x64.png" alt="Project Logo" width="64" height="64"></a>
                    </div>

                    <div class="flowed-block">
                        <h2>Process Hacker</h2>
                        <ul class="facetmenu">
                            <li><a href="/">Overview</a></li>
                            <li><a href="downloads.php">Downloads</a></li>
                            <li class="active"><a href="faq.php">FAQ</a></li>
                            <li><a href="about.php">About</a></li>
                            <li><a href="forums/">Forum</a></li>
                        </ul>
                    </div>
                </nav>

                <dl>
                    <dt>Why is there annoying bug X in Process Hacker? Why is Process Hacker missing feature Y?</dt>
                    <dd>Please report any bugs or feature requests in the <a href="forums">forums</a>.</dd>

                    <dt>Is Process Hacker a portable application?</dt>
                    <dd>
                        Yes. In the same directory as <code>ProcessHacker.exe</code>, create a file named <code>ProcessHacker.exe.settings.xml</code>,
                        settings will then be automatically saved here.
                    </dd>

                    <dt>"Process Hacker"? Is this a dangerous "hacking" tool?</dt>
                    <dd>No. Please read about the <a href="http://catb.org/~esr/faqs/hacker-howto.html#what_is">correct definition of "hacker"</a>.</dd>

                    <dt>Why is Process Hacker able to kill processes that no other tools can kill?</dt>
                    <dd>Process Hacker loads a driver that searches memory for an internal kernel function and calls it. This special function is not known to be hooked by
                    any malware and security software.</dd>

                    <dt>Symbols don't work properly!</dt>
                    <dd>
                        Firstly, you need the latest <code>dbghelp.dll</code> version:<br>
                        <br>
                        1) Install the latest Windows SDK. (links are below)<br>
                        2) Open Process Hacker options via the main menu: Hacker &gt; Options<br>
                        3) Click Symbols, and locate<code>dbghelp.dll</code><br>
                        <dl><dd>
                        It is usually in <code>C:\Program Files\Debugging Tools for Windows (x86)\</code>
                            or if you're using the Windows 8 SDK it'll be located at <code>\Program Files (x86)\Windows Kits\8.0\Debuggers\x86\</code> for 32bit users
                            or at <code>\Program Files (x86)\Windows Kits\8.0\Debuggers\x64\</code> for 64bit users.<br>
                        </dd></dl>
                        Secondly, you need to configure the search path:<br>
                        If you don't know what to do, enter <code>SRV*SOME_FOLDER*http://msdl.microsoft.com/download/symbols</code>.
                        Replace <code>SOME_FOLDER</code> with any folder you can write to, like <code>D:\Symbols</code>.
                        Now you can restart Process Hacker and view full symbols.
                    </dd>

                    <dt>Process Hacker can kill my anti-virus software! Is this a bug in the anti-virus software?</dt>
                    <dd>No. Please do not report these incidents as bugs because you will be wasting their time.</dd>

                    <dt>Anti-cheat software reports Process Hacker as a game cheating tool!</dt>
                    <dd>Unfortunately there is nothing much that can be done about this.</dd>

                    <dt>Why can't I build Process Hacker?</dt>
                    <dd>
                        The most likely problem is that you do not have the latest Windows SDK installed.<br>
                        For Windows XP, Vista and Windows 7 you'll need the
                        <a href="http://msdn.microsoft.com/en-us/windows/bb980924.aspx">Windows SDK for Windows 7 and .NET Framework 4</a>.<br>
                        For Windows Vista, Windows 7 and Windows 8 you'll need the
                        <a href="http://msdn.microsoft.com/en-US/windows/hardware/hh852363">Windows 8 SDK</a>.
                    </dd>
                </dl>
            </div>
        </div>
    </div>
</div>

<?php include "footer.php"; ?>
