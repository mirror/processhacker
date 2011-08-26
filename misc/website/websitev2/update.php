<?php include('config.php'); 

//header("Pragma: public");
header('Pragma: no-cache');
header('Expires: Mon, 26 Jul 1997 05:00:00 GMT');
header('Cache-Control: post-check=0, pre-check=0', FALSE); 
header('Cache-Control: no-store, no-cache, must-revalidate');
//header('Cache-Control: max-age=28800');

//calc an offset of 24 hours
//$offset = 3600 * 24;
//calc the string in GMT not localtime and add the offset
//$expire = "Cache-Control: Expires: " . gmdate("D, d M Y H:i:s", time() + offset) . " GMT";
//output the HTTP header
//header($expire);
header('Content-Type: text/xml; charset=UTF-8');

echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>".PHP_EOL;
echo "<latest>".PHP_EOL;
echo "<ver>".LATEST_PH_VERSION."</ver>".PHP_EOL;
echo "<reldate>".LATEST_PH_RELEASE_DATE."</reldate>".PHP_EOL;
echo "<size>".LATEST_PH_SETUP_SIZE."</size>".PHP_EOL;
echo "<sha1>".LATEST_PH_SETUP_SHA1."</sha1>".PHP_EOL;
echo "<md5>".LATEST_PH_SETUP_MD5."</md5>".PHP_EOL;
echo "</latest>".PHP_EOL;
?>