<?php //include('config.php'); 
// output the config.php modification date as a cache control helper.
//header("last-modified: {$lastmod} GMT");
// calc the expires string in GMT not localtime and add the offset for two hours using php.
//header($expires = "Expires: ".gmdate("D, d M Y H:i:s", strtotime('+2 days'))." GMT");
// setup caching.
//header("Cache-Control: {$expires}");
//
    header("Cache-Control: {$expires}");
    header('Content-type: application/xml; charset=UTF-8');
    header("last-modified: {$lastmod} GMT");
    header($expires = "Expires: ".gmdate("D, dMYH:i:s", strtotime('+2 days'))." GMT");
    
    include("config.php"); 

    echo '<?xml version="1.0" encoding="ISO-8859-1" ?>';
?>

<latest>
<ver><?php echo $LATEST_PH_VERSION ?></ver>
<rev><?php echo $LATEST_PH_REVISION ?></rev>
<reldate><?php echo $LATEST_PH_RELEASE_DATE ?></reldate>
<size><?php echo $LATEST_PH_SETUP_SIZE ?></size>
<sha1><?php echo $LATEST_PH_SETUP_SHA1 ?></sha1>
<md5><?php echo $LATEST_PH_SETUP_MD5 ?></md5>
</latest>