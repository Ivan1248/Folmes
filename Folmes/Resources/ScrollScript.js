var scrollerRelMouseY;
var pageLuft;
var scrollerLuft;
var dragging = false;
var doc = document;

function setScrollerPos(Y) {
    document.getElementById('scroller').style.top = (Y < 0 ? "0" : Y < scrollerLuft ? Y : scrollerLuft) + "px";
}

function dragNscroll(e) {
    var newScrollerY = e.clientY - scrollerRelMouseY,
        scrollVal = Math.round(pageLuft * newScrollerY / scrollerLuft);
    setScrollerPos(newScrollerY);
    window.scrollTo(0, scrollVal);
}

function updateScrollerPos() {
    setScrollerPos(Math.round(scrollerLuft * window.pageYOffset / pageLuft));
}

var scrollTarget = 0;
var scrolling = false;
function smoothScroll() {
    if (scrollTarget < 0) {
        scrollTarget = -5;
    } else if (scrollTarget > pageLuft) {
        scrollTarget = pageLuft + 5;
    }
    window.scrollTo(0, (6 * window.pageYOffset + scrollTarget) / 7);
    if (Math.abs(scrollTarget - window.pageYOffset) > 5) {
        setTimeout(smoothScroll, 10);
    } else {
        scrollTarget = Math.round(window.pageYOffset);
        scrolling = false;
    }
    updateScrollerPos();
}

function wheelScroll(e) {
    var delta = e.wheelDelta ? e.wheelDelta / 120 : -e.detail / 3;
    scrollTarget = parseInt(scrolling ? scrollTarget : window.pageYOffset, 10) - delta * 50;
    if (!scrolling) {
        setTimeout(smoothScroll, 10);
        scrolling = true;
    }
}
window.onmousewheel = wheelScroll;

function scrollDown(e) {
    window.scrollTo(0, document.documentElement.scrollHeight - e);
}

function loadScroller() {
    dragging = false;

    var doc = document,
        scroller = doc.getElementById('scroller'),
        docHeight = doc.documentElement.scrollHeight,
        windowHeight = window.innerHeight,
        scrollerSize = windowHeight * windowHeight / docHeight,
        scrollerStyle = scroller.style;

    pageLuft = docHeight - windowHeight;
    scrollerLuft = windowHeight - scrollerSize;

    scrollerStyle.height = Math.round(scrollerSize) + "px";
    scrollerStyle.top = scrollerLuft + "px";

    scroller.onmousemove = function () { if (!dragging) { scrollerRelMouseY = event.clientY - parseInt(doc.getElementById('scroller').style.top, 10); } };
    scroller.onmousedown = function () { dragging = true; doc.onmousemove = dragNscroll; document.onselectstart = function () { return false } };

    updateScrollerPos();
}

//document.addEventListener("animationstart", refreshScroller, false);

window.onmouseup = function () { document.onmousemove = null; dragging = false; document.onselectstart = null; };