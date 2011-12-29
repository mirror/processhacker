<?php
	// Setup last modified time Caching for the current file.
	// This will keep allow some pages to export caching +2 days from the last time this file was edited.
	$file = __FILE__; 
	$lastmod = date("D, d M Y H:i:s", filemtime($file)); 

	$LATEST_PH_VERSION = "2.26";
	$LATEST_PH_RELEASE_DATE = "29th of December 2011";

	$LATEST_PH_BIN_SIZE = "2.1 MB";
	$LATEST_PH_BIN_SHA1 = "eef16c19acc04564ff3ed329f9aae53a97c9d99a";
	$LATEST_PH_BIN_MD5 = "f206761f5d9d302eca833ba60432fc48";

	$LATEST_PH_SDK_SIZE = "2.4 MB";
	$LATEST_PH_SDK_SHA1 = "60d3254d0392b68b820ca23b021761450b22e18c";
	$LATEST_PH_SDK_MD5 = "60d4f6b4e8d150148a9e2a936f1f70ae";

	$LATEST_PH_SETUP_SIZE = "1.8 MB";
	$LATEST_PH_SETUP_SHA1 = "3d2711d4ecd4776a7d4558551e2ffde7eb575c86";
	$LATEST_PH_SETUP_MD5 = "169d795087936cf067034cc4f4dea3bc";

	$LATEST_PH_SOURCE_SIZE = "2.4 MB";
	$LATEST_PH_SOURCE_SHA1 = "a0adb406dd33a97b9f3d1b4c2b35876c1d8d7ae0";
	$LATEST_PH_SOURCE_MD5 = "f2433dfae7a6dd0f7479462332a22d48";
	
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