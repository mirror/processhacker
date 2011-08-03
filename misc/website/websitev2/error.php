<?php $pagetitle = "Error ".$_SERVER['REDIRECT_STATUS']; include("header.php"); 

	function curPageURL() 
	{
		 $pageURL = 'http';
		 if ($_SERVER["HTTPS"] == "on") {$pageURL .= "s";}
		 $pageURL .= "://";
		 if ($_SERVER["SERVER_PORT"] != "80") {
		  $pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
		 } else {
		  $pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
		 }
	 return $pageURL;
	}
?>

<center style="max-width: 80em; align: center; margin: 0 auto;"> 
	<div class="yui-d0">
		<div id="watermark" class="watermark-apps-portlet">			
			<div class="flowed-block">
				<img alt="" width="64" height="64" src="/images/logo.png">
			</div>
			
			<div class="flowed-block wide">
				<h2>Process Hacker</h2>
			  
				<ul class="facetmenu">					
					<li><a href="/">Overview</a></li>							
					<li><a href="/features.php">Features</a></li>
					<li><a href="/screenshots.php">Screenshots</a></li>
					<li><a href="/downloads.php">Downloads</a></li>
					<li><a href="/faq.php">FAQ</a></li>
					<li><a href="/about.php">About</a></li>
					<li><a href="/forums/">Forum</a></li>
				</ul>
			</div>
		</div>	
		</div>
	</div>	
	
	<div class="top-portlet" style="width: 70%;">				
		<div class="summary">
			</br>
			<span class="neg">ERROR <?php echo $AA_STATUS_CODE ?></span>
			<p>
				<?php echo curPageURL(); ?>
			</p>

			<li>* Send us an e-mail to notify this and try it later.</li>
			<li>* Press CTRL+ALT+DEL to restart your computer. You will<br />
				 &nbsp; lose unsaved information in any programs that are running.
			</li>
			
			<li><?php echo $php_errormsg ?></li>
			
			<div class="menu">
				<a href="/">Home</a> |
				<a href="/about.php">Contact</a>
			</div>

			</div>
		</div>
	</div>
	
</center>

<?php include("footer.php"); ?>