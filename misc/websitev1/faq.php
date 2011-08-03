<?php
    $title = "FAQ";
    include("header.php");
?>

<dl>
  <dt>Why should I use Process Hacker? Why not Process Explorer or some other program?</dt>
  <dd>Process Hacker offers some pretty unique features, like an awesome run-as tool and 
  the ability to find and terminate hidden processes. The intended users are developers and 
  people interested in Windows internals. This tool is NOT intended for general system 
  optimization.</dd>
  
  <dt>"Process Hacker"? Is this a dangerous "hacking" tool?</dt>
  <dd>Please read about the 
  <a href="http://catb.org/~esr/faqs/hacker-howto.html#what_is">correct definition of 
  "hacker"</a>.</dd>
  
  <dt>Why is there annoying bug X in Process Hacker? Why is Process Hacker missing 
  feature Y?</dt>
  <dd>Please report any bugs to our 
  <a href="https://sourceforge.net/tracker2/?group_id=242527">tracker</a>. Feature requests 
  should be posted in the <a href="http://processhacker.sourceforge.net/forums/">forums</a>.</dd>
  
  <dt>Is Process Hacker a portable application?</dt>
  <dd>Yes. If you want settings to be saved on your USB drive, create a file named 
  "ProcessHacker.exe.settings.xml" in the same directory as ProcessHacker.exe. Settings will then 
  automatically be saved there.</dd>
  
  <dt>Symbols don't work properly!</dt>
  <dd>Firstly, you need the latest <code>dbghelp.dll</code> version. Install 
  <a href="http://www.microsoft.com/whdc/devtools/debugging/installx86.mspx">Debugging Tools 
  for Windows</a>, open Process Hacker options, click on Symbols, and locate <code>dbghelp.dll</code>. 
  It is usually in <code>C:\Program Files\Debugging Tools for Windows (x86)</code>. Next, you need 
  to configure the search path. If you don't know what to do, enter 
  <code>SRV*SOME_FOLDER*http://msdl.microsoft.com/download/symbols</code>. 
  Replace <code>SOME_FOLDER</code> with any folder you can write to, like <code>D:\Symbols</code>. 
  Now you can restart Process Hacker and view full symbols.</dd>
  
  <dt>Why can't I build Process Hacker?</dt>
  <dd>The most likely problem is that you do not have the latest 
  <a href="http://msdn.microsoft.com/en-us/windows/bb980924.aspx">Windows SDK</a> installed.</dd>
  
  <dt>Process Hacker can kill my anti-virus software! Is this a bug in the anti-virus software?</dt>
  <dd>No. Please do not report these incidents as bugs because you will be wasting their time.</dd>
  
  <dt>My anti-virus software reports Process Hacker as a threat!</dt>
  <dd>This is an expected part of the FUD (fear, uncertainty and doubt) campaigns of anti-virus companies 
  like Kaspersky Lab, who want to scare their users as much as possible. This is completely 
  misleading and irresponsible, but is typical of the "security industry". The source code of 
  Process Hacker is available; verify and build the code yourself if you do not trust the binary 
  releases.</dd>
  
  <dt>Anti-cheat software reports Process Hacker as a game cheating tool!</dt>
  <dd>As with anti-virus FUD campaigns, this is similarly ridiculous. Unfortunately there is nothing 
  much that can be done about this.</dd>
</dl>

<?php include("footer.php"); ?>
