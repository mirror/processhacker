<?php
include('config.php');
header("Content-Type: text/xml");

// http://www.go4expert.com/forums/showthread.php?t=3479

// create doctype
$dom = new DOMDocument("1.0", "UTF-8");

// create root element
$root = $dom->createElement("latest");
$dom->appendChild($root);

// create child element
$item = $dom->createElement("ver");
$root->appendChild($item);
// create text node
$text = $dom->createTextNode(LATEST_PH_VERSION);
$item->appendChild($text);

// create child element
$item = $dom->createElement("reldate");
$root->appendChild($item);
// create another text node
$text = $dom->createTextNode(LATEST_PH_RELEASE_DATE);
$item->appendChild($text);

// create child element
$item = $dom->createElement("size");
$root->appendChild($item);
// create another text node
$text = $dom->createTextNode(LATEST_PH_SETUP_SIZE);
$item->appendChild($text);

// create child element
$item = $dom->createElement("sha");
$root->appendChild($item);
// create another text node
$text = $dom->createTextNode(LATEST_PH_SETUP_SHA1);
$item->appendChild($text);

// save and display tree
echo $dom->saveXML();

?>