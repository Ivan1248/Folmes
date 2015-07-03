document.documentElement.onclick = function (e) {
    e = window.event ? event.srcElement : e.target;
    if (e.className && e.className.indexOf('item') != -1) {
        if (e.style.backgroundColor !== 'rgba(255, 0, 0, 0.1)') {
            e.style.backgroundColor = 'rgba(255, 0, 0, 0.1)';
        } else { e.style.backgroundColor = 'transparent'; }
    }
};
document.documentElement.onmouseleave = function () {
    var elements = document.getElementsByClassName('item'),
        reds = "",
        i,
        style;
    for (i = 0; i < elements.length; i++) {
        style = window.getComputedStyle(elements[i], null);
        if (style.getPropertyValue('background-color') === 'rgba(255, 0, 0, 0.1)') {
            reds += elements[i].childNodes[1].textContent + '|';
        }
    }
    document.body.setAttribute('data-sel', reds);
};