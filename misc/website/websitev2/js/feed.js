/* globals moment, google */

function summary(text, limit) {
    text = text.replace("/p/processhacker/code/", "https://sourceforge.net/p/processhacker/code/");
    if (text.length > limit) {
        text = text.substring(0, limit);
        text = text.replace(/\s*$/, "");
        return text.trim() + "...";
    }
    return text.trim();
}

function displayNewsFeed(result) {
    if (!result.error) {
        var rssOutput = "";
        var theFeeds = result.feed.entries;
        for (var i = 0; i < theFeeds.length; i++) {
            rssOutput += "<div>";
            rssOutput += "<a href=\"" + theFeeds[i].link + "\" target=\"_blank\" \">" + summary(theFeeds[i].title, 32) + "</a>";
            rssOutput += "<span class=\"forumdate\"> by <span class=\"author\">" + theFeeds[i].author + "</span></span>";
            rssOutput += "<div class=\"forumdate\">" + moment(theFeeds[i].publishedDate).fromNow() + " - " + new Date(theFeeds[i].publishedDate).toLocaleString() + "</div>";
            rssOutput += "</div>";
        }
        window.newsFeedContainer.innerHTML = rssOutput;
    } else {
        window.newsFeedContainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

function displayForumfeed(result) {
    if (!result.error) {
        var rssOutput = "";
        var theFeeds = result.feed.entries;
        for (var i = 0; i < theFeeds.length; i++) {
            rssOutput += "<div>";
            rssOutput += "<a href=\"" + theFeeds[i].link + "\" target=\"_blank\" \">" + summary(theFeeds[i].title, 32) + "</a>";
            rssOutput += "<span class=\"forumdate\"> by <span class=\"user\">" + theFeeds[i].author + "</span></span>";
            rssOutput += "<div class=\"forumdate\">" + moment(theFeeds[i].publishedDate).fromNow() + " - " + new Date(theFeeds[i].publishedDate).toLocaleString() + "</div>";
            rssOutput += "</div>";
        }
        window.forumFeedContainer.innerHTML = rssOutput;
    } else {
        window.newsFeedContainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

function displaySvnFeed(result) {
    if (!result.error) {
        var rssOutput = "";
        var theFeeds = result.feed.entries;
        for (var i = 0; i < theFeeds.length; i++) {
            rssOutput += "<div>";
            rssOutput += "<a href=\"" + theFeeds[i].link + "\" target=\"_blank\" \">" + summary(theFeeds[i].title, 32) + "</a>";
            rssOutput += "<span class=\"forumdate\"> by <span style=\"color:#AA0000\">" + theFeeds[i].author + "</span></span>";
            rssOutput += "<div class=\"forumdate\">" + moment(theFeeds[i].publishedDate).fromNow() + " - " + new Date(theFeeds[i].publishedDate).toLocaleString() + "</div>";
            rssOutput += "</div>";
        }
        window.sourceFeedContainer.innerHTML = rssOutput;
    } else {
        window.newsFeedContainer.innerHTML = "Error fetching feed: " + result.error.message;
    }
}

window.onload = function() {
    window.newsFeedContainer = document.getElementById("news_feed_div");
    window.forumFeedContainer = document.getElementById("forum_feed_div");
    window.sourceFeedContainer = document.getElementById("source_feed_div");

    if (!window.google) {
        window.newsFeedContainer.innerHTML = "<div>Google API error.</div>";
        window.forumFeedContainer.innerHTML = "<div>Google API error.</div>";
        window.sourceFeedContainer.innerHTML = "<div>Google API error.</div>";
        return;
    }

    window.newsFeedContainer.innerHTML = "<div>Loading Latest News...</div>";
    window.forumFeedContainer.innerHTML = "<div>Loading Forum Activity...</div>";
    window.sourceFeedContainer.innerHTML = "<div>Loading Commit History...</div>";

    var newsfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?f=1");
    newsfeedpointer.setNumEntries(5);
    newsfeedpointer.load(displayNewsFeed);

    var forumfeedpointer = new google.feeds.Feed("http://wj32.org/processhacker/forums/feed.php?mode=topics_active");
    forumfeedpointer.setNumEntries(5);
    forumfeedpointer.load(displayForumfeed);

    var sourcefeedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
    sourcefeedpointer.setNumEntries(5);
    sourcefeedpointer.load(displaySvnFeed);

};
