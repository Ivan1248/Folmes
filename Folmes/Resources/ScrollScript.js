var RelMouseY;
var maxPageYOffset;
var scrollTrackSpace;
var scrollerRelMouseY;
var scroller;

function setScrollerPos(y) {
    scroller.style.top = (y < 0 ? "0" : y < scrollTrackSpace ? y : scrollTrackSpace) + "px";
}

function updateScrollerPos() {
    setScrollerPos(Math.round(scrollTrackSpace * window.pageYOffset / maxPageYOffset));
}

var scrollTarget = 0;
var scrolling = false;

function smoothScrollToTarget() {
    window.scrollTo(0, (6 * window.pageYOffset + scrollTarget) / 7);
    if (Math.abs(scrollTarget - window.pageYOffset) > 5) {
        setTimeout(smoothScrollToTarget, 10);
    } else {
        window.scrollTo(0, scrollTarget);
        scrolling = false;
    }
    updateScrollerPos();
}

function wheelScroll(e) { // handles window.onmousewheel
    var delta = e.wheelDelta ? e.wheelDelta / 120 : -e.detail / 3;
    scrollTarget -= delta * 50;
    if (scrollTarget < 0) scrollTarget = 0;
    else if (scrollTarget > maxPageYOffset) scrollTarget = maxPageYOffset;
    if (!scrolling) {
        scrolling = true;
        smoothScrollToTarget();
    }
}

function refreshScroller() { // handles, window.onresize
    var doc = document,
        docHeight = doc.documentElement.scrollHeight,
        windowHeight = window.innerHeight,
        scrollerSize = windowHeight * windowHeight / docHeight,
        scrollerStyle = scroller.style;

    window.scrollTo(0, docHeight);

    scrolling = false;
    scrollTarget = window.pageYOffset;

    maxPageYOffset = docHeight - windowHeight;
    scrollTrackSpace = windowHeight - scrollerSize;

    scrollerStyle.height = Math.round(scrollerSize) + "px";
    scrollerStyle.top = scrollTrackSpace + "px";

    updateScrollerPos();
}

function dragNscroll(e) {
    var newScrollerY = e.clientY - scrollerRelMouseY,
        scrollVal = Math.round(maxPageYOffset * newScrollerY / scrollTrackSpace);
    setScrollerPos(newScrollerY);
    window.scrollTo(0, scrollVal);
}

function initializeScroller() { // handles window.onload
    var container = document.getElementById("container");
    scroller = document.createElement("div");
    scroller.id = "scroller";
    container.appendChild(scroller);
    var scrollerTrack = document.createElement("div");
    scrollerTrack.id = "scrollertrack";
    container.appendChild(scrollerTrack);

    scroller.onmousedown = function (e) {
        scrollerRelMouseY = e.clientY - parseInt(scroller.style.top, 10);
        document.addEventListener("mousemove", dragNscroll);
        document.onselectstart = function () { e.preventDefault(); return false; }; // disable text selection
    };
    window.addEventListener("mouseup", function () {
        document.removeEventListener("mousemove", dragNscroll);
        document.onselectstart = null; // enable text selection
    });
    window.addEventListener("mousewheel", wheelScroll);
    window.addEventListener("resize", refreshScroller);
    window.addEventListener("DOMNodeInserted", refreshScroller, false);

    refreshScroller();
}

window.addEventListener("load", initializeScroller);