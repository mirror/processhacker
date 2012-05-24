<?php include('config.php');

header('Content-Type: application/xml; charset=UTF-8');
// output the config.php modification date as a cache control helper.
header("last-modified: {$lastmod} GMT");
// calc the expires string in GMT not localtime and add the offset for two hours using php.
header($expires = "Expires: ".gmdate("D, d M Y H:i:s", strtotime('+2 days'))." GMT");
// setup caching.
header("Cache-Control: {$expires}");

echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>".PHP_EOL;
echo "<latest>".PHP_EOL;
echo "<ver>{$LATEST_PH_VERSION}</ver>".PHP_EOL;
echo "<reldate>{$LATEST_PH_RELEASE_DATE}</reldate>".PHP_EOL;
echo "<size>{$LATEST_PH_SETUP_SIZE}</size>".PHP_EOL;
echo "<sha1>{$LATEST_PH_SETUP_SHA1}</sha1>".PHP_EOL;
echo "<md5>{$LATEST_PH_SETUP_MD5}</md5>".PHP_EOL;
echo "</latest>".PHP_EOL;
?>