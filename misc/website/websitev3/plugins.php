<?php $pagetitle = "Plugins"; include "include/header.php"; ?>

<div class="container">
    <h1 class="page-header"><small>Plugins</small></h1>
    <?php
        $dbLink = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbPluginsRo);  

        // Check connection
        if (mysqli_connect_errno())
        {
            echo "Failed to connect to MySQL: " . mysqli_connect_error();
        }
                    
        $sql = "SELECT * FROM p242527_plugins.STABLE db ORDER BY db.name ASC";
                    
        if ($result = mysqli_query($dbLink, $sql))
        {
            while ($row = mysqli_fetch_array($result))
            {
                $plugin_name = $row["name"];
                $plugin_version = $row["version"];
                $plugin_description = $row['description'];
                $plugin_author = $row["author"];
                $plugin_url = $row["url"];
                $plugin_internalName = $row["internalname"];
                            
                echo 
                "<div class=\"row\"> 
                    <div class=\"btn-group pull-right\">
                        <div class=\"label label-primary pull-right\">{$plugin_author}</div>
                        </br>
                        <div class=\"label label-success pull-right\">{$plugin_version}</div>
                    </div>
                    <div class=\"col-md-6\">
                        <div class=\"media-body\">                    
                            <h4 class=\"media-heading\"><a href=\"{$plugin_url}\">{$plugin_name} </a></h4>
                            <small><cite title=\"Source Title\">{$plugin_description}</cite></small>
                        </div>
                    </div>
                </div><hr>";
            }
            mysqli_free_result($result);
        }
        
         mysqli_close($dbLink);
    ?>
</div>

<?php include "include/footer.php"; ?>