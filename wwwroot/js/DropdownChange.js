var Business, remove, add ,channelfilter
function Dropdown_change() {
    debugger;
    Business = document.getElementById('Business').value;
    Schemefilter = document.getElementById("Scheme");
    channelfilter = document.getElementById("Channel");
    channeloptions = document.getElementById("Channel").options;
    if (Business == 'string:B2B') {

        Schemefilter.value = 'string:Value';
        channeloptions[1].selected = true;
        Schemefilter.disabled = true;
        channelfilter.disabled = true;
        return true;
    }

    else if (Business == 'string:B2C') {

        channeloptions[1].hidden = true;
        Schemefilter.disabled = false;
        channelfilter.disabled = false;
        Schemefilter.selectedIndex = 0;
        channelfilter.selectedIndex = 0;
    }
    else {
        Schemefilter.enable = true;
        channelfilter.enable = true;
        channeloptions[1].hidden = false;
    }
}