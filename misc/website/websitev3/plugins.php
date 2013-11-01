<?php $pagetitle = "Plugins"; include "include/header.php"; 

$dbLink = mysqli_connect($dbHostRo, $dbUserRo, $dbPasswdRo, $dbPluginsRo);  
?>

<div class="row">
    <h1 class="page-header"><small>Plugins</small></h1>
</div>

<div class="row row-offcanvas row-offcanvas-right">
    <div class="col-sm-9">
        <table class="table table-bordered">
            <tbody>
                <tr>
                    <th>Name</th>
                    <th>Description</th>
                    <th>Version</th>
                    <th>Author</th>
                </tr>
                
                <?php
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
                            "<tr>
                                <td><a href=\"{$plugin_url}\">{$plugin_name}</a></td>
                                <td>{$plugin_description}</td>
                                <td>{$plugin_version}</td>
                                <td style=\"color:#AA0000\">{$plugin_author}</td>
                            </tr>";
                        }
                        mysqli_free_result($result);
                    }
                ?>
            </tbody>
        </table>
    </div>
    
    <div class="col-sm-3 sidebar-offcanvas" id="sidebar" role="navigation">
        <div class="well sidebar-nav">
            <ul class="nav">
                <li><a href="http://sourceforge.net/projects/processhacker/">SourceForge project page</a></li>
                <li><a href="forums/viewforum.php?f=5">Ask a question</a></li>
                <li><a href="forums/viewforum.php?f=24">Report a bug</a></li>
                <li><a href="http://sourceforge.net/p/processhacker/code/">Browse source code</a></li>
                <li><a href="doc/">Source code documentation</a></li>
            </ul>
        </div>
    </div>
</div>

<?php 

if (!empty($dbLink))
{
    mysqli_close($conn);
}

include "include/footer.php"; ?>