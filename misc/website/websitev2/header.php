<?php ob_start("ob_gzhandler"); header('Content-Type: text/html; charset=UTF-8'); ?>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd"><!-- <!DOCTYPE html> -->
<html lang="en">
	<head>
		<meta name="description" content="A free and open source process viewer with powerful process termination and memory searching/editing capabilities." >
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" >
		<!-- <meta charset="utf-8" > -->
		<!-- Always force latest IE rendering engine (even in intranet) & Chrome Frame -->
		<!--[if IE]><meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1"><![endif]-->
		
		<link rel="icon" href="/images/favicon.ico" >
		<link rel="stylesheet" type="text/css" media="screen, print" href="/css/combo.css" >
		<link rel="stylesheet" type="text/css" media="screen" href="/css/lytebox.css">
		
		<script type="text/javascript" language="javascript" src="/scripts/lytebox.js"></script>
		
		<title><?php echo $pagetitle ?> - Process Hacker</title>
	</head>
	
	<body>