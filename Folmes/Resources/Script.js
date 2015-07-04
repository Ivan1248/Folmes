function getSelectedText() {
    var text = "";
    if (window.getSelection) {
        text = window.getSelection().toString();
    } else if (document.selection && document.selection.type != "Control") {
        text = document.selection.createRange().text;
    }
    return text;
}

function clickO(inp) {
    var body = document.body;
    body.setAttribute("data-click", inp);
    setTimeout(function () {body.setAttribute("data-click", ""); }, 100); //ako se poslije klika u prazno
}

document.onmouseup = function () { document.body.setAttribute("data-sel", getSelectedText()); };

function removeFirst() {
    document.getElementById("container").firstChild.removeNode(true);
}