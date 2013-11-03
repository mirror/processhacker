<?php

// How Many Topics do you want to display?
$topicnumber = 5;
define('IN_PHPBB', true);
define('IN_LOGIN', true);
// We need to define the root path for phpbb to find it's root directory.
$phpbb_root_path = '../forums/';
$phpEx = 'php';

// import phpbb functions
@include('../forums/config.php');
@include('../forums/common.php');
@include('../forums/includes/bbcode.php');
@include('../forums/includes/functions_display.php');

// select database tables
$table_topics = @$table_prefix. "topics";
$table_forums = @$table_prefix. "forums";
$table_posts = @$table_prefix. "posts";
$table_users = @$table_prefix. "users";
$table_sessions = @$table_prefix. "sessions";
$table_plugins = @$table_prefix. "plugins";

// Check if we imported the phpbb functions.
if (!empty($user) && !empty($auth)) 
{
    // Start forum session
    $user->session_begin();
    $auth->acl($user->data);
    $user->setup();
}


// This function summarizes posts to max. 1200 characters
function summary($str, $limit = 1200, $strip = false) {
    $str = ($strip == true)?strip_tags($str):$str;
    if (strlen ($str) > $limit) {
        $str = substr ($str, 0, $limit - 3);
        return trim(substr ($str, 0, strrpos ($str, ' ')).'...');
    }
    return trim($str);
}
?>