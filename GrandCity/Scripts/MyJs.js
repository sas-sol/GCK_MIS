﻿// Global Variables
// ^(?:[\t ]*(?:\r?\n|\r))+
var dealercounter = 1;
var instcoutstrc = 1;
var paycont = 1;
var PhaseBlockData = "";
var filealloccounter = 1;
var instdata = [];
var layoutpresent = false;
var formcontrolToken = false;
var singleinst = 0;
var singleword = ['', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ', 'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '];
var tenwords = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];
var plotinstdata = [];
var nextown;
var cn = '';
var plotowncount = 1;
var closegate = false;
var InventoryCounter = 1;
var drcr = 1;
var InventoryArray = [];
var citylist = '<option value="">Choose..</option><option value="Abbottabad">Abbottabad</option><option value="Adezai">Adezai</option><option value="Ahmadpur East">Ahmadpur East</option>' +
    '<option value="Ahmed Nager Chatha">Ahmed Nager Chatha</option><option value="Alipur">Alipur</option><option value="Alpuri">Alpuri</option><option value="Arifwala">Arifwala</option>' +
    '<option value="Attock">Attock</option><option value="Ayubia">Ayubia</option><option value="Badin">Badin</option><option value="Bahawalnagar">Bahawalnagar</option>' +
    '<option value="Bahawalpur">Bahawalpur</option><option value="Banda Daud Shah">Banda Daud Shah</option><option value="Bannu">Bannu</option><option value="Batkhela">Batkhela</option>' +
    '<option value="Battagram">Battagram</option><option value="Bhakkar">Bhakkar</option><option value="Bhalwal">Bhalwal</option><option value="Birote">Birote</option>' +
    '<option value="Burewala">Burewala</option><option value="Chakdara">Chakdara</option><option value="Chakwal">Chakwal</option><option value="Chaman">Chaman</option>' +
    '<option value="Charsadda">Charsadda</option><option value="Chichawatni">Chichawatni</option><option value="Chillianwala">Chillianwala</option><option value="Chiniot">Chiniot</option>' +
    '<option value="Chishtian">Chishtian</option><option value="Chitral">Chitral</option><option value="Dadu">Dadu</option><option value="Daggar">Daggar</option>' +
    '<option value="Dargai">Dargai</option><option value="Darya Khan">Darya Khan</option><option value="Daska">Daska</option><option value="Dera Allah Yar">Dera Allah Yar</option>' +
    '<option value="Dera Ghazi Khan">Dera Ghazi Khan</option><option value="Dera Ismail Khan">Dera Ismail Khan</option><option value="Dera Murad Jamali">Dera Murad Jamali</option><option value="Dhaular">Dhaular</option>' +
    '<option value="Digri">Digri</option><option value="Dina">Dina</option><option value="Dinga">Dinga</option><option value="Dipalpur">Dipalpur</option>' +
    '<option value="Diplo">Diplo</option><option value="Dir">Dir</option><option value="Dokri">Dokri</option><option value="Drosh">Drosh</option>' +
    '<option value="Faisalabad">Faisalabad</option><option value="Fateh Jang">Fateh Jang</option><option value="Ghakhar Mandi">Ghakhar Mandi</option><option value="Ghotki">Ghotki</option>' +
    '<option value="Gilgit">Gilgit</option><option value="Gojra">Gojra</option><option value="Gujar Khan">Gujar Khan</option><option value="Gujranwala">Gujranwala</option>' +
    '<option value="Gujrat">Gujrat</option><option value="Gwadar">Gwadar</option><option value="Haala">Haala</option><option value="Hafizabad">Hafizabad</option>' +
    '<option value="Hangu">Hangu</option><option value="Haripur">Haripur</option><option value="Haroonabad">Haroonabad</option><option value="Hasilpur">Hasilpur</option>' +
    '<option value="Haveli Lakha">Haveli Lakha</option><option value="Hub">Hub</option><option value="Hyderabad">Hyderabad</option><option value="Islamabad">Islamabad</option>' +
    '<option value="Islamkot">Islamkot</option><option value="Jacobabad">Jacobabad</option><option value="Jalalpur Jattan">Jalalpur Jattan</option><option value="Jampur">Jampur</option>' +
    '<option value="Jamshoro">Jamshoro</option><option value="Jaranwala">Jaranwala</option><option value="Jauharabad">Jauharabad</option><option value="Jhang">Jhang</option>' +
    '<option value="Jhelum">Jhelum</option><option value="Jungshahi">Jungshahi</option><option value="Kalabagh">Kalabagh</option><option value="Kalat">Kalat</option>' +
    '<option value="Kamalia">Kamalia</option><option value="Kamoke">Kamoke</option><option value="Kandiaro">Kandiaro</option><option value="Karachi">Karachi</option>' +
    '<option value="Karak">Karak</option><option value="Karor Lal Esan">Karor Lal Esan</option><option value="Kashmore">Kashmore</option><option value="Kasur">Kasur</option>' +
    '<option value="Keti Bandar">Keti Bandar</option><option value="Khairpur">Khairpur</option><option value="Khanewal">Khanewal</option><option value="Khanpur">Khanpur</option>' +
    '<option value="Kharan">Kharan</option><option value="Kharian">Kharian</option><option value="Khushab">Khushab</option><option value="Khuzdar">Khuzdar</option>' +
    '<option value="Kohat">Kohat</option><option value="Kot Adu">Kot Adu</option><option value="Kotri">Kotri</option><option value="Lahore">Lahore</option>' +
    '<option value="Lakki Marwat">Lakki Marwat</option><option value="Lalamusa">Lalamusa</option><option value="Larkana">Larkana</option><option value="Latamber">Latamber</option>' +
    '<option value="Layyah">Layyah</option><option value="Liaquat Pur">Liaquat Pur</option><option value="Lodhran">Lodhran</option><option value="Loralai">Loralai</option>' +
    '<option value="Madyan">Madyan</option><option value="Mailsi">Mailsi</option><option value="Mamoori">Mamoori</option><option value="Mandi Bahauddin">Mandi Bahauddin</option>' +
    '<option value="Mansehra">Mansehra</option><option value="Mardan">Mardan</option><option value="Mastuj">Mastuj</option><option value="Mastung">Mastung</option>' +
    '<option value="Matiari">Matiari</option><option value="Mehar">Mehar</option><option value="Mehrabpur">Mehrabpur</option><option value="Mian Channu">Mian Channu</option>' +
    '<option value="Mianwali">Mianwali</option><option value="Mingora">Mingora</option><option value="Mirpur Khas">Mirpur Khas</option><option value="Mithani">Mithani</option>' +
    '<option value="Mithi">Mithi</option><option value="Moro">Moro</option><option value="Multan">Multan</option><option value="Muridke">Muridke</option>' +
    '<option value="Murree">Murree</option><option value="Muzaffarabad">Muzaffarabad</option><option value="Muzaffargarh">Muzaffargarh</option><option value="Nagarparkar">Nagarparkar</option>' +
    '<option value="Narowal">Narowal</option><option value="Naudero">Naudero</option><option value="Naushahro Feroze">Naushahro Feroze</option><option value="Naushara">Naushara</option>' +
    '<option value="Nawabshah">Nawabshah</option><option value="Nazimabad">Nazimabad</option><option value="Nowshera">Nowshera</option><option value="Nushki">Nushki</option>' +
    '<option value="Okara">Okara</option><option value="Paharpur">Paharpur</option><option value="Pakpattan">Pakpattan</option><option value="Pasni">Pasni</option>' +
    '<option value="Pattoki">Pattoki</option><option value="Peshawar">Peshawar</option><option value="Pir Mahal">Pir Mahal</option><option value="Qila Didar Singh">Qila Didar Singh</option>' +
    '<option value="Quetta">Quetta</option><option value="Rabwah">Rabwah</option><option value="Rahim Yar Khan">Rahim Yar Khan</option><option value="Raiwind">Raiwind</option>' +
    '<option value="Rajanpur">Rajanpur</option><option value="Rajo Khanani">Rajo Khanani</option><option value="Ranipur">Ranipur</option><option value="Ratodero">Ratodero</option>' +
    '<option value="Rawalpindi">Rawalpindi</option><option value="Renala Khurd">Renala Khurd</option><option value="Rohri">Rohri</option><option value="Sadiqabad">Sadiqabad</option>' +
    '<option value="Safdarabad">Safdarabad</option><option value="Sahiwal">Sahiwal</option><option value="Saidu Sharif">Saidu Sharif</option><option value="Sakrand">Sakrand</option>' +
    '<option value="Sanghar">Sanghar</option><option value="Sangla Hill">Sangla Hill</option><option value="Sarai Alamgir">Sarai Alamgir</option><option value="Sargodha">Sargodha</option>' +
    '<option value="Shahbandar">Shahbandar</option><option value="Shahdadkot">Shahdadkot</option><option value="Shahdadpur">Shahdadpur</option><option value="Shahpur Chakar">Shahpur Chakar</option>' +
    '<option value="Shakargarh">Shakargarh</option><option value="Sheikhupura">Sheikhupura</option><option value="Shikarpaur">Shikarpaur</option><option value="Sialkot">Sialkot</option>' +
    '<option value="Sibi">Sibi</option><option value="Sohawa">Sohawa</option><option value="Soianwala">Soianwala</option><option value="Sukkur">Sukkur</option>' +
    '<option value="Swabi">Swabi</option><option value="Swat">Swat</option><option value="Talagang">Talagang</option><option value="Tando Adam Khan">Tando Adam Khan</option>' +
    '<option value="Tando Allahyar">Tando Allahyar</option><option value="Tangi">Tangi</option><option value="Tank">Tank</option><option value="Taxila">Taxila</option>' +
    '<option value="Thall">Thall</option><option value="Thatta">Thatta</option><option value="Timergara">Timergara</option><option value="Toba Tek Singh">Toba Tek Singh</option>' +
    '<option value="Tordher">Tordher</option><option value="Turbat">Turbat</option><option value="Umerkot">Umerkot</option><option value="Usta Mohammad">Usta Mohammad</option>' +
    '<option value="Vehari">Vehari</option><option value="Wah Cantonment">Wah Cantonment</option><option value="Warah">Warah</option><option value="Wazirabad">Wazirabad</option>' +
    '<option value="Zhob">Zhob</option><option value="Other">Other</option>';

var banklists = '<option value="">Choose..</option>' +
    '<option value="1">Dubai Islamic Bank - 0441619001</option>' +
    '<option value="2">Dubai Islamic Bank - 0441619002</option>' +
    '<option value="3">Habib Bank - 06447902018403</option>' +
    '<option value="4">JS Bank - 174334</option>' +
    '<option value="5">Faysal Bank - 0186007000000684</option>' +
    '<option value="6">Bank Alfalah - 0136-1005789121</option>' +
    '<option value="7">Dubai Islamic Bank - 0441619004</option>' +
    '<option value="8">Allied Bank - 0010031815300016</option>' +
    '<option value="9">Habib Bank - 0126-79040349-55</option>' +
    '<option value="10">Habib Bank - 00427991891003</option>' +
    '<option value="11">Bank Alfalah - 0136-1007542638</option>' +
    '<option value="12">Bank Alfalah - 0136-1007542962</option>' +
    '<option value="13">Bank Alfalah - 0136-1007543221</option>' +
    '<option value="14">Bank Alfalah - 0136-1007542829</option>' +
    '<option value="15">Bank Alfalah - 0682-1007937888</option>';

var unitlist = `<option value=""></option>
     <option value="Each">Each</option>
     <option value="Dumper">Dumper</option>
     <option value="Trackor Trolly">Trackor Trolly</option>
     <option value="Pack">Pack</option>
     <option value="Jaar">Jaar</option>
     <option value="No.">No.</option>
     <option value="Box">Box</option>
     <option value="Carton">Carton</option>
     <option value="Bundle">Bundle</option>
     <option value="Mound/Mann">Mound/Mann</option>
     <option value="Block">Block</option>
     <option value="Bag">Bag</option>
    <optgroup label="Electric">
        <option value="Volt">Volt</option>
        <option value="Amp">Amp</option>
        <option value="KW">KW</option>
        <option value="KVA">KVA</option>
        <option value="VA">VA</option>
        <option value="Watt">Watt</option>
        <option value="HP">HP</option>
    </optgroup>
<optgroup label="Cooling">
        <option value="BTU">BTU</option>
        <option value="TON">TON</option>
    </optgroup>
 <optgroup label="Volume">
        <option value="Gallon">Gallon</option>
        <option value="Quarter">Quarter</option>
        <option value="Ounce">Ounce</option>
        <option value="Cubic Metre">Cubic Metre</option>
        <option value="Litre">Litre</option>
        <option value="Millilitre">Millilitre</option>
        <option value="Cubic Foot">Cubic Foot</option>
        <option value="Cubic Inch">Cubic Inch</option>
    </optgroup>
    <optgroup label="Weight">
        <option value="Tonne">Tonne</option>
        <option value="Kilogram">Kilogram</option>
        <option value="Gram">Gram</option>
        <option value="Milligram">Milligram</option>
        <option value="Pound">Pound</option>
    </optgroup>
    <optgroup label="Length">
        <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option>
    </optgroup>`;
var unitlistg =
    `<option value=""></option>
     <option value="Each">Each</option>
     <option value="Dumper">Dumper</option>
     <option value="Trackor Trolly">Trackor Trolly</option>
     <option value="Pack">Pack</option>
     <option value="Jaar">Jaar</option>
     <option value="No.">No.</option>
     <option value="Box">Box</option>
     <option value="Carton">Carton</option>
     <option value="Bundle">Bundle</option>
     <option value="Mound/Mann">Mound/Mann</option>
     <option value="Block">Block</option>
     <option value="Bag">Bag</option>
 <optgroup label="Electric">
        <option value="Volt">Volt</option>
        <option value="Amp">Amp</option>
        <option value="KW">KW</option>
        <option value="KVA">KVA</option>
        <option value="VA">VA</option>
        <option value="Watt">Watt</option>
        <option value="HP">HP</option>
    </optgroup>
<optgroup label="Cooling">
        <option value="BTU">BTU</option>
        <option value="TON">TON</option>
    </optgroup>
    <optgroup label="Volume">
        <option value="Gallon">Gallon</option>
        <option value="Quarter">Quarter</option>
        <option value="Ounce">Ounce</option>
        <option value="Cubic Metre">Cubic Metre</option>
        <option value="Litre">Litre</option>
        <option value="Millilitre">Millilitre</option>
        <option value="Cubic Foot">Cubic Foot</option>
        <option value="Cubic Inch">Cubic Inch</option>
    </optgroup>
    <optgroup label="Weight">
        <option value="Tonne">Tonne</option>
        <option value="Kilogram">Kilogram</option>
        <option value="Gram">Gram</option>
        <option value="Milligram">Milligram</option>
        <option value="Pound">Pound</option>
    </optgroup>
    <optgroup label="Length">
        <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option>
    </optgroup>`;
// Generic functions
// Convert to float
function Float(val) {
    return parseFloat(val).toFixed(2);
}
// Convert to int
function Int(val) {
    return parseInt(val | 0);
}
// Calculate the remaining amount from total price
function RemainCal(Total, Curentamt) {
    return Float(Total) - Float(Curentamt);
}
// Calculate the Development Charges per marla
function DevChargesCal(rate, plotsize) {
    return Float(rate) * Float(plotsize);
}
// Calculate Percentages 
function CalPercentage(percent, figure) {
    return (Float(percent) * Float(figure)) / 100;
}
// divide function
function Divide(figure, divider) {
    return Float(figure) / Float(divider);
}
// Sum Numbers
function Sum(a, b) {
    a = Float(a);
    b = Float(b);
    return a + b;
}
// write amount in words
function InWords(num) {
    if ((num = num.toString()).length > 9) return 'overflow';
    n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return; var str = '';
    str += (n[1] != 0) ? (singleword[Number(n[1])] || tenwords[n[1][0]] + ' ' + singleword[n[1][1]]) + 'Crore ' : '';
    str += (n[2] != 0) ? (singleword[Number(n[2])] || tenwords[n[2][0]] + ' ' + singleword[n[2][1]]) + 'Lac ' : '';
    str += (n[3] != 0) ? (singleword[Number(n[3])] || tenwords[n[3][0]] + ' ' + singleword[n[3][1]]) + 'Thousand ' : '';
    str += (n[4] != 0) ? (singleword[Number(n[4])] || tenwords[n[4][0]] + ' ' + singleword[n[4][1]]) + 'Hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (singleword[Number(n[5])] || tenwords[n[5][0]] + ' ' + singleword[n[5][1]]) + 'Only ' : '';
    return str;
}
// Convert Amount in digit
function RemoveComma(e) {
    e = (e == "" || e == undefined) ? "" : e;
    e = e.replace('Rs.', '');
    e = e.replace('Rs', '');
    e = e.trim();
    return e.replace(/,/g, '');
}
// if string is empty ""
function isEmpty(str) {
    return (!str || 0 === str.length);
}
// to check if there is white space " "
function isBlank(str) {
    return (!str || /^\s*$/.test(str));
}
// to Add commas while typing number 
$(document).on("keyup", ".coma", function () {
    var num = this.value;
    var b = RemoveComma(num);
    if (isNaN(b)) {
        return false;
    }
    var a = format(b);
    this.value = a;
    //this.value = this.value.replace(/\D/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    $(this).next().val(RemoveComma(this.value));
});
//
function format(num) {
    return ('' + num).replace(
        /(\d)(?=(?:\d{3})+(?:\.|$))|(\.\d\d?)\d*$/g,
        function (m, s1, s2) {
            return s2 || (s1 + ',');
        }
    );
}
// Marla and Kanal Conversion
function MarlatoKanal(e) {
    var pltsize = e.split(' ')[0];
    if (pltsize / 20 >= 1) {
        pltsize = pltsize / 20 + ' Kanal';
    }
    else {
        pltsize = e;
    }
    return pltsize;
}
//Add coma in amount
function ComaAmount(val) {
    val = (val == null) ? "0" : val.toString()
    return val.replace(/\D/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}
// Adjust tables 
function Adjusttable() {
    // Change the selector if needed
    var $table = $('table.scroll'),
        $bodyCells = $table.find('tbody tr:first').children(),
        colWidth;
    // Adjust the width of thead cells when window resizes
    $(window).resize(function () {
        // Get the tbody columns width array
        colWidth = $bodyCells.map(function () {
            return $(this).width();
        }).get();
        // Set the width of thead columns
        $table.find('thead tr').children().each(function (i, v) {
            $(v).width(colWidth[i]);
        });
    }).resize(); // Trigger resize handler
}
//
function ChatBottom() {
    var d = $('#chat-msg');
    d.scrollTop(d.prop("scrollHeight"));
}
//
function convert() {
    var codeBag = document.querySelector('#result');
    var val = htmlToPdfmake(codeBag.outerHTML);
    //document.querySelector('#result').value = JSON.stringify(val, null, '  ');
    var dd = {
        defaultStyle: {
            fontSize: 10
        },
        pageOrientation: 'landscape',
        content: val
    };
    //pdfMake.createPdf(dd).getDataUrl(function (outDoc) {
    //    document.getElementById('pdf').src = outDoc;
    //});
    pdfMake.createPdf(dd).open();
}
//function Sum(a, b, c) {
//    return Float(a) + Float(b) + Float(c);
//}
//function Sum(a, b, c, d) {
//    return Float(a) + Float(b) + Float(c) + Float(d);
//}
//function Sum(a, b, c, d, e) {
//    return Float(a) + Float(b) + Float(c) + Float(d) + Float(e);
//}
function EmptyModel() {
    $('#modalbody').html(" ");
    $('.modal-footer').empty();
    $('.modal-footer').append('<button type="button" class="btn btn-secondary modal-close-btn" data-dismiss="modal">Close</button>');
}
function resetsr() {
    var sr = 1;
    $('.sr').each(function () {
        $(this).text(sr);
        sr++;
    });
}
//Custome functions
$(document).on("click", ".dash-tile", function () {
    var tile = $(this).data("dash");
    $("#mainContent").load("/Home/DashTiles/", { Type: tile });
});
//Custome functions
$(document).on("click", ".staff-dash-tile", function () {
    var tile = $(this).data("dash");
    $("#mainContent").load("/Home/StaffDashTiles/", { Type: tile });
});
//Custome functions
$(document).on("click", ".ceo-dash-tile", function () {
    var tile = $(this).data("dash");
    $("#mainContent").load("/Home/CEODashTiles/", { Type: tile });
});
// create a role
$(document).on("submit", "#r-c", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#r-c").attr('action'),
        data: $("#r-c").serialize(),
        success: function (data) {
            if (data) {
                //alert("Role added");
                Swal.fire({
                    icon: 'success',
                    text: "Role added successfully"
                });
            }
            else {
                //alert("This Role is already Exist");
                Swal.fire({
                    icon: 'info',
                    text: "The role already exists"
                });
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            }
        }
    });
});
//assign a role to user
$(document).on("submit", "#a-r", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#a-r").attr('action'),
        data: $("#a-r").serialize(),
        success: function (data) {
            if (data) {
                //alert("Role Assigned");
                Swal.fire({
                    icon: 'success',
                    text: "Role assigned successfully"
                });
            }
            else {
                //alert("This Role is already Assigned to this User");
                Swal.fire({
                    icon: 'info',
                    text: "This role is already assigned to the User"
                });
            }
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            }
        }
    });
});
//assign a MIS to Employee
$(document).on("submit", "#a-mis", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#a-mis").attr('action'),
        data: $("#a-mis").serialize(),
        success: function (data) {
            alert("MIS Assigned");
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//Create a new responsibility
$(document).on("submit", "#res-c", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#res-c").attr('action'),
        data: $("#res-c").serialize(),
        success: function (data) {
            if (data) {
                alert("Responsibility Created");
                $("#res-lst").load("/RolesAndResponsibilities/AllResponsibilities/");
            }
            else {
                alert("This Responsibility is already Created");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// get all the roles responsibilites
$(document).on("change", ".g-role-respon", function (e) {
    e.preventDefault();
    var roleid = $("#Roles").val();
    $.ajax({
        type: "POST",
        url: '/RolesAndResponsibilities/GetResponsibilitiesOfRole/',
        data: { id: roleid },
        success: function (data) {
            var rolestring = $("#Roles option:selected").text();
            $('#sel-role').html(rolestring);
            if (data.Sel.length <= 0) {
                $('.sel-res').text("No Responsibility is Assigned Yet");
            }
            else {
                $(data.Sel).each(function (i) {
                    var html = '<div id="' + data.NonSel[i].Id + '"><div class="form-group" >' +
                        '<div class="checkbox checkbox-circle checkbox-info peers ai-c">' +
                        '<input type="checkbox" value="' + data.Sel[i].Id + '" name="" class="peer respons">' +
                        '<label class="peers peer-greed js-sb ai-c">' +
                        '<span class="peer peer-greed" style="font-weight:bold">' + data.Sel[i].Responsibility_Name + '</span>' +
                        '</label>' +
                        '</div><span class="peer peer-greed">' + data.Sel[i].Details + '</span></div></div>';
                    $('.sel-res').append(html);
                });
            }
            if (data.NonSel.length <= 0) {
                $('.nonsel-res').text("All Responsibilities are Assigned");
            }
            else {
                $(data.NonSel).each(function (i) {
                    var html = '<div id="' + data.NonSel[i].Id + '"><div class="form-group">' +
                        '<div class="checkbox checkbox-circle checkbox-info peers ai-c">' +
                        '<input type="checkbox" value="' + data.NonSel[i].Id + '" name="" class="peer respons">' +
                        '<label class="peers peer-greed js-sb ai-c">' +
                        '<span class="peer peer-greed" style="font-weight:bold">' + data.NonSel[i].Responsibility_Name + '</span>' +
                        '</label>' +
                        '</div><span class="peer peer-greed">' + data.NonSel[i].Details + '</span></div></div>';
                    $('.nonsel-res').append(html);
                });
            }
        },
        error: function (data) {
            alert("An Error occured Try Again");
        }
    });
});
// assign responsibilities to role
$(document).on("submit", "#role-res-ass", function (e) {
    e.preventDefault();
    var roleid = $("#Roles").val();
    var resArray = [];
    $(".respons:checked").each(function () {
        resArray.push($(this).val());
    });
    var res = { Roleid: roleid, Responsibilities: resArray };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: $("#role-res-ass").attr('action'),
        data: JSON.stringify(res),
        success: function () {
            for (var i = 0; i < resArray.length; i++) {
                var id = resArray[i];
                var html = $('#' + id).html();
                $('.nonsel-res #' + id).remove();
                $('.sel-res').prepend('<div id="' + id + '">' + html + '</div>');
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// update assign responsibilities to role
$(document).on("submit", "#up-role-res-ass", function (e) {
    e.preventDefault();
    var roleid = $("#Roles").val();
    var resArray = [];
    $(".respons:checked").each(function () {
        resArray.push($(this).val());
    });
    var data = { Roleid: roleid, Responsibilities: resArray };
    $.ajax({
        type: "POST",
        url: $("#role-res-ass").attr('action'),
        data: JSON.stringify(data),
        success: function (data) {
            if (data) {
                alert("Responsibility Created");
            }
            else {
                alert("This Responsibility is already Created");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Content loader in Divs
$(document).on("click", ".get-link", function () {
    var url = $(this).data("link");
    //$('#mainContent').load(url);
    window.location = url;
});
// Content loader in Divs
function LoadSideContent(url) {
    $('#sideContent').load(url);
}
// Load a new Page
function LoadPage(url) {
    window.location.href(url);
}
// Register a User
$(document).on("submit", "#r-u", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#r-u").attr('action'),
        data: $("#r-u").serialize(),
        success: function (data) {
            if (data.Succeeded) {
                alert("User Registered");
            }
            else {
                $.each(data.Errors, function (index, value) {
                    $(".RegisterResult").append("<li>" + value + "</li>");
                });
                alert('Unable to register at the moment.');
                focus(".RegisterResult")
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Select Label for Payment Type
$(document).on("change", ".paytypesel", function () {
    var res = $(this).val();
    var id = "#" + $(this).parent().parent().attr("id");
    var text = $("option:selected", this).text();
    if (res !== "Cash" && res !== "Online") {
        $(id + " .paymentotherinfo").css("display", "block");
        $(id + " .paymentotherinfo :input").attr("disabled", false);
        $(id + " #paytypelabel").text(text + "No.");
        $(id + " #paymenttype").attr("placeholder", text);
    }
    else if (res == "Online") {
        $(" .paymentotherinfo").css("display", "block");
        $(" .paymentotherinfo :input").attr("disabled", false);
        $("#paytypelabel").text("Bank Acc No.");
        $(" #paymenttype").attr("placeholder", "Account No.");
    }
    else {
        $(id + " .paymentotherinfo").hide();
        $(id + " .paymentotherinfo :input").attr("disabled", true);
    }
});
// Select Label for Payment Type Fine
$(document).on("change", ".paytypesel-fine", function () {
    var res = $(this).val();
    var id = "#" + $(this).parent().parent().attr("id");
    var text = $("option:selected", this).text();
    if (res !== "Cash" && res !== "Online") {
        $(id + " .paymentotherinfo-fine").css("display", "block");
        $(id + " .paymentotherinfo-fine :input").attr("disabled", false);
        $(id + " #paytypelabel-fine").text(text + "No.");
        $(id + " #paymenttype-fine").attr("placeholder", text);
    }
    else if (res == "Online") {
        $(" .paymentotherinfo-fine").css("display", "block");
        $(" .paymentotherinfo-fine :input").attr("disabled", false);
        $("#paytypelabel-fine").text("Bank Acc No.");
        $(" #paymenttype-fine").attr("placeholder", "Account No.");
    }
    else {
        $(id + " .paymentotherinfo-fine").hide();
        $(id + " .paymentotherinfo-fine :input").attr("disabled", true);
    }
});
// plot prefrence information details
$(document).on("change", ".plotpreinfo", function (e) {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/PlotPrefrenceDetails/',
        data: { Id: id },
        success: function (data) {
            var text = data.Extra_Value + "% has to be given at the time of Balloting";
            $('#prefinfo').text(text);
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('click', '#day_Close', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: '/Banking/CashierDayClose/',
        success: function (data) {
            alert("Day Closed");
            var codeBag = document.querySelector('#result');
            var val = htmlToPdfmake(codeBag.outerHTML);
            var dd = {
                defaultStyle: {
                    fontSize: 10
                },
                pageOrientation: 'landscape',
                content: val
            };
            pdfMake.createPdf(dd).open();
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('click', '#day-close-sam', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: '/Banking/CashierSAMDayClose/',
        success: function (data) {
            alert("Day Closed");
            var codeBag = document.querySelector('#result');
            var val = htmlToPdfmake(codeBag.outerHTML);
            var dd = {
                defaultStyle: {
                    fontSize: 10
                },
                pageOrientation: 'landscape',
                content: val
            };
            pdfMake.createPdf(dd).open();
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Project Creation
//$(document).on("submit", "#p-c", function (e) {
//    debugger
//    e.preventDefault();
//    $.ajax({
//        type: "POST",
//        url: $("#p-c").attr('action'),
//        data: $("#p-c").serialize(),
//        success: function (data) {
//            if (data) {
//                alert("Project Has been Initiated");
//            }
//            else {
//                alert("This Project is already Initiated");
//            }
//        }
//        , error: function (xmlhttprequest, textstatus, message) {
//            if (textstatus === "timeout") {
//                alert("got timeout");
//            } else {
//                alert(textstatus);
//            }
//        }
//    });
//});
// Phase Creation
//$(document).on("submit", "#ph-c", function (e) {
//    e.preventDefault();
//    $.ajax({
//        type: "POST",
//        url: $("#ph-c").attr('action'),
//        data: $("#ph-c").serialize(),
//        success: function (data) {
//            if (data) {
//                alert("Phase has been created");
//            }
//            else {
//                alert("This Phase is already exists in this Project");
//            }
//        }
//        , error: function (xmlhttprequest, textstatus, message) {
//            if (textstatus === "timeout") {
//                alert("got timeout");
//            } else {
//                alert(textstatus);
//            }
//        }
//    });
//});
// Phase Creation
$(document).on("submit", "#com-c", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#com-c").attr('action'),
        data: $("#com-c").serialize(),
        success: function (data) {
            if (data) {
                alert("Commercial Floor has been created");
            }
            else {
                alert("This Commercial Floor is already exists in this Project");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", ".PP", function () {
    var shpid = $(this).attr('id');
    $("#com__details").load("/Commercial/CommercialInformationMapView/", { Commercial_Id: shpid });
});
// block Creation
//$(document).on("submit", "#bl-c", function (e) {
//    debugger
//    e.preventDefault();
//    $.ajax({
//        type: "POST",
//        url: $("#bl-c").attr('action'),
//        data: $("#bl-c").serialize(),
//        success: function (data) {
//            if (data) {
//                alert("Block has been created");
//            }
//            else {
//                alert("This Block is already exists in this Phase");
//            }
//        }
//        , error: function (xmlhttprequest, textstatus, message) {
//            if (textstatus === "timeout") {
//                alert("got timeout");
//            } else {
//                alert(textstatus);
//            }
//        }
//    });
//});
// Select Label for Payment Type
$(document).on("change", ".ph-list", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/RealEstateBlocks/GetBlocks/',
        data: { Id: id },
        success: function (data) {
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Add dealership owner template
$(document).on("click", ".add-owner", function () {
    dealercounter++;
    var html = '<div id="dealer-' + dealercounter + '" class="deal-id"><div class="layer w-100"><div class="peers fxw-nw jc-sb ai-c bgc-white"><div class="peers ai-c"><div class="peer"><h6 class="lh-1 mB-0 d-counter">' + dealercounter + '.</h6></div>' +
        '</div><div class="peers"><a href="javascript:void(0);" class="rm-dealer peer td-n c-grey-900 cH-blue-500 fsz-md mR-30" title=""><i class="ti-minus"></i></a></div>' +
        '</div></div><div class="row"><div class="col-md-8"><div class="form-row"><div class="form-group col-md-6"><label>Name</label><input type="text" class="form-control name" /></div>' +
        '<div class="form-group col-md-6"><label>Father Name</label><input type="text" class="form-control fname" /></div></div><div class="form-row"><div class="form-group col-md-10">' +
        '<label>CNIC / NICOP</label><input type="text" class="form-control cnic" /></div></div><div class="form-row"><div class="form-group col-md-6"><label>Mobile 1</label>' +
        '<input type="text" class="form-control mob1" /></div><div class="form-group col-md-6"><label>Mobile 2</label><input type="text" class="form-control mob2" /></div></div></div>' +
        '<div class="col-md-4 dealer-img"><div class="main-img-preview"><img class="thumbnail img-preview deal-img" src="/assets/static/images/def-img.png" width="170" height="170" title="Preview Logo">' +
        '</div><div class="input-group py-3 ml-4"><div class="fileUpload btn btn-danger fake-shadow"><span><i class="glyphicon glyphicon-upload"></i> Upload Logo</span>' +
        '<input name="logo" type="file" class="attachment_upload"></div></div></div></div></div>';
    $('#dealers').append('' + html);
});
// Remove a Dealer owner Template
$(document).on("click", ".rm-dealer", function () {
    var dealer = $(this).parent().parent().parent().parent().remove();
    dealercounter = 1;
    $('.d-counter').each(function () {
        $(this).text(dealercounter + '.');
        dealercounter++;
    });
    dealercounter = 1;
    $('.deal-id').each(function () {
        dealercounter++;
        $(this).attr('id', 'dealer-' + dealercounter);
    });
});
// Add Dealership with dealers list
$(document).on("submit", "#a-d", function (e) {
    debugger;
    e.preventDefault();
    var dealersdata = [];
    var dealership = $('#Dealership_Name').val();
    for (var i = 1; i <= dealercounter; i++) {
        var dealers = { Name: "", Father_Name: "", CNIC_NICOP: "", Mobile_1: "", Mobile_2: "", Image: "" };
        dealers.Name = $('#dealer-' + i + ' .name').val();
        dealers.Father_Name = $('#dealer-' + i + ' .fname').val();
        dealers.CNIC_NICOP = $('#dealer-' + i + ' .cnic').val();
        dealers.Mobile_1 = $('#dealer-' + i + ' .mob1').val();
        dealers.Mobile_2 = $('#dealer-' + i + ' .mob2').val();
        dealers.Image = $('#dealer-' + i + ' .deal-img').attr("src");
        dealersdata.push(dealers);
    }
    var data = { Dealership: dealership, Dealers: dealersdata };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: $("#a-d").attr('action'),
        data: JSON.stringify(data),
        success: function (data) {
            if (data.DealerData) {
                var html = '';
                $("#dealer-barcode").html("");
                if (data.ReturnData != "error") {
                    html += '<img width="170" height="170" src="' + data.ReturnData + '" id="base64Img" class="img-thumbnail">'
                    $("#dealer-barcode").append(html);
                    alert("Dealership Registered");
                }
            }
            else {
                alert("Some Problem Occured");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Get Blocks information with Phase of Projects
$(document).on("change", ".prj-ph-blk", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: "/RealEstateBlocks/GetBlocks/",
        data: { Id: id },
        success: function (data) {
            $('#blocks').html(' ');
            $('#blocks').append('<option>Select Block</option>');
            var $prevGroup, prevGroupName;
            $.each(data, function () {
                if (prevGroupName !== this.Group.Name) {
                    $prevGroup = $('<optgroup />').prop('label', this.Group.Name).appendTo('#blocks');
                }
                $("<option />").val(this.Value).text(this.Text).appendTo($prevGroup);
                prevGroupName = this.Group.Name;
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Get Blocks information with Phase of Projects
$(document).on("change", ".prj-com-flo", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: "/RealEstatePhases/GetCommercialFloor/",
        data: { Id: id },
        success: function (data) {
            $('#floor').html(' ');
            $('#floor').append('<option>Select Floor</option>');
            $.each(data, function (key, value) {
                $("#floor").append('<option value=' + value.Id + '>' + value.Floor + '</option>');
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Add Plot defination
$(document).on("submit", "#pl-cr", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#pl-cr").attr('action'),
        data: $("#pl-cr").serialize(),
        success: function (data) {
            if (data) {
                alert("Plot Defination Added");
            }
            else {
                alert("Same Defination Already Added");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Get Blocks information with Phase of Projects
$(document).on("change", ".per-num", function () {
    var text = $("option:selected", this).text();
    $("#perc-num").text(text);
});
// Getting Values from the stored json of installment types
$(document).on("change", ".ra-type", function () {
    var val = $(this).val();
    var res = $(this).parent().siblings('.ra-grp');
    if (val === "%") {
        $(res).find('.rate').attr('data-sym', "%");
    }
    else if (val === "/") {
        $(res).find('.rate').attr('data-sym', "/");
    }
    else if (val === "*") {
        $(res).find('.rate').attr('data-sym', "*");
    }
    else if (val === "+") {
        $(res).find('.rate').attr('data-sym', "+");
    }
    else if (val === "^") {
        $(res).find('.rate').attr('data-sym', "^");
    }
});
// Add installment type
$(document).on("click", ".add-inst-type", function () {
    instcoutstrc++;
    var html = '<div class="form-row in-st" id="inst-strc-' + instcoutstrc + '"><div class="form-group col-md-2"><input type="text" class="form-control inst-name" /></div>' +
        '<div class="form-group col-md-2"><select class="form-control inst-type"><option>Select Installment Type</option><option value = "3" >Advance</option><option value="1">Installments</option>' +
        '<option value="6">Confirmation</option><option value = "4" > Possession</option><option value="2">Development Charges</option><option value="5">After Time</option><option value="0">Other Charges</option></select></div><div class="form-group col-md-2">' +
        '<select class="form-control ra-type"><option>Select Calculation Type</option><option value="%">Percentage</option><option value="/">Remaining Figures</option>' +
        '<option value="^">Multiple</option><option value="+">Fixed Charges</option></select></div><div class="form-group col-md-2 ra-grp"><input type="text" class="form-control rate hide-rate" data-sym="" />' +
        '<input type="hidden" class="hid-rate"/></div><div class="form-group col-md-1 "><input type="text" class="form-control af-tim" placeholder="Months"/></div><div class="form-group col-md-1 "><input type="text" class="form-control interval" placeholder="Months" /></div ><div class="form-group col-md-1"><input type="text" class="form-control value" readonly/>' +
        '</div><div class="form-group col-md-1"><a href="javascript:void(0);" class="rm-ins-typ peer td-n c-grey-900 cH-blue-500 fsz-md mR-30" title=""><i class="ti-minus"></i></a></div></div>';
    $('.instal-str').append(html);
    // Removed from Calculation type <option value="*">Per Marla</option>
});
// Add commercial installment type
$(document).on("click", ".add-com-inst-type", function () {
    instcoutstrc++;
    var html = '<div class="form-row in-st" id="inst-strc-' + instcoutstrc + '"><div class="form-group col-md-3"><input type="text" class="form-control inst-name" /></div><div class="form-group col-md-2"><select class="form-control inst-type">' +
        '<option>Select Installment Type</option> <option value="2">Booking</option><option value="3">Confirmation</option><option value="1">Installments</option><option value="4">Possession</option><option value="5">Balloon Payments</option><option value="6"> Skipping Installments</option></select></div><div class="form-group col-md-1"><input type="text" class="form-control perc" />' +
        '</div><div class="form-group col-md-2"><input type="text" class="form-control ttl-inst" /></div><div class="form-group col-md-1"><input type="text" class="form-control af-tim" placeholder="Months" /></div>' +
        '<div class="form-group col-md-1"><a href="javascript:void(0);" class="rm-ins-typ peer td-n c-grey-900 cH-blue-500 fsz-md mR-30" title=""><i class="ti-minus"></i></a></div></div >';
    $('.instal-str').append(html);
});
// remove installment type
$(document).on("click", ".rm-ins-typ", function () {
    $(this).parent().parent().remove();
    instcoutstrc = 1;
    $('[data-insstruc]').each(function () {
        $(this).attr('data-insstruc', instcoutstrc);
        instcoutstrc++;
    });
    instcoutstrc = 1;
    $('.in-st').each(function () {
        instcoutstrc++;
        $(this).attr('id', 'inst-strc-' + instcoutstrc);
    });
});
// Calculate File / Plot Price Structure
$(document).on("click", ".cal-ins", function () {
    var p_s = $('#Plots option:selected').val();
    if (p_s === "") {
        alert("Select Plot Size");
        $("#Plots").focus();
        return false;
    }
    var price = Float(RemoveComma($("#f-p-pric").val()));
    if (price <= 0) {
        alert("Enter Plot Amount");
        return false;
    }
    var remainingAmount = price;
    // to calculate all percentages
    $(".rate").each(function (i) {
        var action = $(this).attr("data-sym");
        if (action === "%") {
            var rate = Float(RemoveComma($(this).val()));
            var valdiv = $(this).closest('.instal-str').find('.value');
            var perctamt = CalPercentage(rate, price);
            $(valdiv[i]).val(perctamt);
            remainingAmount = RemainCal(remainingAmount, perctamt);
        }
    });
    // To Add the fixed Values
    $(".rate").each(function (i) {
        var action = $(this).attr("data-sym");
        if (action === "+") {
            var valdiv = $(this).closest('.instal-str').find('.value');
            var rate = Float($(this).val());
            $(valdiv[i]).val(rate);
            remainingAmount = RemainCal(remainingAmount, rate);
        }
    });
    // To Calculate All Development charges or per marla rates
    $(".rate").each(function (i) {
        var action = $(this).attr("data-sym");
        if (action === "*") {
            var rate = Float($(this).val());
            var valdiv = $(this).closest('.instal-str').find('.value');
            var plotsize = Float($('#Plots option:selected').text());
            var perctamt = DevChargesCal(rate, plotsize);
            $(valdiv[i]).val(perctamt);
        }
    });
    $(".rate").each(function (i) {
        var action = $(this).attr("data-sym");
        if (action === "^") {
            var rate = Float($(this).val());
            var valdiv = $(this).closest('.instal-str').find('.value');
            var time = $(this).closest('.in-st').find('.af-tim').val();
            var perctamt = DevChargesCal(rate, time);
            $(valdiv[i]).val(perctamt);
            remainingAmount = RemainCal(remainingAmount, perctamt);
        }
    });
    // To Divide remaining Amounts on installments
    $(".rate").each(function (i) {
        var action = $(this).attr("data-sym");
        if (action === "/") {
            var rate = Float($(this).val());
            var valdiv = $(this).closest('.instal-str').find('.value');
            var inst = parseFloat(remainingAmount / rate).toFixed(3);
            $(this).val(rate + " x " + inst)
            var all = parseFloat(DevChargesCal(rate, inst)).toFixed(0);
            $(valdiv[i]).val(all);
            remainingAmount = RemainCal(remainingAmount, all);
        }
    });
    var grandtotal = 0;
    $('.value').each(function () {
        var z = parseInt(grandtotal);
        var y = parseInt($(this).val());
        grandtotal = z + y;
    });
    $('#gr-tlt').val(grandtotal);
    $("#remain").val(remainingAmount);
});
//Add value to hidden text box rate
$(document).on("keyup", ".hide-rate", function () {
    //debugger
    var val = $(this).val();
    $(this).next().val(val);
});
// Calculate the total plot rate 
$(document).on("keyup", "#rate-marla", function () {
    var val = Number(RemoveComma($(this).val()));
    var plt_size = $("#Plots").val();
    var pltval = val * plt_size;
    $("#f-p-pric").val(pltval.toLocaleString());
});
// Create File Installment Structure
$(document).on("submit", "#cr-inst-str", function (e) {
    debugger
    e.preventDefault();
    var totalprice = parseFloat(RemoveComma($("#gr-tlt").val()));
    var actcalprice = 0;
    $(".value").each(function () {
        actcalprice = parseFloat(actcalprice | 0) + parseFloat(RemoveComma($(this).val()) | 0);
    });
   
    var de;
    var plot = $('#Plots').val();
    var block = $("#blocks option:selected").val();
    var rate = Number(RemoveComma($("#rate-marla").val()));
    var total = Number(RemoveComma($("#f-p-pric").val()));
    var grandtotal = Number(RemoveComma($("#gr-tlt").val()));
    var remaining = Number(RemoveComma($("#remain").val()));
    debugger
    if (grandtotal == total && remaining == 0) {
        var inststrdata = [];
        for (var i = 1; i <= instcoutstrc; i++) {
            var installtypes = { Installment_Name: "", Installment_Type: "", Rate: "", Plot_Size: "", Amount: "", Block_Id: "", After: "", Interval: "" };
            installtypes.Installment_Name = $('#inst-strc-' + i + ' .inst-name').val();
            installtypes.Installment_Type = $('#inst-strc-' + i + ' .inst-type').val();
            debugger
            installtypes.Rate = $('#inst-strc-' + i + ' .hid-rate').val();
            installtypes.After = $('#inst-strc-' + i + ' .af-tim').val();
            debugger
            installtypes.Interval = $('#inst-strc-' + i + ' .interval').val();
            installtypes.Amount = $('#inst-strc-' + i + ' .value').val();
            inststrdata.push(installtypes);
        }
        var finaldata = { I_S: inststrdata, Plot: plot, Block: block, Rate: rate, Total: total, GrandTotal: grandtotal };

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: $("#cr-inst-str").attr('action'),
            data: JSON.stringify(finaldata),
            success: function (data) {
                if (data) {
                    alert("Installment Plan Created!");
                    window.location.reload();
                }
                else {
                    alert("Installment Structure of " + + " of Plot Size :" + +" is already Present");
                }
            },
            error: function () {
                alert("Error Occured Try Again")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
    else {
        alert("Sum of all Values are not equal to Total Price.")
    }
});
   
// Create Commercial Structure
$(document).on("submit", "#cr-com-inst-str", function (e) {
    e.preventDefault();
    var floor = $('#floor').val();
    if (floor == null || floor == "") {
        //alert("Select Project Floor");
        Swal.fire({
            icon: 'info',
            text: 'Choose the floor for the project'
        });
    }
    var inststrdata = [];
    for (var i = 1; i <= instcoutstrc; i++) {
        var installtypes = { Installment_Name: "", Type: "", Percentage: "", Total_Inst: "", After_Time: "", CommercialFloor_Id: "" };
        installtypes.Installment_Name = $('#inst-strc-' + i + ' .inst-name').val();
        installtypes.Type = $('#inst-strc-' + i + ' .inst-type').val();
        installtypes.Percentage = $('#inst-strc-' + i + ' .perc').val();
        installtypes.Total_Inst = $('#inst-strc-' + i + ' .ttl-inst').val();
        installtypes.After_Time = $('#inst-strc-' + i + ' .af-tim').val();
        installtypes.CommercialFloor_Id = floor;
        inststrdata.push(installtypes);
    }
    var finaldata = { I_S: inststrdata };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: $("#cr-com-inst-str").attr('action'),
        data: JSON.stringify(finaldata),
        success: function (data) {
            if (data) {
                //alert("Commercial Installment Structure Added");
                Swal.fire({
                    icon: 'success',
                    text: 'Commercial installment structure successfully added'
                }).then(() => {
                    window.location.reload();
                });
                
            }
            else {
                //alert("Installment Structure of this Floor is already Present");
                Swal.fire({
                    icon: 'info',
                    text: 'The installment structure for this floor already exists'
                })
            }
        },
        error: function () {
            //alert("Error Occured Try Again")
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
                })
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    text: 'Request timed out'
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            }
        }
    });
});
// Image previewer
function readURL(input) {
    var $this = $(input).closest('.dealer-img').find('.img-preview');
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $($this).attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
// on image change 
$(document).on("change", ".attachment_upload", function () {
    readURL(this);
});
//Add File form security Prices
$(document).on("submit", "#ad-sec", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-sec").attr('action'),
        data: $("#ad-sec").serialize(),
        success: function (data) {
            if (data) {
                alert("Security Price Added");
            }
            else {
                alert("This Price Already Exist in this Phase")
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Get Phases and Blocks of a Project
$(document).on("change", ".prj-phas", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/RealEstatePhases/GetPhasesAndBlock/',
        data: { Id: id },
        success: function (data) {
            PhaseBlockData = data;
            var phasename = [];
            $("#phase").html("<option>Select Phase</option>")
            $.each(data, function (key, value) {
                var phase = phasename.find(function (e) { return e == value.Phase_Name });
                if (phase == null) {
                    $("#phase").append('<option value=' + value.Phase_Id + '>' + value.Phase_Name + '</option>');
                    phasename.push(value.Phase_Name);
                }
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Get Blocks list from global variable list 
$(document).on("change", "#phase", function () {
    var id = $(this).val();
    var blocks = PhaseBlockData.filter(x => x.Phase_Id == id);
    $("#block").html("<option>Select Block</option>")
    $.each(blocks, function (key, value) {
        $("#block").append('<option value=' + value.Id + '>' + value.Block_Name + '</option>');
    });
});

// Get Security Fees of Phase and Block
$(document).on("change", ".pha-blk", function () {
    var blockid = $("#block").val();
    var phaseid = $("#phase").val();
   /* var sectorid = $("#sector").val();  */
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileSecurity/',
        data: { Phase: phaseid, Block: blockid},
        success: function (data) {
            $("#sec-fee").html(data.Price)
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Add a new row in file allotment
$(document).on("click", ".ad-file-inf", function () {
    filealloccounter++;
    var html = `<div class="form-row fil-alloc" id="file-asoc-${filealloccounter}"><div class="form-group col-md-1"><select class="form-control plots"><option>Select Plot Size</option></select>
        </div><div class="form-group col-md-1"><input type="number" min="1" class="form-control total-file" /></div><div class="form-group col-md-2">
        <select class="form-control dev-char"><option value="">Select Development Charges</option><option value="true">With Development Charges</option><option value="false">Without Development Charges</option>
        </select></div>
        <div class="form-group col-md-2"><input type="text" class="form-control com" />
         </div>
        <div class="form-group col-md-3"><select class="form-control plt-ins"></select></div>
        <div class="form-group col-md-1" ><a href="javascript:void(0);" data-filaloc="${filealloccounter}" class="rm-file-as td-n c-grey-900 cH-blue-500 fsz-md mR-30" title=""><i class="ti-close"></i></a></div></div>`;
    $('#file-form').append(html);
    InitPlots(filealloccounter);
});
//
$(document).on("change", ".plots", function () {
    var pltmarla = $(this).val() + " Marla";
    var id = $(this).parent().parent().attr("id");
    InitPlotInstallmentPlan(pltmarla, id);
});
//
function InitPlotInstallmentPlan(pltgroup, id) {
    $("#" + id + " .plt-ins").empty();
    var $prevGroup, prevGroupName;
    var fdata = plotins.filter(x => x.Group.Name == pltgroup);
    alert(plotins.filter(x => x.Group.Name == pltgroup));
    $.each(fdata, function () {
        if (prevGroupName !== this.Group.Name) {
            $prevGroup = $('<optgroup />').prop('label', this.Group.Name).appendTo("#" + id + " .plt-ins");
        }
        $("<option />").val(this.Value).text(this.Text).appendTo($prevGroup);
        prevGroupName = this.Group.Name;
    });
}
// Remove a row from file allocation to dealers
$(document).on("click", ".rm-file-as", function () {
    $(this).parent().parent().remove();
    filealloccounter = 2;
    $('[data-filaloc]').each(function () {
        $(this).attr('data-filaloc', filealloccounter);
        filealloccounter++;
    });
    filealloccounter = 1;
    $('.fil-alloc').each(function () {
        filealloccounter++;
        $(this).attr('id', 'file-asoc-' + filealloccounter);
    });
});
// Initialize Plots sizes list
function InitPlots(i) {
    $.each(unipllotdata, function (key, value) {
        $("#file-asoc-" + i + " .plots").append('<option value="' + value.Id + '">' + value.Text + '</option>');
    });
}
// Add the file forms 
$(document).on("submit", "#de-fo-as", function (e) {
    e.preventDefault();
    if (formcontrolToken) {
        return false;
    }
    else {
        var totalcount = 0;
        $(".total-file").each(function () {
            totalcount = totalcount + parseInt($(this).val());
        });
        //if (totalcount > 50) {
        //    alert("Please Enter Total MAX 50 Files");
        //    return false;
        //}
        formcontrolToken = true;
        var filesformdata = [];
        for (var i = 1; i <= filealloccounter; i++) {
            var fillalc = { Phase: "", Block: "", Dealership_Name: "", Dealership_Id: "", Plot_Size: "", Filecount: "", Dev_Charges_Id: "", Dev_Charges_Text: "", Sec_NoSec_Id: "", Security: "", Installment_Plan: "" };
            fillalc.Phase = $('#phase').val();
            fillalc.Phase_Name = $('#phase option:selected').text();
            fillalc.Block = $('#block').val();
            fillalc.Block_Name = $('#block option:selected').text();
            fillalc.Type = $('#type option:selected').val();
            fillalc.Dealership_Id = $('#Dealership').val();
            fillalc.Dealership_Name = $('#Dealership option:selected').text();
            fillalc.Plot_Size = $('#file-asoc-' + i + ' .plots').val();
            fillalc.Commession = $('#file-asoc-' + i + ' .com').val();
            fillalc.Filecount = $('#file-asoc-' + i + ' .total-file').val();
            fillalc.Dev_Charges_Id = $('#file-asoc-' + i + ' .dev-char option:selected').val();
            fillalc.Dev_Charges_Text = $('#file-asoc-' + i + ' .dev-char option:selected').text();
            fillalc.Sec_NoSec_Id = $('#file-asoc-' + i + ' .sec-cha').val();
            fillalc.Sec_NoSec_Name = $('#file-asoc-' + i + ' .sec-cha option:selected').text();
            fillalc.Security = $('#file-asoc-' + i + ' .sec').val();
            fillalc.Installment_Plan = $('#file-asoc-' + i + ' .plt-ins').val();
            filesformdata.push(fillalc);
        }
        $("#del-sub-btn").attr("disabled", true);
        $("#reset").show();
        if (Installment_Plan == null) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: $("#de-fo-as").attr('action'),
                data: JSON.stringify(filesformdata),
                success: function (data) {
                    $('#re-fi-for').load('/FileSystem/ShowFileFormList/', { dealerFileForm: data })
                    window.open("/Dealership/NewFileDesign?Group_Id=" + data[0].Group_Id, '_blank');
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        alert("got timeout");
                    } else {
                        alert(textstatus);
                    }
                }
            });
        }
        else {
            alert("Select Installment Plan.");
        }
    }
});
// Get Application Form Details
$(document).on("click", "#sea-file", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetAppFileFormDetails/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                //alert("No Result found");
                Swal.fire({
                    icon: 'info',
                    text: "No Result Found."
                });
            }
            else {
                var dev = "";
                if (data.Development_Charges_Val == true) { dev = "With Development Charges"; } else if (data.Development_Charges_Val == false) { dev = "Non Development Charges"; }
                $('#app-fil-id').val(data.Id);
                $('#deal-nam').val(data.Dealership_Name);
                $('#pl-size').val(data.Plot_Size);
                $('#blk_Id').val(data.Block_Id);
                $('#blk').val(data.Block_Name);
                $('#type').val(data.Type);
                $('#status').text(data.Status);
                $('#dev-char').text(dev);
                $('#dev-char-st').val(data.Development_Charges_Val);
                var num = $("#app-num").val();
                $('#qr_img').attr("src", '/QR Codes/' + num + '.png');
                $('#qr_Id').val(data.QR_Code_Id);
                $('#adv-pmt').text(Number(data.Advance).toLocaleString());
                $('#adv').val(data.Advance);
                if (data.Discount > 0) {
                    $('#dis_amt').text(Number(data.Discount).toLocaleString());
                    $('#dis_div').show();
                }
                else {
                    $('#dis_div').hide();
                }
                $('#total').val(data.Total_Price);
                if (data.Status == "Already Received") {
                    $("#reg-file").hide();
                    $("#status").css("color", "green");
                    //alert("This File is Already Registered");
                    Swal.fire({
                        icon: 'info',
                        text: "This File is Already Registered."
                    });
                }
                else if (data.Status == "Registered") {
                    $("#reg-file").hide();
                    $("#status").css("color", "green");
                    //alert("This File is Already Registered");
                    Swal.fire({
                        icon: 'info',
                        //title: 'Success!',
                        text: "This File is Already Registered"
                    });
                }
                else if (data.Status == "Pending") {
                    $("#reg-file").show();
                    $("#status").css("color", "green");
                }
                else {
                    $("#reg-file").hide();
                    $("#status").css("color", "Red");
                    //alert("This File is Canceled");
                    Swal.fire({
                        icon: 'info',
                        text: "This File is Cancelled."
                    });
                }
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'info',
                    text: textstatus
                });
            }
        }
    });
});
// Get Application Form Details
$(document).on("click", "#sea-file-bid", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetAppFileFormDetails_Bid/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $('#app-fil-id').val(data.File.Id);
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#blk').val(data.File.Block_Name);
                $('#status').text(data.File.Status);
                if (data.File.Status != "Open") {
                    $("#reg-file").remove();
                }
                $('#dev-char').text(data.File.Development_Charges);
                $('#qr_img').attr("src", '/QR Codes/' + val + '.png');
                var totalamt = 0;
                $.each(data.ReceivedAmounts, function (i) {
                    var html = '<tr id="' + data.ReceivedAmounts[i].Id + '" data-token="' + data.ReceivedAmounts[i].TokenParameter + '" >' +
                        '<td scope="row">' + data.ReceivedAmounts[i].ReceiptNo + '</td>' +
                        '<td>' + Number(data.ReceivedAmounts[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.ReceivedAmounts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].PaymentType + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                    totalamt = totalamt + parseFloat(data.ReceivedAmounts[i].Amount);
                });
                var html1 = '<tr><td>Total</td><td colspan="4">' + totalamt + '</td></tr>';
                $("#rec-amts tbody").append(html1);
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Register  a file
$(document).on("submit", "#re-fil", function (e) {
    e.preventDefault();
    var advamt = parseFloat(RemoveComma($("#adv-pmt").text()));
    var totalamt = 0
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    if (advamt != totalamt) {
        alert("Received Amount is not equal to Booking Amount");
        return false;
    }
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", FileImage: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.FileImage = $("#pay-" + i + " #scanned").attr('src');
        allrecepts.push(recedata);
    }
    $("#reg-file").prop("disabled", true);
    var fileno = $("#app-num").val();
    var fulpaid = $("#full-paid").is(":checked");
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Phase_Id: $("#phs_Id").val(),
        Block_Id: $("#blk_Id").val(),
        QR_Code: $("#qr_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var alldata = {
        filedata: regfiledata,
        Flag: flag,
        DevCharStatus: $("#dev-char-st").val(),
        FileFormNumber: $("#app-num").val(),
        Receiptdata: allrecepts,
        FullPaid: fulpaid
    };
    var con = confirm("Are you sure you want to Register File");
    if (con) {
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("File No. " + fileno + " Has Been Registerd");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
                else if (data.Status == "2") {
                    alert("Wait Until You Payment is Clear from Bank");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                $("#reg-file").removeAttr("disabled");
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
// Register  a file
$(document).on("submit", "#bid-re-fil", function (e) {
    e.preventDefault();
    $("#reg-file").prop("disabled", true);
    var fileno = $("#app-num").val();
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Phase_Id: $("#phs_Id").val(),
        Block_Id: $("#blk_Id").val(),
        QR_Code: $("#qr_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var alldata = {
        filedata: regfiledata,
        FileFormNumber: $("#app-num").val(),
    };
    var con = confirm("Are you sure you want to Register File");
    if (con) {
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("File No. " + fileno + " Has Been Registerd");
                    window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".fil-disc", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Discount");
    var html = '<form><div class="row"><div class="col-md-2"><label>Total Amount</label><input class="form-control coma" id="dis-ttl-amt dis-cal-file" placeholder="12,345" required><input type="hidden" id="dis-ttl-amt-val"  name="Total_Amount" class="amt" required></div>' +
        '<div class="col-md-2"><label>Discount Amount <b><span id="percent"></span></b></label><input id="dis-amt" class="form-control coma dis-cal-file" placeholder="12,345" required><input type="hidden" id="dis-amt-val" name="Discount_Amount" class="amt" required></div>' +
        '<div class="col-md-3"><label>Discount Date</label><input id="dis-date" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" required></div>' +
        '<div class="col-md-5"><label>Remarks</label><textarea class="form-control" id="remrk" name="Remarks"></textarea></div></div></form>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-primary ad-fil-dis" type="button">Save</button>');
});
// Get all File Details
$(document).on("keyup", ".dis-cal-file", function () {
    var tot = $("#dis-ttl-amt-val").val() || 0;
    var amt = $("#dis-amt-val").val() || 0;
    var per = ((amt / tot) * 100).toFixed(2);
    $("#percent").text(per + "%");
});
//
// To Generate Other Amounts Receipts
$(document).on("click", ".ad-fil-dis", function (e) {
    e.preventDefault();
    var ttl = $("#dis-ttl-amt-val").val();
    var dis = $("#dis-amt-val").val();
    var rem = $("#remrk").val();
    var id = $("#file-id").val()
    var disDate = $('#dis-date').val();
    //var con = confirm("Are you sure you want to add Discount");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to add the Discount?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/FileSystem/FileDiscount/',
                data: { TotalAmt: ttl, DiscountAmt: dis, Remarks: rem, Id: id, DiscountDate: disDate },
                success: function (data) {
                    //alert("Discount Added")
                    Swal.fire({
                        icon: 'success',
                        text: 'Discount added successfully'
                    });
                },
                error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        $('#gen-rec').attr("disabled", false);
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        $('#gen-rec').attr("disabled", false);
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
// Get all File Details
$(document).on("click", "#sea-file-data", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileResult/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $("#rece-btn").attr("Disabled", false);
                var dev = "";
                if (data.File.Development_Charges_Val == true) { dev = "With Development Charges"; } else if (data.File.Development_Charges_Val == false) { dev = "Non Development Charges"; }
                if (data.File.Status != "Registered") {
                    alert("File is " + data.File.Status);
                    $("#rece-btn").attr("Disabled", true);
                }
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $("#rec-amts tbody").empty();
                $('#file-id').val(data.File.Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#qr_Id').val(data.QR_Code_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#prj').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#blk').val(data.File.Block_Name);
                $('#status').text(data.File.Status);
                $('#dev-char').text(dev);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                $('#Name').val(data.File.Name);
                $('#Father_Husband').val(data.File.Father_Husband);
                $('#Postal_Address').val(data.File.Postal_Address);
                $('#Residential_Address').val(data.File.Residential_Address);
                $('#City').val(data.File.City);
                $('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                $('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                $('#Occupation').val(data.File.Occupation);
                $('#Nationality').val(data.File.Nationality);
                $('#Email').val(data.File.Email);
                $('#Phone_Office').val(data.File.Phone_Office);
                $('#Residential').val(data.File.Residential);
                $('#Mobile_1').val(data.File.Mobile_1);
                $('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                var instserialno = 1;
                $.each(data.Installments, function (i) {
                    var statuscolor = "";
                    if (data.Installments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                    }
                    else if (data.Installments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.Installments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + data.Installments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.Installments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.Installments[i].Status + '</td></tr>';
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                var otherinstserialno = 1;
                $.each(data.OtherInstallments, function (i) {
                    var statuscolor = "", btn = '<button data-toggle="modal" data-target="#Modal" class="btn btn-info otherins">Receive</button></td>';
                    if (data.OtherInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                    }
                    else if (data.OtherInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                        btn = "";
                    }
                    var html = '<tr id="' + data.OtherInstallments[i].Id + '"  class=' + statuscolor + ' ><td scope="row">' + otherinstserialno + '</td>' +
                        '<td class="nam">' + data.OtherInstallments[i].Installment_Name + '</td>' +
                        '<td class="amt">' + Number(data.OtherInstallments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.OtherInstallments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.OtherInstallments[i].Status + '</td>' +
                        '<td>' + btn + '</tr>';
                    $("#oth-inst tbody").append(html);
                    otherinstserialno++;
                });
                var totalamt = 0;
                $.each(data.ReceivedAmounts, function (i) {
                    var stat = "";
                    totalamt = totalamt + parseFloat(data.ReceivedAmounts[i].Amount);
                    if (data.ReceivedAmounts[i].Status == null) {
                        data.ReceivedAmounts[i].Status = "-";
                    }
                    if (data.ReceivedAmounts[i].Ch_Pay_Draft_No == null) {
                        data.ReceivedAmounts[i].Ch_Pay_Draft_No = "-";
                    }
                    if (data.ReceivedAmounts[i].Status == "Pending") {
                        stat = "bgc-yellow-50";
                    }
                    else if (data.ReceivedAmounts[i].Status == "Dishonored") {
                        stat = "bgc-red-50";
                        totalamt = totalamt - parseFloat(data.ReceivedAmounts[i].Amount);
                    }
                    else if (data.ReceivedAmounts[i].Status == "Approved") {
                        stat = "bgc-green-50";
                    }
                    else if (data.ReceivedAmounts[i].Status == "Deposited") {
                        stat = "bgc-orange-50";
                    }
                    else {
                        stat = "";
                    }
                    var html = '<tr class="' + stat + '" id="' + data.ReceivedAmounts[i].Id + '" data-token="' + data.ReceivedAmounts[i].TokenParameter + '" >' +
                        '<td scope="row">' + data.ReceivedAmounts[i].ReceiptNo + '</td>' +
                        '<td>' + Number(data.ReceivedAmounts[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.ReceivedAmounts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].PaymentType + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].Ch_Pay_Draft_No + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].Status + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                });
                var html1 = '<tr><td>Total</td><td colspan="4">' + Number(totalamt).toLocaleString() + '</td></tr>';
                $("#rec-amts tbody").append(html1);
                $.each(data.Discounts, function (i) {
                    $(".dis").show();
                    var html = `<tr id="${data.Discounts[i].Id}" ><td> ${Number(data.Discounts[i].Total_Amount).toLocaleString()} </td>
                                <td> ${Number(data.Discounts[i].Discount_Amount).toLocaleString()} </td>
                                <td> ${data.Discounts[i].Percentage}% </td>
                                <td> ${data.Discounts[i].Remarks} </td>
                                <td><i class="fa fa-trash rem-dis"></i></td>
                                </tr>`;
                    $("#dis-amts tbody").append(html);
                });
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//pay installments for other Payments
$(document).on("click", ".otherins", function () {
    var a = $(this).closest('tr').attr("id");
    var name = $('#' + a + " .nam").text();
    var amt = RemoveComma($('#' + a + " .amt").text());
    EmptyModel();
    $('#ModalLabel').text("Installment");
    $('#modalbody').load('/Installments/OtherInstallments/', { Name: name, Amount: amt, Id: a });
});
$(document).on("click", ".addremin", function () {
    EmptyModel();
    $('#ModalLabel').text("Create New Reminder");
    $('#modalbody').load('/Reminders/NewReminder');
});
$(document).on("click", "#gen-deal-print", function () {
    let grp = $('.datagrpdeal').val();
    var dealer = $('.dealerselected :selected').val();
    window.open('/Dealership/DealerDealForm?Id=' + grp + '&Dealer=' + dealer, '_blank');
});
// To Generate Other Amounts Receipts
$(document).on("click", ".rem-dis", function (e) {
    var id = $(this).closest('tr').attr('id');
    var plt = $('#file-id').val();
    if (confirm("Are you sure you want to Remove Discount")) {
        $.ajax({
            type: "POST",
            url: '/FileSystem/RemoveDiscount/',
            data: { Id: id, Plot_Id: plt },
            success: function (data) {
                if (data.Status) {
                    $('#' + id).remove();
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
            }
        });
    }
});
// To Generate Other Amounts Receipts
$(document).on("click", "#gen-oth-rec", function (e) {
    e.preventDefault();
    var balamt = $("#bal-val").val();
    var amt = $("#amt").val();
    var totalamt = parseFloat(amt) + parseFloat(balamt);
    var actamt = parseFloat($("#act-amt").val());
    if (totalamt < actamt) {
        alert("Can not Submit Amount Less than Actual Amount");
        return false;
    }
    $('#gen-oth-rec').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#re-oth-ins").attr('action'),
            data: $("#re-oth-ins").serialize(),
            success: function (data) {
                window.open("/Banking/InstallmentReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    $('#gen-rec').attr("disabled", false);
                    alert("got timeout");
                } else {
                    $('#gen-rec').attr("disabled", false);
                    alert(textstatus);
                }
            }
        });
    }
});
// To generate Reciepts
$(document).on("click", "#rece-btn", function () {
    EmptyModel();
    $('#ModalLabel').text("Installment");
    $('#modalbody').load('/Installments/AddInstallment/');
});
// to receive Installment
$(document).on("click", "#ch-ins", function (e) {
    e.preventDefault();
    $('#gen-rec').attr("disabled", true);
    $('#gen-rec-test').attr("disabled", true);
    $.ajax({
        type: "POST",
        url: '/Installments/CheckInstallment/',
        data: $("#re-ins").serialize(),
        success: function (data) {
            $('#avail-inst tbody').empty();
            var inst = "";
            if (data.length <= 0) {
                $('#gen-rec').hide();
                $('#gen-rec').attr("disabled", true);
                $('#gen-rec-test').hide();
                $('#gen-rec-test').attr("disabled", true);
            }
            else {
                $(data).each(function (i) {
                    var html1 = '<input type="hidden" Name="Ids[]" value="' + data[i].Id + '" />';
                    $("#re-ins").append(html1);
                    var html = "<tr><td>" + data[i].Installment_Name + "</td><td>" + Number(data[i].Amount).toLocaleString() + "</td></tr>";
                    $('#avail-inst tbody').append(html);
                    inst = inst + data[i].Months + ",";
                });
                inst = inst + " Installments";
                $("#re-ins #paymentfor").val(inst);
                $('#gen-rec').show();
                $('#gen-rec').attr("disabled", false);
                $('#gen-rec-test').show();
                $('#gen-rec-test').attr("disabled", false);
            }
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
                $('#gen-rec').attr("disabled", false);
                $('#gen-rec-test').attr("disabled", false);
            } else {
                alert(textstatus);
                $('#gen-rec').attr("disabled", false);
                $('#gen-rec-test').attr("disabled", false);
            }
        }
    });
});
// Add Genrate Invoice
$(document).on("click", "#gen-rec", function (e) {
    e.preventDefault();
    $('#gen-rec').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var im = $("#scanned").attr('src');
    $("#imge").val(im);
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#re-ins").attr('action'),
            data: $("#re-ins").serialize(),
            success: function (data) {
                window.open("/Banking/InstallmentReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
            },
            error: function () {
                alert("Error Occured");
                $('#gen-rec').attr("disabled", false);
            }
        });
    }
});
// Load data in tab container
$(document).on("click", "[data-toggle='tab']", function (e) {
    var ids = $(this).data('lead');
    if (ids == "1") {
    }
    else if (ids == "2") {
    }
    else {
        var $this = $(this),
            targ = $this.attr('href'),
            loadurl = $this.attr('data-link');
        SASLoad(targ);
        $(targ).load(loadurl, function () {
            SASUnLoad(targ);
            $('#myTabs .active').removeClass("active");
            $this.addClass("active");
        });
    }
});
//
$(document).on("click", ".chddpo-link", function () {
    var url = $(this).data("link");
    $('#cheques').load(url);
});
//
$(document).on("click", ".lead-link-par", function () {
    var comp = $('#comp').val();
    var from = $('#from').val();
    var to = $('#to').val();
    var url = $(this).data("link");
    $('#cheques').load(url);
});
// Get Details of Cheque bankdraft or payorder details
$(document).on("click", ".cbp-details", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var data = id.split('`');
    $('.modal-body').load('/Banking/CBPDetails/', { No: data[0], Date: data[1], Bank: data[2], Status: data[3] }, function () { });
});
// Get Details of Cheque bankdraft or payorder details
$(document).on("click", ".cbp-details-pdc", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var data = id.split('`');
    $('.modal-body').load('/Banking/PDCCBPDetails/', { No: data[0], Date: data[1], Bank: data[2], Status: data[3] }, function () { });
});
// Get Leads Details of Cheque bankdraft or payorder details
$(document).on("click", ".ld-cbp-details", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var data = id.split('`');
    $('.modal-body').load('/Banking/LeadsCBPDetails/', { No: data[0], Date: data[1], Bank: data[2], Status: data[3] }, function () { });
});
// Search Instruments 
$(document).on("click", ".inst-search", function () {
    var type = $(this).data("type");
    var stat = $(this).data("stat");
    var from = $("#startdate").val();
    var to = $("#enddate").val();
    $('#cheques').load('/Banking/SearchInstruments/', { Type: type, Status: stat, From: from, To: to });
});
// Search Instruments 
$(document).on("click", ".pdc-inst-search", function () {
    var type = $(this).data("type");
    var stat = $(this).data("stat");
    var from = $("#startdate").val();
    var to = $("#enddate").val();
    $('#cheques').load('/Banking/PDCSearchInstruments/', { Type: type, Status: stat, From: from, To: to });
});
//
$(document).on("click", ".cbpimage", function () {
    debugger
    EmptyModel();
    var id = $(this).attr("id");
    var Plt_File_id = $(this).attr("name");
    var InstrumentName = id + '.jpg';
    //var data = id.split('`');
    //var pth = data[4] + data[0] + '_' + data[2] + '_' + data[1] + '.jpg';
    //var pp = data[0] + '_' + data[2] + '_' + data[1] + '.jpg';
    $("#modalbody").append("<img src='../../Repository/Instruments/Plots/" + Plt_File_id + "/" + InstrumentName + "' width='500px' height='500px' onerror=\"this.src='../../Repository/Instruments/Plots/defaultNotFound.png'\" >");
    
    //var fileExt = {};
});
// Get detials
$(document).on("click", "#up-cbp-btn", function (e) {
    e.preventDefault();
    if (confirm("Are you sure you want to update the Instrument Status")) {
        $.ajax({
            type: "POST",
            url: $("#up-cbp").attr('action'),
            data: $("#up-cbp").serialize(),
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
                alert("Error Occured Try Again");
            }
        });
    }
});
// Get detials
$(document).on("click", "#up-cbp-btn-pdc", function (e) {
    e.preventDefault();
    if (confirm("Are you sure you want to update the Instrument Status")) {
        $.ajax({
            type: "POST",
            url: $("#up-cbp").attr('action'),
            data: $("#up-cbp").serialize(),
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
                alert("Error Occured Try Again");
            }
        });
    }
});
// Get detials
$(document).on("click", "#up-ld-cbp-btn", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-cbp").attr('action'),
        data: $("#up-cbp").serialize(),
        success: function (data) {
            if (data) {
                alert("Updated");
            }
            else {
                alert("Error Occured Try Again");
            }
        },
        error: function () {
            alert("Error Occured Try Again");
        }
    });
});
// Get Dealers Data
$(document).on("click", ".dea-data", function () {
    var id = $(this).attr("id");
    window.open('/Dealership/DealershipData?Id=' + id, '_blank');
});
// Count the sum of all security Fees
$(document).on("keyup", ".total-file", function () {
    var val = $(this).val();
    var secval = parseFloat($("#sec-fee").html());
    var tl_sec = parseFloat(val * secval).toFixed(2);
    var id = $(this).parent().parent().attr("id");
    $("#" + id + " .sec").val(tl_sec);
    var totalsec = 0;
    $(".sec").each(function () {
        totalsec = parseFloat(totalsec) + parseFloat($(this).val());
    });
    $("#tot-sec").text(totalsec);
});
// Search File for delivery
$(document).on("click", "#sea-file-data-deliv", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileDelivery/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                //alert("No Result found");
                Swal.fire({
                    icon: 'info',
                    text: 'No record found'
                });
            }
            else {
                var dev = "";
                if (data.File.Development_Charges_Val == true) { dev = "With Development Charges"; } else if (data.File.Development_Charges_Val == false) { dev = "Non Development Charges"; }
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $('#file-id').val(data.File.Id);
                $('#file-trans-id').val(data.File.File_Transfer_Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#qr_Id').val(data.QR_Code_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#prj').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#blk').val(data.File.Block_Name);
                $('#status').text(data.File.Status);
                $('#dev-char').text(dev);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                $('#Name').val(data.File.Name);
                $('#Father_Husband').val(data.File.Father_Husband);
                $('#Postal_Address').val(data.File.Postal_Address);
                $('#Residential_Address').val(data.File.Residential_Address);
                $('#City').val(data.File.City);
                $('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                $('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                $('#Occupation').val(data.File.Occupation);
                $('#Nationality').val(data.File.Nationality);
                $('#Email').val(data.File.Email);
                $('#Phone_Office').val(data.File.Phone_Office);
                $('#Residential').val(data.File.Residential);
                $('#Mobile_1').val(data.File.Mobile_1);
                $('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                $("#Plot_Prefrence").val(data.File.Plot_Prefrence);
                var instserialno = 1, overduetotal = 0;
                if (data.File.Status == "Registered") {
                    if (data.File.Delivery) {
                        var html = '<button id="fil-del" style="width:100%" class="btn btn-success"><i class="ti-check"></i>&nbsp;&nbsp;File Deliverd</button>';
                        $('#file-del').html(html);
                    }
                    else {
                        var html = '<button id="fil-del" style="width:100%" class="btn btn-primary">Deliver File</button>';
                        $('#file-del').html(html);
                    }
                }
                $.each(data.Installments, function (i) {
                    var statuscolor = "";
                    singleinst = data.Installments[i].Amount * 2;
                    if (data.Installments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.Installments[i].Amount);
                    }
                    else if (data.Installments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.Installments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("MMMM YYYY") + ', ' + data.Installments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.Installments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.Installments[i].Status + '</td></tr>';
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                var otherinstserialno = 1;
                $.each(data.OtherInstallments, function (i) {
                    var statuscolor = "";
                    if (data.OtherInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.OtherInstallments[i].Amount);
                    }
                    else if (data.OtherInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.OtherInstallments[i].Id + '" ><td scope="row">' + otherinstserialno + '</td>' +
                        '<td>' + data.OtherInstallments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.OtherInstallments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.OtherInstallments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.OtherInstallments[i].Status + '</td></tr>';
                    $("#oth-inst tbody").append(html);
                    otherinstserialno++;
                });
                $("#ov-du-amt").text(overduetotal)
            }
        },
        error: function (data) {
            //alert("Error Occured Try Again")
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
            });
        }
    });
});
// enable all inputs of a user to update a information
$(document).on("click", "#upd-info", function () {
    $(".file-owner-data-nsdjk :input").prop("readonly", false);
    $(".owner-stat-jfkldgf").prop("disabled", false);
    $("#upd-info").hide();
    //$("#sav-info").show();
    $(".updt-exstng-owner-rec-dsffskfs").show();
});
// SAve the updated information
$(document).on("click", "#sav-info", function () {
    var od = {
        Id: $('#file-trans-id').val()
        , Name: $('#Name').val()
        , Currency_Note_No: $('#CNo').val()
        , Father_Husband: $('#Father_Husband').val()
        , Postal_Address: $('#Postal_Address').val()
        , Residential_Address: $('#Residential_Address').val()
        , Phone_Office: $('#Phone_Office').val()
        , Residential: $('#Residential').val()
        , Mobile_1: $('#Mobile_1').val()
        , Mobile_2: $('#Mobile_2').val()
        , Email: $('#Email').val()
        , Occupation: $('#Occupation').val()
        , Nationality: $('#Nationality').val()
        , Date_Of_Birth: $('#Date_Of_Birth').val()
        , CNIC_NICOP: $('#CNIC_NICOP').val()
        , Nominee_Name: $('#Nominee_Name').val()
        , Nominee_Relation: $('#Nominee_Relation').val()
        , Nominee_Address: $('#Nominee_Address').val()
        , Nominee_CNIC_NICOP: $('#Nominee_CNIC_NICOP').val()
        , DateTime: $('#DateTime').val()
        , City: $('#City').val()
    };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/FileSystem/UpdateOwnerResult/',
        data: JSON.stringify(od),
        success: function (data) {
            $("#upd-info").show();
            $("#sav-info").hide();
            $(".up-info :input").prop("readonly", true);
            alert("Data Updated");
        },
        error: function (data) {
        }
    });
});
// Update File Delivery
$(document).on("click", "#fil-del", function () {
    var overdueamt = parseFloat($("#ov-du-amt").text());
    if (overdueamt > singleinst) {
        //alert("Clear the Over Due Amount First");
        Swal.fire({
            icon: 'info',
            text: 'File with overdue amount cannot be marked as Delivered'
        })
        return false;
    }
    var id = $(this).data('grptag')
    //var act = confirm("Are you Sure you want Deliver this File")
    //if (act == true) {
    Swal.fire({
        text: 'Are you sure you want to mark this file as delivered?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/FileSystem/DeliverFile/',
                data: { Id: id },
                success: function (data) {
                    //alert("File Delivered Sucessfully");
                    Swal.fire({
                        icon: 'success',
                        text: 'File marked as delivered successfully'
                    }).then(() => {
                        var html = '<button id="fil-del" style="width:100%" class="btn btn-success"><i class="ti-check"></i>&nbsp;&nbsp;File Deliverd</button>';
                        $('#file-del').html(html);
                    })
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// search file for Transfer
$(document).on("click", "#sea-file-data-trans", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileDelivery/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $("#file-trans-alert").html("");
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $("#trns-btn").html("");
                $('#file-id').val(data.File.Id);
                $('#file-trans-id').val(data.File.File_Transfer_Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#grd-ttl').val(data.File.GrandTotal)
                $('#qr_Id').val(data.File.QR_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#plt-size').val(data.File.Plot_Size);
                $('#plt-pref').val(data.File.Plot_Prefrence);
                $('#fp-num').val(data.File.File_Form_Number);
                $('#prj').val(data.File.Project_Name);
                $('#prj-nam').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#ph-id').val(data.File.Phase_Id);
                $('#blk').val(data.File.Block_Name);
                $('#blk-id').val(data.File.Block_Id);
                $('#status').text(data.File.Status);
                $('#dev-char').text(data.File.Development_Charges);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                $('#Name').val(data.File.Name);
                $('#Father_Husband').val(data.File.Father_Husband);
                $('#Postal_Address').val(data.File.Postal_Address);
                $('#Residential_Address').val(data.File.Residential_Address);
                $('#City').val(data.File.City);
                $('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                $('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                $('#Occupation').val(data.File.Occupation);
                $('#Nationality').val(data.File.Nationality);
                $('#Email').val(data.File.Email);
                $('#Phone_Office').val(data.File.Phone_Office);
                $('#Residential').val(data.File.Residential);
                $('#Mobile_1').val(data.File.Mobile_1);
                $('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                $("#Plot_Prefrence").val(data.File.Plot_Prefrence);
                var instserialno = 1, overduetotal = 0;
                var pltsize = data.File.Plot_Size;
                var pltsize = pltsize.split(" ");
                var fees = pltsize[0] * 500;
                $("#transfer-fees").val(fees);
                $("#tras-fee").html(fees);
                $("#amt-in-words").val(InWords(fees));
                $.each(data.Installments, function (i) {
                    var statuscolor = "";
                    if (data.Installments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.Installments[i].Amount);
                    }
                    else if (data.Installments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.Installments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("MMMM YYYY") + '</td>' +
                        '<td>' + Number(data.Installments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.Installments[i].Status + '</td></tr>';
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                var otherinstserialno = 1;
                $.each(data.OtherInstallments, function (i) {
                    var statuscolor = "";
                    if (data.OtherInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.OtherInstallments[i].Amount);
                    }
                    else if (data.OtherInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.OtherInstallments[i].Id + '" ><td scope="row">' + otherinstserialno + '</td>' +
                        '<td>' + data.OtherInstallments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.OtherInstallments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.OtherInstallments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.OtherInstallments[i].Status + '</td></tr>';
                    $("#oth-inst tbody").append(html);
                    otherinstserialno++;
                });
                $("#ov-du-amt").text(overduetotal)
                var overdue = parseFloat(overduetotal);
                if (overduetotal > 0) {
                    var html = '<div class="alert alert-danger" role="alert">This File Can not be Transfer Due to pending OVER DUE Amount</div>';
                    $("#file-trans-alert").prepend(html);
                }
                else {
                    var btn = '<button class="btn btn-primary" type="submit" id="trans-frm" style="width:100%" >Transfer</button>';
                    $("#trns-btn").html(btn);
                }
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
// Transfer File
$(document).on("submit", "#transfer-file", function (e) {
    e.preventDefault();
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#transfer-file").attr('action'),
            data: $("#transfer-file").serialize(),
            success: function (data) {
                alert("Transfer Successfully");
                window.location.reload();
                window.open("/Banking/TransferReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
            }
        });
    }
});
// Transfer File
$(document).on("submit", "#transfer-plot", function (e) {
    e.preventDefault();
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#transfer-plot").attr('action'),
            data: $("#transfer-plot").serialize(),
            success: function (data) {
                alert("Transfer Successfully");
                window.location.reload();
                window.open("/Receipts/PlotTransferReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
            }
        });
    }
});
// Search the File information
$(document).on("submit", "#sea-fil-info", function (e) {
    e.preventDefault();
    var seaopt = $('input[name=SearchOption]').val();
    $.ajax({
        type: "POST",
        url: $('#sea-fil-info').attr("action"),
        data: $('#sea-fil-info').serialize(),
        success: function (data) {
            $('#tab-data').show();
            if (data.length > 0) {
                var html1 = '<table class="scroll table table-striped table-bordered" cellspacing="0" width="100%"><thead class="thead-dark">' +
                    '<tr><th>Sr.</th><th >Plot Size</th><th >File Number</th><th >Dev Charges</th><th  >Dealership</th><th  >Name</th><th  >Father Husband</th><th  >City</th><th >CNIC NICOP</th><th>Mobile</th><th>Ballot Status</th></tr></thead><tbody></tbody></table>';
                $('#tab-data').html(html1);
                var srcount = 1;
                $(data).each(function (i) {
                    var dev = "";
                    if (data[i].Development_Charges == true) { dev = "WDC"; } else if (data[i].Development_Charges == false) { dev = "NDC"; }
                    if (data[i].Ownership_Status == 'Owner') {
                        if (data[i].BallotedPlot_Id != null) {
                            var html = '<tr class="pointer bgc-green-50 get-fil-res" data-file="' + data[i].File_No + '" data-baltplt="' + data[i].BallotedPlot_Id + '" data-baltpltno="' + data[i].BallotedPlot_Number + '"><td width="50px">' + srcount + '</td><td width="90px">' + data[i].Plot_Size + '</td><<td width="140px">' + data[i].FileFormNumber + '</td><td width="120px">' + dev + '</td><td width="180px">' + data[i].Dealership_Name + '</td><td width="250px">' + data[i].Name + '</td><td width="250px">' + data[i].Father_Husband + '</td><td width="140px">' + data[i].City + '</td><td width="140px">' + data[i].CNIC_NICOP + '</td><td  width="140px">' + data[i].Mobile_1 + '</td><td  width="140px">' + 'Balloted File' + '</td></tr>';
                        }
                        else {
                            var html = '<tr class="pointer get-fil-res" data-file="' + data[i].File_No + '" data-baltplt="' + data[i].BallotedPlot_Id + '" data-baltpltno="' + data[i].BallotedPlot_Number + '"><td width="50px">' + srcount + '</td><td width="90px">' + data[i].Plot_Size + '</td><<td width="140px">' + data[i].FileFormNumber + '</td><td width="120px">' + dev + '</td><td width="180px">' + data[i].Dealership_Name + '</td><td width="250px">' + data[i].Name + '</td><td width="250px">' + data[i].Father_Husband + '</td><td width="140px">' + data[i].City + '</td><td width="140px">' + data[i].CNIC_NICOP + '</td><td  width="140px">' + data[i].Mobile_1 + '</td></tr>';
                        }
                    }
                    else {
                        if (data[i].BallotedPlot_Id != null) {
                            var html = '<tr class="pointer bgc-green-50 get-fil-res" data-file="' + data[i].File_No + '" data-baltplt="' + data[i].BallotedPlot_Id + '" data-baltpltno="' + data[i].BallotedPlot_Number + '"><td width="50px">' + srcount + '</td><td width="90px">' + data[i].Plot_Size + '</td><<td width="140px">' + data[i].FileFormNumber + '</td><td width="120px">' + dev + '</td><td width="180px">' + data[i].Dealership_Name + '</td><td width="250px">' + data[i].Name + ' (Previous Owner)</td><td width="250px">' + data[i].Father_Husband + '</td><td width="140px">' + data[i].City + '</td><td width="140px">' + data[i].CNIC_NICOP + '</td><td  width="140px">' + data[i].Mobile_1 + '</td><td  width="140px">' + 'Balloted File' + '</td></tr>';
                        }
                        else {
                            var html = '<tr class="pointer get-fil-res" data-file="' + data[i].File_No + '" data-baltplt="' + data[i].BallotedPlot_Id + '" data-baltpltno="' + data[i].BallotedPlot_Number + '"><td width="50px">' + srcount + '</td><td width="90px">' + data[i].Plot_Size + '</td><<td width="140px">' + data[i].FileFormNumber + '</td><td width="120px">' + dev + '</td><td width="180px">' + data[i].Dealership_Name + '</td><td width="250px">' + data[i].Name + ' (Previous Owner)</td><td width="250px">' + data[i].Father_Husband + '</td><td width="140px">' + data[i].City + '</td><td width="140px">' + data[i].CNIC_NICOP + '</td><td  width="140px">' + data[i].Mobile_1 + '</td></tr>';
                        }
                    }
                    $('#tab-data tbody').append(html);
                    srcount++;
                });
            }
            else {
                var html = '<h6>No Result Found</h6>';
                $('#tab-data').html(html);
            }
            Adjusttable();
        },
        error: function (data) {
        }
    });
});
// get the file information
$(document).on("click", ".get-fil-res", function () {
    var val = $(this).attr("data-file");
    let ballotedId = $(this).attr('data-baltplt');
    let ballotedno = $(this).attr('data-baltpltno');
    if (ballotedId != null && ballotedId != undefined && ballotedId != 'null') {
        window.location = '/Plots/PlotInformation?bltId=' + ballotedId + '&plotno=' + ballotedno;
    }
    else {
        SASLoad($("#file-details"));
        $("#file-details").load('/FileSystem/GetFileDetails/', { FileId: val }, function () {
            SASUnLoad($("#file-details"));
            $('html, body').animate({
                scrollTop: $("#file-details").offset().top
            }, 1000);
        });
    }
});
//
$(document).on("click", ".blk-elc-bills__type__wise", function () {
    var id = $(this).attr("id");
    var ty = $(this).attr("data-id");
    window.open("/ServiceCharges/ElectricityBillByTypeWise?BlockId=" + id + "&Type=" + ty, '_blank');
})
// initialize all banks
function InitBanks(i) {
    $.each(banklist, function (key, value) {
        $("#pay-" + i + " #bank").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
// add a payment
$(document).on("click", "#add-pay-typ", function () {
    Swal.fire({
        //title: 'Are you sure you want to Add New Owner?',
        text: 'Are you sure you want to Add another Payment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            paycont++;
            var html = '<div class="form-row pay-num" id="pay-' + paycont + '"><div class="form-group col-md-2"><label>Cash / Bank</label><select class="form-control paytypesel" id="cah-chq-bak" required>' +
                '<option value="Cash">Cash</option><option value="BankDraft">Bank Draft</option><option value="Cheque">Cheque</option><option value="PayOrder">Pay Order</option>' +
                '<option value="Online_Cash">Online - Cash</option><option value="Online_Cheque">Online - Cheque</option><option value="Online_Payorder">Online - PayOrder</option><option value="Online_Bankdraft">Online - BankDraft</option>' +
                '</select></div><div class="form-group col-md-1"><label>Amount</label><input class="form-control coma" placeholder="250,000" required><input type="hidden" id="Amount" class="amt" required>' +
                '</div><div class="form-group col-md-2 paymentotherinfo"><label id="paytypelabel">-</label><input type="text" class="form-control" id="paymenttype" placeholder="" disabled required>' +
                '</div><div class="form-group col-md-1 paymentotherinfo"><label>Date</label><input type="text" class="form-control" id="cbp-date" data-provide="datepicker" placeholder="mm/dd/yyyy" disabled required>' +
                '</div><div class="form-group col-md-2 paymentotherinfo"><label>Bank</label><select class="form-control" id="bank" disabled><option>Choose..</option></select></div>' +
                '<div class="form-group col-md-1 paymentotherinfo"><label>Branch </label><input type="text" class="form-control" id="Branch" placeholder="Mall Rd" disabled required></div>' +
                '<div class="form-group col-md-1 paymentotherinfo"><label>Scan File</label><button class=" btn btn-block" data-v=' + paycont + ' type="button" id="scanbtn">Scan</button></div>' +
                '<div id="images-' + paycont + '" class="col-md-1 images"></div>' +
                '<div class="form-group col-md-1"><i class="pointer ti-close rm-pa-type"></i></div></div>';
            $('#pay-list').append(html);
            InitBanks(paycont);
        }
    });
});
// add a payment
$(document).on("click", ".add-pay-typ", function () {
    Swal.fire({
        //title: 'Are you sure you want to Add New Owner?',
        text: 'Are you sure you want to Add another Payment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            paycont++;
            var html = '<div class="form-row pay-num" id="pay-' + paycont + '"><div class="form-group col-md-2"><label>Cash / Bank</label><select class="form-control paytypesel" id="cah-chq-bak" required>' +
                '<option value="Cash">Cash</option><option value="BankDraft">Bank Draft</option><option value="Cheque">Cheque</option><option value="PayOrder">Pay Order</option>' +
                '<option value="Online_Cash">Online - Cash</option><option value="Online_Cheque">Online - Cheque</option><option value="Online_Payorder">Online - PayOrder</option><option value="Online_Bankdraft">Online - BankDraft</option>' +
                '</select></div><div class="form-group col-md-2"><label>Amount</label><input class="form-control coma" placeholder="250,000" required><input type="hidden" id="Amount" class="amt" required>' +
                '</div><div class="form-group col-md-2 paymentotherinfo"><label id="paytypelabel">-</label><input type="text" class="form-control" id="paymenttype" placeholder="" disabled required>' +
                '</div><div class="form-group col-md-2 paymentotherinfo"><label>Date</label><input type="text" class="form-control" id="cbp-date" data-provide="datepicker" placeholder="mm/dd/yyyy" disabled required>' +
                '</div><div class="form-group col-md-2 paymentotherinfo"><label>Bank</label><select class="form-control" id="bank" disabled><option>Choose..</option></select></div>' +
                '<div class="form-group col-md-1 paymentotherinfo"><label>Branch </label><input type="text" class="form-control" id="Branch" placeholder="Mall Rd" disabled required></div><div class="form-group col-md-1"><i class="pointer ti-close rm-pa-type"></i></div></div>';
            $("#pay-list").append(html);
            InitBanks(paycont);
        }
    });
});
// remove a payment type
$(document).on("click", ".rm-pa-type", function () {
    Swal.fire({
        //title: 'Are you sure you want to Discard this Payment?',
        text: 'Are you sure you want to Discard this Payment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $(this).parent().parent().remove();
            paycont = 1;
            $(".pay-num").each(function () {
                paycont++;
                $(this).prop('id', 'pay-' + paycont);
            });
        }
    });
});
// Get Details of Generated Group Files
$(document).on("click", ".gene-files-detail", function () {
    var groupid = $(this).attr("id");
    window.open("/Dealership/GenerateDealerForms?Group_Id=" + groupid, '_blank');
});
// initialize plot installments 
$(document).on("submit", "#re-fil-test", function (e) {
    e.preventDefault();
    var advamt = parseFloat(RemoveComma($("#adv-pmt").text()));
    var totalamt = 0
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    if (advamt != totalamt) {
        alert("Received Amount is not equal to Booking Amount");
        return false;
    }
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash" && cash_che_bank !== "Online") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", ReceiptNo: ""
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.ReceiptNo = $("#pay-" + i + " #rece").val();
        allrecepts.push(recedata);
    }
    $("#reg-file").prop("disabled", true);
    var fileno = $("#app-num").val();
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Phase_Id: $("#phs_Id").val(),
        Block_Id: $("#blk_Id").val(),
        QR_Code: $("#qr_Id").val(),
        DateTime: $("input[name=DateTime]").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var alldata = {
        filedata: regfiledata,
        Flag: flag,
        DevCharStatus: $("#dev-char-st").val(),
        FileFormNumber: $("#app-num").val(),
        Receiptdata: allrecepts
    };
    $.ajax({
        type: "POST",
        url: $(this).attr("action"),
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(alldata),
        success: function (data) {
            window.location.reload();
        },
        error: function (data) {
        }
    });
});
//
$(document).on("click", "#add-pay-typ-test", function () {
    paycont++;
    var html = '<div class="form-row pay-num" id="pay-' + paycont + '"><div class="form-group col-md-1"><label>Cash / Bank</label><select class="form-control paytypesel" id="cah-chq-bak" required>' +
        '<option value="Cash">Cash</option><option value="BankDraft">Bank Draft</option><option value="Cheque">Cheque</option><option value="PayOrder">Pay Order</option>' +
        '<option value="Online_Cash">Online - Cash</option><option value="Online_Cheque">Online - Cheque</option><option value="Online_Payorder">Online - PayOrder</option><option value="Online_Bankdraft">Online - BankDraft</option>' +
        '</select></div><div class="form-group col-md-1"><label>Amount</label><input class="form-control coma" placeholder="250,000" required><input type="hidden" id="Amount" class="amt" required>' +
        '</div><div class="form-group col-md-1"><label>Receipt No.</label><input class="form-control " id="rece" required></div><div class="form-group col-md-1 paymentotherinfo"><label id="paytypelabel">-</label><input type="text" class="form-control" id="paymenttype" placeholder="" disabled required>' +
        '</div><div class="form-group col-md-1 paymentotherinfo"><label>Date</label><input type="text" class="form-control" id="cbp-date" data-provide="datepicker" placeholder="mm/dd/yyyy" disabled required>' +
        '</div><div class="form-group col-md-1 paymentotherinfo"><label>Bank</label><select class="form-control" id="bank" disabled><option>Choose..</option></select></div>' +
        '<div class="form-group col-md-1 paymentotherinfo"><label>Branch </label><input type="text" class="form-control" id="Branch" placeholder="Mall Rd" disabled></div><div class="form-group col-md-1"><i class="pointer ti-close rm-pa-type"></i></div></div>';
    $('#pay-list').append(html);
    InitBanks(paycont);
});
//
$(document).on("click", "#rece-btn-man", function () {
    EmptyModel();
    $('#ModalLabel').text("Installment");
    $('#modalbody').load('/Installments/AddInstallmentTest/');
});
//
$(document).on("click", "#on-btn-man", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Amount");
    $('#modalbody').load('/Banking/AddOnlineAmount/');
});
$(document).on("click", "#on-btn-man_un_supr", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Pending Amount");
    $('#modalbody').load('/Banking/AddOnlineAmountUnSupervised/');
});
//
$(document).on('click', '#add-new-unidnfd-rcpt-row', function () {
    var _strct = `
 <div class="cal item-unidnfd-rcpt-row col-md-12">
    <input type="hidden" name="ReceiptId" id="rece-id" value="0" />

     <div class="form-row dm-stat" style="justify-content:center"></div>
     <div class="form-row col-md-12">
     <span class="sr" style="margin: 1% 10px 0 0;">1</span>
     <div class="form-group col-md-2">
            <input type="text" class="form-control coma" id="Amount" name="Amount">   </div>
      <div class="form-group col-md-2">
            <select class="form-control" id="From_Bank" name="From_Bank">
                <option value="DIB">Dubai Islamic Bank</option>
                <option value="HBL">Habib Bank Limited</option>
            </select>
        </div>
     <div class="form-group col-md-3" >
          <select class="form-control" name="Bank" id="Bank">` + banklists + `</select>
     </div>
        <i class="ti-trash rmv-unidnfd-rcpt-row rmv" style="margin: 1% 0 0 2%;"></i>
    </div>
</div>`;
    $('#ad-demand').append(_strct);
    resetsr();


});
//

$(document).on('click', '.rmv-unidnfd-rcpt-row', function () {
    let con = confirm("Are you sure you want to drop this row?");

    if (con) {
        $(this).closest('.item-unidnfd-rcpt-row').remove();
    }
    resetsr();
    //AllCalc();
});
function resetsr() {
    var sr = 1;
    $('.sr').each(function () {
        $(this).text(sr);
        sr++;
    });
}
//
$(document).on("click", ".gen-tick", function () {
    EmptyModel();
    $('#ModalLabel').text("Create Ticket");
    $('#modalbody').load('/Tickets/CreateTicket/');
});
// Add Genrate Invoice
$(document).on("click", "#c-tick-btn", function (e) {
    e.preventDefault();
    $("#dep-nam").val($("#Department :selected").text());
    if (confirm("Are you sure you want to Generate Ticket")) {
        $.ajax({
            type: "POST",
            url: $("#c-tik").attr('action'),
            data: $("#c-tik").serialize(),
            success: function (data) {
                if (data.Status == true) {
                    var dat = new FormData();
                    var files = $('#quote-file').get(0).files;
                    if (files.length == 0) {
                        return;
                    }
                    for (var i = 0; i < files.length; i++) {
                        dat.append("Files", files[i]);
                    }
                    dat.append("Tick_Id", data.Ticket_Id);

                    $.ajax({
                        type: "POST",
                        processData: false,
                        contentType: false,
                        url: '/Tickets/UploadTickets/',
                        data: dat,
                        success: function (data) {
                            if (data.Status) {
                                closeModal();
                            }
                            else {
                                alert(data.Msg)
                            }
                        }
                    });
                    alert("Ticket Generated");
                    $("#tick-no").text(data.Ticket_No);
                    $('#c-tick-btn').attr("disabled", true);
                }
                else {
                    alert("Ticket Not Generated")
                    return false;
                }
            },

            error: function () {
                alert("Error Occured");
                $('#c-tick-btn').attr("disabled", false);
            }
        });
    }
});
// Add Genrate Invoice
$(document).on("click", ".tik-sea-res", function (e) {
    var from = $("#from").val();
    var to = $("#to").val();
    var dep = $("#Department").val();
    $("#tick-rep").load("/Reports/TicketStatus/", { From: from, To: to, Dep_Id: dep });
    $("#report").load("/Tickets/SearchTicket/", { From: from, To: to, Dep_Id: dep });
});
// Add Genrate Invoice
$(document).on("click", ".tik-sea-res-use", function (e) {
    var from = $("#from").val();
    var to = $("#to").val();
    var dep = $("#Users").val();
    $("#tick-rep").load("/Reports/HODTicketStatus/", { From: from, To: to, UserId: dep });
    $("#report").load("/Tickets/HODSearchTicket/", { From: from, To: to, UserId: dep });
});
//
$(document).on("click", ".tik-my-sea-res", function (e) {
    var from = $("#from").val();
    var to = $("#to").val();
    $("#report").load("/Tickets/SearchMyTicket/", { From: from, To: to });
});
// Add Genrate Invoice
$(document).on("change", ".dep-user", function (e) {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/Home/DepMisUser/',
        data: { DepId: id },
        success: function (data) {
            $('#AssignedTo').empty();
            $('#AssignedTo').append('<option value="">Select Employee</option>');
            $.each(data, function (key, value) {
                $('#AssignedTo').append('<option value="' + value.Id + '">' + value.Name + '</option>');
            });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
// Add Genrate Invoice
$(document).on("change", ".dep-tick-type", function (e) {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/Tickets/DepTicketsTypes/',
        data: { Id: id },
        success: function (data) {
            $('#title').empty();
            $('#title').append('<option value="">Select Problem Type</option>');
            $.each(data, function (key, value) {
                $('#title').append('<option value="' + value.Title + '">' + value.Title + '</option>');
            });
            $('#title').append('<option value="Other">Other</option>');
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
$(document).on("change", "#title", function () {
    var id = $(this).val();
    if (id === "Other") {
        $("#other-title").show()
    }
    else {
        $("#other-title").hide()
    }
});
// Add Ticket Type
$(document).on("click", ".a-t-t", function (e) {
    var dep = $('#Department').val();
    var title = $('#title').val();
    $.ajax({
        type: "POST",
        url: '/Tickets/AddTicketType/',
        data: { DepId: dep, Title: title },
        success: function (data) {
            if (data == 1) {
                alert("Added");
                window.location.reload();
            }
            else {
                alert("Already Exists");
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
// Delete Ticket Type
$(document).on("click", ".del-tic-type", function (e) {
    var id = $(this).data("id");
    var a = confirm("Are you sure you want to Delete this Ticket Type");
    if (a) {
        $.ajax({
            type: "POST",
            url: '/Tickets/DeleteTicketType/',
            data: { Id: id },
            success: function (data) {
                $('#' + id).remove();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
// Add Genrate Invoice
$(document).on("click", "#gen-rec-test", function (e) {
    e.preventDefault();
    $('#gen-rec-test').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    $.ajax({
        type: "POST",
        url: $("#re-ins").attr('action'),
        data: $("#re-ins").serialize(),
        success: function (data) {
            window.location.reload();
        },
        error: function () {
            alert("Error Occured");
            $('#gen-rec').attr("disabled", false);
        }
    });
});
// Add online amt Invoice
$(document).on("click", "#add-on-amt", function (e) {
    e.preventDefault();
    $('#add-on-amt').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    Swal.fire({
        text: 'Are you sure you want to add online receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#onli-amt").attr('action'),
                data: $("#onli-amt").serialize(),
                success: function (data) {
                    if (data.Status) {
                        Swal.fire({
                            icon: 'sucess',
                            text: 'Updated successfully, receipt number is ' + data.ReceiptNo
                        })
                        //alert("receipt no is " + data.ReceiptNo);
                    }
                    else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'info',
                            text: data.Msg
                        });
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Add online amt Invoice
$(document).on("click", "#add-on-amt_un_sup", function (e) {
    e.preventDefault();
    $('#add-on-amt').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    $.ajax({
        type: "POST",
        url: $("#add-amt_un").attr('action'),
        data: $("#add-amt_un").serialize(),
        success: function (data) {
            if (data.Status) {
                alert("receipt no is " + data.ReceiptNo);
            }
            else {
                alert(data.Msg);
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", ".paytypesel-test", function () {
    var res = $(this).val();
    var text = $("option:selected", this).text();
    if (res !== "Cash" && res !== "Online") {
        $(" .paymentotherinfo").css("display", "block");
        $(" .paymentotherinfo :input").attr("disabled", false);
        $("#paytypelabel").text(text + "No.");
        $(" #paymenttype").attr("placeholder", text);
    }
    else if (res == "Online") {
        $(" .paymentotherinfo").css("display", "block");
        $(" .paymentotherinfo :input").attr("disabled", false);
        $("#paytypelabel").text("Bank Acc No.");
        $(" #paymenttype").attr("placeholder", "Account No.");
    }
    else {
        $(" .paymentotherinfo").hide();
        $(" .paymentotherinfo :input").attr("disabled", true);
    }
});
//// to hide or unhide a table 
//$("input:checkbox:not(:checked)").each(function () {
//    var column = "table ." + $(this).attr("name");
//    $(column).hide();
//});
//
//$("input:checkbox").click(function () {
//    var column = "table ." + $(this).attr("name");
//    $(column).toggle();
//});
// Empty from to
$(document).on("click", "#date", function () {
    $("#from").val(" ");
    $("#to").val(" ");
});
// Empty from to
$(document).on("click", "#from, #to", function () {
    $("#date").val(" ");
});
// to search Account results
$(document).on("click", ".acc-sea-res", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    var type = $("#Type").val();
    SASLoad($("#report"));
    $("#report").load("/Reports/SearchAccountReport/", { From: from, To: to, Users: user, Type: type }, function () {
        SASUnLoad($("#report"));
    });
});
// to search Account results
$(document).on("click", ".acc-sam-sea-res", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    $("#report").load("/Reports/SearchSAMAccountReport/", { From: from, To: to, Users: user });
});
// to search Account results
$(document).on("click", ".sea-can-req", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    $("#report").load("/Finance/AllCancelRequestSearch/", { From: from, To: to, Users: user });
});
// to search Account results
$(document).on("click", ".sea-can-req-mar", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    $("#report").load("/Finance/MarketingAllCancelRequestSearch/", { From: from, To: to, Users: user });
});
//
$(document).on("click", ".rec-sea-res", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    $("#report").load("/Finance/ReceiptSearchResult/", { From: from, To: to, Users: user });
});
//
$(document).on("click", ".rec-sea-res-mar", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#users").val();
    $("#report").load("/Finance/MarketingReceiptSearchResult/", { From: from, To: to, Users: user });
});
//
$(document).on("click", ".rec-sea-res-bkmis", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var val = $("#bank").val();
    $("#report").load("/Banking/Search_BanksMIS/", { From: from, To: to, Val: val });
});
//
$(document).on("click", ".button_click_activity_search", function () {
    var from = $(".date-from_activity").val();
    var to = $(".date-to_activity").val();
    $("#accivity_seacrh_manag").load("/Activity/SearchMyActivitReport/", { From: from, To: to, });
});
//
function fnExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#result').html();
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test').attr('download', filename + '.xls');
    }
}
//
$(document).on("click", ".get-f-res", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileResult/',
        data: { Filefromid: val },
        success: function (data) {
            $("#file-details").show();
            $("#oth-inst tbody").empty();
            $("#rec-amts tbody").empty();
            $("#all-instments tbody").empty();
            $("#dis-amts tbody").empty();
            if (data == false) {
                //alert("No Result found");
                Swal.fire({
                    icon: 'error',
                    text: 'No record found'
                });
            }
            else {
                var dev = "";
                if (data.File.Development_Charges_Val == true) { dev = "With Development Charges"; } else if (data.File.Development_Charges_Val == false) { dev = "Non Development Charges"; }
                $('#app-num').val(data.File.File_Form_Number);
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $('#file-id').val(data.File.Id);
                $('#plt-id').val(data.File.Id);
                $('.file_id').val(data.File.Id);
                if (data.File.Image1 != null) {
                    $('#img1').attr('src', '/Repository/CustomerImages/' + data.File.File_Transfer_Id + '/1.png');
                }
                else {
                    $('#img1').attr('src', '/assets/static/images/def-img.png');
                }
                if (data.File.Image2 != null) {
                    $('#img2').attr('src', '/Repository/CustomerImages/' + data.File.File_Transfer_Id + '/2.png');
                }
                else {
                    $('#img2').attr('src', '/assets/static/images/def-img.png');
                }
                if (data.File.Image3 != null) {
                    $('#img3').attr('src', '/Repository/CustomerImages/' + data.File.File_Transfer_Id + '/3.png');
                }
                else {
                    $('#img3').attr('src', '/assets/static/images/def-img.png');
                }
                if (data.File.Image4 != null) {
                    $('#img4').attr('src', '/Repository/CustomerImages/' + data.File.File_Transfer_Id + '/4.png');
                }
                else {
                    $('#img4').attr('src', '/assets/static/images/def-img.png');
                }
                $('#file-trans-id').val(data.File.File_Transfer_Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#qr_Id').val(data.QR_Code_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#prj').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#blk').val(data.File.Block_Name);
                $('#status').text(data.File.Status);
                $('#dev-char').text(dev);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                //$('#Name').val(data.File.Name);
                //$('#Father_Husband').val(data.File.Father_Husband);
                //$('#Postal_Address').val(data.File.Postal_Address);
                //$('#Residential_Address').val(data.File.Residential_Address);
                //$('#City').val(data.File.City);
                //$('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                //$('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                //$('#Occupation').val(data.File.Occupation);
                //$('#Nationality').val(data.File.Nationality);
                //$('#Email').val(data.File.Email);
                //$('#Phone_Office').val(data.File.Phone_Office);
                //$('#Residential').val(data.File.Residential);
                //$('#Mobile_1').val(data.File.Mobile_1);
                //$('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                $("#Plot_Prefrence").val(data.File.Plot_Prefrence);
                $("#DateTime").val(moment(data.File.Date_Reg).format("MM-DD-YYYY"))
                var stat = data.File.Status;
                if (stat != "Registered") {
                    $("#rece-btn-man").hide();
                    $("#on-btn-man").hide();
                }
                else {
                    $("#rece-btn-man").show();
                    $("#on-btn-man").show();
                }
                if (data.File.Verify) {
                    $("#veri-stat").prop('checked', true);;
                }
                else {
                    $("#veri-stat").prop('checked', false);;
                }
                var instserialno = 1, overduetotal = 0;
                if (data.File.Delivery) {
                    $("#del-stat").prop('checked', true);;
                }
                else {
                    $("#del-stat").prop('checked', false);;
                }
                $.each(data.Installments, function (i) {
                    var statuscolor = "";
                    if (data.Installments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.Installments[i].Amount);
                    }
                    else if (data.Installments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class="' + statuscolor + ' Ins__rec__upd  pointer" data-toggle="modal" data-target="#Modal"   id="' + data.Installments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("MMMM YYYY") + ', ' + data.Installments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.Installments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.Installments[i].Status + '</td></tr>';
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                var otherinstserialno = 1;
                $.each(data.OtherInstallments, function (i) {
                    var statuscolor = "";
                    if (data.OtherInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.OtherInstallments[i].Amount);
                    }
                    else if (data.OtherInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class="' + statuscolor + ' Ins__rec__upd  pointer" data-toggle="modal" data-target="#Modal"   id="' + data.OtherInstallments[i].Id + '" ><td scope="row">' + otherinstserialno + '</td>' +
                        '<td>' + data.OtherInstallments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.OtherInstallments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.OtherInstallments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.OtherInstallments[i].Status + '</td></tr>';
                    $("#oth-inst tbody").append(html);
                    otherinstserialno++;
                });
                $("#ov-du-amt").text(overduetotal);
                var totalamt = 0;
                $.each(data.ReceivedAmounts, function (i) {
                    var statuscolor = "";
                    if (data.ReceivedAmounts[i].Status == "" || data.ReceivedAmounts[i].Status == null || data.ReceivedAmounts[i].Status == "Approved") {
                        totalamt = totalamt + parseFloat(data.ReceivedAmounts[i].Amount);
                    }
                    if (data.ReceivedAmounts[i].Status == 'Approved') {
                        statuscolor = "bgc-green-50";
                    }
                    else if (data.ReceivedAmounts[i].Status == 'Dishonored') {
                        statuscolor = "bgc-red-50";
                    }
                    else if (data.ReceivedAmounts[i].Status == 'Pending') {
                        statuscolor = "bgc-yellow-50";
                    }
                    var html = '<tr class=" ' + statuscolor + '" id="' + data.ReceivedAmounts[i].Id + '" data-token="' + data.ReceivedAmounts[i].TokenParameter + '" >' +
                        '<td scope="row" class="rece-num">' + data.ReceivedAmounts[i].ReceiptNo + '</td>' +
                        '<td class="rece-amt">' + Number(data.ReceivedAmounts[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].Bank + '</td>' +
                        '<td>' + moment(data.ReceivedAmounts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td class="rece-type">' + data.ReceivedAmounts[i].PaymentType + '</td>' +
                        '<td><ul style="list-style:none;margin-left:0px;"><li class="dropdown">' +
                        '<a href="" class="dropdown-toggle no-after peers " data-toggle="dropdown"><i class="ti-more-alt  c-grey-900" style="transform:rotate(90deg);"></i></a>' +
                        '<ul class="dropdown-menu" style="padding:10px;width:auto !important">' +
                        '<li><a id="" class="get-rec-data pointer" data-toggle="modal" data-target="#Modal"><span>Update/Delete</span></a></li>' +
                        '<hr />' +
                        '<li><a id="" class="rece-refund-pop-file pointer" data-toggle="modal" data-target="#Modal"><span>Refund</span></a></li>' +
                        '</ul></li></ul ></td>' +
                        '</tr>';
                    $("#rec-amts tbody").append(html);
                });
                var html1 = '<tr><td>Total</td><td colspan="4">' + Number(totalamt).toLocaleString() + '</td></tr>';
                $("#rec-amts tbody").append(html1);
                $.each(data.Discounts, function (i) {
                    $(".dis").show();
                    var html = `<tr id="${data.Discounts[i].Id}" ><td> ${Number(data.Discounts[i].Total_Amount).toLocaleString()} </td>
                                <td> ${Number(data.Discounts[i].Discount_Amount).toLocaleString()} </td>
                                <td> ${parseFloat(data.Discounts[i].Percentage).toFixed(2)}% </td >
                                <td> ${data.Discounts[i].Remarks} </td>
                                <td><i class="fa fa-trash rem-dis"></i></td>
                                </tr>`;
                    $("#dis-amts tbody").append(html);
                });
                $('.owners-trail-fjhdsf').load('/FileSystem/FileOwnersData/', { fileId: data.File.Id });
            }
        },
        error: function (data) {
            //alert("Error Occured Try Again")
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
            });
        }
    });
});
//
$(document).on("click", ".Ins__rec__upd", function () {
    var id = $(this).attr("id");
    var mod = $("#module").val();
    EmptyModel();
    $('#ModalLabel').text("Update Installments");
    $('.modal-body').load('/Installments/UpdateSingleInstallment/', { Id: id, Module: mod }, function () {
    });
});
//
$(document).on("click", ".upda___Sing__ins ", function () {
    var id = $(this).attr("id");
    var mod = $("#module").val();
    var psd = $('#Dv__date').val();
    if (psd == "" || psd == null) {
        alert('Please fill the required feilds');
        return false;
    }
    var psa = $('#Dv__amt').val();
    if (psa == "" || psa == null) {
        alert('Please fill the required feilds');
        return false;
    }
    var insty = $('.inst-type').val();
    var dataset =
    {
        InsatallmentStartingDate: psd,
        Installment_Type: insty,
        InstallmentAmountPerMonth: psa,
        Id: id
    };
    var ch = confirm('Are you sure you want to update');
    if (ch) {
        $.post('/Installments/InstallmentUpdate/', { plan: dataset, Module: mod }, function () {
            alert('Successfully Updated');
        });
    }
});
//
$(document).on("click", "#update__Ins__pln", function () {
    var id = $('#file-id').val();
    EmptyModel();
    $('#ModalLabel').text("Update Installments");
    $('.modal-body').load('/FileSystem/UpdationForInstallments/', { FileId: id }, function () {
    });
});
//
$(document).on("click", "#Make__update__request__table", function () {
    var ad = $("#Advance_date").val();
    if (ad == "" || ad == null) {
        alert('All feilds are required');
        return false;
    }
    var am = $("#advance_amt").val();
    var psd = $("#p__st__date").val();
    if (psd == "" || psd == null) {
        alert('All feilds are required');
        return false;
    }
    var psi = $("#p__st__ins").val();
    var psa = $("#p__st__amt").val();
    var bd = $("#b__date").val();
    if (bd == "" || bd == null) {
        alert('All feilds are required');
        return false;
    }
    var ba = $("#b__amt").val();
    var DD = $("#Dv__date").val();
    if (DD == "" || DD == null) {
        alert('All feilds are required');
        return false;
    }
    var Dva = $("#Dv__amt").val();;
    var dataset =
    {
        AdvanceDate: ad,
        AdvacneAmount: am,
        InsatallmentStartingDate: psd,
        NoOfInstallments: psi,
        InstallmentAmountPerMonth: psa,
        BalltionDate: bd,
        BallotinAmount: ba,
        DevelopmentDate: DD,
        DevelopmentAmount: Dva
    };
    $('#body_load__instal').load('/FileSystem/InstallmentPlanMaker/', { Plan: dataset }, function (data) {
        if (data == false) {
            alert("All feilds are required");
        }
    });
});
//
$(document).on("click", ".upda___fil__str", function () {
    var Fileid = $(this).attr("id");
    var ad = $("#Advance_date").val();
    if (ad == "" || ad == null) {
        alert('All feilds are required');
        return false;
    }
    var am = $("#advance_amt").val();
    var psd = $("#p__st__date").val();
    if (psd == "" || psd == null) {
        alert('All feilds are required');
        return false;
    }
    var psi = $("#p__st__ins").val();
    var psa = $("#p__st__amt").val();
    var bd = $("#b__date").val();
    if (bd == "" || bd == null) {
        alert('All feilds are required');
        return false;
    }
    var ba = $("#b__amt").val();
    var DD = $("#Dv__date").val();
    if (DD == "" || DD == null) {
        alert('All feilds are required');
        return false;
    }
    var Dva = $("#Dv__amt").val();
    var dataset =
    {
        FileId: Fileid,
        AdvanceDate: ad,
        AdvacneAmount: am,
        InsatallmentStartingDate: psd,
        NoOfInstallments: psi,
        InstallmentAmountPerMonth: psa,
        BalltionDate: bd,
        BallotinAmount: ba,
        DevelopmentDate: DD,
        DevelopmentAmount: Dva
    };
    var ch = confirm('Are you sure you want to generate this plan');
    if (ch) {
        $.post('/FileSystem/FinalizeInstallments/', { Plan: dataset }, function (data) {
            if (data == false) {
                alert("All feilds are required");
            }
            else {
                alert('Successfully generated');
            }
        });
    }
});
//
$(document).on("click", ".get-rec-data", function () {
    EmptyModel();
    var id = $(this).closest('tr').attr("id");
    $('.modal-body').load('/Banking/ReceiptData/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".rec-adj-det", function () {
    EmptyModel();
    var id = $(this).attr("id");
    $('.modal-body').load('/Finance/ReceiptAdjustmentDetails/', { Id: id }, function () { });
});
//
$(document).on("click", ".fin-adj-det", function () {
    EmptyModel();
    var id = $(this).attr("id");
    $('.modal-body').load('/Finance/FinanceAdjustmentDetails/', { Id: id }, function () { });
});
//
$(document).on("click", "#adjust-rec", function () {
    var id = $('#adj-req-id').val();
    var conf = confirm("Are you Sure You want to Adjust this Receipt")
    if (conf) {
        $.ajax({
            type: "POST",
            url: '/Finance/UpdateReceiptAdjustment/',
            data: { Id: id },
            success: function (data) {
                if (data) {
                    alert("Receipt Adjusted Sucessfully");
                    window.open("/Receipts/AdjustmentReceipt?Id=" + data, '_blank');
                    window.location.reload();
                }
                else {
                    alert("Error ");
                }
            }
        });
    }
});
//
$(document).on("click", ".prnt-prev", function () {
    EmptyModel();
    var id = $(this).closest('tr').data("rece");
    window.open("/Receipts/AdjustmentReceipt?Id=" + id, '_blank');
});
//
$(document).on("click", "#adjust-rec-fin", function () {
    var id = $('#adj-req-id').val();
    var conf = confirm("Are you Sure You want to Adjust this Receipt")
    if (conf) {
        $.ajax({
            type: "POST",
            url: '/Finance/FinanceUpdateReceiptAdjustment/',
            data: { Id: id },
            success: function (data) {
                if (data) {
                    alert("Receipt Adjusted Sucessfully");
                    window.location.reload();
                }
                else {
                    alert("Error Occured");
                }
            }
        });
    }
});
//
$(document).on("click", "#up-rece-btn", function () {
    var amt = Number($("input[name=Amount]").val());
    if (amt <= 0 || amt == null) {
        //alert("Amount Cannot be empty or zero");
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid amount to proceed'
        });
        return false;
    }
    //if (confirm("Are you sure you want to update")) {
    Swal.fire({
        text: 'Are you sure you want to update the receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#up-rece").attr('action'),
                data: $("#up-rece").serialize(),
                success: function (data) {
                    if (data.Status) {
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        });
                        //alert(data.Msg);
                    }
                    else {
                        Swal.fire({
                            icon: 'info',
                            text: data.Msg
                        });
                        //alert(data.Msg);
                    }
                },
                error: function () {
                    //alert("Error");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
//
$(document).on("click", ".del__rece__btn", function () {
    var R_Id = $(this).attr("id");
    //var ch = confirm('Are you sure you want to delete this receipt');
    //if (ch) {
    Swal.fire({
        text: 'Are you sure you want to delete the receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Banking/ReceiptDelete/',
                data: { ReceiptId: R_Id },
                success: function (data) {
                    if (data == true) {
                        //alert("Successfully Deleted");
                        Swal.fire({
                            icon: 'success',
                            text: 'Receipt deleted successfully'
                        });
                    } else {
                        //alert("Already deleted");

                    }
                },
                error: function () {
                    //alert("Error");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
//
$(document).on("click", ".del-rep-ven", function () {
    var R_Id = $(this).attr("id");
    var ch = confirm('Are you sure you want to delete this Representative');
    if (ch) {
        $.ajax({
            type: "POST",
            url: '/Vendor/DeleteRepresentative/',
            data: { id: R_Id },
            success: function (data) {
                if (data == true) {
                    alert("Successfully Deleted");
                    window.location.reload();
                } else {
                    alert("Already deleted");
                }
            },
            error: function () {
                alert("Error");
            }
        });
    }
});
//$(document).on("click", ".add-all-quot", function () {
//    var itemsData = [];
//    allok = true;
//    //let trans = $('.trans-id').val();
//    let grp = $('#Group_Id').val();
//    let venId = $('.vendor').val();
//    let quot_ref = $('.quot-ref').val();
//    let curr = $('.ccy').val();
//    let desc = $('.Description').val();
//    $('.it-qu-ch:checked').each(function () {
//        let reqid = $(this).closest('tr').data('id');
//        let itemid = $(this).closest('tr').data('item');
//        let qty = RemoveComma($(this).closest('tr').find('.qty').val());
//        let rate = RemoveComma($(this).closest('tr').find('.rate-p-uom').val());
//        let gst = RemoveComma($(this).closest('tr').find('.gst').val());
//        let del_date = $(this).closest('tr').find('.deliv-date').val();
//        itemsData.push({
//            PurchaseReq_Id: reqid,
//            Item_Id: itemid,
//            Qty: qty,
//            PurchaseRate: rate,
//            Tax: gst,
//            Deliver_Date: del_date
//        });
//    });
//    if (allok) {
//        if (confirm("Are you sure you want to Upload Quotations")) {
//            $.ajax({
//                type: "POST",
//                url: "/Procurement/AddQuotation_All/",
//                contentType: "application/json",
//                traditional: true,
//                beforeSend: function () {
//                },
//                complete: function () {
//                },
//                data: JSON.stringify({ items: itemsData, Vendor_Id: venId, Group_Id: grp, Quote_Ref: quot_ref, Currency: curr, Description: desc }),
//                success: function (data) {
//                    if (data.Status == true) {
//                        alert(data.Msg);
//                        var dat = new FormData();
//                        var files = $('#quote-file').get(0).files;
//                        if (files.length == 0) {
//                            return;
//                        }
//                        dat.append("Files", files[0]);
//                        dat.append("Group_Id", grp);
//                        dat.append("QuoteId", data.Id);
//                        $.ajax({
//                            type: "POST",
//                            processData: false,
//                            contentType: false,
//                            url: '/Procurement/UploadQuotation/',
//                            data: dat,
//                            success: function (data) {
//                                if (data.Status) {
//                                }
//                                else {
//                                    alert(data.Msg)
//                                }
//                            }
//                        });
//                        //window.location.reload();
//                    }
//                }
//            });
//        }
//    }
//});
// General Verification
$(document).on("change", "#veri-stat", function () {
    var id = $("#file-id").val();
    if (this.checked) {
        $.post('/FileSystem/VerifyFile/', { Id: id, Status: true }, function () {
            alert("Verified")
        });
    } else {
        $.post('/FileSystem/VerifyFile/', { Id: id, Status: false }, function () {
        });
    }
});
//
$(document).on("change", "#del-stat", function () {
    var id = $("#file-trans-id").val();
    if (this.checked) {
        $.post('/FileSystem/DeliverFile/', { Id: id }, function () {
            alert("Delivered")
        });
    } else {
        $.post('/FileSystem/DeliverFile/', { Id: id }, function () {
        });
    }
});
//
$(document).on("change", "#Plot_Prefrence", function () {
    var id = $(".file-trans-id").val();
    var plotpref = $(this).val();
    Swal.fire({
        text: 'Are you sure you want to update the plot preference?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/FileSystem/UpdatePlotPref/", { Id: id, Pref: plotpref }, function () {
                //alert("Updated")
                Swal.fire({
                    icon: 'success',
                    text: 'Plot preference updated successfully'
                });
            });
        }
    });
});
// Register  a file
$(document).on("submit", "#expo-re-fil", function (e) {
    e.preventDefault();
    var advamt = parseFloat(RemoveComma($("#adv-pmt").text()));
    var totalamt = 0
    var tokenexp = $("#tok-exp-date").val();
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    var token = $("#token-stat").is(":checked")
    if (!token) {
        if (advamt != totalamt) {
            alert("Received Amount is not equal to Booking Amount");
            return false;
        }
    }
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        allrecepts.push(recedata);
    }
    $("#reg-file").prop("disabled", true);
    var fileno = $("#app-num").val();
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Phase_Id: $("#phs_Id").val(),
        Block_Id: $("#blk_Id").val(),
        QR_Code: $("#qr_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var alldata = {
        filedata: regfiledata,
        Flag: flag,
        DevCharStatus: $("#dev-char-st").val(),
        FileFormNumber: $("#app-num").val(),
        Receiptdata: allrecepts,
        Token: token,
        TokenExp: tokenexp
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("File No. " + fileno + " Has Been Registerd");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
                else if (data.Status == "2") {
                    alert("Wait Until You Payment is Clear from Bank");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
            },
            error: function (data) {
            }
        });
    }
});
//
$(document).on("change", "#token-stat", function () {
    if (this.checked) {
        $("#tok-exp-date").show();
    } else {
        $("#tok-exp-date").hide();
    }
});
//
$(document).on("click", "#sea-tok-file-data", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetTokenFileResult/',
        data: { Filefromid: val },
        success: function (data) {
            $("#file-details").show();
            $("#rec-amts tbody").empty();
            if (data == false) {
                alert("No Result found");
            }
            else {
                $('#app-num').val(data.File.File_Form_Number);
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $('#file-id').val(data.File.Id);
                $('#file-trans-id').val(data.File.File_Transfer_Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#qr_Id').val(data.QR_Code_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#prj').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#blk').val(data.File.Block_Name);
                $('#status').text(data.File.Status);
                $('#dev-char').text(data.File.Development_Charges);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                $('#Name').val(data.File.Name);
                $('#Father_Husband').val(data.File.Father_Husband);
                $('#Postal_Address').val(data.File.Postal_Address);
                $('#Residential_Address').val(data.File.Residential_Address);
                $('#City').val(data.File.City);
                $('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                $('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                $('#Occupation').val(data.File.Occupation);
                $('#Nationality').val(data.File.Nationality);
                $('#Email').val(data.File.Email);
                $('#Phone_Office').val(data.File.Phone_Office);
                $('#Residential').val(data.File.Residential);
                $('#Mobile_1').val(data.File.Mobile_1);
                $('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                var totalamt = 0;
                $.each(data.ReceivedAmounts, function (i) {
                    var html = '<tr id="' + data.ReceivedAmounts[i].Id + '" data-token="' + data.ReceivedAmounts[i].TokenParameter + '" >' +
                        '<td scope="row">' + data.ReceivedAmounts[i].ReceiptNo + '</td>' +
                        '<td>' + Number(data.ReceivedAmounts[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.ReceivedAmounts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].PaymentType + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                    totalamt = totalamt + parseFloat(data.ReceivedAmounts[i].Amount);
                });
                var html1 = '<tr><td>Total</td><td colspan="4">' + totalamt + '</td></tr>';
                $("#rec-amts tbody").append(html1);
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
//
$(document).on("click", ".file-folo-det", function () {
    var val = $(this).attr("id");
    $("tr").removeClass("highlight");
    $('#fol-data').load('/CallCenter/GetFileFollowupData/', { Id: val }, function () {
        $("#" + val).addClass("highlight");
        $("#file-no").text($("#" + val + " .file-num").text());
        $("#name").text($("#" + val + " .name").text());
        $("#File_Plot_Number").val($("#" + val + " .file-num").text());
        $("#Owner_Name").val($("#" + val + " .name").text());
    });
});
//
$(document).on("submit", "#ad-flo", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    if ((msg == null || msg == "") && file.length == 0) {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#ad-flo");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#ad-flo").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/Followups/' + data[i].File_plot_number + '/' + data[i].Msg + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
        }
    });
});
//

$(document).on('click', '.supervis_pnd', function () {
    var id = [];
    $('.all-pnd').each(function () {
        var t = $(this).is(":checked");
        if (t) {
            id.push($(this).data('id'));
        }
    });
    if (confirm("Are you sure you want to Supervise these entries?")) {
        $.ajax({
            type: "POST",
            url: '/Banking/SupervisePndReceipts/',
            data: { Id: id },
            success: function (data) {
                if (data.Status) {
                    alert("Supervised");
                    window.location.reload();
                }
                else {
                    alert("Error Occured")
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
})


//
$(document).on("click", "#ad-rec-adj-unspr-pndg-req", function () {

    fileplotarray = [];
    var fileplotno = $("#PlotId").val();
    var id = $("#rece-id").val();
    var type = $('#Module').val();
    var conf = confirm("Are you Sure You want to Update unit no to Receipt")
    if (conf) {
        $.ajax({
            traditional: true,
            type: "POST",
            data: { FilePlotNo: fileplotno, Rcp_Id: id, Type: type },
            url: "/Finance/UpdateFilePlotNo/",
            success: function (data) {
                if (data) {
                    alert("Successfully Update Unit No");
                }
                else {
                    alert("Error");
                }
            },
            error: function () {
            }
        });
    }
});


//
$(document).on("click", "#ad-rec-adj-unspr-pndg-multiunits", function () {

    fileplotarray = [];
    var id = $("#rece-id").val();
    var i = 1;
    var notok = false;
    $('.item-unidnfd-rcpt-row').each(function () {
        var Amount = $(this).find('#Amount').val();
        var From_Bank = $(this).find('#From_Bank').val();
        var coma = RemoveComma($(this).find('#Amount').val());
        var AmountInWords = InWords(coma);
        var Bank = $(this).find('#Bank').val();

        if (From_Bank == "" || From_Bank == null) {
            alert("Please Select Bank ");
            notok = true;
            return false;
        }
        if (Bank == "" || Bank == null) {
            alert("Please Select Bank ");
            notok = true;
            return false;
        }
        if (Amount == "" || Amount == null) {
            alert("Please Enter Amount");
            notok = true;
            return false;
        }
        if (i != '2') {
            fileplotarray.push({
                Bank: Bank,
                From_Bank: From_Bank,
                AmountInWords: AmountInWords,
                Amount: coma,
            });
        }
        i++
    });
    if (notok) {
        return false;
    }
    //debugger
    //for (var i = 1; i < fileplotarray.length; i++) {
    //    if (fileplotarray[i] !== fileplotarray[0]) {
    //        return false;
    //    }
    //}
    //if (fileplotarray == null  || fileplotarray=='')
    //{
    //    return false;
    //}
    var conf = confirm("Are you Sure You want to Adjust amount against this Receipt")
    if (conf) {
        $.ajax({
            type: "POST",
            data: { Rcp_Id: id, FilePlotArray: fileplotarray },
            url: "/Banking/AddOnlineAmountMultiunitsUnSupervised/",
            success: function (data) {
                debugger
                if (data) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
            }
        });
    }
});

//
$(document).on("click", "#reg-PurReq", function (e) {
    InventoryArray = [];
    var groupId = $('.grpId').val();
    var shopid = $('.shop_id').val();
    var notok = false;


    $('.item-pur-req-row').each(function () {
        var Product = $(this).find(".product").val();
        var product_name = $(this).find(".product").text();
        var srnum = $(this).find(".sr").text();
        var Quantity = $(this).find(".qty").val();
        var Rate = $(this).find(".rate").val();
        var date = $(this).find(".date").val();

        if (Quantity == "" || Quantity == null) {
            alert("Please Enter Quantity in " + product_name + " at line number " + srnum);
            notok = true;
            return false;
        }
        if (Rate == "" || Rate == null) {
            alert("Please Enter Rate in " + product_name + " at line number " + srnum);
            notok = true;
            return false;
        }

        if (InventoryArray.filter(x => x.Item_Id == Product).length > 0) {
            alert(product_name + " can not be repeat again at line number : " + srnum);
            notok = true;
            return false;
        }
        InventoryArray.push({
            Item_Id: Product,
            Qty: Quantity,
            Rate: Rate,
            Group_Id: groupId,
            Purchase_Date: date
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Submit the Purchases")) {
        $.ajax({
            type: "POST",
            url: '/HOH/NewPurchaseRequisition/',
            data: { demand: InventoryArray, Shop_Id: shopid },
            success: function (data) {
                if (data.Status == false) {
                    alert(data.Msg);
                }
                else {
                    $('#reg-DemndOrdr').prop('disabled', true);
                    alert(data.Msg);
                    window.location.reload();
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".rc-up", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/ReceiptsDetails/', { Id: id }, function () {
    });
});
////
$(document).on("click", ".rc-up-Unspr-Pnd", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/PendingReceiptsDetails/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".rc-up-Unspr-Pnd-multifileplot", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/PendingReceiptsMultifileplot/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".rc-adj", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/ReceiptDetails/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".rc-can", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/CancelReq/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".rc-up-mar", function () {
    var id = $(this).closest('tr').attr("id");
    var ld = $(this).closest('tr').data("leddeal");
    EmptyModel();
    $('.modal-body').load('/Finance/MarketingReceiptsDetails/', { Id: id, LeadDeal: ld }, function () {
    });
});
//
$(document).on("click", ".rc-can-mar", function () {
    var id = $(this).closest('tr').attr("id");
    var ld = $(this).closest('tr').data("leddeal");
    EmptyModel();
    $('.modal-body').load('/Finance/MarketingCancelReq/', { Id: id, LeadDeal: ld }, function () {
    });
});
//
$(document).on("click", "#up-req-cbp", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-re-refrm").attr('action'),
        data: $("#up-re-refrm").serialize(),
        success: function (data) {
            if (data) {
                alert("Requested");
                $('#modal').modal('toggle');
            }
            else {
                alert("Error Requested");
            }
        }
    });
});
//
$(document).on("click", "#up-req-cbp-mar", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-re-refrm").attr('action'),
        data: $("#up-re-refrm").serialize(),
        success: function (data) {
            if (data) {
                alert("Requested");
                $('#modal').modal('toggle');
            }
            else {
                alert("Error Requested");
            }
        }
    });
});
//
$(document).on("click", "#can-req-rece", function (e) {
    e.preventDefault();
    var conf = confirm("Are you Sure You want to Cancel this Receipt")
    if (conf) {
        $.ajax({
            type: "POST",
            url: $("#can-re-refrm").attr('action'),
            data: $("#can-re-refrm").serialize(),
            success: function (data) {
                if (data) {
                    alert("Requested");
                    $('#modal').modal('toggle');
                }
                else {
                    alert("Error Requested");
                }
            }
        });
    }
});
//
$(document).on("click", "#can-req-rece-mar", function (e) {
    e.preventDefault();
    var conf = confirm("Are you Sure You want to Cancel this Receipt")
    if (conf) {
        $.ajax({
            type: "POST",
            url: $("#can-re-refrm").attr('action'),
            data: $("#can-re-refrm").serialize(),
            success: function (data) {
                if (data) {
                    alert("Requested");
                }
                else {
                    alert("Error Requested");
                }
            }
        });
    }
});
//
$(document).on("click", "#up-rec-btn", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-rec").attr('action'),
        data: $("#up-rec").serialize(),
        success: function (data) {
            if (data) {
                alert("updated");
            }
            else {
                alert("Error Requested");
            }
        }
    });
});
//
$(document).on("click", "#up-rec-btn-mar", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-rec").attr('action'),
        data: $("#up-rec").serialize(),
        success: function (data) {
            if (data) {
                alert("updated");
                window.location.reload();
            }
            else {
                alert("Error Requested");
            }
        }
    });
});
//
$(document).on("change", ".can-rev", function () {
    var id = $(this).val();
    if (id == "3") {
        $(".ret-amt").show();
    }
    else {
        $(".ret-amt").hide();
    }
});
//
$(document).on("click", "#up-rec-can-btn", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-cancel-rec").attr('action'),
        data: $("#up-cancel-rec").serialize(),
        success: function (data) {
            if (data == true) {
                $("#up-rec-can-btn").prop("disabled", true)
                alert("updated");
            }
            else {
                window.open("/Vouchers/SAGVouchers?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
            }
        }
    });
});
//
$(document).on("click", "#up-rec-can-btn-mar", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-cancel-rec").attr('action'),
        data: $("#up-cancel-rec").serialize(),
        success: function (data) {
            if (data == true) {
                $("#up-rec-can-btn-mar").prop("disabled", true)
                alert("updated");
            }
        }
    });
});
//
$(document).on("click", ".req-rec-det", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/RequestDetails/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".req-rec-det-mar", function () {
    var id = $(this).closest('tr').attr("id");
    var ld = $(this).closest('tr').data("ld");
    EmptyModel();
    $('.modal-body').load('/Finance/MarketingRequestDetails/', { Id: id, LeadDeal: ld }, function () {
    });
});
//
$(document).on("click", ".can-rece-det", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Finance/CancelRequestDetails/', { Id: id }, function () {
    });
});
//
$(document).on("click", ".can-rece-det-mar", function () {
    var id = $(this).closest('tr').attr("id");
    var ld = $(this).closest('tr').data("ld");
    EmptyModel();
    $('.modal-body').load('/Finance/MarketingCancelRequestDetails/', { Id: id, LeadDeal: ld }, function () {
    });
});
//
$(document).on("click", ".cust-info", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/FileSystem/GetFileDetails/', { FileId: id }, function () {
        $(".modal-dialog").attr('style', 'max-width: 1500px !important');
        $("#ModalLabel").text("Details");
    });
});
//
$(document).on("click", ".cust-plt-info", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Plots/PlotDetails/', { Plotid: id }, function () {
        $(".modal-dialog").attr('style', 'max-width: 1500px !important');
        $("#ModalLabel").text("Details");
    });
});
//
$(document).on("click", ".cust-com-info", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/Commercial/CommercialInformationSearch/', { Commercial_Id: id }, function () {
        $(".modal-dialog").attr('style', 'max-width: 1500px !important');
        $("#ModalLabel").text("Details");
    });
});
// create a Service Charges 
$(document).on("submit", "#ad-sur-ch", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-sur-ch").attr('action'),
        data: $("#ad-sur-ch").serialize(),
        success: function (data) {
            if (data) {
                alert("Service Charge added");
                $("#surch-lst").load("/ServiceCharges/AllServiceCharges/");
            }
            else {
                alert("This is Already Exist");
            }
        }
    });
});
// create a Service Charges 
$(document).on("submit", "#ad-con-ch", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-con-ch").attr('action'),
        data: $("#ad-con-ch").serialize(),
        success: function (data) {
            if (data) {
                alert("Connection Charge added");
                $("#Con-lst").load("/ServiceCharges/ConnectionChargesList/");
            }
            else {
                alert("This is Already Exist");
            }
        }
    });
});
// Update Electricity Bill
$(document).on("click", ".ed-rate", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/UpdateRates/', { Id: id }, function () {
    });
});
//update level meter rate
$(document).on("click", "#up-elc-uni-btn", function () {
    $.ajax({
        type: "POST",
        url: $("#up-elec-rat").attr('action'),
        data: $("#up-elec-rat").serialize(),
        success: function () {
            alert("updated");
            window.location.reload();
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
// Plot List
$(document).on("click", "#elec-plt-lst", function () {
    var id = $("#Blocks").val();
    $("#plot-det").empty()
    $("#plot-det").load("/ServiceCharges/PlotsElectricityList/", { Id: id });
});
//// Plot List
$(document).on("click", ".plt-lst", function () {
    var id = $(this).closest('tr').attr("id");
    $.post("/Plots/PlotListByBlock/", { Id: id }, function (data) {
        $('#Blk-nam').text($("#" + id + " .blk-nam").text() + "Block");
        $("#com-plt-lst tbody").empty();
        $.each(data.Commercial, function (i) {
            var html = '<tr class="pointer " ><td class="plt-num">' + data.Commercial[i].Plot_Number + '</td>' +
                '<td>' + data.Commercial[i].Plot_Size + '</td>' +
                '<td>' + data.Commercial[i].Type + '</td>' +
                '<td>' + data.Commercial[i].Status + '</td>' +
                '<td>' + data.Commercial[i].Name + '</td><td><i class="ti-bookmark-alt all-ser-char" data-toggle="modal" data-target="#Modal" data-pltid="' + data.Commercial[i].Plot_Id + '"></i> <i class="ti-pencil plt-cur-bil" data-pltid="' + data.Commercial[i].Plot_Id + '" data-toggle="modal" data-target="#Modal" ></i></td></tr>';
            $("#com-plt-lst tbody").append(html);
        });
        $("#res-plt-lst tbody").empty();
        $.each(data.Residential, function (i) {
            var html = '<tr class="pointer"><td class="plt-num">' + data.Residential[i].Plot_Number + '</td>' +
                '<td>' + data.Residential[i].Plot_Size + '</td>' +
                '<td>' + data.Residential[i].Type + '</td>' +
                '<td>' + data.Residential[i].Status + '</td>' +
                '<td>' + data.Residential[i].Name + '</td><td><i class="ti-bookmark-alt all-ser-char" data-toggle="modal" data-target="#Modal" data-pltid="' + data.Residential[i].Plot_Id + '"></i> <i class="ti-pencil plt-cur-bil" data-pltid="' + data.Residential[i].Plot_Id + '" data-toggle="modal" data-target="#Modal" class"ti-pencil"></i></td></tr>';
            $("#res-plt-lst tbody").append(html);
        })
    })
});
// Elecetric Plot List
$(document).on("click", "plt-lst", function () {
    var id = $(this).closest('tr').attr("id");
    $.post("/Plots/PlotListByBlock/", { Id: id }, function (data) {
        $('#Blk-nam').text($("#" + id + " .blk-nam").text() + "Block");
        $("#plt-lst tbody").empty();
        $.each(data, function (i) {
            var html = '<tr class="pointer plt-bl-det" data-id="' + data[i].Plot_Id + '" id="plt_' + data[i].Plot_Id + '"><td class="plt-num">' + data[i].Plot_Number + '</td>' +
                '<td>' + data[i].Plot_Size + '</td>' +
                '<td>' + data[i].Type + '</td>' +
                '<td>' + data[i].Status + '</td>' +
                '<td>' + data[i].Name + '</td></tr>';
            $("#plt-lst tbody").append(html);
        })
    })
});
// Plot Bill Details 
$(document).on("click", ".plt-bl-det", function (data) {
    var id = $(this).attr("id");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/NewElectricityBill/', { Id: id }, function () {
    });
});
$(document).on("click", ".bill__pre__his", function () {
    var pltid = $(this).attr('data-id');
    EmptyModel();
    $('#ModalLabel').text("History");
    $('.modal-body').load('/ServiceCharges/BillHistory/', { PlotId: pltid }, function () {
    });
});
// Plot Bill Details 
$(document).on("click", ".plt-scbl-det", function (data) {
    var id = $(this).attr("data-id");
    EmptyModel();
    $('#ModalLabel').text("History");
    $('.modal-body').load('/ServiceCharges/ServiceChargesBills/', { Id: id }, function () {
    });
    //$.post("/ServiceCharges/ServiceChargesBills/", { Id: id }, function (data) {
    //    $('#plot-num').text("Plot No.  : " + $("#plt_" + id + " .plt-num").text());
    //    var icon = '<button style="margin-left:10%" data-toggle="modal" data-target="#Modal" data-pltid="' + id + '" class="btn btn-info  plt-cur-bil">Generate This Month Bill</button><i style="float:right" data-pltid="' + id + '" class="ti-pencil ser-char"  data-toggle="modal" data-target="#Modal"></i>';
    //    $('#plot-num').append(icon);
    //    $("#bill-lst tbody").empty();
    //    if (data.length > 0) {
    //        $.each(data, function (i) {
    //            var html = '<tr id="' + data[i].Id + '">' +
    //                '<td>' + Number(data[i].Grand_Total).toLocaleString() + '</td>' +
    //                '<td>' + Number(data[i].Amount_Paid).toLocaleString() + '</td>' +
    //                '<td>' + Number(data[i].Remaining).toLocaleString() + '</td>' +
    //                '<td>' + moment(data[i].Date).format("MMM-D-YYYY") + '</td>' +
    //                '<td>' + moment(data[i].Due_Date).format("MMM-D-YYYY") + '</td>' +
    //                '<td>' + Number(data[i].Net_Total).toLocaleString() + '</td>' +
    //                '<td><i class="ti-eye vie-ser-bill" data-id="' + data[i].Id + '"></i></td></tr>';
    //            $("#bill-lst tbody").append(html);
    //        })
    //    }
    //    else {
    //        var html = '<tr><td colspan="4">No Bill is Generated Yet</td></tr>';
    //        $("#bill-lst tbody").append(html);
    //    }
    //});
});
//
$(document).on("click", ".re-gen-ser-bill", function () {
    var id = $(this).attr("data-id");
    EmptyModel();
    window.open("/ServiceCharges/ViewServiceBill?Id=" + id, '_blank');
});
//
$(document).on("click", ".vie-ser-bill", function () {
    var id = $(this).attr("data-id");
    EmptyModel();
    window.open("/ServiceCharges/ViewServiceBill?Id=" + id, '_blank');
});
//
$(document).on("click", ".vie_ser_bill_Remarks", function () {
    var id = $(this).attr("data-id");
    $.post('/ServiceCharges/ServiceChargesBillsRemarks/', { Id: id }, function (data) {
        if (data == false) {
            alert('Not Found');
        }
        else {
            alert(data);
        }
    });
});
//
$(document).on("click", ".bill__pre__his_remarks", function () {
    var pltid = $(this).attr('data-id');
    $.post('/ServiceCharges/ElectricityBillsRemarks/', { Id: pltid }, function (data) {
        if (data == false) {
            alert('Not Found');
        }
        else {
            alert(data);
        }
    });
});
//
$(document).on("click", ".vie-elec-bill", function () {
    var id = $(this).attr("data-id");
    EmptyModel();
    window.open("/ServiceCharges/ViewPlotElectricityBill?billNo=" + id, '_blank');
});
// Service Charges Bill Details
$(document).on("click", ".ser-char", function () {
    var pltid = $(this).attr("data-pltid");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/PlotServiceChargesList/', { Id: pltid }, function () {
    });
});
// All Service Charges Details
$(document).on("click", ".all-ser-char", function () {
    var pltid = $(this).attr("data-pltid");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/PlotsSubscribtions/', { Id: pltid }, function () {
    });
});
// Meter remove
$(document).on("click", ".rm-elec-met", function () {
    var id = $(this).attr("id");
    $.post('/ServiceCharges/RemoveMeter/', { Id: id }, function (data) {
        if (data) {
            $("#m-" + id).remove();
        }
        else {
            alert(" Already Removed");
        }
    });
});
// to subscribe or unscubscribe a Service Charge
$(document).on("change", ".ser-char-subunsb", function () {
    var id = $("#plot_id").val();
    var ser_chid = $(this).val();
    var ser_chnam = $(this).attr("data-scnam");
    if (this.checked) { // subscribed
        $.post('/ServiceCharges/Subscribe_Unsubscribe/', { PlotId: id, SC_Id: ser_chid, Name: ser_chnam, Status: true }, function (data) {
            if (data) {
            }
            else {
                alert(ser_chnam + " is Already Subscribed");
            }
        });
    } else { // for un subscribe
        $.post('/ServiceCharges/Subscribe_Unsubscribe/', { PlotId: id, SC_Id: ser_chid, Name: ser_chnam, Status: false }, function (data) {
            if (data) {
            }
            else {
                alert(ser_chnam + " is Already Unsubscribed");
            }
        });
    }
});
// to subscribe or unscubscribe a Service Charge
$(document).on("click", "#req-pay", function () {
    var plot_id = $("#plot_id").val();
    var amt = RemoveComma($("#amt").val());
    var reason = $("#reason").val();
    if (confirm("Are you sure you want to request Payment")) {
        $.ajax({
            type: "POST",
            url: '/ServiceCharges/RequestPayment/',
            data: { Plot_Id: plot_id, Amount: amt, Reason: reason },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
                alert("Error Occured");
                $('#gen-rec').attr("disabled", false);
            }
        });
    }
});
// Generate Service charges Bill
$(document).on("click", ".plt-cur-bil", function () {
    var pltid = $(this).attr("data-pltid");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/CurrentMonthBill/', { PlotId: pltid }, function () {
    });
});
// Generate Service charges Bill
$(document).on("click", ".ser-pay", function () {
    var pltid = $(this).attr("data-pltid");
    EmptyModel();
    $('.modal-body').load('/ServiceCharges/PaymentReturnRequest/', { Plot_Id: pltid }, function () {
    });
});
//
$(document).on("keyup", ".recal-bil", function () {
    CalBill();
});
//
function CalBill() {
    var tot = parseInt($("#total-bill").val());
    var arre = parseInt($("#arrears").val()) || 0;
    var gtot = tot + arre;
    $("#g-tot").val(gtot);
}
//
$(document).on("keyup", ".calebill", function (e) {
    var cur = $(this).val() * 1;
    var pre = $("#prev").val() * 1;
    if (cur < pre) {
        $("#unit").val("");
        $("#total-bill").val("");
        $("#g-tot").val("");
    }
    else {
        var uni = cur - pre;
        $("#unit").val(uni)
        var rat = $("#rate").val();
        var amt = uni * rat;
        $("#total-bill").val(amt);
        CalBill()
    }
});
//
$(document).on("click", "#gen-ser-bill", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#re-gen-bill").attr('action'),
        data: $("#re-gen-bill").serialize(),
        success: function (data) {
            if (data == false) {
                alert('You cannot generate the bill because amount is already paid');
            }
            else {
                window.open("/ServiceCharges/ViewServiceBill?Id=" + data, "_blank");
            }
        }
    });
});
//
$(document).on("click", "#gen-elec-bill", function (e) {
    e.preventDefault();
    var arre = parseInt($("#arrears").val()) || 0;
    var prr = Number($(".pr__rea__up").val());
    var cur = Number($(".calebill").val());
    if (cur < prr) {
        alert('Current Reading cannot less then previous reading');
        return false;
    }
    var rem = $("#rem").val();
    var id = $("#bill-id").val();
    var plt = $("#plt-id").val();
    var gt = $("#g-tot").val();
    $.ajax({
        type: "POST",
        url: '/ServiceCharges/UpdateElecBill/',
        data: { Arrears: arre, Current: cur, Remarks: rem, Id: id, Previous: prr, grndtot: gt },
        success: function (data) {
            if (data == false) {
                alert('Current Reading cannot less then previous reading...');
            }
            else {
                window.open("/ServiceCharges/ViewPlotElectricityBill?Plotid=" + id, "_blank");
                window.location.reload();
            }
        }
    });
});
$(document).on("click", ".bill__pre__his", function () {
    var pltid = $(this).attr('data-id');
    EmptyModel();
    $('#ModalLabel').text("History");
    $('.modal-body').load('/ServiceCharges/BillHistory/', { PlotId: pltid }, function () {
    });
});
//....service charges report
$(document).on("click", "#servce-plt-lst-sht_new", function () {
    var id = $("#elec-Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#pre__ap__detls").empty();
        if (id != null) {
            $("#pre__ap__detls").load("/ServiceCharges/ElectricityCharges_Short/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#pre__ap__detls").load("/ServiceCharges/ElectricityCharges_Short/", { month: mm, year: yy });
        }
    }
});
//....service charges bills 
$(document).on("click", "#servce-service-lst-sht_new_Show", function () {
    var id = $("#elec-Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#pre__ap__detls").empty();
        if (id != null) {
            $("#pre__ap__detls").load("/ServiceCharges/ServiceCharges_Short/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#pre__ap__detls").load("/ServiceCharges/ServiceCharges_Short/", { month: mm, year: yy });
        }
    }
});
//
$(document).on("click", ".blk-sc-bills", function () {
    var id = $(this).attr("id");
    window.open("/ServiceCharges/ServiceChargesBillsByBlock?BlockId=" + id, "_blank");
})
//
$(document).on("click", ".blk-elc-bills", function () {
    var id = $(this).attr("id");
    window.open("/ServiceCharges/ElectricityBillByBlock?BlockId=" + id, "_blank");
})
//
$(document).on("click", ".blk-elc-bills__type__wise", function () {
    var id = $(this).attr("id");
    var ty = $(this).attr("data-id");
    window.open("/ServiceCharges/ElectricityBillByTypeWise?BlockId=" + id + "&Type=" + ty, '_blank');
});
//
$(document).on("click", ".blk-ser-bills__type__wise", function () {
    var id = $(this).attr("id");
    var ty = $(this).attr("data-id");
    window.open("/ServiceCharges/ServicechargesBillByTypeWise?BlockId=" + id + "&Type=" + ty, '_blank');
});
// Search File for Transfer request
$(document).on("click", "#sea-file-data-trans-req", function () {
    var val = $('#app-num').val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/GetFileDelivery/',
        data: { Filefromid: val },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else if (data == -1) {
                alert("This Plot Cannot be Transfer from Here");
            }
            else {
                var dev = "";
                if (data.File.Development_Charges_Val == true) { dev = "With Development Charges"; } else if (data.File.Development_Charges_Val == false) { dev = "Non Development Charges"; }
                $("#file-trans-alert").html("");
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $("#trns-btn").html("");
                $('#file-id').val(data.File.Id);
                $('#file-trans-id').val(data.File.File_Transfer_Id);
                $('#file-rate').val(data.File.Rate)
                $('#ttl-amt').val(data.File.TotalPlotVal)
                $('#grd-ttl').val(data.File.GrandTotal)
                $('#qr_Id').val(data.File.QR_Id)
                $('#deal-nam').val(data.File.Dealership_Name);
                $('#pl-size').val(data.File.Plot_Size);
                $('#plt-size').val(data.File.Plot_Size);
                $('#plt-pref').val(data.File.Plot_Prefrence);
                $('#fp-num').val(data.File.File_Form_Number);
                $('#prj').val(data.File.Project_Name);
                $('#prj-nam').val(data.File.Project_Name);
                $('#phs').val(data.File.Phase_Name);
                $('#ph-id').val(data.File.Phase_Id);
                $('#blk').val(data.File.Block_Name);
                $('#blk-id').val(data.File.Block_Id);
                $('#status').text(data.File.Status);
                $('#dev-char').text(dev);
                $('#qr_img').attr("src", '/QR Codes/' + data.File.File_Form_Number + '.png');
                $('#Name').val(data.File.Name);
                $('#Father_Husband').val(data.File.Father_Husband);
                $('#Postal_Address').val(data.File.Postal_Address);
                $('#Residential_Address').val(data.File.Residential_Address);
                $('#City').val(data.File.City);
                $('#Date_Of_Birth').val(data.File.Date_Of_Birth);
                $('#CNIC_NICOP').val(data.File.CNIC_NICOP);
                $('#Occupation').val(data.File.Occupation);
                $('#Nationality').val(data.File.Nationality);
                $('#Email').val(data.File.Email);
                $('#Phone_Office').val(data.File.Phone_Office);
                $('#Residential').val(data.File.Residential);
                $('#Mobile_1').val(data.File.Mobile_1);
                $('#Mobile_2').val(data.File.Mobile_2);
                $('#Nominee_Name').val(data.File.Nominee_Name);
                $('#Nominee_CNIC_NICOP').val(data.File.Nominee_CNIC_NICOP);
                $('#Nominee_Relation').val(data.File.Nominee_Relation);
                $('#Nominee_Address').val(data.File.Nominee_Address);
                $('#bal-amt').text(Number(data.File.Balance_Amount).toLocaleString());
                $("#Plot_Prefrence").val(data.File.Plot_Prefrence);
                var instserialno = 1, overduetotal = 0;
                var pltsize = data.File.Plot_Size;
                var pltsize = pltsize.split(" ");
                var fees = pltsize[0] * 500;
                $("#transfer-fees").val(fees);
                $("#tras-fee").html(fees);
                $("#amt-in-words").val(InWords(fees));
                $.each(data.Installments, function (i) {
                    var statuscolor = "";
                    if (data.Installments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.Installments[i].Amount);
                    }
                    else if (data.Installments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.Installments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("MMMM YYYY") + '</td>' +
                        '<td>' + Number(data.Installments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.Installments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.Installments[i].Status + '</td></tr>';
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                var otherinstserialno = 1;
                $.each(data.OtherInstallments, function (i) {
                    var statuscolor = "";
                    if (data.OtherInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        overduetotal = parseFloat(overduetotal) + parseFloat(data.OtherInstallments[i].Amount);
                    }
                    else if (data.OtherInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.OtherInstallments[i].Id + '" ><td scope="row">' + otherinstserialno + '</td>' +
                        '<td>' + data.OtherInstallments[i].Installment_Name + '</td>' +
                        '<td>' + Number(data.OtherInstallments[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.OtherInstallments[i].Due_Date).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.OtherInstallments[i].Status + '</td></tr>';
                    $("#oth-inst tbody").append(html);
                    otherinstserialno++;
                });
                var totalamt = 0;
                $.each(data.ReceivedAmounts, function (i) {
                    var html = '<tr id="' + data.ReceivedAmounts[i].Id + '" data-token="' + data.ReceivedAmounts[i].TokenParameter + '" >' +
                        '<td scope="row">' + data.ReceivedAmounts[i].ReceiptNo + '</td>' +
                        '<td>' + Number(data.ReceivedAmounts[i].Amount).toLocaleString() + '</td>' +
                        '<td>' + moment(data.ReceivedAmounts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.ReceivedAmounts[i].PaymentType + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                    totalamt = totalamt + parseFloat(data.ReceivedAmounts[i].Amount);
                });
                var html1 = '<tr><td>Total</td><td colspan="4">' + totalamt + '</td></tr>';
                $("#rec-amts tbody").append(html1);
                $("#ov-du-amt").text(Number(overduetotal).toLocaleString())
                var overdue = parseFloat(overduetotal);
                if (data.File.Status != "Registered") {
                    alert("File is " + data.File.Status);
                } else {
                    if (data.File.AuditVerified) {
                        if (overduetotal > 0) {
                            var html = '<div class="alert alert-danger" role="alert">This File Can not be Transfer Due to pending OVER DUE Amount</div>';
                            $("#file-trans-alert").prepend(html);
                        }
                        else {
                            var btn = '<button class="btn btn-primary" type="submit" id="trans-frm" style="width:100%" >Generate Transfer Request</button>';
                            $("#trns-btn").html(btn);
                        }
                    }
                    else {
                        var html = '<div class="alert alert-danger" role="alert">This File is not Verified</div>';
                        if (data.File.Verification_Req) {
                            var html1 = '<div class="col-md-4"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6><label class="switch"><input type="checkbox" id="veri-req-f" checked><span class="slider round"></span></label></div>';
                            $("#file-trans-alert").prepend(html1);
                        }
                        else {
                            var html1 = '<div class="col-md-4"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6><label class="switch"><input type="checkbox" id="veri-req-f"><span class="slider round"></span></label></div>';
                            $("#file-trans-alert").prepend(html1);
                        }
                        $("#file-trans-alert").prepend(html);
                    }
                }
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
// Search File for Plot request
$(document).on("click", "#sea-plot-data-trans-req", function () {
    var id = $("#plot-details").val();
    $.ajax({
        type: "POST",
        url: '/Plots/LastestPlotDetailsData/',
        data: { Plotid: id },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $("#plot-trans-alert").html("");
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $("#rec-amts tbody").empty();
                $("#trns-btn").html("");
                $('#plt-no').text(data.PlotData.Plot_No);
                $('#plt-blk').text(data.PlotData.Block_Name);
                $('#plt-id').val(data.PlotData.Id);
                $('#plt-size').text(data.PlotData.Plot_Size);
                $('#plot-id').val(data.PlotData.Id);
                $('#plot-size').val(data.PlotData.Plot_Size);
                $('#plt-type').text(data.PlotData.Type);
                $('#plt-dim').text(data.PlotData.Dimension);
                $('#plt-status').text(data.PlotData.Develop_Status.replace('_', ' '));
                $('#plt-road').text(data.PlotData.Road_Type);
                $('#plt-area').text(data.PlotData.Area + " Sq-Ft");
                $('#plt-loc').text(data.PlotData.Plot_Location);
                if (data.PlotData.Development_Charges == true) {
                    $('#dev-ch').text("With Development Charges");
                }
                else if (data.PlotData.Development_Charges == false) {
                    $('#dev-ch').text("Non Development Charges");
                }
                else if (data.PlotData.Development_Charges == null && data.PlotData.Block_Name == "Sher Afghan") {
                    $('#dev-ch').text("To Be Announced");
                }
                else {
                    $('#dev-ch').text("");
                }
                //$('#Name').val(data.PlotOwners.Name);
                //$('#Father_Husband').val(data.PlotOwners.Father_Husband);
                //$('#Postal_Address').val(data.PlotOwners.Postal_Address);
                //$('#Residential_Address').val(data.PlotOwners.Residential_Address);
                //$('#City').val(data.PlotOwners.City);
                //$('#Date_Of_Birth').val(data.PlotOwners.Date_Of_Birth);
                //$('#CNIC_NICOP').val(data.PlotOwners.CNIC_NICOP);
                //$('#Occupation').val(data.PlotOwners.Occupation);
                //$('#Nationality').val(data.PlotOwners.Nationality);
                //$('#Email').val(data.PlotOwners.Email);
                //$('#Phone_Office').val(data.PlotOwners.Phone_Office);
                //$('#Residential').val(data.PlotOwners.Residential);
                //$('#Mobile_1').val(data.PlotOwners.Mobile_1);
                //$('#Mobile_2').val(data.PlotOwners.Mobile_2);
                //$('#Nominee_Name').val(data.PlotOwners.Nominee_Name);
                //$('#Nominee_CNIC_NICOP').val(data.PlotOwners.Nominee_CNIC_NICOP);
                //$('#Nominee_Relation').val(data.PlotOwners.Nominee_Relation);
                //$('#Nominee_Address').val(data.PlotOwners.Nominee_Address);
                $('.trnsfr-plt-prc').val(data.Plot_Price_DC_Rate.toLocaleString());
                $('#DC_Plot_Price').val(data.Plot_Price_DC_Rate);
                $('.dc-rate-plt-prc-sjdfhkl').text(data.Plot_Price_DC_Rate.toLocaleString());
                $("#chat-his").load("/Messages/PlotsComments/", { Plotid: data.PlotData.Id });
                $('.plot-owns-fsdhjk').load('/Plots/PlotOwners/', { plotId: data.PlotData.Id, current: true });
                var instserialno = 1, overduetotal = 0;
                var bokamt = 0;
                var totalamt = 0;
                $.each(data.PlotInstallments, function (i) {
                    var statuscolor = "";
                    if (data.PlotInstallments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        totalamt = totalamt + data.PlotInstallments[i].Amount;
                    }
                    else if (data.PlotInstallments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.PlotInstallments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + data.PlotInstallments[i].Installment_Name + '</td>' +
                        '<td>' + moment(data.PlotInstallments[i].DueDate).format("MMMM YYYY") + '</td>' +
                        '<td>' + data.PlotInstallments[i].Amount + '</td>' +
                        '<td>' + data.PlotInstallments[i].Status + '</td></tr>';
                    bokamt = bokamt + data.PlotInstallments[i].Amount;
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                bokamt = parseFloat(bokamt).toFixed(0);
                $("#plt-bk-price").text(Number(bokamt).toLocaleString());
                var rec = 0
                $.each(data.PlotReceipts, function (i) {
                    var html = '<tr id="' + data.PlotReceipts[i].Id + '" >' +
                        '<td scope="row">' + data.PlotReceipts[i].ReceiptNo + '</td>' +
                        '<td>' + data.PlotReceipts[i].Amount + '</td>' +
                        '<td>' + moment(data.PlotReceipts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.PlotReceipts[i].PaymentType + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                    if (data.PlotReceipts[i].Status == "Approved" || data.PlotReceipts[i].Status == null) {
                        rec = rec + data.PlotReceipts[i].Amount;
                    }
                });
                rec = parseFloat(totalamt).toFixed(0);
                $("#rece-amt").text(Number(rec).toLocaleString());
                var html1 = '<tr><td>Total</td><td colspan="4">' + rec + '</td></tr>';
                $("#rec-amts tbody").append(html1);
                var disc = 0;
                $.each(data.Discounts, function (i) {
                    disc = disc + data.Discounts[i].Discount_Amount;
                });
                disc = parseFloat(disc).toFixed(0);
                $("#disc-amt").text(Number(disc).toLocaleString());
                overduetotal = totalamt;
                overduetotal = parseFloat(overduetotal).toFixed(0);
                $("#due-amt").text(Number(overduetotal).toLocaleString())
                var overdue = parseFloat(overduetotal);
                if (data.PlotData.Registry == true) {
                    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">Registry Plot</div></div>';
                    $("#plot-trans-alert").append(html);
                }
                if (data.PlotData.Status == "Disputed") {
                    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is DISPUTED</div></div>';
                    $("#plot-trans-alert").append(html);
                    return false;
                }
                if (overduetotal > 0) {
                    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot Can not be Transfer Due to pending OVER DUE Amount</div></div>';
                    $("#plot-trans-alert").append(html);
                    if (data.PlotData.Verified != true) {
                        var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is not Verified</div></div>' +
                            '<div class="col-md-12"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6>' +
                            '<label class="switch"><input type="checkbox" id="veri-req"><span class="slider round"></span></label></div>';
                        $("#plot-trans-alert").append(html);
                    }
                }
                else {
                    if (data.PlotData.Verified == true) {
                        var btn = '<button class="btn btn-primary plt-trans-btn-dhjksf" type="submit" id="trans-frm" style="width:100%" >Generate Transfer Request</button>';
                        $("#trns-btn").html(btn);
                    }
                    else {
                        var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is not Verified</div></div>' +
                            '<div class="col-md-12"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6>' +
                            '<label class="switch"><input type="checkbox" id="veri-req"><span class="slider round"></span></label></div>';
                        $("#plot-trans-alert").append(html);
                    }
                }
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
//
$(document).on("click", ".De__Owner__from__plts", function () {
    var Ownerid = $(this).attr("data-id");
    var plotid = $("#plt-id").val();
    //var ch = confirm("Are you sure you want to delete this owner");
    //if (ch) {
    Swal.fire({
        text: 'Are you sure you want to delete the owner?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Plots/DeleteOwner/', { OwnerId: Ownerid, Plotid: plotid }, function (data) {
                //alert('Owner deleted successfully');
                Swal.fire({
                    icon: 'success',
                    text: 'Owner deleted successfully'
                }).then(() => {
                    window.location.reload();
                })
            });
        }
    });
});
//Generate File Request 
$(document).on("submit", "#trans-file-req", function (e) {
    e.preventDefault();
    if (confirm("Are you sure you want to generate Plot Transfer Request")) {
        $.ajax({
            type: "POST",
            url: $("#trans-file-req").attr('action'),
            data: $("#trans-file-req").serialize(),
            success: function (data) {
                alert("Request Form Generated");
                window.location.reload();
                window.open("/FileSystem/NDCForm?SerialNum=" + data, "_blank")
            },
            error: function () {
            }
        });
    }
});
//Send Back Request 
$(document).on("click", ".send-bk-req", function (e) {
    e.preventDefault();
    var id = $(this).data("reqid");
    var mod = $(this).data("mod");
    if (confirm("Are you sure you want to Send Back Request to Manager")) {
        $.ajax({
            type: "POST",
            url: '/Transfer/SendBackToManager/',
            data: { Id: id, Module: mod },
            success: function (data) {
                alert("Sent Back to Manager")
                window.location.reload();
            },
            error: function () {
            }
        });
    }
});
//Generate plot Request 
//$(document).on("submit", "#trans-plot-req", function (e) {
//    e.preventDefault();
//    var ch = confirm('Are you sure you want to transfer this plot');
//    if (ch) {
//        $.ajax({
//            type: "POST",
//            url: $("#trans-plot-req").attr('action'),
//            data: $("#trans-plot-req").serialize(),
//            success: function (data) {
//                alert("Tranfer Request Generated");
//                window.location.reload();
//                window.open("/Transfer/PlotNDCForm?SerialNum=" + data, '_Blank');
//            }
//        });
//    }
//});
//Generate plot Request 
$(document).on("click", ".f-tran-paper", function (e) {
    var id = $(this).attr("id");
    //if (confirm("Are you sure you want to Generate Letter")) {
    Swal.fire({
        text: 'Are you sure you want to generate the transfer paper?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            window.open("/Transfer/FileTransferLetter?Id=" + id, '_blank');
        }
    });
});
//Generate plot Request 
$(document).on("click", ".p-tran-paper", function (e) {
    var id = $(this).attr("id");
    //if (confirm("Are you sure you want to Generate Letter")) {
    Swal.fire({
        text: 'Are you sure you want to generate the transfer paper?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            window.open("/Transfer/PlotsTransferLetter?Id=" + id, '_blank');
        }
    });
});
// List of all Track list
$(document).on("click", ".trans-det", function () {
    var id = $(this).attr("id");
    window.location = "/FileSystem/FileTransferRequestDetails?Id=" + id;
});
// List of all Plots Transfer list
$(document).on("click", ".plt-trans-det", function () {
    var id = $(this).attr("id");
    window.location = "/Transfer/PlotTransferDetails?Id=" + id;
});
//
$(document).on("change", ".rec-pres", function () {
    var $this = $(this)
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    var prop = $(this).data("checklst");
    $.post('/FileSystem/CheckList/', { ReqId: id, Updateprop: prop, Status: chkstat }, function (data) {
        if (!data) {
            alert("System Not Responding");
            $(this).removeAttr("checked")
        }
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="button" id="transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    }).fail(function () {
        alert("System Not Responding");
        $($this).removeAttr("checked")
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="submit" id="transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    });
});
//
$(document).on("change", ".rec-plt-pres", function () {
    var $this = $(this)
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    var prop = $(this).data("checklst");
    $.post('/Transfer/PlotCheckList/', { ReqId: id, Updateprop: prop, Status: chkstat }, function (data) {
        if (!data) {
            alert("System Not Responding");
            $(this).removeAttr("checked")
        }
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="button" id="plt-transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    }).fail(function () {
        alert("System Not Responding");
        $($this).removeAttr("checked")
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="submit" id="plt-transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    });
});
// 
$(document).on("click", "#transf-veri", function () {
    var tranreqid = $("#req-id").val();
    var blod_rel = $("#blod-rel").val();
    var wave = $("#Trans-val").val();
    var othtranval = $("#oth-Trans-val").val();
    var rate = $("#amt").val();
    var remarks = $("#remarks").val();
    //if (confirm("Are you sure you want to approve the Transfer Request")) {
    Swal.fire({
        text: 'Are you sure you want to approve the transfer request?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (!result.isConfirmed) {
            $.post('/FileSystem/OkForTransfer/', { Reqid: tranreqid, Blood_rel: blod_rel, Wave_off: wave, OtherTransferCharges: othtranval, Rate: rate, Remarks: remarks }, function (data) {
                if (data == true) {
                    //alert("Ready for Transfer");
                    Swal.fire({
                        icon: 'success',
                        text: 'Transfer request processed successfully'
                    });
                }
            }).fail(function () {
                //alert("System Not Responding");
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            });
        }
    });
});
// 
$(document).on("click", "#plt-transf-veri", function () {
    var Plot_Id = $("#Plot_Id").val();
    var tranreqid = $("#req-id").val();
    var blod_rel = $("#blod-rel").val();
    var wave = $("#Trans-val").val();
    var othtranval = $("#oth-Trans-val").val();
    var rate = $("#amt").val();
    var remarks = $("#remarks").val();
    if (confirm('Are you sure you want to approve the transfer')) {
        $.post('/Transfer/PlotOkForTransfer/', { Plot_Id: Plot_Id, Reqid: tranreqid, Blood_rel: blod_rel, Wave_off: wave, OtherTransferCharges: othtranval, Rate: rate, Remarks: remarks }, function (data) {
            if (data == true) {
                alert("Ready for Transfer");
            }
        }).fail(function () {
            alert("System Not Responding");
        });
    }
});
//
$(document).on("click", "#file-tran-req-det", function () {
    var fileno = $("#req-file-no").val();
    if (fileno) {
        $("#tran-data").empty();
        $("#tran-data").load("/FileSystem/FileTransferRequestData/", { Id: fileno });
    }
    else {
        Swal.fire({
            icon: 'info',
            text: 'Enter file number to proceed'
        });
    }
});
//
$(document).on("click", "#plt-tran-req-det", function () {
    var id = $("#plot-details").val();
    if (id) {
        $("#tran-data").empty();
        $("#tran-data").load("/Transfer/PlotTransferData/", { Id: id });
    }
    else {
        Swal.fire({
            icon: 'info',
            text: 'Select plot to proceed'
        });
    }
});
//...................Get Project Types/ Floor
$(document).on("change", ".proj-types", function () {
    var id = $("#Projects").val();
    $('#phase-type').empty();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id },
        url: "/Commercial/GetCommercialFloors/",
        success: function (data) {
            $('#phase-type').append('<option value="">Choose ..</option>');
            $.each(data, function (key, value) {
                $('#phase-type').append('<option value=' + value.Id + '>' + value.Floor + '</option>');
            });
        },
        error: function () {
        }
    });
});
//....get shops of phase
$(document).on("change", ".prj-phase", function () {
    var FloorId = $("#phase-type").val();
    $('#shp-lst').empty();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: FloorId },
        url: "/Commercial/GetCommercial_Rooms/",
        success: function (data) {
            $('#shp-lst').append('<option value="">Select Unit</option>');
            $.each(data, function (key, value) {
                if (value.Project_Name == "SA Premium Homes") {
                    $('#shp-lst').append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
                }
                else {
                    $('#shp-lst').append('<option value=' + value.Id + '>' + value.Com_App_Shop_Number + '</option>');
                }
            });
        },
        error: function () {
        }
    });
});
//........... get detail of project shop
// Need to Change id.
$(document).on("change", "#shp-lst", function () {
    var id = $(this).val();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Commercial_Id: id },
        url: "/Commercial/GetCommercial_RoomsDetail/",
        success: function (data) {
            $("#shop-alert").empty();
            if (data == null) {
                alert("No Record Found");
                return False;
            }
            if (data[0].Status == 'Registered') {
                var html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Unit is Already Registered</div>';
                $("#shop-alert").prepend(html);
                $("#reg-btn").hide();
            }
            else if (data[0].Status == 'Hold') {
                var html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Unit is Holded By Company</div>';
                $("#shop-alert").prepend(html);
                $("#reg-btn").hide();
            }
            else {
                $("#reg-btn").show();
            }
            $("#shp-no").text(data[0].shop_no);
            $('#shp-id').val(id);
            $("#shp-size").text(data[0].Total_SqFt_Marlas);
            $('#sp-size').text(data[0].Total_SqFt_Marlas);
            $('#shop-type').text(data[0].Type);
            $('#shop-status').text(data[0].Status);
            $('#shp-loc').text(data[0].Location);
            //$('#plt-id').val(data.Id);
            //$('#pl-size').val(data.Plot_Size);
        },
        error: function () {
        }
    });
});
// get Plots of Blocks
$(document).on("change", ".blk-plts", function () {
    var id = $("#Block").val();
    var type = $('#plot-type').val();
    $('#plt-lst').empty();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id, Type: type },
        url: "/Plots/GetPlots/",
        success: function (data) {
            $('#plt-lst').append('<option value="">Select Plot</option>');
            $.each(data, function (key, value) {
                $('#plt-lst').append('<option value=' + value.Id + '>' + value.Plot_No + '</option>');
            });
        },
        error: function () {
        }
    });
});
// Get Plot Data
$(document).on("click", ".sec-plt-lst", function () {
    debugger;
    var id = $('#plt-lst').val();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id },
        url: "/Plots/GetPlotData/",
        success: function (data) {
            $("#file-trans-alert").empty();
            if (data == null) {
                alert("No Record Found");
                return False;
            }
            if (data.Status != 'Available_For_Sale') {
                var html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot cannot be Register</div>';
                $("#file-trans-alert").prepend(html);
                $("#reg-btn").hide();
            }
            else {
                $("#reg-btn").show();
            }
            $('#plt-no').text(data.Plot_No);
            $('#plt-size').text(data.Plot_Size);
            $('#plt-dim').text(data.Dimension);
            $('#plt-type').text(data.Type);
            $('#plt-status').text(data.Develop_Status);
            $('#plt-area').text(data.Area + " Sq-ft");
            $('#plt-road').text(data.Road_Type);
            $('#plt-loc').text(data.Plot_Location);
            $('#plt-id').val(data.Id);
            $('#pl-size').val(data.Plot_Size);
            $('#dealership').text(data.Dealership_Name);

        },
        error: function () {
        }
    });
});
//Register Plot
$(document).on("click", ".register-plot", function (e) {
    debugger;
    e.preventDefault();
    //To disable after click
    $(this).prop("disabled", true);
    var trans = $('#transaction-id').val();
    var DealersId = $('#Dealers').val();
    var availbal = -(Number(RemoveComma($('#avl-bal').text())));
    var pltpric = Number(RemoveComma($('#plt-pric').text()));
    if (availbal < pltpric) {
        //alert("Plot Cannot be Register Due to Low Dealer Balance");
        Swal.fire({
            icon: 'info',
            text: "Unable to Register Plot Due to Insufficient Dealer Balance"
        });
        return false;
    }
    var own = [];
    $('.Tran-own').each(function () {
        var plotowndata = {
            Id: 0,
            Name: $(this).find("input[name=Name]").val(),
            Father_Husband: $(this).find("input[name=Father_Husband]").val(),
            Postal_Address: $(this).find("input[name=Postal_Address]").val(),
            Residential_Address: $(this).find("input[name=Residential_Address]").val(),
            Phone_Office: $(this).find("input[name=Phone_Office]").val(),
            Residential: $(this).find("input[name=Residential]").val(),
            Mobile_1: $(this).find("input[name=Mobile_1]").val(),
            Mobile_2: $(this).find("input[name=Mobile_2]").val(),
            Ownership_DateTime: $(this).find("input[name=Ownership_DateTime]").val(),
            Email: $(this).find("input[name=Email]").val(),
            Occupation: $(this).find("input[name=Occupation]").val(),
            Nationality: $(this).find("input[name=Nationality]").val(),
            Date_Of_Birth: $(this).find("input[name=Date_Of_Birth]").val(),
            CNIC_NICOP: $(this).find("input[name=CNIC_NICOP]").val(),
            Nominee_Name: $(this).find("input[name=Nominee_Name]").val(),
            Nominee_Relation: $(this).find("input[name=Nominee_Relation]").val(),
            Nominee_Address: $(this).find("input[name=Nominee_Address]").val(),
            Nominee_CNIC_NICOP: $(this).find("input[name=Nominee_CNIC_NICOP]").val(),
            Plot_Id: $("#plt-id").val(),
            City: $("#City option:selected").val(),
            //City: $("#City").val(),
            Currency_Note_No: $(this).find("input[name=CN]").val(),
            Total_Price: Number(RemoveComma($('#plt-pric').text()))
        }
        own.push(plotowndata)
    });

    //debugger
    //var PaymentT = $('#cah-chq-bak').val();
    //var BAmount = $('#Amount').val();
    //var Ch_D_ONo = $('.paymentotherinfo #paymenttype').val();
    //var Datetime = $('#cbp-date').val();
    //var bank = $('#Bank').val();
    //var branch = $('#Branch').val();

    var rd = {
        PaymentType: $('#cah-chq-bak').val(),
        Amount: $('#Amount').val(),
        PayChqNo: $('.paymentotherinfo #paymenttype').val(),
        Ch_bk_Pay_Date: $('#cbp-date').val(),
        Bank: $('#Bank').val(),
        Branch: $('#Branch').val()
    }
    //debugger
    var data = { Owners: own, Plot_Id: $("#plt-id").val(), TransactionId: trans, isPayment: true, DealersId: DealersId, brdd: rd };
    //var conf = confirm("Are You Want to Submit Owners Informations");
    //if (conf) {
    Swal.fire({
        //title: 'Are you sure you want to Register the Plot?',
        text: 'Are you sure you want to Register the Plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Plots/RegisterDealerPlot/',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.Status) {
                        //alert("Plot Registered")
                        Swal.fire({
                            icon: 'success',
                            title: 'Success!',
                            text: "Plot has been Registered Successfully"
                        });
                        window.open("/Banking/PlotReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        window.location.reload();
                    }
                    else {
                        //alert(data.Msg)
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: data.Msg
                        });
                    }

                }
            });
        }
    });
});

// Get Plot Data
$(document).on("change", ".plt-reg-lst", function () {
    var id = $(this).val();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id },
        url: "/Plots/GetPlotData/",
        success: function (data) {
            $("#file-trans-alert").empty();
            if (data == null) {
                alert("No Record Found");
                return False;
            }
            if (data.Status == 'Registered') {
                var html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Already Registered</div>';
                $("#file-trans-alert").prepend(html);
                $("#reg-btn").hide();
            }
            else {
                $("#reg-btn").show();
            }
            $('#plt-no').text(data.Plot_No);
            $('#plt-size').text(data.Plot_Size);
            $('#plt-dim').text(data.Dimension);
            $('#plt-type').text(data.Type);
            $('#plt-status').text(data.Develop_Status);
            $('#plt-area').text(data.Area + " Sq-ft");
            $('#plt-road').text(data.Road_Type);
            $('#plt-loc').text(data.Plot_Location);
            $('#plt-id').val(data.Id);
            $('#pl-size').val(data.Plot_Size);
            var size = data.Plot_Size.split(' ');
            var grandtotal = size[0] * data.RatePerMarla;
            var advance = (grandtotal * 25) / 100;
            $("#adv-pmt").text(Number(advance).toLocaleString());
            $("#adv-pmt-hid").val(advance);
            var plotratecoma = (grandtotal == null) ? "" : grandtotal.toString();
            plotratecoma = Number(plotratecoma).toLocaleString();
            $('#gran-cal').val(plotratecoma);
        },
        error: function () {
        }
    });
});
// Generate Plot Installments
$(document).on("click", "#gen-ins", function () {
    plotinstdata = [];
    var total_price = RemoveComma($("#plt-pric").val());
    if (total_price == "" || total_price == null) {
        $("#plt-pric").focus();
        //alert("Enter Total Price");
        Swal.fire({
            icon: 'info',
            text: 'Enter total price to proceed'
        });
        return false;
    }
    var dis_pric = RemoveComma($("#dis-amt").val());
    if (dis_pric == "" || dis_pric == null) {
        $("#dis-amt").val(0)
    }
    var inst = $("#ttl-ins").val();
    if (inst == "" || inst == null) {
        $("#ttl-ins").focus();
        //alert("Enter Number of Installments");
        Swal.fire({
            icon: 'info',
            text: 'Enter number of installments to proceed'
        });
        return false;
    }
    var adva = RemoveComma($("#adv-amt").val());
    if (adva == "" || adva == null) {
        $("#adv-amt").focus();
        //alert("Enter Advance Amounts");
        Swal.fire({
            icon: 'info',
            text: 'Enter advance amount to proceed'
        });
        return false;
    }
    var regdate = $("#reg-date").val();
    if (regdate == "" || regdate == null) {
        $("#reg-date").focus();
        //alert("Enter Registeration Date");
        Swal.fire({
            icon: 'info',
            text: 'Enter registration date to proceed'
        });
        return false;
    }
    var instdate = $("#inst-date").val();
    if (instdate == "" || instdate == null) {
        var date = $("#reg-date").val();
        instdate = moment(date).add(1, 'M');
    }
    var currentDate = moment(instdate);
    var netamt = total_price - dis_pric;
    var remain = netamt - adva;
    if (remain > 0) {
        var instamt = parseFloat(remain / inst).toFixed(2);
        $('#all-instments tbody').empty();
        for (var i = 0; i < inst; i++) {
            var ploinst = { InstNo: "", Amount: "", DueDate: "" };
            var srno = i + 1;
            var a = srno + 1;
            var futureMonth = moment(currentDate).add(i, 'M');
            var html = '<tr>' +
                '<td width="5%">' + a + '</td>' +
                '<td width="50%">' + moment(futureMonth).format("MMMM YYYY") + ', ' + srno + ' Installment</td>' +
                '<td width="25%">' + Number(instamt).toLocaleString() + ' </td > ' +
                '<td width="20%">' + moment(futureMonth).format("DD-MMM-YYYY") + '</td>' +
                '</tr>';
            ploinst.Amount = instamt;
            ploinst.InstNo = srno + " Installment";
            ploinst.DueDate = moment(futureMonth).format("MM/DD/YYYY");
            plotinstdata.push(ploinst);
            $('#all-instments tbody').append(html);
        }
    }
    var ploinst1 = { InstNo: "", Amount: "", DueDate: "" };
    var html = '<tr>' +
        '<td width="5%">1</td>' +
        '<td width="50%">' + moment(regdate).format("MMMM YYYY") + ' Advance</td>' +
        '<td width="25%">' + Number(adva).toLocaleString() + '</td > ' +
        '<td width="20%">' + moment(regdate).format("DD-MMM-YYYY") + '</td>' +
        '</tr>';
    ploinst1.Amount = adva;
    ploinst1.InstNo = "Advance";
    ploinst1.DueDate = regdate;
    plotinstdata.push(ploinst1);
    $('#all-instments tbody').prepend(html);
    $(".own-info").show();
});
// plot installments information
$(document).on("click", ".plot-inst-btn", function () {
    var div = $(this).closest(".own-det")
    var id = $("#plt-id").val();
    var mob = $(div).find("input[name=Mobile_1]").val()
    var fth = $(div).find("input[name=Father_Husband]").val();
    var nam = $(div).find("input[name=Name]").val();
    EmptyModel();
    $('.modal-body').load('/Installments/AddPlotInstallment/', { Id: id, Name: nam, Father: fth, Mobile: mob }, function () {
    });
});
//... Commercial Installments Information
$(document).on("click", ".com-inst-btn", function () {
    var div = $(this).closest(".own-det")
    var id = $("#shp-id").val();
    var mob = $(div).find("input[name=Mobile_1]").val();
    var fth = $(div).find("input[name=Father_Husband]").val();
    var nam = $(div).find("input[name=Name]").val();
    EmptyModel();
    $('.modal-body').load('/Installments/AddCommercialInstallment/', { Id: id, Name: nam, Father: fth, Mobile: mob }, function () {
    });
});
// Add Profile Image
$(document).on("click", ".add-own-img", function () {
    EmptyModel();
    var id = $(this).closest(".own-det").attr("id");
    var ecode = $(this).closest(".own-det").attr("data-emp");
    var ref = $(this).attr("id");
    $('#ModalLabel').text("Setup Employee Profile Image");
    $('.modal-body').load('/Home/GetUserImage/', { Id: id, Ref: ref, empCode: ecode }, function () { });
});
// Save Owner Image
$(document).on("click", "#sav-own-img", function () {
    var src = $("#snap-photo").attr("src");
    var ref = $(this).data("ref");
    if (src == null || src == "") {
        src = $(".deal-img").attr("src");
    }
    var id = $(this).data("ids");
    $("#" + id + " #" + ref).siblings("img").attr("src", src);
    ShutCam();
});
//Close Camera
function ShutCam() {
    Webcam.reset();
    Webcam.off();
}
// Add Plot Transfer to 
$(document).on("click", "#trans-btn", function () {
    plotowncount++;
    var html = '<div class="own-det" id="own-' + plotowncount + '" style="margin-top:5%"><h6 class="c-grey-900">' + plotowncount + ' Owner Information</h6><div class="form-row row close-info"><div class="col-md-10"><div class="form-row">' +
        '<div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Name" required></div>' +
        '<div class="form-group col-md-3"><label>Father / Husband Name</label > <input type="text" class="form-control" name="Father_Husband" required></div>' +
        '<div class="form-group col-md-3"><label>CNIC / NICOP</label><input type="text" class="form-control" name="CNIC_NICOP" placeholder="12345-1234567-1   or   123456-123456-1" required></div>' +
        '<div class="form-group col-md-2"><label>Date Of Birth</label><input type="text" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" name="Date_Of_Birth" required></div>' +
        '<div class="form-group col-md-6"><label>Postal Address</label><input type="text" class="form-control" name="Postal_Address" placeholder="1234 Main St" required></div>' +
        '<div class="form-group col-md-6"><label>Residential Address</label><input type="text" class="form-control" name="Residential_Address" placeholder="Apartment, Plot, or floor" required></div>' +
        '<div class="form-group col-md-2"><label>City</label><select class="form-control" name="City">' + citylist + '</select></div>' +
        '<div class="form-group col-md-3"><label>Occupation</label><input type="text" class="form-control" name="Occupation"></div>' +
        '<div class="form-group col-md-2"><label>Nationality</label><input type="text" class="form-control" name="Nationality"></div>' +
        '<div class="form-group col-md-3"><label>Email</label><input type="text" class="form-control" name="Email"></div></div>' +
        '<div class="form-row"><div class="form-group col-md-3"><label>Phone Office</label><input type="text" class="form-control" name="Phone_Office"></div>' +
        '<div class="form-group col-md-3"><label>Residential</label><input type="text" class="form-control" name="Residential"></div>' +
        '<div class="form-group col-md-3"><label>Mobile 1</label><input type="text" class="form-control" placeholder="03121234567" name="Mobile_1" required></div>' +
        '<div class="form-group col-md-3"><label>Mobile 2</label><input type="text" class="form-control" pattern="^0\d{10}" placeholder="03121234567" name="Mobile_2"></div></div>' +
        '</div><div class="col-md-2"><input type="button" class="btn btn-info plot-inst-btn" data-toggle="modal" data-target="#Modal" value="Receive Installments" />' +
        '<img style="margin-top:10px" src="/assets/static/images/no-img.png" width="200" height="200" id="own_img" />' +
        '<input type="button" class="btn btn-info add-own-img" id="add-own-img" style="margin-top:10px" data-toggle="modal" value="Add Image" data-target="#Modal" /></div></div>' +
        '<h6 class="c-grey-900">Nominee</h6><div class="form-row"><div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Nominee_Name"></div>' +
        '<div class="form-group col-md-2"><label>CNIC / NICOP</label><input type="text" class="form-control" placeholder="12345-1234567-1 or 123456-123456-1" name="Nominee_CNIC_NICOP"></div>' +
        '<div class="form-group col-md-2"><label>Relation</label><input class="form-control" name="Nominee_Relation" placeholder=""></div><div class="form-group col-md-5">' +
        '<label>Address</label><input type="text" class="form-control" name="Nominee_Address" placeholder="1234 Main St"></div></div><button style="margin:5px" id="re-oth-plt-own" class="btn btn-primary">Register</button></div>';
    $("#trans-btn").hide();
    $(".Tran-own").append(html);
});
//.....Add Shop Transfer to...
$(document).on("click", "#com-trans-btn", function () {
    plotowncount++;
    var html = '<div class="own-det" id="own-' + plotowncount + '" style="margin-top:5%"><h6 class="c-grey-900">' + plotowncount + ' Owner Information</h6><div class="form-row row close-info"><div class="col-md-10"><div class="form-row">' +
        '<div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Name" required></div>' +
        '<div class="form-group col-md-3"><label>Father / Husband Name</label > <input type="text" class="form-control" name="Father_Husband" required></div>' +
        '<div class="form-group col-md-3"><label>CNIC / NICOP</label><input type="text" class="form-control" name="CNIC_NICOP" placeholder="12345-1234567-1   or   123456-123456-1" required></div>' +
        '<div class="form-group col-md-2"><label>Date Of Birth</label><input type="text" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" name="Date_Of_Birth" required></div>' +
        '<div class="form-group col-md-6"><label>Postal Address</label><input type="text" class="form-control" name="Postal_Address" placeholder="1234 Main St" required></div>' +
        '<div class="form-group col-md-6"><label>Residential Address</label><input type="text" class="form-control" name="Residential_Address" placeholder="Apartment, Plot, or floor" required></div>' +
        '<div class="form-group col-md-2"><label>City</label><select class="form-control" id="city" name="City">' + citylist + '</select></div>' +
        '<div class="form-group col-md-3"><label>Occupation</label><input type="text" class="form-control" name="Occupation"></div>' +
        '<div class="form-group col-md-2"><label>Nationality</label><input type="text" class="form-control" name="Nationality"></div>' +
        '<div class="form-group col-md-3"><label>Email</label><input type="text" class="form-control" name="Email"></div></div>' +
        '<div class="form-row"><div class="form-group col-md-3"><label>Phone Office</label><input type="text" class="form-control" name="Phone_Office"></div>' +
        '<div class="form-group col-md-3"><label>Residential</label><input type="text" class="form-control" name="Residential"></div>' +
        '<div class="form-group col-md-3"><label>Mobile 1</label><input type="text" class="form-control" placeholder="03121234567" name="Mobile_1" required></div>' +
        '<div class="form-group col-md-3"><label>Mobile 2</label><input type="text" class="form-control" pattern="^0\d{10}" placeholder="03121234567" name="Mobile_2"></div></div>' +
        '</div><div class="col-md-2"><input type="button" class="btn btn-info com-inst-btn" data-toggle="modal" data-target="#Modal" value="Receive Installments" />' +
        '<img style="margin-top:10px" src="/assets/static/images/no-img.png" width="200" height="200" id="own_img" />' +
        '<input type="button" class="btn btn-info add-own-img" id="add-own-img" style="margin-top:10px" data-toggle="modal" value="Add Image" data-target="#Modal" /></div></div>' +
        '<h6 class="c-grey-900">Nominee</h6><div class="form-row"><div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Nominee_Name"></div>' +
        '<div class="form-group col-md-2"><label>CNIC / NICOP</label><input type="text" class="form-control" placeholder="12345-1234567-1 or 123456-123456-1" name="Nominee_CNIC_NICOP"></div>' +
        '<div class="form-group col-md-2"><label>Relation</label><input class="form-control" name="Nominee_Relation" placeholder=""></div><div class="form-group col-md-5">' +
        '<label>Address</label><input type="text" class="form-control" name="Nominee_Address" placeholder="1234 Main St"></div></div><button style="margin:5px" id="re-oth-com-own" class="btn btn-primary re-oth-com-own">Register</button></div>';
    $("#com-trans-btn").hide();
    $(".Tran-own").append(html);
});
//....submit commercial ownership data (ayan center)
$(document).on("submit", "#re-shp", function (e) {
    e.preventDefault();
    var dis_pric = $("#dis-amt").val();
    if (dis_pric == "" || dis_pric == null) {
        $("#dis-amt").val(0)
    }
    var img = $("#own_img").attr("src")
    var comowndata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Total_Price: $("#plt-pric").val(),
        Discount: $("#dis-amt").val() || 0,
        ComRom_Id: $("#shp-id").val(),
        Owner_Img: img,
        City: $("#City").val(),
        DateTime: $("#reg-date").val(),
    }
    var data = { comdata: comowndata, PI: plotinstdata };
    var conf = confirm("Are You Want to Submit Owners Informations");
    if (conf) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: $("#re-shp").attr('action'),
            data: JSON.stringify(data),
            success: function (data) {
                alert("Shop Registered")
                $("#reg-btn").hide();
                $("#com-trans-btn").show();
            }
        });
    }
});
//submit plot Ownership data
$(document).on("submit", "#re-plt", function (e) {
    e.preventDefault();
    var dis_pric = $("#dis-amt").val();
    if (dis_pric == "" || dis_pric == null) {
        $("#dis-amt").val(0)
    }
    var img = $("#own_img").attr("src")
    var plotowndata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Total_Price: $("#plt-pric").val(),
        Discount: $("#dis-amt").val() || 0,
        Plot_Id: $("#plt-id").val(),
        Owner_Img: img,
        City: $("#City").val(),
    }
    var Dealercom = {
        Dealer_Id: $("#Dealership").val(),
        Dealer_Name: $("#Dealership :selected").text(),
        Com_Type: $(".com-typ").val(),
        Com_Amount: RemoveComma($(".com-amt").val()),
        Percentage: RemoveComma($(".per-val").val()),
        Com_Maturity: RemoveComma($(".com-mat").val())
    };
    var data = { PO: plotowndata, PI: plotinstdata, Img: img, Com: Dealercom };
    var conf = confirm("Are You Want to Submit Owners Informations");
    if (conf) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: $("#re-plt").attr('action'),
            data: JSON.stringify(data),
            success: function (data) {
                alert("Plot Registered")
                $("#reg-btn").hide();
                $("#trans-btn").show();
            }
        });
    }
});
//
$(document).on("click", ".re-oth-com-own", function (e) {
    e.preventDefault();
    var id = $(this).closest(".own-det").attr("id")
    var img = $("#" + id + " #own_img").attr("src")
    var shopowndata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("#" + id + " input[name=Name]").val(),
        Father_Husband: $("#" + id + " input[name=Father_Husband]").val(),
        Postal_Address: $("#" + id + " input[name=Postal_Address]").val(),
        Residential_Address: $("#" + id + " input[name=Residential_Address]").val(),
        Phone_Office: $("#" + id + " input[name=Phone_Office]").val(),
        Residential: $("#" + id + " input[name=Residential]").val(),
        Mobile_1: $("#" + id + " input[name=Mobile_1]").val(),
        Mobile_2: $("#" + id + " input[name=Mobile_2]").val(),
        Email: $("#" + id + " input[name=Email]").val(),
        Occupation: $("#" + id + " input[name=Occupation]").val(),
        Nationality: $("#" + id + " input[name=Nationality]").val(),
        Date_Of_Birth: $("#" + id + " input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("#" + id + " input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("#" + id + " input[name=Nominee_Name]").val(),
        Nominee_Relation: $("#" + id + " input[name=Nominee_Relation]").val(),
        Nominee_Address: $("#" + id + " input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("#" + id + " input[name=Nominee_CNIC_NICOP]").val(),
        ComRom_Id: $("#shp-id").val(),
        Owner_Img: img,
        City: $("#" + id + " #city").val(),
        DateTime: $("#reg-date").val(),
    }
    var data = { comdata: shopowndata };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Commercial/CommercialTransferData/',
        data: JSON.stringify(data),
        success: function (data) {
            alert("Shop Transfered")
            $(".re-oth-com-own").remove();
            $("#com-trans-btn").show();
            $("#up-trans-btn").show();
        }
    });
});
//
$(document).on("click", "#re-oth-plt-own", function (e) {
    e.preventDefault();
    var id = $(this).closest(".own-det").attr("id")
    var img = $("#" + id + " #own_img").attr("src")
    var plotowndata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("#" + id + " input[name=Name]").val(),
        Father_Husband: $("#" + id + " input[name=Father_Husband]").val(),
        Postal_Address: $("#" + id + " input[name=Postal_Address]").val(),
        Residential_Address: $("#" + id + " input[name=Residential_Address]").val(),
        Phone_Office: $("#" + id + " input[name=Phone_Office]").val(),
        Residential: $("#" + id + " input[name=Residential]").val(),
        Mobile_1: $("#" + id + " input[name=Mobile_1]").val(),
        Mobile_2: $("#" + id + " input[name=Mobile_2]").val(),
        Email: $("#" + id + " input[name=Email]").val(),
        Occupation: $("#" + id + " input[name=Occupation]").val(),
        Nationality: $("#" + id + " input[name=Nationality]").val(),
        Date_Of_Birth: $("#" + id + " input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("#" + id + " input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("#" + id + " input[name=Nominee_Name]").val(),
        Nominee_Relation: $("#" + id + " input[name=Nominee_Relation]").val(),
        Nominee_Address: $("#" + id + " input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("#" + id + " input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Id: $("#plt-id").val(),
        Owner_Img: img,
        City: $("#" + id + " #City").val(),
    }
    var data = { PO: plotowndata };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Plots/PlotTransferData/',
        data: JSON.stringify(data),
        success: function (data) {
            alert("Plot Transfered")
            $("#re-oth-plt-own").remove();
            $("#trans-btn").show();
            $("#up-trans-btn").show();
        }
    });
});
//
$(document).on("click", "#re-up-oth-plt-own", function (e) {
    e.preventDefault();
    var id = $(this).closest(".own-det").attr("id")
    var img = $("#" + id + " #own_img").attr("src")
    var plotid = $("#plt-id").val();
    var plotowndata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("#" + id + " input[name=Name]").val(),
        Father_Husband: $("#" + id + " input[name=Father_Husband]").val(),
        Postal_Address: $("#" + id + " input[name=Postal_Address]").val(),
        Residential_Address: $("#" + id + " input[name=Residential_Address]").val(),
        Phone_Office: $("#" + id + " input[name=Phone_Office]").val(),
        Residential: $("#" + id + " input[name=Residential]").val(),
        Mobile_1: $("#" + id + " input[name=Mobile_1]").val(),
        Mobile_2: $("#" + id + " input[name=Mobile_2]").val(),
        Email: $("#" + id + " input[name=Email]").val(),
        Occupation: $("#" + id + " input[name=Occupation]").val(),
        Nationality: $("#" + id + " input[name=Nationality]").val(),
        Date_Of_Birth: $("#" + id + " input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("#" + id + " input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("#" + id + " input[name=Nominee_Name]").val(),
        Nominee_Relation: $("#" + id + " input[name=Nominee_Relation]").val(),
        Nominee_Address: $("#" + id + " input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("#" + id + " input[name=Nominee_CNIC_NICOP]").val(),
        Ownership_Status: $("#" + id + " input[name=Ownership_Status]").val(),
        Ownership_DateTime: $("#" + id + " input[name=Ownership_DateTime]").val(),
        Plot_Id: plotid,
        Owner_Img: img,
        City: $("#" + id + " #City").val(),
    }
    if (plotowndata.Name == null || plotowndata.Name.trim() == "") {
        Swal.fire({
            icon: 'info',
            text: 'Enter name to proceed'
        });
        return false;
    }
    else if (plotowndata.CNIC_NICOP == null || plotowndata.CNIC_NICOP.trim() == "") {
        Swal.fire({
            icon: 'info',
            text: 'Enter CNIC to proceed'
        });
        return false;
    }
    //else if (plotowndata.Date_Of_Birth == null || plotowndata.Date_Of_Birth.trim() == "") {
    //    return false;
    //}
    else if (plotowndata.Ownership_DateTime == null || plotowndata.Ownership_DateTime.trim() == "") {
        Swal.fire({
            icon: 'info',
            text: 'Select ownership date to proceed'
        });
        return false;
    }
    var data = { PO: plotowndata };
    Swal.fire({
        text: 'Are you sure you want to transfer the plot to this owner?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Plots/PlotTransferData/',
                data: JSON.stringify(data),
                success: function (data) {
                    //alert("Plot Transfered")
                    Swal.fire({
                        icon: 'success',
                        text: 'Plot transfered succesfully'
                    }).then(() => {
                        $(".Tran-own").load('/Plots/PlotsOwnersListUpadate/', { Id: plotid }, function () {
                            $("#up-trans-btn").show();
                        });
                    })
                }
            });
        }
    });
});
//...... Commercial Transfer Data
$(document).on("click", "#re-up-oth-com-own", function (e) {
    if (plotowncount < 2) {
        //alert('Please Add form of multiple owners');
        Swal.fire({
            icon: 'info',
            text: 'Kindly add a form for multiple owners'
        });
        return false;
    }
    var id = $(this).closest(".own-det").attr("id")
    var img = $("#" + id + " #own_img").attr("src")
    var Name = "", Postal_Address = "", Phone_Office = "", Residential = "", Mobile_1 = "", Mobile_2 = "", Email = "", Occupation = "", Nationality = "", Date_Of_Birth = "", CNIC_NICOP = "", Nominee_Name = "",
        Nominee_Relation = "", Nominee_Address = "", Nominee_CNIC_NICOP = "", Owner_Image2 = "", Owner_Image3 = "", Owner_Image4 = "", Block_Id = "", QR_Code = "", City = "", Residential_Address = "", Father_Husband = "";
    for (var r = 2; r <= plotowncount; r++) {
        Name = Name + 'Ѭ' + ($("#add-reg-" + r + " .Name").val() || '_');
        Father_Husband = Father_Husband + 'Ѭ' + ($("#add-reg-" + r + "  .Father_Husband").val() || '_');
        Postal_Address = Postal_Address + 'Ѭ' + ($("#add-reg-" + r + " #Postal_Address").val() || '_');
        Residential_Address = Residential_Address + 'Ѭ' + ($("#add-reg-" + r + "  .Residential_Address").val() || '_');
        Phone_Office = Phone_Office + 'Ѭ' + ($("#add-reg-" + r + " #Phone_Office").val() || '_');
        Residential = Residential + 'Ѭ' + ($("#add-reg-" + r + " #Residential").val() || '_');
        Mobile_1 = Mobile_1 + 'Ѭ' + ($("#add-reg-" + r + " #Mobile_1").val() || '_');
        Mobile_2 = Mobile_2 + 'Ѭ' + ($("#add-reg-" + r + " #Mobile_2").val() || '_');
        Email = Email + 'Ѭ' + ($("#add-reg-" + r + " #Email").val() || '_');
        Occupation = Occupation + 'Ѭ' + ($("#add-reg-" + r + " #Occupation").val() || '_');
        Nationality = Nationality + 'Ѭ' + ($("#add-reg-" + r + " #Nationality").val() || '_');
        Date_Of_Birth = Date_Of_Birth + 'Ѭ' + ($("#add-reg-" + r + " #Date_Of_Birth").val() || '_');
        CNIC_NICOP = CNIC_NICOP + 'Ѭ' + ($("#add-reg-" + r + " #CNIC_NICOP").val() || '_');
        Nominee_Name = Nominee_Name + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Name").val() || '_');
        Nominee_Relation = Nominee_Relation + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Relation").val() || '_');
        Nominee_Address = Nominee_Address + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Address").val() || '_');
        Nominee_CNIC_NICOP = Nominee_CNIC_NICOP + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_CNIC_NICOP").val() || '_');
        City = City + 'Ѭ' + ($("#add-reg-" + r + " .city").val() || '_');
        if (r == 2) {
            Owner_Image1 = $("#add-reg-" + r + " #own_img").attr('src');
        }
        if (r == 3) {
            Owner_Image2 = $("#add-reg-" + r + " #own_img").attr('src');
        }
        if (r == 4) {
            Owner_Image3 = $("#add-reg-" + r + " #own_img").attr('src');
        }
        if (r == 5) {
            Owner_Image4 = $("#add-reg-" + r + " #own_img").attr('src');
        }
    }
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: Name.substr(1),
        Father_Husband: Father_Husband.substr(1),
        Postal_Address: Postal_Address.substr(1),
        Residential_Address: Residential_Address.substr(1),
        Phone_Office: Phone_Office.substr(1),
        Residential: Residential.substr(1),
        Mobile_1: Mobile_1.substr(1),
        Mobile_2: Mobile_2.substr(1),
        Email: Email.substr(1),
        Occupation: Occupation.substr(1),
        City: City.substr(1),
        Nationality: Nationality.substr(1),
        Date_Of_Birth: Date_Of_Birth.substr(1),
        CNIC_NICOP: CNIC_NICOP.substr(1),
        Nominee_Name: Nominee_Name.substr(1),
        Nominee_Relation: Nominee_Relation.substr(1),
        Nominee_Address: Nominee_Address.substr(1),
        Nominee_CNIC_NICOP: Nominee_CNIC_NICOP.substr(1),
        OwnerCount: plotowncount - 1,
        ComRom_Id: $("#shp-id").val(),
        Owner_Image1: img,
        DateTime: $("#reg-date").val(),
        Owner_Image1: Owner_Image1,
        Owner_Image2: Owner_Image2,
        Owner_Image3: Owner_Image3,
        Owner_Image4: Owner_Image4,
    }
    var data = { comdata: regfiledata };
    Swal.fire({
        text: 'Are you sure you want to add another owner?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Commercial/CommercialTransferData/',
                data: JSON.stringify(data),
                success: function (data) {
                    //alert("Successfully Transfered")
                    Swal.fire({
                        icon: 'success',
                        text: 'Transfer owner added successfully'
                    }).then(() => {
                        window.location.reload();
                    })
                    //$(".Tran-own").load('/Plots/PlotsOwnersListUpadate/', { Id: plotid }, function () {
                    //    $("#up-trans-btn").show();
                    //});
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    })
                }
            });
        }
    });
});
//........Commercial Installments Receive
$(document).on("click", "#sav-com-rece", function (e) {
    e.preventDefault();
    var balamt = $("#bal-val").val();
    var amt = $("#amt").val();
    $('#gen-oth-rec').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var form = $("#pay-com-ins");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    Swal.fire({
        text: 'Are you sure you want to add manual installment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: $("#pay-com-ins").attr('action'),
                data: data,
                success: function (data) {
                    if (data.ReturnVal == 0) {
                        //alert("Reciept Already Exists")
                        Swal.fire({
                            icon: 'info',
                            text: 'The receipt already exists'
                        });
                    }
                    else {
                        $("#rec-amts tbody").empty();
                        //alert("Installment Added");
                        Swal.fire({
                            icon: 'success',
                            text: 'Installment successfully added'
                        });
                        var total = 0;
                        $.each(data.ReceAmt, function (i) {
                            var a = i + 1;
                            total = total + data.ReceAmt[i].Amount;
                            var html = '<tr id="' + data.ReceAmt[i].Id + '" data-token="' + data.ReceAmt[i].TokenParameter + '" >' +
                                '<td scope="row">' + a + '</td>' +
                                '<td scope="row">' + data.ReceAmt[i].ReceiptNo + '</td>' +
                                '<td>' + data.ReceAmt[i].Amount + '</td>' +
                                '<td>' + moment(data.ReceAmt[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                                '<td>' + data.ReceAmt[i].PaymentType + '</td><td><i class="ti-close del-rec"></i></td></tr>';
                            $("#rec-amts tbody").append(html);
                        });
                        var html1 = '<tr><td>Total</td><td colspan="5">' + total + '</td></tr>'
                        $("#rec-amts tbody").append(html1);
                        var id = $("#shp-id").val();
                        //  $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: id }, function () { });
                    }
                },
                error: function (xmlhttprequest, textstatus, message) {
                    $('#gen-rec').attr("disabled", false);
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
//
$(document).on("click", "#sav-plt-rece", function (e) {
    e.preventDefault();
    $(this).attr("disabled", true);
    var balamt = $("#bal-val").val();
    var amt = $("#amt").val();
    $('#gen-oth-rec').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var form = $("#pay-plot-ins");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: $("#pay-plot-ins").attr('action'),
                data: data,
                success: function (data) {
                    if (data.ReturnVal == 0) {
                        //alert("Reciept Already Exists")
                        Swal.fire({
                            icon: 'info',
                            text: 'The receipt already exists'
                        });
                    }
                    else {
                        $("#rec-amts tbody").empty();
                        //alert("Installment Added");
                        Swal.fire({
                            icon: 'success',
                            text: 'Receipt generated successfully'
                        }).then(() => {
                            var total = 0;
                            $.each(data.ReceAmt, function (i) {
                                var a = i + 1;
                                total = total + data.ReceAmt[i].Amount;
                                var html = '<tr id="' + data.ReceAmt[i].Id + '" data-token="' + data.ReceAmt[i].TokenParameter + '" >' +
                                    '<td scope="row">' + a + '</td>' +
                                    '<td scope="row">' + data.ReceAmt[i].ReceiptNo + '</td>' +
                                    '<td>' + data.ReceAmt[i].Amount + '</td>' +
                                    '<td>' + moment(data.ReceAmt[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                                    '<td>' + data.ReceAmt[i].PaymentType + '</td><td><i class="ti-close del-rec"></i></td></tr>';
                                $("#rec-amts tbody").append(html);
                            });
                            var html1 = '<tr><td>Total</td><td colspan="5">' + total + '</td></tr>'
                            $("#rec-amts tbody").append(html1);
                            var id = $("#plt-id").val();
                            $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: id }, function () { });
                        })
                    }
                },
                error: function (xmlhttprequest, textstatus, message) {
                    $('#gen-rec').attr("disabled", false);
                    $('#sav-plt-rece').attr("disabled", false);
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
// Adjust Installments
$(document).on("click", "#adj-inst", function () {
    $("#plot-rep").empty();
    var id = $("#plt-id").val();
    $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: id }, function () {
    });
});
//
$(document).on("change", ".blk-plts", function () {
    var id = $("#Block").val();
    var blk_nam = $("#Block :selected").text();
    if (id == -998) {
        //Road select ki hai user nay
        $(this).parent('.form-group').next('.form-group').next('.form-group').hide();
        $(this).parent('.form-group').next('.form-group').children('.road_slctn').show();
        $(this).parent('.form-group').next('.form-group').children('.blk-plts').hide();
        return;
    }
    else if (id == -999 || id == -997 || id == -996 || id == -1000) {
        //Boundary wall select ki hai user nay
        $(this).parent('.form-group').nextAll('.form-group').hide();
        var Pltbndedid = $(".pltid").attr("id");
        var Pltbndedwith = $(this).val();
        var Block = '';
        var Block_name = '';
        var Position = $('#bnded_po__sel option:selected').val();
        var type = '';//$(id == -999) ? "Road" : "Boundary Wall";
        if (id == -998) {
            type = "Road";
        }
        else if (id == -999) {
            type = "Boundary Wall";
        } else if (id == -996) {
            type = "Park";
        }
        else if (id == -997) {
            type = "Public Building";
        }
        else if (id == -1000) {
            type = "Degh";
        }
        if (Position == "" || Position == null) {
            alert('Plese Select Postion');
            return false;
        }
        var dataset =
        {
            Plot_Id: Pltbndedid,
            BoundedPlotId: Pltbndedwith,
            Block: Block,
            Position: Position,
            Block_Name: Block_name,
            Plot_Type: 0,
        };
        $.post('/Plots/PlotBounding/', { B: dataset, isRoad: true }, function (data) {
            if (data.Status) {
                $("#bound-det-tab").load('/Plots/PlotBoundTabular/', { Plot_Id: Pltbndedid });
                $("#bound-det").load('/Plots/PlotBounded/', { Plot_Id: Pltbndedid });
                alert(data.Msg);
            }
            else {
                alert(data.Msg);
            }
        });
        return;
    }
    else {
        $(this).parent('.form-group').nextAll('.form-group').show();
        $(this).parent('.form-group').next('.form-group').children('.road_slctn').hide();
        $(this).parent('.form-group').next('.form-group').children('.blk-plts').show();
    }
    var type = $('#plot-type').val();
    $('#plot-details').empty();
    if (blk_nam == "Sher Afghan3") {
        $(".plt-field").hide();
        $(".file-field").show();
    }
    else {
        $(".plt-field").show();
        $(".file-field").hide();
        $.ajax({
            traditional: true,
            type: "POST",
            data: { Id: id, Type: type },
            url: "/Plots/GetPlots/",
            success: function (data) {
                $('#plot-details').append('<option value="">Select Plot</option>');
                $.each(data, function (key, value) {
                    $('#plot-details').append('<option value=' + value.Id + '>' + value.Plot_No + '</option>');
                });
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("change", ".blk-plots", function () {
    var id = $("#Block").val();
    var type = $('#plot-type').val();
    $('#plot-details').empty();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id, Type: type },
        url: "/Plots/GetPlots/",
        success: function (data) {
            $('#plot-details').append('<option value="">Select Plot</option>');
            $.each(data, function (key, value) {
                $('#plot-details').append('<option value=' + value.Id + '>' + value.Plot_No + '</option>');
            });
        },
        error: function () {
        }
    });
});

//
$(document).on("change", ".al-plt-det", function () {
    var id = $(this).val();
    $("#plot-det").load("/Plots/PlotVerification/", { Plotid: id }, function () {
    });
});
//
$(document).on("click", ".plt-details", function () {
    var id = $(this).data("id");
    SASLoad($("#plot-det"));
    $("#plot-det").load("/Plots/PlotDetails/", { Plotid: id }, function () {
        SASUnLoad($("#plot-det"));
        $('html, body').animate({
            scrollTop: $("#plot-det").offset().top
        }, 1000);
    });
});
//
$(document).on("click", ".com-details", function () {
    var id = $(this).data("id");
    $("#plot-det").load("/Commercial/CommercialInformationSearch/", { Commercial_Id: id }, function () {
        $('html, body').animate({
            scrollTop: $("#plot-det").offset().top
        }, 1000);
    });
});
//
$(document).on("click", "#ver-all", function () {
    var id = $("#plt-id").val();
    var ownid = $("#own-id").val();
    var overdueamt = parseFloat($("#ov-du-amt").val()).toFixed(0);
    if (overdueamt > 0) {
        alert("Cannot Generate Allotment Letter Because of OVER DUE AMOUNT");
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/Plots/VerificationforAllotment/',
        data: { Id: id, PlotOwnId: ownid },
        success: function (data) {
            alert("Allotment Letter is gone for finalization");
            $("#ver-all").prop("Disabled", true);
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
$(document).on("click", ".allotment-let", function () {
    EmptyModel();
    var id = $(this).closest('tr').attr("id");
    var html = '<div class="form-row"><input type="hidden" id="plot-id" value="' + id + '"><div class="form-group col-md-4"><label class="">1st Witness</label><input type="text" value="Naqash Ishtiaq" class="form-control" id="wit-1" /></div>' +
        '<div class="form-group col-md-4"><label class="">2nd Witness</label><input type="text" class="form-control" id="wit-2" value="Zarghoona Qureshi" /></div>' +
        '<div class="form-group col-md-4"><label>------------</label><select class="form-control" id="head-nam"><option value="Chief Executive">Shoaib Afzal Malik</option><option value="Chairman">Sohail Afzal Malik</option></select></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="gen-allot-let" type="submit">Generate Allotment Letter</button>');
});
//
$(document).on("click", ".ver-allotment-let", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var html = '<div class="form-row"><input type="hidden" id="plot-id" value="' + id + '"><div class="form-group col-md-4"><label class="">1st Witness</label><input type="text" value="Naqash Ishtiaq" class="form-control" id="wit-1" /></div>' +
        '<div class="form-group col-md-4"><label class="">2nd Witness</label><input type="text" class="form-control" id="wit-2" value="Zarghoona Qureshi" /></div>' +
        '<div class="form-group col-md-4"><label>------------</label><select class="form-control" id="head-nam"><option value="Chief Executive">Shoaib Afzal Malik</option><option value="Chairman">Sohail Afzal Malik</option></select></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="gen-allot-let" type="submit">Generate Allotment Letter</button>');
});
//
$(document).on("click", ".plt__post__up", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Update Positions");
    $('#modalbody').load('/Plots/PlotPosition/', { Plotid: id });
});
//
$(document).on("keyup", ".key__pos__plt", function () {
    var id = $(".pltid").attr("id");
    var North = $('.North').val() || 0;
    var South = $('.South').val() || 0;
    var East = $('.East').val() || 0;
    var West = $('.West').val() || 0;
    var North_East = $('.North_East').val() || 0;
    var North_West = $('.North_West').val() || 0;
    var Souh_East = $(".Souh_East ").val() || 0;
    var South_West = $('.South_West').val() || 0;
    var Front = $('#frn__slctn option:selected').val();
    var dataset =
    {
        Id: id,
        North: North,
        South: South,
        East: East,
        West: West,
        North_East: North_East,
        North_West: North_West,
        Souh_East: Souh_East,
        South_West: South_West,
        Front: Front
    };
    $.post('/Plots/PlotPositionUpdation/', { p: dataset }, function () {
    });
});
//
$(document).on("change", "#frn__slctn", function () {
    var id = $(".pltid").attr("id");
    var North = $('.North').val() || 0;
    var South = $('.South').val() || 0;
    var East = $('.East').val() || 0;
    var West = $('.West').val() || 0;
    var North_East = $('.North_East').val() || 0;
    var North_West = $('.North_West').val() || 0;
    var Souh_East = $(".Souh_East ").val() || 0;
    var South_West = $('.South_West').val() || 0;
    var Front = $('#frn__slctn option:selected').val();
    var dataset =
    {
        Id: id,
        North: North,
        South: South,
        East: East,
        West: West,
        North_East: North_East,
        North_West: North_West,
        Souh_East: Souh_East,
        South_West: South_West,
        Front: Front
    };
    $.post('/Plots/PlotPositionUpdation/', { p: dataset }, function () {
    });
});
//
$(document).on("change", ".plt__bndng", function () {
    var Pltbndedid = $(".pltid").attr("id");
    var Pltbndedwith = $(this).val();
    var Block = $('#Block').val();
    var Block_name = $('#Block option:selected').text();
    var Position = $('#bnded_po__sel option:selected').val();
    var type = $("#plot-type option:selected").val();
    if (Position == "" || Position == null) {
        //alert('Plese Select Postion');
        Swal.fire({
            icon: 'info',
            text: 'Select a position to proceed'
        });
        return false;
    }
    var dataset =
    {
        Plot_Id: Pltbndedid,
        BoundedPlotId: Pltbndedwith,
        Block: Block,
        Position: Position,
        Block_Name: Block_name,
        Plot_Type: type,
    };
    Swal.fire({
        text: 'Are you sure you want to bound this plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Plots/PlotBounding/', { B: dataset, isRoad: false }, function (data) {
                if (data.Status) {
                    $("#bound-det-tab").load('/Plots/PlotBoundTabular/', { Plot_Id: Pltbndedid });
                    $("#bound-det").load('/Plots/PlotBounded/', { Plot_Id: Pltbndedid });
                    //alert(data.Msg);
                    Swal.fire({
                        icon: 'success',
                        text: data.Msg
                    });
                }
                else {
                    //alert(data.Msg);
                    Swal.fire({
                        icon: 'info',
                        text: data.Msg
                    });
                }
            });
        }
    });
});
$(document).on("change", ".road_slctn", function () {
    var Pltbndedid = $(".pltid").attr("id");
    var Pltbndedwith = $(this).val();
    var Block = '';
    var Block_name = '';
    var Position = $('#bnded_po__sel option:selected').val();
    var type = "Road";
    if (Position == "" || Position == null) {
        alert('Plese Select Postion');
        return false;
    }
    var dataset =
    {
        Plot_Id: Pltbndedid,
        BoundedPlotId: Pltbndedwith,
        Block: Block,
        Position: Position,
        Block_Name: Block_name,
        Plot_Type: 0,
    };
    $.post('/Plots/PlotBounding/', { B: dataset, isRoad: true }, function (data) {
        if (data.Status) {
            $("#bound-det-tab").load('/Plots/PlotBoundTabular/', { Plot_Id: Pltbndedid });
            $("#bound-det").load('/Plots/PlotBounded/', { Plot_Id: Pltbndedid });
            alert(data.Msg);
        }
        else {
            alert(data.Msg);
        }
    });
});
//
$(document).on("click", ".del__bounding", function () {
    var Pltbndedid = $(this).attr("data-id");
    var id = $("#plt-id").val();
    //var ch = confirm('Are you sure you want to delete');
    //if (ch) {
    Swal.fire({
        text: 'Are you sure you want to delete the bounded plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Plots/DeleteBounding/', { Id: Pltbndedid }, function (data) {
                //alert('Successfully Deleted');
                Swal.fire({
                    icon: 'success',
                    text: 'Bounded plot deleted successfully'
                }).then(() => {
                    $("#bound-det-tab").load('/Plots/PlotBoundTabular/', { Plot_Id: id });
                    $("#bound-det").load('/Plots/PlotBounded/', { Plot_Id: id });
                })

            });
        }
    });
});
function readURLs(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#own_img__new').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
};
$(document).on("change", "#files", function () {
    $('.btn_gen__poss').show();
    readURLs(this);
});
//
$(document).on("submit", "#plt-imge", function (e) {
    e.preventDefault();
    var file = $("#files").get(0).files;
    if (file.length == 0) {
        //alert("Error Occured Or Image not uploaded");
        Swal.fire({
            icon: 'info',
            text: 'Image is not uploaded'
        });
        return false;
    }
    var form = $("#plt-imge");
    var matAltdPlt = $('.mat-altd-plt').val();
    $('#mat_altd_plt').val(matAltdPlt);
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    //var ch = confirm('Are you sure you want to proceed');
    //if (ch) {
    Swal.fire({
        text: 'Are you sure you want to proceed?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: $("#plt-imge").attr('action'),
                data: data,
                success: function (data) {
                    if (data == true) {
                        //alert('Request is now InProcess');
                        Swal.fire({
                            icon: 'success',
                            text: 'Request is in process'
                        }).then(() => {
                            window.location.reload();
                        })
                    }
                    else {
                        window.open("/Plots/PossessionPrint?PlotId=" + data, '_blank');
                        window.location.reload();
                    }
                }
                ,
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#gen-allot-let-s", function () {
    var id = $("#plot-id").val();
    var wit1 = $("#wit-1").val();
    var wit2 = $("#wit-2").val();
    var head = $("#head-nam option:selected").text();
    var des = $("#head-nam").val();
    $.ajax({
        type: "POST",
        url: '/Plots/SpecialAllotmentLetter/',
        data: { Id: id, Witness1: wit1, Witness2: wit2, Name: head, Designation: des },
        success: function (data) {
            if (data == 0) {
                alert("Allotment Letter Already Generated");
                return false;
            }
            alert("Allotment Letter Generated");
            $("#gen-allot-let").prop("disabled", true);
            $("#ver-allotment-let").prop("disabled", true);
            closeModal();
            window.open("/Plots/Special_AllotmentLetter?Id=" + data, '_blank');
        }
    });
});
//
$(document).on("click", "#gen-allot-let", function () {
    var id = $("#plot-id").val();
    var wit1 = $("#wit-1").val();
    var wit2 = $("#wit-2").val();
    var head = $("#head-nam option:selected").text();
    var des = $("#head-nam").val();
    Swal.fire({
        text: 'Are you sure you want to regenerate allotment letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/PlotAllotmentLetter/',
                data: { Id: id, Witness1: wit1, Witness2: wit2, Name: head, Designation: des },
                success: function (data) {
                    if (data.Status) {
                        //alert("Allotment Letter Generated");
                        Swal.fire({
                            icon: 'success',
                            text: 'Allotment letter generated successfully'
                        }).then(() => {
                            $("#gen-allot-let").prop("disabled", true);
                            $("#ver-allotment-let").prop("disabled", true);
                            window.open("/Plots/AllotmentLetter?Id=" + data.Id, '_blank');
                        })
                    }
                    else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                        return false;
                    }
                }
            });
        }
    });
});
//
$(document).on("click", ".del-rec", function () {
    var id = $(this).closest('tr').attr("id");
    var pltid = $("#plt-id").val();
    //var inp = confirm("Are you sure you want to Delete this Receipt");
    //if (inp == true) {
    Swal.fire({
        text: 'Are you sure you want to delete the receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Installments/DeleteReceipt/", { Id: id, Plotid: pltid }, function (data) {
                $("#rec-amts tbody").empty();
                //alert("Receipt Removed");
                Swal.fire({
                    icon: 'success',
                    text: 'Receipt deleted successfully'
                }).then(() => {
                    var total = 0;
                    $.each(data, function (i) {
                        var a = i + 1;
                        total = total + data[i].Amount;
                        var html = '<tr id="' + data[i].Id + '" data-token="' + data[i].TokenParameter + '" >' +
                            '<td scope="row">' + a + '</td>' +
                            '<td scope="row">' + data[i].ReceiptNo + '</td>' +
                            '<td>' + data[i].Amount + '</td>' +
                            '<td>' + moment(data[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + data[i].PaymentType + '</td><td><i class="ti-close del-rec"></i></td></tr>';
                        $("#rec-amts tbody").append(html);
                    });
                    var html1 = '<tr><td>Total</td><td colspan="5">' + total + '</td></tr>'
                    $("#rec-amts tbody").append(html1);
                    $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: pltid }, function () { });
                })
                
            });
        }
    });
});
//
$(document).on("click", ".del-rec-com", function () {
    var id = $(this).closest('tr').attr("id");
    var comid = $("#shp-id").val();
    var inp = confirm("Are you sure you want to Delete this Receipt");
    if (inp == true) {
        $.post("/Commercial/DeleteReceipt/", { Id: id, Com_id: comid }, function (data) {
            alert("Receipt Removed");
            $("#" + id).remove();
        });
    }
});
//
$(document).on("change", ".gran-cal", function () {
    var total_price = $("#plt-pric").val();
    var dis_pric = $("#dis-amt").val();
    var grand = total_price - dis_pric;
    $("#grnd-total").val(Number(grand).toLocaleString());
});
//
$(document).on("click", ".up-plot-det", function () {
    debugger;
    var id = $('#plot-details').val();
    SASLoad("#plot-det")
    $("#plot-det").load("/Plots/UpdateInformation?", { Plotid: id }, function () {
       SASUnLoad("#plot-det")
    });
});

// 
$(document).on("click", ".up-com-det", function () {
    var id = $('#shp-lst').val();
    SASLoad("#shop-det")
    $("#shop-det").load("/Commercial/CommercialInformation/", { Commercial_Id: id }, function () {
        SASUnLoad("#shop-det")
    });
});
// 
$(document).on("change", ".plt-rent", function () {
    var id = $(this).val();
    $("#plot-det").load("/Rental/AddRentalDetails/", { Plot_Id: id }, function () {
    });
});
//
$(document).on("submit", "#rentalData", function (e) {
    e.preventDefault();
    //var con = confirm("Are you sure you want to save the form for rental property?");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to save the form for rental property?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            //var form = $("#rentalData");
            //var cdata = new FormData();
            //var files = $("#renteePic").get(0).files;
            //if (files.length > 0) {
            //    cdata.append("Files", files[0]);
            //}
            //$.each(form.serializeArray(), function (key, input) {
            //    cdata.append(input.name, input.value);
            //});
            $.ajax({
                type: "POST",
                url: $("#rentalData").attr('action'),
                data: $("#rentalData").serialize(),
                success: function (data) {
                    if (data != null) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Form saved successfully'
                        }).then(() => {
                            window.location.reload();
                            window.open("/Rental/RentalInfo?Id=" + data, '_blank');
                        })
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
$(document).on("submit", "#rentalDataupdate", function (e) {
    e.preventDefault();
    var con = confirm("Are you sure you want to save this form?");
    if (con) {
        //var form = $("#rentalData");
        //var cdata = new FormData();
        //var files = $("#renteePic").get(0).files;
        //if (files.length > 0) {
        //    cdata.append("Files", files[0]);
        //}
        //$.each(form.serializeArray(), function (key, input) {
        //    cdata.append(input.name, input.value);
        //});
        $.ajax({
            type: "POST",
            url: $("#rentalDataupdate").attr('action'),
            data: $("#rentalDataupdate").serialize(),
            success: function (data) {
                if (data != null) {
                    window.location.replace("/Rental/RentalInfo?Id=" + data);
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", "#addplot", function () {
    var dimenid = $('.dime-id').val();
    var block = $('#blocks').val();
    var plotn = $('.plotno').val();
    var plotsec = $('.plotsec').val();
    var plotlo = $('.plotlo').val();
    var plottype = $('#plottype').val();
    var plotroad = $('.plotroad').val();
    var dataset =
    {
        Block_Id: block,
        Phase_Id: 0,
        Plot_Number: plotn,
        Sector: plotsec,
        Plot_Location: plotlo,
        Type: plottype,
        Road_Type: plotroad,
        Status: "Available_For_Sale",
        Develop_Status: "Non_Constructed"
    }
    $.ajax({
        type: "POST",
        url: '/Plots/Addplot/',
        data: { plot: dataset, DimensionId: dimenid },
        success: function (data) {
            if (!data) {
                alert("Error Occured");
            }
            else {
                alert("Plot Added");
            }
        },
        error: function () {
        }
    });
});
//......
$(document).on("click", ".upd-info", function () {
    debugger
    Swal.fire({
        text: 'Are you sure you want to update the owners information?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            var id = $(this).closest(".own-det").attr("id");
            $("#" + id + " :input").prop("readonly", false);
            $(this).hide();
            $("#" + id + " .sav-info").show();
            $("#" + id + " .own-img").show();
        }

    });
});
//.....update commercial..
$(document).on("click", ".com-upd-info", function () {
    var id = $(this).closest(".own-det").attr("id");
    $("#" + id + " :input").prop("readonly", false);
    $(this).hide();
    $("#" + id + " .sav-com-info").show();
    $("#" + id + " .own-img").show();
});
// SAve the updated information
$(document).on("click", ".sav-info", function () {
    var that = $(this);
    var id = $(this).closest(".own-det").attr("id");
    var plotowndata = {
        Id: id,
        Plot_Size: $("#" + id + " #pl-size").val(),
        Name: $("#" + id + " input[name=Name]").val(),
        Currency_Note_No: $("#" + id + " input[name=CNo]").val(),
        Father_Husband: $("#" + id + " input[name=Father_Husband]").val(),
        Postal_Address: $("#" + id + " input[name=Postal_Address]").val(),
        Residential_Address: $("#" + id + " input[name=Residential_Address]").val(),
        Phone_Office: $("#" + id + " input[name=Phone_Office]").val(),
        Residential: $("#" + id + " input[name=Residential]").val(),
        Mobile_1: $("#" + id + " input[name=Mobile_1]").val(),
        Mobile_2: $("#" + id + " input[name=Mobile_2]").val(),
        Email: $("#" + id + " input[name=Email]").val(),
        Occupation: $("#" + id + " input[name=Occupation]").val(),
        Nationality: $("#" + id + " input[name=Nationality]").val(),
        Date_Of_Birth: $("#" + id + " input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("#" + id + " input[name=CNIC_NICOP]").val(),
        Ownership_DateTime: $("#" + id + " input[name=Ownership_DateTime]").val(),
        Nominee_Name: $("#" + id + " input[name=Nominee_Name]").val(),
        Nominee_Relation: $("#" + id + " input[name=Nominee_Relation]").val(),
        Nominee_Address: $("#" + id + " input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("#" + id + " input[name=Nominee_CNIC_NICOP]").val(),
        DateTime: $("#" + id + " input[name=DateTime]").val(),
        Ownership_Status: $("#" + id + " select[name=Ownership_Status]").val(),
        Plot_Id: $("#plt-id").val(),
        Owner_Img: $("#img1").attr("src"),
        Owner_Img2: $("#img2").attr("src"),
        Owner_Img3: $("#img3").attr("src"),
        Owner_Img4: $("#img4").attr("src"),
        City: $("#" + id + " #City").val(),
       // Currency_Note_No: $("#" + id + " #CNo").val(),
        IsFiler: $(that).closest(".own-det").find('.IsFiler').is(':checked'),
    }
    if (plotowndata.Name == null || plotowndata.Name.trim() == "") {
        //alert("Please Enter Name");
        Swal.fire({
            icon: 'info',
            text: 'Enter name to proceed'
        });
        return false;
    }
    else if (plotowndata.CNIC_NICOP == null || plotowndata.CNIC_NICOP.trim() == "") {
        //alert("Please CNIC");
        Swal.fire({
            icon: 'info',
            text: 'Enter CNIC to proceed'
        });
        return false;
    }
    else if (plotowndata.Ownership_DateTime == null || plotowndata.Ownership_DateTime.trim() == "") {
        //alert("Ownership Date");
        Swal.fire({
            icon: 'info',
            text: 'Select ownership date to proceed'
        });
        return false;
    }
    Swal.fire({
        text: 'Are you sure you want to save updated owner information?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Plots/UpdateOwnerResult/',
                data: JSON.stringify(plotowndata),
                success: function (data) {
                    $("#" + id + " .upd-info").show();
                    $("#" + id + " .sav-info").hide();
                    $("#" + id + " .own-img").hide();
                    $("#" + id + " :input").prop("readonly", true);
                    //alert("Data Updated");
                    Swal.fire({
                        icon: 'success',
                        text: "Owner's information updated successfully"
                    });
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//...... save the updated information of commercial
$(document).on("click", ".sav-com-info", function () {
    var $this = $(this);
    var id = $(this).closest(".own-det").attr("id");
    var img = $("#own_img").attr("src")
    var plotowndata = {
        Id: 0,
        Plot_Size: $("#" + id + " #pl-size").val(),
        Name: $("#" + id + " input[name=Name]").val(),
        Father_Husband: $("#" + id + " input[name=Father_Husband]").val(),
        Postal_Address: $("#" + id + " input[name=Postal_Address]").val(),
        Residential_Address: $("#" + id + " input[name=Residential_Address]").val(),
        Phone_Office: $("#" + id + " input[name=Phone_Office]").val(),
        Residential: $("#" + id + " input[name=Residential]").val(),
        Mobile_1: $("#" + id + " input[name=Mobile_1]").val(),
        Mobile_2: $("#" + id + " input[name=Mobile_2]").val(),
        Email: $("#" + id + " input[name=Email]").val(),
        Occupation: $("#" + id + " input[name=Occupation]").val(),
        Nationality: $("#" + id + " input[name=Nationality]").val(),
        Date_Of_Birth: $("#" + id + " input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("#" + id + " input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("#" + id + " input[name=Nominee_Name]").val(),
        Nominee_Relation: $("#" + id + " input[name=Nominee_Relation]").val(),
        Nominee_Address: $("#" + id + " input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("#" + id + " input[name=Nominee_CNIC_NICOP]").val(),
        ComRom_Id: id,
        Owner_Img: img,
        City: $("#" + id + " #City").val(),
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Commercial/UpdateCommercialOwnerResult/',
        data: JSON.stringify(plotowndata),
        success: function (data) {
            $("#" + id + " .com-upd-info").show();
            $("#" + id + " .sav-com-info").hide();
            $("#" + id + " .own-img").hide();
            $("#" + id + " :input").prop("readonly", true);
            alert("Data Updated");
        },
        error: function (data) {
        }
    });
});
//// Add Plot Transfer to 
$(document).on("click", "#up-trans-btn", function () {
    nextown = $(this).data("owncount");
    var html = '<div class="own-det" id="own-' + nextown + '" style="margin-top:5%"><h6 class="c-grey-900">' + nextown + ' Owner Information</h6><div class="form-row row close-info"><div class="col-md-10"><div class="form-row">' +
        '<div class="form-group col-md-3"><label>Ownership Status</label><select name="Ownership_Status" class="form-control"><option value="Owner">Owner</option><option value="Transfered">Transfered</option><option value="Cancelled">Cancelled</option></select></div>' +
        '<div class="form-group col-md-3"><label>Ownership Date</label><input type="text" class="form-control" data-provide="datepicker" name="Ownership_DateTime" required></div>' +
        '<div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Name" required></div>' +
        '<div class="form-group col-md-3"><label>Father / Husband Name</label > <input type="text" class="form-control" name="Father_Husband" required></div>' +
        '<div class="form-group col-md-3"><label>CNIC / NICOP</label><input type="text" class="form-control" name="CNIC_NICOP" placeholder="12345-1234567-1   or   123456-123456-1" required></div>' +
        '<div class="form-group col-md-2"><label>Date Of Birth</label><input type="text" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" name="Date_Of_Birth" required></div>' +
        '<div class="form-group col-md-6"><label>Postal Address</label><input type="text" class="form-control" name="Postal_Address" placeholder="1234 Main St" required></div>' +
        '<div class="form-group col-md-6"><label>Residential Address</label><input type="text" class="form-control" name="Residential_Address" placeholder="Apartment, Plot, or floor" required></div>' +
        '<div class="form-group col-md-2"><label>City</label><select class="form-control" name="City">' + citylist + '</select></div>' +
        '<div class="form-group col-md-3"><label>Occupation</label><input type="text" class="form-control" name="Occupation"></div>' +
        '<div class="form-group col-md-2"><label>Nationality</label><input type="text" class="form-control" name="Nationality"></div>' +
        '<div class="form-group col-md-3"><label>Email</label><input type="text" class="form-control" name="Email"></div></div>' +
        '<div class="form-row"><div class="form-group col-md-3"><label>Phone Office</label><input type="text" class="form-control" name="Phone_Office"></div>' +
        '<div class="form-group col-md-3"><label>Residential</label><input type="text" class="form-control" name="Residential"></div>' +
        '<div class="form-group col-md-3"><label>Mobile 1</label><input type="text" class="form-control" placeholder="03121234567" name="Mobile_1" required></div>' +
        '<div class="form-group col-md-3"><label>Mobile 2</label><input type="text" class="form-control" pattern="^0\d{10}" placeholder="03121234567" name="Mobile_2"></div></div>' +
        '</div><div class="col-md-2"><input type="button" class="btn btn-info plot-inst-btn" data-toggle="modal" data-target="#Modal" value="Receive Installments" />' +
        '<img style="margin-top:10px" src="/assets/static/images/no-img.png" width="200" height="200" id="own_img" />' +
        '<input type="button" class="btn btn-info add-own-img" id="add-own-img" style="margin-top:10px" data-toggle="modal" value="Add Image" data-target="#Modal" /></div></div>' +
        '<h6 class="c-grey-900">Nominee</h6><div class="form-row"><div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Nominee_Name"></div>' +
        '<div class="form-group col-md-2"><label>CNIC / NICOP</label><input type="text" class="form-control" placeholder="12345-1234567-1 or 123456-123456-1" name="Nominee_CNIC_NICOP"></div>' +
        '<div class="form-group col-md-2"><label>Relation</label><input class="form-control" name="Nominee_Relation" placeholder=""></div><div class="form-group col-md-5">' +
        '<label>Address</label><input type="text" class="form-control" name="Nominee_Address" placeholder="1234 Main St"></div></div><button style="margin:5px" id="re-up-oth-plt-own" class="btn btn-primary">Register</button></div>';
    $("#up-trans-btn").hide();
    nextown++;
    $(".Tran-own").append(html);
});
//..... add to shop transfer update
$(document).on("click", "#up-trans-com-btn", function () {
    nextown = $(this).data("owncount");
    var html = '<div class="own-det" id="own-' + nextown + '" style="margin-top:5%"><h6 class="c-grey-900">' + nextown + ' Owner Information</h6><div class="form-row row close-info"><div class="col-md-10"><div class="form-row">' +
        '<div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Name" required></div>' +
        '<div class="form-group col-md-3"><label>Father / Husband Name</label > <input type="text" class="form-control" name="Father_Husband" required></div>' +
        '<div class="form-group col-md-3"><label>CNIC / NICOP</label><input type="text" class="form-control" name="CNIC_NICOP" placeholder="12345-1234567-1   or   123456-123456-1" required></div>' +
        '<div class="form-group col-md-2"><label>Date Of Birth</label><input type="text" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" name="Date_Of_Birth" required></div>' +
        '<div class="form-group col-md-6"><label>Postal Address</label><input type="text" class="form-control" name="Postal_Address" placeholder="1234 Main St" required></div>' +
        '<div class="form-group col-md-6"><label>Residential Address</label><input type="text" class="form-control" name="Residential_Address" placeholder="Apartment, Plot, or floor" required></div>' +
        '<div class="form-group col-md-2"><label>City</label><select class="form-control" id="City" name="City">' + citylist + '</select></div>' +
        '<div class="form-group col-md-3"><label>Occupation</label><input type="text" class="form-control" name="Occupation"></div>' +
        '<div class="form-group col-md-2"><label>Nationality</label><input type="text" class="form-control" name="Nationality"></div>' +
        '<div class="form-group col-md-3"><label>Email</label><input type="text" class="form-control" name="Email"></div></div>' +
        '<div class="form-row"><div class="form-group col-md-3"><label>Phone Office</label><input type="text" class="form-control" name="Phone_Office"></div>' +
        '<div class="form-group col-md-3"><label>Residential</label><input type="text" class="form-control" name="Residential"></div>' +
        '<div class="form-group col-md-3"><label>Mobile 1</label><input type="text" class="form-control" placeholder="03121234567" name="Mobile_1" required></div>' +
        '<div class="form-group col-md-3"><label>Mobile 2</label><input type="text" class="form-control" pattern="^0\d{10}" placeholder="03121234567" name="Mobile_2"></div></div>' +
        '</div><div class="col-md-2"><input type="button" class="btn btn-info plot-inst-btn" data-toggle="modal" data-target="#Modal" value="Receive Installments" />' +
        '<img style="margin-top:10px" src="/assets/static/images/no-img.png" width="200" height="200" id="own_img" />' +
        '<input type="button" class="btn btn-info add-own-img" id="add-own-img" style="margin-top:10px" data-toggle="modal" value="Add Image" data-target="#Modal" /></div></div>' +
        '<h6 class="c-grey-900">Nominee</h6><div class="form-row"><div class="form-group col-md-3"><label>Name</label><input type="text" class="form-control" name="Nominee_Name"></div>' +
        '<div class="form-group col-md-2"><label>CNIC / NICOP</label><input type="text" class="form-control" placeholder="12345-1234567-1 or 123456-123456-1" name="Nominee_CNIC_NICOP"></div>' +
        '<div class="form-group col-md-2"><label>Relation</label><input class="form-control" name="Nominee_Relation" placeholder=""></div><div class="form-group col-md-5">' +
        '<label>Address</label><input type="text" class="form-control" name="Nominee_Address" placeholder="1234 Main St"></div></div><button style="margin:5px" id="re-up-oth-com-own" class="btn btn-primary">Register</button></div>';
    $("#up-trans-com-btn").hide();
    nextown++;
    $(".Tran-own").append(html);
});
//
$(document).on("click", ".sh-all-let", function () {
    var id = $(this).closest('tr').attr("id");
    window.location = "/Plots/AllotmentLetter?Id=" + id;
});
//
$(document).on("click", ".sh-all-let-v", function () {
    var id = $(this).closest('tr').attr("id");
    window.open("/Plots/AllotmentLetterView?Id=" + id, "_Blank");
});
//
$(document).on("click", ".all-rec", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    $.post('/Installments/VerifyReceipt/', { Id: id, Status: chkstat }, function (data) {
        alert("Receipt Verified");
    });
});
//
$(document).on("click", ".all-rec", function () {
    var allrec = $('.all-rec').length;
    var checkrec = $('.all-rec:checked').length;
    if (allrec == checkrec)
        $('#ver-all').show();
    else $('#ver-all').hide();
});
//
$(document).on("click", "#cr-ins-plan", function () {
    $("#inst-plan").show();
});
//
$(document).on("click", "#sav-ins", function () {
    var plotid = $("#plot-details").val();
    var plt_price = $("#plt-pric").val();
    var disc = $("#dis-amt").val();
    var data = { PI: plotinstdata, Plotid: plotid, Plot_Price: plt_price, Discount: disc };
    //var conf = confirm("Are You Want to Plot Installments");
    //if (conf) {
    Swal.fire({
        text: 'Are you sure you want to add to the installments?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Installments/UpdatePlotInstallments/',
                data: JSON.stringify(data),
                success: function (data) {
                    //alert("Installments Updated")
                    Swal.fire({
                        icon: 'success',
                        text: 'Installment added successfully'
                    }).then(() => {
                        $("#inst-plan").hide();
                        $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: plotid }, function () { });
                    })
                   
                }
            });
        }
    });
});
//...... Commercial Installments Update
$(document).on("click", ".ch-bk-pay", function () {
    var id = $(this).attr("id");
    var conf = confirm("Are You Want Mark ChargeBack");
    if (conf) {
        $.ajax({
            type: "POST",
            url: '/Banking/UpdateChargeBack/',
            data: { Id: id },
            success: function (data) {
                alert(data.Msg);
            },
            error: function () {
                alert("Error Occured");
                $('#gen-plot-rec').attr("disabled", false);
            }
        });
    }
});
//...... Commercial Installments Update
$(document).on("click", "#sav-com-ins", function () {
    var shpid = $("#shp-lst").val();
    var shp_price = $("#plt-pric").val();
    var disc = $("#dis-amt").val();
    var data = { comIns: plotinstdata, shopid: shpid, shop_Price: shp_price, Discount: disc };
    //var conf = confirm("Are You Want to Plot Installments");
    //if (conf) {
    Swal.fire({
        text: 'Are you sure you want to add to the installments?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: '/Commercial/UpdateCommercialInstallments/',
                data: JSON.stringify(data),
                success: function (data) {
                    //alert("Installments Updated")
                    Swal.fire({
                        icon: 'success',
                        text: 'Installment added successfully'
                        })
                    $("#inst-plan").hide();
                    // $("#plot-rep").load("/Plots/PlotInstallmentsReports/", { Plotid: plotid }, function () { });
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                        })
                }
            });
        }
    });
});
//
$(document).on("click", ".plt-stat", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    $.post('/Plots/UpdatePlotDisputedStatus/', { Id: id, Status: chkstat }, function (data) {
        alert("Plot Status Changed");
    });
});
// Mark Plot as registry
$(document).on("click", ".regis-plt", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    Swal.fire({
        text: 'Are you sure you want to registry the plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Plots/UpdatePlotRegistryStatus/', { Id: id, Status: chkstat }, function (data) {
                //alert("Plot Registry Updated");
                Swal.fire({
                    icon: 'success',
                    text: 'Plot registry updated successfully'
                    })
            });
        }
    });
});
//
$(document).on("click", "#can-rec", function () {
    var opt = confirm("Are you sure you want to cancel the receipt");
    if (opt) {
        $("#can-stat").val(1);
        $("#canrecdata").show();
        $("#first-rec :input").prop("disabled", true);
    }
});
//
$(document).on("click", "#ser-plot-info", function () {
    var id = $("#plot-details").val();
    $("#plot-det").load("/Plots/LastestPlotDetails/", { Plotid: id }, function () {
    });
});
//
$(document).on("change", ".n-r-al-plt", function () {
    var id = $("#plot-details").val();
    $("#plot-det").load("/Plots/NewOwnerAllotmentLetter/", { Plotid: id }, function () {
    });
});
//
$(document).on("click", "#rec-plot-install", function () {
    EmptyModel();
    $('.modal-body').load('/Installments/PlotInstallment/', function () {
    });
});
//
$(document).on("click", ".orig-rece", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to Reprint Receipt");
    if (con) {
        window.open("/Receipts/Receipt?Id=" + id, '_blank');
    }
});
//
$(document).on("click", ".orig-pay", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to Reprint Payment Voucher");
    if (con) {
        window.open("/Vouchers/SAGVoucher?Id=" + id, '_blank');
    }
});
//
$(document).on("click", ".dup-rece", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to print Duplicate Receipt");
    if (con) {
        window.open("/Receipts/DupReceipt?Id=" + id, '_blank');
    }
});
$(document).on("click", ".pay-vou", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to print Duplicate Voucher");
    if (con) {
        window.open("/Vouchers/Voucher?GroupId=" + id, '_blank');
    }
});
//
$(document).on("click", ".orig-rece-mar", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to Reprint Receipt");
    if (con) {
        window.open("/Receipts/SAM_Receipt?Id=" + id, '_blank');
    }
});
//
$(document).on("click", ".dup-rece-mar", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    var con = confirm("Are you sure you want to print Duplicate Receipt");
    if (con) {
        window.open("/Receipts/Dup_SAM_Receipts?Id=" + id, '_blank');
    }
});
//
$(document).on("click", "#gen-plot-rec", function (e) {
    //alert("Clicked.");
    e.preventDefault();
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var img = $("#scanned").attr('src');
    $("#scanimage").val(img);
    if ($("#amt").val() <= 0) {
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid amount to proceed'
        });
        return false;
    }
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#gen-plot-rec').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: $("#re-plot-ins").attr('action'),
                data: $("#re-plot-ins").serialize(),
                success: function (data) {
                    if (data.Status == false) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'error',
                            text: data.Msg
                        });
                    }
                    else if (data.Status == true) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        });
                        window.open("/Banking/PlotReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: "Something went wrong"
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("click", "#gen-com-rec", function (e) {
    e.preventDefault();
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var img = $("#scanned").attr('src');
    $("#scanimage").val(img);
    if ($("#amt").val() <= 0) {
        return false;
    }
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#gen-plot-rec').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: $("#re-com-ins").attr('action'),
                data: $("#re-com-ins").serialize(),
                success: function (data) {
                    if (data.Status == false) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'error',
                            text: data.Msg
                        });
                    }
                    else if (data.Status == true) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        }).then(() => {
                            window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                        })
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("click", ".ref-del-rec", function (e) {
    e.preventDefault();
    var id = $(this).attr(id);
    var con = confirm("Are you sure you want to Refund This Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            data: { Id: id },
            url: '///',
            success: function (data) {
                window.open("/Vouchers/SAM_Voucher?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
                $('#gen-plot-rec').attr("disabled", false);
            }
        });
    }
});
//
$(document).on("submit", "#gen-oth-recov", function (e) {
    e.preventDefault();
    $('#gen-oth-recov').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var img = $("#scanned").attr('src');
    $("#scanimage").val(img);
    if ($("#amt").val() <= 0) {
        return false;
    }
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#gen-oth-recov").attr('action'),
            data: $("#gen-oth-recov").serialize(),
            success: function (data) {
                window.open("/Receipts/OtherRecovery?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
                $('#gen-oth-recov').attr("disabled", false);
            }
        });
    }
});
// 
$(document).on("click", "#op-plt", function () {
    var chkstat = $(this).is(":checked");
    if (chkstat) {
        $(".close-info :input").prop("disabled", true);
    }
    else {
        $(".close-info :input").prop("disabled", false);
    }
});
//
$(document).on("click", "#full-pay", function () {
    var chkstat = $(this).is(":checked");
    if (chkstat) {
        $("#adv-pmt").text($('#gran-cal').val());
    }
    else {
        var amt = $("#adv-pmt-hid").val();
        $("#adv-pmt").text(Number(amt).toLocaleString());
    }
});
// Register  a Plots
$(document).on("submit", "#re-bid-plot", function (e) {
    e.preventDefault();
    var advamt = parseFloat($("#adv-pmt-hid").val());
    var totalamt = 0
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    if (advamt != totalamt) {
        alert("Received Amount is not equal to Booking Amount");
        return false;
    }
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        allrecepts.push(recedata);
    }
    $("#reg-plot").prop("disabled", true);
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Block_Id: $("#blk_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var pids = [];
    $(".plt-ids").each(function () {
        var val = $(this).val();
        pids.push(val);
    })
    var alldata = {
        PO: regfiledata,
        Flag: flag,
        Receiptdata: allrecepts,
        PlotId: pids,
        Dealerid: $("#deal-id").val(),
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("Plots Has Been Registerd");
                    window.open("/Banking/BidingPlotReceipt?Id=" + data.Receiptid, '_blank');
                    //window.location.reload();
                }
                else if (data.Status == "2") {
                    alert("Wait Until You Payment is Clear from Bank");
                    window.open("/Banking/BidingPlotReceipt?Id=" + data.Receiptid, '_blank');
                    //window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
// Register  a Plots
$(document).on("submit", "#re-bid-fil", function (e) {
    e.preventDefault();
    var advamt = parseFloat($("#adv-pmt-hid").val());
    var totalamt = 0
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    if (advamt != totalamt) {
        alert("Received Amount is not equal to Booking Amount");
        return false;
    }
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#app-fil-id").val(),
        Block_Id: $("#blk_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", Mobile_1: "", Father_Husband: "", Name: "", Block: ""
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = "Meher Estate Developers";
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.Name = $("input[name=Name]").val();
        recedata.Father_Husband = $("input[name=Father_Husband]").val();
        recedata.Mobile_1 = $("input[name=Mobile_1]").val();
        recedata.Block = "Sher Afghan";
        allrecepts.push(recedata);
    }
    $("#reg-plot").prop("disabled", true);
    var alldata = {
        Receiptdata: allrecepts,
        filedata: regfiledata,
        GroupId: $("#group-no").val()
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("Plots Has Been Registerd");
                    window.open("/Banking/FileBiddingReceipt?Id=" + data.Receiptid, '_blank');
                    //window.location.reload();
                }
                else if (data.Status == "2") {
                    alert("Wait Until You Payment is Clear from Bank");
                    window.open("/Banking/FileBiddingReceipt?Id=" + data.Receiptid, '_blank');
                    //window.location.reload();
                }
                else if (data.Status == "5") {
                    window.open("/Banking/BidingPlotReceipt?Id=" + data.Receiptid, '_blank');
                    //window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
// Plot Details
$(document).on("click", ".plot-det", function () {
    var id = $(this).attr("id");
    window.open("/Plots/PlotsVerification?Plotid=" + id, '_blank');
});
//
$(document).on("click", ".all-rec-veri", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    $.post('/Installments/VerifyReceipt/', { Id: id, Status: chkstat }, function (data) {
    }).fail(function () {
        alert("error occured Try Again");
    });
});
//
$(document).on("click", ".all-file-rec-veri", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    $.post('/Installments/VerifyFileReceipt/', { Id: id, Status: chkstat }, function (data) {
    }).fail(function () {
        alert("error occured Try Again");
    });
});
//
$(document).on("click", ".all-rec-veri", function () {
    var allrec = $('.all-rec-veri').length;
    var checkrec = $('.all-rec-veri:checked').length;
    if (allrec == checkrec) {
        $('#ver-all').show();
        $('#ver-plot').show();
    }
    else {
        $('#ver-all').hide();
        $('#ver-plot').hide();
    }
});
// Audit Verified
$(document).on("click", "#aud-veri-stat", function () {
    var id = $("#file-id").val();
    $.post('/FileSystem/AuditVerifyFile/', { Id: id }, function () { alert("Verified") });
});
//
$(document).on("click", ".all-file-rec-veri", function () {
    var allrec = $('.all-file-rec-veri').length;
    var checkrec = $('.all-file-rec-veri:checked').length;
    if (allrec == checkrec) {
        $('#aud-veri-stat').show();
    }
    else {
        $('#aud-veri-stat').hide();
    }
});
//
$(document).on("click", "#ver-plot", function () {
    var id = $("#plt-id").val();
    Swal.fire({
        text: 'Are you sure you want to verify this plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/VerifingPlot/',
                data: { Id: id },
                success: function (data) {
                    //alert("Plot is Verified");
                    Swal.fire({
                        icon: 'success',
                        text: "Plot verified successfully"
                    });
                    $("#ver-plot").prop("Disabled", true);
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("change", ".p-plt-details", function () {
    var id = $(this).val();
    $("#plot-det").load("/Plots/PostPlotDetails/", { Plotid: id }, function () {
    });
});
//
$(document).on("change", "#plot-veri-stat", function () {
    var id = $("#plt-id").val();
    if (this.checked) {
        $.post('/Plots/PostVerify/', { Id: id, Status: true }, function () {
            alert("Verified")
        });
    } else {
        $.post('/Plots/PostVerify/', { Id: id, Status: false }, function () {
        });
    }
});
//

$(document).on("click", ".up-dim", function () {
    debugger
    EmptyModel();
    var road = $("#plt-road").text();
    var plotsize = $("#plt-size").text();
    var dim = $("#plt-dim").text();
    var plt_cat = $("#plt-dim-id").val();
    var area = $("#plt-area").text();
    var location = $("#plt-loc").text();
    $('#modalbody').load('/Plots/UpdatePlotDimension/', { Road: road, Plotsize: plotsize, Dimension: dim, Plot_Cat: plt_cat, Area: area, Location: location });
});
//
$(document).on("click", ".add-dim", function () {
    EmptyModel();
    $('#modalbody').load('/Plots/AddPlotCategory/');
});
//

$(document).on("click", ".up-dim-c", function () {
    EmptyModel();
    $('#modalbody').load('/Commercial/UpdateShopDimension/');
});
//
$(document).on("click", "#up-plt-dim", function (e) {
    e.preventDefault();
    Swal.fire({
        text: 'Are you sure you want to update the plot Dimensions?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#up-plot-dime").attr('action'),
                data: $("#up-plot-dime").serialize(),
                success: function (data) {
                    //alert("Updated");
                    Swal.fire({
                        icon: 'success',
                        text: 'Dimensions updated successfully'
                    });
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
//
$(document).on("click", "#up-plt-dim-c", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#up-plot-dime-c").attr('action'),
        data: $("#up-plot-dime-c").serialize(),
        success: function (data) {
            alert("Updated");
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("submit", "#com-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    if ((msg == null || msg == "") && file.length == 0) {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#com-comt");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#com-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Msg_Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});
//
$(document).on("submit", "#plt-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    if ((msg == null || msg == "") && file.length == 0) {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#plt-comt");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#plt-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Msg_Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});
//
$(document).on("submit", "#fl-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    if ((msg == null || msg == "") && file.length == 0) {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#fl-comt");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#fl-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Msg_Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});
//
$(document).on("submit", "#ld-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    if (msg == null || msg == "") {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#ld-comt");
    var data = new FormData();
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#ld-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});
//
$(document).on("submit", "#del-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    if (msg == null || msg == "") {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#del-comt");
    var data = new FormData();
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#del-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});


$(document).on("submit", "#T-comt", function (e) {
    e.preventDefault();
    var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    if ((msg == null || msg == "") && file.length == 0) {
        return false;
    }
    var date = moment().format('DD MMM, h:mm a');
    var nam = $("#user-name").text();
    var form = $("#T-comt");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    $.ajax({
        type: "POST",
        processData: false,
        contentType: false,
        url: $("#T-comt").attr('action'),
        data: data,
        success: function (data) {
            $.each(data, function (i) {
                if (data[i].Type == "Text") {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<span>' + msg + '</span>' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
                else {
                    var html = '<div class="layer"><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white bdrs-2 lh-3/2"><div class="peer-greed ord-0">' +
                        '<img src="/PlotsData/' + data[i].Plot_Id + '/' + data[i].Comment + '" width="200" height="200" />' +
                        '</div></div><div class="peers fxw-nw ai-c pY-3 pX-10 bgc-white"><div class="peer-greed ord-0"><span>' + nam + '</span>' +
                        '</div><div class="peer mL-10 ord-1"><small>' + date + '</small></div></div></div>';
                    $(html).hide().appendTo("#chat-msg").fadeIn(500);
                    $(".fl-text").val('');
                    $("#files").val("");
                }
            });
            ChatBottom();
        }
    });
});
//
$(document).on("change", ".t-stat", function () {
    var id = $("#t-id").val();
    var from = $(this).data("from");
    var stat = $(this).val();
    $.post('/Tickets/UpdateStatus/', { Ticket_Id: id, Status: stat, From: from }, function () {
        alert("Updated");
        window.location.reload();
    });
});
//
$(document).on("click", ".for-tick", function () {
    var id = $("#t-id").val();
    var depid = $("#Department").val();
    var depnam = $("#Department :selected").text();
    var to = $("#AssignedTo").val();
    var reas = $("#Reason").val();
    if (reas == "" || reas == null) {
        $("#Reason").focus();
        //alert("Please Enter the Reason");
        Swal.fire({
            icon: 'info',
            text: "Enter the reason to proceed"
        });
        return false;
    }
    $.post('/Tickets/ForwardTicket/', { Id: id, Dep_Id: depid, Dep: depnam, AssignedTo: to, Reason: reas }, function () {
        //alert("Forwarded");
        Swal.fire({
            icon: 'success',
            text: "Ticket forwarded successfully"
        }).then(() => {
            window.location.reload();
        })
    });
});
//
//
$(document).on("click", ".reop-tick", function () {
    var id = $("#t-id").val();
    var reas = $("#reopen-rem").val();
    if (reas == "" || reas == null) {
        $("#reopen-rem").focus();
        //alert("Please Enter the Reason");
        Swal.fire({
            icon: 'info',
            text: "Enter reason to reopen the ticket to proceed"
        });
        return false;
    }
    $.post('/Tickets/Reopenticket/', { Id: id, Reason: reas }, function () {
        //alert("Ticket is Reopened");
        Swal.fire({
            icon: 'success',
            text: "The ticket has been reopened"
        }).then(() => {
            window.location.reload();
        })
    });
});
//
$(document).on("click", ".for-tick-btn", function () {
    $("#dep-sec").show();
});
//
$(document).on("change", "#al-req", function () {
    var id = $("#plt-id").val();
    if (this.checked) {
        $.post('/Plots/AllotmentLetterStatus/', { Id: id, Status: 3 }, function () {
            alert("Requested")
        });
    } else {
        $.post('/Plots/AllotmentLetterStatus/', { Id: id, Status: 0 }, function () {
        });
    }
});
//
// Possession 
$(document).on("change", "#Poss__req__plots", function () {
    var id = $("#plt-id").val();
    if (this.checked) {
        $.post('/Plots/PossessionLetterStatus/', { Id: id, Status: 1 }, function () {
            //alert("Requested")
            Swal.fire({
                icon: 'success',
                text: 'Requested for verification successfully'
                })
        });
    } else {
        $.post('/Plots/PossessionLetterStatus/', { Id: id, Status: 0 }, function () {
        });
    }
});
// for possession
$(document).on("click", ".plot__get__det__poss", function () {
    var id = $(this).attr("id");
    var tp = $(this).attr('data-tp');
    if (tp == 3) {
        window.location = "/Plots/PossessionPrint?PlotId=" + id;
    }
    else {
        window.location = "/Plots/GetPlotPossessionRequest?Plotid=" + id;
    }
});
//
$(document).on('click', '.poss-print-sdflkjn', function () {
    let tp = $(this).attr('data-tp');
    let posID = $(this).attr('id');
    if (tp == 3) {
        window.open('/Plots/PossessionPrint?PlotId=' + posID);
    }
    else if (tp == 2 || tp == 1) {
        window.open('/Plots/GetPlotPossessionRequest?Plotid=' + posID);
    }
});
//
$(document).on("click", ".plot-all-det", function () {
    var id = $(this).attr("id");
    window.location = "/Plots/PlotAllotmentReqAppr?Plotid=" + id;
});
//
$(document).on("click", ".all-let-del", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    var html =
        '<form method="post" id="up-al-del"><input id="all-id" type="hidden" value="' + id + '" name="Id" /><div class="form-row"><div class="col-md-3">' +
        '<label>Delivery Date</label><input type="text" name="Delivery_Date" class="form-control" data-provide="datepicker" placeholder="MM / DD / YYYY" required></div>' +
        '<div class="col-md-3"><input type="file" id="files" name="Files" style="margin-top:15%" class="attachment_upload" required></div></form>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="up-del-all" type="submit">Update</button>');
});
//
$(document).on("click", ".prt-blk-form", function () {
    var id = $(this).attr("id");
    window.open("/Plots/BlockFile?Id=" + id, '_blank');
});
//
$(document).on("click", ".prt-form", function () {
    var id = $(this).attr("id");
    window.open("/Plots/PlotsFile?Id=" + id, '_blank');
});
//
$(document).on("click", ".all-let-del-pre", function () {
    var id = $(this).attr("data-ownid");
    EmptyModel();
    var html =
        '<form method="post" id="up-al-del"><input id="all-id" type="hidden" value="' + id + '" name="Id" /><div class="form-row">' +
        '<div class="col-md-3"><input type="file" id="files" name="Files" style="margin-top:15%" class="attachment_upload" required></div>' +
        '<div class="col-md-3"><label>Delivered Date</label><input type="text" name="Delivered_Date" data-provide="datepicker" id="del-dat"/></div></form>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="up-del-all-pre" type="submit">Update</button>');
});
//
$(document).on("click", "#up-del-all", function () {
    var id = $("#all-id").val();
    var form = $("#up-al-del");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    Swal.fire({
        text: 'Are you sure you want to mark allotment letter Delivered?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: '/Plots/UpdateDeliveryofAllotment/',
                data: data,
                success: function (data) {
                    //alert("Delivered Successfull");
                    Swal.fire({
                        icon: 'success',
                        text: 'Marked as delivered successfully'
                    }).then(() => {
                        $("#" + id).remove();
                        closeModal();
                    })
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#up-del-all-pre", function () {
    var id = $("#all-id").val();
    var form = $("#up-al-del");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    Swal.fire({
        text: 'Are you sure you want to deliver allotment letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: '/Plots/UpdateDeliveryofAllotmentPrevious/',
                data: data,
                success: function (data) {
                    //alert("Delivered Successfull");
                    Swal.fire({
                        icon: 'success',
                        text: 'Allotmwnt letter delivered successfully'
                    });
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Plots Verification
$(document).on("click", "#veri-req", function () {
    var id = $("#plt-id").val();
    $.ajax({
        type: "POST",
        url: '/Plots/VerifyReq/',
        data: { Id: id },
        success: function (data) {
            //alert("Request Successfull");
            Swal.fire({
                icon: 'success',
                text: 'Requested for verification successfully'
            });
        },
        error: function () {
            //alert("Error Occured");
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
            });
        }
    });
});
// Plots Verification
$(document).on("click", "#veri-req-id", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "POST",
        url: '/Plots/VerifyReq/',
        data: { Id: id },
        success: function (data) {
            alert("Request Successfull");
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
// File Verification
$(document).on("click", "#veri-req-f", function () {
    var id = $("#file-id").val();
    $.ajax({
        type: "POST",
        url: '/FileSystem/VerifyReq/',
        data: { Id: id },
        success: function (data) {
            //alert("Request Successfull");
            Swal.fire({
                icon: 'success',
                text: 'Requested for verification successfully'
            })
        },
        error: function () {
            //alert("Error Occured");
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
            });
        }
    });
});
// File Verification id
$(document).on("click", "#veri-req-f-id", function () {
    var id = $(this).data("id");
    $.ajax({
        type: "POST",
        url: '/FileSystem/VerifyReq/',
        data: { Id: id },
        success: function (data) {
            alert("Request Successfull");
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
// First warning to File owner
$(document).on("click", ".fir-war-f", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    var num = $(this).data("num");
    //var con = confirm("Are you sure you want to Generate First Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the first warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/FileSystem/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "First" },
                success: function (data) {
                    if (data) {
                        Swal.fire({
                            icon: 'success',
                            text: 'First warning letter generated successfully'
                        }).then(() => {
                            window.open("/FileSystem/FirstWarningLetter?Id=" + id, '_blank');
                        })
                        //window.open("/Reports/FileStatmentAcc?Token=" + num, '_blank');
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// First warning to File owner
$(document).on("click", ".fir-war-p", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate First Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the first warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "First" },
                success: function (data) {
                    window.open("/Plots/FirstWarningLetter?Id=" + id, '_blank');
                    window.open("/Plots/PlotStatment?Plotid=" + id, '_blank');
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Second warning to File owner
$(document).on("click", ".sec-war-p", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate Second Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the second warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Second" },
                success: function (data) {
                    window.open("/Plots/SecondWarningLetter?Id=" + id, '_blank');
                    window.open("/Plots/PlotStatment?Plotid=" + id, '_blank');
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Cancellation to File owner
$(document).on("click", ".can-let-p", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate Cancellation Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the cancellation letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Last" },
                success: function (data) {
                    window.open("/Plots/CancellationLetter?Id=" + id, '_blank');
                    //window.open("/Plots/PlotStatment?Plotid=" + id, '_blank');
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Second warning to File owner
$(document).on("click", ".sec-war-f", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    var num = $(this).data("num");
    //var con = confirm("Are you sure you want to Generate Second Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the second warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/FileSystem/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Second" },
                success: function (data) {
                    if (data) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Second warning letter generated successfully'
                        }).then(() => {
                            window.open("/FileSystem/SecondWarningLetter?Id=" + id, '_blank');
                        })
                        /*     window.open("/Reports/FileStatmentAcc?Token=" + num, '_blank');*/
                    }
                    //else {
                    //    alert("Installments are Settled")
                    //}
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Cancellation to File owner
$(document).on("click", ".can-let-f", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    var num = $(this).data("num");
    //var con = confirm("Are you sure you want to Generate Cancellation Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the cancellation letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/FileSystem/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Last" },
                success: function (data) {
                    if (data) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Cancellation letter generated successfully'
                        }).then(() => {
                            window.open("/FileSystem/CancellationLetter?Id=" + id, '_blank');
                        })
                        //window.open("/Reports/FileStatmentAcc?Token=" + num, '_blank');
                    }
                    //else {
                    //    alert("Installments are Settled")
                    //}
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
// First warning to File owner
$(document).on("click", ".fir-war-com", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate First Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the first warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Commercial/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "First" },
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'First warning letter generated'
                    }).then(() => {
                        window.open("/Commercial/FirstWarningLetter?Id=" + id, '_blank');
                        window.open("/Commercial/LedgerdetailReport?Commercial_Id=" + id, '_blank');
                    })
                    
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Second warning to File owner
$(document).on("click", ".sec-war-com", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate Second Warning Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the second warning letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Commercial/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Second" },
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Second warning letter generated'
                    }).then(() => {
                        window.open("/Commercial/SecondWarningLetter?Id=" + id, '_blank');
                        window.open("/Commercial/LedgerdetailReport?Commercial_Id=" + id, '_blank');
                    })
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// Cancellation to File owner
$(document).on("click", ".can-let-com", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate Cancellation Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the cancellation letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Commercial/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "Last" },
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Cancellation letter generated'
                    }).then(() => {
                        window.open("/Commercial/CancellationLetter?Id=" + id, '_blank');
                        //window.open("/Commercial/LedgerdetailReport?Commercial_Id=" + id, '_blank');
                    });
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});

// Cancellation to File owner
$(document).on("click", ".can-let-com-con", function () {
    var id = $(this).data("id");
    var ownid = $(this).data("ownid");
    //var con = confirm("Are you sure you want to Generate Cancellation Letter");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the first cancellation letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Commercial/WarningIssues/',
                data: { Id: id, OwnerId: ownid, Type: "ConfirmLast" },
                success: function (data) {
                    if (data) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Cancellation letter generated'
                        }).then(() => {
                            window.open("/Commercial/CancellationLetterConfirm?Id=" + id, '_blank');
                        })
                        //window.open("/Commercial/LedgerdetailReport?Commercial_Id=" + id, '_blank');
                    }
                    else {
                        //alert("Adjusted")
                        Swal.fire({
                            icon: 'info',
                            text: 'Adjusted'
                        })
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
// First warning to File owner
$(document).on("click", ".war-step-f", function () {
    var ownid = $(this).data("ownid");
    var let = $(this).data("let");
    var num = $(this).data("num");
    var con = confirm("Are you sure you want to Step Back this Letter");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/FileSystem/UpdateLetterStep/',
            data: { Id: ownid, Let: let },
            success: function (data) {
                if (data) {
                    alert(data.Msg);
                    $('#' + num).remove();
                }
                else {
                    alert("Installments Settled")
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
// First warning to File owner
$(document).on("click", ".war-step-p", function () {
    var ownid = $(this).data("ownid");
    var let = $(this).data("let");
    var id = $(this).data("id");
    var con = confirm("Are you sure you want to Step Back this Letter");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Plots/UpdateLetterStep/',
            data: { Id: ownid, Let: let },
            success: function (data) {
                if (data) {
                    alert(data.Msg);
                    $('#' + id).remove();
                }
                else {
                    alert("Installments Settled")
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
////
//$(document).on("click", ".plot_noc", function () {
//    var type = $(this).data("type");
//    var id = $('#plt-id').val();
//    if (type == "Customer") {
//        if (confirm("Are you sure you want to generate NOC of This Plot")) {
//            window.open("/Plots/PlotNOC?Id=" + id, '_blank');
//        }
//    }
//    else if (type == "Dealer") {
//        EmptyModel();
//        $('#ModalLabel').text("Dealer NOC");
//        $('#modalbody').load('/Plots/Dealershipname/');
//    }
//});
//
$(document).on("click", ".generate_noc", function () {
    var id = $('#plt-id').val();
    EmptyModel();
    $('#ModalLabel').text("Plot NOC");
    $('#modalbody').load('/Plots/Dealershipname/', { Id: id });
});

$(document).on("change", ".chck_posession_NOC", function (e) {
    debugger
    var PlotId = $('#Plot_Id').val();
    var text = $("option:selected", this).text();
    if (text == 'Select Purpose') {
        return false;
    }
    if (text != 'Possession') {
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/Plots/CheckPosessionNOC/',
        data: { PlotId: PlotId },
        success: function (data) {
            if (!data) {

                $("#Possession").prop('disabled', true);
                $('.chck_posession_NOC')
                    .find('option')
                    .remove()
                    .end()
                    .append('<option value="" class="select">Select Purpose</option><option value="Record">Record</option><option value="Transfer">Transfer</option><option value="GWE">Gas, Water, & Electricity </option><option disabled dis id="Possession">Possession</option>')
                    ;
                alert("Total Payment is Not Paid");
            }
        },
        //error: function () {
        //    debugger
        //    alert("Error Occured");

        //}
    });
});

//
$(document).on("click", ".Generate_Cust_dealer_noc", function () {
    debugger
    //var type = $(this).data("type");
    var Plot_Id = $('#Plot_Id').val();
    var NocType = $('#NocType').val();
    var CustomeType = $('#CustomeType').val();
    var DealerType = $('#DealerType').val();
    var Dealer_Id = $('#Dealership').val();
    if (NocType == 'Select NOC') {
        //alert("Select NOC Type");
        Swal.fire({
            icon: 'info',
            text: 'Select NOC type to proceed'
        });
        return false;
    }

    if (NocType == "Customer") {
        if (CustomeType == 'Select Purpose' || CustomeType == '') {
            //alert("Select NOC Purpose");
            Swal.fire({
                icon: 'info',
                text: 'Select the purpose of NOC to proceed'
            });
            return false;
        }

        //if (confirm("Are you sure you want to generate NOC of This Plot")) {
        Swal.fire({
            text: 'Are you sure you want to generate NOC for this plot?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire({
                    icon: 'success',
                    text: 'NOC generated successfully'
                }).then(() => {
                    window.open("/Plots/PlotNOC?Id=" + Plot_Id + "&CustomeType=" + CustomeType, '_blank');

                    closeModal();
                })
            }
        });
    }
    else if (NocType == "Dealers") {
        if (DealerType == 'Select Purpose' || DealerType == '') {
            //alert("Select NOC Purpose");
            Swal.fire({
                icon: 'info',
                text: 'Select the purpose of NOC to proceed'
            });
            return false;
        }
        if (Dealer_Id == null || Dealer_Id == '') {
            //alert("Select Dealership");
            Swal.fire({
                icon: 'info',
                text: 'Select the dealership to proceed'
            });
            return false;
        }
        //if (confirm("Are you sure you want to generate NOC of This Plot")) {
        Swal.fire({
            text: 'Are you sure you want to generate NOC for this plot?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                window.open("/Plots/DealerNOC?Id=" + Plot_Id + "&DealerType=" + DealerType + "&Dealer_Id=" + Dealer_Id, '_blank');
                closeModal();
            }
        });
    }
});
//
$(document).on("click", ".dealer_noc", function () {
    var id = $('#plt-id').val();
    var deal = $('#Dealership').val();
    if (confirm("Are you sure you want to generate NOC of This Plot")) {
        window.open(`/Plots/DealerNOC?Id=${id}&Dealer_Id=${deal}`, '_blank');
        closeModal();
    }
});
$(document).on("click", ".up-pur-order", function () {
    var id = $(this).data("bididen");
    EmptyModel();
    $('#ModalLabel').text("Revise Purchase Order");
    $('#modalbody').load('/Procurement/Revise_PO/', { po: id });
});
//
$(document).on("click", ".up-ser-pur-order", function () {
    var id = $(this).data("bididen");
    EmptyModel();
    $('#ModalLabel').text("Revise Purchase Order");
    $('#modalbody').load('/Services/Revise_PO/', { po: id });
});
//
$(document).on("click", "#ref-amt", function () {
    var id = $(this).data("id");
    EmptyModel();
    $('#ModalLabel').text("Refund Amount");
    $('#modalbody').load('/Projects/CreateProject/');
    InventoryCounter = 1;
});
//
$(document).on("click", "#plot_file", function () {
    var id = $(this).data("id");
    window.open("/Plots/PlotCustomerFile?Plotid=" + id, '_blank');
});
//
$(document).on("click", "#com_file", function () {
    var id = $(this).data("id");
    window.open("/Commercial/CustomerFile?Commercial_Id=" + id, '_blank');
});
//
$(document).on("click", "#plot_file-sa", function () {
    var id = $(this).data("id");
    window.open("/Plots/PlotCustomer_App?Plotid=" + id, '_blank');
});
//
$(document).on("click", "#elec-rea-plt-lst", function () {
    var id = $("#PlotMeters").val();
    $("#plot-det").empty()
    $("#plot-det").load("/ServiceCharges/PlotsMeterList/", { Id: id });
});
//
$(document).on("click", "#serv-plt-lst", function () {
    var id = $("#Blocks").val();
    $("#plot-det").empty()
    $("#plot-det").load("/ServiceCharges/PlotsServiceChargesList/", { Id: id });
});
//....service charges report
$(document).on("click", "#servce-plt-lst", function () {
    var id = $("#serv-Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#plot-ser-det").empty();
        if (id != null) {
            $("#plot-ser-det").load("/Reports/ServiceCharges/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#plot-ser-det").load("/Reports/ServiceCharges/", { month: mm, year: yy });
        }
    }
});
//..... electricity charges report
$(document).on("click", "#electric-plt-lst", function () {
    var id = $("#Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#plot-elect-det").empty();
        if (id != null) {
            $("#plot-elect-det").load("/Reports/ElectricityChargesBill/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#plot-elect-det").load("/Reports/ElectricityChargesBill/", { month: mm, year: yy });
        }
    }
});
//....service charges report
$(document).on("click", "#servce-plt-lst-sht", function () {
    var id = $("#serv-Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#plot-ser-det").empty();
        if (id != null) {
            $("#plot-ser-det").load("/Reports/ServiceCharges_Short/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#plot-ser-det").load("/Reports/ServiceCharges_Short/", { month: mm, year: yy });
        }
    }
});
//..... electricity charges report
$(document).on("click", "#electric-plt-lst-sht", function () {
    var id = $("#elec-Blocks").val();
    var month = $("#months").val();
    var year = $("#year").val();
    if (month != '' && year != '') {
        var mm = year + '-' + month + '-' + '01';
        var yy = year + '-' + month + '-' + '01';
        $("#plot-elect-det").empty();
        if (id != null) {
            $("#plot-elect-det").load("/Reports/ElectricityCharges_Short/", { month: mm, year: yy, blockid: id });
        }
        else {
            $("#plot-elect-det").load("/Reports/ElectricityCharges_Short/", { month: mm, year: yy });
        }
    }
});
//....
$(document).on("keyup", ".read", function () {
    var cur = $(this).val();
    var prev = $(this).parent().siblings(".pre-read").text();
    if (cur < prev) {
        alert('Enter Correct Units');
        return false;
    }
    var uncon = parseFloat(cur) - parseFloat(prev);
    $(this).parent().siblings(".uni-cons").text(uncon);
});
//
//....
$(document).on("keyup", ".read__cur__readings", function () {
    var cur = $(this).val() * 1;
    var prev = $(this).parent().siblings(".pre-read").text() * 1;
    var uncon = 0;
    if (cur < prev) {
        $(this).parent().siblings(".uni-cons").text(uncon);
    }
    else {
        var id = $(this).closest('tr').attr('id');
        uncon = parseFloat(cur) - parseFloat(prev);
        $(this).parent().siblings(".uni-cons").text(uncon);
        $.post('/ServiceCharges/UpdateMeterReadings/', { Id: id, Reading: cur, Units: uncon }, function () {
        });
    }
});
//
// New Meter Readings management.......................
$(document).on("click", "#New__elec__rea__plt__lst", function () {
    var id = $("#PlotMeters").val();
    $("#plot__det__New").empty()
    $("#plot__det__New").load("/ServiceCharges/NewPlotsMeterList/", { Id: id });
});
//
function UnitsCalculation() {
    var status = true;
    $('.meter__read__table__check_res').each(function () {
        var NewUnits = RemoveComma($(this).closest('tr').find('.read__cur__readings').val()) * 1;
        var Preunits = RemoveComma($(this).closest('tr').find('.pre-read').attr('id')) * 1;
        //var zerochk = RemoveComma($(this).closest('tr').find('.uni-cons').text()) * 1;
        //if (zerochk == 0 || zerochk=="") {
        //    var id = $(this).closest('tr').attr('id');
        //    $('.result__com__res tbody tr#' + id).addClass("testbg");
        //    $('.result__com__res tbody tr#' + id).focus();
        //    lastElementTop = $('.result__com__res tbody tr#' + id).position().top;
        //    scrollAmount = lastElementTop - 200;
        //    $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
        //    status = false;
        //    return false;
        //}
        if (NewUnits < Preunits) {
            var id = $(this).closest('tr').attr('id');
            $('.result__com__res tbody tr#' + id).addClass("testbg");
            $('.result__com__res tbody tr#' + id).focus();
            lastElementTop = $('.result__com__res tbody tr#' + id).position().top;
            scrollAmount = lastElementTop - 200;
            $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
            status = false;
            return false;
        }
        else {
            status = true;
        }
    });
    if (status) {
        $('.meter__read__table__check_com').each(function () {
            var NewUnits = RemoveComma($(this).closest('tr').find('.read__cur__readings').val()) * 1;
            var Preunits = RemoveComma($(this).closest('tr').find('.pre-read').attr('id')) * 1;
            //var zerochk = RemoveComma($(this).closest('tr').find('.uni-cons').text()) * 1;
            //if (zerochk == 0) {
            //    var id = $(this).closest('tr').attr('id');
            //    $('.result__com__res tbody tr#' + id).addClass("testbg");
            //    $('.result__com__res tbody tr#' + id).focus();
            //    lastElementTop = $('.result__com__res tbody tr#' + id).position().top;
            //    scrollAmount = lastElementTop - 200;
            //    $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
            //    status = false;
            //    return false
            //}
            if (NewUnits < Preunits) {
                var id = $(this).closest('tr').attr('id');
                $('.result__com__res tbody tr#' + id).addClass("testbg");
                $('.result__com__res tbody tr#' + id).focus();
                lastElementTop = $('.result__com__res tbody tr#' + id).position().top;
                scrollAmount = lastElementTop - 200;
                $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
                status = false;
                return false
            }
            else {
                status = true;
            }
        });
    }
    return status
}
//....
$(document).on("click", "#Fin__meter__readings", function () {
    var ver__units = UnitsCalculation();
    var blkval = $(this).val();
    if (!ver__units) {
        alert('Wronge Entery Exists');
        return false;
    }
    var con = confirm("Are You sure you want to Finalize Units");
    if (con) {
        $.post('/ServiceCharges/GetUpdatedMeterReadings/', { Id: blkval }, function (data) {
            alert('Units are finalize');
            window.location.reload();
        });
    }
});
//
//
$(document).on("click", ".meter__read__table__check_res", function () {
    var id = $(this).attr('id');
    $('.result__com__res tbody tr#' + id).removeClass("testbg");
});
//
//
$(document).on("click", ".meter__read__table__check_com", function () {
    var id = $(this).attr('id');
    $('.result__com__res tbody tr#' + id).removeClass("testbg");
});
//
$(document).on("change", ".read", function () {
    var cur = $(this).val();
    var id = $(this).closest('tr').attr("id");
    $.post("/ServiceCharges/UpdateBillReading/", { Id: id, Reading: cur }, function () {
        $("#check-" + id).show();
        setTimeout(function () {
            $("#check-" + id).fadeOut();
        }, 1000);
    });
});
//
$(document).on("change", "#ch-stat", function () {
    var val = $(this).val();
    if (val == "Dishonored") {
        $("#dis-rea").show();
    }
    else {
        $("#dis-rea").hide();
    }
});
//
$(document).on("click", ".allo-sign", function () {
    var id = $(this).closest('tr').attr("id");
    var plotid = $(this).closest('tr').data("id");
    //var a = confirm("Are you sure you want to mark the Allotment Letter sign");
    //if (a) {
    Swal.fire({
        text: 'Are you sure you want to mark the allotment letter as signed?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Plots/SignAllotmentLetter/", { Id: id, PlotId: plotid }, function () {
                //alert("Signed");
                Swal.fire({
                    icon: 'success',
                    text: 'Allotment letter signed successfully'
                });
                $("#" + id).remove();
            });
        }
    });
});
//
$(document).on("click", ".allo-unsign", function () {
    var id = $(this).closest('tr').attr("id");
    var plotid = $(this).closest('tr').data("id");
    //var a = confirm("Are you sure you want to mark the Allotment Letter Un singed");
    //if (a) {
    Swal.fire({
        text: 'Are you sure you want to mark the Allotment Letter as unsigned?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Plots/UnsingedAllotmentLetter/", { Id: id, PlotId: plotid }, function () {
                //alert("Un Signed Successfully");
                Swal.fire({
                    icon: 'success',
                    text: 'Marked as unsigned successfully'
                }).then(() => {
                    $("#" + id).remove();
                })
            });
        }
    });
});
//
$(document).on("click", ".all-let-ref", function () {
    var id = $(this).closest('tr').attr("id");
    var plotid = $(this).closest('tr').attr("data-id");
    EmptyModel();
    $('#ModalLabel').text("Update Allotment Letter Information");
    $('#modalbody').load('/Plots/LetterInfomation/', { PlotId: plotid, Id: id });
    //var id = $(this).closest('tr').attr("id");
    //var plotid = $(this).closest('tr').data("id");
    //var a = confirm("Are you sure you want to update the Allotment Letter Information");
    //if (a) {
    //    $.post("/Plots/UpdateAllotmentLetterInfo/", { Id: id, PlotId: plotid }, function () {
    //        alert("Updated");
    //    });
    //}
});
//
$(document).on("click", ".all-let-ref-update", function () {
    var plotid = $('.plt_id').attr("id");
    var id = $('.Local__id').attr("id");
    var W1 = $('#W1__al').val();
    var W2 = $('#W2__al').val();
    if (W1 == null || W1 == "") {
        //alert('Please Enter name of witness 1');
        Swal.fire({
            icon: 'info',
            text: 'Enter the name of the first witness to proceed'
        });
        return false;
    }
    if (W2 == null || W2 == "") {
        //alert('Please Enter name of witness 2');
        Swal.fire({
            icon: 'info',
            text: 'Enter the name of the second witness to proceed'
        });
        return false;
    }
    //var id = $(this).closest('tr').attr("id");
    //var plotid = $(this).closest('tr').data("id");
    //var a = confirm("Are you sure you want to update the Allotment Letter Information");
    //if (a) {
    Swal.fire({
        text: 'Are you sure you want to update the information in the Allotment Letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Plots/UpdateAllotmentLetterInfo/", { Id: id, PlotId: plotid, Witness1: W1, Witness2: W2 }, function () {
                //alert("Updated");
                Swal.fire({
                    icon: 'success',
                    text: 'Information updated successfully'
                });
                closeModal();
            });
        }
    });
});
//
$(document).on("click", "#src-srch-bill", function () {
    var id = $(".billno-kdjfsh").val();
    $("#plt-serv-data").empty();
    $("#plt-serv-data").load("/ServiceCharges/ServiceChargesDetails/", { billNo: id });
});
//
$(document).on("click", "#src-elec-bill", function () {
    //var id = $("#plot-details").val();
    var metno = $("#met-no").val();
    if (metno) {
        $("#plt-serv-data").empty();
        $("#plt-serv-data").load("/ServiceCharges/ElectricityBillDetails/", { MeterNo: metno });
    }
    else {
        Swal.fire({
            icon: 'info',
            text: 'Enter Bill number to proceed'
        });
    }
});
////
$(document).on("click", "#Com-src-elec-bill", function () {
    //var id = $("#plot-details").val();
    var metno = $("#met-no").val();
    if (metno) {
        $("#plt-serv-data").empty();
        $("#plt-serv-data").load("/ServiceCharges/ComElectricityBillDetails/", { MeterNo: metno });
    }
    else{
        Swal.fire({
            icon: 'info',
            text: 'Enter bill number to proceed'
        });
    }
});
//
$(document).on("click", "#rec-serv", function () {
    var id = $('.billno-kdjfsh').val();
    EmptyModel();
    $('#ModalLabel').text("Service Charges");
    $('#modalbody').load('/ServiceCharges/PayServiceChargesBill/', { billNo: id });
});
//
$(document).on("click", "#rec-elec", function () {
    var id = $(this).data("id");
    var metno = $("#met-no").val();
    EmptyModel();
    $('#ModalLabel').text("Service Charges");
    $('#modalbody').load('/ServiceCharges/PayElectricityBill/', { Plotid: id, MeterNo: metno });
});
//
$(document).on("click", "#gen-serv-ch-rec", function (e) {
    var payable = RemoveComma($("#pay__amt").val());
    var bamount = RemoveComma($(".b__amt").attr("id"));
    var alpha = parseInt(payable);
    var beta = parseInt(bamount);
    if (alpha == "" || alpha == null) {
        //alert("Please Enter Amount");
        Swal.fire({
            icon: 'info',
            text: 'Please Enter a valid amount'
        });
        return false;
    }
    //if (alpha < beta) {
    //    alert("Amount should be equal or greater then  balance amount");
    //    return false;
    //}
    e.preventDefault();
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#gen-serv-ch-rec').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: $("#pay-ser-ch").attr('action'),
                data: $("#pay-ser-ch").serialize(),
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt generated successfully'
                    }).then(() => {
                        window.open("/Receipts/ServiceChargesReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                    })
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("submit", "#pay-con-char", function (e) {
    e.preventDefault();
    $('#pay-con-char-btn').attr("disabled", true);
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#pay-con-char").attr('action'),
                data: $("#pay-con-char").serialize(),
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt generated successfully'
                    }).then(() => {
                        window.open("/Receipts/NewConnctionCharges?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                    })
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("submit", "#pay-fin-char", function (e) {
    e.preventDefault();
    debugger
    $('#pay-fin-char-btn').attr("disabled", true);
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
    //Swal.fire({
    //    text: 'Are you sure you want to generate the Receipt?',
    //    icon: 'question',
    //    showCancelButton: true,
    //    confirmButtonText: 'Yes',
    //    cancelButtonText: 'No'
    //}).then((result) => {
    //    if (result.isConfirmed) {
    
                $.ajax({
                type: "POST",
                url: $("#pay-fin-char").attr('action'),
                data: $("#pay-fin-char").serialize(),
                success: function (data) {
                    if (!data) {
                        //alert("Error Occured");
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                    else {
                        Swal.fire({
                            icon: 'success',
                            text: 'Receipt generated successfully'
                        }).then(() => {
                            window.open("/Receipts/FineCharges?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                            window.location.reload();
                        });   
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
});
//
$(document).on("click", "#gen-elec-ch-rec", function (e) {
    var payable = RemoveComma($(".p__am").attr("id"));
    // var bamount = RemoveComma($("#paying__amt").val());
    //var alpha=  parseInt(payable);
    // var beta= parseInt(bamount);
    // if (beta == "" || beta == null)
    // {
    //     alert("Please enter amount");
    //     return false;
    // }
    // if (beta < alpha) {
    //     alert("Amount should be equal or greater then payable");
    //     return false;
    // }
    // e.preventDefault();
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#gen-elec-ch-rec').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: $("#pay-elec-bill").attr('action'),
                data: $("#pay-elec-bill").serialize(),
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt generated successfully'
                    }).then(() => {
                        window.open("/Receipts/ElectricChargesReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                        window.location.reload();
                    })
                    
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
////
$(document).on("click", "#gen-com-elec-ch-rec", function (e) {
    var payable = RemoveComma($(".p__am").attr("id"));
    // var bamount = RemoveComma($("#paying__amt").val());
    //var alpha=  parseInt(payable);
    // var beta= parseInt(bamount);
    // if (beta == "" || beta == null)
    // {
    //     alert("Please enter amount");
    //     return false;
    // }
    // if (beta < alpha) {
    //     alert("Amount should be equal or greater then payable");
    //     return false;
    // }
    // e.preventDefault();
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#gen-elec-ch-rec').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: $("#pay-com-elec-bill").attr('action'),
                data: $("#pay-com-elec-bill").serialize(),
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt generated successfully'
                    }).then(() => {
                        window.open("/Receipts/ElectricChargesReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                        window.location.reload();
                    })
                    
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-plot-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("click", "#plot_reinstate", function () {
    var id = $(this).data("id");
    var con = confirm("Are you sure you want to Reinstate this plot?");
    if (con) {
        $.post('/Plots/UpdatePlotDisputedStatus/', { Id: id, Status: false }, function (data) {
            alert("Plot Status Changed");
            window.location.reload();
        });
    }
});
//
$(document).on("click", ".re-ins", function () {
    EmptyModel();
    $('#modalbody').load('/FileSystem/ReinstateFile/');
    $('#ModalLabel').html("Plot Reinstation");
    $('.modal-footer').append('<button class="btn btn-success" id="re-ins-file" type="submit">Update Plot Status</button>');
});
//
$(document).on("click", ".re-ins-p", function () {
    EmptyModel();
    $('#modalbody').load('/Plots/ReinstatePlot/');
    $('#ModalLabel').html("Plot Reinstation");
    $('.modal-footer').append('<button class="btn btn-success" id="re-ins-plot" type="submit">Update Plot Status</button>');
});
//
$(document).on("click", ".re-ins-c", function () {
    EmptyModel();
    $('#modalbody').load('/Commercial/ReinstateUnit/');
    $('#ModalLabel').html("Unit Reinstation");
    $('.modal-footer').append('<button class="btn btn-success" id="re-ins-com" type="submit">Update Plot Status</button>');
});
//
$(document).on("click", "#re-ins-file", function () {
    var id = $("#file-id").val();
    var val = $("#re-val").val();
    var rem = $("#remark").val();
    if (val == "") {
        alert("Please Select an option")
        return false;
    }
    if (rem == "") {
        alert("Please Enter the Remarks")
        return false;
    }
    var filenum = $("#plt-no").text();
    if (confirm("Are you sure you want to update the Status")) {
        if (val == 1) {
            $.post('/FileSystem/FileReinstate/', { Id: id, Remarks: rem }, function (data) {
                alert("Plot Status Changed");
                closeModal();
                $("#file-details").load('/FileSystem/GetFileDetails/', { FileId: filenum });
            });
        }
        //else if (val == 2) {
        else {
            $.post('/FileSystem/ReinstateWithTBA/', { Id: id, Remarks: rem, ReinstateType: val }, function (data) {
                alert(data.Msg);
                closeModal();
                $("#file-details").load('/FileSystem/GetFileDetails/', { FileId: filenum });
            });
        }
    }
});
//
$(document).on("click", "#re-ins-plot", function () {
    var id = $("#plt-id").val();
    var val = $("#re-val").val();
    var rem = $("#remark").val();
    if (val == "") {
        alert("Please Select an option")
        return false;
    }
    if (rem == "") {
        alert("Please Enter the Remarks")
        return false;
    }
    if (confirm("Are you sure you want to update the Status")) {
        if (val == 1) {
            $.post('/Plots/PlotReinstate/', { Id: id, Remarks: rem }, function (data) {
                alert("Plot Status Changed");
                closeModal();
                $("#plot-det").load("/Plots/PlotDetails/", { Plotid: id }, function () {
                    SASUnLoad($("#plot-det"));
                    $('html, body').animate({
                        scrollTop: $("#plot-det").offset().top
                    }, 1000);
                });
            });
        }
        //else if (val == 2) {
        else {
            $.post('/Plots/ReinstateWithTBA/', { Id: id, Remarks: rem, ReinstateType: val }, function (data) {
                alert(data.Msg);
                closeModal();
                $("#plot-det").load("/Plots/PlotDetails/", { Plotid: id }, function () {
                    SASUnLoad($("#plot-det"));
                    $('html, body').animate({
                        scrollTop: $("#plot-det").offset().top
                    }, 1000);
                });
            });
        }
    }
});
//
$(document).on("click", "#re-ins-com", function () {
    var id = $("#shp-id").val();
    var val = $("#re-val").val();
    var rem = $("#remark").val();
    if (val == "") {
        alert("Please Select an option")
        return false;
    }
    if (rem == "") {
        alert("Please Enter the Remarks")
        return false;
    }
    if (confirm("Are you sure you want to update the Status")) {
        if (val == 'Registered') {
            $.post('/Commercial/UnitReinstate/', { Id: id, Remarks: rem }, function (data) {
                alert("Unit Status Changed");
                closeModal();
            });
        }
    }
});
////
//$(document).on("click", "#sea-com-file", function () {
//    var proj = $("#Project").val();
//    var app = $("#app-fil-id").val();
//    $.ajax({
//        type: "POST",
//        url: "/Commercial/SearchComFile/",
//        data: { ProjectId: proj, FormId: app },
//        success: function (data) {
//            if (data.length <= 0) {
//                alert("No Result Found");
//                return false;
//            }
//            $("#area").val(data.Area);
//            $("#type").val(data.Type);
//            $("#loc").val(data.Location);
//            $("#com-id").val(data.Comercial_Id);
//            $("#floor").val(data.Floor);
//            $('#qr_img').attr("src", '/QR Codes/' + data.Comercial_Id + '_' + data.Floor_Id + '.png');
//            var totval = data.Total_SqFt_Marlas * data.Final_Rate;
//            var adv = (totval * 25) / 100;
//            var bok = (totval * 5) / 100;
//            $("#adv").val(adv);
//            $("#bok").val(bok);
//            $("#adv-pmt").text(Number(adv).toLocaleString());
//        }
//        , error: function (xmlhttprequest, textstatus, message) {
//            if (textstatus === "timeout") {
//                alert("got timeout");
//            } else {
//                alert(textstatus);
//            }
//        }
//    });
//});
//
$(document).on("change", "#book-stat", function (e) {
    var token = $("#book-stat").is(":checked");
    if (token) {
        $("#adv-pmt").text(Number($("#bok").val()).toLocaleString());
    }
    else {
        $("#adv-pmt").text(Number($("#adv").val()).toLocaleString());
    }
});
//
//$(document).on("submit", "#re-com", function (e) {
//    e.preventDefault();
//    var advamt = parseFloat(RemoveComma($("#adv-pmt").text()));
//    var totalamt = 0
//    $(".amt").each(function () {
//        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
//    });
//    if (advamt != totalamt) {
//        alert("Received Amount is not equal to Booking Amount");
//        return false;
//    }
//    var token = $("#book-stat").is(":checked")
//    var flag = true;
//    for (var i = 1; i <= paycont; i++) {
//        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
//        if (cash_che_bank !== "Cash") {
//            flag = false;
//        }
//    }
//    var allrecepts = []
//    for (var i = 1; i <= paycont; i++) {
//        var recedata = {
//            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
//            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", Block: ""
//        };
//        recedata.Amount = $("#pay-" + i + " #Amount").val();
//        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
//        recedata.Bank = $("#pay-" + i + " #bank").val();
//        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
//        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
//        recedata.Project_Name = $("#Project option:selected").text();
//        recedata.Branch = $("#pay-" + i + " #Branch").val();
//        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
//        recedata.Block = $("#floor").val();
//        allrecepts.push(recedata);
//    }
//    $("#reg-file").prop("disabled", true);
//    var fileno = $("#app-num").val();
//    var regfiledata = {
//        Id: 0,
//        Name: $("input[name=Name]").val(),
//        Father_Husband: $("input[name=Father_Husband]").val(),
//        Postal_Address: $("input[name=Postal_Address]").val(),
//        Residential_Address: $("input[name=Residential_Address]").val(),
//        Phone_Office: $("input[name=Phone_Office]").val(),
//        Residential: $("input[name=Residential]").val(),
//        Mobile_1: $("input[name=Mobile_1]").val(),
//        Mobile_2: $("input[name=Mobile_2]").val(),
//        Email: $("input[name=Email]").val(),
//        Occupation: $("input[name=Occupation]").val(),
//        Nationality: $("input[name=Nationality]").val(),
//        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
//        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
//        Nominee_Name: $("input[name=Nominee_Name]").val(),
//        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
//        Nominee_Address: $("input[name=Nominee_Address]").val(),
//        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
//        ComRom_Id: $("#com-id").val(),
//        City: $("#City").val(),
//        Delivery: 0,
//    }
//    var alldata = {
//        comdata: regfiledata,
//        Flag: flag,
//        ComRom_Num: $("#app-fil-id").val(),
//        Receiptdata: allrecepts,
//        Token: token
//    };
//    var con = confirm("Are you sure you want to Generate Receipt");
//    if (con) {
//        $.ajax({
//            type: "POST",
//            url: $(this).attr("action"),
//            contentType: "application/json; charset=utf-8",
//            data: JSON.stringify(alldata),
//            success: function (data) {
//                if (data.Status == "-1") {
//                    alert("No Installment Plan Has Found Contact Administrator")
//                }
//                if (data.Status == "0") {
//                    alert("Already a Plan is Generated");
//                }
//                else if (data.Status == "1") {
//                    alert("File No. " + fileno + " Has Been Registerd");
//                    $(data.Receiptid).each(function (i) {
//                        window.open("/Receipts/FaisalHeight?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
//                    });
//                    window.location.reload();
//                }
//                else if (data.Status == "2") {
//                    alert("Wait Until You Payment is Clear from Bank");
//                    $(data.Receiptid).each(function (i) {
//                        window.open("/Receipts/FaisalHeight?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
//                    });
//                    window.location.reload();
//                }
//            }
//            , error: function (xmlhttprequest, textstatus, message) {
//                if (textstatus === "timeout") {
//                    alert("got timeout");
//                } else {
//                    alert(textstatus);
//                }
//            }
//        });
//    }
//});
//
$(document).on("click", "#sear-con", function (e) {
    var id = $("#plot-details").val();
    var con = $("#conn-type").val();
    $("#plot-det").empty();
    $("#plot-det").load("/ServiceCharges/GetCustomerData/", { PlotId: id, Connection: con });
});
//
$(document).on("click", "#sear-fin", function (e) {
    var id = $("#plot-details").val();
    $("#plot-det").empty();
    $("#plot-det").load("/ServiceCharges/FineResult/", { PlotId: id });
});
//
$(document).on("click", "#sear-plt-short", function (e) {
    var id = $("#plot-details").val();
    $("#plot-det").empty();
    $("#plot-det").load("/Plots/PlotsShortResult/", { PlotId: id });
});
//
$(document).on("click", "#sear-file-short", function (e) {
    var id = $("#file_num").val();
    $("#plot-det").empty();
    $("#plot-det").load("/FileSystem/FileShortResult/", { FileNumber: id });
});
//
$(document).on("click", "#sear-com-short", function (e) {
    var id = $("#file_num").val();
    $("#plot-det").empty();
    $("#plot-det").load("/Commercial/ComShortResult/", { FileNumber: id });
});
//adjust-rec
$(document).on("click", "#sear-file-char", function (e) {
    var id = $("#file_num").val();
    $("#plot-det").empty();
    $("#plot-det").load("/ServiceCharges/FileResult/", { FileNumber: id });
});
//
$(document).on("click", ".up-del-info", function () {
    EmptyModel();
    var a = $(this).attr("id");
    $('#ModalLabel').text("Dealer info Update");
    $('#modalbody').load('/Dealership/UpdateInfo/', { Id: a });
});
//
$(document).on("change", ".blk-plotss", function () {
    var id = $(".blk-plotss").val();
    var type = $('#plot-type').val();
    $('#plot-details').empty();
    $.ajax({
        traditional: true,
        type: "POST",
        data: { Id: id, Type: type },
        url: "/Plots/GetPlots/",
        success: function (data) {
            $('#plot-details').append('<option value="">Select Plot</option>');
            $.each(data, function (key, value) {
                $('#plot-details').append('<option value=' + value.Id + '>' + value.Plot_No + '</option>');
            });
        },
        error: function () {
        }
    });
});
//
$(document).on("click", "#ad-rec-adj-req", function () {
    var blocknam = $(".block-nam option:selected").text();
    var recid = $("#rece-id").val();
    var reason = $("#reason").val();
    var paymenttype = $("#paytype").text();
    var from = $("#fromplt").text();
    var to = $("#plt-no").text();
    var to_Name = $("#nam").text();
    var to_id = $("#PlotId").val();
    var mod = $("#module").val();
    var tmod = $(".modu :selected").val();
    if (confirm("Are you sure you want to generate Adjustment request")) {
        $.ajax({
            type: "POST",
            url: "/Finance/AdjustReq/",
            data: { ReceiptId: recid, Reason: reason, PaymentType: paymenttype, From: from, To: to, To_Id: to_id, To_Name: to_Name, Block: blocknam, Module: mod, FromModule: mod, ToMod: tmod },
            success: function (data) {
                alert("Requested");
                $("#ad-rec-adj-req").attr("Disabled", true);
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
function ShowFiles(date) {
    var data = files.filter(x => x.Date == date);
    var html = '<table class="table table-striped table-bordered"><thead><tr> <th style="text-align:center" colspan="4">' + date + '</th></tr><tr><th>File Number</th><th>Plot Size</th><th>Status</th><th>Development Charges</th></tr></thead><tbody id="file-tab"></tbody></table>';
    $('#assigne-fil').html(html);
    $(data).each(function (a) {
        var dev = "";
        var col = "";
        if (data[a].status == "Pending") { }
        if (data[a].Development_Charges == true) { dev = "With Development Charges"; } else if (data[a].Development_Charges == false) { dev = "Non Development Charges"; }
        var tr = '<tr><td>' + data[a].FileFormNumber + '</td><td>' + data[a].Plot_Size + '</td><td>' + data[a].Status + ' - ' + data[a].Reg_Date + '</td><td>' + dev + '</td></tr>';
        $('#assigne-fil tbody').append(tr);
    });
}
//
$(document).on('click', '#imagesfile', function () {
    cn = $(this).attr('data-cn');
})
//.....image upload..
function show(input) {
    if (input.files && input.files[0]) {
        var filerdr = new FileReader();
        filerdr.onload = function (e) {
            $('#uploadedimage-' + cn).attr('src', e.target.result);
            $('#bs64-' + cn).val(e.target.result);
        }
        filerdr.readAsDataURL(input.files[0]);
    }
}
//
$(document).on("click", "#add-part", function () {
    var html = '<div class="form-row"><div class="form-group col-md-2"><label>Name</label><input type="text" id="ndel-name" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>Father Name</label><input id="ndel-fname" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>CNIC</label><input id="ndel-cnic" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>City</label><input id="ndel-city" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>Date of Birth</label><input id="ndel-dob" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>Mobile 1</label><input id="ndel-mob1" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>Mobile 2</label><input id="ndel-mob2" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-4"><label>Address</label><input id="ndel-add" type="text" class="form-control"></div>' +
        '<div class="form-group col-md-2"><label>------</label><button type="button" id="del-save" class="btn btn-info">Save</button></div></div>';
    $('#deler-info').append(html);
});
//
$(document).on("click", "#del-update", function () {
    var id = $("#deal-id").val();
    var name = $("#deal-nam").val();
    var date = $("#reg-dat").val();
    var stat = $("#stat").val();
    $.ajax({
        type: "POST",
        url: '/Dealership/UpdateDealership/',
        data: { Id: id, Dealername: name, Status: stat, Date: date },
        success: function (data) {
            alert("Dealership Status Updated")
            window.location.reload();
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", "#del-save", function () {
    var id = $("#deal-id").val();
    var name = $("#ndel-name").val();
    var fnam = $("#ndel-fname").val();
    var cnic = $("#ndel-cnic").val();
    var city = $("#ndel-city").val();
    var dob = $("#ndel-dob").val();
    var mob1 = $("#ndel-mob1").val();
    var mob2 = $("#ndel-mob2").val();
    var add = $("#ndel-add").val();
    $.ajax({
        type: "POST",
        url: '/Dealership/AddDealerInfo/',
        data: { Dealership_Id: id, Name: name, Father_Name: fnam, CNIC_NICOP: cnic, Mobile_1: mob1, Mobile_2: mob2, Address: add, City: city, Date_Birth: dob },
        success: function (data) {
            if (data) {
                alert("Dealer added");
                $("#deler-info").load("/Dealership/DealersData/", { Id: id });
            }
            else {
                alert("This Dealer is already Exist");
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".updat-deal", function () {
    var id = $(this).parent().parent().attr("id");
    $("#" + id + " :input").prop("readonly", false);
    $('.del-btn-' + id).show();
    $("#" + id + " .updat-deal").hide();
    $("#" + id + " .sav-deal").show();
});
//
$(document).on("click", ".sav-deal", function () {
    var id = $(this).parent().parent().attr("id");
    var name = $("#" + id + " #del-name").val();
    var fnam = $("#" + id + " #del-fname").val();
    var cnic = $("#" + id + " #del-cnic").val();
    var city = $("#" + id + " #del-city").val();
    var dob = $("#" + id + " #del-dob").val();
    var mob1 = $("#" + id + " #del-mob1").val();
    var mob2 = $("#" + id + " #del-mob2").val();
    var add = $("#" + id + " #del-add").val();
    var dealid = $("#deal-id").val();
    var img = $("#bs64-" + id).val();
    $.ajax({
        type: "POST",
        url: '/Dealership/UpdateDealerInfo/',
        data: { Id: id, Name: name, Father_Name: fnam, CNIC_NICOP: cnic, Mobile_1: mob1, Mobile_2: mob2, Address: add, City: city, Date_Birth: dob, img: img },
        success: function (data) {
            if (data) {
                alert("Dealer information Updated");
                $("#deler-info").load("/Dealership/DealersData/", { Id: dealid });
            }
            else {
                alert("This Dealer is already Exist");
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", "#dealer-trans", function (e) {
    var token = $("#dealer-trans").is(":checked");
    if (token) {
        $("#Dealership").show();
        $("input[name=Dealer_Req]").val(true);
    }
    else {
        $("#Dealership").hide();
        $(".dea-info").hide();
        $("input[name=Dealer_Req]").val(false);
    }
});
//
$(document).on("change", "#emp-trans", function (e) {
    var token = $("#emp-trans").is(":checked");
    if (token) {
        $("input[name=Employee_Req]").val(true);
    }
    else {
        $("input[name=Employee_Req]").val(false);
    }
});
//
$(document).on("change", ".dealers-detail", function () {
    var dealershipid = $(this).val();
    $.ajax({
        type: "POST",
        url: '/Dealership/GetDealerNames/',
        data: { Id: dealershipid },
        success: function (data) {
            $('#dealers').empty();
            $(".dea-info").show();
            $(".up-del-info").attr("id", dealershipid);
            $('#dealers').append('<option value="">Select Dealer</option>');
            $.each(data, function (key, value) {
                $('#dealers').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", ".dealers-detail", function () {
    var dealershipid = $(this).val();
    $.ajax({
        type: "POST",
        url: '/Dealership/GetDealerNames/',
        data: { Id: dealershipid },
        success: function (data) {
            $('#dealers-bid').empty();
            $(".dea-info").show();
            $(".up-del-info").attr("id", dealershipid);
            $('#dealers-bid').append('<option value="">Select Dealer</option>');
            $.each(data, function (key, value) {
                $('#dealers-bid').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", "#dealers", function (e) {
    var deal = $(this).val();
    if (deal == null || deal == '') {
        return;
    }
    $.ajax({
        type: "POST",
        url: '/Dealership/GetDealerDetails/',
        data: { Id: deal },
        success: function (data) {
            $(".autfil-name").val(data.Name);
            $(".autfil-father").val(data.Father_Name);
            $(".autfil-postal").val(data.Address);
            $(".autfil-moba").val((data.Mobile_1 == null) ? data.Mobile_1 : (data.Mobile_1).replace(' ', ''));
            $(".autfil-mobb").val((data.Mobile_2 == null) ? data.Mobile_2 : (data.Mobile_2).replace(' ', ''));
            $(".autfil-city").val(data.City);
            $(".autfil-dob").val(moment(data.Date_Birth).format("MM/DD/YYYY"));
            $(".autfil-cnic").val(data.CNIC_NICOP);
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", "#dealers-bid", function (e) {
    debugger;
    var deal = $(this).val();
    if (deal == null || deal == '') {
        return;
    }
    $.ajax({
        type: "POST",
        url: '/Dealership/GetDealerDetails/',
        data: { Id: deal },
        success: function (data) {
            $("input[name=Name]").val($("#Dealership option:selected").text());
            $("input[name=Father_Husband]").val(data.Name);
            $("input[name=Postal_Address]").val(data.Address);
            $("input[name=Residential_Address]").val(data.Address);
            $("input[name=Mobile_1]").val((data.Mobile_1 == null) ? data.Mobile_1 : (data.Mobile_1).replace(' ', ''));
            $("input[name=Mobile_2]").val((data.Mobile_2 == null) ? data.Mobile_2 : (data.Mobile_2).replace(' ', ''));
            $("input[name=City]").val(data.City);
            $("input[name=Date_Of_Birth]").val(moment(data.Date_Birth).format("MM/DD/YYYY"));
            $("input[name=CNIC_NICOP]").val(data.CNIC_NICOP);
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("change", "#bl-rel", function (e) {
    var token = $("#bl-rel").is(":checked");
    if (token) {
        $("#blod-rel").val(true);
    }
    else {
        $("#blod-rel").val(false);
    }
});
//
$(document).on("change", "#tran-ch", function (e) {
    var token = $("#tran-ch").is(":checked");
    if (token) {
        $("#Trans-val").val(true);
        $('.remarks').show();
    }
    else {
        $("#Trans-val").val(false);
        $('.remarks').hide();
    }
});
//
$(document).on("change", "#oth-tran-ch", function (e) {
    var token = $("#oth-tran-ch").is(":checked");
    if (token) {
        $("#oth-Trans-val").val(true);
        $('.remarks').show();
        $('.tran-amt').show();
    }
    else {
        $("#oth-Trans-val").val(false);
        $('.remarks').hide();
        $('.tran-amt').hide();
    }
});
// Search the File information
$(document).on("submit", "#sea-plt-info", function (e) {
    e.preventDefault();
    var seaopt = $('input[name=SearchOption]').val();
    $.ajax({
        type: "POST",
        url: $('#sea-plt-info').attr("action"),
        data: $('#sea-plt-info').serialize(),
        success: function (data) {
            $('#tab-data').parent().show();
            if (data.length > 0) {
                var html1 = '<table class="table table-striped table-bordered" style="margin-bottom:0px" cellspacing="0" width="100%"><thead class="thead-dark">' +
                    '<tr><th width="10">Sr.</th><th width="80">Plot No</th><th width="150">Block</th><th width="120">Size</th><th width="140">Type</th><th width="340">Name</th><th width="200">Location</th><th width="150">Dimension</th><th width="130">Status</th><th width="100">Verified</th></tr></thead></table>' +
                    '<div style="min-height:0px; max-height:400px;overflow:auto"><table class="table table-striped table-bordered" cellspacing="0" width="100%"><thead class="thead-dark"><tbody></tbody></table></div>';
                $('#tab-data').html(html1);
                var srcount = 1;
                $(data).each(function (i) {
                    var pltsize = MarlatoKanal(data[i].Plot_Size);
                    var veri = (data[i].Verified == true) ? "Verified" : "Not Verified";
                    if (data[i].Ownership_Status == 'Owner') {
                        var html = '<tr class="pointer plt-details" data-id="' + data[i].Id + '"><td width="10">' + srcount + '</td><td width="80">' + data[i].Plot_No + '</td><td width="150">' + data[i].Block_Name + '</td><td width="120">' + pltsize + '</td><td width="140">' + data[i].Type + '</td><td width="340">' + data[i].Name + '</td><td width="200">' + data[i].Plot_Location + '</td><td width="150">' + data[i].Dimension + '</td><td width="130">' + data[i].Status + '</td><td width="100">' + veri + '</td></tr>';
                    }
                    else {
                        var html = '<tr class="pointer plt-details" data-id="' + data[i].Id + '"><td width="10">' + srcount + '</td><td width="80">' + data[i].Plot_No + '</td><td width="150">' + data[i].Block_Name + '</td><td width="120">' + pltsize + '</td><td width="140">' + data[i].Type + '</td><td width="340">' + data[i].Name + ' (Previous Owner)</td><td width="200">' + data[i].Plot_Location + '</td><td width="150">' + data[i].Dimension + '</td><td width="130">' + data[i].Status + '</td><td width="100">' + veri + '</td></tr>';
                    }
                    $('#tab-data tbody').append(html);
                    srcount++;
                });
            }
            else {
                var html = '<h6>No Result Found</h6>';
                $('#tab-data').html(html);
            }
            $('#tab-data').html(html);
        },
        error: function (data) {
        }
    });
});
//
$(document).on("click", ".up-stat", function () {
    EmptyModel();
    var id = $("#plt-id").val();
    var stat = $("#plt-stat").val();
    $('#modalbody').load('/Plots/PlotUpStatus/', { PlotId: id, Status: stat });
});
//
$(document).on("click", ".up-stat-c", function () {
    EmptyModel();
    var id = $("#plt-id").val();
    var stat = $("#shp-stat").val();
    $('#modalbody').load('/Commercial/ShopUpdateStatus/', { ShopId: id, Status: stat });
});
//
$(document).on("click", "#ref-amt-plt", function () {
    var pltid = 0;
    var id = $('#receipt-id').val();
    var refamt = $('.ref-amt').val();
    if (refamt <= 0 || refamt == '' || refamt == null) {
        $('.coma').focus();
        return false;
    }
    var mod = $('#module').val();
    if (mod == "PlotManagement") {
        pltid = $("#plt-id").val();
    }
    else if (mod == "Building") {
        pltid = $("#shp-id").val();
    }
    var receamt = RemoveComma($('#rec-amt').text());
    //if (receamt < refamt) {
    //    alert("Refund Amount should be less than or Equal to Receipt Amount")
    //    return false;
    //}
    //var inp = confirm("Are you sure you want to Refund this Receipt");
    //if (inp == true) {
    Swal.fire({
        text: 'Are you sure you want to refund the receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Installments/RefundAmountPlot/", { Id: id, Amount: refamt }, function (data) {
                if (data.Status) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt refund successful'
                    }).then(() => {
                        if (mod == "PlotManagement") {
                            window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        }
                        else if (mod == "CommercialManagement") {
                            window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        }
                    })
                    
                }
                else {
                    //alert(data.Msg);
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
//
$(document).on("click", "#extra-ref-amt-plt", function () {
    var pltid = 0;
    var id = $('#receipt-id').val();
    var refamt = $('.ref-amt').val();
    if (refamt <= 0 || refamt == '' || refamt == null) {
        $('.coma').focus();
        return false;
    }
    var mod = $('#module').val();
    if (mod == "PlotManagement") {
        pltid = $("#plt-id").val();
    }
    else if (mod == "CommercialManagement") {
        pltid = $("#shp-id").val();
    }
    var receamt = RemoveComma($('#rec-amt').text());
    //if (receamt < refamt) {
    //    alert("Refund Amount should be less than or Equal to Receipt Amount")
    //    return false;
    //}
    //var inp = confirm("Are you sure you want to Refund this Receipt");
    //if (inp == true) {
    Swal.fire({
        text: 'Are you sure you want to refund the receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Installments/ExtraRefundAmountPlot/", { Id: id, Amount: refamt }, function (data) {
                debugger
                if (data.Status) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt refund successful'
                    }).then(() => {
                        if (mod == "PlotManagement") {
                            window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        }
                        else if (mod == "CommercialManagement") {
                            window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        }
                    })
                }
                else {
                    //alert(data.Msg);
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#up-plt-stat", function (e) {
    e.preventDefault();
    Swal.fire({
        text: 'Are you sure you want to update plot status?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#up-plot-stat').attr("action"),
                data: $('#up-plot-stat').serialize(),
                success: function (data) {
                    if (data.Status) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        });
                    }
                    else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'info',
                            text: data.Msg
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#up-com-stat", function (e) {
    e.preventDefault();
    Swal.fire({
        text: 'Are you sure you want to update the status?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#up-com-sta').attr("action"),
                data: $('#up-com-sta').serialize(),
                success: function (data) {
                    if (data.Status === "Requested") {
                        //alert("Cancelation Requested");
                        Swal.fire({
                            icon: 'info',
                            text: 'Status update is already requested'
                            })
                    }
                    if (data.Status === "Repurchased") {
                        //alert("Repurchased and Cancelation Requested");
                        Swal.fire({
                            icon: 'info',
                            text: 'Repurchased with a request for cancellation'
                            })
                    }
                    else {
                        //alert("Updated")
                        Swal.fire({
                            icon: 'success',
                            text: 'Status updated successfully'
                            })
                    }
                },
                error: function (data) {
                }
            });
        }
    });
});
//
$(document).on("click", ".plot-can-det", function () {
    var id = $(this).attr("id");
    var pltid = $(this).attr("data-pltid");
    window.location = "/Plots/PlotCancellationDetails?Plotid=" + pltid + "&Id=" + id;
});
//
$(document).on("click", ".com-can-det", function () {
    var id = $(this).attr("id");
    var comid = $(this).data("comid");
    window.location = "/Commercial/CommercialCancellationDetails?Commercial_Id=" + comid + "&Id=" + id;
});
//
$(document).on("click", ".plot-ref-det", function () {
    var id = $(this).attr("id");
    var pltid = $(this).attr("data-pltid");
    window.location = "/Plots/PlotReceiptRefundDetails?Plotid=" + pltid + "&ReqId=" + id;
});
//
$(document).on("click", ".file-can-det", function () {
    var id = $(this).attr("id");
    var pltid = $(this).attr("data-pltid");
    window.location = "/FileSystem/FileCancellationDetails?FileId=" + pltid + "&Id=" + id;
});
//
$(document).on("click", ".frd-plt-ref", function () {
    var id = $("#can-req-id").val();
    //if (confirm("Are you sure you want to Approve the refund")) {
    Swal.fire({
        text: 'Are you sure you want to proceed with the receipt refund request?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Plots/ApproveRefund/",
                data: { Id: id },
                success: function (data) {
                    //alert("Updated and Requested to Finance");
                    Swal.fire({
                        icon: 'success',
                        text: 'Receipt refund request updated and forwarded to the finance department for processing'
                    }).then(() => {
                        window.location.reload();
                    });
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});

//
$(document).on("click", ".show-plt-can", function () {
    debugger
    EmptyModel();
    var type = $("#type").val();
    var ttl_pric = $("#ttl-pric").val();
    //var rec_amt = $("#plt-rec-amt").val();
    var rec_amt = $("#rec-amt").val();
    if (type == "Repurchased") {
        var html = '<div class="form-row"><div class="form-group col-md-2"><label>Approval Status</label><select class="form-control" id="stat"><option >Choose..</option><option value="true">Approved</option>' +
            '<option value="false">Rejected</option></select></div><div class="form-group col-md-2"><label>Repurchased Amount</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="amt" name="Amount" class="repur-amt amt" required></div>' +
            '<div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    else {
        var html = '<input type="hidden" value="' + rec_amt + '" id="ttl-pric"><div class="form-row"><div class="form-group col-md-2"><label>Approval Status</label><select class="form-control" id="stat"><option value="" >Choose..</option><option value="true">Approved</option>' +
            '<option value="false">Rejected</option></select></div><div class="form-group col-md-2"><label>Deducation (%)</label><input class="form-control ded-per-cal" type="text"/></div>' +
            '<div class="form-group col-md-2"><label>Received Amountt</label><h6 id="rec-amt">' + Number(rec_amt).toLocaleString() + '</h6></div><div class="form-group col-md-2">' +
            '<label>Deducted Amount</label><h6 id="ded-amt"></h6></div><div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="can-plt" type="submit">Cancel Plot</button>');
});
//
$(document).on("click", ".show-com-can", function () {
    EmptyModel();
    var type = $("#type").val();
    var shtype = $("#shp-type").text();
    var ttl_pric = $("#ttl-pric").val();
    var rec_amt = $("#rec-amt").val();
    if (type == "Repurchased") {
        var html = '<div class="form-row"><div class="form-group col-md-2"><label>Approval Status</label><select class="form-control" id="stat"><option >Choose..</option><option value="true">Approved</option>' +
            '<option value="false">Rejected</option></select></div><div class="form-group col-md-2"><label>Repurchased Amount</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="amt" name="Amount" class="repur-amt amt" required></div>' +
            '<div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    else {
        var html = '<input type="hidden" value="' + rec_amt + '" id="ttl-pric"><div class="form-row"><div class="form-group col-md-2"><label>Approval Status</label><select class="form-control" id="stat"><option value="" >Choose..</option><option value="true">Approved</option>' +
            '<option value="false">Rejected</option></select></div><div class="form-group col-md-2"><label>Deducation (%)</label><input class="form-control ded-per-cal" type="text"/></div>' +
            '<div class="form-group col-md-2"><label>Received Amountt</label><h6 id="rec-amt">' + Number(rec_amt).toLocaleString() + '</h6></div><div class="form-group col-md-2">' +
            '<label>Deducted Amount</label><h6 id="ded-amt"></h6></div><div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    $('#modalbody').html(html);
    $('.modal-footer').append(`<button class="btn btn-success" id="can-com" type="submit">Cancel ${shtype}</button>`);
});
//
$(document).on("click", ".show-file-can", function () {
    EmptyModel();
    var type = $("#type").val();
    var ttl_pric = $("#ttl-pric").val();
    var rec_amt = $("#rec-amt").val();
    if (type == "Repurchased") {
        var html = '<div class="form-row"><div class="form-group col-md-2"><label>Repurchased Amount</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="amt" name="Amount" class="repur-amt amt" required></div>' +
            '<div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    else {
        var html = '<div class="form-row"><div class="form-group col-md-2"><label>Deducation (%)</label><input class="form-control ded-per-cal" type="text"/></div>' +
            '<div class="form-group col-md-2"><label>Received Amountt</label><h6 id="rec-amt">' + Number(rec_amt).toLocaleString() + '</h6></div><div class="form-group col-md-2">' +
            '<label>Deducted Amount</label><h6 id="ded-amt"></h6></div><div class="form-group col-md-4"><label>Remarks</label><textarea class="form-control" id="Remarks"></textarea></div></div>';
    }
    $('#modalbody').html(html);
    $('#ModalLabel').text("File Cancelation");
    $('.modal-footer').append('<button class="btn btn-success" id="can-file" type="submit">Cancel File</button>');
});
//
$(document).on("keyup", ".ded-per-cal", function () {
    var per = $(this).val();
    var rec_amt = $("#ttl-pric").val();
    var a = (rec_amt * per) / 100;
    $("#ded-amt").text(Number(parseFloat(a).toFixed(0)).toLocaleString());
});
//
$(document).on('click', '.new-aloc', function () {
    var newp = $('#n-pltnos').val();
    var fileid = $('#PlotId').val();

    if (newp == null) {
        alert("Please Select Plot");
        return false;
    }

    if (confirm("Are you sure you want to move Plots")) {

        $.ajax({
            type: "POST",
            url: "/Balloting/NewPlotAllocation/",
            data: { File_Id: fileid, New_Id: newp },
            success: function (data) {
                alert("Allocated");
                window.location.reload();
            }
        });
    }
});
//
$(document).on("click", ".rece-refund-pop", function () {
    EmptyModel();
    var id = $(this).closest('tr').attr('id');
    var num = $("#" + id + " .rece-num").text();
    var amount = $("#" + id + " .rece-amt").text();
    var type = $("#" + id + " .rece-type").text();
    var html = '<div class="row"><input type="hidden" value="' + id + '" id="receipt-id">' +
        '<div class="form-group col-md-2"><label>Receipt No.</label><h6>' + num + '</h6></div>' +
        '<div class="form-group col-md-2"><label>Amount</label><h6 id="rec-amt">' + amount + '</h6></div><div class="form-group col-md-2">' +
        '<label>Type</label><h6 id="ded-amt">' + type + '</h6></div><div class="form-group col-md-3"><label>Amount To Refund</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="amt" name="Amount" class="ref-amt amt" required></div></div>';
    $('#modalbody').html(html);
    $('#ModalLabel').text("Refund Amount");
    $('.modal-footer').append('<button class="btn btn-success" id="ref-amt-plt" type="submit">Request for Refund</button>');
});
//
$(document).on("click", "#can-file", function () {
    var id = $("#can-req-id").val();
    var pltid = $("#file-id").val();
    var filnum = $("#plt-no").text();
    var remark = $("#Remarks").val();
    var stat = true;
    var repur = $(".repur-amt").val();
    var per = $(".ded-per-cal").val();
    if (stat == null || stat == "") {
        $("#stat").focus();
        return false;
    }
    //if (confirm("Are you sure you want to Process")) {
    Swal.fire({
        text: 'Are you sure you want to process this file for cancellation?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/FileSystem/UpdateCancelStat/",
                data: { Id: id, FileId: pltid, FileNumber: filnum, Remarks: remark, Status: stat, Deduction: per, Repurchase: repur },
                success: function (data) {
                    //alert("Updated and Requested to Finance")
                    Swal.fire({
                        icon: 'success',
                        text: 'Cancellation request updated and forwarded to the finance department for processing'
                    }).then(() => {
                        window.open("/Receipts/Cancellation_Receipts?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                        window.location.reload();
                    });
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#can-plt", function () {
    var id = $("#can-req-id").val();
    var pltid = $("#plt-id").val();
    var remark = $("#Remarks").val();
    var stat = $("#stat").val();
    var repur = $(".repur-amt").val();
    var per = $(".ded-per-cal").val();
    if (stat == null || stat == "") {
        $("#stat").focus();
        return false;
    }
    if (confirm("Are you sure you want to cancel this Plot")) {
        $.ajax({
            type: "POST",
            url: "/Plots/UpdateCancelStat/",
            data: { Id: id, PlotId: pltid, Remarks: remark, Status: stat, Deduction: per, Repurchase: repur },
            success: function (data) {
                //alert("Updated and Requested to Finance")
                Swal.fire({
                    icon: 'success',
                    text: 'Cancellation request updated and forwarded to the finance department for processing'
                }).then(() => {
                    window.open("/Receipts/Cancellation_Receipts?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                    //window.open("/Transfer/RepurchaseTransfer?Id=" + pltid, '_blank');
                });
            },
            error: function (data) {
            }
        });
    }
});
//
$(document).on("click", "#can-com", function () {
    var id = $("#can-req-id").val();
    var comid = $("#shp-id").val();
    var remark = $("#Remarks").val();
    var stat = $("#stat").val();
    var repur = $(".repur-amt").val();
    var per = $(".ded-per-cal").val();
    if (stat == null || stat == "") {
        $("#stat").focus();
        return false;
    }
    //if (confirm("Are you sure you want to cancel this Commercial")) {
    Swal.fire({
        text: 'Are you sure you want to cancel this commercial unit?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Commercial/UpdateCancelStat/",
                data: { Id: id, ComId: comid, Remarks: remark, Status: stat, Deduction: per, Repurchase: repur },
                success: function (data) {
                    //alert("Updated and Requested to Finance")
                    Swal.fire({
                        icon: 'success',
                        text: 'Cancellation request updated and forwarded to the finance department for processing'
                    }).then(() => {
                        window.open("/Receipts/Cancellation_Receipts_Commercial?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                    })
                    //window.open("/Transfer/RepurchaseTransfer?Id=" + pltid, '_blank');
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// View for finance Plot Cancelation
$(document).on("click", ".show-plt-can-f", function () {
    EmptyModel();
    debugger
    var canid = $("#can-req-id").attr("id");
    var pltid = $("#plt-id").attr("id");
    $('#ModalLabel').text("Cancel Plot");
    $('#modalbody').load('/Plots/PlotCancelation/', { Id: canid, PlotId: pltid });
    $('.modal-footer').append('<button class="btn btn-success" id="can-plt-f" type="button">Cancel Plot</button>');
});
// View for finance Plot Cancelation
$(document).on("click", ".show-com-can-f", function () {
    EmptyModel();
    var canid = $("#can-req-id").attr("id");
    var pltid = $("#plt-id").attr("id");
    $('#ModalLabel').text("Cancel Property");
    $('#modalbody').load('/Commercial/ComCancelation/');
    $('.modal-footer').append('<button class="btn btn-success" id="can-com-f" type="button">Cancel Unit</button>');
});
// View For Finance File Cancelation
$(document).on("click", ".show-file-can-f", function () {
    EmptyModel();
    var canid = $("#can-req-id").attr("id");
    var pltid = $("#file-id").attr("id");
    $('#ModalLabel').text("Cancel Plot");
    $('#modalbody').load('/FileSystem/FileCancelation/', { Id: canid, FileId: pltid });
    $('.modal-footer').append('<button class="btn btn-success" id="can-file-f" type="button">Cancel Plot</button>');
});
//
$(document).on("blur", ".rev-qty", function () {
    let val = $(this).val();
    let type = $(this).data('type');
    let id = $(this).closest('tr').attr('id');
    let po = $('.PO_Number').val();
    $.ajax({
        type: "POST",
        url: "/Procurement/UpdatePO_Qty_Rate/",
        data: { id: id, val: val, Type: type, PO: po },
        success: function () {
        }
    });
});
//
$(document).on("click", ".sen-bk-to-dep", function () {
    let grp = $(this).data('grpid');
    let ponum = $(this).data('ponum');
    if (confirm("Are you sure want to send back the finalized requisition")) {
        $.ajax({
            type: "POST",
            url: "/Procurement/SendBackToDepartment/",
            data: { Group_Id: grp, PoNumber: ponum },
            success: function () {
                $('#' + ponum).remove();
            }
        });
    }
});
//
$(document).on("click", ".sen-bk-to-dep-wo", function () {
    let grp = $(this).data('grpid');
    let ponum = $(this).data('ponum');
    if (confirm("Are you sure want to send back the finalized requisition")) {
        $.ajax({
            type: "POST",
            url: "/Services/SendBackToDepartment/",
            data: { Group_Id: grp, PoNumber: ponum },
            success: function () {
                $('#' + ponum).remove();
            }
        });
    }
});
//
$(document).on("blur", ".rev-ser-qty", function () {
    let val = $(this).val();
    let type = $(this).data('type');
    let id = $(this).closest('tr').attr('id');
    let po = $('.PO_Number').val();
    $.ajax({
        type: "POST",
        url: "/Services/UpdatePO_Qty_Rate/",
        data: { id: id, val: val, Type: type, PO: po },
        success: function () {
        }
    });
});
// View For Finance File Cancelation
$(document).on("click", ".frd-plt-ref-f", function () {
    EmptyModel();
    var canid = $("#can-req-id").val();
    var pltid = $("#plt-id").val();
    $('#ModalLabel').text("Refund Amount");
    $('#modalbody').load('/Plots/RefundAmount/', { Id: canid });
    $('.modal-footer').append('<button class="btn btn-success" id="ref-plot-f-btn" type="button">Refund Amount</button>');
});
// final Cancelation of Plot
$(document).on("click", "#can-plt-f", function (e) {
    e.preventDefault();
    $('#can-plt-f').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var im = $("#scanned").attr('src');
    $("#imge").val(im);
    //var con = confirm("Are you sure you want to Cancell this Plot");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to cancel this plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#plt-fin-can").attr('action'),
                data: $("#plt-fin-can").serialize(),
                success: function (data) {
                    if (data.Status) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        }).then(() => {
                            window.open("/Vouchers/Voucher?GroupId=" + data.Token, '_blank');
                        })
                    }
                    else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'error',
                            text: data.Msg
                        });
                        $('#can-plt-f').removeAttr("disabled");
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-rec').attr("disabled", false);
                }
            });
        }
    });
});
// final Cancelation of Plot
$(document).on("click", "#can-com-f", function (e) {
    e.preventDefault();
    $('#can-com-f').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var im = $("#scanned").attr('src');
    $("#imge").val(im);
    //var con = confirm("Are you sure you want to Cancell this Property");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to cancel this Property?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#com-fin-can").attr('action'),
                data: $("#com-fin-can").serialize(),
                success: function (data) {
                    if (data.Status == true) {
                        window.open("/Vouchers/SAGVouchers?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
                    }
                    else {
                        //alert(data.Msg)
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-rec').attr("disabled", false);
                }
            });
        }
    });
});
//
$(document).on("click", ".rels-pay", function (e) {
    var id = $(this).closest('tr').attr('id');
    var con = confirm("Are you sure you want to Release the payment");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Finance/ReleasePayment/',
            data: { Id: id },
            success: function (data) {
                window.open("/Vouchers/SAGVouchers?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
            },
            error: function () {
                alert("Error Occured");
                $('#gen-rec').attr("disabled", false);
            }
        });
    }
});
// final Cancelation of Plot
$(document).on("click", "#ref-plot-f-btn", function (e) {
    e.preventDefault();
    $('#can-plt-f').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var im = $("#scanned").attr('src');
    $("#imge").val(im);
    //var con = confirm("Are you sure you want to Refund this Amount");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to refund the amount?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#plt-fin-ref").attr('action'),
                data: $("#plt-fin-ref").serialize(),
                success: function (data) {
                    window.open("/Vouchers/Voucher?GroupId=" + data.Token, '_blank');
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-rec').attr("disabled", false);
                }
            });
        }
    });
});
// Final Cancelatio of file
$(document).on("click", "#can-file-f", function (e) {
    e.preventDefault();
    $('#can-file-f').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var im = $("#scanned").attr('src');
    $("#imge").val(im);
    var con = confirm("Are you sure you want to Cancell this Plot");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#plt-fin-can").attr('action'),
            data: $("#plt-fin-can").serialize(),
            success: function (data) {
                window.open("/Vouchers/Voucher?GroupId=" + data.Token, '_blank');
            },
            error: function () {
                alert("Error Occured");
                $('#gen-rec').attr("disabled", false);
            }
        });
    }
});
//
$(document).on("click", "#ser-out-st", function (e) {
    e.preventDefault();
    var id = $("#Blocks").val();
    SASLoad('#rpt-data');
    $.ajax({
        type: "POST",
        url: "/Reports/PlotsOutStandingReport/",
        data: { Id: id },
        success: function (data) {
            SASUnLoad('#rpt-data');
            $('#rpt-data').empty();
            var html1 = `<table class="table table-striped table-bordered rpttable" >
                            <thead class="thead-dark">
                                <tr>
                                    <th width="80px">Plot No</th>
                                    <th width="100px">Type</th>
                                    <th width="100px">Status</th>
                                    <th width="150px">Total Amount</th>
                                    <th width="150px">Received</th>
                                    <th width="150px">Discount</th>
                                    <th width="150px">Remaining</th>
                            </thead>
                        <tbody style="max-height: 300px !important;"></tbody>
                        <tfoot>
                        </tfoot>
                        </table>`;
            $('#tab-data').html(html1);
            var srcount = 1;
            $(data.Result).each(function (i) {
                var html = `<tr class="pointer plt-details" data-id="${data.Result[i].Id}">
                                <td width="80px">${data.Result[i].Plot_No}</td>
                                <td width="80px">${data.Result[i].Type}</td>
                                <td width="80px">${data.Result[i].Status}</td>
                                <td width="150px">${Number(parseFloat(data.Result[i].total).toFixed(0)).toLocaleString('en')}</td>
                                <td width="150px">${Number(parseFloat(data.Result[i].Received).toFixed(0)).toLocaleString('en')}</td>
                                <td width="150px">${Number(parseFloat(data.Result[i].Discount).toFixed(0)).toLocaleString('en')}</td>
                                <td width="150px">${Number(parseFloat(data.Result[i].Remaining).toFixed(0)).toLocaleString('en')}</td>
                            </tr>`;
                $('#tab-data tbody').append(html);
                srcount++;
            });
            var html = `<h5>Total Amount:  ${Number(parseFloat(data.TotalAmt).toFixed(0)).toLocaleString()}</h5>
                        <h5>Received Amount:  ${Number(parseFloat(data.ReceivedAmt).toFixed(0)).toLocaleString()}</h5>
                        <h5>Discount Amount:  ${Number(parseFloat(data.Discount).toFixed(0)).toLocaleString()}</h5>
                        <h5>Excess Amount:  ${Number(parseFloat(data.Access).toFixed(0)).toLocaleString()}</h5>
                        <h5>Remaining Amount:  ${Number(parseFloat(data.Remaining).toFixed(0)).toLocaleString()}</h5>`
            $('#rpt-data').append(html);
            $('.rpttable').DataTable({
                dom: 'Bfrtip'
            });
        },
        error: function (data) {
            SASUnLoad('#rpt-data');
        }
    });
});
//
$(document).on("change", "#full-paid", function (e) {
    var token = $("#full-paid").is(":checked");
    if (token) {
        var total_price = parseInt($('#total').val());
        $('#adv-pmt').text(Number(total_price).toLocaleString());
    }
    else {
        var advan = parseInt($('#adv').val());
        $('#adv-pmt').text(Number(advan).toLocaleString());
    }
});
// 
$(document).on("click", ".can-rec", function () {
    EmptyModel();
    var a = $(this).attr("id");
    $('#ModalLabel').text("Cancel Receipt");
    $('#modalbody').load('/Banking/ChequeDetailtoCancel/', { Id: a });
});

// dishonored cheque recovery
$(document).on("click", ".dis-chqs", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var data = id.split('`');
    $('.modal-body').load('/Banking/DisHoncheqesDetails/', { No: data[0], Date: data[1], Bank: data[2], Status: data[3] }, function () { });
});
// leads dishonordd cheque recovery
$(document).on("click", ".ld-dis-chqs", function () {
    EmptyModel();
    var id = $(this).attr("id");
    var data = id.split('`');
    $('.modal-body').load('/Banking/LeadsDisHoncheqesDetails/', { No: data[0], Date: data[1], Bank: data[2], Status: data[3] }, function () { });
});
// Search the File information
$(document).on("click", "#gen-can", function (e) {
    e.preventDefault();
    var allrecepts = [], id = [], receid = [];
    var fineamt = [];
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.File_Plot_Number = $("#pay-" + i + " #file-plt").val();
        recedata.FileImage = $("#pay-" + i + " #scanned").attr('src');
        allrecepts.push(recedata);
    }
    var finedata = {
        Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
        Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
    };
    finedata.Amount = $("#fine-1 #Amount-fine").val();
    finedata.AmountInWords = InWords($("#fine-1 #Amount-fine").val());
    finedata.Bank = $("#fine-1 #bank-fine").val();
    finedata.PayChqNo = $("#fine-1 #paymenttype-fine").val();
    finedata.PaymentType = $("#fine-1 #cah-chq-bak-fine").val();
    finedata.Project_Name = $("#prj").val();
    finedata.Branch = $("#fine-1 #Branch-fine").val();
    finedata.Ch_bk_Pay_Date = $("#fine-1 #cbp-date-fine").val();
    finedata.File_Plot_Number = $("#fine-1 #file-plt").val();
    fineamt.push(finedata);
    //if (!($("#switch-fine-r").is(":checked"))) {
    //    var finedata = {
    //        Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
    //        Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
    //    };
    //    finedata.Amount = $("#fine-1" + " #Amount-fine").val();
    //    finedata.AmountInWords = InWords($("#fine-1" + " #Amount-fine").val());
    //    finedata.Bank = $("#fine-1" + " #bank-fine").val();
    //    finedata.PayChqNo = $("#fine-1" + " #paymenttype-fine").val();
    //    finedata.PaymentType = $("#fine-1" + " #cah-chq-bak-fine").val();
    //    finedata.Project_Name = $("#prj").val();
    //    finedata.Branch = $("#fine-1" + " #Branch-fine").val();
    //    finedata.Ch_bk_Pay_Date = $("#fine-1" + " #cbp-date-fine").val();
    //    finedata.File_Plot_Number = $("#fine-1" + " #file-plt").val();
    //    fineamt.push(finedata);
    //}
    //var isFine = $("#switch-fine-r").is(":checked");
    $(".ids").each(function () {
        id.push($(this).val());
    });
    $(".Rec-ids").each(function () {
        receid.push($(this).val());
    });
    var can_file = $("#can-file-r").is(":checked");
    var Trans = $("#trans-id").val();
    var alldata = {
        Receiptdata: allrecepts,
        FineData: fineamt,
        //isFine,
        Id: id,
        RecpIds: receid,
        CancelFile: can_file,
        TransactionId: Trans
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $('#gen-cancel').attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                $(data.Receiptid).each(function (i) {
                    if (data.Receiptid[i].Type == "Booking") {
                        if (data.Receiptid[i].Module == "PlotManagement") {
                            window.open("/Receipts/PlotCanReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                        else if (data.Receiptid[i].Module == "CommercialManagement") {
                            window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                        else {
                            window.open("/Banking/Receipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                    }
                    else if (data.Receiptid[i].Type == "Installment") {
                        if (data.Receiptid[i].Module == "PlotManagement") {
                            window.open("/Receipts/PlotCanReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                        else if (data.Receiptid[i].Module == "CommercialManagement") {
                            window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                        else {
                            window.open("/Banking/InstallmentReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                        }
                    }
                    else if (data.Receiptid[i].Type == "Cancellation") {
                        window.open("/Receipts/CancellationReceipt?Id=" + data.Receiptid[i].ReceiptNo + "&Token=" + data.Token, '_blank');
                    }
                });
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
// Search the File information
$(document).on("click", "#ld-gen-can", function (e) {
    e.preventDefault();
    var allrecepts = [], id = [], receid = [];
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.File_Plot_Number = $("#pay-" + i + " #file-plt").val();
        recedata.FileImage = $("#pay-" + i + " #scanned").attr('src');
        allrecepts.push(recedata);
    }
    $(".ids").each(function () {
        id.push($(this).val());
    });
    $(".Rec-ids").each(function () {
        receid.push($(this).val());
    });
    var can_file = $("#can-file").is(":checked");
    var Trans = $("#trans-id").val();
    var alldata = {
        Receiptdata: allrecepts,
        Id: id,
        RecpIds: receid,
        CancelFile: can_file,
        TransactionId: Trans
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $('#gen-cancel').attr("action"),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                $(data.Receiptid).each(function (i) {
                    if (data.Company == "SAM") {
                        window.open("/Receipts/SAM_Receipts?Id=" + data.Receiptid[i].ReceiptId + "&Token=" + data.Token, '_blank');
                    }
                    else {
                        window.open("/Receipts/SPE_Receipts?Id=" + data.Receiptid[i].ReceiptId + "&Token=" + data.Token, '_blank');
                    }
                });
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".cr-lead", function () {
    EmptyModel();
    $('.modal-body').load('/Leads/CreateLeads/', function () { });
});
//
$(document).on("click", ".dr-pay", function () {
    EmptyModel();
    var id = $("#deal-id").val();
    $('.modal-body').load('/PropertyDeal/DirectDeal/', { Id: id }, function () { });
});
//
$(document).on("click", ".cr-deal", function () {
    EmptyModel();
    $('.modal-body').load('/PropertyDeal/CreateDeal/', function () { });
});
//
$(document).on("click", ".ad-de-buy", function () {
    EmptyModel();
    $('.modal-body').load('/PropertyDeal/AddBuyer/', function () { });
});
//
$(document).on("click", ".up-par-info", function () {
    EmptyModel();
    var id = $(this).closest('tr').attr('id');
    $('.modal-body').load('/PropertyDeal/UpdatePartyInfo/', { Id: id }, function () { });
});
//
$(document).on("click", ".gen-pr-req", function () {
    EmptyModel();
    var req = $(this).data('req');
    $('.modal-body').load('/PropertyDeal/PaymentRequestDetails/', { Id: req }, function () { });
});
// 
$(document).on("click", "#gen-dire-pay", function (e) {
    e.preventDefault();
    var remamt = Number($("#rec-amt").val());
    var amt = Number($("#Amount").val());
    var dealamt = Number($("#del-ttlamt").val());
    var receiv_amt = Number($("#Total_Amt").val());
    if (dealamt < amt) {
        //alert("You Cant Generate Direct payment More than Deal Amount");
        Swal.fire({
            icon: 'info',
            text: 'A direct payment exceeding the deal amount cannot be generated'
        });
        return false;
    }
    else {
        if (amt > remamt) {
            //alert("You Cant Generate Direct payment More than Remaining Amount");
            Swal.fire({
                icon: 'info',
                text: 'A direct payment exceeding the remaining amount cannot be generated'
            });
            return false;
        }
    }
    //var con = confirm("Are you sure you want to Genreate");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the direct payment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#prop-del-di-req").attr("action"),
                data: $("#prop-del-di-req").serialize(),
                success: function (data) {
                    //alert("Direct Payment has done")
                    Swal.fire({
                        icon: 'success',
                        text: 'Direct payment generated successfully'
                    }).then(() => {
                        window.location.reload();
                    })
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
// Register 
$(document).on("click", ".ad-oth-char-btn", function (e) {
    e.preventDefault();
    var con = confirm("Are you sure you want to Add These Charges");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#ad-oth-ch").attr("action"),
            data: $("#ad-oth-ch").serialize(),
            success: function (data) {
                window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
// Register  a file
$(document).on("click", "#gen-del-req", function (e) {
    e.preventDefault();
    $(this).attr("disabled", true);
    //var con = confirm("Are you sure you want to Genreate");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt/Voucher?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#prop-del-req").attr("action"),
                data: $("#prop-del-req").serialize(),
                success: function (data) {
                    if (data.Status) {
                        Swal.fire({
                            icon: 'success',
                            text: data.Type + ' generated successfully'
                        }).then(() => {
                            if (data.Type == 'Voucher') {
                                window.open("/Vouchers/PropertyDeal_Voucher?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
                            }
                            else {
                                window.open("/Receipts/PropertyDeal_Receipts?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                            }
                            window.location.reload();
                        })
                    }
                    else {
                        //alert("Error");
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                        $('#gen-del-req').attr("disabled", false);
                    }
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                        $('#gen-del-req').attr("disabled", false);
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                        $('#gen-del-req').attr("disabled", false);
                    }
                }
            });
        }
    });
});
//
$(document).on("click", ".prop-pay-vou-req", function () {
    var html = '<div class="form-row"><div class="form-group col-md-4"><label class="">Amount</label>  <input class="form-control coma" placeholder="250,000" required><input type="hidden" id="Amount" class="amt" required></div>' +
        '<div class="form-group col-md-8"><label class="">Description</label>  <input id="desc" class="form-control" name="Description" placeholder="Description Written on Payment Voucher" required></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="req-vouch" type="submit">Request for Payment Voucher</button>');
});
//
$(document).on("change", ".com-cal", function () {
    var id = $(this).val();
    if (id === "Other") {
        $("#oth-com").show()
        $(".oth-com").show()
    }
    else {
        $("#oth-com").hide()
        $(".oth-com").hide()
        var off = $("#off-pric").val();
        var val = (off * id) / 100;
        $("#f-com-s").text(Number(val).toLocaleString());
        $("#com-rate").val(val);
    }
});
//
$(document).on("keyup", "#oth-com", function () {
    var val = RemoveComma($(this).val());
    $("#f-com-s").text(Number(val).toLocaleString());
    $("#com-rate").val(val);
});
//
$(document).on("click", ".v-r-c", function () {
    var id = $(this).attr("id");
    $("#v-r-t").val(id);
    $("#type").text(id);
    if (id == "Voucher") {
        $("#amt-t-p").prop("readonly", false);
        $(".des").show();
    }
    else if (id == "Commession") {
        var id = $("#com-rate").val();
        $("#amt-t-p").val(Number(id).toLocaleString());
        $("#amt-t-p").prop("readonly", true);
        $("#Amount").val(id);
        $(".des").hide();
    }
    else {
        $("#amt-t-p").prop("readonly", false);
        $(".des").hide();
    }
    $("#pay-voc, #v-r-c-btn").show();
    $("#up-inf-par").hide();
});
//
$(document).on("click", "#v-r-c-btn", function (e) {

    e.preventDefault();
    debugger
    var offeramt = parseInt($('#off-pric').val());
    var remain = parseInt($('#rec-amt').val());
    var Rece_amt = parseInt($('#reciv-amt').val());
    var de_typ = $('#de-typ').text();
    var type = $("#v-r-t").val();
    var amount = parseInt($("#Amount").val());
    var desc = $("#desc").val();
    var del_id = $("#deal-id").val();
    var part_id = $("#Id").val();
    if (de_typ == "Resale" || de_typ == "New_Lead") {
        if (type == "Voucher") {
            if (amount <= Rece_amt) {
                if (amount > remain) {
                    //alert("You Cant Request Voucher More than received Amount");
                    Swal.fire({
                        icon: 'info',
                        text: 'Voucher of amount more than received amount cannot be requested'
                    });
                    return false;
                }
            }
            else {
                //alert("You Cant Request Voucher More than received Amount");
                Swal.fire({
                    icon: 'info',
                    text: 'Voucher of amount more than received amount cannot be requested'
                });
                return false;
            }
        }
        else if (type == "Receipt") {
            var rem = offeramt - Rece_amt;
            if (amount > rem) {
                //alert("You Cant Request Receipt More than Deal Amount");
                Swal.fire({
                    icon: 'info',
                    text: 'Receipt of amount more than deal amount cannot be requested'
                });
                return false;
            }
        }
    }
    //if (confirm("Are you sure you want to request " + type)) {
    Swal.fire({
        text: 'Are you sure you want to request ' + type +' ?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/PropertyDeal/PaymentRequest/',
                data: { Type: type, Amount: amount, Description: desc, Deal_Id: del_id, Party_Id: part_id },
                success: function (data) {
                    if (data.Status) {
                        //alert(data.Msg)
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        });
                    }
                    else {
                        //alert("Deal is Closed you can not request a " + type + " Now")
                        Swal.fire({
                            icon: 'error',
                            text: 'Deal is closed, ' + type + ' cannot be requested'
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#lead-from-to", function () {
    var to = $('#lead-to').val();
    var from = $('#lead-from').val();
    $("#cheques").load('/Leads/Walkin/', { Status: 'Initial_Discussion', From: from, To: to });
});
//
$(document).on("click", "#ser-SPE-lead", function () {
    var Lead_User = $('.lead_user').val();
    var SPE_lead_Status = $("#SPE_Lead_Status option:selected").val();
    var to = $('#SPE-lead-to').val();
    var from = $('#SPE-lead-from').val();
    $("#SPElead").load('/Leads/SPESearch/', { From: from, To: to, LeadStatus: SPE_lead_Status, LeadUser: Lead_User });
});
//
$(document).on("click", "#ser-SAM-lead", function () {
    var Lead_User = $('.lead_user').val();
    var SPE_lead_Status = $("#SAM_Lead_Status option:selected").val();
    var to = $('#SAM-lead-to').val();
    var from = $('#SAM-lead-from').val();
    var phone = $('#leadphone').val();
    $("#SAMLeads").load('/Leads/SAMSearch/', { From: from, To: to, LeadStatus: SPE_lead_Status, LeadUser: Lead_User, Phone: phone });
});
//
$(document).on("click", "#ser-my-lead", function () {
    var Lead_User = $('.lead_user').val();
    var SPE_lead_Status = $("#SAM_Lead_Status option:selected").val();
    var to = $('#SAM-lead-to').val();
    var from = $('#SAM-lead-from').val();
    var phone = $('#leadphone').val();
    $("#SAMLeads").load('/Leads/MyLeadSearch/', { From: from, To: to, LeadStatus: SPE_lead_Status, LeadUser: Lead_User, Phone: phone });
});
$('.all-leads-up-btn').unbind().click(function () {
    var pr = "Grand City";
    EmptyModel();
    $('#ModalLabel').text("Upload Leads");
    $('#modalbody').load('/Leads/UploadLeads', { Project: pr, status: "Assigned" });
});
$(document).on('click', '.all-leads-sa-up-btn', function () {
    var pr = "SA Premium Homes";
    EmptyModel();
    $('#ModalLabel').text("Upload Leads");
    $('#modalbody').load('/Leads/UploadLeads', { Project: pr, status: "Assigned" });
});
//
$(document).on("keyup", "#of-pr", function () {
    var off = RemoveComma($(this).val())
    var size = $("#plt-size").text().split(" ")[0];
    var ratepermarla = off / size;
    $("#rateper").val(Number(parseFloat(ratepermarla).toFixed(0)).toLocaleString());
    $("#rate").val(parseFloat(ratepermarla).toFixed(0));
});
//
$(document).on("keyup", "#off-pr", function () {
    var f_p = $("#fil-plt").val();
    var type = $("#type").val();
    var off = RemoveComma($(this).val())
    if (f_p === "Plot" && (type == "Resale" || type == "Investment")) {
        var size = $("#plt-size-n").val().split(" ")[0];
        var ratepermarla = off / size;
        $("#rateper").val(Number(parseFloat(ratepermarla).toFixed(0)).toLocaleString());
        $("#rate").val(parseFloat(ratepermarla).toFixed(0));
    }
    else if (f_p === "File" && (type == "Resale" || type == "Investment")) {
        var size = $("#fil-plt-size").val().split(" ")[0];
        var ratepermarla = off / size;
        $("#rateper").val(Number(parseFloat(ratepermarla).toFixed(0)).toLocaleString());
        $("#rate").val(parseFloat(ratepermarla).toFixed(0));
    }
});
//
$(document).on("change", ".r-n-f-p", function (e) {
    var project = $('#l-proj').val();
    var f_p = $("#fil-plt").val();
    var type = $("#type").val();
    Resale_NewLead_Control(f_p, type, project);
});
//
function Resale_NewLead_Control(f_p, type, project) {
    if (project == 'SA Premium Homes') {
        $('.fil_plt').hide();
        $('.unitList').show();
        if (type == "New_Lead") {
            $.ajax({
                type: "POST",
                url: "/Leads/GetProjectWiseInventory/",
                data: { Project: project, Type: type },
                success: function (data) {
                    $('#units').html(' ');
                    $('#units').append('<option>Select Unit</option>');
                    $.each(data, function (key, value) {
                        $("#units").append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
                    });
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        alert("got timeout");
                    } else {
                        alert(textstatus);
                    }
                }
            });
        }
        else if (type == "Resale" || type == "Investment") {
            $('.fil_plt').hide();
            $('.unitList').show();
            $.ajax({
                type: "POST",
                url: "/Leads/GetProjectWiseInventory/",
                data: { Project: project, Type: type },
                success: function (data) {
                    $('#units').html(' ');
                    $('#units').append('<option>Select Unit</option>');
                    $.each(data, function (key, value) {
                        $("#units").append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
                    });
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        alert("got timeout");
                    } else {
                        alert(textstatus);
                    }
                }
            });
        }
    }
    else {
        $('.fil_plt').show();
        $('.unitList').hide();
        if (f_p === "Plot" && type == "New_Lead") {
            $(".n-p").show();
            $(".plt-info").show();
            $(".n-p").find('input, textarea, button, select').removeAttr("disabled", false);
            $(".fil").hide();
            $(".fil").find('input, textarea, button, select').attr("disabled", true);
            $(".r-plt").hide();
            $(".r-plt").find('input, textarea, button, select').attr("disabled", true);
            // Buyer and seller Check
            $("#buy-sel").text("Buyer Information");
            $("#bu-se").val("Buyer");
            $(".demand").hide();
        }
        else if (f_p === "Plot" && (type == "Resale" || type == "Investment")) {
            $(".n-p").hide();
            $(".plt-info").show();
            $(".n-p").find('input, textarea, button, select').attr("disabled", true);
            $(".fil").hide();
            $(".fil").find('input, textarea, button, select').attr("disabled", true);
            $(".r-plt").show();
            $(".r-plt").find('input, textarea, button, select').removeAttr("disabled", false);
            $("#buy-sel").text("Seller Information");
            $("#bu-se").val("Seller");
            $(".demand").show();
        }
        if (f_p === "File" && type == "New_Lead") {
            $(".n-p").hide();
            $(".plt-info").hide();
            $(".n-p").find('input, textarea, button, select').attr("disabled", true);
            $(".fil").show();
            $(".fil").find('input, textarea, button, select').removeAttr("disabled", false);
            $(".r-plt").hide();
            $(".r-plt").find('input, textarea, button, select').attr("disabled", true);
            $("#buy-sel").text("Buyer Information");
            $("#bu-se").val("Buyer");
            $(".demand").hide();
        }
        else if (f_p === "File" && (type == "Resale" || type == "Investment")) {
            $(".n-p").hide();
            $(".plt-info").hide();
            $(".n-p").find('input, textarea, button, select').attr("disabled", true);
            $(".fil").show();
            $(".fil").find('input, textarea, button, select').removeAttr("disabled", false);
            $(".r-plt").hide();
            $(".r-plt").find('input, textarea, button, select').attr("disabled", true);
            $("#buy-sel").text("Seller Information");
            $("#bu-se").val("Seller");
            $(".demand").show();
        }
    }
}
//
$(document).on("click", ".rem-req", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    if (confirm("Are you sure you want to Remove this request")) {
        $.ajax({
            type: "POST",
            url: '/PropertyDeal/RemoveRequest/',
            data: { Id: id },
            success: function (data) {
                window.location.reload();
            },
            error: function (data) {
            }
        });
    }
});
//
$(document).on("click", ".com-appr", function (e) {
    e.preventDefault();
    var id = $(this).closest('tr').attr("id");
    //if (confirm("Are you sure you want to Approve this Commission request")) {
    Swal.fire({
        text: 'Are you sure you want to approve this commission request?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/PropertyDeal/UpdateApproval/',
                data: { Id: id },
                success: function (data) {
                    //alert("Approved")
                    Swal.fire({
                        icon: 'success',
                        text: 'Request approved successfully'
                    }).then(() => {
                        window.location.reload();
                    })
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
$(document).on("click", "#cra-lead", function (e) {
    e.preventDefault();
    var name = $("#Name").val();
    var f_name = $("#Father_Husband").val();
    var mob = $("#Mobile_1").val();
    var prj = $("#Project").val();
    var blk = $("#Block").val();
    var siz = $("#plt-siz").val();
    var pric = RemoveComma($("#off-pric").val());
    var stat = $('#LeadStatus option:selected').val();
    if (name == "" || name == null) {
        //alert("Please Enter Name")
        Swal.fire({
            icon: 'info',
            text: 'Enter name to proceed'
        });
        return false;
    }
    else if (f_name == "" || f_name == null) {
        //alert("Please Father Name")
        Swal.fire({
            icon: 'info',
            text: 'Enter father name to proceed'
        });
        return false;
    }
    else if (mob == "" || mob == null) {
        //alert("Please Mobile Number")
        Swal.fire({
            icon: 'info',
            text: 'Enter mobile number to proceed'
        });
        return false;
    }
    else if (prj == "" || prj == null) {
        //alert("Please Select Project");
        Swal.fire({
            icon: 'info',
            text: 'Select a project to proceed'
        });
        return false;
    }
    else if (pric == "" || pric == null || pric == 0) {
        //alert("Please Enter Offered Price")
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid offered price to proceed'
        });
        return false;
    }
    else if (stat == "" || stat == null) {
        //alert("Please Select Lead Status");
        Swal.fire({
            icon: 'info',
            text: 'Select lead status to proceed'
        });
        return false;
    }
    $("#off-pric").val(RemoveComma($("#off-pric").val()));
    //if (confirm("Are you sure you want to create Lead")) {
    Swal.fire({
        text: 'Are you sure you want to create the lead?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#c-led').attr("action"),
                data: $('#c-led').serialize(),
                success: function (data) {
                    if (data.Status) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        }).then(() => {
                            closeModal();
                            var $this = $('.active'),
                                targ = $this.attr('href'),
                                loadurl = $this.attr('data-link');
                            var Lead_User = $('.lead_user').val();
                            var SPE_lead_Status = $("#SAM_Lead_Status option:selected").val();
                            var to = $('#SAM-lead-to').val();
                            var phone = $('#leadphone').val();
                            var from = $('#SAM-lead-from').val();
                            SASLoad(targ);
                            $(targ).load(loadurl, { From: from, To: to, LeadStatus: SPE_lead_Status, LeadUser: Lead_User, Phone: phone }, function () {
                                SASUnLoad(targ);
                                $('#myTabs .active').removeClass("active");
                                $this.addClass("active");
                            });
                        })
                        
                    } else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#cra-un-lead", function (e) {
    e.preventDefault();
    var name = $("#Name").val();
    var f_name = $("#Father_Husband").val();
    var mob = $("#Mobile_1").val();
    var prj = $("#Project").val();
    if (name == "" || name == null) {
        alert("Please Enter Name")
        return false;
    }
    else if (f_name == "" || f_name == null) {
        alert("Please Father Name")
        return false;
    }
    else if (mob == "" || mob == null) {
        alert("Please Mobile Number")
        return false;
    }
    else if (prj == "" || prj == null) {
        alert("Please Select Project");
        return false;
    }
    $("#off-pric").val(RemoveComma($("#off-pric").val()));
    if (confirm("Are you sure you want to create Lead")) {
        $.ajax({
            type: "POST",
            url: $('#c-led').attr("action"),
            data: $('#c-led').serialize(),
            success: function (data) {
                closeModal();
                var $this = $('.active'),
                    targ = $this.attr('href'),
                    loadurl = $this.attr('data-link');
                var Lead_User = $('.lead_user').val();
                var SPE_lead_Status = $("#SAM_Lead_Status option:selected").val();
                var to = $('#SAM-lead-to').val();
                var phone = $('#leadphone').val();
                var from = $('#SAM-lead-from').val();
                SASLoad(targ);
                $(targ).load(loadurl, { From: from, To: to, LeadStatus: SPE_lead_Status, LeadUser: Lead_User, Phone: phone }, function () {
                    SASUnLoad(targ);
                    $('#myTabs .active').removeClass("active");
                    $this.addClass("active");
                });
            },
            error: function (data) {
            }
        });
    }
});
//
$(document).on("click", "#up-inf-par", function (e) {
    e.preventDefault();
    Swal.fire({
        text: 'Are you sure you want to update the information?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#up-info-part').attr("action"),
                data: $('#up-info-part').serialize(),
                success: function (data) {
                    if (data.Status) {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'success',
                            text: data.Msg
                        }).then(() => {
                            window.location.reload();
                        });
                    }
                    else {
                        //alert(data.Msg);
                        Swal.fire({
                            icon: 'info',
                            text: data.Msg
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#cra-buy", function (e) {
    e.preventDefault();
    var email = /^0\d{10}/;
    var val = $("#Mobile").val();
    var res = email.test(val)
    if (!res) {
        //alert('Please Enter Proper Mobile Number');
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid mobile number to proceed'
        });
        return false;
    }
    $.ajax({
        type: "POST",
        url: $('#c-buy').attr("action"),
        data: $('#c-buy').serialize(),
        success: function (data) {
            Swal.fire({
                icon: 'success',
                text: 'Buyer added successfully'
            }).then(() => {
                window.location.reload();
            })
        },
        error: function (data) {
            Swal.fire({
                icon: 'error',
                text: 'Something went wrong'
            });
        }
    });
});
//
$(document).on("click", "#cra-Deal", function (e) {
    e.preventDefault();
    var email = /^0\d{10}/;
    var val = $("#Mobile").val();
    var res = email.test(val)
    if (!res) {
        //alert('Please Enter Proper Mobile Number');
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid mobile number to proceed'
        });
        return false;
    }
    Swal.fire({
        text: 'Are you sure you want to create the deal?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            //if (confirm("Are you sure you want to Create Deal")) {
            $.ajax({
                type: "POST",
                url: $('#c-deal').attr("action"),
                data: $('#c-deal').serialize(),
                success: function (data) {
                    if (data) {
                        //alert("Deal Created");
                        Swal.fire({
                            icon: 'success',
                            text: 'Deal created successfully'
                        }).then(() => {
                            window.location.reload();
                        })
                    }
                    else {
                        //alert("Previous Deal is still not Closed")
                        Swal.fire({
                            icon: 'info',
                            text: 'The previous deal has not yet been closed'
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", "#cra-l-Deal", function (e) {
    e.preventDefault();
    var email = /^0\d{10}/;
    var val = $("#Mobile").val();
    var res = email.test(val)
    if (!res) {
        //alert('Please Enter Proper Mobile Number');
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid mobile number to proceed'
        });
        return false;
    }
    var demand = $("#dem-pric").val();
    var offeredprice = $("#off-pric").val();
    var ratepermarla = $("#rate").val();
    var type = $("#type").val();
    if (type == "New_Lead") {
        var filplt = $("#fil-plt").val();
        if (filplt == "Plot") {
            var newleadplt = $("#NewLeadPlots").val();
            if (newleadplt == "" || newleadplt == null) {
                //alert("Please Select Plot");
                Swal.fire({
                    icon: 'info',
                    text: 'Select a plot to proceed'
                });
                return false;
            }
        }
        else if (filplt == "File") {
            var filenum = $("input[name=File_Plot_Number]").val();
            if (filenum == "" || filenum == null) {
                //alert("File Number Cannot be empty")
                Swal.fire({
                    icon: 'info',
                    text: 'Enter a file number to proceed'
                });
                return false;
            }
        }
        else if (filplt == "Apartment") {
            var appNum = $('.leadUnits').val();
            if (appNum == "" || appNum == null) {
                //alert("Unit No. Cannot be Empty");
                Swal.fire({
                    icon: 'info',
                    text: 'Enter unit number to proceed'
                });
                return false;
            }
        }
    }
    else if (type == "Resale") {
        var filplt = $("#fil-plt").val();
        if (filplt == "Plot") {
            var newleadplt = $("#plot-details").val();
            if (newleadplt == "" || newleadplt == null) {
                //alert("Please Select Plot");
                Swal.fire({
                    icon: 'info',
                    text: 'Select a plot to proceed'
                });
                return false;
            }
        }
        else if (filplt == "File") {
            var filenum = $("input[name=File_Plot_Number]").val();
            if (filenum == "" || filenum == null) {
                //alert("File Number Cannot be empty")
                Swal.fire({
                    icon: 'info',
                    text: 'Enter a file number to proceed'
                });
                return false;
            }
        }
        else if (filplt == "Apartment") {
            var appNum = $('.leadUnits').val();
            if (appNum == "" || appNum == null) {
                //alert("Unit No. Cannot be Empty");
                Swal.fire({
                    icon: 'info',
                    text: 'Enter unit number to proceed'
                });
                return false;
            }
        }
        if (demand == null || demand <= 0) {
            //alert("Demand Cannot be Zero or Empty");
            Swal.fire({
                icon: 'info',
                text: 'Enter a valid demand amount'
            });
            return false;
        }
    }
    if (offeredprice == null || offeredprice <= 0) {
        //alert("Offered Price Cannot be Zero or Empty");
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid offered amount'
        });
        return false;
    }
    if (ratepermarla == null || ratepermarla <= 0) {
        //alert("Rate Per Marla Cannot be Zero or Empty");
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid rate per Marla'
        });
        return false;
    }
    //if (confirm("Are you sure you want to Create Deal")) {
    Swal.fire({
        text: 'Are you sure you want to convert the lead to deal?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#c-l-deal').attr("action"),
                data: $('#c-l-deal').serialize(),
                success: function (data) {
                    if (data) {
                        //alert("Deal Created");
                        Swal.fire({
                            icon: 'success',
                            text: 'Deal created successfully'
                        }).then(() => {
                            window.location.reload();
                        })
                    }
                    else {
                        //alert("Previous Deal is still not Closed")
                        Swal.fire({
                            icon: 'info',
                            text: 'The previous deal has not yet been finalized'
                        });
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
// List of all Plots Transfer list
$(document).on("click", ".Plt-Lead-dets", function () {
    var id = $(this).attr("id");
    window.open("/Leads/LeadDetails?Id=" + id, "_blank")
});
// List of all Plots Transfer list
$(document).on("click", ".deal-dets", function () {
    var id = $(this).attr("id");
    window.open("/PropertyDeal/DealDetails?Id=" + id, "_blank")
});
//
$(document).on("click", ".cash-deal-dets", function () {
    var id = $(this).attr("id");
    window.open("/PropertyDeal/CashierDealDetails?Id=" + id, "_blank")
});
// List of all Plots Transfer list
$(document).on("click", ".t-det", function () {
    var id = $(this).attr("id");
    window.open("/Tickets/TicketDetails?Id=" + id, "_blank")
});
//
$(document).on("change", "#type", function (e) {
    var token = $(this).val();
    if (token === "Resale") {
        $(".ask-pric").show();
    }
    else {
        $(".ask-pric").hide();
    }
});
$(document).on("click", ".mig-data", function (e) {
    var id = $(this).closest("tr").attr("id");

    var a = confirm("Are you sure you want to Migeration File To Plot");
    if (a) {
        $.ajax({
            type: "POST",
            url: '/Balloting/MigrateFileRecordToPlotRecord/',
            data: { bp: id },
            success: function (data) {
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("change", ".led-plot-det", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: '/Plots/GetPlotData/',
        data: { Id: id },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $('#plt-no').text(data.Plot_No);
                $('#blk-no').text(data.Block_Name);
                $('#plt-size').text(data.Plot_Size);
                $('#plt-size-n').val(data.Plot_Size);
                $('#plt-type').text(data.Type);
                $('#plt-status').text(data.Develop_Status.replace('_', ' '));
                $('#plt-loc').text(data.Plot_Location);
                var html = "";
                if (data.Status == "Repurchased") {
                    html = '<div class="alert alert-info" style="text-align:center" role="alert">This Plot is Repurchased by Company</div>';
                }
                else if (data.Status == "Available_For_Sale") {
                    html = '<div class="alert alert-info" style="text-align:center" role="alert">This Plot is Available For Sale</div>';
                }
                else if (data.Status == "Disputed") {
                    html = '<div class="alert alert-warning" style="text-align:center" role="alert">This Plot is Disputed </div>';
                }
                else if (data.Status == "CancelledDueAmount") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Cancelled Due to Over Due Amount</div>';
                }
                else if (data.Status == "Cancelled") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Cancelled By Company</div>';
                }
                else if (data.Status == "Temporary_Cancelled") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Temporary Cancelled By Company</div>';
                }
                else if (data.Status == "Hold") {
                    html = '<div class="alert alert-dark" style="text-align:center" role="alert">This Plot is Holded by Company</div>';
                }
                $("#stat").html(html);
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
//
$(document).on("submit", "#up-lead", function (e) {
    e.preventDefault();
    var offpric = parseInt($("#off-price").val());
    if (offpric < 0 || offpric == "") {
        alert("Offered Price Cannot be zero");
        return false;
    }
    var prj = $('#prjName').val();
    if (prj == "SA Premium Homes") {
        var floor = $('#Floors option:selected').val();
        if (floor == "" || floor == null) {
            alert("Please Select Floor");
            return false;
        }
    }
    $('#ld-up-stat').attr("disabled", true);
    Swal.fire({
        text: 'Are you sure you want to update the lead?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#up-lead").attr('action'),
                data: $("#up-lead").serialize(),
                success: function (data) {
                    //alert("Updated");
                    Swal.fire({
                        icon: 'success',
                        text: 'Lead updated successfully'
                    }).then(() => {
                        window.location.reload();
                    })
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    }).then(() => {
                        $('#ld-up-stat').attr("disabled", false);
                        $('#gen-plot-rec').attr("disabled", false);
                    })
                }
            });
        }
    });
});
//
$(document).on("click", "#reg-ex-file", function (e) {
    e.preventDefault();
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        allrecepts.push(recedata);
    }
    $("#reg-file").prop("disabled", true);
    var fileno = $("#app-num").val();
    var fulpaid = false;
    var regfiledata = {
        Id: 0,
        Plot_Size: $("#pl-size").val(),
        Name: $("input[name=Name]").val(),
        Father_Husband: $("input[name=Father_Husband]").val(),
        Postal_Address: $("input[name=Postal_Address]").val(),
        Residential_Address: $("input[name=Residential_Address]").val(),
        Phone_Office: $("input[name=Phone_Office]").val(),
        Residential: $("input[name=Residential]").val(),
        Mobile_1: $("input[name=Mobile_1]").val(),
        Mobile_2: $("input[name=Mobile_2]").val(),
        Email: $("input[name=Email]").val(),
        Occupation: $("input[name=Occupation]").val(),
        Nationality: $("input[name=Nationality]").val(),
        Date_Of_Birth: $("input[name=Date_Of_Birth]").val(),
        CNIC_NICOP: $("input[name=CNIC_NICOP]").val(),
        Nominee_Name: $("input[name=Nominee_Name]").val(),
        Nominee_Relation: $("input[name=Nominee_Relation]").val(),
        Nominee_Address: $("input[name=Nominee_Address]").val(),
        Nominee_CNIC_NICOP: $("input[name=Nominee_CNIC_NICOP]").val(),
        Plot_Prefrence: $("#Plot_Prefrence").val(),
        File_Form_Id: $("#file-id").val(),
        Phase_Id: $("#phs_Id").val(),
        Block_Id: $("#blk_Id").val(),
        QR_Code: $("#qr_Id").val(),
        City: $("#City").val(),
        Rate: 0,
        Total: 0,
        GrandTotal: 0,
        Delivery: 0,
    }
    var alldata = {
        filedata: regfiledata,
        Flag: flag,
        DevCharStatus: $("#dev-char-st").val(),
        FileFormNumber: $("#app-num").val(),
        Receiptdata: allrecepts,
        FullPaid: fulpaid
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/FileSystem/RegisterFile/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data.Status == "-1") {
                    alert("No Installment Plan Has Found Contact Administrator")
                }
                if (data.Status == "0") {
                    alert("Already a Plan is Generated");
                }
                else if (data.Status == "1") {
                    alert("File No. " + fileno + " Has Been Registerd");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
                else if (data.Status == "2") {
                    alert("Wait Until You Payment is Clear from Bank");
                    $(data.Receiptid).each(function (i) {
                        window.open("/Banking/Receipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                    });
                    window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".add-elc-met", function () {
    var html = '<div class="form-row"><div class="form-group col-md-3"><label>Meter No :</label><input class="form-control" id="met-no" /></div><div class="form-group col-md-1"><label>------</label><input type="button" class="btn btn-primary sav-met" value="Save" /></div></div>';
    $("#addmet").html(html);
});
//
$(document).on("click", ".get-f-res-ver-par", function () {
    var val = $("#app-num").val();
    $('#file-data').load('/FileSystem/GetFileVeriR/', { FileId: val }, function () { });
});
//
$(document).on("click", ".get-f-res-ver", function () {
    var val = $(this).attr("id");
    window.open('/FileSystem/GetFileVeriResult?FileId=' + val, '_blank');
});
//
$(document).on("click", ".get-b-res-ver", function () {
    var val = $(this).attr("id");
    window.open('/Commercial/DetailInformation?Commercial_Id=' + val, '_blank');
});
//
$(document).on("click", ".up-f-stat", function () {
    var id = $("#app-num").val();
    var stat = $("#status").text();
    EmptyModel();
    $('#modalbody').load('/FileSystem/UpdateFileStatus/', { FileId: id, Status: stat }, function () { });
});
//
$(document).on("click", "#up-f-stat-btn", function (e) {
    e.preventDefault();
    Swal.fire({
        text: 'Are you sure you want to update file status?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $('#up-f-stat').attr("action"),
                data: $('#up-f-stat').serialize(),
                success: function (data) {
                    if (data.Status === "Requested") {
                        //alert("Cancelation Requested");
                        Swal.fire({
                            icon: 'success',
                            text: 'File cancellation requested successfully'
                        });
                    }
                    if (data.Status === "Repurchased") {
                        //alert("Repurchased and Cancelation Requested");
                        Swal.fire({
                            icon: 'success',
                            text: 'File repurchase and cancellation requested successfully'
                        });
                    }
                    else {
                        window.location.reload();
                    }
                },
                error: function (data) {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", ".sav-met", function () {
    var meterno = $("#met-no").val();
    var id = $("#plot_id").val();
    $.ajax({
        type: "POST",
        url: '/ServiceCharges/AddMeter/',
        data: { PlotId: id, MeterNo: meterno },
        success: function (data) {
            if (data == true) {
                alert("Added");
                $("#addmet").empty();
                var html = '<div class="form-row"><div class="form-group col-md-3"><label>Meter No :</label><input class="form-control" readonly value="' + meterno + '" /></div></div>';
                $("#addmet").append(html);
            }
            else {
                alert(data);
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".del-reg-fee", function () {
    var id = $("#Dealership").val();
    var amt = $("#amt").val();
    var trans = $("#trans-id").val();
    var paymentType = $("#cah-chq-bak").val();
    var instNo = $("#paymenttype").val();
    var bankN = $("#Bank option:selected").val();
    var branch = $("#Branch").val();
    var instDate = $("#instDate").val();
    if (!amt || amt == 0) {
        alert("Amount Not Entered!");
    }
    else if (!id) {
        alert("Dealership Not Selected!");
    }
    else {
        $.ajax({
            type: "POST",
            url: '/Dealership/DealerRegister/',
            data: { Id: id, Amount: amt, TransactionId: trans, payType: paymentType, chqNo: instNo, bankName: bankN, bankBranch: branch, payDate: instDate },
            success: function (data) {
                window.open("/Receipts/Dealership_Registeration?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".del-sec-fee-ret", function () {
    var id = $("#Dealership").val();
    var amt = $("#amt").val();
    var trans = $("#trans-id").val();
    var lednat = $('#pay-nat').val();
    var desc = $("#desc").val();
    var paytype = $("#cah-chq-bak").val();
    var instno = $("#inst-no").val();
    var bank = $("#Bank").val();
    var bran = $("#Branch").val();
    var img = $("#scanned").attr('src');
    var instdate = $("#inst-date").val();

    var taxName = "";
    var tax_id = $('.tax-accounts option:selected').val();
    if (tax_id != "") {
        taxName = $('.tax-accounts option:selected').text();
    }
    var taxamount = RemoveComma($('.tax-amt').val());
    if (taxamount == "" || taxamount == null || amt < 0) {
        taxamount = 0;
    }
    if (taxamount > 0 && taxName == "") {
        //alert("Please Select Tax Account");
        Swal.fire({
            icon: 'info',
            text: 'Select Tax account to proceed'
        });
        return false;
    }

    if ((Number(amt) + Number(taxamount)) <= Number($('#ttlamt').val())) {
        //if (confirm("Are you sure want to Generate Voucher")) {
        Swal.fire({
            text: 'Are you sure want to generate the Voucher?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "POST",
                    url: '/Dealership/DealerPayment/',
                    data: { Id: id, Amount: amt, TransactionId: trans, PaymentType: paytype, Bank: bank, Inst_No: instno, Inst_Date: instdate, Branch: bran, Description: desc, Img: img, TaxAmount: taxamount, TaxAccount: tax_id, Led_Nat: lednat },
                    success: function (data) {
                        if (data.Status == true) {
                            window.open("/Vouchers/Voucher?GroupId=" + data.Token, '_blank');

                            window.location.reload();
                        }
                        else {
                            alert(data.Msg)
                        }
                    },
                    error: function () {
                        alert("Error Occured");
                    }
                });
            }
        })
    }
    else {
        //alert("Invalid Amount");
        Swal.fire({
            icon: 'info',
            text: 'Enter a valid amount to proceed'
        });
    }
});
// create a Company
$(document).on("submit", "#ad-com", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-com").attr('action'),
        data: $("#ad-com").serialize(),
        success: function (data) {
            if (data) {
                alert("Company Added");
                window.location.reload();
            }
            else {
                alert("Company Already Exists");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// create a Department
$(document).on("submit", "#ad-dep", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-dep").attr('action'),
        data: $("#ad-dep").serialize(),
        success: function (data) {
            if (data) {
                alert("Department Added");
                submitNewDepEmps(data);
            }
            else {
                alert("Department Already Exists");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// create a Designation
$(document).on("submit", "#ad-des", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#ad-des").attr('action'),
        data: $("#ad-des").serialize(),
        success: function (data) {
            if (data) {
                alert("Designation Added");
                window.location.reload();
            }
            else {
                alert("Designation Already Exists");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Register  a file
$(document).on("click", "#gen-sam-rec", function (e) {
    e.preventDefault();
    var recedata = {
        Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
        Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
    };
    recedata.Amount = $("#amt").val();
    recedata.AmountInWords = InWords($("#amt").val());
    recedata.Bank = $("#Bank").val();
    recedata.PayChqNo = $("#paymenttype").val();
    recedata.PaymentType = $("#cah-chq-bak").val();
    recedata.Project_Name = $("#prj").val();
    recedata.Branch = $("#Branch").val();
    recedata.Ch_bk_Pay_Date = $("#cbp-date").val();
    recedata.Project_Name = $("#prj-name").val();
    $("#reg-file").prop("disabled", true);
    var alldata = {
        Id: $("#amt-file-id").val(),
        rd: recedata,
        TransactionId: $("#trans-id").val(),
    };
    if (recedata.Amount <= 0 || recedata.Amount == "") {
        //alert("Amount Cannot be Zero or Empty");
        Swal.fire({
            icon: 'info',
            text: 'Enter valid amount to proceed'
        });
        return false;
    }
    //var con = confirm("Are you sure you want to Generate Receipt");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Receipt?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Installments/PayLeadPaymentWithReceipt/',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(alldata),
                success: function (data) {
                    if (data.Status == true) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Receipt generated successfully'
                        }).then(() => {
                            window.open("/Receipts/SAM_Receipts?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                            window.location.reload();
                        })  
                    }
                    else {
                        //alert("You cannot receive more than offered Amount");
                        Swal.fire({
                            icon: 'info',
                            text: 'You cannot receive an amount greater than the offered amount'
                        });
                    }
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
//
$(document).on("click", ".up-dire-rec", function () {
    EmptyModel();
    var id = $(this).closest('tr').attr('id');
    var amt = $("#" + id + " .deal-amt").text();
    var paymenttype = $("#" + id + " .pay-type").text();
    var html = '<input type="hidden" value="' + id + '" id="deal-rece"><div class="form-row"><div class="form-group col-md-3"><label class="">Amount</label>  <h6>' + amt + '</h6></div>' +
        '<div class="form-group col-md-8"><label class="">Description</label>  <input id="desc" class="form-control" id="Description" placeholder="Description" required></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="up-dire-pay" type="submit">Cancel Direct Payment</button>');
});
//
$(document).on("click", "#up-dire-pay", function (e) {
    e.preventDefault();
    var id = $("#deal-rece").val();
    var des = $("#Description").val();
    var con = confirm("Are you sure you want to Cancel Direct Payment");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/PropertyDeal/UpdateDealReceipt/',
            data: { Id: id, Description: des },
            success: function (data) {
                alert("Payment Updated");
                window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".ld-pay-vou-req", function () {
    EmptyModel();
    var html = '<div class="form-row"><div class="form-group col-md-4"><label class="">Amount</label>  <input class="form-control coma" placeholder="250,000" required><input type="hidden" id="Amount" class="amt" required></div>' +
        '<div class="form-group col-md-8"><label class="">Description</label>  <input id="desc" class="form-control" name="Description" placeholder="Description Written on Payment Voucher" required></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="req-vouch" type="submit">Request for Payment Voucher</button>');
});
// Register  a file
$(document).on("click", "#gen-sam-rec-man", function (e) {
    e.preventDefault();
    var recedata = {
        Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
        Project_Name: "", Branch: "", Ch_bk_Pay_Date: "",
    };
    recedata.Amount = $("#amt").val();
    recedata.AmountInWords = InWords($("#amt").val());
    recedata.Bank = $("#bank").val();
    recedata.PayChqNo = $("#paymenttype").val();
    recedata.PaymentType = $("#cah-chq-bak").val();
    recedata.Project_Name = $("#prj").val();
    recedata.Branch = $("#Branch").val();
    recedata.Ch_bk_Pay_Date = $("#cbp-date").val();
    recedata.Project_Name = "Grand City";
    recedata.ReceiptNo = $("#rece-no").val();
    recedata.Date = $("#rece-date").val();
    $("#reg-file").prop("disabled", true);
    var alldata = {
        Id: $("#amt-file-id").val(),
        rd: recedata,
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Leads/LeadManualReceipt/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(alldata),
            success: function (data) {
                alert("Receipt added")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".add-sam-rec-man", function () {
    EmptyModel();
    var id = $("#l-id").val();
    $('#modalbody').load('/Leads/AddLeadReceipts/', { Id: id });
});
//
$(document).on("click", ".ld-to-deal", function () {
    EmptyModel();
    var id = $("#l-id").val();
    $('#modalbody').load('/Leads/LeadsToDeal/', { Id: id });
});
// Register  a file
$(document).on("click", "#req-vouch", function (e) {
    e.preventDefault();
    var id = $("#l-id").val();
    var amount = Number($("#Amount").val());
    var desc = $("#desc").val();
    var offeredamt = Number($("#off-price").val());
    var recamt = Number($("#receivedAmount").val());
    var remamout = Number($("#Remaingamount").val());
    var vocamt = Number($("#VoucherAmount").val());
    if (desc == null || desc == "") {
        alert("Please Enter Description")
        return false;
    }
    if (amount < 0 || amount == "") {
        alert("Amount cannot be empty or 0");
        return false;
    }
    if (amount > remamout) {
        alert("You cannot request more than Received Amount");
        return false;
    }
    var con = confirm("Are you sure you want to Request for Voucher");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Vouchers/RequestPaymentVoucher/',
            data: { Id: id, Amount: amount, Description: desc },
            success: function (data) {
                alert("Requested")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});

$(document).on("change", ".developstat", function (e) {
    e.preventDefault();
    var id = $("#plt-id").val();
    var val = $(this).val();
    //var con = confirm("Are you sure you want to Update");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to update development status of the plot?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Plots/UpdateConstructionStatus/',
                data: { Id: id, DevelopStatus: val },
                success: function (data) {
                    //alert("Requested")
                    Swal.fire({
                        icon: 'success',
                        text: 'Development status update requested successfully'
                    });
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});

// Register  a file
$(document).on("click", "#up-vou-app-btn", function (e) {
    e.preventDefault();
    //var con = confirm("Are you sure you want to Confirm the Voucher");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to generate the Voucher?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: $("#up-vou-app").attr("action"),
                data: $("#up-vou-app").serialize(),
                success: function (data) {
                    Swal.fire({
                        icon: 'success',
                        text: 'Voucher generated successfully'
                    }).then(() => {
                        window.open("/Vouchers/SAM_Voucher?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
                        window.location.reload();
                    })   
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
// show and hide tab of add employee
$(document).on("click", "#move3rdstep", function () {
    $('.st1').hide();
    $('.st2').hide();
    $('.st3').show();
});

function getbookingdata() {
    var stdate = $("#startdate").val();
    var enddate = $("#enddate").val();
    SASLoad("#bookin");
    if (stdate != '' && enddate != '') {
        $("#bookin").load('/Reports/PlotBooking/', { Startdate: stdate, Enddate: enddate }, function () {
            SASUnLoad("#bookin");
        });
        $("#bookingplot").load('/Reports/PlotBookingBarGraph/', { Startdate: stdate, Enddate: enddate });
    }
}
//
function getoptions() {
    var op = $("#opt").val();
    var today = new Date();
    var stdate = '';
    var enddate = '';
    var d = today.getDate();
    var m = today.getMonth() + 1;
    var yyy = today.getFullYear();
    if (d < 10) {
        d = '0' + d;
    }
    if (m < 10) {
        m = '0' + m;
    }
    enddate = m + '-' + d + '-' + yyy;
    if (op == "weekly") {
        var lastWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
        var dd = lastWeek.getDate();
        var mm = lastWeek.getMonth() + 1;
        var yyyy = lastWeek.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        stdate = mm + '-' + dd + '-' + yyyy;
    }
    if (op == 'monthly') {
        var prevMonthLastDate = new Date(today.getFullYear(), today.getMonth(), 0);
        var prevMonthFirstDate = new Date(today.getFullYear() - (today.getMonth() > 0 ? 0 : 1), (today.getMonth() - 1 + 12) % 12, 1);
        var firstdate = prevMonthFirstDate.getDate();
        var m = prevMonthFirstDate.getMonth() + 1;
        if (firstdate < 10) {
            firstdate = '0' + firstdate;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + firstdate + '-' + prevMonthFirstDate.getFullYear();
        enddate = m + '-' + prevMonthLastDate.getDate() + '-' + prevMonthLastDate.getFullYear();
    }
    if (op == '6months') {
        var sixMonthsFromNow = new Date(today.setMonth(today.getMonth() - 6));
        var d = sixMonthsFromNow.getDate();
        var m = sixMonthsFromNow.getMonth() + 1;
        if (d < 10) {
            d = '0' + d;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + d + '-' + sixMonthsFromNow.getFullYear();
    }
    if (op == 'yearly') {
        var curyear = today.getFullYear();
        var curyearMonth = today.getMonth() + 1;
        var curyearDay = today.getDate();
        var lastYear = curyear - 1;
        if ((curyearMonth == 2) && (curyearDay == 29)) {
            curyearDay = 28;
        }
        stdate = ("00" + curyearMonth.toString()).slice(-2) + "-" + ("00" + curyearDay.toString()).slice(-2) + '-' + ("0000" + lastYear.toString()).slice(-4);
    }
    $("#bookin").load('/Reports/PlotBooking/', { Startdate: stdate, Enddate: enddate });
    $("#bookingplot").load('/Reports/PlotBookingBarGraph/', { Startdate: stdate, Enddate: enddate });
}
//.................plot transfer report
function gettransferdata() {
    var stdate = $("#transferstartdate").val();
    var enddate = $("#transferenddate").val();
    if (stdate != '' && enddate != '') {
        $("#transferplot").load('/Reports/PlotTranfer/', { Startdate: stdate, Enddate: enddate });
        $("#transferbarplot").load('/Reports/PlotTransferBarGraph/', { Startdate: stdate, Enddate: enddate });
    }
}
function getpurrep() {
    var stdate = $("#po-start").val();
    var enddate = $("#po-to").val();
    if (stdate != '' && enddate != '') {
        $("#pur-order-repo").load('/Graph/PurchaseOrdersDep/', { From: stdate, To: enddate });
    }
}
//Blocks AllRecovery Report
$(document).on("click", ".all-recovery-search", function () {
    var blockId = $("#Blocks option:selected").val();
    var blockName = $("#Blocks option:selected").text();
    var propertyType = $(".property-type-dd").val();
    $("#file-year").load('/Reports/FilesYearlyRecovery/', { block: blockId, blockName: blockName, propertytype: propertyType });
});
function gettransferoptions() {
    var op = $("#opttransfer").val();
    var today = new Date();
    var stdate = '';
    var enddate = '';
    var d = today.getDate();
    var m = today.getMonth() + 1;
    var yyy = today.getFullYear();
    if (d < 10) {
        d = '0' + d;
    }
    if (m < 10) {
        m = '0' + m;
    }
    enddate = m + '-' + d + '-' + yyy;
    if (op == "weekly") {
        var lastWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
        var dd = lastWeek.getDate();
        var mm = lastWeek.getMonth() + 1;
        var yyyy = lastWeek.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        stdate = mm + '-' + dd + '-' + yyyy;
    }
    if (op == 'monthly') {
        var prevMonthLastDate = new Date(today.getFullYear(), today.getMonth(), 0);
        var prevMonthFirstDate = new Date(today.getFullYear() - (today.getMonth() > 0 ? 0 : 1), (today.getMonth() - 1 + 12) % 12, 1);
        var firstdate = prevMonthFirstDate.getDate();
        var m = prevMonthFirstDate.getMonth() + 1;
        if (firstdate < 10) {
            firstdate = '0' + firstdate;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + firstdate + '-' + prevMonthFirstDate.getFullYear();
        enddate = m + '-' + prevMonthLastDate.getDate() + '-' + prevMonthLastDate.getFullYear();
    }
    if (op == '6months') {
        var sixMonthsFromNow = new Date(today.setMonth(today.getMonth() - 6));
        var d = sixMonthsFromNow.getDate();
        var m = sixMonthsFromNow.getMonth() + 1;
        if (d < 10) {
            d = '0' + d;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + d + '-' + sixMonthsFromNow.getFullYear();
    }
    if (op == 'yearly') {
        var curyear = today.getFullYear();
        var curyearMonth = today.getMonth() + 1;
        var curyearDay = today.getDate();
        var lastYear = curyear - 1;
        if ((curyearMonth == 2) && (curyearDay == 29)) {
            curyearDay = 28;
        }
        stdate = ("00" + curyearMonth.toString()).slice(-2) + "-" + ("00" + curyearDay.toString()).slice(-2) + '-' + ("0000" + lastYear.toString()).slice(-4);
    }
    $("#transferplot").load('/Reports/PlotTranfer/', { Startdate: stdate, Enddate: enddate });
    $("#transferbarplot").load('/Reports/PlotTransferBarGraph/', { Startdate: stdate, Enddate: enddate });
}
//......Revenue Report
function getrevenuedata() {
    var stdate = $("#revenuestartdate").val();
    var enddate = $("#revenueenddate").val();
    if (stdate != '' && enddate != '') {
        $("#revreport").load('/Reports/RevenueReport/', { From: stdate, To: enddate });
    }
}

function GetBlkRecov() {
    var stdate = $("#blk-rec-start").val();
    var enddate = $("#blk-rec-to").val();
    if (stdate != '' && enddate != '') {
        $("#blk-recovery").load('/Reports/PlotsRecovery/', { From: stdate, To: enddate });
    }
}
//......Payment Report
function getpaydata() {
    var stdate = $("#p-st-dat").val();
    var enddate = $("#p-end-dat").val();
    if (stdate != '' && enddate != '') {
        $("#paymentreport").load('/Reports/PayableReport/', { From: stdate, To: enddate });
    }
}

//......Payment Report
//
$(document).on("click", ".src-inst-rep", function () {
    var stdate = $("#monthdate").val();
    var todate = $("#monthtodate").val();
    var type = $(".blk-type").val();
    var typename = $(".blk-type :selected").text();

    SASLoad($('#instrep'));
    $("#instrep").load('/Reports/MonthlyInstallment/', { From: stdate, To: todate, Type: type, TypeName: typename }, function () {
        SASUnLoad($("#instrep"));
    });
});
//......Payment Report
function getmonthiodata() {
    var stdate = $("#iofrom").val();
    var todate = $("#ioto").val();
    if (stdate != '') {
        $("#ifof-div").load('/Graph/MonthlyInflow_Outflow/', { From: stdate, To: todate });
    }
}
//......Payment Report
function get_rec_rep() {
    var stdate = $("#f-res").val();
    var todate = $("#t-res").val();
    if (stdate != '' || todate != '') {
        $("#dail-cash-rep").load('/Graph/DailyCollectionSearch/', { From: stdate, To: todate });
    }
}
// Leads Report
//
function getleadsdata() {
    var stdate = $("#startdate").val();
    var enddate = $("#enddate").val();
    var user = $("#LeadsUser").val();
    var stat = $("#SAM_Lead_Status").val();
    var comp = $("#comp").val();
    if (stdate != '' && enddate != '') {
        $("#lst-deal").load('/Reports/DealsListReport/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
        $("#us-lds").load('/Reports/LeadsStatusReport/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
        $("#ld-stat").load('/Reports/LeadsRecovery/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
        $("#ld-plt-stat").load('/Reports/LeadsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
        $("#pd-n-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'New_Lead' });
        $("#pd-r-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'Resale' });
        $("#pd-i-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'Investment' });
    }
}
//
function getleadsoptions() {
    var user = $("#LeadsUser").val();
    var stat = $("#SAM_Lead_Status").val();
    var comp = $("#comp").val();
    var op = $("#opt").val();
    var today = new Date();
    var stdate = '';
    var enddate = '';
    var d = today.getDate();
    var m = today.getMonth() + 1;
    var yyy = today.getFullYear();
    if (d < 10) {
        d = '0' + d;
    }
    if (m < 10) {
        m = '0' + m;
    }
    enddate = m + '-' + d + '-' + yyy;
    if (op == "weekly") {
        var lastWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
        var dd = lastWeek.getDate();
        var mm = lastWeek.getMonth() + 1;
        var yyyy = lastWeek.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        stdate = mm + '-' + dd + '-' + yyyy;
    }
    if (op == 'monthly') {
        var prevMonthLastDate = new Date(today.getFullYear(), today.getMonth(), 0);
        var prevMonthFirstDate = new Date(today.getFullYear() - (today.getMonth() > 0 ? 0 : 1), (today.getMonth() - 1 + 12) % 12, 1);
        var firstdate = prevMonthFirstDate.getDate();
        var m = prevMonthFirstDate.getMonth() + 1;
        if (firstdate < 10) {
            firstdate = '0' + firstdate;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + firstdate + '-' + prevMonthFirstDate.getFullYear();
        enddate = m + '-' + prevMonthLastDate.getDate() + '-' + prevMonthLastDate.getFullYear();
    }
    if (op == '6months') {
        var sixMonthsFromNow = new Date(today.setMonth(today.getMonth() - 6));
        var d = sixMonthsFromNow.getDate();
        var m = sixMonthsFromNow.getMonth() + 1;
        if (d < 10) {
            d = '0' + d;
        }
        if (m < 10) {
            m = '0' + m;
        }
        stdate = m + '-' + d + '-' + sixMonthsFromNow.getFullYear();
    }
    if (op == 'yearly') {
        var curyear = today.getFullYear();
        var curyearMonth = today.getMonth() + 1;
        var curyearDay = today.getDate();
        var lastYear = curyear - 1;
        if ((curyearMonth == 2) && (curyearDay == 29)) {
            curyearDay = 28;
        }
        stdate = ("00" + curyearMonth.toString()).slice(-2) + "-" + ("00" + curyearDay.toString()).slice(-2) + '-' + ("0000" + lastYear.toString()).slice(-4);
    }
    $("#lst-deal").load('/Reports/DealsListReport/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
    $("#us-lds").load('/Reports/LeadsStatusReport/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
    $("#ld-stat").load('/Reports/LeadsRecovery/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
    $("#ld-plt-stat").load('/Reports/LeadsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp });
    $("#pd-n-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'New_Lead' });
    $("#pd-r-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'Resale' });
    $("#pd-i-plt-stat").load('/Reports/PropertyDealsPlotSizeReports/', { Status: stat, Startdate: stdate, Enddate: enddate, Users: user, Comp: comp, Type: 'Investment' });
}
//
$(document).on("click", ".ld-rec", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Lead Installment");
    $('#modalbody').load('/Installments/PayLeadPayment/', { Id: id });
});
//
$(document).on("click", ".led-vou", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Lead Voucher");
    $('#modalbody').load('/Installments/PayLeadPayment/', { Id: id });
});
//
$(document).on("click", ".led-vou-app", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Lead Voucher");
    $('#modalbody').load('/Vouchers/VoucherRequestDetails/', { Id: id });
});
//
$(document).on("change", ".monthinstrep", function () {
    var val = $(this).val();
    var stdate, todate;
    if (val == "twomonth") {
        stdate = moment().subtract(1, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "thremonth") {
        stdate = moment().subtract(2, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q1") {
        stdate = moment('2019-01-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q2") {
        stdate = moment('2019-04-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q3") {
        stdate = moment('2019-07-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q4") {
        stdate = moment('2019-10-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "6months") {
        stdate = moment().subtract(6, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "yearly") {
        stdate = moment().subtract(12, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    $("#instrep").load('/Reports/MonthlyInstallment/', { From: stdate, To: todate });
});
//
$(document).on("change", ".monthifofrep", function () {
    var val = $(this).val();
    var stdate, todate;
    if (val == "twomonth") {
        stdate = moment().subtract(1, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "thremonth") {
        stdate = moment().subtract(2, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q1") {
        stdate = moment('2019-01-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q2") {
        stdate = moment('2019-04-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q3") {
        stdate = moment('2019-07-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "q4") {
        stdate = moment('2019-10-01').utc().quarter().format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "6months") {
        stdate = moment().subtract(6, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    else if (val == "yearly") {
        stdate = moment().subtract(12, 'months').format('MM/DD/YYYY');
        todate = moment().format('MM/DD/YYYY');
    }
    $("#ifof-div").load('/Graph/MonthlyInflow_Outflow/', { From: stdate, To: todate });
});
// initialize all comp
function InitCompany(i) {
    $("#des-" + i + " .get-dep").append('<option value="">Select Company</option>');
    $.each(comp, function (key, value) {
        $("#des-" + i + " .get-dep").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
function loadvalueCompanyvalues() {
    $.ajax({
        type: "POST",
        url: "/Company/GetCompanyfor/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({}),
        success: function (data) {
            //$('').append('<option>Select Department</option>');
            $.each(data.compan, function (key, value) {
                $('.test').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
}
//// add a department desgnation company
//$(document).on("click", ".ad-cdd", function () {
//    paycont++;
//    var html = '<div class="col-md-10 c-d-d" id="des-' + paycont + '"><div class="row"><div class="form-group col-md-3"><label>Company</label><select class="form-control get-dep test" data-id="' + paycont + '"></select></div><div class="form-group col-md-3">' +
//        '<label>Departments</label><select class="form-control com-dep" data-id="' + paycont + '" name="Parent_Dep"></select></div><div class="form-group col-md-3"><label>Designation</label><select class="form-control dep-des" data-id="' + paycont + '" name="Parent_Dep"></select>' +
//        '</div><input name="Dep_Des" class="com-dep-des" type="hidden" /><div class="col-md-1"><i class="ti-minus rm-des"></i><i class="ti-plus  add-PlotDepartmentDesignation add-pdd-hide"></i></div></div>';
//    $('#com-dep-des').append(html);
//    InitCompany(paycont);
//});
//add another company department and designation
$(document).on("click", ".ad-cdd", function () {
    paycont++;
    var html = '<div class="col-md-10 cdd" id="des-' + paycont + '"> <div class="row"><h6 class="lh-1 mB-0 add-des-counter">' + paycont + '.</h6><div class="form-group col-md-3"><label>Company</label><select class="form-control get-dep test" data-id="' + paycont + '"></select></div><div class="form-group col-md-3">' +
        '<label>Departments</label><select class="form-control com-dep" data-id="' + paycont + '" name="Parent_Dep"></select></div><div class="form-group col-md-3"><label>Designation</label><select class="form-control dep-des" data-id="' + paycont + '" name="Parent_Dep"></select>' +
        '</div><input name="Dep_Des" class="com-dep-des" type="hidden" /> <input  name="Dep_Des_Val"  class="com-dep-des-value" type="hidden" /><div class="col-md-1"><i class="ti-minus rm-des"></i></div></div>';
    $('#com-dep-des').append(html);
    InitCompany(paycont);
});

$(document).on("click", ".rm-des", function () {
    $(this).parent().parent().parent().remove();
    paycont = 1;
    $(".c-d-d").each(function () {
        paycont++;
        $(this).prop('id', 'des-' + paycont);
    });
});

$(document).on("change", ".get-dep-ad-des", function () {
    var id = $(this).val();
    var at = $(this).data("id");
    $.ajax({
        type: "POST",
        url: "/Company/GetCompanyDepartmentsOnly/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id }),
        success: function (data) {
            $('#des-' + at + ' .com-dep').html(' ');
            $('#des-' + at + ' .com-dep').append('<option>Select Department</option>');
            $.each(data.Departments, function (key, value) {
                $('#des-' + at + ' .com-dep').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("change", ".get-dep", function () {
    var id = $(this).val();
    var at = $(this).data("id");
    $.ajax({
        type: "POST",
        url: "/Company/GetCompanyDepartments/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id }),
        success: function (data) {
            $('#des-' + at + ' .com-dep').html(' ');
            $('#des-' + at + ' .com-dep').append('<option>Select Department</option>');
            $.each(data.Departments, function (key, value) {
                $('#des-' + at + ' .com-dep').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("change", ".com-dep", function () {
    var id = $(this).val();
    var at = $(this).data("id");
    $.ajax({
        type: "POST",
        url: "/Company/GetDepDesignation/",
        data: { Id: id },
        success: function (data) {
            $('#des-' + at + ' .dep-des').html(' ');
            $('#des-' + at + ' .dep-des').append('<option>Select Designation</option>');
            $.each(data, function (key, value) {
                $('#des-' + at + ' .dep-des').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// create a Employee
$(document).on("submit", "#ad-emp", function (e) {
    e.preventDefault();
    for (var i = 1; i <= paycont; i++) {
        var comp = $("#des-" + i + " .get-dep option:selected").text();
        var dep = $("#des-" + i + " .com-dep option:selected").text();
        var des = $("#des-" + i + " .dep-des option:selected").text();
        var compv = $("#des-" + i + " .get-dep").val() || 0;
        var depv = $("#des-" + i + " .com-dep").val() || 0;
        var desv = $("#des-" + i + " .dep-des").val() || 0;
        if (comp == "" || comp == null || comp == "Select Company") {
            alert("Select Company")
            $("#des-" + i + " .get-dep").focus();
            return false;
        }
        if (dep == 'Select Department') {
            dep = ' ';
            depv = 0;
        }
        var name = comp + ' - ' + dep + ' - ' + des;
        var val = compv + ' - ' + depv + ' - ' + desv;
        $("#des-" + i + " .com-dep-des").val(name);
        $("#des-" + i + " .com-dep-des-value").val(val);
    }
    var img = $("#own_img").attr("src");
    $("#image").val(img);
    //var reporting_To = $('.Reporting_To').val();
    //var reporting_To_2 = $('.Reporting_To_2').val();
    //if (reporting_To == id || reporting_To_2 == id) {
    //    alert('Cant report to himself!!')
    //    return;
    //}
    $.ajax({
        type: "POST",
        url: $("#ad-emp").attr('action'),
        data: $("#ad-emp").serialize(),
        success: function (data) {
            if (data) {
                $('.st1').hide();
                $('.st2').show();
                $('.st3').hide();
                $('.Employe_id').val(data);
                alert("Employee basic informaion added");
                //window.location.reload();
            }
            else {
                alert("Designation Already Exists");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});

$(document).on("click", ".ad-led-tok", function () {
    $(".tok-fld").show();
});

$(document).on("click", ".finalize", function () {
    window.location.reload();
});

$(document).on("click", "#move3rdstep", function () {
    $('.st1').hide();
    $('.st2').hide();
    $('.st3').show();
});

$(document).on("click", ".RemoveAssetListFromList", function () {
    var i = $(this).attr("id");
    var id = $("#emp-id").val();
    $.ajax({
        type: "POST",
        url: '/Company/RemoveAssetList/',
        data: { Id: i },
        success: function (data) {
            if (!data) {
                alert("error Occured")
            }
            else {
                window.location.reload(true);
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("submit", "#leaveform", function (e) {
    e.preventDefault();
    var LeaveType = $('input[name=LeaveType]:checked').val();
    let apldDays = $('#NoOfDays').val();
    let grntdLvs = parseInt($('.ttlGrantedLeaves').val());
    grntdLvs = Math.floor(grntdLvs / 4);
    if (typeof LeaveType === "undefined") {
        alert('Please Select Leave Type');
        return false;
    }
    if ((LeaveType == "Annual") && (apldDays < grntdLvs)) {
        alert('Annual Leaves cannot be less than 50% of the allotted leaves');
        return false;
    }
    $.ajax({
        type: "POST",
        url: $(this).attr("action"),
        data: $(this).serialize(),
        success: function (data) {
            if (data == "false") {
                alert("Already created");
            }
            else {
                alert("Created");
            }
        }
    });
});
$(document).on("blur", ".db", function () {
    let val = RemoveComma($(this).val());
    let id = $(this).closest('tr').attr('id');
    $.ajax({
        type: "POST",
        url: "/OpeningClosingBalance/EditOpeningBal/",
        data: { id: id, D_bal: val, C_Bal: 0 },
        success: function () {

        }
    });
});
$(document).on("blur", ".cb", function () {
    let val = RemoveComma($(this).val());
    let id = $(this).closest('tr').attr('id');
    $.ajax({
        type: "POST",
        url: "/OpeningClosingBalance/EditOpeningBal/",
        data: { id: id, D_bal: 0, C_Bal: val },
        success: function () {

        }
    });
});
//
$(document).on("submit", "#leaveformOfficials", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $(this).attr("action"),
        data: $(this).serialize(),
        success: function (data) {
            if (data == "false") {
                alert("Already created");
            }
            else {
                alert("Created");
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//////////////////////////////// *************** salaries working functions
var salariesAllown = 1;
function Initsalariesall(i) {
    $("#allownce-" + i + " .allon-typ").append('<option value="' + 0 + '">Choose..</option>');
    $.each(salar_alwnc, function (key, value) {
        $("#allownce-" + i + " .allon-typ").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
$(document).on("click", ".add_another_allownce", function () {
    salariesAllown++;
    var html = '<div class="col-md-10 S_A" id="allownce-' + salariesAllown + '">' +
        '<div class="row">' +
        '<div class="form-group col-md-3">' +
        '<label>Allownce Type</label>' +
        '<select class="form-control  allon-typ" data-id="' + salariesAllown + '"></select>' +
        ' </div>' +
        '<div class="form-group col-md-3">' +
        '<label>Amount</label>' +
        '<input class="form-control Allwnc_Amt" data-id="' + salariesAllown + '">' +
        '</div><i class="ti-minus rm-allonc"></i>' +
        ' </div>' +
        '</div>' +
        '</div>';
    $('#salaries_alln').append(html);
    Initsalariesall(salariesAllown);
});
// add another allownces on employee edit
$(document).on("click", ".add_another_allownce_emp_edit", function () {
    salariesAllown++;
    var html = '<div class="col-md-10 S_A" id="allownce-' + salariesAllown + '">' +
        '<div class="row">' +
        '<div class="form-group col-md-3">' +
        '<label>Allownce Type</label>' +
        '<select class="form-control  allon-typ" data-id="' + salariesAllown + '"></select>' +
        ' </div>' +
        '<div class="form-group col-md-3">' +
        '<label>Amount</label>' +
        '<input class="form-control Allwnc_Amt" data-id="' + salariesAllown + '">' +
        '</div><i class="ti-minus rm-allonc">' +
        '</i></div><i class="ti-plus sav-Allownces"></i>' +
        ' </div>' +
        '</div>' +
        '</div>';
    $('#salaries_alln').append(html);
    Initsalariesall(salariesAllown);
});

$(document).on("click", "#ser-sal-date", function () {
    var month = $(".sel-date").val();
    $("#res").load('/Salaries/SearchSalaries/', { From: month, To: month, Status: "All", DepartmentId: 0 });
});

$(document).on("click", "#ser-sal-date-stat", function () {
    var month = $(".sel-date").val();
    $("#res").load('/Salaries/SearchSalariesStatus/', { From: month, To: month, Status: "Received", DepartmentId: 0 });
});

$(document).on("click", ".rm-allonc", function () {
    $(this).parent().parent().remove();
    salariesAllown = 1;
    $(".S_A").each(function () {
        paycont++;
        $(this).prop('id', 'allownce-' + salariesAllown);
    });
});

$(document).on("keyup", ".sala-gen-txtbox", function () {
    var d_amt = $(this).closest('tr').find('.Dedu_amt').val() || 0;
    var tx_cal = $(this).closest('tr').find('.tax_cal').val() || 0;
    var any_oth = $(this).closest('tr').find('.any_othr_amt').val() || 0;
    var bval = $(this).parent().siblings('.s-read').text() || 0;
    var tot_all = $(this).parent().siblings('.t_allownce').text() || 0;
    var cal = parseFloat(bval) + parseFloat(tot_all);
    var perc_cal_tax = ((parseFloat(bval) / 100) * tx_cal);
    $(this).parent().siblings('.tx_cal_amt').text(perc_cal_tax);
    var total_calul = (parseFloat(cal) - parseFloat(d_amt) - parseFloat(perc_cal_tax) + parseFloat(any_oth));
    $(this).parent().siblings('.GrndTot').text(total_calul);
});
//
$(document).on("change", ".sala-gen-txtbox", function () {
    var id = $(this).closest('tr').attr("id");
    var de_amt = $(this).closest('tr').find('.Dedu_amt').val() || 0;
    var txx_cal = $(this).closest('tr').find('.tax_cal').val() || 0;
    var anyt_oth = $(this).closest('tr').find('.any_othr_amt').val() || 0;
    var deduc_remrk = $(this).closest('tr').find('.deduc_txt').val();
    var resn_remrk = $(this).closest('tr').find('.R_any_othr_amt').val();
    var d_amt = $(this).closest('tr').find('.Dedu_amt').val() || 0;
    var tx_cal = $(this).closest('tr').find('.tax_cal').val() || 0;
    var any_oth = $(this).closest('tr').find('.any_othr_amt').val() || 0;
    var bval = $(this).parent().siblings('.s-read').text() || 0;
    var tot_all = $(this).parent().siblings('.t_allownce').text() || 0;
    var cal = parseFloat(bval) + parseFloat(tot_all);
    var perc_cal_tax = ((parseFloat(bval) / 100) * tx_cal);
    var total_calul = (parseFloat(cal) - parseFloat(d_amt) - parseFloat(perc_cal_tax) + parseFloat(any_oth));
    $.post("/Salaries/UpdateSalariesRec/", { Id: id, deduc_amt: de_amt, tax_alcu: txx_cal, remaarks: deduc_remrk, grnd_tot: total_calul, amottx: perc_cal_tax, anyoth: any_oth, R_cut: resn_remrk }, function () {
    });
});
//
$(document).on("click", ".gen_salaris", function () {
    var id = $(this).attr("id");
    var bank = $(this).attr("data-bank");
    var con = confirm("Are you Sure you want to Generate Salary Slip")
    if (con) {
        //$.post("", function () {
        window.open("/Salaries/PaySlipView?SalariesId=" + id, "_blank");
        window.location.reload();
        //});
    }
})
// row locker
$(document).on("click", ".row_locker", function () {
    $(this).closest('tr').find('id').prop("disabled", true);
})
//
$(document).on("click", ".Ad-emp-lv", function () {
    var con = confirm("Are you sure you want to Import Leaves");
    if (con) {
        $.ajax({
            type: 'POST',
            url: '/Salaries/ImportLeave/',
            date: {},
            success: function () {
                alert('Leave Successfull Added');
                window.location.reload();
            },
            error: function () {
                alert('Error Occured');
            }
        });
    }
});
//
$(document).on("click", ".gen-sal", function () {
    var con = confirm("Are you sure you want to Generate This Month Salaries");
    if (con) {
        $.ajax({
            type: 'POST',
            url: '/Salaries/GenerateiontableSalariesSlip/',
            date: {},
            success: function () {
                alert('Successfull');
                window.location.reload();
            },
            error: function () {
                alert('Error Occured');
            }
        });
    }
});
///  Generate salaris by month
$(document).on("click", ".gen_sal_by_month", function () {
    var selecteddate = new Date();
    var d = new Date(selecteddate);
    var mm = d.getMonth() + 1;
    var yy = d.getFullYear();
    var valu = mm + '_' + yy;
    $.ajax({
        type: "POST",
        url: "/Salaries/Generatebymont/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ date: valu, actualdate: selecteddate }),
        success: function (data) {
            $("#salariesappend").load('/Salaries/GenerateiontableSalariesSlip/', { uniquedate: valu });
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
// approvel leave
$(document).on("change", "#pick", function () {
    var id = $('.apprvl_id').val();
    var manager_R = $('textarea#mytextarea').val();
    var v = $("#pick option:selected").val(); /// yahan kam karna ha
    $.ajax({
        type: "POST",
        url: "/Leave/ManagerApproved/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id, remarks: manager_R, val: v }),
        success: function (data) {
            // approved leaves show
            alert("Approved")
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
//
$(document).on("change", "#pickhr", function () {
    var id = $('.apprvl_id').val();
    var manager_R = $('textarea#mytextarea').val();
    var v = $("#pickhr option:selected").val(); /// yahan kam karna ha
    $.ajax({
        type: "POST",
        url: "/Leave/ManagerApproved/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id, remarks: manager_R, val: v }),
        success: function (data) {
            // approved leaves show
            alert("Approved")
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
//approved leaves show
$(document).on("click", ".app_leaves_show", function () {
    $.ajax({
        type: "POST",
        url: "/Leave/ManagerApprovedList/",
        contentType: "application/json; charset=utf-8",
        data: {},
        success: function (data) {
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
//
$(document).on("click", ".pop_show_pwnding_lvs", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Leave");
    $('#modalbody').load('/Leave/AllPendingLeaves/', { Id: id });
});
//
$(document).on("click", "#lv-appro", function () {
    var id = $('.apprvl_id').val();
    var mang_r = $("#mang-remarks").val();
    var mang_stat = $("#manager-stat").val();
    //var hr_r = $("#hr-remarks").val();
    //var hr_stat = $("#hr-stat").val();
    var frm = $("#Manager").val();
    var lvtype = $("#lv_type").val();
    if (mang_stat == "" || mang_stat == undefined) {
        alert("Please Select Approval");
        return false;
    }
    if (confirm("Are you sure you want to apply leave")) {
        $.ajax({
            type: "POST",
            url: "/Leave/UpdateStatus/",
            data: { Id: id, Manger_Remarks: mang_r, Manger_Status: mang_stat,/* HR_Remarks: hr_r, HR_Status: hr_stat,*/ Manager: frm, lvtype: lvtype },
            success: function (data) {
                if (data) {
                    alert("Approved");
                    closeModal();
                }
                else {
                    alert("Rejected");
                    closeModal();
                }
                $("#leaves_table_user").load("/Leave/AllLeaves/");
            },
            error: function (xmlhttprequest, textstatus, message) {
            }
        });
    }
});
//
$(document).on("click", ".comp-assets_details", function () {
    var name = $(this).closest('td').attr("id");
    EmptyModel();
    $('#ModalLabel').text("Details");
    $('#modalbody').load('/Company/AssetDetailsList/', { AssetName: name });
});
//
//
$(document).on('click', '#returning_asset', function () {
    var assetid = $(this).closest('tr').attr('id');
    $.ajax({
        type: "POST",
        url: '/Company/DeleteAsset/',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Id: assetid,
        }),
        success: function (data) {
            if (!data) {
                alert("Error occur")
            }
            else {
                alert("Retured")
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});
// user level show
function Userpenlaves(url) {
    $("#leaves_table_user").load(url);
}
// Speed Fest
$(document).on("submit", "#spefest", function (e) {
    e.preventDefault();
    if (!closegate) {
        closegate = true;
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            data: $(this).serialize(),
            success: function (data) {
                $("#Name").val("");
                $("#Mobile").val("");
                $("#Address").val("");
                $("#TicketNo").val("");
                alert("Created");
                closegate = false;
                $("#Name").focus();
                $("#sp-leads").load("/Leads/UserSpeedFestLeads_Short/");
            },
            error: function () {
                closegate = false;
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".sp-asi", function (e) {
    e.preventDefault();
    var res = []
    $('.sp-f-leds:checked').each(function () {
        res.push(this.value);
    });
    var id = $("#users").val();
    $.ajax({
        type: "POST",
        url: "/Leads/AssignindSpeedFestLeads/",
        data: { Ids: res, Userid: id },
        success: function (data) {
            alert("Assigned");
            window.location.reload();
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".rece-dispute", function () {
    var id = $(this).data("id");
    EmptyModel();
    $('#ModalLabel').text("Disputed Payment");
    $('#modalbody').load('/Installments/PaymentIssue/', { Id: id });
});
// ******************** Arrears
//
$(document).on("change", ".arr-salaries", function () {
    var id = $(this).closest('tr').attr('id');
    var row = $(this).closest('tr').html();
    if (this.checked) {
        $('#arrears-salary tbody').append('<tr id=' + id + '>' + row + '</tr>');
        $('#arrears-salary tbody tr#' + id + '').find('.rev-chk').remove();
    } else {
        $('#arrears-salary tbody tr#' + id + '').remove();
    }
});
//
$(document).on("click", ".rmv-al-arr", function () {
    var id = $(this).attr("id");
    var salId = $('.arr-sal-id').val();
    $.ajax({
        type: "POST",
        url: '/ArrearsSalaries/RemoveAllowncesAndBonus/',
        data: { Id: id, SalaryId: salId },
        success: function (data) {
            $("#" + salId + " td.grnd_sum").html(Number(data.GrandSum).toLocaleString());
            $("#" + salId + " td.allownces").html(Number(data.ABSResult).toLocaleString());
            var Totall = 0;
            $('.' + Groupid + '-1').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-11').text(Number(Totall).toLocaleString()); // group grand total
            Totall = 0;
            $('.' + Groupid + '-allownces').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-all').text(Number(Totall).toLocaleString()); // group allownces sum
            ArrearAllowncesAndDeductions(); // for footer
            ArrearNetPayable(); // for footer
            $("#arr-al-ad-bo").load('/ArrearsSalaries/GetAllownceAndBonus/', { Id: salId });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
////
$(document).on("click", "#sv-al-bn-arr", function () {
    var description = $('.detail_descrip').val();
    var amt = $('.sala_amt').val();
    var type = "AllowncesAndBonus";
    var sal_id = $('.arr-sal-id').val();
    $.ajax({
        type: "POST",
        url: '/ArrearsSalaries/AddAllowncesAndBonus/',
        data: { Description: description, Amount: amt, Type: type, Salaries_id: sal_id },
        success: function (data) {
            $("#" + sal_id + " td.grnd_sum").html(Number(data.GrandSum).toLocaleString());
            $("#" + sal_id + " td.allownces").html(Number(data.ABSResult).toLocaleString());
            var Totall = 0;
            $('.' + Groupid + '-1').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-11').text(Number(Totall).toLocaleString()); // group grand total 
            Totall = 0;
            $('.' + Groupid + '-allownces').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-all').text(Number(Totall).toLocaleString()); // group allownces sum
            ArrearAllowncesAndDeductions(); // for footer
            ArrearNetPayable(); // for footer
            $("#arr-al-ad-bo").load('/ArrearsSalaries/GetAllownceAndBonus/', { Id: sal_id });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
//
$(document).on("click", "#sv-de-an-tx-arr", function () {
    var description = $('#detail_descrip').val();
    var amt = $('#sala_amt').val();
    var type = "DeductionAndTaxes";
    var sal_id = $('.arr-sal-id').val();
    $.ajax({
        type: "POST",
        url: '/ArrearsSalaries/AddDeductionAndTaxes/',
        data: { Description: description, Amount: amt, Type: type, Salaries_id: sal_id },
        success: function (data) {
            $("#" + sal_id + " td.grnd_sum").html(Number(data.GrandSum).toLocaleString());
            $("#" + sal_id + " td.deduction").html(Number(data.DTResult).toLocaleString());
            var Totall = 0;
            $('.' + Groupid + '-1').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-11').text(Number(Totall).toLocaleString()); // group grand total 
            Totall = 0;
            $('.' + Groupid + '-deduction').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-ded').text(Number(Totall).toLocaleString()); // group deductions sum
            ArrearAllowncesAndDeductions(); // for footer
            ArrearNetPayable(); // for footer
            $("#arr-de-ad-tx").load('/ArrearsSalaries/GetDeductionAndTaxes/', { Id: sal_id });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".rmv-ded-taxes-arr", function () {
    var id = $(this).attr("id");
    var salId = $('.arr-sal-id').val();
    $.ajax({
        type: "POST",
        url: '/ArrearsSalaries/RemoveDeductionAndTaxes/',
        data: { Id: id, SalaryId: salId },
        success: function (data) {
            $("#" + salId + " td.grnd_sum").html(Number(data.GrandSum).toLocaleString());
            $("#" + salId + " td.deduction").html(Number(data.DTResult).toLocaleString());
            var Totall = 0;
            $('.' + Groupid + '-1').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-11').text(Number(Totall).toLocaleString()); // group grand total 
            Totall = 0;
            $('.' + Groupid + '-deduction').each(function () {
                var ded = $(this).text();
                if (ded != "") {
                    var coma = RemoveComma(ded);
                    Totall = (Totall) + parseInt(coma);
                }
            });
            $('.' + Groupid + '-ded').text(Number(Totall).toLocaleString()); // group deductions sum
            ArrearAllowncesAndDeductions(); // for footer
            ArrearNetPayable(); // for footer
            $("#arr-de-ad-tx").load('/ArrearsSalaries/GetDeductionAndTaxes/', { Id: salId });
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".finalize-arrears-salary", function () {
    var Empids = [];
    $("#arrears-salary tr").each(function () {
        Empids.push($(this).attr('id'));
    });
    $.ajax({
        type: "POST",
        url: '/ArrearsSalaries/GenerateiontableSalariesSlip/',
        data: { EmpIds: Empids },
        success: function (data) {
            if (data != null || data == true) {
                $.each(data, function (i, item) {
                    alert(data[i] + ' Already Exists in Arrear');
                });
                //window.open('/ArrearsSalaries/AllSalaries/');
                window.location.reload();
            }
        }
        , error: function () {
        }
    });
});
//
$(document).on("click", ".arr-edt", function () {
    var id = $(this).attr('id');
    Groupid = $(this).closest('tr').find('.arr-grn').attr('id');
    $('#ModalLabel').text("Details");
    $('#modalbody').load('/ArrearsSalaries/SalariesDetail/', { Id: id });
});
// arrear Amount
$(document).on("keyup", ".ArrearsAmount", function () {
    var id = $(this).closest('tr').attr("id");
    var arr_amt = $(this).closest('tr').find('.emp-arrear').val();
    var Groupid = $(this).closest('tr').find('.arr-grn').attr('id');
    $.post("/ArrearsSalaries/ArrearsAmount/", { Id: id, Amount: arr_amt }, function (data) {
        $("#" + id + " .arr-grn").text(Number(data).toLocaleString());
        var Totall = 0;
        $('.' + Groupid + '-1').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-11').text(Number(Totall).toLocaleString());
        ArrearNetPayable();
    });
});
function ArrearNetPayable() {
    var Total = 0;
    $('.arr-grn').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $('.ArNetPaySum').text(Number(Total).toLocaleString());
}
function ArrearAllowncesAndDeductions() {
    var Total = 0;
    $('.arr-ded-1').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .arr-ded').text(Number(Total).toLocaleString());
    Total = 0;
    $('.arr-all-1').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .arr-all').text(Number(Total).toLocaleString());
}
//
function ArrearsdesAndAmount() {
    var statuss = true;
    var arrsta = $('.Arrears_Salary_Status').val();
    if (arrsta == "InProcess") {
        $('.fin-up-arre').each(function () {
            var id = $(this).closest('tr').attr("id");
            var remarks = $(this).closest('tr').find('.salary_remarks_Arrears').val();
            var amt = $(this).closest('tr').find('.ArrearsAmount').val();
            if (remarks == "" || remarks == null || amt == null || amt == "" || amt == "0.0" || amt == "0" || amt == "0.00") {
                $('#salary-dat tbody tr#' + id).addClass("testbg");
                statuss = false;
                return false
            }
        });
    }
    return statuss;
}
//
$(document).on('change', '.salary_remarks_Arrears', function () {
    var id = $(this).closest('tr').attr("id");
    var remarks = $(this).closest('tr').find('.salary_remarks_Arrears').val();
    $.post("/ArrearsSalaries/UpdateDescription/", { Id: id, Remarks: remarks }, function () {
    });
})
$(document).on('click', '.Arrears_Salary_Status', function () {
    var val = ArrearsdesAndAmount();
    if (!val) {
        return false;
    }
    var x = confirm("Are you sure you want to Forward?");
    var val = $(this).val();
    if (x) {
        $.ajax({
            type: "POST",
            url: '/ArrearsSalaries/Status/',
            data: { status: val },
            success: function (data) {
                if (val == "Approved") {
                    window.open("/ArrearsSalaries/FinanceSalarySlipPrint/");
                    alert('Successfully Forward');
                    window.location.reload();
                }
                else {
                    alert('Successfully Forward');
                    window.location.reload();
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
    else {
        return false;
    }
});
//
$(document).on("click", ".fin-up-arre", function () {
    var id = $(this).attr('id');
    $('#salary-dat tbody tr#' + id).removeClass("testbg");
});
//
$(document).on("click", ".gen-arr-sal", function () {
    var id = $(".emp-arr-selc").val();
    $("#emp-arrears-salary").load('/ArrearsSalaries/AllSalaries/', { Id: id });
});
//
$(document).on("click", ".reverse_arrears_salary", function () {
    var id = $(this).closest('tr').attr('id');
    var status = $(this).attr('id');
    $.post('/ArrearsSalaries/ReverseStatus/', { Id: id, Status: status }, function () {
        $('#salary-dat tbody tr#' + id + '').remove();
        BasicSalary();
    });
});
//
$(document).on("click", "#btnExportsalaryArrears", function () {
    window.open("/ArrearsSalaries/FinanceSalarySlipPrint/");
});
//
$(document).on("click", ".sal-rep", function () {
    window.open("/Salaries/DepartmentReport/", "_blank");
    window.open("/Salaries/CompaniesReport/", "_blank");
    window.open("/Salaries/Stipend_DepartmentReport/", "_blank");
    window.open("/Salaries/Stipend_CompaniesReport/", "_blank");
});
//
$(document).on("click", ".gen_salaris_arrears", function () {
    var id = $(this).closest('tr').attr("id");
    var con = confirm("Are you sure Want to generate Arrear Slip");
    if (con) {
        window.open("/ArrearsSalaries/PayArrearsView?SalariesId=" + id, "_blank");
        window.location.reload();
    }
})
//***************** ASSET MANAGEMENT
$(document).on("click", ".comp-assets_pop_up", function () {
    var name = $(this).closest('td').attr("id");
    EmptyModel();
    $('#ModalLabel').text("Assignement");
    $('#modalbody').load('/Company/RemaningAssets/', { AssetName: name });
});
//
$(document).on("click", ".comp-assets_details", function () {
    var name = $(this).closest('td').attr("id");
    EmptyModel();
    $('#ModalLabel').text("Details");
    $('#modalbody').load('/Company/AssetDetailsList/', { AssetName: name });
});
//**************** FUEL MANAGEMENT
//
$(document).on("click", "#btnextrafuel", function () {
    $.post("/Salaries/ExtraFuelDeductions/", function (data) {
        alert('Extra fuel charges added');
    });
});
//
$(document).on("click", ".ad-fuel", function () {
    EmptyModel();
    $('#ModalLabel').text("Fuel Rate");
    $('#modalbody').load('/Company/AddFuel/');
});
//
$(document).on("click", "#ad-fuel-rate", function () {
    var Fuel_Type = $(' #Fuel_Type option:selected').val();
    var Fuel_Rate = $('.Fuel_Rate').val();
    var dataasset = {
        Fuel_Type: Fuel_Type,
        Fuel_Rate: Fuel_Rate,
    }
    $.ajax({
        type: "POST",
        url: '/Company/AddFuelRates/',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            fuel: dataasset,
        }),
        success: function (data) {
            if (!data) {
                alert("Error occur")
            }
            else {
                alert("Successfully Register");
                $('#ad-fuel-rate').prop('disabled', true);
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});
//
$(document).on("click", ".up-fuel", function () {
    EmptyModel();
    $('#ModalLabel').text("Update Rate");
    $('#modalbody').load('/Company/UpdateFuel/');
});
//
$(document).on("click", ".upd-fuel-rate", function () {
    var Id = $(this).attr('id');
    var Fuel_Rate = $(this).closest('.ful-up').find('.Fuel_Rate').val();
    var Date = $(this).closest('.ful-up').find('.Date').val();
    var dataasset = {
        Fuel_Rate: Fuel_Rate,
        Date: Date,
        Id: Id,
    }
    $.ajax({
        type: "POST",
        url: '/Company/UpdateFuelRates/',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            fuel: dataasset,
        }),
        success: function (data) {
            if (!data) {
                alert("Error occur")
            }
            else {
                alert("Updated");
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});
//
$(document).on("change", ".fuel-consmptn", function () {
    var id = $(this).closest('tr').attr("id");
    var cons_val = $(this).val();
    var rate = $(this).closest('tr').find('.vfr').attr('id');
    var Allow_fuel = $(this).closest('tr').find('.allow_fuel').attr('id');
    var dataasset = {
        Vehicle_Id: id,
        Vehicle_Fuel_Allow: Allow_fuel,
        Vehicle_Fuel_Rate: rate,
        Vehicle_Consumption: cons_val,
    }
    $.post("/Company/UpdateFuelConsum/", { details: dataasset },
        function () {
        });
})
//
$(document).on("keyup", ".con-rate", function () {
    var conval = $(this).closest('tr').find('.fuel-consmptn').val();
    var fuel_allow = $(this).closest('tr').find('.allow_fuel').attr('id');
    var rate = $(this).closest('tr').find('.vfr').attr('id');
    var calc = ((conval - fuel_allow) * rate)
    //$("#label1").text($(this).val());
    $(this).closest('tr').find('#tot-rate').text(Number(calc).toLocaleString());
})
//
$(document).on("keyup", ".extra_con", function () {
    var conval = $(this).closest('tr').find('.fuel-consmptn').val();
    var fuel_allow = $(this).closest('tr').find('.allow_fuel').attr('id');
    var calc = ((conval - fuel_allow) * (+1))
    $(this).closest('tr').find('#tot-con').text(Number(calc).toLocaleString());
})
//********END
$(document).on("click", "#Asset_Assining_btn", function () {
    var id = $(this).closest('.asset_id').attr("id");
    var assetval = $("input[name='AssetSelection']:checked").val();
    var Emp_name_id = $('.Emp_id_selection').val();
    var dataasset = {
        Asset_Id: assetval,
        Employee_Id: Emp_name_id,
    }
    $.ajax({
        type: "POST",
        url: '/Company/AssetAssinToEmployee/',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Emp_Asset: dataasset,
        }),
        success: function (data) {
            if (!data) {
                alert("Error occur")
            }
            else {
                alert("Successfully Assigned")
                window.location.reload(true);
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});
//
$(document).on('click', '#returning_asset', function () {
    var assetid = $(this).closest('tr').attr('id');
    $.ajax({
        type: "POST",
        url: '/Company/DeleteAsset/',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Id: assetid,
        }),
        success: function (data) {
            if (!data) {
                alert("Error occur")
            }
            else {
                alert("Retured")
                window.location.reload(true);
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});

$(document).on('change', '.emp-det-raw-tim-adj', function () {
    var id = $(this).val();
    $("#emp-tim-adj").load('/Attendance/OtherEmployeeTimeAdj/', { Emp_Code: id }, function () {
    });
});

function loanrequsition(url) {
    SASLoad($("#loan_table_append"));
    $("#loan_table_append").load(url, function () {
        SASUnLoad($("#loan_table_append"));
    });
}
$(document).on('change', '.emp-det-raw', function () {
    var id = $(this).val();
    SASLoad($(".oth-loan"));
    $(".oth-loan").load('/Loan/Create/', { EmpId: id }, function () {
        SASUnLoad($(".oth-loan"));
    });
});
//
//
$(document).on("click", "#loan-generation", function () {
    var opt = $('#loan-advance-app option:selected').val();
    var empid = $("#emp_loan_id").val();
    if (opt == "choose") {
        alert('select a value');
        return false;
    }
    var Basic_salary = Math.trunc(($('.basic_sal').val() * 3));
    var v = $("#Loan_Installment").val();
    var L_Rsn = $('#loanreason').val();
    if (opt == "Loan") {
        var inst = $('#inst-num').val();
        //if (v > Basic_salary) {
        //    alert('Enter Amount less then ot equal to basic salary');
        //    return false;
        //}
        //else if (v == "" || v == null) {
        //    alert('Enter Amount');
        //    return false;
        //} else
        if (L_Rsn == "") {
            alert('Enter reason');
            return false;
        }
        else if (inst == "" || inst == null) {
            alert('Enter Installments');
            return false;
        }
    }
    else {
        inst = 1;
        if (L_Rsn == "") {
            alert('Enter reason');
            return false;
        }
    }
    $("#loan-inst").load('/Loan/LoanInstallmentsGeneration/', { Installments: inst, Amt: v, Reason: L_Rsn, EmpId: empid });
});
//
$(document).on('click', '.app-for-loan', function () {
    var inst = $("#Loan_Installment").val();
    var opt = $('#loan-advance-app option:selected').val();
    if (inst == "") {
        inst = Math.trunc(($('.basic_sal').val()));
    }
    var Basic_salary = $('.basic_sal').val();
    var emp_id = $('#emp_loan_id').val();
    var L_Rsn = $('#loanreason').val();
    var InstVal = $('#inst-num').val();
    if (InstVal == "") {
        InstVal = 1;
    }
    $.ajax({
        type: "POST",
        url: '/Loan/LoanCreation/',
        data: { EmpId: emp_id, Reason: L_Rsn, InstVal: InstVal, Basic_salary: Basic_salary, inst: inst, Option: opt },
        success: function (data) {
            if (data) {
                alert("Applied")
                window.location.reload();
            }
            else {
                alert("Already Applied")
            }
        },
        error: function (data) {
            alert("error")
        }
    });
});
//
$(document).on("click", ".pop_show_pending_loan", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Loan");
    $('#modalbody').load('/Loan/ManagerPendingLoanDetails/', { Id: id });
});
//
$(document).on('click', '.doc-prnt-btn', function () {
    let lnId = $(this).attr('data-lntag');
    window.open('/Loan/GetLoanPhysicalDoc?loanId=' + lnId);
});
//
$(document).on("click", ".pop_show_pending_loan_hr", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Loan");
    $('#modalbody').load('/Loan/HRPendingLoanDetails/', { Id: id });
});
//
$(document).on("click", "#up-loan-stat-btn", function () {
    var id = $('.loan_apprvl_id').val();
    var mang_r = $("#mang-remarks").val();
    var mang_stat = $("#manager-stat").val();
    var hr_r = $("#hr-remarks").val();
    var hr_stat = $("#hr-stat").val();
    var frm = $("#Manager").val();
    var chk = false;
    if ($('#hrmancheck').is(":checked")) {
        chk = true;
    }
    $.ajax({
        type: "POST",
        url: "/Loan/UpdateStatus/",
        data: { Id: id, Manger_Remarks: mang_r, Manger_Status: mang_stat, HR_Remarks: hr_r, HR_Status: hr_stat, Manager: frm, Check: chk },
        success: function (data) {
            window.location.reload();
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
//
$(document).on("change", "#loan-pick-hr", function () {
    var id = $('.loan_apprvl_id').val();
    var manager_R = $('textarea#mytextarea').val();
    var v = $("#loan-pick-hr option:selected").val(); /// yahan kam karna ha
    $.ajax({
        type: "POST",
        url: "/Loan/Approve/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id, remarks: manager_R, val: v }),
        success: function (data) {
            // approved leaves show
            window.location.reload();
        },
        error: function (xmlhttprequest, textstatus, message) {
        }
    });
});
// voucher popup
$(document).on("click", ".g-l-vouc", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Loan Voucher");
    $('#modalbody').load('/Loan/LoanVoucherDetails/', { Id: id });
});
//
$(document).on("click", "#gen-l-vou", function () {
    var id = $('#loanId').val();
    var con = confirm("Are you sure you want to generate Voucher");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Loan/LoanAdvanceVoucher/',
            data: { Id: id },
            success: function (data) {
                if (data.VoucherId == false) {
                    alert(data.Token);
                }
                else {
                    window.open("/Vouchers/SAGLoanAdvanceVouchers?Id=" + data.VoucherId + "&Token=" + data.Token, '_blank');
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
// voucher popup
$(document).on("click", ".l-a-det", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Loan Details");
    $('#modalbody').load('/Loan/LoanDetails/', { Id: id });
});
//************************* SALARY
$(document).on('change', '.salary_remarks', function () {
    var id = $(this).closest('tr').attr("id");
    var remarks = $(this).closest('tr').find('.salary_remarks').val();
    $.post("/Salaries/UpdateSalariesRec/", { Id: id, Remarks: remarks }, function () {
    });
})
//
$(document).on("keyup", ".absentee", function () {
    var Absen = RemoveComma($(this).val());
    Absen = (isNaN(Absen)) ? 0 : Absen;
    if (Absen < 0) {
        return false;
    }
    var id = $(this).closest('tr').attr("id");
    //var leaves = $(this).closest('tr').find('.salary_leave').val();
    //if (leaves > Absen) {
    //    alert('You Cannot Enter Absentee Greater then Working days');
    //    return false;
    //}
    //var wr__days = $(this).closest('tr').find('.no__of__days').attr('id');
    //if (Absen > wr__days) {
    //    alert('You Cannot Enter Absentee Greater then Working days');
    //    return false;
    //}
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post("/Salaries/Absentee/", { Id: id, Absente: Absen }, function (data) {
        //$("#" + id + " .absn-cal").text(Number(data.AbsenteeDed - data.Leave_cal).toLocaleString());
        if (data == false) {
            alert('Absentee should not be greater then No of days');
            return false;
        }
        $("#" + id + " .absn-cal").text(Number(data.AbsenteeDed).toLocaleString());
        $("#" + id + " .GrndTot").text(Number(data.GrandTot).toLocaleString());
        $("#" + id + " .absen-cal-days").text(data.Total_Worked_Days);
        Absentee();
        NetPayable();
        var Totall = 0;
        $('.' + Groupid).each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
    });
});
//
$(document).on("keyup", ".ExtraDays", function () {
    var extraday = RemoveComma($(this).val());
    extraday = (isNaN(extraday)) ? 0 : extraday;
    var id = $(this).closest('tr').attr("id");
    if (extraday < 0) {
        return false;
    }
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post("/Salaries/ExtraWorkingDays/", { Id: id, ExtraDays: extraday }, function (data) {
        $("#" + id + " .GrndTot").text(Number(data.GrandTot).toLocaleString());
        $("#" + id + " .absen-cal-days").text(data.Total_Worked_Days);
        $("#" + id + " .extra_work").text(data.WorkedDaysCalculation);
        NetPayable(); // footer net payable calculation
        ExtraWorkingDays(); // footer extra working days calculations
        var Totall = 0;
        $('.' + Groupid + '-Ewd').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1Ewd').text(Number(Totall).toLocaleString()); // extradays calculation department wise
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString()); // department wise netpayable count
    });
});
//
$(document).on("keyup", ".salary_leave", function () {
    var leave = RemoveComma($(this).val()) * 1;
    //leave = (isNaN(leave)) ? 0 : leave;
    //if (leave < 0) {
    //    return false;
    //}
    //var absent = $(this).closest('tr').find('.absentee').val()*1;
    //if (leave > absent) {
    //    return false;
    //}
    var id = $(this).closest('tr').attr("id");
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post("/Salaries/Leave/", { Id: id, Leave: leave }, function (data) {
        if (data == false) {
            alert('Leave should not be  greater then No of days');
            return false;
        }
        $("#" + id + " .GrndTot").text(Number(data.GrandTot).toLocaleString());
        //$("#" + id + " .absn-cal").text(Number(data.Absen - data.Leave).toLocaleString());
        $("#" + id + " .absn-cal").text(Number(data.Absen).toLocaleString());
        $("#" + id + " .absen-cal-days").text(data.Total_Worked_Days);
        NetPayable(); // footer net payable calculation
        Absentee();
        var Totall = 0;
        $('.' + Groupid).each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString()); // department wise netpayable count
    });
});
//
$(document).on("keyup", ".Leaves", function () {
    var id = $(this).closest('tr').attr("id");
    var extraday = RemoveComma($(this).closest('tr').find('.emp-extradays').val());
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post("/Salaries/Leaves/", { Id: id, ExtraDays: extraday }, function (data) {
        $("#" + id + " .GrndTot").text(Number(data.GrandTot).toLocaleString());
        $("#" + id + " .absen-cal-days").text(data.Total_Worked_Days);
        $("#" + id + " .extra_work").text(data.WorkedDaysCalculation);
        NetPayable(); // footer net payable calculation
        ExtraWorkingDays(); // footer extra working days calculations
        var Totall = 0;
        $('.' + Groupid + '-Ewd').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1Ewd').text(Number(Totall).toLocaleString()); // extradays calculation department wise
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString()); // department wise netpayable count
    });
});
//
$(document).on("keyup", ".SAL__Overtime", function () {
    var o__t = RemoveComma($(this).val());
    o__t = (isNaN(o__t)) ? 0 : o__t;
    var id = $(this).closest('tr').attr("id");
    if (o__t < 0) {
        return false;
    }
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post("/Salaries/OverTime/", { Id: id, OverTime: o__t }, function (data) {
        $("#" + id + " .GrndTot").text(Number(data.GrandTot).toLocaleString());
        //$("#" + id + " .absen-cal-days").text(data.Total_Worked_Days);
        //$("#" + id + " .extra_work").text(data.WorkedDaysCalculation);
        NetPayable();
        var Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString()); // department wise netpayable count
    });
});
//
function ExtraWorkingDays() {
    var Total = 0;
    $('.extra_work').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .total_extra_wage').text(Number(Total).toLocaleString());
}
//
function CashBankCalculation() {
    var stat = true;
    $('.fin-up-rec').each(function () {
        var cashValue = RemoveComma($(this).closest('tr').find('.cash').val()) * 1;
        var bankValue = RemoveComma($(this).closest('tr').find('.bank').val()) * 1;
        var grandtot = RemoveComma($(this).closest('tr').find('.GrndTot').text()) * 1;
        if (grandtot != cashValue + bankValue) {
            var id = $(this).closest('tr').attr('id');
            alert('Cash and bank is not equal');
            $('#salary-dat tbody tr#' + id).addClass("testbg");
            $('#salary-dat tbody tr#' + id).closest('tbody').show();
            $('#salary-dat tbody tr#' + id).focus();
            lastElementTop = $('#salary-dat tbody tr#' + id).position().top;
            scrollAmount = lastElementTop - 200;
            $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
            stat = false;
            return false
        }
    });
    return stat;
}
//
function BasicSalary() {
    var Total = 0;
    $('.basic_salary').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .total_basic_salary').text(Number(Total).toLocaleString());
}
//
function Absentee() {
    var Total = 0;
    $('.absn-cal').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .footer_ded_absen').text(Number(Total).toLocaleString());
}
//
function TaxCalculation() {
    var Total = 0;
    $(".Tax-deduc").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .footer_ded_Tax').text(Number(Total).toLocaleString());
}
//
function OtherDeduction() {
    var Total = 0;
    $(".fin-dedc").each(function () {
        var ded = $(this).text();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .footer_othr_ded').text(Number(Total).toLocaleString());
}
//
function CashCalculation() {
    var Total = 0;
    $(".cash").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $('.cash_calcu').text(Number(Total).toLocaleString());
}
//
function BankCalculation() {
    var Total = 0;
    $(".bank").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .bank_calcu').text(Number(Total).toLocaleString());
}
//
$(document).on("change", ".sal-cashbank", function () {
    var id = $(this).closest('tr').attr("id");
    var Cashtype = $(this).closest('tr').find('.cash').attr('id');
    var grandtot = RemoveComma($(this).closest('tr').find('.GrndTot').text()) * 1;
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    var tax = RemoveComma($(this).closest('tr').find('.Tax-deduc').val()) * 1;
    var cashValue = RemoveComma($(this).closest('tr').find('.cash').val()) * 1;
    var bankValue = RemoveComma($(this).closest('tr').find('.bank').val()) * 1;
    var sum = cashValue + bankValue;
    if ((sum * 1) != grandtot) {
        alert('Cash or Bank should be equal to Net Payable');
        return false
    }
    $.post("/Salaries/CashBank/", { Id: id, Type: Cashtype, Cash: cashValue, Bank: bankValue }, function (data) {
        var Totall = 0;
        $('.' + Groupid + '-57').each(function () {
            var ded = $(this).closest('tr').find('.cash').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1540').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-56').each(function () {
            var ded = $(this).closest('tr').find('.bank').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1541').text(Number(Totall).toLocaleString());
        BankCalculation();
        CashCalculation();
    });
});
//
function Bonus() {
    var Total = 0;
    $('.bonus').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .total_bonus').text(Number(Total).toLocaleString());
}
//
function Allownces() {
    var Total = 0;
    $('.all_calc').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .total_allownces').text(Number(Total).toLocaleString());
}
//
$(document).on("click", ".fin-up-rec", function () {
    var id = $(this).attr('id');
    $('#salary-dat tbody tr#' + id).removeClass("testbg");
});
//
function NetPayable() {
    var Total = 0;
    $('.GrndTot').each(function () {
        var ded = $(this).text();
        if (ded != "") {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' .NetPaysum').text(Number(Total).toLocaleString());
}
//
$(document).on("keyup", ".Tax-deduc", function () {
    var id = $(this).closest('tr').attr("id");
    var tax = RemoveComma($(this).val());
    var Groupid = $(this).closest('tr').find('.tax-calc').attr('id');
    $.post("/Salaries/Tax/", { Id: id, Tax: tax }, function (data) {
        if (data.resul == false) {
            //$('.Tax-deduc').val("");
            alert('Tax is greater then NetPayable');
        }
        $("#" + id + " .GrndTot").html(Number(data.GrandTot).toLocaleString());
        NetPayable();
        TaxCalculation();
        var Totall = 0;
        $('.' + Groupid + '-5').each(function () {
            var ded = $(this).closest('tr').find('.Tax-deduc').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-15').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
    });
});
//
$(document).on("click", ".view-salary", function () {
    id = $(this).attr('id');
    window.open("/Salaries/ViewSalary?SalariesId=" + id, "_blank");
});
//
var salary_id = 0;
var Grp_Name = "";
var Groupid = 0;
$(document).on("click", ".Salary_details", function () {
    salary_id = $(this).closest('tr').attr("id");
    Grp_Name = $(this).attr('id');
    Groupid = $(this).closest('tr').find('.all_calc').attr('id');
    EmptyModel();
    $('#ModalLabel').text("Details");
    $('#modalbody').load('/Salaries/SalariesDetails/', { Id: salary_id });
});
//
//
$(document).on("click", "#radiochk", function () {
    var rval = $("input[name='bngrp']:checked").val();
    // 
    if (rval == "alldep") {
        var bonuspercn = RemoveComma($('.bon-per').val());
        var description = $('.bon-des').val();
        if (description == null || description == "") {
            alert('Please Enter Description Of All Department');
            return false;
        }
        if (bonuspercn == null || bonuspercn == "") {
            alert('Please Enter Bonus Percentage Of All Department');
            return false;
        }
        if (bonuspercn > 100) {
            alert('Bonus Percentage Of All Department should not be greater then 100');
            return false;
        }
        var con = confirm("Are You sure you want to Add Bonus On All Employees");
        if (con) {
            $.ajax({
                type: "POST",
                url: '/salaries/AddBonus/',
                data: { BonusPercentage: bonuspercn, Description: description },
                success: function (data) {
                    alert('Bonus Applied On All Employees');
                },
                error: function () {
                    alert("Error Occured");
                }
            });
        }
    }
    else if (rval == "depbonus") {
        var depid = $('.dep-bo-val').val();
        var bonuspercn = RemoveComma($('.bon-per-dep').val());
        var description = $('.bon-dep-des').val();
        if (depid == null || depid == "") {
            alert('Please Select Department');
            return false;
        }
        if (description == null || description == "") {
            alert('Please Enter Description In Departmnet');
            return false;
        }
        if (bonuspercn == null || bonuspercn == "") {
            alert('Please Enter Bonus Percentage In Departmnet');
            return false;
        }
        if (bonuspercn > 100) {
            alert('Bonus Percentage In Departmnet should not be greater then 100');
            return false;
        }
        var con = confirm("Are You sure you want to Add Bonus Of Specified Department");
        if (con) {
            $.ajax({
                type: "POST",
                url: '/salaries/DepartmentBonus/',
                data: { BonusPercentage: bonuspercn, DepartmentId: depid, Description: description },
                success: function (data) {
                    alert('Bonus Applied On Specified Department');
                },
                error: function () {
                    alert("Error Occured");
                }
            });
        }
    }
    else if (rval == "religion") {
        var regn = $("#relign option:selected").val();
        var bonuspercn = RemoveComma($('.bon-per-re').val());
        var description = $('.bon-re-des').val();
        if (regn == null || regn == "") {
            alert('Please Select Religion');
            return false;
        }
        if (description == null || description == "") {
            alert('Please Enter Description Of Religion');
            return false;
        }
        if (bonuspercn == null || bonuspercn == "") {
            alert('Please Enter Bonus Percentage In Religion');
            return false;
        }
        if (bonuspercn > 100) {
            alert('Bonus Percentage In Religion should not greater then 100');
            return false;
        }
        var con = confirm("Are You sure you want to Add Bonus Of Specified Religion");
        if (con) {
            $.ajax({
                type: "POST",
                url: '/salaries/ReligionWise/',
                data: { BonusPercentage: bonuspercn, Religion: regn, Description: description },
                success: function (data) {
                    alert('Bonus Applied On Specified Religion');
                },
                error: function () {
                    alert("Error Occured");
                }
            });
        }
    }
    else if (rval == "cola__on__sal") {
        var bonuspercn = RemoveComma($('.cola__per').val());
        var description = $('.cola__des').val();
        if (description == null || description == "") {
            alert('Please Enter Description of COLA');
            return false;
        }
        if (bonuspercn == null || bonuspercn == "") {
            alert('Please Enter Percentage of COLA');
            return false;
        }
        if (bonuspercn > 100) {
            alert('Percentage of COLA should not be greater then 100%');
            return false;
        }
        var con = confirm("Are You sure you want to apply COLA");
        if (con) {
            $.ajax({
                type: "POST",
                url: '/salaries/COLA/',
                data: { BonusPercentage: bonuspercn, Description: description },
                success: function (data) {
                    if (data == true) {
                        alert('COLA Successfully applied');
                        window.location.reload();
                    }
                    else {
                        alert('Something went wronge');
                    }
                },
                error: function () {
                    alert("Error Occured");
                }
            });
        }
    }
    else {
        alert('Option Not Selected')
    }
});
//
$(document).on("click", ".radiochkIndividual", function () {
    var rval = $("input[name='bngrp']:checked").val();
    var id = $(this).attr('id');
    // 
    if (rval == "cola__on__sal") {
        var bonuspercn = RemoveComma($('.cola__per').val());
        var description = $('.cola__des').val();
        if (description == null || description == "") {
            alert('Please Enter Description of COLA');
            return false;
        }
        if (bonuspercn == null || bonuspercn == "") {
            alert('Please Enter Percentage of COLA');
            return false;
        }
        if (bonuspercn > 100) {
            alert('Percentage of COLA should not be greater then 100%');
            return false;
        }
        var con = confirm("Are You sure you want to apply COLA");
        if (con) {
            $.ajax({
                type: "POST",
                url: '/salaries/IndividualCOLA/',
                data: { Id: id, BonusPercentage: bonuspercn, Description: description },
                success: function (data) {
                    if (data == true) {
                        alert('COLA Successfully applied');
                        window.location.reload();
                    }
                    else {
                        alert('Cost of living allownce will not apply on this salary');
                    }
                },
                error: function () {
                    alert("Error Occured");
                },
                timeout: 5000 // sets timeout to 5 seconds
            });
        }
    }
    else {
        alert('Option Not Selected')
    }
});
//
$(document).on("click", "#btnbonus", function () {
    EmptyModel();
    $('#ModalLabel').text("Bonus");
    $('#modalbody').load('/Salaries/Bonus/');
});
//
$(document).on("click", "#btnbonusIndividual", function () {
    var id = $(this).attr('data-id');
    EmptyModel();
    $('#ModalLabel').text("Bonus");
    $('#modalbody').load('/Salaries/BonusIndividual/', { Id: id });
});
//
$(document).on("click", "#btn-bon", function () {
    var bonuspercn = $('.bon-per').val();
    var description = $('.bon-des').val();
    if (description == null || description == "") {
        alert('Please Enter Description Of All Department');
        return false;
    }
    if (bonuspercn == null || bonuspercn == "") {
        alert('Please Enter Bonus Percentage Of All Department');
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/salaries/AddBonus/',
        data: { BonusPercentage: bonuspercn, Description: description },
        success: function (data) {
            alert('Bonus Applied On All Employees');
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", "#btn-dep-bonus", function () {
    var depid = $('.dep-bo-val').val();
    var bonuspercn = $('.bon-per-dep').val();
    var description = $('.bon-dep-des').val();
    if (depid == null || depid == "") {
        alert('Please Select Department Of Departmnet Wise Bonus');
        return false;
    }
    if (description == null || description == "") {
        alert('Please Enter Description Of Departmnet Wise Bonus');
        return false;
    }
    if (bonuspercn == null || bonuspercn == "") {
        alert('Please Enter Bonus Percentage In Departmnet');
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/salaries/DepartmentBonus/',
        data: { BonusPercentage: bonuspercn, DepartmentId: depid, Description: description },
        success: function (data) {
            alert('Bonus Applied On Specific Department');
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", "#save_deduc_and_Tax", function () {
    var $this = $(this);
    var description = $('#detail_descrip').val();
    var amt = RemoveComma($('#sala_amt').val());
    if (description == null || description == "") {
        alert("Description should not be empty");
        return false;
    }
    if (amt == null || amt == "") {
        alert("Amount should not be empty");
        return false;
    }
    var type = "OtherDeduction";
    var x = confirm('Are you sure you want to add deduction');
    if (x) {
        $.ajax({
            type: "POST",
            url: '/salaries/AddDeductionAndBonus/',
            data: { Description: description, Amount: amt, Type: type, Salaries_id: salary_id, Group: Grp_Name },
            success: function (data) {
                $('#detail_descrip').val("");
                $('#sala_amt').val("");
                $("#" + salary_id + " td.fin-dedc").html(Number(data.Result).toLocaleString());
                $("#" + salary_id + " td.GrndTot").html(Number(data.grntot).toLocaleString());
                $("#" + Grp_Name + "td.deduc_group").html(Number(data.GroupSum).toLocaleString());
                NetPayable();
                var Totall = 0;
                $('.' + Groupid + '-45').each(function () {
                    //$(this).text(dealercounter + '.');
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-450').text(Number(Totall).toLocaleString());
                Totall = 0;
                $('.' + Groupid + '-2').each(function () {
                    //$(this).text(dealercounter + '.');
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
                $("#Deductions_and_tax").load('/Salaries/GetOtherDeductions/', { Id: salary_id });
                OtherDeduction();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".ded-taxes-rmv", function () {
    var id = $(this).attr("id");
    var salId = $('.DeductionAndTaxesSalaries').val();
    var x = confirm('Are you sure you want to remove deductions')
    if (x) {
        $.ajax({
            type: "POST",
            url: '/salaries/RemoveDeductionAndTax/',
            data: { Id: id, salaryid: salId, Group: Grp_Name },
            success: function (data) {
                $("#" + salId + " td.fin-dedc").html(Number(data.Result).toLocaleString());
                $("#" + salId + " td.GrndTot").html(Number(data.grntot).toLocaleString());
                $("#" + Grp_Name + " td.deduc_group").html(Number(data.GroupSum).toLocaleString());
                NetPayable();
                var Totall = 0;
                $('.' + Groupid + '-45').each(function () {
                    //$(this).text(dealercounter + '.');
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-450').text(Number(Totall).toLocaleString());
                Totall = 0;
                $('.' + Groupid + '-2').each(function () {
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
                $("#Deductions_and_tax").load('/Salaries/GetOtherDeductions/', { Id: salId });
                OtherDeduction();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", "#save_allownces_and_bonus", function () {
    var description = $('.detail_descrip').val();
    if (description == null || description == "") {
        alert("Description should not be empty");
        return false;
    }
    var amt = RemoveComma($('.sala_amt').val());
    if (amt == null || amt == "") {
        alert("Amount should not be empty");
        return false;
    }
    var type = "AllowncesAndBonus";
    var x = confirm('Are you sure you want to add Allownces/Arrears');
    if (x) {
        $.ajax({
            type: "POST",
            url: '/salaries/AddDeductionAndBonus/',
            data: { Description: description, Amount: amt, Type: type, Salaries_id: salary_id, Group: Grp_Name },
            success: function (data) {
                $('.detail_descrip').val("");
                $('.sala_amt').val("");
                $("#" + salary_id + " td.t_allownce").text(Number(data.Result).toLocaleString());
                $("#" + salary_id + " td.GrndTot").text(Number(data.grntot).toLocaleString());
                $("#" + Grp_Name + "td.totl_allownce").text(Number(data.GroupSum).toLocaleString());
                NetPayable(); // for footer
                Allownces();// for footer
                var Totall = 0;
                $('.' + Groupid + '-8').each(function () {
                    //$(this).text(dealercounter + '.');
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-9').text(Number(Totall).toLocaleString());
                Totall = 0;
                $('.' + Groupid + '-2').each(function () {
                    //$(this).text(dealercounter + '.');
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
                $("#Allownces_And_bonus").load('/Salaries/GetAllowncesAndBonus/', { Id: salary_id });
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".allownces-bonus-rmv", function () {
    var id = $(this).attr("id");
    var salId = $('.AllowncesAndBonusSalaries').val();
    var x = confirm('Are you sure you want to remove Allownces/Arrears')
    if (x) {
        $.ajax({
            type: "POST",
            url: '/salaries/RemoveAllowncesAndBonus/',
            data: { Id: id, salaryid: salId, Group: Grp_Name },
            success: function (data) {
                $("#" + salId + " td.t_allownce").text(Number(data.Result).toLocaleString());
                $("#" + salId + " td.GrndTot").text(Number(data.grntot).toLocaleString());
                $("#" + Grp_Name + " td.totl_allownce").text(Number(data.GroupSum).toLocaleString());
                NetPayable(); // for footer
                Allownces();// for footer allownces
                var Totall = 0;
                $('.' + Groupid + '-8').each(function () {
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-9').text(Number(Totall).toLocaleString());
                Totall = 0;
                $('.' + Groupid + '-2').each(function () {
                    var ded = $(this).text();
                    if (ded != "") {
                        var coma = RemoveComma(ded);
                        Totall = (Totall) + parseInt(coma);
                    }
                });
                $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
                $("#Allownces_And_bonus").load('/Salaries/GetAllowncesAndBonus/', { Id: salId });
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".finc-fin-sal", function () {
    var val = CashBankCalculation();
    if (!val) {
        return false;
    }
    var flag = true;
    //var banksalary = [];
    //$('.bank_det').each(function () {
    //    //
    //    //var headId = $(this).find('.head_Name').attr('id');
    //    //var bankhead = $(this).find('.head_Name').val();
    //    //if (bankhead == null || bankhead == "") {
    //    //    alert("Select Bank for all Bank Salaried Members");
    //    //    flag = false;
    //    //    return false;
    //    //}
    //    //var instnumber = $(this).find('#instNumber').val();
    //    //if (instnumber == null || instnumber == "") {
    //    //    alert("Please Enter Instrument number for all Bank Salaried Members");
    //    //    flag = false;
    //    //    return false;
    //    //}
    //    //var instDate = $(this).find('#pur-date').val();
    //    //if (instDate == null || instDate == "") {
    //    //    alert("Please Enter Date for all Salaried Members");
    //    //    flag = false;
    //    //    return false;
    //    //}
    //    //var totAmount = $(this).find('.tot_Amount').val();
    //    //if (totAmount == null || totAmount == "") {
    //    //    alert("Please Enter Amount of all Instruments");
    //    //    flag = false;
    //    //    return false;
    //    //}
    //    banksalary.push({
    //        HeadId: headId,
    //        Bankhead: bankhead,
    //        InstrumentNo: instnumber,
    //        InstrumentDate: instDate,
    //        InstrumentAmount: totAmount
    //    });
    //});
    if (flag) {
        var con = confirm("Are You sure you want to Finalize Salries");
        if (con) {
            SASLoad($(".loadlogo"));
            $.ajax({
                type: "POST",
                url: '/Salaries/FinalizeSalries/',
                dataType: "json",
                success: function (data) {
                    SASUnLoad($(".loadlogo"));
                    alert('Successfully done');
                    window.location.reload();
                    //window.open("/Salaries/DepartmentReport/");
                },
                error: function () {
                    SASUnLoad($(".loadlogo"));
                    alert("Error Occured");
                    window.location.reload();
                }
            });
        }
    }
});
//
$(document).on('click', '.Salary_Status1', function () {
    var val = NagitiveCalOfGrandAndAbsentee();
    if (!val) {
        alert('A nagitive amount exist in Absentee Deduction');
        return false;
    }
    var x = confirm("Are you sure you want to Forward?");
    if (x) {
        $.post("/Salaries/UpdateStatus1/", function (data) {
            alert('Successfully Forward');
            window.location.reload();
        });
    }
    else {
        return false;
    }
});
//
$(document).on('click', '.Salary_Status2', function () {
    var val = NagitiveCalOfGrandtotal();
    if (!val) {
        alert('A nagitive amount exist in Grand Total');
        return false;
    }
    var x = confirm("Are you sure you want to Forward?");
    if (x) {
        $.post("/Salaries/UpdateStatus2/", function (data) {
            alert('Successfully Forward');
            window.location.reload();
        });
    }
    else {
        return false;
    }
});
//
function NagitiveCalOfGrandAndAbsentee() {
    var stat = true;
    $('.absn-cal').each(function () {
        var abcal = RemoveComma($(this).closest('tr').find('.absn-cal').text()) * 1;
        if (abcal < 0) {
            var id = $(this).closest('tr').attr('id');
            $('#salary-dat tbody tr#' + id).addClass("testbg");
            $('#salary-dat tbody tr#' + id).focus();
            lastElementTop = $('#salary-dat tbody tr#' + id).position().top;
            scrollAmount = lastElementTop - 200;
            $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
            stat = false;
            return false
        }
    });
    return stat;
}
//
function NagitiveCalOfGrandtotal() {
    var stat = true;
    $('.GrndTot').each(function () {
        var gr = RemoveComma($(this).closest('tr').find('.GrndTot').text()) * 1;
        if (gr < 0) {
            var id = $(this).closest('tr').attr('id');
            $('#salary-dat tbody tr#' + id).addClass("testbg");
            $('#salary-dat tbody tr#' + id).focus();
            lastElementTop = $('#salary-dat tbody tr#' + id).position().top;
            scrollAmount = lastElementTop - 200;
            $('#scrolling-box').animate({ scrollTop: scrollAmount }, 1000);
            stat = false;
            return false
        }
    });
    return stat;
}
//
$(document).on('click', '.Salary_Status', function () {
    var val = $(this).val();
    var x = confirm("Are you sure you want to Forward?");
    if (x) {
        $.ajax({
            type: "POST",
            url: '/Salaries/UpdateStatus/',
            data: { status: val },
            success: function (data) {
                if (val == "Received") {
                    window.location.reload();
                }
                else {
                    alert('Successfully Forward');
                    window.location.reload();
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
    else {
        return false;
    }
})
//
$(document).on("click", ".reverse_salary", function () {
    var id = $(this).attr('data-id');
    var status = $(this).attr('id');
    var Groupid = $(this).closest('tr').find('.absn-cal').attr('id');
    $.post('/salaries/UpdateSalaryStatus/', { Id: id, Status: status }, function () {
        $('#salary-dat tbody tr#' + id + '').remove();
        var Totall = 0;
        $('.' + Groupid).each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-basic_salary').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-basic_salarygr').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-Ewd').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1Ewd').text(Number(Totall).toLocaleString()); // extradays calculation department wise
        Totall = 0;
        $('.' + Groupid + '-bon').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1depBo').text(Number(Totall).toLocaleString()); // bonus calculation department wise
        Totall = 0;
        $('.' + Groupid + '-basicSalary').each(function () {
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1basicsalary').text(Number(Totall).toLocaleString()); // basic salary calculation department wise
        Totall = 0;
        $('.' + Groupid).each(function () { // deparmanr wise 
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1').text(Number(Totall).toLocaleString());// absentee calculation department wise
        Totall = 0;
        $('.' + Groupid + '-2').each(function () {  // department total
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-22').text(Number(Totall).toLocaleString()); // grand total department wise
        Totall = 0;
        $('.' + Groupid + '-5').each(function () {
            var ded = $(this).closest('tr').find('.Tax-deduc').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-15').text(Number(Totall).toLocaleString()); // dedduction tax department wise
        Totall = 0;
        $('.' + Groupid + '-8').each(function () {
            //$(this).text(dealercounter + '.');
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        }); // allowence departmen wise
        $('.' + Groupid + '-9').text(Number(Totall).toLocaleString());// allowence departmen wise
        Totall = 0;
        $('.' + Groupid + '-57').each(function () {
            var ded = $(this).closest('tr').find('.cash').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1540').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-56').each(function () {
            var ded = $(this).closest('tr').find('.bank').val();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-1541').text(Number(Totall).toLocaleString());
        Totall = 0;
        $('.' + Groupid + '-45').each(function () {
            //$(this).text(dealercounter + '.');
            var ded = $(this).text();
            if (ded != "") {
                var coma = RemoveComma(ded);
                Totall = (Totall) + parseInt(coma);
            }
        });
        $('.' + Groupid + '-450').text(Number(Totall).toLocaleString());
        Absentee();
        NetPayable();
        BasicSalary();
        Allownces();// for footer allownces calculations
        TaxCalculation();// tax calculationsw
        ExtraWorkingDays();
        Bonus();
        BasicSalary();
        CashCalculation();
        BankCalculation();
        OtherDeduction();
    });
});
// delete salary 
$(document).on("click", ".del-sal", function () {
    var id = $(this).attr('data-id');
    var res = confirm("Are you sure you want to delete this salary");
    if (res) {
        $.ajax({
            type: "POST",
            url: '/Salaries/Delete/',
            data: { Id: id },
            success: function (data) {
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
// Update Basic Salary
$(document).on("click", ".up-sal", function () {
    var id = $(this).attr('data-id');
    var res = confirm("Are you sure you want to Update this salary");
    if (res) {
        $.ajax({
            type: "POST",
            url: '/Salaries/Update/',
            data: { Id: id },
            success: function (data) {
                if (data == true) {
                    alert('Salaries Successfully Updated');
                    window.location.reload();
                }
                else {
                    $.each(data, function (i, item) {
                        alert(data[i] + ' salary is not generated');
                    });
                    window.location.reload();
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", "#cbp__Ser", function () {
    var mo = $("#m__ser option:selected").val() * 1;
    var yer = $("#ye__ser option:selected").val() * 1;
    $('#cb__app').load('/Salaries/FinanceFinalSlipSearch/', { Month: mo, Year: yer });
});
//
$(document).on("click", "#cbp__arr", function () {
    var mo = $("#m__ser option:selected").val() * 1;
    var yer = $("#ye__ser option:selected").val() * 1;
    $('#cb__app').load('/ArrearsSalaries/FinanceFinalArrSlipSearch/', { Month: mo, Year: yer });
});
//
$(document).on("click", "#srs__Ser", function () {
    var mo = $("#m__ser option:selected").val() * 1;
    var yer = $("#ye__ser option:selected").val() * 1;
    $('#r__s').load('/Salaries/SalariesReportsSearch/', { Month: mo, Year: yer });
});
//
$(document).on("click", "#ser-sal-date", function () {
    var month = $(".sel-date").val();
    $("#res").load('/Salaries/SearchSalaries/', { From: month, To: month, Status: "All", DepartmentId: 0 });
});
//
$(document).on("click", "#ser-sal-date-stat", function () {
    var month = $(".sel-date").val();
    $("#res").load('/Salaries/SearchSalariesStatus/', { From: month, To: month, Status: "Received", DepartmentId: 0 });
});


$(document).on("click", "#sav-ban-acc", function (e) {
    var bank = $('.bank').val();
    var account = $('#acnt').val();
    $.ajax({
        type: "POST",
        url: '/Banking/AddBankAccount/',
        data: { Bank: bank, Account_Number: account },
        success: function (data) {
            if (!data) {
                alert("error Occured")
            }
            else {
                $("#bankaccount").load('/Banking/GetBankAccount/');
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".Bank-rmv", function (e) {
    var id = $(this).attr("id");
    $.ajax({
        type: "POST",
        url: '/Banking/RemoveBankAccount/',
        data: { Id: id },
        success: function (data) {
            if (!data) {
                alert("error Occured")
            }
            else {
                $("#bankaccount").load('/Banking/GetBankAccount/');
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", "#sav-ban-onl-acc", function (e) {
    var bank = $('#Bank option:selected').val();
    var account = $('.accnum option:selected').val();
    $.ajax({
        type: "POST",
        url: '/Banking/AddBankOnlineAccount/',
        data: { Bank: bank, Account_Number: account },
        success: function (data) {
            if (!data) {
                alert("error Occured")
            }
            else {
                $("#bankaccount").load('/Banking/GetBankOnlineAccount/');
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".Bank-on-rmv", function (e) {
    var id = $(this).attr("id");
    $.ajax({
        type: "POST",
        url: '/Banking/RemoveBankAccount/',
        data: { Id: id },
        success: function (data) {
            if (!data) {
                alert("error Occured")
            }
            else {
                $("#bankaccount").load('/Banking/GetBankOnlineAccount/');
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
/// Prjoject head saving
//
$(document).on("click", ".sav-prj-acc", function (e) {
    var prj = $('.prj option:selected').val();
    var head = $('.head option:selected').val();
    $.ajax({
        type: "POST",
        url: '/AccountHeads/AddPrjHead/',
        data: { Head_Code: head, Prj_Id: prj },
        success: function (data) {
            if (data.Status) {
                alert(data.Msg)
                $("#coahead").load('/ConstructProjectManagement/ProjectMapping/');
            }
            else {
                alert(data.Msg)
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", ".prj-head-rmv", function (e) {
    var id = $(this).attr("id");
    $.ajax({
        type: "POST",
        url: '/AccountHeads/DeletePrjHead/',
        data: { Id: id },
        success: function (data) {
            if (data.Status) {
                alert(data.Msg)
                $("#coahead").load('/ConstructProjectManagement/ProjectMapping/');
            }
            else {
                alert(data.Msg)
            }
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
////////////// Inventory Managmenet
//************************************************ Inventory Management
var InventoryCounter = 1;
var InventoryArray = [];
//************ Demand
$(document).on("click", ".reg-demand", function () {
    EmptyModel();
    $('#ModalLabel').text("Demand Order");
    $('#modalbody').load('/Inventory/RegisterDemandOrder/');
});
$(document).on("change", ".proj-selec", function () {
    var name = $(".proj-selec option:selected").text();
    $("#DemadnOrder").load('/Inventory/DemandOrderSearch/', { ProjectName: name });
});
//
$(document).on("change", "#Del-Non-Dlvr", function () {
    var status = $("#Del-Non-Dlvr option:selected").val();
    $("#DemadnOrder").load('/Inventory/DemandOrderSearch/', { Status: status });
});
//
$(document).on("click", "#inv-btn-src", function () {
    var name = $(".proj-selec option:selected").text();
    var status = $("#Del-Non-Dlvr option:selected").val();
    $("#DemadnOrder").load('/Inventory/DemandOrderSearch/', { Status: status, ProjectName: name });
});
//
$(document).on("click", ".date-dmd-src", function () {
    var name = $(".proj-selec option:selected").text();
    var status = $("#Del-Non-Dlvr option:selected").val();
    $("#DemadnOrder").load('/Inventory/DemandOrderSearch/', { Status: status, ProjectName: name });
});
//
function InitProject(i) {
    $("#add-demd-" + i + " .Project").append('<option>Select Project</option>');
    $.each(Project, function (key, value) {
        $("#add-demd-" + i + " .Project").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
$(document).on("click", ".add-row-dmd", function () {
    InventoryCounter++;
    var html = '<div class="form-row cal dmd-id" id="add-demd-' + InventoryCounter + '"><h6 class="lh-1 mB-0 add-dmd-counter">' + InventoryCounter + '.</h6>' +
        //'< div class="form-group col-md-2" ><select class="form-control Project" data-id="' + InventoryCounter + '"></select></div >'+
        '<div class="form-group col-md-2"><input type="text" class="form-control Product" name="Product" data-id="' + InventoryCounter + '" required  placeholder="Product"/></div>' +
        '<div class="form-group col-md-2"><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="" name="Quantity" class="amt Quantity"  data-id="' + InventoryCounter + '" required ></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Measurement" name="Measurement" data-id="' + InventoryCounter + '" placeholder="Measurement" required ></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Required_Date" data-provide="datepicker" placeholder="mm/dd/yyyy" data-id="' + InventoryCounter + '" name=""></div>' +
        '<i class="ti-minus rm-dmd pointer"></i></div>';
    $('#ad-demand').append(html);
    InitProject(InventoryCounter);
});
//
$(document).on("click", ".rm-dmd", function () {
    $(this).parent().remove();
    InventoryCounter = 1;
    $('.add-dmd-counter').each(function () {
        $(this).text(InventoryCounter + '.');
        InventoryCounter++;
    });
    InventoryCounter = 1;
    $('.dmd-id').each(function () {
        InventoryCounter++;
        $(this).attr('id', 'add-demd-' + InventoryCounter);
    });
})
//
$(document).on("click", ".inven-demand-crud", function () {
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Purchase Request Update");
    $('#modalbody').load('/Inventory/DemandByIdSearch/', { Id: id });
})
//
$(document).on("click", ".del-dmd", function (e) {
    var groupid = $(this).attr('id');
    var dataset =
    {
        GroupId: groupid
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/DemandOrderRemove/',
        data: { dmd: dataset },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                alert("Deleted");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//*********Inventory
$(document).on("click", "#invent-popup-po", function () {
    window.open('/Inventory/StockIn');
})
//
function InitSupplier(i) {
    $("#add-inv-" + i + " .vendor").append('<option>Select Supplier</option>');
    $.each(supplier, function (key, value) {
        $("#add-inv-" + i + " .vendor").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
function InitHeads(i) {
    $("#add-inv-" + i + " .acc-head").empty();
    $.each(AccHead, function (key, value) {
        $("#add-inv-" + i + " .acc-head").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
function linerInitHeads(i, a) {
    $("." + a + i + " .acc-head").empty();
    $.each(AccHead, function (key, value) {
        $("." + a + i + " .acc-head").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
function InitHeadsWithVal(i, j) {
    $("#" + i + " .acc-head").empty();
    $.each(AccHead, function (key, value) {
        $("#" + i + " .acc-head").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
    $("#" + i + " .acc-head").val(j);
}
//Vouchers
// Load data in tab container
$(document).on("click", "[data-toggle='tab']", function (e) {
    var $this = $(this),
        targ = $this.attr('data-url'),
        loadurl = $this.attr('href');
    var f = $("#frm-val").val();
    var t = $("#to-val").val();
    var type = $($this).data("type");
    SASLoad($(targ));
    $(targ).load(loadurl, { From: f, To: t, Type: type }, function () {
        $('#myTabs .active').removeClass("active");
        SASUnLoad($(targ));
        $this.addClass("active");
    });
});
//
$(document).on("click", ".add-row-new-inven", function () {
    InventoryCounter++;
    var html = '<div class="form-row cal add-inv-id" id="add-inv-' + InventoryCounter + '" ><h6 class="lh-1 mB-0 add-inven-counter" style="margin-top: 10px;font-size: 15px;">' + InventoryCounter + '.</h6><div class="form-group col-md-3">' +
        '<input type="text" class="form-control Name" name = "Name" data-id="' + InventoryCounter + '" required placeholder="Product" /></div><div class="form-group col-md-1">' +
        '<input class="form-control coma" placeholder="12,345" required><input type = "hidden" id="" name="Quantity" class="amt Quantity inv-rate" data-id="' + InventoryCounter + '" required ></div>' +
        '<div class="form-group col-md-1"><select style="padding:5px" class="form-control Measurement" name="Measurement" data-id="' + InventoryCounter + '"><option value="Kg">Kg</option><option value="Piece">Piece</option><option value="Bag">Bag</option><option value="Liter">Liter</option></select></div>' +
        '<div class="form-group col-md-1"><input type="text" class="form-control Rate inv-rate" name="Rate" data-id="' + InventoryCounter + '" placeholder="Rate" ></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Rate Purchase_Rate" name="Purchase_Rate" data-id="' + InventoryCounter + '" placeholder="Purchase Rate" readonly></div>' +
        '<div class="form-group col-md-3"><input type="text" class="form-control Rate Remarks" name="Remarks" data-id="' + InventoryCounter + '" placeholder="Remarks"></div>' +
        '<i class="ti-minus rm-add-inven pointer" ></i></div>';
    $('#ad-iven-stock').append(html);
    InitSupplier(InventoryCounter);
});
//
$(document).on("click", ".add-ven-pay-de", function () {
    InventoryCounter++;
    var html = '<div class="col-md-12 cal" id="add-inv-' + InventoryCounter + '"><div><hr></div>' +
        '<div class="form-row"><span class="add-inven-counter" style="margin-top: 35px;font-size: 15px;width:35px">' + InventoryCounter + '.</span>' +
        '<div class="form-group col-md-4"><label>Description</label><input type="text" class="form-control Name" name="Name"  required /></div>' +
        '<div class="form-group col-md-1"><label>Quantity</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="" name="Quantity" class="amt Quantity inv-rate rate-calc"  required></div>' +
        '<div class="form-group col-md-1"><label>UOM</label><select style="padding:5px" class="form-control Measurement meas-che" name="Measurement" ><option value="-">N/A</option><option value="Kg">Kg</option><option value="Piece">Piece</option><option value="Bag">Bag</option><option value="Liter">Liter</option></select></div>' +
        '<div class="form-group col-md-1"><label>Rate</label><input type="text" class="form-control Rate inv-rate rate-calc" name="Rate"  placeholder="Rate"></div>' +
        '<div class="form-group col-md-2"><label>Amount</label><input type="text" class="form-control coma p-r-c rate-calc" placeholder="Amount"><input type="hidden" id="" name="Quantity" class="amt Rate Purchase_Rate"  required></div>' +
        '<i class="ti-minus rm-add-inven pointer" ></i></div>' +
        '<div class="form-row"><span style="width:35px"></span><div class="col-md-4"></div><div class="col-md-3"><h6>Head</h6></div><div class="col-md-1"><h6>Debit</h6></div><div class="col-md-1"><h6>Credit</h6></div></div>' +
        '<div class="dr-line"><div class="form-row dr-row"><span style="width:35px"></span><div class="form-group col-md-4"><i class="ti-plus  add-dr-row" style="float:right;margin-top:10px;font-size:12px"></i></div>' +
        '<div class="form-group col-md-3"><select class="form-control acc-head searchbox" ></select></div>' +
        '<div class="form-group col-md-1"><input type="text" class="form-control coma " placeholder="Amount"><input type="hidden" id="" class="amt Rate dr"  required></div></div></div>' +
        '<div class="cr-line"><div class="form-row cr-row"><span style="width:35px"></span>' +
        '<div class="form-group col-md-4"><i class="ti-plus add-cr-row" style="float:right;margin-top:10px;font-size:12px"></i></div>' +
        '<div class="form-group col-md-3"><select class="form-control acc-head searchbox" ></select></div>' +
        '<div class="form-group col-md-1"></div>' +
        '<div class="form-group col-md-1"><input type="text" class="form-control coma " placeholder="Amount"><input type="hidden" id="" class="amt Rate cr"  required></div></div></div>' +
        '</div>';
    //
    $('#ad-iven-stock').append(html);
    $('#add-inv-' + InventoryCounter + ' .searchbox').select2({
        //dropdownParent: $('#Modal'),
        placeholder: "Search for Head",
        ajax: {
            url: '/AccountHeads/GetHead/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term// search term
                };
            },
            processResults: function (data) {
                return {
                    results: data.items
                };
            }
        }
    });
    //InitHeads(InventoryCounter);
});
//
$(document).on("click", ".add-dr-row", function () {
    var numItems = $('.cr-row').length + 1;
    var html = '<div class="form-row  dr-row dr-row-' + numItems + '"><span style="width:35px"></span><div class="form-group col-md-4"><i class="ti-minus rm-dr-row" style="float:right;margin-top:10px"></i></div><div class="form-group col-md-3">' +
        '<select class="form-control acc-head searchbox"></select></div><div class="form-group col-md-1"><input type="text" class="form-control coma " placeholder="Amount">' +
        '<input type="hidden" id="" class="amt Rate dr" data-id="1" required></div></div>';
    var id = $(this).closest(".cal").attr("id");
    $('#' + id + ' .dr-line ').append(html);
    //linerInitHeads(numItems, 'dr-row-');
    $('#' + id + ' .searchbox').select2({
        //dropdownParent: $('#Modal'),
        placeholder: "Search for Head",
        ajax: {
            url: '/AccountHeads/GetHead/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term// search term
                };
            },
            processResults: function (data) {
                return {
                    results: data.items
                };
            }
        }
    });
});
//
$(document).on("click", ".add-cr-row", function () {
    var numItems = $('.cr-row').length + 1;
    var html = '<div class="form-row cr-row cr-row-' + numItems + '"><span style="width:35px"></span><div class="form-group col-md-4"><i class="ti-minus rm-cr-row" style="float:right;margin-top:10px"></i></div><div class="form-group col-md-3">' +
        '<select class="form-control acc-head searchbox"></select></div><div class="form-group col-md-1"></div><div class="form-group col-md-1"><input type="text" class="form-control coma " placeholder="Amount">' +
        '<input type="hidden" id="" class="amt Rate cr" data-id="1" required></div></div>';
    var id = $(this).closest(".cal").attr("id");
    $('#' + id + ' .cr-line ').append(html);
    //linerInitHeads(numItems, 'cr-row-');
    $('#' + id + ' .searchbox').select2({
        //dropdownParent: $('#Modal'),
        placeholder: "Search for Head",
        ajax: {
            url: '/AccountHeads/GetHead/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term// search term
                };
            },
            processResults: function (data) {
                return {
                    results: data.items
                };
            }
        }
    });
});
//
$(document).on("click", ".rec-ent", function (e) {
    var tran = $("#trans-id").val();
    for (var i = 1; i <= InventoryCounter; i++) {
        var dr = 0, cr = 0;
        $("#add-inv-" + i + " .dr").each(function () {
            var dr_amt = RemoveComma($(this).val());
            if (dr_amt > 0) {
                dr = dr + parseInt(dr_amt);
            }
            else {
                alert("You Cannot Submit Empty Fields");
                return false;
            }
        });
        $("#add-inv-" + i + " .cr").each(function () {
            var cr_amt = RemoveComma($(this).val());
            if (cr_amt > 0) {
                cr = cr + parseInt(cr_amt);
            }
            else {
                alert("You Cannot Submit Empty Fields");
                return false;
            }
        });
        var t_amt = RemoveComma($("#add-inv-" + i + " .Purchase_Rate").val());
        if (dr == cr) {
            if (dr == t_amt) {
                continue;
            }
            else {
                alert("Debit/Credit Values are not Equal to Total Amount at : " + $("#add-inv-" + i + " .Name").text());
                return false;
            }
        }
        else {
            alert("Debit & Credit Values are not Matching at :" + $("#add-inv-" + i + " .Name").text());
            return false;
        }
    }
    var des = $("#Description").val();
    var Amt = $("#amt").val();
    InventoryArray = [];
    for (var i = 1; i <= InventoryCounter; i++) {
        var Item_Name = $("#add-inv-" + i + " .Name").val();
        var Rate = $("#add-inv-" + i + " .inv-rate").val();
        var Quantity = $("#add-inv-" + i + " .Quantity").val();
        var UOM = $("#add-inv-" + i + " .Measurement").val();
        var Amount = $("#add-inv-" + i + " .Purchase_Rate").val(); // Amount
        $("#add-inv-" + i + " .dr-row").each(function () {
            var dat = $(this).find(".acc-head").select2('data')
            var dataset =
            {
                Item_Name: Item_Name,
                Rate: Rate,
                Quantity: Quantity,
                UOM: UOM,
                Amount: Amount,
                Account_Head: dat[0].text,
                Account_Head_Id: dat[0].id,
                Debit: $(this).find(".dr").val(),
                Credit: $(this).find(".cr").val(),
                Line: i
            };
            InventoryArray.push(dataset);
        });
        $("#add-inv-" + i + " .cr-row").each(function () {
            var dat = $(this).find(".acc-head").select2('data')
            var dataset =
            {
                Item_Name: Item_Name,
                Rate: Rate,
                Quantity: Quantity,
                UOM: UOM,
                Amount: Amount,
                Account_Head: dat[0].text,
                Account_Head_Id: dat[0].id,
                Debit: $(this).find(".dr").val(),
                Credit: $(this).find(".cr").val(),
                Line: i
            };
            InventoryArray.push(dataset);
        });
    }
    if (confirm("Are you sure you want to Record Entries")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/RecordEntries/',
            data: { Details: InventoryArray, TransactionId: tran },
            success: function (data) {
                if (data) {
                    alert("Journal Entry Recorded");
                    window.location.reload();
                }
                else {
                    alert("Can not make Transaction at the Moment")
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
//
$(document).on("click", ".up-rec-ent", function (e) {
    var tran = $("#trans-id").val();
    InventoryCounter--;
    for (var i = 1; i <= InventoryCounter; i++) {
        var dr = 0, cr = 0;
        $("#add-inv-" + i + " .dr").each(function () {
            var dr_amt = RemoveComma($(this).val());
            if (dr_amt > 0) {
                dr = dr + parseInt(dr_amt);
            }
            else {
                alert("You Cannot Submit Empty Fields");
                return false;
            }
        });
        $("#add-inv-" + i + " .cr").each(function () {
            var cr_amt = $(this).val();
            if (cr_amt > 0) {
                cr = cr + parseInt(cr_amt);
            }
            else {
                alert("You Cannot Submit Empty Fields");
                return false;
            }
        });
        var t_amt = RemoveComma($("#add-inv-" + i + " .Purchase_Rate").val());
        if (dr == cr) {
            if (dr == t_amt) {
                continue;
            }
            else {
                alert("Debit/Credit Values are not Equal to Total Amount at : " + $("#add-inv-" + i + " .Name").text());
                return false;
            }
        }
        else {
            alert("Debit & Credit Values are not Matching at :" + $("#add-inv-" + i + " .Name").text());
            return false;
        }
    }
    var des = $("#Description").val();
    var Amt = $("#amt").val();
    InventoryArray = [];
    for (var i = 1; i <= InventoryCounter; i++) {
        var Item_Name = $("#add-inv-" + i + " .Name").val();
        var Rate = $("#add-inv-" + i + " .inv-rate").val();
        var Quantity = $("#add-inv-" + i + " .Quantity").val();
        var UOM = $("#add-inv-" + i + " .Measurement").val();
        var Amount = $("#add-inv-" + i + " .Purchase_Rate").val(); // Amount
        $("#add-inv-" + i + " .dr-row").each(function () {
            var dat = $(this).find(".acc-head").select2('data')
            var dataset =
            {
                Item_Name: Item_Name,
                Rate: Rate,
                Quantity: Quantity,
                UOM: UOM,
                Amount: Amount,
                Account_Head: dat[0].text,
                Account_Head_Id: dat[0].id,
                Debit: $(this).find(".dr").val(),
                Credit: $(this).find(".cr").val(),
                Line: i
            };
            InventoryArray.push(dataset);
        });
        $("#add-inv-" + i + " .cr-row").each(function () {
            var dat = $(this).find(".acc-head").select2('data')
            var dataset =
            {
                Item_Name: Item_Name,
                Rate: Rate,
                Quantity: Quantity,
                UOM: UOM,
                Amount: Amount,
                Account_Head: dat[0].text,
                Account_Head_Id: dat[0].id,
                Debit: $(this).find(".dr").val(),
                Credit: $(this).find(".cr").val(),
                Line: i
            };
            InventoryArray.push(dataset);
        });
    }
    if (confirm("Are you sure you want to Update Entries")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/UpdateRecordedEntries/',
            data: { Details: InventoryArray, TransactionId: tran },
            success: function (data) {
                alert("Payment Requested");
                window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".sup-vouc", function () {
    var id = $("#g-id").val();
    if (confirm("Are you sure you want to Supervise The Payment Voucher")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/UpdateSuperVoucher/',
            data: { Id: id },
            success: function (data) {
                if (data) {
                    alert("Supervised Successfully");
                    window.location.reload();
                }
                else {
                    alert("Can not make Transaction at this moment")
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".rev-ent", function () {
    var id = $(this).attr("id");
    if (confirm("Are you sure you want to Revert The Entry")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/RevertEntry/',
            data: { Id: id },
            success: function (data) {
                alert("Reverted Successfully");
                $("#" + id).remove();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".up-line-head", function () {
    var j = $(this).closest('.own-det').find('.hed-lst');
    var html = `<select class="form-control acc-head"></select>`;
    $(j).append(html);
    $(j).find('.hed-lst-in').hide();
    $(this).closest('.own-det').find('.he-sav-btn').show();
    var lst = $(j).find('.acc-head').last();
    $(lst).select2({
        placeholder: "Search for Head",
        ajax: {
            url: '/AccountHeads/GetHead/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term// search term
                };
            },
            processResults: function (data) {
                return {
                    results: $.map(data.items, function (item) {
                        return {
                            text: item.Parent,
                            children: [{
                                id: item.id,
                                text: item.text + ' - ' + item.head
                            }]
                        }
                    })
                };
            }
        }
    });
});
//
$(document).on("click", ".he-sav-btn", function () {
    var id = $(this).data('id');
    var $this = $(this);
    var head = $(this).closest('.own-det').find('.acc-head').val();
    var headtext = $(this).closest('.own-det').find('.acc-head :selected').text();
    if (head == "" || head == null) {
        alert("Please Select a Head");
        return false;
    }
    if (confirm("Are you sure you want to Update the line")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/UpdateGeneralEntry/',
            data: { Id: id, Head_Id: head },
            success: function (data) {
                alert("Updated");
                var j = $($this).closest('.own-det').find('.hed-lst');
                var html = `<input type="text" value="${headtext}" class="form-control hed-lst-in" readonly />`;
                $(j).html(html);
                $("#" + id + " .he-sav-btn").hide();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".pay-vouc", function () {
    var Transid = $("#t-id").val();
    var id = $("#g-id").val();
    var insno = $("#inst-no").val();
    var amt = Number($("#Amount").val());
    var paymenttype = $("#paymenttype").val();
    var cbpdate = $("#cbp-date").val();
    var Bank = $("#Bank").val();
    var Branch = $("#Branch").val();
    var actamt = Number($("#t-amt").val());
    var head = $("#head-id").val();
    var descr = $(".desc").val();
    if (amt > actamt) {
        alert("You Cannot Pay more than Bill Amount");
        return false;
    }
    if (confirm("Are you sure you want to Generate The Payment Voucher")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/GenerateVoucher/',
            data: { Id: id, TransactionId: Transid, Amount: amt, Bank: Bank, Branch: Branch, Cbp_No: insno, Cbp_date: cbpdate, PaymentType: paymenttype, Head_Id: head, Description: descr },
            success: function (data) {
                if (data == false) {
                    alert("Can not make Transaction at this moment")
                }
                else {
                    window.open("/Vouchers/SAGVouchers_Vendor?Id=" + data.VoucherNo + "&Token=" + data.Token, '_blank');
                    window.location.reload();
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
////
//$(document).on("click", ".ent-rever", function () {
//    var id = $("#g-id").val();
//    if (confirm("Are you sure you want to Reverse The Entry")) {
//        $.ajax({
//            type: "POST",
//            url: '/GeneralEntry/ReverseEntry/',
//            data: { Id: id },
//            success: function (data) {
//                alert("Entries Reversed Successfully")
//                window.location.reload();
//            }
//            , error: function (xmlhttprequest, textstatus, message) {
//                if (textstatus === "timeout") {
//                    alert("got timeout");
//                } else {
//                    alert(textstatus);
//                }
//            }
//        });
//    }
//});
//
$(document).on("click", ".op-gen-entr", function () {
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Entry History");
    $('#modalbody').load('/GeneralEntry/GE_History/', { GroupId: id });
})
//
$(document).on("click", ".vouc-det", function () {
    var Id = $(this).closest('tr').attr("id");
    window.open("/GeneralEntry/EntryDetails?Id=" + Id, '_blank');
});
//
$(document).on("click", ".up-gen-det", function () {
    var Id = $(this).closest('tr').attr("id");
    window.open("/GeneralEntry/UpdateGeneralEntry?Id=" + Id, '_blank');
});
//
$(document).on("click", ".vouc-pay-det", function () {
    var Id = $(this).closest('tr').attr("id");
    var head = $(this).closest('tr').data("head");
    window.open("/GeneralEntry/VoucherPayableDetails?Id=" + Id + "&HeadId=" + head, '_blank');
});
//
$(document).on("keyup", ".rate-calc", function () {
    CalTotalAmt();
});
//
function CalTotalAmt() {
    var ttl = 0
    $(".Purchase_Rate").each(function () {
        var v = RemoveComma($(this).val());
        ttl = ttl + parseFloat(v);
    });
    $(".f-amt").val(Number(ttl).toLocaleString());
    $("#amt").val(ttl);
}
//
$(document).on("click", ".rm-dr-row, .rm-cr-row", function () {
    $(this).parent().parent().remove();
})
//
$(document).on("click", ".rm-add-inven", function () {
    $(this).parent().parent().remove();
    InventoryCounter = 1;
    $('.add-inven-counter').each(function () {
        $(this).text(InventoryCounter + '.');
        InventoryCounter++;
    });
    InventoryCounter = 1;
    $('.add-inv-id').each(function () {
        InventoryCounter++;
        $(this).attr('id', 'add-inv-' + InventoryCounter);
    });
})
//
var imgid = '#images-';
$(document).on('click', '#Add-inven-scan', function () {
    $('#Add-inven-scan').prop('disabled', true);
    imgid = '';
    imgid = '#images-';
    var v = $(this).attr('data-id');
    imgid = imgid + v;
    scanToJpg();
})
//
$(document).on("click", "#ad-inven-nogrn", function (e) {
    InventoryArray = [];
    var Supplier_Id = $('.Vendor').val();
    var Supplier_Name = $(".Vendor option:selected").text();
    var PO_Numbe = $(" .PO_Number").val();
    var PO_Dat = $(".PO_Date").val();
    for (var i = 1; i <= InventoryCounter; i++) {
        var Name = $("#add-inv-" + i + " .Name").val();
        var Quantity = $("#add-inv-" + i + " .Quantity").val();
        var Measurement = $("#add-inv-" + i + " .Measurement").val();
        var Rate = $("#add-inv-" + i + " .Rate").val();
        var Remark = $("#add-inv-" + i + " .Remarks").val();
        var Invoice_Numbe = $("#add-inv-" + i + " .Invoice_Number").val();
        var Invoice_Dat = $("#add-inv-" + i + " .Invoice_Date").val();
        var FileImage = $("#add-inv-" + i + " #scanned").attr('src');
        var len = $('#inv-len').val();
        var wid = $('#inv-wid').val();
        var hei = $('#inv-hei').val();
        var dia = $('#inv-dia').val();
        var dep = $('#inv-for-dep option:selected').val();
        var dataset =
        {
            Name: Name,
            Quantity: Quantity,
            Measurement: Measurement,
            Rate: Rate,
            Supplier_Id: Supplier_Id,
            Supplier_Name: Supplier_Name,
            Remarks: Remark,
            Invoice_Number: Invoice_Numbe,
            Invoice_Date: Invoice_Dat,
            PO_Number: PO_Numbe,
            PO_Date: PO_Dat,
            Image: FileImage,
            Len: len,
            Wid: wid,
            Hei: hei,
            Dia: dia,
            Dep: dep
        }
        InventoryArray.push(dataset);
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/AddInventory/',
        data: { inventory: InventoryArray },
        success: function (data) {
            $('#ad-inven').prop('disabled', true);
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("keyup", ".inv-rate", function () {
    var quan = RemoveComma($(this).closest('.cal').find('.Quantity').val());
    var rate = RemoveComma($(this).closest('.cal').find('.Rate').val());
    var calcu = (quan * rate)
    $(this).closest('.cal').find('.p-r-c').val(Number(calcu).toLocaleString());
    $(this).closest('.cal').find('.Purchase_Rate').val(Number(calcu).toLocaleString());
    CalTotalAmt();
});
//
$(document).on("click", ".all-inve", function () {
    var name = $(this).attr('id');
    var recId = $(this).attr('data-item');
    EmptyModel();
    $('#ModalLabel').text("Inventory List");
    $('#modalbody').load('/Inventory/InventoryByName/', { Name: name, item: recId });
})
//
//
$(document).on("click", ".del-inv", function (e) {
    var Id = $(this).attr('id');
    var dataset =
    {
        Id: Id,
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/InvenoyDelete/',
        data: { inv: dataset },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                alert("Deleted");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//********Requisition
$(document).on("click", "#reg-inven-req", function () {
    EmptyModel();
    $('#ModalLabel').text("Inventory Requisition");
    $('#modalbody').load('/Inventory/AddInventoryRequisition/');
})
//
$(document).on("click", ".add-row-inven-req", function () {
    InventoryCounter++;
    var html = '<div class="form-row inv-id" id="inv-' + InventoryCounter + '"><h6 class="lh-1 mB-0 inven-counter">' + InventoryCounter + '.</h6>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Name" name="Name" data-id=' + InventoryCounter + '" required  placeholder="Product"/></div>' +
        '<div class="form-group col-md-2">   <input class="form-control coma" placeholder="12,345" required><input type="hidden" id="" name="Quantity" class="amt Quantity" data-id="' + InventoryCounter + '" required ></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Measurement" name="Measurement" data-id="' + InventoryCounter + '" required placeholder="Measurement"></div>' +
        //'<div class="form-group col-md-2"><input type="text" class="form-control Project_Name" name="Project_Name" data-id="' + InventoryCounter + '" required placeholder="Project Name"></div>'+
        '<div class="form-group col-md-2"><input type="text" class="form-control Need_Date" data-provide="datepicker" placeholder="mm/dd/yyyy" name="Need_Date" data-id="' + InventoryCounter + '"></div>' +
        '<i class="ti-minus rm-inven-req pointer"></i></div>';
    $('#inv-req-row').append(html);
});
//
$(document).on("click", ".rm-inven-req", function () {
    $(this).parent().remove();
    InventoryCounter = 1;
    $('.inven-counter').each(function () {
        $(this).text(InventoryCounter + '.');
        InventoryCounter++;
    });
    InventoryCounter = 1;
    $('.inv-id').each(function () {
        InventoryCounter++;
        $(this).attr('id', 'inv-' + InventoryCounter);
    });
});
//
$(document).on("click", "#add-inven-req", function () {
    InventoryArray = [];
    var Project_Name = $(".Project_Name").val();
    for (var i = 1; i <= InventoryCounter; i++) {
        var Name = $("#inv-" + i + " .Name").val();
        var Quantity = $("#inv-" + i + " .Quantity").val();
        var Measurement = $("#inv-" + i + " .Measurement").val();
        var Need_Date = $("#inv-" + i + " .Need_Date").val();
        var dataset =
        {
            Name: Name,
            Quantity: Quantity,
            Measurement: Measurement,
            Need_Date: Need_Date,
            Project_Name: Project_Name
        }
        InventoryArray.push(dataset);
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/AddInventoryRequisition/',
        data: { inventory: InventoryArray },
        success: function (data) {
            alert("Requsition Registerd");
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", ".inven-req-update", function () {
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Requisition Update");
    $('#modalbody').load('/Inventory/RequisitionUpdate/', { Id: id });
});
//
$(document).on("click", "#Update-req", function (e) {
    var Name = $('.Name').val();
    var req_update = $('.req_update').val();
    var Quantity = $('.Quantity').val();
    var Need_Date = $('.Need_Date').val();
    var Date = $('.Date').val();
    var Project_Name = $('.Project_Name').val();
    var dataset =
    {
        Name: Name,
        Id: req_update,
        Quantity: Quantity,
        Need_Date: Need_Date,
        Date: Date,
        Project_Name: Project_Name,
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/RequisitionUpdation/',
        data: { req: dataset },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                alert("Updated");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//********************Vendor
$(document).on("click", ".reg-ven", function () {
    EmptyModel();
    $('#ModalLabel').text("Register Vendor");
    $('#modalbody').load('/Vendor/RegisterVendor/');
});
//
$(document).on("click", ".reg-vendor-btn", function (e) {
    if (confirm("Are you sure you want to Register Vendor")) {
        $.ajax({
            type: "POST",
            url: $("#reg-vendor").attr('action'),
            data: $("#reg-vendor").serialize(),
            success: function (data) {
                if (data) {
                    alert("Vendor Registerd");
                    window.location.reload();
                }
                else {
                    alert("Vendor Already Exists")
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".temp-reg-vendor-btn", function (e) {
    if (confirm("Are you sure you want to Register Vendor")) {
        $.ajax({
            type: "POST",
            url: $("#reg-vendor").attr('action'),
            data: $("#reg-vendor").serialize(),
            success: function (data) {
                if (data) {
                    alert("Vendor Registerd");
                    $('.modal-close-btn').click();
                }
                else {
                    alert("Vendor Already Exists")
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".ven-det", function () {
    var id = $(this).attr('id');
    window.open('/Vendor/VendorDetails?Id=' + id, '_blank');
});
//
$(document).on("click", ".temp-reg-ven", function () {
    var vendorbidid = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Temporary Add Vendor");
    $('#modalbody').load('/Vendor/TemporaryRegisterVendor/');
});
//
$(document).on("click", ".RFQ-reg-ven", function () {
    var grp = $('#Group_Id').val();
    window.open("/Procurement/GenerateRFQ?Group_Id=" + grp, '_blank');
});
//
$(document).on("click", "#printRFQ", function () {
    var grp = $('#Group_Id').val();
    var Id = $("#Ven_Id").val();
    window.open("/Procurement/PrintRFQ?Group_Id=" + grp + '&Id=' + Id, '_blank');
});
//
$(document).on("click", ".add-al-quot", function () {
    var id = $('#Group_Id').val();
    EmptyModel();
    $('#ModalLabel').text("Add Quotations");
    $('#modalbody').load('/Procurement/AddAllQuotations/', { Group: id });
});
//
$(document).on("click", ".add-vend-view", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Vendor Representative");
    $('#modalbody').load('/Vendor/AddVendorRepresentative/');
});
//
$(document).on("click", ".add-ven-term", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Vendor Term");
    $('#modalbody').load('/Vendor/AddTermsVendor/');
});
//
$(document).on("click", ".ven-rep-btn", function (e) {
    if (confirm("Are you sure you want to Register Representative")) {
        $.ajax({
            type: "POST",
            url: $("#ven-rep").attr('action'),
            data: $("#ven-rep").serialize(),
            success: function (data) {
                alert("Vendor Representative Added");
                window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".rec-advan", function () {
    EmptyModel();
    $('#ModalLabel').text("Advance Payment");
    $('#modalbody').load('/Procurement/RecordAdv/');
});

$(document).on('change', '.del-com-pay', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateDealerCommissionMapper/',
        data: { Dealer: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
//
$(document).on("click", ".ad-ve-term", function () {
    var bidid = $("#ven-id").val();
    var term = $("#term").val();
    var date = $("#day").val();
    if (confirm("Are you sure you want to Save this Terms")) {
        $.ajax({
            type: 'Post',
            url: '/Vendor/SaveTerm/',
            data: { Term: term, Date: date, Ven_Id: bidid },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
    }
});
//
$(document).on("click", ".rec-advance-btn", function (e) {
    var grp = $(".grpId").val();
    var transcationid = $(".transaction").val();
    var vend = $("#vendors").val();
    var vend_id = $("#vendId").val();
    var amt = Number(RemoveComma($("#adv-amt").val()));
    var desc = $("#nar").val();
    var ttl_pay = Number($("#total-payable").val());
    if (amt > ttl_pay) {
        alert("Amount cannot be greater than payable amount");
        return false;
    }
    if (confirm("Are you sure you want to Request Advance Payment")) {
        $.ajax({
            type: "POST",
            url: $("#rec-advance").attr('action'),
            data: { Vendor: vend, Vendor_Id: vend_id, Amount: amt, Transaction_Id: transcationid, PO_Group_Id: grp, Descriptions: desc },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
                //window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
//
$(document).on("click", ".upd-ven", function () {
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Vendor Detail");
    $('#modalbody').load('/Vendor/VendorUpdate/', { Id: id });
});
//
$(document).on("click", "#ven-update-btn", function (e) {
    if (confirm("Are you sure you want to update Record")) {
        $.ajax({
            type: "POST",
            url: $("#ven-update").attr('action'),
            data: $("#ven-update").serialize(),
            success: function (data) {
                alert("Updated");
                window.location.reload();
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("click", ".del-ven", function (e) {
    var id = $(this).attr('id');
    $.ajax({
        type: "POST",
        url: '/Vendor/DeleteVendor/',
        data: { Id: id },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                alert("Deleted");
                $('#Vendor tbody tr#' + id + '').remove();
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("change", ".v-det-sh", function (e) {
    var Id = $("#Vendor").val();
    $.ajax({
        type: "POST",
        url: '/Vendor/VendorDetails_Short/',
        data: { Id: Id },
        success: function (data) {
            $("#v-nam").val(data.Name);
            $("#v-comp").val(data.Company);
            $("#v-add").val(data.Address);
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", ".cr-prj-srt", function () {
    EmptyModel();
    var html = '<div class="form-row"><div class="form-group col-md-2"></div><div class="form-group col-md-8"><label>Project Name</label><input class="form-control" id="prj-nam" required></div><div class="form-group col-md-2"></div></div>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-success" id="cr-prj-srt-btn" type="submit">Create Project</button>');
});
//
$(document).on("click", ".vouh-print", function (e) {
    e.preventDefault();
    var id = $(this).data('grp');
    var con = confirm("Are you sure you want to print Voucher");
    if (con) {
        window.open("/Vouchers/Voucher?GroupId=" + id, '_blank');
    }
});
//
//
$(document).on("click", "#cr-prj-srt-btn", function (e) {
    var prj = $("#prj-nam").val();
    if (prj == "" || prj == null) {
        alert("Project Name is Required");
        return false;
    }
    if (confirm("Are you sure you want to Create a Project")) {
        $.ajax({
            type: "POST",
            url: '/Projects/ProjectShort/',
            data: { Name: prj },
            success: function (data) {
                if (data) {
                    alert("Project Created");
                    window.location.reload();
                }
                else {
                    alert("Project Already Exists")
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
//
$(document).on("change", ".meas-che", function () {
    var val = $(this).val();
    var id = $(this).data("id");
    if (val != '-') {
        $("#add-inv-" + id + " .Purchase_Rate").attr("readonly", true)
        $("#add-inv-" + id + " .Purchase_Rate").val('')
    }
    else {
        $("#add-inv-" + id + " .Purchase_Rate").removeAttr("readonly")
    }
});
//
$(document).on("click", ".ven-lst-det", function () {
    var vendorid = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Transaction Details");
    $('#modalbody').load('/Vendor/VendorDetail/', { VendorId: vendorid });
});
//
$(document).on("change", ".ven-selec", function () {
    var vendorid = $(" .Vendors").val();
    $('#ven-lst').load('/Vendor/VendorDetailById/', { VendorId: vendorid });
});
//********Goods Receiving
$(document).on("click", "#gd-rec", function () {
    EmptyModel();
    $('#ModalLabel').text("Goods Receiving");
    $('#modalbody').load('/Inventory/GoodReceived/');
})
//
//**********Bidding
$(document).on("click", "#inven-bid", function () {
    EmptyModel();
    $('#ModalLabel').text("Inventory Bidding");
    $('#modalbody').load('/Inventory/Bidding/');
});
//
$(document).on('click', '#bid-grp-search', function () {
    var grpid = $('.grp_bid_num').val();
    $("#dmd_requisition").load('/Inventory/DemandSearch/', { GroupId: grpid });
});
//
function InitVendor(i) {
    $("#add-bid-" + i + " .Vendor").append('<option>Select Vendor</option>');
    $.each(Vendor, function (key, value) {
        $("#add-bid-" + i + " .Vendor").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
function InitDemand(i) {
    $("#add-bid-" + i + " .DemandDropDown").append('<option>Select Demand</option>');
    $.each(DemandDropDown, function (key, value) {
        $("#add-bid-" + i + " .DemandDropDown").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
$(document).on("click", ".add-row-bid", function () {
    InventoryCounter++;
    var html = '<div class="form-row bid-id bid" id="add-bid-' + InventoryCounter + '"><h6 class="bid-counter" > ' + InventoryCounter + '.</h6 >' +
        '<div class="form-group col-md-3"><select class="form-control Requisition_Product_Name DemandDropDown" data-id="' + InventoryCounter + '"></select></div>' +
        '<div class="form-group col-md-2"><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="" name="Quantity" class="amt Quantity inv-bid-rate" data-id="' + InventoryCounter + '" required placeholder = "Quantity" ></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Rate inv-bid-rate" name="Rate" data-id="' + InventoryCounter + '" required placeholder="Unit Price Rs."></div>' +
        '<div class="form-group col-md-2"><input type="text" class="form-control Tot" name="Total" data-id="' + InventoryCounter + '" required readonly></div>' +
        '<i class="ti-minus rm-inven-bid pointer" ></i >'
    '</div>';
    $('#ad-bidding-row').append(html);
    InitDemand(InventoryCounter);
});
//
$(document).on("click", ".rm-inven-bid", function () {
    $(this).parent().remove();
    InventoryCounter = 1;
    $('.bid-counter').each(function () {
        $(this).text(InventoryCounter + '.');
        InventoryCounter++;
    });
    InventoryCounter = 1;
    $('.bid-id').each(function () {
        InventoryCounter++;
        $(this).attr('id', 'add-bid-' + InventoryCounter);
    });
});
//
var bidgrandtot = 0;
$(document).on("keyup", ".inv-bid-rate", function () {
    var qua = $(this).closest('.bid').find('.Quantity').val();
    var rat = $(this).closest('.bid').find('.Rate').val();
    var calcul = (qua * rat);
    var bidgrandtot = $(this).closest('.bid').find('.Tot').val(calcul);
});
//
var imgid = '#images-';
$(document).on('click', '#inv-scan', function () {
    imgid = '';
    imgid = '#images-';
    var v = $(this).attr('data-id');
    imgid = imgid + v;
    scanToJpg();
});
//
$(document).on("click", "#inven-reg-bid", function (e) {
    InventoryArray = [];
    var Vendor_Name = $(" .Vendors option:selected").text();
    var Vendor_Id = $(" .Vendors").val();
    if (Vendor_Id == "" || Vendor_Id == null) {
        alert("Select Vendor");
        return false;
    }
    var Gid = $(".dmd-grp-id").val();
    for (var i = 1; i <= InventoryCounter; i++) {
        var Requisition_Product_Name = $("#add-bid-" + i + " .Requisition_Product_Name").val();
        var Requisition_Product_Id = $("#add-bid-" + i + " .Requisition_Product_Name").attr('data-prdct');
        var Rate = $("#add-bid-" + i + " .Rate").val();
        var Quantity = $("#add-bid-" + i + " .Quantity").val();
        var FileImage = $("#add-bid-" + i + " #scanned").attr('src');
        var dataset =
        {
            Requisition_Product_Name: Requisition_Product_Name,
            Requisition_Product_Id: Requisition_Product_Id,
            Vendor_Name: Vendor_Name,
            Vendor_Id: Vendor_Id,
            Rate: Rate,
            Quantity: Quantity,
            Demad_Order_id: Gid,
            Image: FileImage,
        }
        InventoryArray.push(dataset);
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/RegisterBidding/',
        data: { bid: InventoryArray },
        success: function (data) {
            $('#inven-reg-bid').prop('disabled', true);
            alert("Bid Register");
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//**********Purchsae Order
$(document).on("click", ".pur-ord", function () {
    var id = $(this).attr("id");
    var grp_id = $(this).closest('tr').attr('id');
    EmptyModel();
    $('#ModalLabel').text("Purchase Order");
    $('#modalbody').load('/Inventory/PurchaseOrder/', { Id: id, groupId: grp_id });
});
//
$(document).on("click", ".req-app", function () {
    var grp = $(this).closest('tr').attr("id");
    if (confirm("Are you sure you want to approve this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Procurement/UpdatePurchaseReq/',
            data: { Group_Id: grp, Status: 'Pending' },
            success: function (data) {
                alert("Request Successfull. And moved to Pending");
                $('#' + grp).remove();
                GetReqCount();
                //window.open("/Procurement/PurchaseRequisitionPrint?Group_Id=" + grp, '_blank');
                //alert("Purchase Requisition Number is : " + );
                //$('.data-view').load('/Inventory/PendingApproval');
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
    else {
        $(this).removeAttr("checked")
    }
});
$(document).on("click", ".req-app-edit", function () {
    var grp = $(this).attr("grp-id");
    window.open("/Inventory/EditPurchaseRequisition?Group_Id=" + grp, '_blank');
});
//
$(document).on("click", ".dem-req-app", function () {
    var grp = $(this).closest('tr').attr("id");
    if (confirm("Are you sure you want to approve this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Procurement/UpdateStat/',
            data: { Group_Id: grp },
            success: function (data) {
                alert("Request Successfull.");
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".ser-req-app", function () {
    var grp = $(this).closest('tr').attr("id");
    if (confirm("Are you sure you want to approve this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Services/UpdatePurchaseReq/',
            data: { Group_Id: grp, Status: 'Pending' },
            success: function (data) {
                alert("Request Successfull. And moved to Pending");
                $('#' + grp).remove();
                GetSerCount();
                //window.open("/Procurement/PurchaseRequisitionPrint?Group_Id=" + grp, '_blank');
                //alert("Purchase Requisition Number is : " + );
                //$('.data-view').load('/Inventory/PendingApproval');
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".oe-app", function () {
    var grp = $(this).closest('tr').attr("id");
    if (confirm("Are you sure you want to approve this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Services/Update_Exp_Status/',
            data: { Group_Id: grp, Status: 'Approved' },
            success: function (data) {
                alert("Request Successfull.");
                window.open("/Services/ExpenseDetails?GroupId=" + grp, '_blank');
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".pri-oe", function () {
    var grp = $(this).closest('tr').attr("id");
    window.open("/Services/ExpenseDetails?GroupId=" + grp, '_blank');
});
//
$(document).on("click", ".fin-quo", function () {
    var grp = $(this).closest('tr').attr("id");
    window.open("/Procurement/QuotationFinalization?Group_Id=" + grp, '_blank');
});
//
$(document).on("click", ".fin-dem", function () {
    var grp = $(this).closest('tr').attr("id");
    window.open("/Procurement/DemandFinalization?Group_Id=" + grp, '_blank');
});
//
$(document).on("click", ".fin-quo-ser", function () {
    var grp = $(this).closest('tr').attr("id");
    window.open("/Services/QuotationFinalization?Group_Id=" + grp, '_blank');
});
//
$(document).on("click", ".all-comp-rep", function () {
    var data = [];
    $('.po-fin').each(function () {
        var t = $(this).is(":checked");
        if (t) {
            var val = $(this).val();
            data.push(val)
        }
    });
    if (confirm("Are you sure you want to Generate Purchase Order Summary")) {
        $.ajax({
            type: "POST",
            url: '/Procurement/UpdateCompPurchaseCompRep/',
            data: { Group: data },
            success: function (data) {
                alert("Request Successfull");
                window.open("/Procurement/CompPurchaseCompRep?Date=" + data, '_blank');
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".all-ser-rep", function () {
    var data = [];
    $('.po-fin').each(function () {
        var t = $(this).is(":checked");
        if (t) {
            var val = $(this).val();
            data.push(val)
        }
    });
    if (confirm("Are you sure you want to Generate Purchase Order Summary")) {
        $.ajax({
            type: "POST",
            url: '/Services/UpdateCompPurchaseCompRep/',
            data: { Group: data },
            success: function (data) {
                alert("Request Successfull");
                window.open("/Services/CompPurchaseCompRep?Date=" + data, '_blank');
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".quo-app", function () {
    var grp = $("#Group_Id").val();
    var dep = $("#Dep_Id").val();
    var allok = true;
    $(".it").each(function () {
        var rates = [];
        var $this = $(this);
        var ch = $($this).find('.it-exp').is(":checked");
        if (ch) {
            return;
        }
        $($this).find('.it-qu-val').each(function () {
            rates.push(RemoveComma($(this).text()));
        });
        var quo3check = false;
        for (var i = 0; i < rates.length; i++) {
            if (Number(rates[i]) >= 100000 || (Number(rates[i]) >= 25000 && dep == 25)) {
                quo3check = true;
                break;
            }
        }
        if (quo3check) {
            if (rates.length < 3) {
                alert("3 Quotation are Necessary");
                allok = false;
            }
        }
    });
    if (allok) {
        if (confirm("Are you sure you want to send for Quotation Approval")) {
            $.ajax({
                type: "POST",
                url: '/Procurement/UpdatePurchaseReq/',
                data: { Group_Id: grp, Status: 'Quotation_Approval' },
                success: function (data) {
                    alert("Request Successfull");
                    window.location.reload();
                },
                error: function () {
                    alert("Error Occured");
                }
            });
        }
    }
});
//
$(document).on("click", ".quo-app-ser", function () {
    var grp = $("#Group_Id").val();
    if (confirm("Are you sure you want to send for Quotation Approval")) {
        $.ajax({
            type: "POST",
            url: '/Services/UpdatePurchaseReq/',
            data: { Group_Id: grp, Status: 'Quotation_Approval' },
            success: function (data) {
                alert("Request Successfull");
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", "#reg-purchase", function () {
    var S_Name = $('.S_Name').val();
    var S_Company = $('.S_Company').val();
    var S_Address = $('.S_Address').val();
    var S_Phone = $('.S_Phone').val();
    var S_Email = $('.S_Email').val();
    var S_Delivery_Date = $('.S_Delivery_Date').val();
    var S_Term_Of_Payment = $(".S_Term_Of_Payment option:selected").val();
    var BidId = $('.BidId').val();
    var ProId = $('.ProId').val();
    var VenId = $('.VenId').val();
    var GrpId = $('.GrpId').val();
    var Rate = $('.Rate').val();
    var Notes_And_Instructions = $('textarea#Notes_And_Instructions').val();
    var dataset =
    {
        S_Name: S_Name,
        S_Company: S_Company,
        S_Address: S_Address,
        S_Phone: S_Phone,
        S_Email: S_Email,
        S_Delivery_Date: S_Delivery_Date,
        S_Term_Of_Payment: S_Term_Of_Payment,
        Bid_Id: BidId,
        Product_Id: ProId,
        Vendor_Id: VenId,
        Rate: Rate,
        Notes_And_Instructions: Notes_And_Instructions,
        Requisition_Group_Id: GrpId,
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/RegisterPurchaseOrder/',
        data: { PurchaseOrd: dataset },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                $('#reg-purchase').prop('disabled', true);
                $('#Gen-pur-ord').prop('disabled', false);
                alert("Purchas Order Added");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", "#Gen-pur-ord", function () {
    var Id = $('.VenId').val();
    var Token = $('.GrpId').val();
    //window.open("/Inventory/PrintPurchaseOrder?Id=" + data, '_blank');
    window.open("/Inventory/PrintPurchaseOrder?Id=" + Id + "&Token=" + Token, '_blank');
});
//
$(document).on("click", ".v-pay-det", function () {
    var Id = $(this).attr("id");
    window.open("/Vendor/VendorPaymentDetails?Id=" + Id, '_blank');
});
//*******Inventory Assining
$(document).on("click", "#Inven-asgn-popup", function () {
    EmptyModel();
    $('#ModalLabel').text("Assign List");
    $('#modalbody').load('/Inventory/DemandHandoveringList/');
});
$(document).on("click", ".inv-issue_rqst", function () {
    EmptyModel();
    $('#ModalLabel').text("Demand Order");
    let itemID = $(this).attr('data-trgt');
    $('#modalbody').load('/Inventory/IssueRequest/', { item: itemID });
});
//
$(document).on("click", ".dem-req", function () {
    EmptyModel();
    $('#ModalLabel').text("Demand Requisition");
    $('#modalbody').load('/Inventory/DemandRequisition/');
});
//
function InitInventory(i) {
    $("#assign-" + i + " .InventoryName").append('<option>Select Inventory</option>');
    $.each(InventoryName, function (key, value) {
        $("#assign-" + i + " .InventoryName").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
//
$(document).on("click", ".add-row-Assign", function () {
    InventoryCounter++;
    var html = '<div class="form-row assign-id assign" id="assign-' + InventoryCounter + '"><h6 class="lh-1 mB-0 assign-counter" > ' + InventoryCounter + '.</h6 >' +
        '<div class="form-group col-md-2" style="display: block;">' +
        '<select class="form-control InventoryName inven-selec" data-id="' + InventoryCounter + '"></select>' +
        '</div>' +
        '<div class="form-group col-md-2" style="display: block;">' +
        '<input type="text" class="form-control Assign_quan" value="" readonly data-id="' + InventoryCounter + '" />' +
        '</div>' +
        '<div class="form-group col-md-2" style="display: block;">' +
        '<input type="text" class="form-control Del_quan" id="" required placeholder="Enter Quantity" data-id="' + InventoryCounter + '" />' +
        '</div>' +
        '<i class="ti-minus  pointer rm-inven-assign"></i>' +
        '</div>';
    $('#inv-assign-row').append(html);
    InitInventory(InventoryCounter);
});
//
$(document).on("keyup", ".inven-selec", function () {
    var name = $(this).closest('.assign').find('.InventoryName option:selected').val();
    var dataset =
    {
        Name: name
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/InventorySum/',
        data: { inv: dataset },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                $(this).closest('.assign').find('.Assign_quan').val(data);
            }
        }
        , error: function () {
        }
    });
});
//
$(document).on("click", ".rm-inven-assign", function () {
    $(this).parent().remove();
    InventoryCounter = 1;
    $('.assign-counter').each(function () {
        $(this).text(InventoryCounter + '.');
        InventoryCounter++;
    });
    InventoryCounter = 1;
    $('.assign-id').each(function () {
        InventoryCounter++;
        $(this).attr('id', 'assign-' + InventoryCounter);
    });
});
//
$(document).on("click", "#Assign_demand", function () {
    var Project_Name = $(".sel-pro option:selected").text();
    var Project_Id = $('.sel-pro').val();
    if (Project_Id === "") {
        alert("Select Project");
        return false;
    }
    for (var i = 1; i <= InventoryCounter; i++) {
        var Inventory_Name = $("#assign-" + i + " .InventoryName option:selected").val();
        var Quantity = $("#assign-" + i + " .Del_quan").val();
        var dataset =
        {
            Project_Name: Project_Name,
            Project_Id: Project_Id,
            Product: Inventory_Name,
            Quantity_Assigning: Quantity,
        }
        InventoryArray.push(dataset);
    }
    $.ajax({
        type: "POST",
        url: '/Inventory/AssignQuantity/',
        data: { demand: InventoryArray },
        success: function (data) {
            if (data == false) {
                alert('Error Occured')
            }
            else {
                alert("Successfully Assigned");
            }
        }
        , error: function () {
        }
    });
});
//
$(document).on("change", ".sel-pro", function () {
    var id = $(".sel-pro option:selected").val();
    $("#pro-list").load('/Inventory/ProjectList/', { Id: id });
});
/// Dealship deals *****************************************************************
var dealArray = [];
var Version = [];
$(document).on("click", "#dealer-deal", function () {
    var Proj_id = $('.prj-phas').val();
    var phase_id = $('#phase').val();
    var block_id = $('#block').val();
    var Pro_naem = $('.prj-phas option:selected').text();
    var phase_name = $('#phase option:selected').text();
    var block_name = $('#block option:selected').text();
    var d_amt = $('#dea_dep_amt_1').val();
    var dealership = $('.dealership').val();
    var dealername = $(".dealership option:selected").text();
    var deal_amt = $('.deal_amt').val();
    var advance = $('#advance').val() * 1;
    var rebate = $('#rebate').val() * 1;
    var t_price = $('#tot_marla_price_1').val();
    if ((advance + rebate) != 25) {
        alert('Advance Or Rebate not equal to 25%');
        return false;
    }
    var dataset =
    {
        Dealer_Name: dealername,
        Dealer_Id: dealership,
        Deal_Total_Amount: t_price,
        Advance: advance,
        Rebate: rebate,
        Project_Id: Proj_id,
        Phase_Id: phase_id,
        Block_Id: block_id,
        Project_Name: Pro_naem,
        Phase_Name: phase_name,
        Block_Name: block_name,
        Deposit: d_amt,
    };
    // Deal Full Plan
    dealArray = [];
    var T_files = $('#tot_files_1').val();
    var t_marlas = $('#tot_marla_amt_1').val();
    var rate_pr_marla = $('.rat-per-mar').val();
    for (var i = 1; i <= paycont; i++) {
        var plt_size = $("#add-pln-" + i + " #plot_size").val();
        var files = $("#add-pln-" + i + " .mrls_cal").val();
        var marlas = $("#add-pln-" + i + " .marlas-cal").val();
        var dataset1 =
        {
            Plot_Size: plt_size,
            Num_Of_Files: files,
            Marlas: marlas,
            Total_Files: T_files,
            Total_Marlas: t_marlas,
            Total_Price: t_price,
            Deposit: d_amt,
            Rate_Per_Marla: RemoveComma(rate_pr_marla),
            Advance: advance,
            Rebate: rebate,
        }
        dealArray.push(dataset1);
    }
    $.ajax({
        type: "POST",
        url: '/Dealership/AddDeal/',
        data: { dealers_deals: dataset, deal: dealArray },
        success: function (data) {
            if (data == false) {
                alert('Already Exist');
            }
            else {
                alert('Deal Added');
                window.open("/Dealership/DealList");
            }
        },
        error: function (data) {
            alert('Error Occured');
        }
    });
});
//
$(document).on("click", ".deal_details_open", function () {
    var id = $(this).attr('id');
    window.open("/Dealership/Detail?details=" + id, '_blank');
});
// Get Dealers Data
$(document).on("click", ".dea-data-up", function () {
    var id = $(this).attr("id");
    $('#dea-data-up').load('/Dealership/DealUpdation/', { Id: id });
});
// \Deal Plan Finalize
$(document).on("click", ".deal-pln", function () {
    var id = $(this).attr("id");
    $('#dea-data-up').load('/Dealership/AllotmentPlan/', { Id: id });
});
//
$(document).on("click", "#up-deal", function () {
    var deal_amt = $('#deal_amt').val();
    var advance = $('#advance').val() * 1;
    var rebate = $('#rebate').val() * 1;
    if ((advance + rebate) != 25) {
        alert('Advance and Rebate Should be equal to 25%');
        return false;
    }
    var dataset =
    {
        Id: $(this).val(),
        Deal_Total_Amount: RemoveComma(deal_amt),
        Advance: advance,
        Rebate: rebate
    };
    $.ajax({
        type: "POST",
        url: '/Dealership/UpdateDeal/',
        data: { dealers_deals: dataset },
        success: function (data) {
            if (data == false) {
                alert('Already Exist');
            }
            else {
                alert('Deal Updated');
            }
        },
        error: function (data) {
            alert('Error Occured');
        }
    });
});
//
$(document).on("click", ".alt-pln", function () {
    paycont++;
    var html = '<div class="form-row cal add-pln-id" id="add-pln-' + paycont + '" ><h6 class="lh-1 mB-0 pln-counter">' + paycont + '.</h6>' +
        '<div class="form-group col-md-2">' +
        '<select class="form-control  plt_ch" id="plot_size" data-id="' + paycont + '">' +
        '<option value="">Choose..</option>' +
        '<option value="1">1</option>' +
        '<option value="2">2</option>' +
        '<option value="3">3</option>' +
        '<option value="4">4</option>' +
        '<option value="5">5</option>' +
        '<option value="6">6</option>' +
        '<option value="7">7</option>' +
        '<option value="8">8</option>' +
        '<option value="9">9</option>' +
        '<option value="10">10</option>' +
        '</select>' +
        '</div>' +
        '<div class="form-group col-md-2">' +
        '<input type="text" class="form-control mrls_cal" name="plot_num" id="plot_num" data-id="' + paycont + '">' +
        '</div>' +
        '<div class="form-group col-md-2">' +
        '<input type="text" class="form-control marlas-cal" name="Marlas"  data-id="' + paycont + '" readonly>' +
        '</div>' +
        '<i class="ti-minus rm-plan-row pointer" ></i></div>';
    $('#ad-pln-altmnt').append(html);
});
//
$(document).on("click", ".rm-plan-row", function () {
    $(this).parent().remove();
    paycont = 1;
    $('.pln-counter').each(function () {
        $(this).text(paycont + '.');
        paycont++;
    });
    paycont = 1;
    $('.add-pln-id').each(function () {
        paycont++;
        $(this).attr('id', 'add-pln-' + paycont);
    });
    TotalFilescalculations();
})
// 
$(document).on("keyup", ".mrls_cal", function () {
    var size = $(this).closest('.cal').find('#plot_size option:selected').val() * 1;
    var numberofplots = $(this).closest('.cal').find('#plot_num').val() * 1;
    var calcu = size * numberofplots;
    $(this).closest('.cal').find('.marlas-cal').val(calcu);
    TotalFilescalculations();
});
// 
$(document).on("change", ".plt_ch", function () {
    var size = $(this).closest('.cal').find('#plot_size option:selected').val() * 1;
    var numberofplots = $(this).closest('.cal').find('#plot_num').val() * 1;
    var calcu = size * numberofplots;
    $(this).closest('.cal').find('.marlas-cal').val(calcu);
    TotalFilescalculations();
});
//
$(document).on("keyup", ".rat-per-mar", function () {
    TotalFilescalculations();
});
//
$(document).on("keyup", "#advance", function () {
    TotalFilescalculations();
});
//
function TotalFilescalculations() {
    var adv = /*$('#dea-adv').val();*/ $('#advance').val();
    var ratepermarla = $('.rat-per-mar').val() || 0;
    if (ratepermarla == 0) {
        var marl = 0;
    }
    else {
        marl = RemoveComma(ratepermarla);
    }
    var Total = 0;
    $(".mrls_cal").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #tot_files').text(Number(Total).toLocaleString());
    $(' #tot_files_1').val(Total);
    Total = 0;
    $(".marlas-cal").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $('#tot_marla_amt').text(Number(Total).toLocaleString());
    $('#tot_marla_amt_1').val(Total);
    $('#tot_marla_price').text(Number(Total * marl).toLocaleString());
    $('#tot_marla_price_1').val(Total * marl);
    $('#dea_dep_amt').text(Number(((Total * marl) / 100) * (adv)).toLocaleString());
    $('#dea_dep_amt_1').val(((Total * marl) / 100) * (adv));
    $('.deal_amt').val(Number(Total * marl).toLocaleString());
}
//
$(document).on("click", "#gen-de-pln", function () {
    id = $(this).val();
    dealArray = [];
    var T_files = $('#tot_files_1').val();
    var t_marlas = $('#tot_marla_amt_1').val();
    var t_price = $('#tot_marla_price_1').val();
    var d_amt = $('#dea-dep-amt-1').val();
    var rate_pr_marla = $('.rat-per-mar').val();
    for (var i = 1; i <= paycont; i++) {
        var plt_size = $("#add-pln-" + i + " #plot_size").val();
        var files = $("#add-pln-" + i + " .mrls_cal").val();
        var marlas = $("#add-pln-" + i + " .marlas-cal").val();
        var dataset =
        {
            Plot_Size: plt_size,
            Num_Of_Files: files,
            Marlas: marlas,
            Total_Files: T_files,
            Total_Marlas: t_marlas,
            Total_Price: t_price,
            Deposit: d_amt,
            Rate_Per_Marla: RemoveComma(rate_pr_marla),
        }
        dealArray.push(dataset);
    }
    $.ajax({
        type: "Post",
        url: '/Dealership/GeneratePlan/',
        data: { Id: id, deal: dealArray },
        success: function (data) {
            if (data === false) {
                alert('Error Occured');
            }
            else {
                alert('Plan Generated');
            }
        },
        error: function () {
        }
    });
});
//
$(document).on("click", "#ser-deal", function () {
    var grpid = $('#deal-grp-id').val();
    $('#full_deal').load('/Dealership/FullDeal/', { GroupId: grpid })
});
//
$(document).on("click", "#gen-dl-ver", function () {
    dealArray = [];
    Version = [];
    var rate = $('.rate').val() * 1;
    var amt = $('#tot_amt_1').val();
    var depo = $('#tot_depo_1').val();
    var dealid = $('.deal_id').val();
    var proid = $('.proj').val();
    var phaseid = $('.phase').val();
    var blkid = $('.blk').val();
    var dealership_id = $('.dealershipid').val();
    var dealername = $('.dealershipname').val();
    $(".deal-ver-cre").each(function () {
        var id = $(this).attr('id');
        var gen_files = $(this).closest('tr').find('.gen_files').attr('id') * 1;
        var plot_size = $(this).closest('tr').find('.plot_size').attr('id') * 1;
        var assign_file = $(this).closest('tr').find('.assign_file').val() * 1;
        var pending = $(this).closest('tr').find('.rem_files').attr('id') * 1;
        var assign = $(this).closest('tr').find('.assign_file').val() * 1;
        if (assign == "" || assign == 0) {
        }
        else if (assign <= pending || assign > pending) {
            var dataset =
            {
                Id: id,
                Remaning_Files: pending - assign,
            }
            dealArray.push(dataset);
            var dataset1 =
            {
                Plot_Size: plot_size,
                Filecount: assign_file,
                Dealership_Id: dealership_id,
                Project_Id: proid,
                Phase: phaseid,
                Block: blkid,
                Rare_Per_marla: rate,
                Amount: amt,
                Deposit: depo,
                Deal_Id: dealid,
                Dealership_Name: dealername,
            }
            Version.push(dataset1);
        }
        else {
            alert("Error Occured");
            return false;
        }
    });
    $.ajax({
        type: "Post",
        url: '/Dealership/CreateVersion/',
        data: { Id: dealid, deal: dealArray, version: Version },
        success: function (data) {
            if (data === false) {
                alert('There are not any remaining files or Plot Installment Structure');
            }
            else {
                $.post('/Dealership/DealsFormAssociation/', { dealerFileForms: data }, function (data) {
                    alert('Files Allocated');
                    window.location.reload();
                });
            }
        },
        error: function () {
        }
    });
});
//
$(document).on("click", "#sub-reb", function () {
    var did = $(this).val();
    var amt = $("#Amount").val();
    var pty = $("#payty option:selected").val();
    if ($('#reb-chk').is(":checked")) {
        var selmode = $('#reb-chk').val();
    }
    if (amt == null || amt == "") {
        alert("Amount Cannot be Empty");
        return false;
    }
    var rebdata =
    {
        Amount: amt,
        PaymentType: pty,
        Method: selmode,
        Deal_Id: did,
    };
    $.post('/Dealership/RebateRelease/', { rebate: rebdata }, function (data) {
        window.open("/Dealership/RebateVoucherRelease?VoucherId=" + data.Receipt_Id, '_blank');
    });
});
//
$(document).on("click", "#reb-rel", function () {
    var deal_id = $(this).val();
    EmptyModel();
    $('#ModalLabel').text('Rebate Release');
    $('#modalbody').load('/Dealership/Rebate/', { Deal_Id: deal_id });
});
//
$(document).on("click", ".shw-file-num", function () {
    var grpid = $(this).attr('id');   /*$('#shw-file-num').val();*/
    EmptyModel();
    $('#ModalLabel').text("Register Files");
    $('#modalbody').load('/Dealership/FilesRetrievel/', { Groupid: grpid });
});
// 
$(document).on("click", "#pnt-deal", function () {
    var grpid = $(this).val();
    $('#pnt-deal').prop('disabled', true);
    window.open("/Dealership/NewFileDesign?Group_Id=" + grpid, '_blank');
});
//
$(document).on("click", ".pnt-deal-vou", function () {
    var ins_pln_id = $(this).attr('id');
    var id = $(this).closest('tr').attr('id');
    window.location.reload();
    window.open("/Dealership/DealVoucher?PlanId=" + ins_pln_id + "&FileFormId=" + id, '_blank');
});
//
$(document).on("click", ".prt-rcpt", function () {
    var id = $(this).val();
    EmptyModel();
    $('#ModalLabel').text("Receipt");
    $('#modalbody').load('/Dealership/ReceiptModel/', { Id: id });
});
//
$(document).on("click", "#gen-de-rec", function () {
    var receiptid = $("#receiptid").val();
    var mobile = $('#Mobile_1').val();
    var dealreceipt = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", FileImage: "",
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#prj").val();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.FileImage = $("#pay-" + i + " #scanned").attr('src');
        recedata.Mobile_1 = mobile;
        dealreceipt.push(recedata);
    }
    $.post('/Dealership/ReceiptGenerate/', { Id: receiptid, Receiptdata: dealreceipt }, function (data) {
        $(data.Receiptid).each(function (i) {
            window.open("/Receipts/DealerAdvance?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
        });
    });
});
//
TotalFilescalculationsTwo();
function TotalFilescalculationsTwo() {
    var Rate = $('.rate').val();
    var advan = $('.adv').val();
    var Total = 0;
    $(".gen_files").each(function () {
        var ded = $(this).attr('id');
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #gene_file').text(Number(Total).toLocaleString());
    $(' #gene_file_1').val(Total);
    Total = 0;
    $(".rem_files").each(function () {
        var ded = $(this).attr('id');
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #remni_files').text(Number(Total).toLocaleString());
    $(' #remni_files_1').val(Total);
    //var rema = $(' #remni_files_1').val() * 1;
    //if (rema == 0) {
    //    $('#gen-dl-ver').prop('disabled', true);
    //}
    Total = 0;
    $(".assign_file").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #assgn_files').text(Number(Total).toLocaleString());
    $(' #assgn_files_1').val(Total);
    Total = 0;
    $(".marlas_cal").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #assgn_marlas').text(Number(Total).toLocaleString());
    $(' #assgn_marlas_1').val(Total);
    Total = 0;
    $(".marlas_cal").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    $(' #tot_amt').text(Number(Total * Rate).toLocaleString());
    $(' #tot_amt_1').val(Total * Rate);
    var amt = $(' #tot_amt_1').val() * 1;
    $('#tot_depo').text(Number(((amt) / 100) * (advan)).toLocaleString());
    $('#tot_depo_1').val(((amt) / 100) * (advan));
}
//
$(document).on("keyup", ".assign_file", function () {
    var size = $(this).closest('.deal-ver-cre').find('.plot_size').attr('id') * 1;
    var numberofplots = $(this).closest('.deal-ver-cre').find('.assign_file').val() * 1;
    var calcu = size * numberofplots;
    $(this).closest('.deal-ver-cre').find('.marlas_cal').val(calcu);
    TotalFilescalculationsTwo();
});
// ******************* Chart of Accounts
$(document).on("click", ".up-add-node", function () {
    EmptyModel();
    var id = $(this).data("id");
    $('#ModalLabel').text("Configure Head");
    $('#modalbody').load('/AccountHeads/AddUpdateNode/', { Id: id });
});
//
$(document).on("click", ".up-head", function () {
    var id = $("#hd-id").val();
    var head = $('#head').val();
    if (confirm("Are you sure you want to Update Head Name")) {
        $.ajax({
            type: "Post",
            url: '/AccountHeads/UpdateHeadName/',
            data: { Id: id, Head: head },
            success: function (data) {
                alert('Updated');
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("click", ".up-op-bal", function () {
    var id = $("#hd-id").val();
    var openbal = RemoveComma($('.bal-amt').val());
    var opendate = $('#opening-date').val();
    var start = $('#fstart').val();
    var end = $('#fend').val();
    if (Date.parse(opendate) < Date.parse(start)) {
        alert("Select Date Within Current Fiscal Year");
        return false;
    }
    else if (Date.parse(opendate) > Date.parse(end)) {
        alert("Select Date Within Current Fiscal Year");
        return false;
    }
    if (confirm("Are you sure you want to Update Opening Balance")) {
        $.ajax({
            type: "Post",
            url: '/AccountHeads/UpdateOpeningBalance/',
            data: { Id: id, Amount: openbal, OpeningDate: opendate },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("click", ".ad-heads", function () {
    var id = $("#hd-id").val();
    var head = $('#name').val();
    if (confirm("Are you sure you want to add a New Head ")) {
        $.ajax({
            type: "Post",
            url: '/AccountHeads/Create/',
            data: { Id: id, Head: head },
            success: function (data) {
                alert('New Head Added');
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("click", ".ch-par-head", function () {
    var id = $("#hd-id").val();
    var newhead = $('#ch-acc').val();
    if (confirm("Are you sure you want to Change the Parent Head")) {
        $.ajax({
            type: "Post",
            url: '/AccountHeads/UpdateParent/',
            data: { NewParent: newhead, Id: id },
            success: function (data) {
                if (data) {
                    alert('Parent Has Been Updated');
                    window.location.reload();
                }
                else {
                    alert('Cannot be Updated because Transactions are available');
                }
            },
            error: function () {
            }
        });
    }
});
///// ********* Admin Facilities
//
$(document).on("click", ".fac-pln", function () {
    paycont++;
    if (paycont <= 2) {
        var html = '<hr /><h6 class="c-grey-900">Yearly</h6><div class="form-row cal add-fac-id" id="add-pln-ad-' + paycont + '" ><h6 class="lh-1 mB-0 pln-counter">' + paycont + '.</h6>' +
            '<div class="form-group col-md-2">' +
            '<label>Employees</label>' +
            '<input class="form-control coma Empfee" placeholder="12,345" required style="" data-id="' + paycont + '">' +
            '<input type="hidden" id="Empfee" name="Amount" class="amt" required>' +
            '</div>' +
            '<div class="form-group col-md-2">' +
            '<label>Residential</label>' +
            '<input class="form-control coma Resfee" placeholder="12,345" required style="" data-id="' + paycont + '">' +
            '<input type="hidden" id="Resfee" name="Amount" class="amt" required>' +
            '</div>' +
            '<div class="form-group col-md-2">' +
            '<label>Outsiders</label>' +
            '<input class="form-control coma Outfee" placeholder="12,345" required style="" data-id="' + paycont + '">' +
            '<input type="hidden" id="Outfee" name="Amount" class="amt" required>' +
            '</div>' +
            '<div class="form-group col-md-2">' +
            '<label>Membership Fee</label>' +
            '<input class="form-control coma Memfee" placeholder="12,345" required style="" data-id="' + paycont + '">' +
            '<input type="hidden" id="Memfee" name="Amount" class="amt" required>' +
            '</div>' +
            '<i class="ti-minus rm-fac-row pointer" style="margin-left:3%" ></i></div>';
        $('#ad-pln-fac').append(html);
    }
    else {
        alert('You cannot enter another subscription');
    }
});
//
$(document).on("click", ".rm-fac-row", function () {
    $(this).parent().remove();
    paycont = 1;
    $('.pln-counter').each(function () {
        $(this).text(paycont + '.');
        paycont++;
    });
    paycont = 1;
    $('.add-fac-id').each(function () {
        paycont++;
        $(this).attr('id', 'add-pln-ad-' + paycont);
    });
})
//
$(document).on("click", ".addpro", function () {
    var pr = $('#prjname').val();
    if (pr == null || pr == "") {
        alert('Please Enter Project Name');
        return false;
    }
    var ef = $('#empmon').val();
    if (ef == null || ef == 0 || ef == "") {
        alert('Please Enter Employee Monthly Fee');
        return false;
    }
    var rm = $('#resmon').val();
    if (rm == null || rm == 0 || rm == "") {
        alert('Please Enter Resident Monthly Fee');
        return false;
    }
    var outf = $('#outmon').val();
    if (outf == null || outf == 0 || outf == "") {
        alert('Please Enter Outsider Monthly Fee');
        return false;
    }
    var datearry = [];
    var alldata =
    {
        project_Name: $('#prjname').val(),
        Description: $('#des').val(),
        Emp_Fee: $('#empmon').val() || 0,
        Emp_Mem_Fee: $('#empmem').val() || 0,
        Res_Fee: $('#resmon').val() || 0,
        Res_Mem_Fee: $('#Resmem').val() || 0,
        Out_Fee: $('#outmon').val() || 0,
        Out_Mem_Fee: $('#outmem').val() || 0
    };
    datearry.push(alldata);
    var x = confirm('Are you sure you want to generate fee structure');
    if (x) {
        $.ajax({
            type: "Post",
            url: '/AdminFacilities/AddProject/',
            data: { AP: datearry },
            success: function (data) {
                if (data === false) {
                    alert('Project Already Exists');
                }
                else {
                    alert('Fee Plan Generated');
                    window.location.reload();
                }
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("click", ".getda", function () {
    var type = $('#regType option:selected').val();
    var empcode = $('#serchv').val();
    if (type == "" || type == null) {
        alert('Please Select Type');
        return false;
    }
    if (empcode == "" || empcode == null) {
        alert('Please Enter Code');
        return false;
    }
    $.ajax({
        type: 'Post',
        url: '/AdminFacilities/GetEmployee/',
        data: { EmpCode: empcode },
        success: function (data) {
            $('.Gen__values').val("");
            $('#Namee').text("");
            $('#Father_Namee').text("");
            $('#CNICe').text("");
            $('#Mobile_1e').text("");
            $('#Date_Of_Birth').val("");
            $('#Address').val("");
            $('#Email').val("");
            $('#Company_Email').val("");
            $('#Mobile_2').val("");
            $('#City').val("");
            $('#Blood_Group').val("");
            $('#Religion').val("");
            $('#Marital_Status').val("");
            $('#Emergency_Contact').val("");
            $('#Relationship').val("");
            $('#Date_Of_Joining').val("");
            // filling data
            $('.Gen__values').val(data.Id);
            $('#Namee').text(data.Name);
            $('#Father_Namee').text(data.Father_Name);
            $('#CNICe').text(data.CNIC);
            $('#Mobile_1e').text(data.Mobile_1);
            //$('#Address').val(data.Address);
            //$('#Email').val(data.Email);
            //$('#Company_Email').val(data.Company_Email);
            //$('#Mobile_2').val(data.Mobile_2);
            //$('#City').val(data.City);
            //$('#Blood_Group').val(data.Blood_Group);
            //$('#Religion').val(data.Religion);
            //$('#Marital_Status').val(data.Marital_Status);
            //$('#Emergency_Contact').val(data.Emergency_Contact);
            //$('#Relationship').val(data.Relationship);
            //if (data.Date_Of_Joining != null)
            //{
            //    $('#Date_Of_Joining').val(moment(data.Date_Of_Joining).format("MM-DD-YYYY"));
            //}
        },
        error: function () {
            alert('Employee Not Found');
        }
    });
});
//
$(document).on("change", "#regType", function () {
    var type = $('#regType option:selected').val();
    $('.tot-memcl').val("");
    $('#tot-me-fe').text(0);
    $(".ent-chk").prop("checked", false);
    if (type == "Employee") {
        $('.Gen__values').val("");
        $('#Namee').text("");
        $('#Father_Namee').text("");
        $('#CNICe').text("");
        $('#Mobile_1e').text("");
        $('#Nameo').text("");
        $('#Father_Nameo').text("");
        $('#CNICo').text("");
        $('#Mobile_1o').text("");
        $('#Owner_slogn').text("");
        $('#serchv').val("");
        $('.emp').show();
        $('.res').hide();
        $('.himemout').hide();
        $(".fildataf").hide();
        $('.fildatad').show();
        $(".fildata :input").prop("readonly", true);
        $("#Marital_Status").attr("readonly", true);
        $("#Religion").attr("readonly", true);
        $("#Blood_Group").attr("readonly", true);
    }
    if (type == "Residential") {
        $('.Gen__values').val("");
        $('#Namee').text("");
        $('#Father_Namee').text("");
        $('#CNICe').text("");
        $('#Mobile_1e').text("");
        $('#Nameo').text("");
        $('#Father_Nameo').text("");
        $('#CNICo').text("");
        $('#Mobile_1o').text("");
        $('#Owner_slogn').text("");
        $('#serchv').val("");
        $('.emp').hide();
        $('.res').show();
        $('.himemout').show();
        $(".fildataf").hide();
        $('.fildatad').show();
        $(".fildata :input").prop("readonly", true);
        $("#Marital_Status").attr("readonly", true);
        $("#Religion").attr("readonly", true);
        $("#Blood_Group").attr("readonly", true);
    }
    if (type == "Outsider") {
        $('.Gen__values').val("");
        $('#Namee').text("");
        $('#Father_Namee').text("");
        $('#CNICe').text("");
        $('#Mobile_1e').text("");
        $('#Nameo').text("");
        $('#Father_Nameo').text("");
        $('#CNICo').text("");
        $('#Mobile_1o').text("");
        $('#Owner_slogn').text("");
        $('#serchv').val("");
        $('.hidd').hide();
        $('.himemout').hide();
        $(".fildataf").show();
        $('.fildatad').hide();
        $(".fildata :input").prop("readonly", false);
        $("#Marital_Status").attr("readonly", false);
        $("#Religion").attr("readonly", false);
        $("#Blood_Group").attr("readonly", false);
        $("#City").attr("readonly", false);
    }
});
//
$(document).on("change", ".plt-reg-lst-data", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        data: { Id: id },
        url: "/AdminFacilities/GetPlotInformation/",
        success: function (data) {
            $('#Owner_slogn').text('Owner Information');
            $('#Nameo').text("");
            $('#Father_Nameo').text("");
            $('#CNICo').text("");
            $('#Mobile_1o').text("");
            $('#Date_Of_Birth').val("");
            $('#Address').val("");
            $('#Email').val("");
            $('#Company_Email').val("");
            $('#Mobile_2').val("");
            $('#City').val("");
            $('#Blood_Group').val("");
            $('#Religion').val("");
            $('#Marital_Status').val("");
            $('#Emergency_Contact').val("");
            $('#Relationship').val("");
            $('#Date_Of_Joining').val("");
            // filling data
            $('#Nameo').text(data.Name);
            $('#Father_Nameo').text(data.Father_Husband);
            $('#CNICo').text(data.CNIC_NICOP);
            $('#Mobile_1o').text(data.Mobile_1);
            $('.Gen__values').val(data.Id);
            if (data.Date_Of_Birth != null && data.Date_Of_Birth != "") {
                $('#Date_Of_Birth').val(moment(data.Date_Of_Birth).format("MM-DD-YYYY"));
            }
            //$('#Address').val(data.Residential_Address);
            //$('#Email').val(data.Email);
            //$('#Mobile_2').val(data.Mobile_2);
            //$('#City').val(data.City);
        },
        error: function () {
            alert('Owner Not Found');
        }
    });
});
// registration
$(document).on("submit", ".rm-fac-row", function () {
    $(this).parent().remove();
    paycont = 1;
    $('.pln-counter').each(function () {
        $(this).text(paycont + '.');
        paycont++;
    });
    paycont = 1;
    $('.add-fac-id').each(function () {
        paycont++;
        $(this).attr('id', 'add-pln-ad-' + paycont);
    });
});
//
$(document).on("click", ".del__ad__stru", function () {
    var x = confirm('Are you sure you want to delete');
    if (x) {
        var id = $(this).attr("id");
        $.ajax({
            type: 'POST',
            url: "/AdminFacilities/RemovePlan/",
            data: { Id: id },
            success: function (data) {
                if (data == true) {
                    alert('Successfully Deleted');
                    window.location.reload();
                }
                else {
                    alert('You cannot delete this fee structure');
                }
            },
            error: function () {
            }
        });
    }
});
////
$(document).on("click", ".upd__ad__stru", function () {
    //var x = confirm('Are you sure you want to update plan');
    //if (x) {
    //    var id = $(this).attr("id");
    //    $.ajax({
    //        type: 'POST',
    //        url: "/AdminFacilities/UpdatePlan/",
    //        data: { Id: id },
    //        success: function (data) {
    //            if (data == true) {
    //                alert('Successfully Updated');
    //                window.location.reload();
    //            }
    //            else {
    //                alert('You cannot delete this fee structure');
    //            }
    //        },
    //        error: function () {
    //        }
    //    });
    //}
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Fee Structure");
    $('#modalbody').load('/AdminFacilities/UpdatePlanData/', { Id: id });
});
////
$(document).on("click", ".upda___inf__fee", function () {
    var ef = $('#empmon').val();
    if (ef == null || ef == 0 || ef == "") {
        alert('Please Enter Employee Monthly Fee');
        return false;
    }
    var rm = $('#resmon').val();
    if (rm == null || rm == 0 || rm == "") {
        alert('Please Enter Resident Monthly Fee');
        return false;
    }
    var outf = $('#outmon').val();
    if (outf == null || outf == 0 || outf == "") {
        alert('Please Enter Outsider Monthly Fee');
        return false;
    }
    var datearry = [];
    var alldata =
    {
        Emp_Fee: $('#empmon').val() || 0,
        Emp_Mem_Fee: $('#empmem').val() || 0,
        Res_Fee: $('#resmon').val() || 0,
        Res_Mem_Fee: $('#Resmem').val() || 0,
        Out_Fee: $('#outmon').val() || 0,
        Out_Mem_Fee: $('#outmem').val() || 0
    };
    datearry.push(alldata);
    var x = confirm('Are you sure you want to generate fee structure');
    if (x) {
        $.ajax({
            type: "Post",
            url: '/AdminFacilities/AddProject/',
            data: { AP: datearry },
            success: function (data) {
                if (data === false) {
                    alert('Project Already Exists');
                }
                else {
                    alert('Fee Plan Generated');
                    window.location.reload();
                }
            },
            error: function () {
            }
        });
    }
});
//
$(document).on("click", ".Get__mem__info", function () {
    var id = $(this).attr('id');
    EmptyModel();
    $('#ModalLabel').text("Membership Detail");
    $('#modalbody').load('/AdminFacilities/Memberdetail/', { MemberId: id });
});
//
$(document).on("keyup", ".up__fees__admin__pro", function () {
    var Feestrucid = $(this).attr('id');
    var amt = RemoveComma($(this).val());
    $.post('/AdminFacilities/UpdateFee/', { Id: Feestrucid, Amount: amt }, function (data) {
    });
});
//
$(document).on("change", ".sub-ent-pro", function () {
    var id = $(this).val();
    var cal = 0;
    var type = $('#regType option:selected').val();
    if (type == "" || type == null) {
        alert('Please Select Member Type option');
        $(".sub-ent-pro").prop("checked", false);
        return false;
    }
    if (type == "Employee") {
        if (this.checked) { // subscribed
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
        else { // for un subscribe
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
    }
    else if (type == "Residential") {
        if (this.checked) { // subscribed
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
        else { // for un subscribe
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
    }
    else if (type == "Outsider") {
        if (this.checked) { // subscribed
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) + parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
        else { // for un subscribe
            $.post('/AdminFacilities/MembershipFee/', { Type: type, Id: id }, function (data) {
                if (data != null) {
                    cal = $('.tot-memcl').val();
                    if (cal == "" || cal == null) {
                        cal = 0;
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                    else {
                        cal = parseInt(cal) - parseInt(data);
                        $('#tot-me-fe').text(Number(cal).toLocaleString());
                        $('.tot-memcl').val(cal);
                    }
                }
            });
        }
    }
    else {
        alert('Member option not selected');
    }
});
//
$(document).on("change", ".unsub__ad__pro", function () {
    var id = $(this).val();
    if (this.checked) {
        $.post('/AdminFacilities/SubsUnsubs/', { Id: id, status: "Registered" }, function () {
            alert("Registerd");
        });
    }
    else {
        $.post('/AdminFacilities/SubsUnsubs/', { Id: id, status: "Registration_Cancelled" }, function () {
            alert("Registration Cancelled");
        });
    }
});
//
$(document).on("click", ".month__parameter", function () {
    var id = $(this).attr('id');
    var d = $('.Date_para').val()
    $('#Fit__mth').load('/AdminFacilities/MonthlyFeeParameter/', { ProjectId: id, date: d });
});
//
$(document).on("click", ".month__parameter__cashco", function () {
    var id = $(this).attr('id');
    var d = $('.Date_para').val()
    $('#Fit__mth__cash').load('/AdminFacilities/MonthlyFeeParameterCashCounter/', { ProjectId: id, date: d });
});
//
$(document).on("click", "#reg-mem", function (e) {
    e.preventDefault();
    var entarr = [];
    var flag = false;
    var trans = $('#trans-id').val();
    $('.ent-chk').each(function () {
        if ($(this).is(":checked")) {
            var id = $(this).val();
            entarr.push(id);
            flag = true;
        }
    });
    if (!flag) {
        alert('Please Select a Atleast one Subscription');
    }
    else {
        var regfiledata = "";
        var ty = $('#regType').val();
        if (ty == "" || ty == null) {
            alert('Please Select Member Type');
            return false;
        }
        if (ty == "Employee") {
            var CN = $('#CNICe').text();
            if (CN == "" || CN == null) {
                alert('Employee CNIC is Invalid or Data does not exists');
                return false;
            }
            regfiledata = {
                Type: $('#regType').val(),
                Name: $('#Namee').text(),
                Father_Name: $('#Father_Namee').text(),
                CNIC: $('#CNICe').text(),
                Mobile_1: $('#Mobile_1e').text(),
                Module_Id: $('.Gen__values').val()
            }
        }
        if (ty == "Residential") {
            var pltid = $('.plt-reg-lst-data option:selected').val();
            if (pltid == "Select Block" || pltid == null || pltid == "") {
                alert('Please Select plot no');
                return false;
            }
            var namee = $("#Nameot").val();
            if (namee == "" || namee == null) {
                alert('Please Enter Name');
                return false;
            }
            var fnamee = $("#fNameot").val();
            if (fnamee == "" || fnamee == null) {
                alert('Please Enter Father Name');
                return false;
            }
            var cnicd = $("#CNICot").val();
            if (cnicd == "" || cnicd == null) {
                alert('Please Enter CNIC');
                return false;
            }
            var cn1 = /^[0-9]{5}-[0-9]{7}-[0-9]{1}$/;
            var val1 = $("#CNICot").val();
            var res1 = cn1.test(val1)
            if (!res1) {
                alert('Please Enter valid CNIC');
                return false;
            }
            var Mobile_1d = $("#Mobile_1ot").val();
            if (Mobile_1d == "" || Mobile_1d == null) {
                alert('Please Enter Mobile 1');
                return false;
            }
            var ms2 = /^0\d{10}/;
            var val2 = $("#Mobile_1ot").val();
            var res2 = ms2.test(val2)
            if (!res2) {
                alert('Please Enter valid Mobile Number');
                return false;
            }
            regfiledata = {
                Type: $('#regType').val(),
                Name: $('#Nameot').val(),
                Father_Name: fnamee,
                CNIC: $('#CNICot').val(),
                Mobile_1: $('#Mobile_1ot').val(),
                Module_Id: $('.Gen__values').val()
            }
        }
        if (ty == "Outsider") {
            var name = $("#Name").val();
            if (name == "" || name == null) {
                alert('Please Enter Name');
                return false;
            }
            var cnic = $("#CNIC").val();
            if (cnic == "" || cnic == null) {
                alert('Please Enter CNIC');
                return false;
            }
            var cn = /^[0-9]{5}-[0-9]{7}-[0-9]{1}$/;
            var val = $("#CNIC").val();
            var res = cn.test(val)
            if (!res) {
                alert('Please Enter valid CNIC');
                return false;
            }
            var Mobile_1 = $("#Mobile_1").val();
            if (Mobile_1 == "" || Mobile_1 == null) {
                alert('Please Enter Mobile 1');
                return false;
            }
            var m = /^0\d{10}/;
            var val = $("#Mobile_1").val();
            var res = m.test(val)
            if (!res) {
                alert('Please Enter valid Mobile Number');
                return false;
            }
            regfiledata = {
                Type: $('#regType').val(),
                Name: $("#Name").val(),
                Father_Name: $("#Father_Name").val(),
                CNIC: $("#CNIC").val(),
                Residential_Address: $("#Residential_Address").val(),
                Date_Of_Birth: $("#Date_Of_Birth").val(),
                Address: $("#Address").val(),
                Mobile_1: $("#Mobile_1").val(),
                Mobile_2: $("#Mobile_2").val(),
                Email: $("#Email").val(),
                Company_Email: $("#Company_Email").val(),
                Date_Of_Birth: $("#Date_Of_Birth").val(),
                City: $("#City").val(),
                Blood_Group: $("#Blood_Group").val(),
                Emergency_Contact: $("#Emergency_Contact").val(),
                Relationship: $("#Relationship").val(),
                Module_Id: $('.Gen__values').val()
            }
        }
        var con = confirm('Are you sure you want to register this memeber?');
        if (con) {
            $.ajax({
                type: 'Post',
                url: '/AdminFacilities/RegisterMember/',
                data: { M: regfiledata, ids: entarr, TransactionId: trans },
                success: function (data) {
                    if (data == true) {
                        alert('Member successfully registerd');
                        window.location.reload();
                    }
                    if (data == false) {
                        alert('Right now you cannot register this member or the feilds are empty');
                    }
                    else if (data == 2) {
                        alert('Member successfully registered and receipt not generated because Membership Fee is 0 Rs/-');
                        window.location.reload();
                    }
                    else {
                        $.each(data.Name, function (i, item) {
                            alert('This Member is already registered in ' + data.Name[i]);
                        });
                        $.each(data.Id, function (i, item) {
                            window.open("/Receipts/MembershipReceipts?Id=" + data.Id[i] + "&Token=" + data.Token, '_blank');
                        });
                        window.location.reload();
                    }
                },
                error: function (data) {
                    alert('Error Occured');
                }
            })
        }
    }
});
//
$(document).on("click", ".mem-rece", function () {
    var Receiptid = $(this).attr('id');
    var Token = $(this).val();
    window.open("/Receipts/MembershipReceipts?Id=" + Receiptid + "&Token=" + Token, '_blank');
    window.location.reload();
});
//
$(document).on("click", ".memb-seg", function () {
    var ProId = $(this).attr('id');
    var type = $(this).attr("data-scnam");
    $('#seg-mem').load("/AdminFacilities/SegregatedMembers/", { Id: ProId, Type: type });
});
//
//
$(document).on("click", ".prnt-fee", function () {
    var feeid = $(this).attr('id');
    var Trans = $("#trans-id").val();
    $.ajax({
        type: "POST",
        url: '/AdminFacilities/FeeReceipt/',
        data: { FeeId: feeid, TransactionId: Trans },
        success: function (data) {
            if (data == false) {
                alert("Error Occured");
            }
            else if (data.Status == "1") {
                window.open("/Receipts/SportssMonthlyFee?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                window.location.reload();
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", ".mem-details", function () {
    var memid = $(this).attr('id');
    $('#m-dtl').load("/AdminFacilities/MembersDetails/", { Id: memid });
});
//// To generate Reciepts
$(document).on("click", ".mem-rece", function () {
    EmptyModel();
    var Receiptid = $(this).attr('id');
    var Amount = $(this).val();
    var id = $(this).closest('tr').attr('id');
    $('#ModalLabel').text("Membership Amount");
    $('#modalbody').load('/AdminFacilities/AmountPayModel/', { Receiptid: Receiptid, Amount: Amount, Id: id });
});
//
$(document).on("click", "#prn-mem-rec", function () {
    var recid = $('#rec-id').val();
    var tokn = $('#tok').val();
    var pamt = $('#amtpen').val();
    var recedata = {
        amount: "", amountinwords: "", bank: "", paychqno: "", paymenttype: "",
        project_name: "", branch: "", ch_bk_pay_date: "",
    };
    recedata.amount = $("#amt").val();
    recedata.amountinwords = inwords($("#amt").val());
    recedata.bank = $("#bank").val();
    recedata.paychqno = $("#paymenttype").val();
    recedata.paymenttype = $("#cah-chq-bak").val();
    recedata.project_name = $("#prj").val();
    recedata.branch = $("#branch").val();
    recedata.ch_bk_pay_date = $("#cbp-date").val();
    recedata.project_name = "Grand City";
    if (recedata.bank == null || recedata.bank == "") {
        if (recedata.amount == null || recedata.amount == "") {
            alert("amount cannot be empty");
            return false;
        }
        if (recedata.amount < pamt) {
            alert("amount cannot be less then pending amount");
            return false;
        }
    }
    $.post('/receipts/membershipreceipts/', { r: recedata, id: recid, token: tokn }, function () {
        alert("verified")
    });
});
//......................................................................SCAN IMAGE FROM SCANNER SCRIPT CODE..............................................
var imgid = '#images-';
$(document).on('click', '#scanbtn', function () {
    debugger
    imgid = '';
    imgid = '#images-';
    var v = $(this).attr('data-v');
    imgid = imgid + v;
    scanToJpg();
});
//
function scanToJpg(c) {
    debugger
    scanner.scan(displayImagesOnPage,
        {
            "output_settings": [
                {
                    "type": "return-base64",
                    "format": "jpg"
                }
            ]
        }
    );
}
/* Process the scan result */
function displayImagesOnPage(successful, mesg, response) {
    if (!successful) { // On error
        console.error('Failed: ' + mesg);
        return;
    }
    if (successful && mesg != null && mesg.toLowerCase().indexOf('user cancel') >= 0) { // User cancelled.
        console.info('User cancelled');
        return;
    }
    var scannedImages = scanner.getScannedImages(response, true, false); // returns an array of ScannedImage
    for (var i = 0; (scannedImages instanceof Array) && i < scannedImages.length; i++) {
        var scannedImage = scannedImages[i];
        processScannedImage(scannedImage);
    }
}
/* Images scanned so far */
var imagesScanned = [];
/* Processess a scanned Image */
function processScannedImage(scannedImage) {
    imagesScanned.push(scannedImage);
    var elementImg = scanner.createDomElementFromModel({
        'name': 'img',
        'attributes': {
            'class': 'scanned',
            'id': 'scanned',
            'src': scannedImage.src
        }
    });
    var elementhide = scanner.createDomElementFromModel({
        'name': 'input',
        'attributes': {
            'type': 'hidden',
            'name': 'FileImg',
            'value': scannedImage.src
        }
    });
    // document.getElementsByClassName('images').appendChild(elementImg);
    $(imgid).append(elementImg);
    $(imgid).append(elementhide);
    imgid = '';
    imgid = '#images-';
    //document.getElementById('images').appendChild(elementImg);
}
$(document).on("click", ".projectpartial", function () {
    $("#prjpartiallist").load("/ConstructProjectManagement/ProjectPartial/");
});
$(document).on("click", ".projectInventory", function () {
    $("#prjpartiallist").load("/ConstructProjectManagement/ProjectInventory/");
});
//
$(document).on("click", ".dems-ord", function () {
    $("#prjpartiallist").load("/Inventory/User_Demand_Orders/");
});
//
$(document).on("click", ".purs-ord", function () {
    $("#prjpartiallist").load("/Procurement/User_Purchase_Req/");
});
//
$(document).on("click", ".ser-ord", function () {
    $("#prjpartiallist").load("/Services/User_Service_Req/");
});
//
$(document).on("click", ".dems-req", function () {
    $("#prjpartiallist").load("/Inventory/DemandReq/");
});
//
$(':input[type=file]').change(function (event) {
    var tmppath = URL.createObjectURL(event.target.files[0]);
    var id = $(this).attr('data-id');
    //get parent using closest and then find img element
    $(this).closest("div").find("img").attr('src', tmppath);
    $(this).closest("div").find(".btn_gen__poss").show();
    //var id= $(this).closest("div").attr('id');
    // $('.Plot_id').val(id);
    //var idf= $('#file-id').val();
    // $('.file_id').val(idf);
});
//
$(document).on("click", ".btn_gen__poss", function () {
    var id = $(this).closest("div").attr('id');
    $('.Plot_id').val(id);
});
//
$(document).on("submit", "#file-imge", function (e) {
    e.preventDefault();
    var id = $(this).closest('.wrap-sfk').find('.file-trans-id').val();
    if (id == null || id == "") {
        //alert('Please Enter File Number');
        Swal.fire({
            icon: 'info',
            text: 'Enter file number to proceed'
        });
        return false;
    }
    //var idim = $(this).val();
    //$('.Plot_id').val(idim);
    $('.file_id').val(id);
    //var id = $('#file-id').val();
    var imageid = $('.Plot_id').val();
    var form = $(this);
    var data = new FormData();
    var files = "";
    if (imageid == 1) {
        //var file1 = $("#file1").get(0).files;
        files = $(this).find("#file1").get(0).files;
        if (files.length == 0) {
            return false;
        }
    }
    if (imageid == 2) {
        //var file2 = $("#file2").get(0).files;
        files = $(this).find("#file2").get(0).files;
        if (files.length == 0) {
            return false;
        }
    }
    if (imageid == 3) {
        //var file3 = $("#file3").get(0).files;
        files = $(this).find("#file3").get(0).files;
        if (files.length == 0) {
            return false;
        }
    }
    if (imageid == 4) {
        //var file4 = $("#file4").get(0).files;
        files = $(this).find("#file4").get(0).files;
        if (files.length == 0) {
            return false;
        }
    }
    // if (file1.length != 0) {
    //     files = $("#file1").get(0).files
    // }
    //else if (file2.length != 0) {
    //     files = $("#file2").get(0).files
    // }
    // else if (file3.length != 0) {
    //      files = $("#file3").get(0).files
    // }
    // else if (file4.length != 0) {
    //      files = $("#file4").get(0).files
    // }
    // else
    // {
    //     alert('kjhsdf');
    //     return false;
    // }
    var Plot_id = $(this).closest("div").find(".file_id").val();
    //var form = $("#file-imge");
    //var data = new FormData();
    //var files = "";
    //if ($("#file1").get(0).files != null) {
    //    files = $("#file1").get(0).files
    //}
    //else if ($("#file2").get(0).files != null) {
    //    files = $("#file2").get(0).files
    //}
    //else if ($("#file3").get(0).files != null) {
    //    files = $("#file3").get(0).files
    //}
    //else if ($("#file4").get(0).files != null) {
    //    files = $("#file4").get(0).files
    //}
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    //var ch = confirm('Are you sure you want to proceed');
    //if (ch) {
    Swal.fire({
        text: 'Are you sure you want to save the owner image?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: $("#file-imge").attr('action'),
                data: data,
                success: function (data) {
                    if (data == true) {
                        $(this).closest("div").find(".btn_gen__poss").hide();
                        //alert('Successfully Saved');
                        Swal.fire({
                            icon: 'success',
                            text: 'Owner image saved successfully'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
$(document).on("submit", "#plt-up-imge", function (e) {
    e.preventDefault();
    //var id = $('#file-id').val();
    var imageid = $(this).closest("div").find(".img_id").val();
    var form = $(this);
    var data = new FormData();
    var files = "";
    if (imageid == 1) {
        //var file1 = $("#file1").get(0).files;
        files = $(this).find("#file1").get(0).files;
        if (files.length == 0) {
            files = $(this).find("#file2").get(0).files;
            imageid = 2;
            if (files.length == 0) {
                files = $(this).find("#file3").get(0).files;
                imageid = 3;
                if (files.length == 0) {
                    files = $(this).find("#file4").get(0).files;
                    imageid = 4;
                    if (files.length == 0) {
                        Swal.fire({
                            icon: 'info',
                            text: 'Choose a file to save'
                            })
                        return false;
                    }
                }
            }
        }
    }
    //if (imageid == 2) {
    //    //var file2 = $("#file2").get(0).files;
    //    files = $(this).find("#file2").get(0).files;
    //    if (files.length == 0) {
    //        return false;
    //    }
    //}
    //if (imageid == 3) {
    //    //var file3 = $("#file3").get(0).files;
    //    files = $(this).find("#file3").get(0).files;
    //    if (files.length == 0) {
    //        return false;
    //    }
    //}
    //if (imageid == 4) {
    //    //var file4 = $("#file4").get(0).files;
    //    files = $(this).find("#file4").get(0).files;
    //    if (files.length == 0) {
    //        return false;
    //    }
    //}
    var Plot_id = $(this).closest("div").find(".Plot_id").val();
    $('.img_id').val(imageid);
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    //var ch = confirm('Are you sure you want to proceed to save image');
    //if (ch) {
    Swal.fire({
        text: "Are you sure you want to save the owner's image?",
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                url: $("#plt-up-imge").attr('action'),
                data: data,
                success: function (data) {
                    if (data == true) {
                        $(this).closest("div").find(".btn_gen___poss").hide();
                        //alert('Successfully Saved');
                        Swal.fire({
                            icon: 'success',
                            text: "Owner's image saved successfully"
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on("click", ".gen__cus__file", function () {
    var id = $(this).attr("data-id");
    var ch = confirm('Are you sure you want to generate Customer file');
    if (ch) {
        window.open("/FileSystem/GenerateCustomerFile?Id=" + id, '_blank');
    }
});
//
$(document).on("click", ".gen__trans__let", function () {
    var id = $(this).attr("data-id");
    var ch = confirm('Are you sure you want to generate Transfer Letter');
    if (ch) {
        window.open("/Transfer/FileTransferLetter?Id=" + id, '_blank');
    }
});
//
$(document).on("click", ".paty-cash-det", function () {
    var id = $(this).attr("id");
    window.open("/Finance/PattyCashEntryDetails?Group_Id=" + id, '_blank');
});
//
$(document).on("click", ".c-ptc", function () {
    var id = $(this).attr("id");
    window.open("/Finance/PattyCashSattlment", '_blank');
});
function showLoader() {
    $('.dark').fadeIn("slow");
}
function hideLoader() {
    $('.dark').fadeOut("slow");
}
function closeModal() {
    $('.modal-close-btn').click();
}
//
$(document).on('click', '.bid-pur-req-row', function () {
    let con = confirm("Are you sure you want to enter Quotation details of this Purchase Requisition?");
    if (con) {
        let grp = $(this).attr('id');
        window.open('/Inventory/BidsListing?Group=' + grp, '_blank');
    }
});
$(document).on('click', '.back-pur-req-row', function () {
    var reason = prompt('Reason to Send Back ', "");
    if (reason != null && reason != "") {
        let grp = $(this).attr('id');
        $.ajax({
            type: "POST",
            url: '/Procurement/UpdatePurchaseReqBack/',
            data: { Group_Id: grp, Status: 'Pending_Approval', Reason: reason },
            success: function (data) {
                alert("Request Successfull. And moved to Pending_Approval");
                $('#' + grp).remove();
                GetReqCount();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on('click', '.bid-Ser-req-row', function () {
    let con = confirm("Are you sure you want to enter Quotation details of this Service Requisition?");
    if (con) {
        let grp = $(this).attr('id');
        window.open('/Services/BidsListing?Group=' + grp, '_blank');
    }
});
// Commercial//
$(document).on("click", ".reg__shp__Floo", function (e) {
    var p = $('#Projects option:selected').val();
    var Prj_Name = $('#Projects option:selected').text();
    var f = $('.prj-phase option:selected').val();
    var flag = true;
    if (p == "" || p == null) {
        //alert('Please Select Project');
        Swal.fire({
            icon: 'info',
            text: 'Please select a project'
        });
        return false;
    }
    if (f == "" || f == null) {
        //alert('Please Select Floor');
        Swal.fire({
            icon: 'info',
            text: 'Please select a floor'
        });
        return false;
    }
    var LeV = [];
    for (var i = 1; i <= paycont; i++) {
        var LevlData = {
            Type: "", Name: "", Area: "", Plan_Id: "", Location: "", Floor_Id: "", Plan_Id: "", Com_App_Shop_Number: "", rate_sq_ft: "", ExtraAmount: "", Project_Name: "",
        };
        LevlData.Plan_Id = p;
        LevlData.Floor_Id = f;
        LevlData.Type = $("#shp-" + i + " .Type option:selected").val();
        if (LevlData.Type == 0 || LevlData.Type == "") {
            //alert('Please Enter Advance in row ' + i);
            Swal.fire({
                icon: 'info',
                text: 'Please Enter Advance for row ' + i
            });
            flag = false;
        }
        LevlData.Com_App_Shop_Number = $("#shp-" + i + " .Name").val();
        if (LevlData.Com_App_Shop_Number == 0 || LevlData.Com_App_Shop_Number == "") {
            //alert('Please Enter Shop Name in row ' + i);
            Swal.fire({
                icon: 'info',
                text: 'Please Enter unit name for row ' + i
            });
            flag = false;
        }
        LevlData.Area = $("#shp-" + i + " .Area").val();
        if (LevlData.Area == 0 || LevlData.Area == "") {
            //alert('Please Enter Total Area in row ' + i);
            Swal.fire({
                icon: 'info',
                text: 'Please Enter total area for row ' + i
            });
            flag = false;
        }
        LevlData.Location = $("#shp-" + i + " .loc").val();
        if (LevlData.Location == 0 || LevlData.Location == "") {
            //alert('Please Enter Location in row ' + i);
            Swal.fire({
                icon: 'info',
                text: 'Please Enter location for row ' + i
            });
            flag = false;
        }
        LevlData.rate_sq_ft = $("#shp-" + i + " .r__Sq__Ft").val() || 0;
        if (LevlData.rate_sq_ft == 0 || LevlData.rate_sq_ft == "") {
            //alert('Please Enter Rate/Sq Ft in row ' + i);
            Swal.fire({
                icon: 'info',
                text: 'Please Enter rate/sqft for row ' + i
            });
            flag = false;
        }
        LevlData.ExtraAmount = $("#shp-" + i + " #Amount").val() || 0;
        LevlData.Project_Name = Prj_Name;
        LeV.push(LevlData);
    }
    if (flag) {
        $.ajax({
            type: "POST",
            url: '/Commercial/AddShopCommercial/',
            data: { cr: LeV },
            success: function (data) {
                if (data == true) {
                    //alert('Successfully Registerd');
                    Swal.fire({
                        icon: 'success',
                        text: 'Unit(s) registered successfully'
                    }).then(() => {
                        window.location.reload();
                    })
                }
                else {
                    $.each(data, function (i, item) {
                        //alert(data[i] + ' Already Exists');
                        Swal.fire({
                            icon: 'info',
                            text: data[i] + ' Already Exists'
                        });
                    });
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    //alert("got timeout");
                    Swal.fire({
                        icon: 'error',
                        text: 'Request timed out'
                    });
                } else {
                    //alert(textstatus);
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            }
        });
    }
});
// building management mapping
$(document).on("click", ".ad__shp", function () {
    Swal.fire({
        text: 'Do you want to add another Unit?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            paycont++;
            var html = '<div class="form-row cal add-s-id" id="shp-' + paycont + '" ><h6 class="lh-1 mB-0 pln-counter">' + paycont + '.</h6>' +
                ' <div class="form-group col-md-2">' +
                '<select class="form-control Type" id="type" data-id="' + paycont + '">' +
                '<option value="Shop">Shop</option>' +
                '<option value="Apartment">Apartment</option>' +
                ' <option value="Office">Office</option>' +
                ' </select>' +
                ' </div>' +
                '<div class="form-group col-md-2">' +
                '<input type="text" id="" name="" class="form-control Name" data-id="' + paycont + '">' +
                '</div>' +
                '<div class="form-group col-md-1">' +
                '<input type="text" id="" name="" class="form-control Area" data-id="' + paycont + '">' +
                '</div>' +
                '<div class="form-group col-md-2">' +
                '<input type="text" id="" name="" class="form-control loc" data-id="' + paycont + '">' +
                '</div>' +
                '<div class="form-group col-md-2">' +
                '<input type="text" id="" name="" class="form-control r__Sq__Ft" data-id="' + paycont + '">' +
                '</div>' +
                '<div class="form-group col-md-2">' +
                '<input class="form-control coma" placeholder="250,000" required data-id="' + paycont + '">' +
                '<input type="hidden" id="Amount" class="amt" required>' +
                //'<input type="text" id="" name="" class="form-control Ex__Amt" data-id="' + paycont + '">' +
                '</div>' +
                '<i class="ti-minus rm_s_row pointer" style="margin-left:3%" ></i></div>';
            $('#ad-shp').append(html);
        }
    });
});
////
$(document).on("click", ".rm_s_row", function () {
    Swal.fire({
        text: 'Are you sure you want to discrad the Unit?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $(this).parent().remove();
            paycont = 1;
            $('.pln-counter').each(function () {
                $(this).text(paycont + '.');
                paycont++;
            });
            paycont = 1;
            $('.add-s-id').each(function () {
                paycont++;
                $(this).attr('id', 'shp-' + paycont);
            });
        }
    });
});
//// Commercial List
$(document).on("click", ".com-lst__upda", function () {
    var id = $(this).attr("id");
    $('#res-com-lst__up').load('/Commercial/CommercialManagementDetails/', { Id: id })
});
//
$(document).on("click", ".upda__comm", function () {
    var id = $(this).attr('id');
});
//
$(document).on("click", ".del__comm", function () {
    var id = $(this).attr('id');
    Swal.fire({
        text: 'Are you sure you want to delete the commercial unit?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Commercial/CommercialManagementDelete/', { Id: id }, function (data) {
                if (data == false) {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    })
                }
                else {
                    Swal.fire({
                        icon: 'success',
                        text: 'Commercial unit deleted successfully'
                    }).then(() => {
                        $('#sp__comm tbody tr#' + id + '').remove();
                    })
                }
            });
        }
    });
});
// Get Blocks information with Phase of Projects
$(document).on("change", ".prj-com-flo", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: "/RealEstatePhases/GetCommercialFloor/",
        data: { Id: id },
        success: function (data) {
            $('#floor').html(' ');
            $('#floor').append('<option>Select Floor</option>');
            $.each(data, function (key, value) {
                $("#floor").append('<option value=' + value.Id + '>' + value.Floor + '</option>');
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// building management mapping
$(document).on("click", ".ad-an-str", function () {
    paycont++;
    var html = '<div class="form-row cal id__str" id="str-' + paycont + '" ><h6 class="lh-1 mB-0 pln-counter">' + paycont + '.</h6>' +
        ' <div class="form-group col-md-3">' +
        '<select class="form-control Type" id="type" data-id="' + paycont + '">' +
        '<option value="1">Advance</option>' +
        '<option value="2">Installment</option>' +
        '<option value="3">Grand Installment</option>' +
        ' </select>' +
        ' </div>' +
        '<div class="form-group col-md-2">' +
        '<input type="text" class="form-control caladva" id="com__adv" name="Advance" placeholder="%" data-id="' + paycont + '">' +
        '</div>' +
        '<div class="form-group col-md-1">' +
        '<input type="text" class="form-control" id="com__remain" name="After" placeholder="" data-id="' + paycont + '">' +
        '</div>' +
        '<div class="form-group col-md-2">' +
        '<input type="text" class="form-control" id="com__insta" name="Installments" data-id="' + paycont + '">' +
        '</div>' +
        '<i class="ti-minus rm_s_row__com pointer" style="margin-left:3%" ></i></div>';
    $('#ad-ins__stru').append(html);
});
////
$(document).on("click", ".rm_s_row__com", function () {
    $(this).parent().remove();
    paycont = 1;
    $('.pln-counter').each(function () {
        $(this).text(paycont + '.');
        paycont++;
    });
    paycont = 1;
    $('.id__str').each(function () {
        paycont++;
        $(this).attr('id', 'str-' + paycont);
    });
})
//
// Create File Installment Structure
$(document).on("click", "#cr__inst__str__Comm", function (e) {
    var p = $("#plan__name").val();
    if (p == null || p == "") {
        alert('Enter Plan Name');
        return false;
    }
    var Total = 0;
    $(".caladva").each(function () {
        var ded = $(this).val();
        if (ded != "" && ded != null) {
            var coma = RemoveComma(ded);
            Total = (Total) + parseInt(coma);
        }
    });
    if (Total != 100) {
        alert('Plan Percentage should be 100 %');
        return false;
    }
    var flag = true;
    var tycheck = 0;
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Plan_Name: "", Advance: "", Installments: "", Duration: "", After: "", Type: "", TypeName: "",
        };
        recedata.Plan_Name = $("#plan__name").val();
        recedata.Type = $("#str-" + i + " .Type option:selected").val();
        tycheck = recedata.Type;
        recedata.Advance = $("#str-" + i + " #com__adv").val();
        if (recedata.Advance == 0 || recedata.Advance == "") {
            alert('Please Enter Percentage in row' + i);
            flag = false;
        }
        recedata.After = $("#str-" + i + " #com__remain").val() || 0;
        if (recedata.After == 0 || recedata.After == "") {
            alert('Please Enter After in row' + i);
            flag = false;
        }
        recedata.Installments = $("#str-" + i + " #com__insta").val() || 0;
        if (recedata.Installments == 0 || recedata.Installments == "") {
            alert('Please Enter Installments in row' + i);
            flag = false;
        }
        recedata.TypeName = $("#str-" + i + " .Type option:selected").text();
        allrecepts.push(recedata);
    }
    var alldata = {
        I_S: allrecepts,
    };
    if (flag == true) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Installments/CreateCommercialInstallmentStruc/',
            data: JSON.stringify(alldata),
            success: function (data) {
                if (data) {
                    alert("Installment Structure Added");
                    window.location.reload();
                }
                else {
                    alert("Installment Structure already exist");
                }
            },
            error: function () {
                alert("Error Occured Try Again")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
    else {
    }
});
//
////
$(document).on("click", ".Del__Stru", function () {
    var id = $(this).attr('id');
    $.post('/Commercial/DeleteInstallmentStructure/', { PlanId: id }, function (data) {
        if (data == false) {
            alert("You cannot delete this plan");
        }
        else {
            alert('Successfully Deleted');
            Window.location.reload();
        }
    });
});
// Get Blocks information with Phase of Projects
$(document).on("change", ".prj-shp", function () {
    var id = $(this).val();
    var pid = $("#Projects").val();
    var prj = $('#Projects option:selected').text();
    $.ajax({
        type: "POST",
        url: "/RealEstateBlocks/GetShops/",
        data: { Id: id, proid: pid },
        success: function (data) {
            $('#shp').html(' ');
            $('#shp').append('<option>Select Unit</option>');
            if (prj == "SA Premium Homes") {
                $.each(data, function (key, value) {
                    $("#shp").append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
                });
            }
            else {
                $.each(data, function (key, value) {
                    $("#shp").append('<option value=' + value.Id + '>' + value.Com_App_Shop_Number + '</option>');
                });
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on("change", ".com__inst__get", function () {
    var id = $(this).val();
    if (id == "") {
        id = 0;
    }
    $('.stru__map').load('/Commercial/InstallmetsCategoryCommercial/', { Id: id })
});
//
$(document).on("click", "#sea-com-file", function () {
    var proj = $("#Projects").val();
    var projName = $("#Projects option:selected").text();
    var app = $("#shp").val();
    var unit = $("#shp option:selected").text();
    $.ajax({
        type: "POST",
        url: "/Commercial/SearchComFile/",
        data: { ProjectId: proj, FormId: app },
        success: function (data) {
            if (data == false) {
                //alert("No Result Found");
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: "No Record Found"
                });
                return false;
            }
            if (data.length <= 0) {
                alert("No Result Found");
                return false;
            }
            if (data.Status != "Registered") {
                $('.btn__reg__com').prop('disabled', false);
            }
            $("#sts__div").show();
            $('#get__stat').text(data.Status.replace(/_/g, ' '));
            $("#area").val(data.Area);
            $("#type").val(data.Type);
            $("#loc").val(data.Location);
            $("#com-id").val(data.Comercial_Id);
            $("#app-fil-id").val(data.Com_App_Shop_Number);
            $("#floor__name").val(data.Floor);
            $('#qr_img').attr("src", '/QR Codes/' + data.Comercial_Id + '_' + data.Floor_Id + '.png');
            var totval = data.Final_Rate;
            var adv = Math.round((totval))
            var bok = (totval);
            $("#adv").val(Number.toLocaleString(adv));
            $("#bok").val(bok);
            $("#adv-pmt").text(Number(adv).toLocaleString());
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: "Request timed out"
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: 'Something went wrong'
                });
            }
        }
    });
    $.ajax({
        type: "POST",
        url: "/Commercial/SearchComReceipts/",
        data: { ProjName: projName, Unit: unit },
        success: function (data) {
            if (data == false) {
                //alert("No Result Found");
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: "No Record Found"
                });
                return false;
            }
            $('#receipt-data').parent().show();
            if (data.length > 0) {
                var html1 = '<table class="table table-striped table-bordered" style="margin-bottom:0px" cellspacing="0" width="100%"><thead class="thead-dark">' +
                    '<tr><th width="10">Sr.</th><th width="80">Receipt No.</th><th width="150">Unit No.</th><th width="120">Floor</th><th width="250">Name</th><th width="250">Father Name</th><th width="200">Contact</th><th width="120">Amount</th><th width="150">Token Date</th><th width="150">Expiry Date</th>' +
                    '<div style="min-height:0px; max-height:400px;overflow:auto"><table class="table table-striped table-bordered" cellspacing="0" width="100%"><thead class="thead-dark"><tbody></tbody></table></div>';
                $('#receipt-data').html(html1);
                var srcount = 1;
                $(data).each(function (i) {
                    var html = '<tr data-id="' + data[i].Id + '"><td width="10">' + srcount + '</td><td width="80">' + data[i].ReceiptNo + '</td><td width="150">' + data[i].File_Plot_Number + '</td><td width="120">' + data[i].Block + '</td><td width="250">' + data[i].Name + '</td><td width="250">' + data[i].Father_Name + '</td><td width="200">' + data[i].Contact + '</td><td width="120">' + data[i].Amount.toLocaleString() + '</td><td width="150">' + moment(data[i].DateTime).format("DD-MMM-YYYY") + '</td><td width="150">' + moment(data[i].Token_Exp_Date).format("DD-MMM-YYYY") + '</td></tr>';
                    $('#receipt-data tbody').append(html);
                    srcount++;
                });
            }
            else {
                var html = '<h6>No Result Found</h6>';
                $('#receipt-data').html(html);
            }
            $('#receipt-data').html(html);
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: "Request timed out"
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    //title: 'Error!',
                    text: 'Something went wrong'
                });
            }
        }
    });
});
//
$(document).on("change", "#Comm__Allotment__Letter", function () {
    var id = $("#plt-id").val();
    if (this.checked) {
        $.post('/Commercial/AllotmentLetterStatus/', { Id: id, Status: 1 }, function () {
            //alert("Requested")
            Swal.fire({
                icon: 'success',
                text: 'Requested for verification successfully'
                })
        });
    } else {
        $.post('/Commercial/AllotmentLetterStatus/', { Id: id, Status: 0 }, function () {
        });
    }
});
//
$(document).on("click", ".fi-reg-ath", function () {
    Swal.fire({
        //title: 'Are you sure you want to Add New Owner?',
        text: 'Are you sure you want to add transfer owner for this property?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            plotowncount++;
            if (plotowncount <= 6) {
                var html2 =
                    ' <hr /><div class="add-reg-id" id="add-reg-' + plotowncount + '" >' +
                    '<i class="ti-trash rm-reg-fl   pointer" style="float:right;"></i>' +
                    '<div class="form-row">' +
                    //'<div class="form-row col-md-12">' +
                    //'<div class="form-group  own-det" style="margin-left:85%"  id="' + plotowncount + '">' +
                    //'<img style="margin-top:0px" src="/assets/static/images/no-img.png" width="200" height="180" data-id="' + plotowncount + '" id="own_img" name="Owner_Image1" />' +
                    //'<input type="hidden"  data-id="' + plotowncount + '" id="image" name="Image" required />' +
                    //'<input type="button" class="btn btn-info own-img add-own-img" id="add-own-img1" style="margin-top:10px;" data-id="' + plotowncount + '" data-toggle="modal" value="Upload Image" data-target="#Modal" />' +
                    //'</div>' +
                    //'</div>' +
                    '<h6 class=" mB-0 add-reg-counter">' + plotowncount + '.</h6>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Name</label>' +
                    '<input type="text" class="form-control Name" id="Name" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Fathers/Husbands Name</label>' +
                    '<input type="text" class="form-control Father_Husband" id="Father_Husband" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>CNIC / NICOP</label>' +
                    '<input type="text" class="form-control" id="CNIC_NICOP" pattern="[0-9+]{5}-[0-9+]{7}-[0-9]{1}|[0-9+]{6}-[0-9+]{6}-[0-9]{1}" placeholder="12345-1234567-1   or   123456-123456-1" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>Date Of Birth</label>' +
                    '<input type="text" class="form-control" id="Date_Of_Birth" data-provide="datepicker" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-6">' +
                    '<label>Postal Address</label>' +
                    '<input type="text" class="form-control" id="Postal_Address" placeholder="1234 Main St" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-6">' +
                    '<label>Residential Address</label>' +
                    '<input type="text" class="form-control Residential_Address" id="Residential_Address" placeholder="Apartment, Plot, or floor" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>Select City</label>' +
                    '<select class="form-control city" data-id="' + plotowncount + '"></select>' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Occupation</label>' +
                    '<input type="text" class="form-control" id="Occupation" name="" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>Nationality</label>' +
                    '<input type="text" class="form-control" id="Nationality" name="" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>Email</label>' +
                    '<input type="text" class="form-control" id="Email" name="" data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Phone Office</label>' +
                    '<input type="text" class="form-control" id="Phone_Office" name="" data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Residential</label>' +
                    '<input type="text" class="form-control" id="Residential" name="" data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Mobile 1</label>' +
                    '<input type="text" class="form-control" id="Mobile_1"  placeholder="03121234567"  data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-3">' +
                    '<label>Mobile 2</label>' +
                    '<input type="text" class="form-control" id="Mobile_2"  placeholder="03121234567" name="" data-id="' + plotowncount + '">' +
                    '</div>' +
                    '</div>' +
                    '<h6 class="c-grey-900">Nominee</h6>' +
                    '<div class="">' +
                    '<div class="form-row">' +
                    '<div class="form-group col-md-3">' +
                    '<label>Name</label>' +
                    '<input type="text" class="form-control" id="Nominee_Name" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>CNIC / NICOP</label>' +
                    '<input type="text" class="form-control" pattern="[0-9+]{5}-[0-9+]{7}-[0-9]{1}|[0-9+]{6}-[0-9+]{6}-[0-9]{1}" placeholder="12345-1234567-1 or 123456-123456-1" id="Nominee_CNIC_NICOP" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-2">' +
                    '<label>Relation</label>' +
                    '<input class="form-control" id="Nominee_Relation" placeholder="" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '<div class="form-group col-md-5">' +
                    '<label>Address</label>' +
                    '<input type="text" class="form-control" id="Nominee_Address" placeholder="1234 Main St" required data-id="' + plotowncount + '">' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                $('#ad-reg-form').append(html2);
                Initcity(plotowncount);
            }
            else {
                //alert('You cannot enter more then 6 owners');
                Swal.fire({
                    icon: 'info',
                    text: 'More than 6 owners are not can not be addedd'
                });
            }
        }
    });
});
//
$(document).on("submit", "#re-com", function (e) {
    e.preventDefault();
    var advamt = parseFloat(RemoveComma($("#adv-pmt").text()));
    var totalamt = 0
    $(".amt").each(function () {
        totalamt = parseFloat(totalamt) + parseFloat($(this).val());
    });
    //if (advamt != totalamt) {
    //    alert("Amount is not equal to Total Amount");
    //    return false;
    //}
    var token = $("#book-stat").is(":checked")
    var plan = $("#InstallmentStruct").val();
    var fulpaid = $("#full__pay__Commer").is(":checked");
    var fileno = $("#app-num").val();
    var Disamt = $("#dis__amt").val();
    if (Disamt > 100) {
        //alert('Discount Amount Should be less then 100 %')
        Swal.fire({
            icon: 'info',
            text: 'The discount amount should be less than 100%'
            })
        return false;
    }
    var img = $("#own_img").attr('src');
    var flag = true;
    for (var i = 1; i <= paycont; i++) {
        var cash_che_bank = $("#pay-" + i + " #cah-chq-bak").val();
        if (cash_che_bank !== "Cash") {
            flag = false;
        }
    }
    var allrecepts = []
    for (var i = 1; i <= paycont; i++) {
        var recedata = {
            Amount: "", AmountInWords: "", Bank: "", PayChqNo: "", PaymentType: "",
            Project_Name: "", Branch: "", Ch_bk_Pay_Date: "", Block: "", FileImage: ""
        };
        recedata.Amount = $("#pay-" + i + " #Amount").val();
        recedata.AmountInWords = InWords($("#pay-" + i + " #Amount").val());
        recedata.Bank = $("#pay-" + i + " #bank").val();
        recedata.PayChqNo = $("#pay-" + i + " #paymenttype").val();
        recedata.PaymentType = $("#pay-" + i + " #cah-chq-bak").val();
        recedata.Project_Name = $("#Project option:selected").text();
        recedata.Branch = $("#pay-" + i + " #Branch").val();
        recedata.Ch_bk_Pay_Date = $("#pay-" + i + " #cbp-date").val();
        recedata.FileImage = $("#pay-" + i + " #scanned").attr('src');
        recedata.Block = $("#floor").val();
        allrecepts.push(recedata);
    }
    var Name = "", Postal_Address = "", Phone_Office = "", Residential = "", Mobile_1 = "", Mobile_2 = "", Email = "", Occupation = "", Nationality = "", Date_Of_Birth = "", CNIC_NICOP = "", Nominee_Name = "",
        Nominee_Relation = "", Nominee_Address = "", Nominee_CNIC_NICOP = "", Owner_Image2 = "", Owner_Image3 = "", Owner_Image4 = "", Block_Id = "", QR_Code = "", City = "", Residential_Address = "", Father_Husband = "";
    for (var r = 1; r <= plotowncount; r++) {
        Name = Name + 'Ѭ' + ($("#add-reg-" + r + " .Name").val() || '_');
        Father_Husband = Father_Husband + 'Ѭ' + ($("#add-reg-" + r + "  .Father_Husband").val() || '_');
        Postal_Address = Postal_Address + 'Ѭ' + ($("#add-reg-" + r + " #Postal_Address").val() || '_');
        Residential_Address = Residential_Address + 'Ѭ' + ($("#add-reg-" + r + "  .Residential_Address").val() || '_');
        Phone_Office = Phone_Office + 'Ѭ' + ($("#add-reg-" + r + " #Phone_Office").val() || '_');
        Residential = Residential + 'Ѭ' + ($("#add-reg-" + r + " #Residential").val() || '_');
        Mobile_1 = Mobile_1 + 'Ѭ' + ($("#add-reg-" + r + " #Mobile_1").val() || '_');
        Mobile_2 = Mobile_2 + 'Ѭ' + ($("#add-reg-" + r + " #Mobile_2").val() || '_');
        Email = Email + 'Ѭ' + ($("#add-reg-" + r + " #Email").val() || '_');
        Occupation = Occupation + 'Ѭ' + ($("#add-reg-" + r + " #Occupation").val() || '_');
        Nationality = Nationality + 'Ѭ' + ($("#add-reg-" + r + " #Nationality").val() || '_');
        Date_Of_Birth = Date_Of_Birth + 'Ѭ' + ($("#add-reg-" + r + " #Date_Of_Birth").val() || '_');
        CNIC_NICOP = CNIC_NICOP + 'Ѭ' + ($("#add-reg-" + r + " #CNIC_NICOP").val() || '_');
        Nominee_Name = Nominee_Name + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Name").val() || '_');
        Nominee_Relation = Nominee_Relation + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Relation").val() || '_');
        Nominee_Address = Nominee_Address + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_Address").val() || '_');
        Nominee_CNIC_NICOP = Nominee_CNIC_NICOP + 'Ѭ' + ($("#add-reg-" + r + " #Nominee_CNIC_NICOP").val() || '_');
        City = City + 'Ѭ' + ($("#add-reg-" + r + " .city").val() || '_');
        if (r == 2) {
            Owner_Image2 = $("#add-reg-" + r + " #own_img").attr('src');
        }
        if (r == 3) {
            Owner_Image3 = $("#add-reg-" + r + " #own_img").attr('src');
        }
        if (r == 4) {
            Owner_Image4 = $("#add-reg-" + r + " #own_img").attr('src');
        }
    }
    var regfiledata = {
        Name: Name.substr(1),
        Father_Husband: Father_Husband.substr(1),
        Postal_Address: Postal_Address.substr(1),
        Residential_Address: Residential_Address.substr(1),
        Phone_Office: Phone_Office.substr(1),
        Residential: Residential.substr(1),
        Mobile_1: Mobile_1.substr(1),
        Mobile_2: Mobile_2.substr(1),
        Email: Email.substr(1),
        Occupation: Occupation.substr(1),
        City: City.substr(1),
        Nationality: Nationality.substr(1),
        Date_Of_Birth: Date_Of_Birth.substr(1),
        CNIC_NICOP: CNIC_NICOP.substr(1),
        Nominee_Name: Nominee_Name.substr(1),
        Nominee_Relation: Nominee_Relation.substr(1),
        Nominee_Address: Nominee_Address.substr(1),
        Nominee_CNIC_NICOP: Nominee_CNIC_NICOP.substr(1),
        OwnerCount: plotowncount,
        Owner_Image1: $("#own_img").attr('src'),
        Owner_Image2: Owner_Image2,
        Owner_Image3: Owner_Image3,
        Owner_Image4: Owner_Image4,
        ComRom_Id: $("#com-id").val(),
    }
    var alldata = {
        comdata: regfiledata,
        Flag: flag,
        ComRom_Num: $('.com_shp__da option:selected').text(),
        Receiptdata: allrecepts,
        Token: token,
        PlanId: plan,
        FullPaid: fulpaid,
        Image: img,
        DisAmt: Disamt,
        TotalAmnt: advamt,
    };
    var con = confirm("Are you sure you want to Generate Receipt");
    var ch = $("#full__pay__Commer").is(":checked");
    if (ch == true) {
        //var con1 = confirm('Installment Plan will not apply because you checked Full payment');
         var con1 = Swal.fire({
         icon: 'info',
         text: 'The installment plan will not be applied as you have selected full payment'
     });
    }
    else {
        con1 = true;
    }
    if (con1) {
        //var con = confirm("Are you sure you want to Generate Receipt");
        //if (con) {
        Swal.fire({
            text: 'Are you sure you want to generate the Receipt?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "POST",
                    url: $(this).attr("action"),
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(alldata),
                    success: function (data) {
                        if (data == false) {
                            //alert('Error Occured Or Installment Plan is invalid');
                            Swal.fire({
                                icon: 'error',
                                text: 'Something went wrong'
                            });
                        }
                        else if (data.Status == "1") {
                            //alert("Successfully Registerd");
                            Swal.fire({
                                icon: 'success',
                                text: 'Unit successfully registered'
                            }).then(() => {
                                $(data.Receiptid).each(function (i) {
                                    window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid[i] + "&Token=" + data.Token, '_blank');
                                });
                                //window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                                window.location.reload();
                            });
                        }
                        else if (data.Status == "2") {
                            //alert("Wait Until You Payment is Clear from Bank");
                            Swal.fire({
                                icon: 'info',
                                text: 'Kindly wait until your payment has been successfully processed by the bank'
                            }).then(() => {
                                window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                                window.location.reload();
                            });
                        }
                    }
                    , error: function (xmlhttprequest, textstatus, message) {
                        if (textstatus === "timeout") {
                            //alert("got timeout");
                            Swal.fire({
                                icon: 'error',
                                text: 'Request timed out'
                            });
                        } else {
                            //alert(textstatus);
                            Swal.fire({
                                icon: 'error',
                                text: 'Something went wrong'
                            });
                        }
                    }
                });
            }
        });
    }
});
//
function Initcity(i) {
    $("#add-reg-" + i + " .city").append('<option>Select City</option>');
    $.each(cities, function (key, value) {
        $("#add-reg-" + i + " .city").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}
// Search the File information
$(document).on("submit", "#sea-com-info", function (e) {
    e.preventDefault();
    var seaopt = $('input[name=SearchOption]').val();
    $.ajax({
        type: "POST",
        url: $('#sea-com-info').attr("action"),
        data: $('#sea-com-info').serialize(),
        success: function (data) {
            $('#tab-data-com').html(data);
            Adjusttable();
        },
        error: function (data) {
        }
    });
});
////
//$(document).on("click", "#sea-Com-info", function () {
//    var seaopt = $('input[name=SearchOption]:checked').val();
//    var ser = parseInt(seaopt);
//    var Text = $('input[name=Text]').val();
//    $('#tab-data-com').load('/Commercial/SearchResult/', { Text: Text, SearchOption: ser });
//    //$.post("/Commercial/SearchResult/", { Text: Text, SearchOption: seaopt }, function (data) {
//    //    if (data == false) {
//    //        var html = '<h6>No Result Found</h6>';
//    //        $('#tab-data').html(html);
//    //    }
//    //});
//});
//
$(document).on("click", ".get-com-roomdata", function () {
    var comid = $(this).attr('id');
    SASLoad('#com-det');
    $('#com-det').load('/Commercial/CommercialInformationSearch/', { Commercial_Id: comid }, function () {
        SASUnLoad('#com-det');
    });
});
$(document).on("change", ".Get__tran__dat", function () {
    var id = $(this).val();
    $("#Shop__Tran__data").load('/Commercial/GetTranferData/', { COmId: id });
});
//
$(document).on("click", ".all__rec_veri__COm", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    $.post('/Installments/VerifyReceipt/', { Id: id, Status: chkstat }, function (data) {
        var allrec = $('.all__rec_veri__COm').length;
        var checkrec = $('.all__rec_veri__COm:checked').length;
        if (allrec == checkrec) {
            $('#ver__all__com').show();
            $('#ver__Com').show();
        }
        else {
            $('#ver__all__com').hide();
            $('#ver__Com').hide();
        }
    }).fail(function () {
        alert("error occured Try Again");
    });
});
//
//
$(document).on("click", "#ver__all__com", function () {
    var id = $("#shp-id").val();
    $.ajax({
        type: "POST",
        url: '/Commercial/Verification/',
        data: { Id: id },
        success: function (data) {
            alert("Verified");
            $("#ver__all__com").prop("Disabled", true);
        },
        error: function () {
            alert("Error Occured");
        }
    });
});
//
$(document).on("click", "#App__trans__comm", function () {
    var ownerrowid = $('.Own_id').val();
    var trnsrowid = $('.owtransid').val();
    var comid = $("#shp-id").val();
    $.post('/Commercial/TranferRecored/', { Ownerid: ownerrowid, TransferId: trnsrowid, ComId: comid }, function (data) {
        if (data == true) {
            alert('Successfully Transferd');
            Window.location.reload();
        }
        else {
        }
    });
});
//
$(document).on("click", ".rem-req-pur", function () {
    var id = $(this).data("id");
    if (confirm("Are you sure you want to remove this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Procurement/DeletePurchaseReq/',
            data: { GroupId: id },
            success: function (data) {
                $("#" + id).remove();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".dem-rem-req", function () {
    var id = $(this).data("id");
    if (confirm("Are you sure you want to remove this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/DeleteDemandReq/',
            data: { Group_Id: id },
            success: function (data) {
                $("#" + id).remove();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".rem-serv", function () {
    var id = $(this).data("id");
    if (confirm("Are you sure you want to remove this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Services/DeleteServiceReq/',
            data: { GroupId: id },
            success: function (data) {
                $("#" + id).remove();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".rem-oe", function () {
    var id = $(this).data("id");
    if (confirm("Are you sure you want to remove this requisition")) {
        $.ajax({
            type: "POST",
            url: '/Services/DeleteOE/',
            data: { GroupId: id },
            success: function (data) {
                $("#" + id).remove();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on('click', '.rem-item', function () {
    var res = $(this).parent().parent().parent().remove();
});
//
$(document).on('click', '#ad-dm-req', function () {
    var html = `<div class="form-row assign" id="assign-1">
                        <div class="form-group col-md-1" style="display: block;">
                            <label class="sr ser">1.</label>
                        </div>
                        <div class="form-group col-md-4" style="display: block;">
                            <label>Item</label>
                            <input type="text" class="form-control itm-req" />
                        </div>
                        <div class="form-group col-md-2" style="display: block;">
                            <label>Req Qty</label>
                            <input type="text" class="form-control rq-qty" />
                        </div>
                        <div class="form-group col-md-4" style="display: block;">
                            <label>Details</label>
                            <input type="text" class="form-control details" />
                        </div>
                        <div class="form-group col-md-1" style="display: block;">
                            <label class="ser"><i class="fas fa-trash-alt rem-item"></i></label>
                        </div>
                    </div>`
    $('#inv-assign-row').append(html);
    resetsr();
});
//
$(document).on("click", ".sv-dem-re", function () {
    var itemsData = [];
    var grp = $("#grp-id").val();
    $('.assign').each(function () {
        let itemnam = $(this).find('.itm-req').val();
        let depId = $('.dep-se').val();
        let qty = $(this).find('.rq-qty').val();
        let det = $(this).find('.details').val();
        itemsData.push({
            Item_Name: itemnam,
            Qty: qty,
            Details: det,
            ReqTo_Dep: depId,
        });
    });
    if (confirm("Are you sure you want to Request the Demand")) {
        $.ajax({
            type: "POST",
            url: "/Inventory/AddDemandReq/",
            contentType: "application/json",
            traditional: true,
            beforeSend: function () {
            },
            complete: function () {
            },
            data: JSON.stringify({ items: itemsData, Group_Id: grp }),
        }).done(function (data) {
            if (data.Status == true) {
                alert(data.Msg);
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
//
$(document).on('click', '.ser-po-inv', function () {
    let po = $(this).closest('tr').attr('id');
    let grp = $(this).closest('tr').data('grp');
    window.open("/Procurement/PO_Details?Group_Id=" + grp + "&po=" + po, '_blank');
});
//
$(document).on('click', '.ser-wo-inv', function () {
    let po = $(this).closest('tr').attr('id');
    let grp = $(this).closest('tr').data('grp');
    window.open("/Procurement/WO_Details?Group_Id=" + grp + "&po=" + po, '_blank');
});
$(document).on("change", ".slwhd", function () {
    var a = $(this).val();
    switch (a) {
        case 's':
            $(this).closest('.item-pur-req-row').find(".s-div").show();
            break;
        case 'l':
            $(this).closest('.item-pur-req-row').find(".l-div").show();
            break;
        case 'w':
            $(this).closest('.item-pur-req-row').find(".w-div").show();
            break;
        case 'h':
            $(this).closest('.item-pur-req-row').find(".h-div").show();
            break;
        case 'd':
            $(this).closest('.item-pur-req-row').find(".d-div").show();
            break;
        case 'b':
            $(this).closest('.item-pur-req-row').find(".b-div").show();
            break;
        case 'sp':
            $(this).closest('.item-pur-req-row').find(".sp-div").show();
            break;
    }
});
$(document).on("click", ".l-cros", function () {
    $(this).closest('.item-pur-req-row').find('.l-div').hide();
});
$(document).on("click", ".w-cros", function () {
    $(this).closest('.item-pur-req-row').find('.w-div').hide();
});
$(document).on("click", ".h-cros", function () {
    $(this).closest('.item-pur-req-row').find('.h-div').hide();
});
$(document).on("click", ".d-cros", function () {
    $(this).closest('.item-pur-req-row').find('.d-div').hide();
});
$(document).on("click", ".s-cros", function () {
    $(this).closest('.item-pur-req-row').find('.s-div').hide();
});
$(document).on("click", ".b-cros", function () {
    $(this).closest('.item-pur-req-row').find('.b-div').hide();
});
$(document).on("click", ".sp-cros", function () {
    $(this).closest('.item-pur-req-row').find('.sp-div').hide();
});
//
$(document).on("click", ".prc-com", function () {
    var id = $(this).closest('tr').attr('id');
    if (confirm("Are you sure you want to Process the Commission")) {
        $.ajax({
            type: "POST",
            url: "/Dealership/Process_Commession/",
            data: { Id: id },
        }).done(function (data) {
            if (data.Status == true) {
                alert(data.Msg);
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
//
$(document).on('click', '.oijk', function () {
    let conf = confirm('Are you sure you want to remove this item?');
    if (conf) {
        $(this).closest('tr').remove();
    }
});
$(document).on('click', '.jkgfhs', function () {
    var returnItems = [];
    var fnl = true;
    var transid = $('#trans-id').val();
    var depid = $('#dep-id').val();
    var depnam = $('#dep-name').val();
    var prjId = $('#prj-id').val();
    var prjNm = $('#prj-name').val();
    var inno = $('#in-no').val();
    var inid = $('#in-id').val();
    $('.agfs').each(function () {
        let itemId = $(this).attr('data-item');
        let itemnam = $(this).find('.it-nam').text();
        let orgQty = $(this).find('.orig').val();
        let newQty = $(this).find('.qty').val() || 0;
        let whId = $(this).find('.warehouse option:selected').val();
        let whText = $(this).find('.warehouse option:selected').text();
        if (parseFloat(newQty) > parseFloat(orgQty)) {
            alert('Cannot return item in more quantity than issued');
            fnl = false;
            return;
        }
        returnItems.push({
            item: itemId,
            Dep_Id: depid,
            Dep_Name: depnam,
            Prj_Id: prjId,
            Prj_Name: prjNm,
            issueId: inid,
            issueNum: inno,
            qtyRet: newQty,
            Warehouse_Id: whId,
            Warehouse_Name: whText
        });
    });
    if (!fnl) {
        return;
    }
    if (confirm("Are you sure you want to add these items to inventory")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/ReturnItems/',
            data: { item: returnItems, TransactionId: transid },
            success: function (data) {
                if (data.Status) {
                    alert('Items return successfully');
                    window.open('/Inventory/ReturnNotePrint?Id=' + data.GRNS);
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function (data) {
                alert("An Error occured Try Again");
            }
        });
    }
});
$(document).on("click", ".pl-ld", function () {
    var id = $(this).closest("tr").attr("id");
    var type = $(this).closest("tr").data("type");
    if (type == "Lead") {
        window.open("/Leads/LeadDetails?Id=" + id, "_blank")
    }
    else if (type == "Deal") {
        window.open("/PropertyDeal/DealDetails?Id=" + id, "_blank")
    }
})
//
$(document).on("click", "#day-close", function () {
    var data = [];
    $('.tik-entry').each(function () {
        var t = $(this).is(":checked");
        if (t) {
            var val = $(this).closest('tr').attr('id');
            data.push(val)
        }
    });
    if (confirm("Are you sure you want to Day Close")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/EntriesSupervision/',
            data: { Grp: data },
            success: function (data) {
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
//
$(document).on("click", ".sea-ldg", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var code = $("#h-code").val();
    var head = $("#h-head").val();
    $(".hd-led").load("/AccountHeads/HeadLedger/", { From: from, To: to, Code: code, Head: head });
});
//
$(document).on("click", ".prj-req", function () {
    var id = $("#Prj_Id").val();
    var req = $(this).data("req");
    if (req == undefined || req == null) {
        window.open('/Inventory/NewPurchaseRequisition?Id=' + id);
    }
    else {
        window.open('/Inventory/NewPurchaseRequisition?Id=' + id + '&Req=' + req);
    }
});
// Req For Other
$(document).on("click", ".prj-req-others", function () {
    var id = $("#Prj_Id").val();
    var req = $(this).data("req");
    if (req == undefined || req == null) {
        window.open('/Inventory/NewPurchaseRequisitionForOther?Id=' + id);
    }
    else {
        window.open('/Inventory/NewPurchaseRequisitionForOther?Id=' + id + '&Req=' + req);
    }
});
//Purchase Req For Others
//$(document).on("click", ".prj-req-for-other", function () {
//    
//    var id = $("#Prj_Id").val();
//    var req = $(this).data("req");
//    if (req == undefined || req == null) {
//        window.open('/Inventory/NewPurchaseRequisitionForOthers?Id=' + id);
//    }
//    else {
//        window.open('/Inventory/NewPurchaseRequisitionForOthers?Id=' + id + '&Req=' + req);
//    }
//});
//
$(document).on("click", ".ser-m-req", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/NewServiceRequisition?Id=' + id);
});
//
$(document).on("click", ".ser-req", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/NewService_Requisition?Id=' + id);
});
//ser req for other
$(document).on("click", ".ser-req-other", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/NewService_Requisition_For_Other?Id=' + id);
});
//
$(document).on("click", "#Waveinst", function () {
    var amt = $('#loan-inst-amt').val();
    var loandt = $('#loan-inst-date').val();
    var reason = $('#reasonwaveoff').val();
    var reason = reason + ' - Instalment of ' + loandt + ' of Amount ' + amt;
    $.ajax({
        type: "POST",
        url: '/Loan/SaveLoanWaiveRequest/',
        data: { instId: $('#instid').val(), reason: reason },
        success: function (data) {
            if (data.Status) {
                alert("Requested");
                $('#Waveinst').prop('disabled', true);
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", ".dail-rec-sea-res", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    $("#report").load("/Banking/SearchDailyCashReport/", { From: from, To: to });
});
$(document).on("click", ".day-close-rec-sea-res", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    $("#report").load("/Banking/SearchDayClosedCashReport/", { From: from, To: to });
});
$(document).on("click", ".day-close-rec-sam", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    $("#report-load").load("/Banking/DayClosedSamCashReceiptView/", { From: from, To: to });
});
$(document).on('click', '#shft-inst-pln-r', function () {
    var filenum = $('#plt-no').text();
    var id = $("#plt-id").val();
    var mod = $("#module").val();
    //if (confirm("Are you sure you want to Shift installments to next Month ")) {
    Swal.fire({
        text: 'Are you sure you want to shift installments to the next month?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Installments/ShiftRightInstallments/',
                data: { Id: id, Module: mod },
                success: function (data) {
                    if (data) {
                        //alert("Updated");
                        Swal.fire({
                            icon: 'success',
                            text: 'Installments shifted to the next month successfully'
                        });
                        //$('.file-pymt-detls').load('/FileSystem/FilePymntDetail/', { FileId: filenum }, function () {
                        //    SASUnLoad($('.file-pymt-detls'));
                        //});
                    }
                    else {
                        //alert("error");
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
//
$(document).on('click', '#shft-inst-pln-l', function () {
    var filenum = $('#plt-no').text();
    var id = $("#plt-id").val();
    var mod = $("#module").val();
    //if (confirm("Are you sure you want to Shift installments to Previous Month ")) {
    Swal.fire({
        text: 'Are you sure you want to shift installments to the previous month?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Installments/ShiftLeftInstallments/',
                data: { Id: id, Module: mod },
                success: function (data) {
                    if (data) {
                        //alert("Updated");
                        Swal.fire({
                            icon: 'success',
                            text: 'Installments shifted to the previous month successfully'
                        });
                        //$('.file-pymt-detls').load('/FileSystem/FilePymntDetail/', { FileId: filenum }, function () {
                        //    SASUnLoad($('.file-pymt-detls'));
                        //});
                    }
                    else {
                        //alert("error");
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
//
$(document).on("submit", "#gen-subs-recovery", function (e) {
    e.preventDefault();
    $('#gen-subs-recovery').attr("disabled", true);
    $("#amt-in-wrds").val(InWords($("#amt").val()));
    var img = $("#scanned").attr('src');
    $("#scanimage").val(img);
    if ($("#amt").val() <= 0) {
        return false;
    }
    var mod = $('#Module option:selected').val();
    if (mod == "" || mod == null) {
        alert("Please Select Company");
        return false;
    }
    var con = confirm("Are you sure you want to Generate Receipt");
    if (con) {
        $.ajax({
            type: "POST",
            url: $("#gen-subs-recovery").attr('action'),
            data: $("#gen-subs-recovery").serialize(),
            success: function (data) {
                window.open("/Receipts/SubsidiaryRecovery?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
                $('#gen-subs-recovery').attr("disabled", false);
            }
        });
    }
});
$(document).on("click", "#assi-new-lead", function () {
    var salesLeads = [];
    var leadCountTotal = $('.totleads').data('counter');
    var leadsCounter = 0;
    $('.salesLeads').each(function () {
        salesLeads.push({
            SalesPersonId: Number($(this).attr('data-id')),
            LeadsCount: Number($(this).find('.userleadcount').val())
        });
        leadsCounter = leadsCounter + Number($(this).find('.userleadcount').val())
    });
    if (leadsCounter > leadCountTotal) {
        //alert("Cannot Assign more than uploaded leads");
        Swal.fire({
            icon: 'info',
            text: 'Cannot assign more than available leads'
        });
        return false;
    }
    //var con = confirm("Are you sure you want to Assign Leads");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to assign leads?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/PropertyDeal/AssignLeadsToSales/',
                dataType: "json",
                data: { SalesLeads: salesLeads },
                success: function (data) {
                    //alert('Leads Successfully Assigned');
                    Swal.fire({
                        icon: 'success',
                        text: 'Leads assigned successfully'
                    });
                    window.location.reload();
                },
                error: function () {
                    //alert('Error Occured');
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
$(document).on("click", ".req-token", function () {
    var id = $("#l-id").val();
    var amount = Number($("#tok_amt").val());
    if (amount < 500 || amount == "") {
        alert("Amount cannot be empty or less than 500");
        return false;
    }
    var roomId = $('#prj_unit option:selected').val();
    var con = confirm("Are you sure you want to Request for Token");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/PropertyDeal/RequestReceiptPayment/',
            data: { Id: id, Amount: amount, unitId: roomId },
            success: function (data) {
                alert("Requested");
                window.location.reload();
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on("change", ".prj-floor", function () {
    var id = $(this).val();
    var prj = $('.prj-floor option:selected').text();
    if (prj != "Select Floor") {
        $.ajax({
            type: "POST",
            url: "/PropertyDeal/GetUnits/",
            data: { Id: id },
            success: function (data) {
                $('#prj_unit').html(' ');
                $('#prj_unit').append('<option value="-1">Select Unit</option>');
                $.each(data, function (key, value) {
                    $("#prj_unit").append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
                });
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on("submit", "#up-prem-lead", function (e) {
    e.preventDefault();
    $('#ld-up-stat').attr("disabled", true);
    $.ajax({
        type: "POST",
        url: $("#up-prem-lead").attr('action'),
        data: $("#up-prem-lead").serialize(),
        success: function (data) {
            alert("Updated");
            window.location.reload();
        },
        error: function () {
            alert("Error Occured");
            $('#ld-up-stat').attr("disabled", false);
            $('#gen-plot-rec').attr("disabled", false);
        }
    });
});
$(document).on("click", ".prem-pay-req", function (e) {
    e.preventDefault();
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("SA Premium Homes Booking Token");
    $('#modalbody').load('/PropertyDeal/CommercialProjectReceipt/', { Id: id });
});
$(document).on("click", "#gen-prem-rec", function (e) {
    e.preventDefault();
    var id = $('#id').val();
    var paytype = $('#cah-chq-bak option:selected').val();
    var instNo = $('#paymenttype').val();
    var instDate = $('#cbp-date').val();
    var bank = $('#bank').val();
    var branch = $('#Branch').val();
    var trans = $('#trans').val();
    $.ajax({
        type: "POST",
        url: "/PropertyDeal/CommecialProjectReceiptAdd/",
        data: { Id: id, PaymentType: paytype, InstNo: instNo, InstDate: instDate, Bank: bank, Branch: branch, TransactionId: trans },
        success: function (data) {
            if (data == false) {
                //alert('Error Occured');
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            }
            else if (data.Status == "1") {
                //alert("Successfully Registerd");
                Swal.fire({
                    icon: 'success',
                    text: 'Successfully registered'
                }).the(() => {
                    window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                    window.location.reload();
                })
            }
            else if (data.Status == "2") {
                //alert("Wait Until You Payment is Clear from Bank");
                Swal.fire({
                    icon: 'info',
                    text: 'Kindly await confirmation of your payment clearance from the bank'
                }).then(() => {
                    window.open("/Receipts/CommercialReceipt?Id=" + data.Receiptid + "&Token=" + data.Token, '_blank');
                    window.location.reload();
                })
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                //alert("got timeout");
                Swal.fire({
                    icon: 'error',
                    text: 'Request timed out'
                });
            } else {
                //alert(textstatus);
                Swal.fire({
                    icon: 'error',
                    text: 'Something went wrong'
                });
            }
        }
    });
});
$(document).on("click", ".lead-prem-uns-search", function () {
    var from = $("#startdate").val();
    var to = $("#enddate").val();
    $('#homedetails').load('/PropertyDeal/PremiumUnassignedLeads/', { From: from, To: to });
});
$(document).on("click", ".lead-prem-assign", function () {
    EmptyModel();
    $('.modal-body').load('/PropertyDeal/AssignNewLeads/', function () { });
});
$(document).on("click", "#addNewLead", function () {
    EmptyModel();
    $('#ModalLabel').text("Add New Lead");
    $('#modalbody').load('/PropertyDeal/PremiumHomeLead/', function () { });
});
$(document).on("click", "#up-inv-sa-prem", function (e) {
    var prj = "SA Premium Homes";
    $.each(AllData, function (i, v) {
        v.Rate = $.trim(RemoveComma(v.Rate));
    });
    if (confirm("Are you sure you want to Upload the Inventory")) {
        $.ajax({
            type: "POST",
            url: '/PropertyDeal/UploadInventory/',
            data: { AllData: AllData, PrjName: prj },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
                }
                else {
                    alert(data.Msg)
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on("click", ".upl-prem-inv", function () {
    EmptyModel();
    $('#modalbody').load('/PropertyDeal/AddNewInventory/');
});
$(document).on("change", ".prj-shp-sa-prem", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: "/PropertyDeal/GetUnits/",
        data: { Id: id },
        success: function (data) {
            $('#shp').html(' ');
            $('#shp').append('<option>Select Unit</option>');
            $.each(data, function (key, value) {
                $("#shp").append('<option value=' + value.Id + '>' + value.ApplicationNo + '</option>');
            });
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on("change", ".flr_com_str", function () {
    var id = $(this).val();
    $.ajax({
        type: "POST",
        url: "/Installments/GetCommercialUnitPrice/",
        data: { Id: id },
        success: function (data) {
            $('.unit_amt').val(data.toLocaleString());
            $('.rem_amt').val(data.toLocaleString());
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on("change", "#Project", function () {
    var prj = $("#Project option:selected").text();
    if (prj == "Grand City") {
        $('.Block').show();
        $('.Offer_Price').show();
        $('.Source').show();
        $('.status').show();
        $('.Plot_size').show();
    }
    else {
        $('.Block').hide();
        $('.Offer_Price').show();
        $('.Source').show();
        $('.status').show();
        $('.Plot_size').hide();
    }
});
$(document).on("change", ".leadUnits", function () {
    var id = $('.leadUnits option:selected').val();
    $.ajax({
        type: "POST",
        url: '/PropertyDeal/GetUnitData/',
        data: { Id: id },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $("#fil-plt").val(data.Type);
                $('#plt-no').text(data.ApplicationNo);
                $('#blk-no').text();
                $('#plt-size').text(data.Total_SqFt_Marlas + " sq.ft");
                $('#plt-size-n').val(data.Total_SqFt_Marlas);
                $('#plt-type').text(data.Type);
                $('#plt-status').text();
                $('#plt-loc').text(data.Location);
                var html = "";
                if (data.Status == "Repurchased") {
                    html = '<div class="alert alert-info" style="text-align:center" role="alert">This Plot is Repurchased by Company</div>';
                }
                else if (data.Status == "Available_For_Sale") {
                    html = '<div class="alert alert-info" style="text-align:center" role="alert">This Plot is Available For Sale</div>';
                }
                else if (data.Status == "Disputed") {
                    html = '<div class="alert alert-warning" style="text-align:center" role="alert">This Plot is Disputed </div>';
                }
                else if (data.Status == "CancelledDueAmount") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Cancelled Due to Over Due Amount</div>';
                }
                else if (data.Status == "Cancelled") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Cancelled By Company</div>';
                }
                else if (data.Status == "Temporary_Cancelled") {
                    html = '<div class="alert alert-danger" style="text-align:center" role="alert">This Plot is Temporary Cancelled By Company</div>';
                }
                else if (data.Status == "Hold") {
                    html = '<div class="alert alert-dark" style="text-align:center" role="alert">This Plot is Holded by Company</div>';
                }
                $("#stat").html(html);
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
$(document).on("click", ".unassigned_lead_up", function () {
    var pr = $('#Prj').val();
    EmptyModel();
    $('#ModalLabel').text("Upload Leads");
    $('#modalbody').load('/Leads/UploadLeads', { Project: pr, status: "UnAssigned" });
});
$(document).on('click', "#ints-add", function (e) {
    e.preventDefault();
    var amt = RemoveComma($("#Amount").val());
    var datee = $("#date").val();
    var mod = $("#module").val();
    var type = $(".inst-type option:selected").val();
    var nam = $(".inst-type option:selected").text();
    //var con = confirm("Are you sure you want to Add Installment Row");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to add the installment?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Installments/AddInstallmentPlann",
                data: { Amount: amt, Date: datee, Type: type, id: $("#plt-id").val(), Name: nam, Module: mod },
                success: function (data) {
                    //alert("added");
                    Swal.fire({
                        icon: 'success',
                        text: 'Installment added successfully'
                    });
                    window.location.reload(true);
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
//
$(document).on('click', '.installment-plan-row-add', function () {
    EmptyModel();
    $('#ModalLabel').text("Manual Installment Plan");
    $('#modalbody').load('/Installments/AddInstallmentRow/', function () { });
});
$(document).on('click', '.prnt-ltr-fdsjkhl', function () {
    if ($(this).hasClass('dplct')) {
        if (!confirm('This intimation letter has already been printed. Are you sure you want to print again?')) {
            return;
        }
    }
    else {
        $(this).removeClass('fa-envelope').addClass('fa-check animate__animated animate__fadeInUp animate__slower');
    }
    let vtag = $(this).attr('data-vtag');
    window.open('/Balloting/IntimationLetter?id=' + vtag);
});
$(document).on('click', '.com__inst__btn', function () {
    EmptyModel();
    var shp = $('#shp-id').val();
    var own = $('#OwnId').val();
    $('#ModalLabel').text("Commercial Installments");
    $('#modalbody').load('/Installments/CommercialInstallment/', { ShopId: shp, OwnId: own }, function () { });
});
$(document).on('click', '.add_new_emp_bank', function () {
    EmptyModel();
    $('#ModalLabel').text("Add Bank Account of an Employee");
    $('#modalbody').load('/Salaries/AddNewBankConfig/', function () { });
});
$(document).on('click', '.com_desc', function () {
    var id = $(this).data("plot");
    EmptyModel();
    $('#ModalLabel').text("Add Discount");
    var html = '<form><div class="row"><div class="col-md-2"><label>Total Amount</label><input class="form-control coma" id="dis-ttl-amt dis-cal-file" placeholder="12,345" required><input type="hidden" id="dis-ttl-amt-val"  name="Total_Amount" class="amt" required></div>' +
        '<div class="col-md-2"><label>Discount Amount <b><span id="percent"></span></b></label><input id="dis-amt" class="form-control coma dis-cal-file" placeholder="12,345" required><input type="hidden" id="dis-amt-val" name="Discount_Amount" class="amt" required></div>' +
        '<div class="col-md-3"><label>Discount Date</label><input id="dis-date" class="form-control" data-provide="datepicker" placeholder="mm/dd/yyyy" required></div>' +
        '<div class="col-md-5"><label>Remarks</label><textarea class="form-control" id="remrk" name="Remarks"></textarea></div></div></form>';
    $('#modalbody').html(html);
    $('.modal-footer').append('<button class="btn btn-primary add_disc_comm" data-unit="' + id + '" type="button">Save</button>');
});
$(document).on("click", ".add_disc_comm", function (e) {
    e.preventDefault();
    var ttl = $("#dis-ttl-amt-val").val();
    var dis = $("#dis-amt-val").val();
    var rem = $("#remrk").val();
    var id = $(this).data("unit");
    var disDate = $('#dis-date').val();
    //var con = confirm("Are you sure you want to add Discount");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to add discount?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Commercial/CommercialUnitDiscount/',
                data: { TotalAmt: ttl, DiscountAmt: dis, Remarks: rem, Id: id, DiscountDate: disDate },
                success: function (data) {
                    if (data) {
                        //alert("Discount Added");
                        Swal.fire({
                            icon: 'success',
                            text: 'Discount added successfully'
                        })
                    }
                    else {
                        //alert("Error. Try Again Later");
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                },
                error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        $('#gen-rec').attr("disabled", false);
                        //alert("got timeout");
                        Swal.fire({
                            icon: 'error',
                            text: 'Request timed out'
                        });
                    } else {
                        $('#gen-rec').attr("disabled", false);
                        //alert(textstatus);
                        Swal.fire({
                            icon: 'error',
                            text: 'Something went wrong'
                        });
                    }
                }
            });
        }
    });
});
$(document).on("click", "#ser-arr-date", function () {
    var month = $(".sel-date").val();
    $("#res").load('/ArrearsSalaries/SearchArrears/', { From: month, Status: "Received" });
});
$(document).on('click', '#ch-inst-plan', function () {
    EmptyModel();
    var size = $('#pl-size').val();
    $('#ModalLabel').text("Manual Installment Plan");
    $('#modalbody').load('/Installments/UpdateInstallmentPlans/', { Plot_Size: size }, function () {
    });
});
$(document).on("click", "#up-inst-plan", function () {
    if (!$("input[type='radio'].intst-id").is(':checked')) {
        //alert("Please Select a Plan");
        Swal.fire({
            icon: 'info',
            text: 'Select a plan to proceed'
        });
        return false;
    }
    var id = $("input[type='radio'].intst-id:checked").val();
    var plt_id = $("#plt-id").val();
    var mod = $("#module").val();
    //if (confirm("Are you sure you want to update the Installment Plan")) {
    Swal.fire({
        text: 'Are you sure you want to update the installment plan?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: '/Installments/UpdateInstallmentPlan/',
                data: { Id: id, File_Id: plt_id, Module: mod },
                success: function (data) {
                    if (data.Status) {
                        Swal.fire({
                            icon: 'success',
                            text: 'Installment plan updated successfully'
                        }).then(() => {
                            window.location.reload();
                        })
                    }
                    else {
                        window.location.reload();
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                    $('#gen-rec').attr("disabled", false);
                }
            });
        }
    });
});
$(document).on('click', '#com-file-del', function () {
    var ownId = $('#OwnId').val();
    var comid = $('#shp-id').val();
    //var con = confirm("Are you sure you want to Mark this File Delivered?");
    //if (con) {
    Swal.fire({
        text: 'Are you sure you want to confirm the delivered status for this file?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Commercial/MarkCommercialFileDelivered/",
                data: { OwnerId: ownId, Commercial_Id: comid },
                success: function (data) {
                    if (data == true) {
                        $("#com-det").load("/Commercial/CommercialInformationSearch/", { Commercial_Id: comid }, function () {
                            $('html, body').animate({
                                scrollTop: $("#com-det").offset().top
                            }, 1000);
                        });
                    }
                },
                error: function () {
                    //alert("Error Occured");
                    Swal.fire({
                        icon: 'error',
                        text: 'Something went wrong'
                    });
                }
            });
        }
    });
});
$(document).on("click", ".mat-rec-rep", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    $("#report").load("/Reports/SearchMaterialReceivingReport/", { From: from, To: to });
});
$(document).on("click", ".p-tran-lett", function (e) {
    var id = $(this).data("ownid");
    //if (confirm("Are you sure you want to Generate Letter")) {
    Swal.fire({
        text: 'Are you sure you want to generate transfer letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            window.open("/Transfer/PlotsTransferLetter?Id=" + id, '_blank');
        }
    });
});
$(document).on("click", ".c-tran-lett", function (e) {
    var id = $(this).data("ownid");
    //if (confirm("Are you sure you want to Generate Letter")) {
    Swal.fire({
        text: 'Are you sure you want to generate the transfer Letter?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            window.open("/Transfer/CommercialTransferLetter?Id=" + id, '_blank');
        }
    });
});
$(document).on('click', '.rmv-emp-bank-account', function () {
    var EmpId = $(this).data('empid');
    if (confirm("Are you sure you want to delete Employee Bank Account?")) {
        $.ajax({
            type: "POST",
            url: "/Salaries/RemoveEmployeeBankAccount/",
            data: { EmployeeId: EmpId },
            success: function (data) {
                if (data == true) {
                    window.location.reload();
                }
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
$(document).on("click", ".ser-dev-req", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/NewServiceRequisition_SC?Id=' + id);
});
$(document).on('click', '.rece-refund-pop-file', function () {
    EmptyModel();
    var id = $(this).closest('tr').attr('id');
    var num = $("#" + id + " .rece-num").text();
    var amount = $("#" + id + " .rece-amt").text();
    var type = $("#" + id + " .rece-type").text();
    var html = '<div class="row"><input type="hidden" value="' + id + '" id="receipt-id">' +
        '<div class="form-group col-md-2"><label>Receipt No.</label><h6>' + num + '</h6></div>' +
        '<div class="form-group col-md-2"><label>Amount</label><h6 id="rec-amt">' + amount + '</h6></div><div class="form-group col-md-2">' +
        '<label>Type</label><h6 id="ded-amt">' + type + '</h6></div><div class="form-group col-md-3"><label>Amount To Refund</label><input class="form-control coma" placeholder="12,345" required><input type="hidden" id="amt" name="Amount" class="ref-amt amt" required></div></div>';
    $('#modalbody').html(html);
    $('#ModalLabel').text("Refund Amount");
    $('.modal-footer').append('<button class="btn btn-success" id="ref-amt-file" type="submit">Request for Refund</button>');//ref-amt-plt
});
$(document).on("click", "#ref-amt-file", function () {
    var pltid = 0;
    var id = $('#receipt-id').val();
    var refamt = $('.ref-amt').val();
    if (refamt <= 0 || refamt == '' || refamt == null) {
        $('.coma').focus();
        return false;
    }
    var mod = $('#module').val();
    if (mod == "PlotManagement") {
        pltid = $("#plt-id").val();
    }
    else if (mod == "Building") {
        pltid = $("#shp-id").val();
    }
    else if (mod == "FileManagement") {
        pltid = $("#file-id").val();
    }
    var receamt = RemoveComma($('#rec-amt').text());
    if (Number(receamt) < Number(refamt)) {
        alert("Refund Amount should be less than or Equal to Receipt Amount")
        return false;
    }
    var inp = confirm("Are you sure you want to Refund this Receipt");
    if (inp == true) {
        $.post("/Installments/RefundAmountPlot/", { Id: id, Amount: refamt }, function (data) {
            if (data.Status) {
                if (mod == "PlotManagement") {
                    window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                }
                else if (mod == "Building") {
                    window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                }
                else if (mod == "FileManagement") {
                    window.open("/Receipts/RefundReceipt?Id=" + data.ReceiptId + "&Token=" + data.Token, '_blank');
                }
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
$(document).on("click", ".file-ref-det", function () {
    var id = $(this).attr("id");
    var pltid = $(this).attr("data-pltid");
    window.location = "/FileSystem/FileReceiptRefundDetails?FileNo=" + pltid + "&ReqId=" + id;
});
// View For Finance File Cancelation
$(document).on("click", ".frd-fil-ref-f", function () {
    EmptyModel();
    var canid = $("#can-req-id").val();
    var pltid = $("#plt-id").val();
    $('#ModalLabel').text("Refund Amount");
    $('#modalbody').load('/FileSystem/RefundAmount/', { Id: canid });
    $('.modal-footer').append('<button class="btn btn-success" id="ref-plot-f-btn" type="button">Refund Amount</button>');
});
$(document).on('click', '.month-due-ser', function () {
    var mon = $('#months option:selected').val();
    var yea = $('#year option:selected').val();
    SASLoad($('#tab-table1'));
    $('#tab-table1').load('/Reports/MonthlyDue', { month: mon, year: yea }, function () {
        SASUnLoad($('#tab-table1'));
    });
});
//
//
//   Land Module Start
//
//
$(document).on("click", '.add-form-row', function () {
    var addrr = `<div class="col-md-12 bgc-white">
<div class="row mt-4">
        <div class="col-md-2"></div>
        <div class="col-md-8 oth-use-rmv-land add-land-info"style="margin-top: -2%;">
            <fieldset class="scheduler-border">
                <div class="row mt-md-2">
                    <div class="col-md-4">
                        <label>Moza No.</label>
                        <input type="text" class="form-control moza" />
                    </div>
                    <div class="col-md-4 ">
                        <label>Marraba No.</label>
                        <input type="text" class="form-control mar" />
                    </div>
                    <div class="col-md-4">
                        <label>Khasra No.</label>
                        <input type="text" class="form-control khasra" />
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-md-3">
                        <h4 style="margin-top:29px">Area Bought</h4>
                    </div>
                    <div class="col-md-3">
                        <label>Kanal</label>
                        <input type="text" class="form-control kanal" />
                    </div>
                    <div class="col-md-3">
                        <label>Marla</label>
                        <input type="text" class="form-control marla" />
                    </div>
                    <div class="col-md-3">
                        <label>SFT.</label>
                        <input type="text" class="form-control squareft" />
                    </div>
                </div>
                <hr />
                <div class="row use-for-rmv-row">
                    <div class="col-md-3">
                        <h6>Khewat</h6>
                        <input type="text" class="form-control khewat" />
                    </div>
                    <div class="col-md-3">
                        <h6>Khatoni</h6>
                        <input type="text" class="form-control khatooni" />
                    </div>
                    <div class="col-md-2"style="padding-top: 4.3%;">
                        <i class="fas fa-plus-square fa-lg pointer add-form-row"></i>
                        <i class="fas fa-minus-square fa-lg pointer ml-2 rmv-land-row"></i>
                    </div>
            </fieldset>
        </div>
        <div class="col-md-2"></div>
    </div> 
</div>`
    $('.add-for-append').append(addrr);
});
$(document).on('click', '.add-seller-btn', function () {
    EmptyModel();
    $('#ModalLabel').text("Add New Seller");
    $('#modalbody').load('/Land/SellerData/');
});
$(document).on('click', '#add-seller-data', function () {
    var Seller_Name = $('#seller-name').val();
    var CNIC = $('#id-card-no').val();
    var Mobile_No = $('#mobile-no').val();
    if (Seller_Name == "" || CNIC == "" || Mobile_No == "") {
        alert("Fill the all fields before add new seller")
        return false;
    }
    else {
    }
    if (confirm("Are you sure you want to add new seller")) {
        $.ajax({
            type: "POST",
            url: '/Land/NewLandSeller/',
            data: { Seller_Name: Seller_Name, CNIC: CNIC, Mobile_No: Mobile_No },
            success: function (data) {
                if (data.Status == true) {
                    alert("New Seller Added Successfully");
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on('click', '.Update-deal-wth-dtl', function () {
    LandDataArray = [];
    DateAmount = [];
    var dealid = $('.del-id').val();
    var Seller_Select_Name = $('.seller_Name').val();
    var Sellect_Seller_Id = $('.seller_id').val();
    var Posession_Status = $('.poss').val();
    var Purchase_Price_Acer = $('.pur-price').val();
    var Deal_No = $('.del-no').val()
    if (Seller_Select_Name == '' || Seller_Select_Name == null) {
        alert("Please select a Land Seller");
        return false;
    }
    $('.add-land-info').each(function () {
        var Moza_No = $(this).find('.moza').val();
        var Marrabba_No = $(this).find('.mar').val();
        var Khasra_No = $(this).find('.khasra').val();
        var Kanal_No = $(this).find('.kanal').val();
        var Marla_No = $(this).find('.marla').val();
        var Square_ft = $(this).find('.squareft').val();
        var Khewat_No = $(this).find('.khewat').val();
        var Khatooni_No = $(this).find('.khatooni').val();
        var Littigation = $('.lit').val();
        var Remarks = $('.rem').val();
        if (Moza_No == '' || Moza_No == null) {
            alert("Please give Moza No");
            return false;
        }
        if (Marrabba_No == '' || Marrabba_No == null) {
            alert("Please give Marabba No");
            return false;
        }
        if (Khasra_No == '' || Khasra_No == null) {
            alert("Please give Khasra No");
            return false;
        }
        if (Kanal_No == '' || Kanal_No == null) {
            alert("Please give Kanal No");
            return false;
        }
        if (Marla_No == '' || Marla_No == null) {
            alert("Please give Marla No");
            return false;
        }
        if (Square_ft == '' || Square_ft == null) {
            alert("Please give Square Fit No");
            return false;
        }
        if (Khewat_No == '' || Khewat_No == null) {
            alert("Please give Khewat No");
            return false;
        }
        if (Khatooni_No == '' || Khatooni_No == null) {
            alert("Please give Khatooni No");
            return false;
        }
        else {
        }
        LandDataArray.push({
            Moza_No: Moza_No,
            Marraba_No: Marrabba_No,
            Khasra_No: Khasra_No,
            Kannal: Kanal_No,
            Marla: Marla_No,
            SFT: Square_ft,
            Khewat: Khewat_No,
            Khatoni: Khatooni_No,
            Litigation: Littigation,
            Remarks: Remarks
        });
    });
    $('.due-amt').each(function () {
        var Due_Date = $(this).find('.due-date').val();
        var Amount = RemoveComma($(this).find('.amt').val());
        if (Due_Date == '' || Due_Date == null) {
            alert("Please Select Valid Due Date");
            return false;
        }
        if (Amount == null || Amount == '' || Amount <= 0) {
            alert("Please Add Valid Amount");
            return false;
        }
        DateAmount.push({
            Due_Date: Due_Date,
            Amount: Amount
        });
    });
    if (confirm("Are you really want to save land deal")) {
        $.ajax({
            type: "POST",
            url: '/Land/UpdateLandRecord/',
            data: { deal_id: dealid, LandRecordData: LandDataArray, LandPaymentDateAmt: DateAmount, SelectSeller_Name: Seller_Select_Name, sellerId: Sellect_Seller_Id, PurchasePrice_Acer: Purchase_Price_Acer, PosessionStatus: Posession_Status, DeaLNo: Deal_No },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg)
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on('click', '.save-deal-wth-dtl', function () {
    LandDataArray = [];
    DateAmount = [];
    var Seller_Select_Name = $('.sel-lst option:selected').text();
    var Sellect_Seller_Id = $('.sel-lst option:selected').val();
    var Posession_Status = $('.poss').val();
    var Purchase_Price_Acer = $('.pur-price').val();
    var Deal_No = $('.del-no').val()
    if (Seller_Select_Name == '' || Seller_Select_Name == null) {
        alert("Please select a Land Seller");
        return false;
    }
    $('.add-land-info').each(function () {
        var Moza_No = $(this).find('.moza').val();
        var Marrabba_No = $(this).find('.mar').val();
        var Khasra_No = $(this).find('.khasra').val();
        var Kanal_No = $(this).find('.kanal').val();
        var Marla_No = $(this).find('.marla').val();
        var Square_ft = $(this).find('.squareft').val();
        var Khewat_No = $(this).find('.khewat').val();
        var Khatooni_No = $(this).find('.khatooni').val();
        var Littigation = $('.lit').val();
        var Remarks = $('.rem').val();
        if (Moza_No == '' || Moza_No == null) {
            alert("Please give Moza No");
            return false;
        }
        if (Marrabba_No == '' || Marrabba_No == null) {
            alert("Please give Marabba No");
            return false;
        }
        if (Khasra_No == '' || Khasra_No == null) {
            alert("Please give Khasra No");
            return false;
        }
        if (Kanal_No == '' || Kanal_No == null) {
            alert("Please give Kanal No");
            return false;
        }
        if (Marla_No == '' || Marla_No == null) {
            alert("Please give Marla No");
            return false;
        }
        if (Square_ft == '' || Square_ft == null) {
            alert("Please give Square Fit No");
            return false;
        }
        if (Khewat_No == '' || Khewat_No == null) {
            alert("Please give Khewat No");
            return false;
        }
        if (Khatooni_No == '' || Khatooni_No == null) {
            alert("Please give Khatooni No");
            return false;
        }
        LandDataArray.push({
            Moza_No: Moza_No,
            Marraba_No: Marrabba_No,
            Khasra_No: Khasra_No,
            Kannal: Kanal_No,
            Marla: Marla_No,
            SFT: Square_ft,
            Khewat: Khewat_No,
            Khatoni: Khatooni_No,
            Litigation: Littigation,
            Remarks: Remarks
        });
    });
    $('.due-amt').each(function () {
        var Due_Date = $(this).find('.due-date').val();
        var Amount = RemoveComma($(this).find('.amt').val());
        if (Due_Date == '' || Amount == '') {
            alert("Please Select Due Date And Add Amount");
            return false;
        }
        DateAmount.push({
            Due_Date: Due_Date,
            Amount: Amount
        });
    });
    if (confirm("Are you really want to save land deal")) {
        $.ajax({
            type: "POST",
            url: '/Land/SaveLandDealDetails/',
            data: { LandRecordData: LandDataArray, LandPaymentDateAmt: DateAmount, SelectSeller_Name: Seller_Select_Name, sellerId: Sellect_Seller_Id, PricePerAcre: Purchase_Price_Acer, PosessionStatus: Posession_Status, DeaLNo: Deal_No },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg)
                }
            },
            error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on("click", '.rmv-land-row', function () {
    $(this).closest('.use-for-rmv-row').closest('.oth-use-rmv-land').remove();
});
$(document).on('click', '.edt_recrd', function () {
    var id = $(this).data("dealid");
    window.open("/Land/LandRecordDetails?dealid=" + id, '_blank');
});
$(document).on('click', '.new-land-rec', function () {
    window.open('/Land/AddLandRecord/');
});
//
//
//   Land Module End
//
//
//
//
//  Commercial Transfer Module Start
//
//
$(document).on("click", "#sea-com-data-trans-req-new", function () {
    var id = $("#unit-details option:selected").val();
    $.ajax({
        type: "POST",
        url: '/Commercial/GetCommercialUnitDetails/',
        data: { ComId: id },
        success: function (data) {
            if (data == false) {
                alert("No Result found");
            }
            else {
                $("#plot-trans-alert").html("");
                $("#oth-inst tbody").empty();
                $("#all-instments tbody").empty();
                $("#rec-amts tbody").empty();
                $("#trns-btn").html("");
                $('#plt-no').text(data.UnitData.shop_no);
                $('#plt-blk').text(data.UnitData.Floor);
                $('#plt-id').val(data.UnitData.Id);
                $('#com-proj').text(data.UnitData.Project_Name);
                $('#plt-size').text(data.UnitData.Total_SqFt_Marlas + " Sq-Ft");
                $('#plot-id').val(data.UnitData.Id);
                $('#plot-size').val(data.UnitData.Total_SqFt_Marlas + " Sq-Ft");
                $('#plt-type').text(data.UnitData.Type);
                $('#plt-dim').text(data.UnitData.Dimension);
                $('#plt-status').text(data.UnitData.Status.replace('_', ' '));
                //$('#plt-road').text(data.UnitData.Road_Type);
                $('#plt-area').text(data.UnitData.Total_SqFt_Marlas + " Sq-Ft");
                $('#plt-loc').text(data.UnitData.Location);
                $("#chat-his").load("/Messages/CommercialComments/", { Commercial_id: data.UnitData.Id });
                $('.plot-owns-fsdhjk').load('/Commercial/CommercialMultiOwners/', { Commercial_Id: data.UnitData.Id });
                var instserialno = 1, overduetotal = 0;
                var bokamt = 0;
                var totalamt = 0;
                $.each(data.UnitInstalments, function (i) {
                    var statuscolor = "";
                    if (data.UnitInstalments[i].Status == "NotPaid") {
                        statuscolor = "bgc-red-50";
                        totalamt = totalamt + data.UnitInstalments[i].Amount;
                    }
                    else if (data.UnitInstalments[i].Status == "Paid") {
                        statuscolor = "bgc-green-50";
                    }
                    var html = '<tr class=' + statuscolor + ' id="' + data.UnitInstalments[i].Id + '" ><td scope="row">' + instserialno + '</td>' +
                        '<td>' + data.UnitInstalments[i].Installment_Name + '</td>' +
                        '<td>' + moment(data.UnitInstalments[i].DueDate).format("MMMM YYYY") + '</td>' +
                        '<td>' + data.UnitInstalments[i].Amount + '</td>' +
                        '<td>' + data.UnitInstalments[i].Status + '</td></tr>';
                    bokamt = bokamt + data.UnitInstalments[i].Amount;
                    $("#all-instments tbody").append(html);
                    instserialno++;
                });
                bokamt = parseFloat(bokamt).toFixed(0);
                $("#plt-bk-price").text(Number(bokamt).toLocaleString());
                var rec = 0
                $.each(data.UnitReceipts, function (i) {
                    var html = '<tr id="' + data.UnitReceipts[i].Id + '" >' +
                        '<td scope="row">' + data.UnitReceipts[i].ReceiptNo + '</td>' +
                        '<td>' + data.UnitReceipts[i].Amount + '</td>' +
                        '<td>' + moment(data.UnitReceipts[i].DateTime).format("DD-MMM-YYYY") + '</td>' +
                        '<td>' + data.UnitReceipts[i].PaymentType + '</td></tr>';
                    $("#rec-amts tbody").append(html);
                    if (data.UnitReceipts[i].Status == "Approved" || data.UnitReceipts[i].Status == null) {
                        rec = rec + Number(data.UnitReceipts[i].Amount);
                    }
                });
                //rec = parseFloat(totalamt).toFixed(0);
                $("#rece-amt").text(Number(rec).toLocaleString());
                var html1 = '<tr><td>Total</td><td colspan="4">' + rec + '</td></tr>';
                $("#rec-amts tbody").append(html1);
                var disc = 0;
                $.each(data.Discounts, function (i) {
                    disc = disc + Number(data.UnitDiscount[i].Discount_Amount);
                });
                //disc = parseFloat(disc).toFixed(0);
                $("#disc-amt").text(Number(disc).toLocaleString());
                overduetotal = totalamt;
                overduetotal = parseFloat(overduetotal).toFixed(0);
                $("#due-amt").text(Number(overduetotal).toLocaleString())
                var overdue = parseFloat(overduetotal);
                //if (data.PlotData.Registry == true) {
                //    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">Registry Plot</div></div>';
                //    $("#plot-trans-alert").append(html);
                //}
                //if (data.PlotData.Status == "Disputed") {
                //    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is DISPUTED</div></div>';
                //    $("#plot-trans-alert").append(html);
                //    return false;
                //}
                if (data.Balance.Balance > 0) {
                    var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot Can not be Transfer Due to pending OVER DUE Amount</div></div>';
                    $("#plot-trans-alert").append(html);
                    if (data.UnitData.Verify != true) {
                        var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is not Verified</div></div>' +
                            '<div class="col-md-12"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6>' +
                            '<label class="switch"><input type="checkbox" id="veri-req"><span class="slider round"></span></label></div>';
                        $("#plot-trans-alert").append(html);
                    }
                }
                else {
                    if (data.UnitData.Status == "Registered") {
                        if (data.UnitData.Verify == true) {
                            var btn = '<button class="btn btn-primary plt-trans-btn-dhjksf" type="submit" id="trans-frm" style="width:100%" >Generate Transfer Request</button>';
                            $("#trns-btn").html(btn);
                        }
                        else {
                            var html = '<div class="col-md-10"><div class="alert alert-danger" role="alert">This Plot is not Verified</div></div>' +
                                '<div class="col-md-12"><h6 style="width: auto;float: left;margin: 7px;">Request For Verifcation</h6>' +
                                '<label class="switch"><input type="checkbox" id="veri-req"><span class="slider round"></span></label></div>';
                            $("#plot-trans-alert").append(html);
                        }
                    }
                }
                if (data.UnitTransferRequest != null && data.UnitTransferRequest != undefined && data.UnitTransferRequest.length > 0) {
                    $('.sel-inf-dfshjgk').remove();
                    $('.trnsfr-plt-prc').val(data.UnitTransferRequest[0].Plot_Price.toLocaleString());
                    //if (data.TransferPending[0].TaxExempted == true) {
                    //    $('#ExemptBuyerTax').prop('checked', true);
                    //    $('.rzn-buy-exe').val((data.TransferPending[0].TaxExemptReason));
                    //    $('.buy-tax-exe-rzn-area').show();
                    //}
                    //if (data.TransferPending[0].Filer_NonFiler == 'Exempted') {
                    //    $('#ExemptSellerTax').prop('checked', true);
                    //    $('.sel-buy-exe').val((data.TransferPending[0].SellExemptReason));
                    //    $('.sel-tax-exe-rzn-area').show();
                    //}
                    $.each(data.UnitTransferRequest, function (i, v) {
                        let strct = `<div class="panel panel-default sel-inf-dfshjgk">
                        <div class="panel-heading">
                            <h5 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#transown${ownsCount}" aria-expanded="true"></a>
                                    Buyer <span class="own-count-info">${($('.panel-default').length + 1)}</span> &nbsp;&nbsp;&nbsp;<i class="fa fa-trash pointer rmv-owns-khsdjf" title="Remove this Owner"></i>
                            </h5>
                        </div>
                        <div class="panel-collapse collapse in show" id="transown${ownsCount}"">
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label>Name</label>
                                    <input type="text" class="form-control Name" value="${v.Name}" required>
                                </div>
                                <div class="form-group col-md-6">
                                    <label>Father's/Husband's Name</label>
                                    <input type="text" class="form-control Father_Husband" value="${v.Father_Husband}" required>
                                </div>
                                <div class="form-group col-md-12">
                                    <label>Postal Address</label>
                                    <input type="text" class="form-control Postal_Address" value="${v.Postal_Address}" placeholder="1234 Main St" required>
                                </div>
                                <div class="form-group col-md-12">
                                    <label>Residential Address</label>
                                    <input type="text" class="form-control Residential_Address" value="${v.Postal_Address}" placeholder="Apartment, Plot, or floor" required>
                                </div>
                                <div class="form-group col-md-5">
                                    <label>City</label>
                                    <select class = "form-control City">${citylist}</select>
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Date Of Birth</label>
                                    <input type="text" class="form-control Date_Of_Birth" value="${v.Date_Of_Birth}" placeholder="mm/dd/yyyy" data-provide="datepicker" required>
                                </div>
                                <div class="form-group col-md-5">
                                    <label>CNIC / NICOP</label>
                                    <input type="text" class="form-control CNIC_NICOP" value=${v.CNIC_NICOP} pattern="[0-9+]{5}-[0-9+]{7}-[0-9]{1}|[0-9+]{6}-[0-9+]{6}-[0-9]{1}" placeholder="12345-1234567-1   or   123456-123456-1" required>
                                </div>
                                <div class="form-group col-md-6" style="margin:auto;">
                                    <h6 style="width: auto;float: left;margin: 7px;">Filer</h6>
                                    <label class="switch"><input type="checkbox" class="filerCheck"><span class="slider round"></span></label>
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Occupation</label>
                                    <input type="text" class="form-control Occupation" value="${v.Occupation}" required>
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Nationality</label>
                                    <input type="text" class="form-control Nationality" value="${v.Nationality}" required>
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Email</label>
                                    <input type="text" class="form-control Email" value="${v.Email}">
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Phone Office</label>
                                    <input type="text" class="form-control Phone_Office" value="${v.Phone_Office}">
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Residential</label>
                                    <input type="text" class="form-control Residential" value="${v.Residential}">
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Mobile 1</label>
                                    <input type="text" class="form-control Mobile_1" value="${v.Mobile_1}" pattern="^0\d{10}" placeholder="03121234567" required>
                                </div>
                                <div class="form-group col-md-4">
                                    <label>Mobile 2</label>
                                    <input type="text" class="form-control Mobile_2" value="${v.Mobile_2}" pattern="^0\d{10}" placeholder="03121234567">
                                    <input type="hidden" class="GroupTag" value="${v.GroupTag}">
                                </div>
                            </div>
                            <hr />
                            <h6 class="c-grey-900">Nominee</h6>
                            <div class="mT-40">
                                <div class="form-row">
                                    <div class="form-group col-md-4">
                                        <label>Name</label>
                                        <input type="text" class="form-control Nominee_Name" value="${v.Nominee_Name}" required>
                                    </div>
                                    <div class="form-group col-md-4">
                                        <label>CNIC / NICOP</label>
                                        <input type="text" class="form-control Nominee_CNIC_NICOP" value="${v.Nominee_CNIC_NICOP}" pattern="[0-9+]{5}-[0-9+]{7}-[0-9]{1}|[0-9+]{6}-[0-9+]{6}-[0-9]{1}" placeholder="12345-1234567-1   or   123456-123456-1" required>
                                    </div>
                                    <div class="form-group col-md-4">
                                        <label>Relation</label>
                                        <input class="form-control Nominee_Relation" value="${v.Nominee_Relation}" placeholder="" required>
                                    </div>
                                    <div class="form-group col-md-10">
                                        <label>Address</label>
                                        <input type="text" class="form-control Nominee_Address" value="${v.Nominee_Address}" placeholder="1234 Main St" required>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>`
                        $('.owns-info').append(strct);
                        if (v.IsFiler == true) {
                            $('.owns-info').last().find('.filerCheck').prop('checked', true);
                        }
                    });
                }
            }
        },
        error: function (data) {
            alert("Error Occured Try Again")
        }
    });
});
$(document).on("click", ".com-trans-det", function () {
    var id = $(this).attr("id");
    window.location = "/Transfer/CommercialTransferDetails?Id=" + id;
});
$(document).on("change", ".com-rec-plt-pres", function () {
    var $this = $(this)
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    var prop = $(this).data("checklst");
    $.post('/Transfer/CommercialCheckList/', { ReqId: id, Updateprop: prop, Status: chkstat }, function (data) {
        if (!data) {
            alert("System Not Responding");
            $(this).removeAttr("checked")
        }
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="button" id="com-transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    }).fail(function () {
        alert("System Not Responding");
        $($this).removeAttr("checked")
        var alldoc = $('#all-doc').is(":checked");
        var comprec = $('#comp-rec').is(":checked");
        var cashpay = $('#cash-pay').is(":checked");
        var chpodd = $('#chpodd').is(":checked");
        if (alldoc && comprec && cashpay && chpodd) {
            var btn = '<button class="btn btn-success" type="submit" id="plt-transf-veri" style="width:100%;margin-top:10px" >Verified & Ready Transfer</button>';
            $("#trns-btn").html(btn);
        }
        else {
            $("#trns-btn").empty();
        }
    });
});
// 
$(document).on("click", "#com-transf-veri", function () {
    var reqgrpid = $("#grpreq-id").val();
    var blod_rel = $("#blod-rel").val();
    var wave = $("#Trans-val").val();
    var othtranval = $("#oth-Trans-val").val();
    var rate = $("#amt").val();
    var remarks = $("#remarks").val();
    if (confirm('Are you sure you want to approve the transfer')) {
        $.post('/Transfer/CommercialOkForTransfer/', { Reqid: reqgrpid, Blood_rel: blod_rel, Wave_off: wave, OtherTransferCharges: othtranval, Rate: rate, Remarks: remarks }, function (data) {
            if (data == true) {
                alert("Ready for Transfer");
            }
        }).fail(function () {
            alert("System Not Responding");
        });
    }
});
//
$(document).on("click", ".com-tran-req-det", function () {
    var id = $("#shp-lst").val();
    if (id) {
        $("#shop-det").empty();
        $("#shop-det").load("/Transfer/CommercialTransferData/", { Id: id });
    }
    else {
        Swal.fire({
            icon: 'info',
            text: 'Select commercial unit to proceed'
        });
    }
});
//
//
//  Commercial Transfer Module End
//
//
//
$(document).on("click", ".del-pt-c", function () {
    var grpid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure you want to Delete the Petty Cash List")) {
        $.ajax({
            type: 'Post',
            url: '/Finance/DeletePTC/',
            data: { Group_Id: grpid },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
    }
});
$(document).on('change', '.pc-acc', function () {
    var headid = $(this).val();
    var pcid = $(this).closest('.pcdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdatePettyCashAccountMapper/',
        data: { PettyCash: pcid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.pc-exp-pay', function () {
    var headid = $(this).val();
    var pcid = $(this).closest('.pcdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdatePettyCashExpMapper/',
        data: { PettyCash: pcid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
// Installments Update for Files, Plots and Commercial Module Start
//
//
$(document).on('click', '.up_inst_btn_com', function () {
    EmptyModel();
    var id = $("#shp-id").val();
    $('#ModalLabel').text("Commercial Installments");
    $('#modalbody').load('/Commercial/UpdateInstallmentInfo/', { id: id }, function () { });
});
$(document).on('click', '.up_inst_btn_file', function () {
    EmptyModel();
    var id = $("#file-id").val();
    $('#ModalLabel').text("File Installments");
    $('#modalbody').load('/FileSystem/UpdateInstallmentInfo/', { id: id }, function () { });
});
$(document).on('click', '.up_inst_btn_plot', function () {
    EmptyModel();
    var id = $("#plt-id").val();
    $('#ModalLabel').text("Plot Installments");
    $('#modalbody').load('/Plots/UpdateInstallmentInfo/', { id: id }, function () { });
});
//
//
// Installments Update for Files, Plots and Commercial Module End
//
//
$(document).on('click', '.btnRemove', function () {
    Swal.fire({
        text: 'Are you sure you want to discard the row?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $(this).closest('.arrElements').remove();
        }
    });
});
$(document).on("change", ".amunt", function () {
    var total = 0;
    $('.amunt').each(function () {
        total = total + parseFloat(RemoveComma($(this).val()));
    });
    $(".ttl").text(total.toLocaleString());
});
//
//
//  File Plot Selection Start
//
//
$(document).on('change', '.fil-plot-dd', function () {
    if ($(this).val() == "File") {
        $(".file-field").show();
        $(".plt-field").hide();
    }
    else {
        $(".file-field").hide();
        $(".plt-field").show();
    }
});
//
//
//  File Plot Selection End
//
//
//
//
//  Company configuration for Chart of Account
//
//
// create a Company
$(document).on("submit", "#c-com", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#c-com").attr('action'),
        data: $("#c-com").serialize(),
        success: function (data) {
            if (data) {
                alert("Company Added");
                window.location.reload();
            }
            else {
                alert("Company Already Exists");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('click', '.shift-crt-kjbasfd', function () {
    EmptyModel();
    $('#ModalLabel').text("Add New Roster");
    $('#modalbody').load('/Attendance/ManageRosters/');
});
$(document).on("click", ".up-ven", function () {
    var gp = $(".gen-pay :selected").val();
    var ad = $(".ad-pay :selected").val();
    var pns = $(".pns-pay :selected").val();
    var ven = $(".ven :selected").val();
    var con = confirm("Are you sure you want to Update Payable config");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/AccountHeads/UpdatePayableMapper/',
            data: { V: ven, ATP: ad, GP: gp, PNS: pns },
            success: function (data) {
                alert("Update Successfully")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on("click", ".up-deal", function () {
    var deal = $(this).val();
    var con = confirm("Are you sure you want to Update Dealer config");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/AccountHeads/UpdateDealershipMapper/',
            data: { Dealer: deal },
            success: function (data) {
                alert("Update Successfully")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on('change', '.del-pay', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateDealerMapper/',
        data: { Dealer: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.blk-rec', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateBlockReceivableMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.blk-sal', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateBlockSalesMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on("click", ".up-inco", function () {
    var a = $(".a :selected").val();
    var b = $(".b :selected").val();
    var c = $(".c :selected").val();
    var d = $(".d :selected").val();
    var e = $(".e :selected").val();
    var f = $(".f :selected").val();
    var g = $(".g :selected").val();
    var h = $(".h :selected").val();
    var i = $(".i :selected").val();
    var j = $(".j :selected").val();
    var k = $(".k :selected").val();
    var l = $(".l :selected").val();
    var m = $(".m :selected").val();
    var n = $(".n :selected").val();
    var con = confirm("Are you sure you want to Update config");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/AccountHeads/UpdateOtherIncomMapper/',
            data: { A: a, B: b, C: c, D: d, E: e, F: f, G: g, H: h, I: i, J: j, K: k, L: l, M: m, N: n },
            success: function (data) {
                alert("Update Successfully")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on('change', '.sc-res', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateSC_ResMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.ec-res', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateEC_ResMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.sc-inc', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateSC_IncMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.ec-inc', function () {
    var headid = $(this).val();
    var blkid = $(this).closest('.blkdiv').attr('id');
    var type = $(this).closest('.blkdiv').data('type');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateEC_IncMapper/',
        data: { Block: blkid, Acc_Id: headid, Type: type },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.ven-pay', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateVendorMapper/',
        data: { Vendor: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on('change', '.prj-exp', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdatePrjMapper/',
        data: { Prj: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
$(document).on("click", ".Update-dealer", function (e) {
    e.preventDefault();
    var id = $('#Dealership option:selected').val();
    var com = $('#new-dealer-commission').val();
    if (isNaN(com) || com > 100) {
        alert("Enter Proper Commission Value");
        return false;
    }
    var res = []
    $('.deler-files:checked').each(function () {
        res.push($(this).data('val'));
    });
    if (confirm("Are you sure you want to Update these Forms")) {
        $.ajax({
            type: "POST",
            url: "/Dealership/UpdateDealeridofFiles/",
            data: { Ids: res, Dealer_Id: id, Commission: com },
            success: function (data) {
                //window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});
// ******************* Chart of Accounts
$(document).on("click", ".up-add-node", function () {
    EmptyModel();
    var id = $(this).data("id");
    $('#ModalLabel').text("Configure Head");
    $('#modalbody').load('/AccountHeads/AddUpdateNode/', { Id: id });
});
$(document).on("click", ".up-salaries", function () {
    var netSalary = $(".sal-net :selected").val();
    var basicSalary = $(".sal-bas :selected").val();
    var overtime = $(".sal-over :selected").val();
    var bonus = $(".sal-bon :selected").val();
    var allowance = $(".sal-all :selected").val();
    var taxdeduction = $(".tax-ded :selected").val();
    var loandeduction = $(".loan-ded :selected").val();
    var advancededuction = $(".adv-ded :selected").val();
    var otherdeduction = $(".oth-ded :selected").val();
    var extrafueldeduction = $(".ext-fuel :selected").val();
    var con = confirm("Are you sure you want to Update HR config");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/AccountHeads/UpdateHRMapper/',
            data: { NetSalary: netSalary, BasicSalary: basicSalary, Overtime: overtime, Bonus: bonus, Allowance: allowance, TaxDeduction: taxdeduction, LoanDeduction: loandeduction, AdvanceDeduction: advancededuction, OtherDeduction: otherdeduction, ExtraFuel: extrafueldeduction },
            success: function (data) {
                alert("Update Successfully")
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});
$(document).on('click', '.bid-Ser-req-row-sc', function () {
    let con = confirm("Are you sure you want to enter Quotation details of this Service Requisition?");
    if (con) {
        let grp = $(this).attr('id');
        window.open('/Services/BidsListing_SC?Group=' + grp, '_blank');
    }
});
$(document).on('click', '.new-bid-serv-sc', function () {
    EmptyModel();
    $('#ModalLabel').text("Add New Quotation");
    var grp = $('#Group_Id').val();
    var inv = $(this).closest('h6').find('.item-qty').text();
    var itemid = $(this).data('itemid');
    $('#modalbody').load('/Services/AddNewQuotation_SC/', { GroupId: grp, Req_Id: itemid, Item: inv });
});
$(document).on("click", ".add-ser-con-quot-btn", function () {
    //var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    //if ((msg == null || msg == "") && file.length == 0) {
    //    return false;
    //}
    var VendorId = $('.vendor option:selected').val();
    var quoteref = $('.quot-ref').val();
    var desc = $('.Description').val();
    var deliverydate = $('.deliv-date').val();
    var Group = $('#GrpId').val();
    var items = [];
    $('.ser-con-items').each(function () {
        let reqid = $(this).attr('data-id');
        let item = $(this).attr('data-item');
        let qty = RemoveComma($(this).find('.qty').val());
        let rate = RemoveComma($(this).find('.rate-p-uom').val());
        let mileid = $(this).attr('data-mileId');
        let milename = $(this).attr('data-milename');
        let nos = $(this).find('.nos').text();
        let depid = $(this).attr('data-depid');
        let depname = $(this).attr('data-depname');
        let desc = $(this).find('.description').text();
        let len = $(this).find('.len').text();
        let wid = $(this).find('.wid').text();
        let hei = $(this).find('.hei').text();
        let bred = $(this).find('.bdth').text();
        let dia = $(this).find('.dia').text();
        items.push({
            ServiceReq_Id: reqid,
            Item_Name: item,
            Qty: qty,
            PurchaseRate: rate,
            Milestone_Id: mileid,
            Milestone_Name: milename,
            Nos: nos,
            Dep_Id: depid,
            Dep_Name: depname,
            Description: desc,
            Length: len,
            Width: wid,
            Heigth: hei,
            Breadth: bred,
            Diameter: dia
        });
    });
    //
    //var form = $("#ad-quo");
    //var data = new FormData();
    //var files = $("#files").get(0).files;
    //data.append("Files", files[0]);
    //$.each(form.serializeArray(), function (key, input) {
    //    data.append(input.name, input.value);
    //});
    //data.append("items", items);
    if (confirm("Are you sure you want to Add a Quotation")) {
        $.ajax({
            type: 'Post',
            url: '/Services/AddQuotation_SC/',
            data: { items: items, Vendor_Id: VendorId, Group_Id: Group, Quote_Ref: quoteref, Currency: '', Description: desc, DeliveryDate: deliverydate },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
                    window.location.reload();
                }
                else {
                    alert(data.Msg)
                }
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
        //$.ajax({
        //    type: "POST",
        //    processData: false,
        //    contentType: false,
        //    url: $("#ad-quo").attr('action'),
        //    data: data,
        //    success: function (data) {
        //        if (data.Status) {
        //            alert(data.Msg)
        //            window.location.reload();
        //        }
        //        else {
        //            alert(data.Msg)
        //        }
        //    }
        //});
    }
});
$(document).on("click", ".fin-quo-ser-con", function () {
    var grp = $(this).closest('tr').attr("id");
    window.open("/Services/QuotationFinalization_SC?Group_Id=" + grp, '_blank');
});
$(document).on("click", ".gen-po-ser-con", function () {
    var grp = $(this).attr("id");
    var po = $(this).attr("data-po");
    if (confirm("Are you sure you want Print Generate Purchase Order")) {
        $.ajax({
            type: 'Post',
            url: '/Services/PrintPO_SC/',
            data: { Group_Id: grp, PO_Num: po },
            success: function (data) {
                $.each(data, function (ind, val) {
                    window.open('/Services/PrintPurchaseOrder_SC?poNum=' + val);
                });
                window.location.reload();
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
    }
});
// create a Work Break Down
$(document).on("click", ".cr-WBD-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#wbd-title").val();
    var desc = $("#wbd-desc").val();
    //var unit = $("#wbd-unit").val();
    //var qty = RemoveComma($("#wbd-qty").val());
    //var rate = RemoveComma($("#wbd-rate").val());
    //var total = RemoveComma($("#wbd-total").val());
    //var deadline = $("#wbd-dead").val();
    //var startdate = $("#wbd-start").val();
    var NoDays = $("#wbd-NoDays").val();
    var wbd_Id = $("#WBDDDown").val();
    var Activity_Id = $("#Dependency").val();
    debugger
    // Empty Validations
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (desc.trim() == null || desc.trim() == "" || desc.trim() == undefined) {
        alert("Please Enter the discription");
        return false;
    }
    //if (unit.trim() == null || unit.trim() == "" || unit.trim() == undefined) {
    //    alert("Please Enter the Unit");
    //    return false;
    //}
    //if (qty.trim() == null || qty.trim() == "" || qty.trim() == undefined) {
    //    alert("Please Enter the Quantity");
    //    return false;
    //}
    //if (rate.trim() == null || rate.trim() == "" || rate.trim() == undefined) {
    //    alert("Please Enter the Rate");
    //    return false;
    //}
    //Unit: unit, Qty: qty, Rate: rate, Amount: total,
    if (NoDays.trim() == null || NoDays.trim() == "" || NoDays.trim() == undefined) {
        alert("Please Enter No Of Days");
        return false;
    }
    //if (total == "" || total == null || total == undefined) {
    //    alert("Please enter the Total Amount first");
    //}
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBD/',
        data: { Prj_Id: prj_id, Title: title, Description: desc, Activity_Id: Activity_Id, NoDays: NoDays, Wbd_Id: wbd_Id },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                $("#WBD").load("/ConstructProjectManagement/Create_WorkBreakDown", { Prj_Id: prj_id });
                window.location.reload();
            }
            else {
                alert("Work Break Down is Not Created");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// create a Work Break Down
$(document).on("click", ".cr-WBDWBD-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#wbdwbd-title").val();
    var desc = $("#wbdwbd-desc").val();
    //var unit = $("#wbdwbd-unit").val();
    //var qty = RemoveComma($("#wbdwbd-qty").val());
    //var rate = RemoveComma($("#wbdwbd-rate").val());
    //var total = RemoveComma($("#wbdwbd-total").val());
    debugger
    // Empty Validations
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (desc.trim() == null || desc.trim() == "" || desc.trim() == undefined) {
        alert("Please Enter the discription");
        return false;
    }
    //if (unit.trim() == null || unit.trim() == "" || unit.trim() == undefined) {
    //    alert("Please Enter the Unit");
    //    return false;
    //}
    //if (qty.trim() == null || qty.trim() == "" || qty.trim() == undefined) {
    //    alert("Please Enter the Quantity");
    //    return false;
    //}
    //if (rate.trim() == null || rate.trim() == "" || rate.trim() == undefined) {
    //    alert("Please Enter the Rate");
    //    return false;
    //}
    //if (total == "" || total == null || total == undefined) {
    //    alert("Please enter the Total Amount first");
    //}
    //Unit: unit, Qty: qty, Rate: rate, Amount: total
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBDWBD/',
        data: { Prj_Id: prj_id, Title: title, Description: desc },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                $("#WBD").load("/ConstructProjectManagement/Create_WorkBreakDown", { Prj_Id: prj_id });
                window.location.reload();
            }
            else {
                alert("Work Break Down is Not Created");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// create a Work Break Down
$(document).on("click", ".cr-WBD-rsrc-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var Activity_Id = $("#ResourceActivity").val();
    var type = $("#Type").val();
    var Inventory = $("#wbdresInventory").val();
    var qty = RemoveComma($("#wbd-res-qty").val());
    var rate = RemoveComma($("#wbd-res-rate").val());
    var total = RemoveComma($("#wbd-res-Amnt").val());
    var Uom = $("#wbd-res-uom").val();
    var phpd = $("#wbd-res-phpd").val();
    var mitem = $("#wbdresAssets").val();
    debugger
    if (type.trim() == null || type.trim() == "" || type.trim() == undefined) {
        alert("Please Select Type");
        return false;
    }
    if (type.trim() == 'Material') {
        if (Inventory.trim() == null || Inventory.trim() == "" || Inventory.trim() == undefined) {
            alert("Please Select Inventory Item");
            return false;
        }
        if (Uom.trim() == null || Uom.trim() == "" || Uom.trim() == undefined) {
            alert("Please Enter UOM");
            return false;
        }
    }
    if (type.trim() == 'Labour') {
        if (phpd.trim() == null || phpd.trim() == "" || phpd.trim() == undefined) {
            alert("Please Enter Per hour/Per day");
            return false;
        }
    }
    if (type.trim() == 'Equipment') {
        if (mitem.trim() == null || mitem.trim() == "" || mitem.trim() == undefined) {
            alert("Please Enter Material item");
            return false;
        }
    }
    if (qty.trim() == null || qty.trim() == "" || qty.trim() == undefined) {
        alert("Please Enter the Quantity");
        return false;
    }
    if (rate.trim() == null || rate.trim() == "" || rate.trim() == undefined) {
        alert("Please Enter the Rate");
        return false;
    }
    if (total == "" || total == null || total == undefined) {
        return false;
    }
    debugger
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBDResource/',
        data: { Prj_Id: prj_id, Activity_Id: Activity_Id, Type: type, Inventory: Inventory, Qty: qty, Rate: rate, Amount: total, UOM: Uom, Phpd: phpd, Mitem: mitem },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                window.location.reload();
                //($('.main-ms').css('display') == 'none')
                //$(".wbd-res").load("/ConstructProjectManagement/WBDResourceMaterial", { Prj_Id: data.Id });
            }
            else {
                alert("WBD Resource is Not Created");
            }
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
// Rate & qty calc
$(document).on('keyup', '.wbd-res-rate', function () {
    var qty = RemoveComma($("#wbd-res-qty").val());
    var rate = RemoveComma($("#wbd-res-rate").val());
    $("#wbd-res-Amnt").val(Number(qty * rate).toLocaleString());
});
// Rate & qty calc
$(document).on('keyup', '.wbdwbd-rate-cal', function () {
    var qty = RemoveComma($("#wbdwbd-qty").val());
    var rate = RemoveComma($("#wbdwbd-rate").val());
    $("#wbdwbd-total").val(Number(qty * rate).toLocaleString());
});
// Rate & qty calc
$(document).on('keyup', '.wbd-rate-cal', function () {
    var qty = RemoveComma($("#wbd-qty").val());
    var rate = RemoveComma($("#wbd-rate").val());
    $("#wbd-total").val(Number(qty * rate).toLocaleString());
});
//
$(document).on('change', '.bank-acc-set', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateBankMapper/',
        data: { BankId: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on('change', '.attendance-acts-ghjksfad', function () {
    let tp = $(this).val();
    let empid = $('.empId').val();
    if (tp == 1) {
        showLoader();
        let empl = parseInt($(this).attr('data-emp'));
        let att = $(this).attr('data-atddt');
        $('#fgkdnhjsfgldjkfgkjndfgkjnsfgkd').click();
        EmptyModel();
        $('#ModalLabel').text("Apply Leave");
        $('#modalbody').load('/Leave/Create/', { Id: empl, attDate: att }, function () {
            hideLoader();
        });
    }
    if (tp == 2) {
        let conf = confirm('Are you sure you want to perform checkout for this attendance?');
        if (!conf) {
            return;
        }
        showLoader();
        let empl = parseInt($(this).attr('data-att'));
        alert(empl);
        //yahan pe ajax ki hit maar k checkout save krao
        $.ajax({
            type: "POST",
            url: '/Attendance/SaveManualCheckOut/',
            data: { attId: empl },
            success: function (data) {
                if (data) {
                    alert("Manual attendance performed successfully.");
                    window.location.reload();
                }
                else {
                    alert("Unable to perform manual attendance for this attendance. Please contact HR");
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("request timeout. check internet connection.");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
    else if (tp == 3) {
        showLoader();
        let att = parseInt($(this).attr('data-att'));
        $('#fgkdnhjsfgldjkfgkjndfgkjnsfgkd').click();
        EmptyModel();
        $('#ModalLabel').text("Time Adjustment Request");
        $('#modalbody').load('/Attendance/AddTimeAdjustment/', { attId: att }, function () {
            hideLoader();
        });
    }
    else if (tp == 4) {
        showLoader();
        let att = $(this).attr('data-atddt');
        console.log(att);
        $('#fgkdnhjsfgldjkfgkjndfgkjnsfgkd').click();
        EmptyModel();
        $('#ModalLabel').text("Mark Holiday");
        $('#modalbody').load('/Attendance/MarkHoliday/', { att: att }, function () {
            hideLoader();
        });
    }
    else if (tp == 5) {
        let att = $(this).attr('data-atddt');
        console.log(att);
        if (confirm("Are you sure you want to Record Attendance")) {
            showLoader();
            $.ajax({
                type: "POST",
                url: '/Attendance/MarkAsOfficialWork/',
                data: { att: att, Emp_Id: empid },
                success: function (data) {
                    if (data.Status) {
                        hideLoader();
                        alert(data.Msg);
                        window.location.reload();
                    }
                    else {
                        alert(data.Msg)
                    }
                }
                , error: function (xmlhttprequest, textstatus, message) {
                    if (textstatus === "timeout") {
                        alert("got timeout");
                    } else {
                        alert(textstatus);
                    }
                }
            });
        }
    }
});
//
function DailyAttPresentExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable1').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test').attr('download', filename + '.xls');
    }
}
//
function DailyAttsftNotStrtExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable7').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test1').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test1').attr('download', filename + '.xls');
    }
}
//
function DailyweekoffExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable8').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test2').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test2').attr('download', filename + '.xls');
    }
}//
//
function DailyAttAbsentExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable2').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test3').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test3').attr('download', filename + '.xls');
    }
}//
//
function DailyAttLeaveExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable3').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test4').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test4').attr('download', filename + '.xls');
    }
}//
//
function DailyAttLateExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable4').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test5').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test5').attr('download', filename + '.xls');
    }
}//
//
function DailyAttearlyStrtExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    tab_text = tab_text + $('#printable5').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "");
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test6').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test6').attr('download', filename + '.xls');
    }
}
// Mark Plot as Mortgaged
$(document).on("click", ".Mortgaged-plt", function () {
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    Swal.fire({
        text: 'Are you sure you want to mark plot as mortgaged?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Plots/UpdatePlotMortgagedStatus/', { Id: id, Status: chkstat, dataStatus: dataStatus }, function (data) {
                //alert("Plot Mortgaged Status Updated");
                Swal.fire({
                    icon: 'success',
                    text: 'Plot marked as mortgaged successfully'
                })
            });
        }
    });
});
//
function DailyAttstotalExcelReport(filename) {
    var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
    tab_text = tab_text + '<head><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
    tab_text = tab_text + '<x:Name>Test Sheet</x:Name>';
    tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
    tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>';
    tab_text = tab_text + "<table border='1px'>";
    //get table HTML code
    var rep = $('#printable6').html().replace(/<select(\s+[^>]*)?>(.|\n)*?<\/select(\s+[^>]*)?>/g, "")
    tab_text = tab_text + rep;
    tab_text = tab_text + '</table></body></html>';
    var data_type = 'data:application/vnd.ms-excel';
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    //For IE
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        if (window.navigator.msSaveBlob) {
            var blob = new Blob([tab_text], { type: "application/csv;charset=utf-8;" });
            navigator.msSaveBlob(blob, filename + '.xls');
        }
    }
    //for Chrome and Firefox 
    else {
        $('#test7').attr('href', data_type + ', ' + encodeURIComponent(tab_text));
        $('#test7').attr('download', filename + '.xls');
    }
}
//
$(document).on('change', '.bank-on-acc-set', function () {
    var headid = $(this).val();
    var dealid = $(this).closest('.dealdiv').attr('id');
    $.ajax({
        type: "POST",
        url: '/AccountHeads/UpdateBankOnlineMapper/',
        data: { BankId: dealid, Acc_Id: headid },
        success: function (data) {
            alert("Update Successfully")
        }
        , error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("got timeout");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on("click", "#sup-on-bank", function () {
    var bankid = $("#Bank").val();
    var data = [];
    $('.tik-entry').each(function () {
        var t = $(this).is(":checked");
        if (t) {
            var val = $(this).closest('tr').attr('id');
            data.push(val)
        }
    });
    if (confirm("Are you sure you want to Day Close")) {
        $.ajax({
            type: "POST",
            url: '/GeneralEntry/EntriesSupervision/',
            data: { Grp: data, BankId: bankid },
            success: function (data) {
                window.location.reload();
            },
            error: function () {
                alert("Error Occured");
            }
        });
    }
});

$(document).on("change", ".WHTCharge", function () {
    var id = $('.hid-plt-id').val();
    var status = $('#WHTCharge').val();

    var ch = confirm('Are you sure you want to Apply With Holding Tax');
    if (ch) {
        $.ajax({
            type: "POST",
            url: "/Plots/WHTChargerPlotInstallment/",
            data: { PlotId: id, Status: status },
            success: function (data) {
                if (data) {
                    let plotIdData = $('.hid-plt-id').val();
                    SASLoad($('.inst-n-pmts'));
                    $('.inst-n-pmts').load('/Plots/WHTPlotInstallmentAndReceiptsPartial/', { Plotid: plotIdData }, function () {
                        SASUnLoad($('.inst-n-pmts'));
                    });
                }
            }
            , error: function (xmlhttprequest, textstatus, message) {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }
});

$(document).on("click", ".SAGarden-lead-prem-uns-search", function () {
    var from = $("#startdate").val();
    var to = $("#enddate").val();
    $('#homedetails').load('/PropertyDeal/SAGardenUnassignedLeads/', { From: from, To: to });
});
//
$(document).on("click", ".SAGArden-lead-prem-assign", function () {
    EmptyModel();
    $('.modal-body').load('/PropertyDeal/SAGardenAssignNewLeads/', function () { });
});
//
$(document).on("click", ".SAGardenunassigned_lead_up", function () {
    var pr = $('#Prj').val();
    EmptyModel();
    $('#ModalLabel').text("Upload Leads");
    $('#modalbody').load('/PropertyDeal/SAGardenUploadLeads', { Project: pr, status: "UnAssigned" });
});

$(document).on("click", ".sag-unlea", function () {
    var pr = $('#Prj').val();
    EmptyModel();
    $('#ModalLabel').text("Upload Leads");
    $('#modalbody').load('/Leads/CreateUnassignLead', { Project: pr, status: "UnAssigned" });
});
//
$(document).on("click", "#SAGarden-assi-new-lead", function () {
    var salesLeads = [];
    var leadCountTotal = $('.totleads').data('counter');
    var leadsCounter = 0;
    $('.salesLeads').each(function () {
        salesLeads.push({
            SalesPersonId: Number($(this).attr('data-id')),
            LeadsCount: Number($(this).find('.userleadcount').val())
        });
        leadsCounter = leadsCounter + Number($(this).find('.userleadcount').val())
    });
    if (leadsCounter > leadCountTotal) {
        alert("Cannot Assign more than uploaded leads");
        return false;
    }
    var con = confirm("Are you sure you want to Assign Leads");
    if (con) {
        $.ajax({
            type: 'POST',
            url: '/PropertyDeal/SAGardenAssignLeadsToSales/',
            dataType: "json",
            data: { SalesLeads: salesLeads },
            success: function (data) {
                alert('Leads Successfully Assigned');
                window.location.reload();
            },
            error: function () {
                alert('Error Occured');
            }
        });
    }
});

$(document).on('click', '.appl-inst-btn-ashdfjk', function () {
    $(this).attr("disabled", true);
    let instArr = [];
    let plan = $('.inst-opt-fhjlkads option:selected').val();
    let amt = parseInt(RemoveComma($('.inst-amt-fasdkjl').val()));
    let reason = $('#loanreason').val();
    let emp = $('.emp-det-raw').val();
    if ($('.emp-det-raw').length <= 0) {
        emp = $('#emp_loan_id').val();
    }
    let nameOfType = $('#loan-advance-app option:selected').val();

    $('.inst-pl-row-hjkl').each(function () {
        let __inst_amt = $.trim($(this).find('.inst-amt').text());
        let __inst_dt = $.trim($(this).find('.inst-dt').text());

        instArr.push({
            Id: parseInt(RemoveComma(__inst_amt)),
            Status: __inst_dt
        });
    });

    //yahan pe push krna hai data server pe
    showLoader();
    $.ajax({
        type: "POST",
        url: "/Loan/SaveLoanApplication/",
        contentType: "application/json",
        traditional: true,
        beforeSend: function () {
        },
        complete: function () {
        },
        data: JSON.stringify({ installments: instArr, plan: plan, rzn: reason, loanAmt: amt, emp: emp, tp: nameOfType }),
    }).done(function (data) {
        if (data.Status == true) {
            alert(data.Msg);
            window.location.reload();
        }
        else {
            alert(data.Msg);
            $(this).attr("disabled", false);
            hideLoader();
        }
    });
});