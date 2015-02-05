var feedContainer = document.getElementById("feeddiv");
feedContainer.innerHTML = "<div>Loading commit history...</div>";

function displayFeed(result) {
    if (!result.error) {
        var rssOutput = "";
        var theFeeds = result.feed.entries;
        for (var i = 0; i < theFeeds.length; i++) {
            rssOutput += "<div>";
            rssOutput += "<a href=\" " + theFeeds[i].link + " \">" +
                            theFeeds[i].title.replace("/p/processhacker/code/", "http://sourceforge.net/p/processhacker/code/") +
                         "</a>";
            rssOutput += "<span class=\"forumdate\"> by <span class=\"author\">" + theFeeds[i].author + "</span></span>";
            rssOutput += "<div class=\"forumdate\">" + moment(theFeeds[i].publishedDate).fromNow() + " - " + new Date(theFeeds[i].publishedDate).toLocaleString() + "</div>";
            rssOutput += "</div>";
        }

        feedContainer.innerHTML = rssOutput;
    } else {
        feedContainer.innerHTML = "Error fetching feeds!";
    }
}

function rssFeedSetup() {
    var feedPointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
    feedPointer.setNumEntries(5);
    feedPointer.load(displayFeed);
}

window.onload = function () {
    rssFeedSetup();
};
