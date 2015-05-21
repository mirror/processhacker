function summary(text, limit) {
    text = text.replace("/p/processhacker/code/", "https://sourceforge.net/p/processhacker/code/");
    if (text.length > limit) {
        text = text.substring(0, limit);
        text = text.replace(/\s*$/, "");
        return text.trim() + "...";
    }
    return text.trim();
}

function DisplayNewsFeed(result) {
    if (!result.error) {
        var rssoutput = "";
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<div>";
            rssoutput += "<a href=\"" + thefeeds[i].link + "\" target=\"_blank\" \">" + summary(thefeeds[i].title, 32) + "</a>";
            rssoutput += "<span class=\"forumdate\"> by <span class=\"author\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "<div class=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
            rssoutput += "</div>";
        }
        window.newsfeedcontainer.innerHTML = rssoutput;
    } else {
        window.newsfeedcontainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

function DisplayForumfeed(result) {
    if (!result.error) {
        var rssoutput = "";
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<div>";
            rssoutput += "<a href=\"" + thefeeds[i].link + "\" target=\"_blank\" \">" + summary(thefeeds[i].title, 32) + "</a>";
            rssoutput += "<span class=\"forumdate\"> by <span class=\"user\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "<div class=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
            rssoutput += "</div>";
        }
        window.forumfeedcontainer.innerHTML = rssoutput;
    } else {
        window.newsfeedcontainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

function DisplaySvnFeed(result) 
{
    if (!result.error) {
        var rssoutput = "";
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<div>";
            rssoutput += "<a href=\"" + thefeeds[i].link + "\" target=\"_blank\" \">" + summary(thefeeds[i].title, 32) + "</a>";
            rssoutput += "<span class=\"forumdate\"> by <span style=\"color:#AA0000\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "<div class=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
            rssoutput += "</div>";
        }
        window.sourcefeedcontainer.innerHTML = rssoutput;
    } else {
        window.newsfeedcontainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

window.onload = function() {
    window.newsfeedcontainer = document.getElementById("news_feed_div");
    window.forumfeedcontainer = document.getElementById("forum_feed_div");    
    window.sourcefeedcontainer = document.getElementById("source_feed_div");    
    
    if (!window.google) {
        window.newsfeedcontainer.innerHTML = "<div>Google API error.</div>";
        window.forumfeedcontainer.innerHTML = "<div>Google API error.</div>";
        window.sourcefeedcontainer.innerHTML = "<div>Google API error.</div>";
        return;
    } else { 
        window.newsfeedcontainer.innerHTML = "<div>Loading Latest News...</div>";
        window.forumfeedcontainer.innerHTML = "<div>Loading Forum Activity...</div>";
        window.sourcefeedcontainer.innerHTML = "<div>Loading Commit History...</div>";
        
        var newsfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?f=1");
        newsfeedpointer.setNumEntries(5);
        newsfeedpointer.load(DisplayNewsFeed);

        var forumfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?mode=topics_active");
        forumfeedpointer.setNumEntries(5);
        forumfeedpointer.load(DisplayForumfeed);

        var sourcefeedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
        sourcefeedpointer.setNumEntries(5);
        sourcefeedpointer.load(DisplaySvnFeed);
    }
};