<?php $pagetitle = "About"; include "include/header.php"; ?>

<div class="row">
    <div class="col-lg-4">        
        <div class="panel panel-default">
            <div class="panel-body">
                Basic panel
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">Panel heading</div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-body">
                Panel content
            </div>
            <div class="panel-footer">Panel footer</div>
        </div>
    </div>
    <div class="col-lg-4">
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title">Panel primary</h3>
            </div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
        <div class="panel panel-success">
            <div class="panel-heading">
                <h3 class="panel-title">Panel success</h3>
            </div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
        <div class="panel panel-warning">
            <div class="panel-heading">
                <h3 class="panel-title">Panel warning</h3>
            </div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
    </div>
    <div class="col-lg-4">
        <div class="panel panel-danger">
            <div class="panel-heading">
                <h3 class="panel-title">Panel danger</h3>
            </div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
        <div class="panel panel-info">
            <div class="panel-heading">
                <h3 class="panel-title">Panel info</h3>
            </div>
            <div class="panel-body">
                Panel content
            </div>
        </div>
    </div>
</div>

<div class="col-xs-12 col-sm-9">
    <div class="row">
        <h1 class="page-header"><small>About</small></h1>
        <div class="col-md-12">
            <p>Process Hacker was started in 2008 as an open source alternative to programs such as Task Manager and Process Explorer.</p>
                <ul>
                    <li><strong>Registered:</strong> 16-10-2008</li>
                    <li><strong>Licence:</strong> GNU General Public License version 3.0 <a href="http://processhacker.svn.sourceforge.net/viewvc/processhacker/2.x/trunk/LICENSE.txt">GPLv3</a></li>
                    <li><strong>Language:</strong> English</li>
                    <li><strong>Intended Audience:</strong> Advanced End Users, Developers</li>
                    <li><strong>Programming Language:</strong> C, C#</li>
                </ul>
        </div>
    </div>
    <div class="row">
        <h1 class="page-header"><small>Ohloh Stats</small></h1>
        <div class="pull-left">
            <script src="http://www.ohloh.net/p/processhacker/widgets/project_basic_stats.js"></script>
        </div>
    </div>
</div>
    
<div class="col-xs-6 col-sm-3 sidebar-offcanvas" id="sidebar" role="navigation">
    <div class="well">
        <ul class="nav nav-list">
            <li class="nav-header">Project Links</li>
            <li><a href="http://sourceforge.net/projects/processhacker/">SourceForge project page</a></li>
            <li><a href="http://sourceforge.net/p/processhacker/code/">Browse source code</a></li>
            <li><a href="doc/">Source code documentation</a></li>
        </ul>
    </div>
</div>
 
<?php include "include/footer.php"; ?>