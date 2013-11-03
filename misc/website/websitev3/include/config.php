<?php
$LATEST_PH_VERSION = "2.32";
$LATEST_PH_BUILD = "5521";
$LATEST_PH_RELEASE_DATE = "31st of October 2013";

$LATEST_PH_BIN_SIZE = "2.7 MB";
$LATEST_PH_BIN_SHA1 = "22543a04c2c68a9e9c1396184da33c2ae0c890d4";

$LATEST_PH_SDK_SIZE = "2.7 MB";
$LATEST_PH_SDK_SHA1 = "f6deb41d216eb5e61b13a77e7155d2e9e0211359";

$LATEST_PH_SETUP_SIZE = "1.9 MB";
$LATEST_PH_SETUP_SHA1 = "aad89a04af0e0cbbe8d9cef2e218b8eeb03f30b0";

$LATEST_PH_SOURCE_SIZE = "2.6 MB";
$LATEST_PH_SOURCE_SHA1 = "7d6368e068975a9aa3426dc2b75920e8970ae2c6";

$LATEST_PH_RELEASE_NEWS = "http://processhacker.sourceforge.net/forums/viewtopic.php?f=1&t=1229";



// =================================================
// Setup database details
@include('../forums/config.php');

$topicnumber = 5;
$table_topics = @$table_prefix. "topics";
$table_forums = @$table_prefix. "forums";
$table_posts = @$table_prefix. "posts";
$table_users = @$table_prefix. "users";
$table_sessions = @$table_prefix. "sessions";
$table_plugins = @$table_prefix. "plugins";

// function for converting time into time elapsed
function get_time_ago($time_stamp)
{
    $time_difference = strtotime('now') - $time_stamp;

    if ($time_difference >= 60 * 60 * 24 * 365.242199) 
    {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 365.242199 days/year
         * This means that the time difference is 1 year or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 365.242199, 'year');
    } 
    elseif ($time_difference >= 60 * 60 * 24 * 30.4368499) 
    {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 30.4368499 days/month
         * This means that the time difference is 1 month or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 30.4368499, 'month');
    } 
    elseif ($time_difference >= 60 * 60 * 24 * 7) 
    {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 7 days/week
         * This means that the time difference is 1 week or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 7, 'week');
    } 
    elseif ($time_difference >= 60 * 60 * 24) 
    {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day
         * This means that the time difference is 1 day or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24, 'day');
    } 
    elseif ($time_difference >= 60 * 60) 
    {
        /*
         * 60 seconds/minute * 60 minutes/hour
         * This means that the time difference is 1 hour or more
         */
        return get_time_ago_string($time_stamp, 60 * 60, 'hour');
    } 
    else 
    {
        /*
         * 60 seconds/minute
         * This means that the time difference is a matter of minutes
         */
        return get_time_ago_string($time_stamp, 60, 'minute');
    }
}

function get_time_ago_string($time_stamp, $divisor, $time_unit)
{
    $time_difference = strtotime("now") - $time_stamp;
    $time_units      = floor($time_difference / $divisor);

    settype($time_units, 'string');

    if ($time_units === '0') {
        return 'less than 1 ' . $time_unit . ' ago';
    } elseif ($time_units === '1') {
        return '1 ' . $time_unit . ' ago';
    } else {
        /*
         * More than "1" $time_unit. This is the "plural" message.
         */
        // TODO: This pluralizes the time unit, which is done by adding "s" at the end; this will not work for i18n!
        return $time_units . ' ' . $time_unit . 's ago';
    }
}
?>
