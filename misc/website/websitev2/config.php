<?php
	// Setup last modified time Caching for the current file.
	// This will keep allow some pages to export caching +2 days from the last time this file was edited.
	$file = __FILE__; 
	$lastmod = date("D, d M Y H:i:s", filemtime($file)); 

	$LATEST_PH_VERSION = "2.24";
	$LATEST_PH_RELEASE_DATE = "22nd of November 2011";

	$LATEST_PH_BIN_SIZE = "2.1 MB";
	$LATEST_PH_BIN_SHA1 = "ff874ce47cb4579785e552de94075c4d22b1ff27";
	$LATEST_PH_BIN_MD5 = "f29adc95f18823d21fe0c2c45456b83f";

	$LATEST_PH_SDK_SIZE = "2.4 MB";
	$LATEST_PH_SDK_SHA1 = "dc805aecb1de1c21508a2030bc2df9e65e9b4689";
	$LATEST_PH_SDK_MD5 = "0a931b04b0dd3c2e3c51557cff87bf67";

	$LATEST_PH_SETUP_SIZE = "1.7 MB";
	$LATEST_PH_SETUP_SHA1 = "728845cb048c07395ce43f3f4a8cc726f50c6302";
	$LATEST_PH_SETUP_MD5 = "3bf183d204bf64fc695832e833b1cf06";

	$LATEST_PH_SOURCE_SIZE = "2.4 MB";
	$LATEST_PH_SOURCE_SHA1 = "18e0b500d447229f4927bea9d28c518472edff6c";
	$LATEST_PH_SOURCE_MD5 = "1ab76e2c99c353810655853bf66943c9";
	
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