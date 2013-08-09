var feedcontainer=document.getElementById("feeddiv");
var rssoutput = "";

feedcontainer.innerHTML = "<div id=\"feeddiv2\">Loading commit history...</div>";

function rssfeedsetup() {
    var feedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
    feedpointer.setNumEntries(5);
    feedpointer.load(displayfeed);
}

function displayfeed(result) {
    if (!result.error) {
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<div>";
            rssoutput += "<a href=\" " + thefeeds[i].link + " \">" +
                            thefeeds[i].title.replace("/p/processhacker/code/", "http://sourceforge.net/p/processhacker/code/") +
                         "</a>";
            rssoutput += "<span class=\"forumdate\"> by <span style=\"color:#A00\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "<div class=\"forumdate\">" + moment(thefeeds[i].publishedDate).fromNow() + " - " + new Date(thefeeds[i].publishedDate).toLocaleString() + "</div>";
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
