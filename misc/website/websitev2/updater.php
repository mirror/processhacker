<?php

include('config.php');

echo "<?xml version='1.0' encoding='ISO-8859-1'?>";

echo "<update>";

echo "<ver>";
echo LATEST_PH_VERSION;
echo "</ver>";

echo "<reldate>";
echo LATEST_PH_RELEASE_DATE;
echo "</reldate>";

echo "<size>";
echo LATEST_PH_SETUP_SIZE;
echo "</size>";

echo "<sha>";
echo LATEST_PH_SETUP_SHA1;
echo "</sha>";

echo "</update>";

?>