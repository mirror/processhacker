<?php
	// Setup last modified time Caching for the current file.
	// This will keep allow some pages to export caching +2 days from the last time this file was edited.
	$file = __FILE__; 
	$lastmod = date("D, d M Y H:i:s", filemtime($file)); 

	$LATEST_PH_VERSION = "2.22";
	$LATEST_PH_RELEASE_DATE = "19th of September 2011";

	$LATEST_PH_SETUP_SIZE = "1.7 MB";
	$LATEST_PH_SETUP_SHA1 = "0023a7b9a282327b55715aa6caa4eb46996c1e5a";
	$LATEST_PH_SETUP_MD5 = "8b6078f6ca09287303a4199b1b1afeb7";

	$LATEST_PH_BIN_SIZE = "2.0 MB";
	$LATEST_PH_BIN_SHA1 = "e689b5e72ba0e8007f688b1459ab454afd447cbd";
	$LATEST_PH_BIN_MD5 = "f27b6eceb3510ce41fd8383ef5dd8aaf";

	$LATEST_PH_SOURCE_SIZE = "2.4 MB";
	$LATEST_PH_SOURCE_SHA1 = "d224718c4142db673cc1210a7841f4cae3142b10";
	$LATEST_PH_SOURCE_MD5 = "ef581a40981f0202366d8b0898b36a2b";

	$LATEST_PH_SDK_SIZE = "2.4 MB";
	$LATEST_PH_SDK_SHA1 = "23c9dc21aa4628bf911b768f4b242c10c54cd01c";
	$LATEST_PH_SDK_MD5 = "d1cf4c0609b6c4d2cee2217bd89b479e";
?>