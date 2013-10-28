var feedcontainer = document.getElementById("feeddiv");
feedcontainer.innerHTML = "<div>Loading commit history...</div>";

function rssfeedsetup() {
    var feedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
    feedpointer.setNumEntries(5);
    feedpointer.load(displayfeed);
}

function displayfeed(result) {
    if (!result.error) {
        var rssoutput = "";
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<div id='forumitem'>";
            rssoutput += "<a href=\" " + thefeeds[i].link + " \">" +
                            thefeeds[i].title.replace("/p/processhacker/code/", "http://sourceforge.net/p/processhacker/code/") +
                         "</a>";
            rssoutput += "<span id=\"forumdate\"> by <span id=\"author\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "<div id=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
            rssoutput += "</div>";
        }

        feedcontainer.innerHTML = rssoutput;
    } else {
        feedcontainer.innerHTML = "Error fetching feeds!";
    }
}

window.onload = function() {
    rssfeedsetup();
};
