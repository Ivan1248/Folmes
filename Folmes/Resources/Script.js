function getSelectedText() {
    var text = "";
    if (window.getSelection) {
        text = window.getSelection().toString();
    } else if (document.selection && document.selection.type != "Control") {
        text = document.selection.createRange().text;
    }
    return text;
}

function fileClick(inp) {
    window.external.ProcessStart_Output(inp);
}

function contMenu() {
    if (getSelectedText()) window.external.ContextMenu_Output();
}

window.oncontextmenu = contMenu;

// called from outside
function removeFirst() {
    document.getElementById("container").firstChild.removeNode(true);
}

