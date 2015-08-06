function getSelectedText() {
    var text = "";
    if (window.getSelection) {
        text = window.getSelection().toString();
    } else if (document.selection && document.selection.type != "Control") {
        text = document.selection.createRange().text;
    }
    return text;
}

var messageDisplay = window.external;

function linkClick(data) {
    window.external.ProcessStart(data);
}

function contMenu() {
    if (getSelectedText()) window.external.RaiseContextMenu();
}

window.oncontextmenu = contMenu;

// called from outside
function removeFirst() {
    document.getElementById("container").firstChild.removeNode(true);
}

