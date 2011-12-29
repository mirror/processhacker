<?php
	// Setup last modified time Caching for the current file.
	// This will keep allow some pages to export caching +2 days from the last time this file was edited.
	$file = __FILE__; 
	$lastmod = date("D, d M Y H:i:s", filemtime($file)); 

	$LATEST_PH_VERSION = "2.25";
	$LATEST_PH_RELEASE_DATE = "29th of November 2011";

	$LATEST_PH_BIN_SIZE = "2.1 MB";
	$LATEST_PH_BIN_SHA1 = "8e0fc4bec4abb5280134451b1e6a39430c26acac";
	$LATEST_PH_BIN_MD5 = "e5a74ae52de06582bf12840dfd0d851f";

	$LATEST_PH_SDK_SIZE = "2.4 MB";
	$LATEST_PH_SDK_SHA1 = "d60afe99ff9e72bd2db05c8426c6c0836eec99a7";
	$LATEST_PH_SDK_MD5 = "59bd1c291e81b93a12d6e0a6a3b98fca";

	$LATEST_PH_SETUP_SIZE = "1.8 MB";
	$LATEST_PH_SETUP_SHA1 = "4cacfc249f4f27325a232f8ce878305fb67ca7a4";
	$LATEST_PH_SETUP_MD5 = "02b80c2d987599624d1933dc51f6eadb";

	$LATEST_PH_SOURCE_SIZE = "2.4 MB";
	$LATEST_PH_SOURCE_SHA1 = "0d994f81a49d7429bfc45676e9b478ed6f30c27c";
	$LATEST_PH_SOURCE_MD5 = "dffb861c48765f8d97468434f1cb9f8f";
	
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