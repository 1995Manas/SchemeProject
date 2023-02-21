var Business, Business_value, Business_text, Scheme, Scheme_value, Scheme_text, Channel, Channel_value, Channel_text
function downloadExcel() {
    debugger;
    Business = document.getElementById("Business");// find Selected Business 
    Business_value = Business.value;
    Business_text = Business.options[Business.selectedIndex].text;

    Scheme = document.getElementById("Scheme");//Find Selected Scheme Type
    Scheme_value = Scheme.value;
    Scheme_text = Scheme.options[Scheme.selectedIndex].text;

    Channel = document.getElementById("Channel");//Find Selected Scheme Type
    Channel_value = Channel.value;
    Channel_text = Channel.options[Channel.selectedIndex].text;

    if (Business_text == "B2B" && Scheme_text == "Value" && Channel_text == "Customer") {
        const link = document.createElement('a');
        link.setAttribute('target', '_blank');
        link.setAttribute('href', '/download/B2B_Value_Customer.xls');
        link.setAttribute('download', `B2B_Value_Customer.xls`);
        document.body.appendChild(link);
        link.click();
    }
    else if (Business_text == "B2C" && Scheme_text == "Value" && Channel_text == "Distributor/Dealer") {
        const link = document.createElement('a');
        link.setAttribute('target', '_blank');
        link.setAttribute('href', '/download/B2C_Value_Distributor_or_DirectDealer.xls');
        link.setAttribute('download', `B2C_Value_Distributor_or_DirectDealer.xls`);
        document.body.appendChild(link);
        link.click();
    }
    else if (Business_text == "B2C" && Scheme_text == "Value" && Channel_text == "Dealer/Indirect Dealer") {
        const link = document.createElement('a');
        link.setAttribute('target', '_blank');
        link.setAttribute('href', '/download/B2C_Value_Dealer_or_IndirectDealer.xls');
        link.setAttribute('download', `B2C_Value_Dealer_or_IndirectDealer.xls`);
        document.body.appendChild(link);
        link.click();
    }
    else if (Business_text == "B2C" && Scheme_text == "Volume" && Channel_text == "Distributor/Dealer") {
        const link = document.createElement('a');
        link.setAttribute('target', '_blank');
        link.setAttribute('href', '/download/B2C_Volume_Distributor_or_DirectDealer.xls');
        link.setAttribute('download', `B2C_Volume_Distributor_or_DirectDealer.xls`);
        document.body.appendChild(link);
        link.click();
    }
    else if (Business_text == "B2C" && Scheme_text == "Volume" && Channel_text == "Dealer/Indirect Dealer") {
        const link = document.createElement('a');
        link.setAttribute('target', '_blank');
        link.setAttribute('href', '/download/B2C_Volume_Dealer_or_IndirectDealer.xls');
        link.setAttribute('download', `B2C_Volume_Dealer_or_IndirectDealer.xls`);
        document.body.appendChild(link);
        link.click();
    }
    
}