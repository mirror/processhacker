<?php include('config.php');

header('Content-Type: application/xml; charset=UTF-8');

echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>".PHP_EOL;
echo "<latest>".PHP_EOL;
echo "<ver>".htmlspecialchars($LATEST_PH_VERSION)."</ver>".PHP_EOL;
echo "<reldate>".htmlspecialchars($LATEST_PH_RELEASE_DATE)."</reldate>".PHP_EOL;
echo "<size>".htmlspecialchars($LATEST_PH_SETUP_SIZE)."</size>".PHP_EOL;
echo "<sha1>".$LATEST_PH_SETUP_SHA1."</sha1>".PHP_EOL;
echo "<releasenotes>".htmlspecialchars($LATEST_PH_RELEASE_NEWS)."</releasenotes>".PHP_EOL;
echo "</latest>".PHP_EOL;
?>