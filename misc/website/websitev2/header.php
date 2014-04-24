<?php include "config.php";

// =================================================
// AD CODE -- DO NOT CHANGE
$DISABLE_ADS = false;
$LIVE_ADS = true;

function ad_unit_1()
{
    global $DISABLE_ADS;
    global $LIVE_ADS;

    if ($DISABLE_ADS) {
        return;
    }

    if ($LIVE_ADS)
    {
        echo <<<END
<script type="text/javascript"><!--
google_ad_client = "ca-pub-9059400272001645";
/* Unit 1 */
google_ad_slot = "8216753389";
google_ad_width = 300;
google_ad_height = 250;
//-->
</script>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
END;
    }
    else
    {
        echo '<img src="img/ad_300x250.png">';
    }
}

function ad_unit_3()
{
    global $DISABLE_ADS;
    global $LIVE_ADS;

    if ($DISABLE_ADS) {
        return;
    }

    if ($LIVE_ADS)
    {
        echo <<<END
<script type="text/javascript"><!--
google_ad_client = "ca-pub-9059400272001645";
/* Unit 3 */
google_ad_slot = "3762227384";
google_ad_width = 336;
google_ad_height = 280;
//-->
</script>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
END;
    }
    else
    {
        echo '<img src="img/ad_336x280.png">';
    }
}

function ad_unit_4()
{
    global $DISABLE_ADS;
    global $LIVE_ADS;

    if ($DISABLE_ADS) {
        return;
    }

    if ($LIVE_ADS)
    {
        echo <<<END
<script type="text/javascript"><!--
google_ad_client = "ca-pub-9059400272001645";
/* Unit 4 */
google_ad_slot = "9669160189";
google_ad_width = 728;
google_ad_height = 90;
//-->
</script>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
END;
    }
    else
    {
        echo '<img src="img/ad_728x90.png">';
    }
}

function ad_unit_5()
{
    global $DISABLE_ADS;
    global $LIVE_ADS;

    if ($DISABLE_ADS) {
        return;
    }

    if ($LIVE_ADS)
    {
        echo <<<END
<script type="text/javascript"><!--
google_ad_client = "ca-pub-9059400272001645";
/* Unit 5 */
google_ad_slot = "3622626584";
google_ad_width = 336;
google_ad_height = 280;
//-->
</script>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
END;
    }
    else
    {
        echo '<img src="img/ad_336x280.png">';
    }
}

// END AD CODE

// =================================================
// Setup database details
@include('./forums/config.php');

$topicnumber = 5;
$table_topics = @$table_prefix. "topics";
$table_forums = @$table_prefix. "forums";
$table_posts = @$table_prefix. "posts";
$table_users = @$table_prefix. "users";
$table_sessions = @$table_prefix. "sessions";

// function for converting time into time elapsed
function get_time_ago($time_stamp)
{
    $time_difference = strtotime('now') - $time_stamp;

    if ($time_difference >= 60 * 60 * 24 * 365.242199) {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 365.242199 days/year
         * This means that the time difference is 1 year or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 365.242199, 'year');
    } elseif ($time_difference >= 60 * 60 * 24 * 30.4368499) {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 30.4368499 days/month
         * This means that the time difference is 1 month or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 30.4368499, 'month');
    } elseif ($time_difference >= 60 * 60 * 24 * 7) {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day * 7 days/week
         * This means that the time difference is 1 week or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24 * 7, 'week');
    } elseif ($time_difference >= 60 * 60 * 24) {
        /*
         * 60 seconds/minute * 60 minutes/hour * 24 hours/day
         * This means that the time difference is 1 day or more
         */
        return get_time_ago_string($time_stamp, 60 * 60 * 24, 'day');
    } elseif ($time_difference >= 60 * 60) {
        /*
         * 60 seconds/minute * 60 minutes/hour
         * This means that the time difference is 1 hour or more
         */
        return get_time_ago_string($time_stamp, 60 * 60, 'hour');
    } else {
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
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title><?php echo $pagetitle ?> - Process Hacker</title>
    <meta name="description" content="A free and open source process viewer with powerful process termination and memory searching/editing capabilities.">
    <link rel="shortcut icon" href="favicon.ico">
    <link rel="stylesheet" href="css/pack.css">
    <link rel="alternate" type="application/atom+xml" href="http://processhacker.sourceforge.net/forums/feed.php?f=1" title="Process Hacker - News">
    <link rel="alternate" type="application/atom+xml" href="http://sourceforge.net/p/processhacker/code/feed" title="Process Hacker - SVN">
    <!--[if lt IE 9]>
    <script src="js/html5shiv.min.js"></script>
    <![endif]-->
</head>
<body>