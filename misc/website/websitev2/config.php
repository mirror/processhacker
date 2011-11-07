<?php
	// Setup last modified time Caching for the current file.
	// This will keep allow some pages to export caching +2 days from the last time this file was edited.
	$file = __FILE__; 
	$lastmod = date("D, d M Y H:i:s", filemtime($file)); 

	$LATEST_PH_VERSION = "2.23";
	$LATEST_PH_RELEASE_DATE = "7th of November 2011";

	$LATEST_PH_BIN_SIZE = "2.1 MB";
	$LATEST_PH_BIN_SHA1 = "f67228912f505356179874c9152ab99883945ec8";
	$LATEST_PH_BIN_MD5 = "2dccaa616bc9603a2db68a57800de1d9";

	$LATEST_PH_SDK_SIZE = "2.4 MB";
	$LATEST_PH_SDK_SHA1 = "257c4a4670123cd2242f8fe62206ec0749d4340f";
	$LATEST_PH_SDK_MD5 = "6b55c75789633ffec31b794e1f7e025d";

	$LATEST_PH_SETUP_SIZE = "1.7 MB";
	$LATEST_PH_SETUP_SHA1 = "168924a632f937d597dc8b48b3f47f27f24c77c3";
	$LATEST_PH_SETUP_MD5 = "e88da71cbbdeaac5115d9f9ae816574a";

	$LATEST_PH_SOURCE_SIZE = "2.4 MB";
	$LATEST_PH_SOURCE_SHA1 = "3ddd5ca935c5730c8cbbec128db614660a792ad8";
	$LATEST_PH_SOURCE_MD5 = "afa8bcf5c64afeae3db5ac361f31a755";
	
	// How Many Topics you want to display?
	$topicnumber = 5;
	
	define('IN_PHPBB', true);
	$phpbb_root_path = './forums/';
	$phpEx = substr(strrchr(__FILE__, '.'), 1);
	
	// import php functions
	include($phpbb_root_path.'config.'.$phpEx); 
	include($phpbb_root_path.'common.'.$phpEx);
	include($phpbb_root_path.'includes/bbcode.'.$phpEx);
	include($phpbb_root_path.'includes/functions_display.'.$phpEx);

	// Start forum session
	$user->session_begin();
	$auth->acl($user->data);
	$user->setup();
	
	// select database tables
	$table_topics = $table_prefix. "topics";
	$table_forums = $table_prefix. "forums";
	$table_posts = $table_prefix. "posts";
	$table_users = $table_prefix. "users";
	$table_sessions = $table_prefix. "sessions";
?>