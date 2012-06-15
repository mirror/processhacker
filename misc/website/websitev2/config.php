<?php
    // Setup last modified time caching for the current file.
    // This will allow caching client-side for +2 days since last update check, unless edited then it resets.
    $file = __FILE__; 
    $lastmod = date("D, d M Y H:i:s", filemtime($file)); 

    $LATEST_PH_VERSION = "2.27";
    $LATEST_PH_RELEASE_DATE = "22nd of January 2012";
    $LATEST_PH_BIN_SIZE = "2.2 MB";
    $LATEST_PH_BIN_SHA1 = "54b285e5e61c8bcde4534b3608c07ebf2c3abc66";
    $LATEST_PH_SDK_SIZE = "2.4 MB";
    $LATEST_PH_SDK_SHA1 = "5c4d14521ff19e264334e3875546f31da925bc47";
    $LATEST_PH_SETUP_SIZE = "1.8 MB";
    $LATEST_PH_SETUP_SHA1 = "b6d90ec86027e474f708b553c7e239dd083c0572";
    $LATEST_PH_SOURCE_SIZE = "2.4 MB";
    $LATEST_PH_SOURCE_SHA1 = "68abb9a6a8a2fd2eec0f5ce86a120819e8e40400";
    
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