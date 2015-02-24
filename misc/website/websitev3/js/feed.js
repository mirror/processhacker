function rssfeedsetup() {
    var feedpointer = new google.feeds.Feed("http://sourceforge.net/p/processhacker/code/feed");
    feedpointer.setNumEntries(5);
    feedpointer.load(displayfeed);
}

function summary(text, limit) {
    text = text.replace("/p/processhacker/code/", "https://sourceforge.net/p/processhacker/code/");
    if (text.length > limit) {
        text = text.substring(0, limit);
        text = text.replace(/\s*$/, "");
        return text.trim() + "...";
    }
    return text.trim();
}

function displayfeed(result) {
    if (!result.error) {
        var rssoutput = "";
        var thefeeds = result.feed.entries;
        for (var i = 0; i < thefeeds.length; i++) {
            rssoutput += "<a href=\" " + thefeeds[i].link + "\" target=\"_blank\" class=\"list-group-item\">";
            rssoutput += "<h4 class=\"list-group-item-heading\">" + summary(thefeeds[i].title, 30) + "</h4>";
            rssoutput += "<p class=\"list-group-item-text\">";
            rssoutput += "<span class=\"text-muted\">" + moment(Date.parse(thefeeds[i].publishedDate)).fromNow() + " by <span style=\"color:#AA0000\">" + thefeeds[i].author + "</span></span>";
            rssoutput += "</p>";
            rssoutput += "</a>";
        }
        window.feedcontainer.innerHTML = rssoutput;
    } else {
        window.feedcontainer.innerHTML = "Error fetching feeds!";
    }
}

window.onload = function() {
    window.feedcontainer = document.getElementById("feeddiv");    
    if (!window.google) {
        window.feedcontainer.innerHTML = "<div>Google API error.</div>";
        return;
    }
    else { 
        window.feedcontainer.innerHTML = "<div>Loading commit history...</div>";
        rssfeedsetup(); 
    }
};