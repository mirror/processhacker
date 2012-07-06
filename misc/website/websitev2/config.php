<?php
    // Setup last modified time caching for the current file.
    // This will allow caching client-side for +2 days since last update check, unless edited then it resets.
    $file = __FILE__; 
    $lastmod = date("D, d M Y H:i:s", filemtime($file)); 

    $LATEST_PH_VERSION = "2.28";
    $LATEST_PH_RELEASE_DATE = "6th of July 2012";
    $LATEST_PH_BIN_SIZE = "2.1 MB";
    $LATEST_PH_BIN_SHA1 = "4f8aedc76aa2a6ffe42e8688d854b8c7d39fd83e";
    $LATEST_PH_SDK_SIZE = "2.4 MB";
    $LATEST_PH_SDK_SHA1 = "2f9c79545f117df458fe3fb62c7a40c9f37c8692";
    $LATEST_PH_SETUP_SIZE = "1.8 MB";
    $LATEST_PH_SETUP_SHA1 = "38b82d3db73bdb707181490c1ada68b29aabe196";
    $LATEST_PH_SOURCE_SIZE = "2.4 MB";
    $LATEST_PH_SOURCE_SHA1 = "2bb6915112318b1bce3e6c829068e5ec5d3f4837";
    
    // How Many Topics do you want to display?
    $topicnumber = 6;
    
    // Allow phpbb functions to be called outside of the forum root.
	define('IN_PHPBB', true);   
    // Allow the site to continue running if the board is unavailable,
    // this means the website and update.php pages will continue working as normal
    // if the board is upgrading, disabled etc... instead of replacing the pages and showing 'Board Offline'
    define('IN_LOGIN', true);   
    // We need to find the root path since we're running on Sourceforge's shared-hosts setup,
    // meaning we end up running on multiple servers (and sometimes multiple paths) at the same time.
    // So search for the current __FILE__ path and append the include paths with the current directory.
	$phpbb_root_path = './forums/';
	$phpEx = substr(strrchr(__FILE__, '.'), 1);
	
	// import phpbb functions
	include($phpbb_root_path.'config.'.$phpEx); 
	include($phpbb_root_path.'common.'.$phpEx);
	include($phpbb_root_path.'includes/bbcode.'.$phpEx);
	include($phpbb_root_path.'includes/functions_display.'.$phpEx);
    
    // Check if we imported the phpbb functions.
    if (!empty($user) && !empty($auth))
    {
        // Start forum session
        $user->session_begin();
        $auth->acl($user->data);
        $user->setup();
    }
    
    // select database tables
    $table_topics = $table_prefix. "topics";
    $table_forums = $table_prefix. "forums";
    $table_posts = $table_prefix. "posts";
    $table_users = $table_prefix. "users";
    $table_sessions = $table_prefix. "sessions";
?>