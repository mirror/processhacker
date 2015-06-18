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
            rssoutput += "<a href=\" " + thefeeds[i].link + "\" target=\"_blank\" class=\"list-group-item\">";
            rssoutput += "<h4 class=\"list-group-item-heading\">" + summary(thefeeds[i].title, 32) + "</h4>";
            rssoutput += "<p class=\"list-group-item-text\">";
            rssoutput += "<span class=\"text-muted\">" + moment(thefeeds[i].publishedDate, "ddd, D MMM YYYY HH:mm:ss ZZ").subtract(7, "h").fromNow() + " by <span style=\"color:#AA0000\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "</p>";
            rssoutput += "</a>";
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
            rssoutput += "<a href=\" " + thefeeds[i].link + "\" target=\"_blank\" class=\"list-group-item\">";
            rssoutput += "<h4 class=\"list-group-item-heading\">" + summary(thefeeds[i].title, 32) + "</h4>";
            rssoutput += "<p class=\"list-group-item-text\">";
            rssoutput += "<span class=\"text-muted\">" + moment(thefeeds[i].publishedDate, "ddd, D MMM YYYY HH:mm:ss ZZ").subtract(7, "h").fromNow() + " by <span style=\"color:#000\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "</p>";
            rssoutput += "</a>";
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
            rssoutput += "<a href=\" " + thefeeds[i].link + "\" target=\"_blank\" class=\"list-group-item\">";
            rssoutput += "<h4 class=\"list-group-item-heading\">" + summary(thefeeds[i].title, 32) + "</h4>";
            rssoutput += "<p class=\"list-group-item-text\">";
            rssoutput += "<span class=\"text-muted\">" + moment(Date.parse(thefeeds[i].publishedDate)).fromNow() + " by <span style=\"color:#AA0000\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "</p>";
            rssoutput += "</a>";
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
    }
    else { 
        window.newsfeedcontainer.innerHTML = "<div>Loading Latest News...</div>";
        window.forumfeedcontainer.innerHTML = "<div>Loading Forum Activity...</div>";
        window.sourcefeedcontainer.innerHTML = "<div>Loading Commit History...</div>";
        
        var newsfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?f=1");
        newsfeedpointer.setNumEntries(5);
        newsfeedpointer.load(DisplayNewsFeed);

        var forumfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?mode=topics_active");
        forumfeedpointer.setNumEntries(5);
        forumfeedpointer.load(DisplayForumfeed);

        var sourcefeedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed"); // http://sourceforge.net/p/processhacker/activity/feed
        sourcefeedpointer.setNumEntries(5);
        sourcefeedpointer.load(DisplaySvnFeed);
    }
};