// Open link for project Details
$(document).on("click", ".prj-det", function () {
    var id = $(this).closest('tr').attr("id");
    window.open("/ConstructProjectManagement/ProjectDetails?Prj_Id=" + id, '_blank');
});
//
$(document).on("click", ".cr-prj", function () {
    EmptyModel();
    $('#ModalLabel').text("Create Project");
    var html = '';
    $('#modalbody').load('/ConstructProjectManagement/CreateProject/', function () {
        $("#deps").empty();
        $.each(dep, function (key, value) {
            $("#deps").append('<option value="' + value.Value + '">' + value.Text + '</option>');
        });
        $('.modal-footer').append('<button class="btn btn-primary cr-prj-btn" type="button">Create Project</button>');
    });
});
// for Project Id
$(document).on("keyup", "#prj-name", function () {
    var prjnam = $(this).val();
    if (prjnam === null || prjnam === "") {
        $("#prj-id-nam").empty();
    }
    else {
        var prjid = prjnam.replace(/\s/g, '') + "-" + moment().format("DDMMYY");
        $("#prj-id-nam").text(prjid);
    }
});
// create a role
$(document).on("click", ".cr-prj-btn", function (e) {
    var prjnam = $("#prj-name").val();
    var accHead = $('#coAccounts option:selected').val();
    var tp = $("#prj-tp").is(":checked");
    var tpv = (tp) ? "Recurring" : null;
    var os = $("#offsite").is(":checked");
    var us = [];
    var ids = $("#deps").val();
    if (accHead == null || accHead == 0) {
        alert("Please Select Expense Account");
        return false;
    }
    $.each(ids, function (a) {
        var id = $(".dep-sel-" + ids[a]).val();
        $.each(id, function (c) {
            us.push(id[c]);
        });
    });
    if (confirm("Are your sure you want to create the Project")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/CreateProject/',
            data: { ProjectName: prjnam, Emp: us, PrjType: tpv, Offsite: os, AccountId: accHead },
            success: function (data) {
                if (data === 0) {
                    alert("This Project already Exist");
                }
                else {
                    alert("Project Created");
                    window.location.reload();
                    window.open("/ConstructProjectManagement/ProjectConfiguration?Id=" + data, '_blank');
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
// Updating Project Name
$(document).on('click', '.editable-prj-nam', function () {
    var t = $(this);
    var prjatt = t.data('prjatt');
    var input = $('<input>').attr('class', 'savable-prj-nam prj-nam form-control').attr('data-prjatt', prjatt).val(t.text());
    t.replaceWith(input);
    input.focus();
});
// Delete MTS
$(document).on('click', '.del-mts', function () {
    if (confirm("Are you sure you want to delete this.")) {
        var id = $(this).closest("tr").attr("id");
        $.post("/ConstructProjectManagement/DeleteMTS/", { Id: id }, function (data) {
            if (data.STATUS) {
                $("#" + id).remove();
                alert(data.Msg);
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
// Delete WBD
$(document).on('click', '.del-WBD', function () {
    if (confirm("Are you sure you want to delete this.")) {
        var id = $(this).closest("tr").attr("id");
        $.post("/ConstructProjectManagement/DeleteWBD/", { Id: id }, function (data) {
            if (data.STATUS) {
                $("#" + id).remove();
                alert(data.Msg);
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
// Delete Project
$(document).on('click', '.del-prj', function () {
    var id = $(this).closest("tr").attr("id");
    if (confirm("Are you sure you want to delete Project")) {
        $.post("/ConstructProjectManagement/DeleteProject/", { Id: id }, function (data) {
            if (data.STATUS) {
                alert(data.Msg);
                window.location.reload();
            }
            else {
                alert(data.Msg);
            }
        });
    }
});
// saving project name
$(document).on('blur', '.savable-prj-nam', function () {
    var input = $(this);
    var atr = input.data("prjatt");
    var id = $("#Prj_Id").val();
    $.post("/ConstructProjectManagement/UpdateProjectDetails/", { PrjAttr: atr, Value: input.val(), Prj_Id: id }, function () {
        var t = $('<h1>').attr('class', 'editable-prj-nam display-4').attr('data-prjatt', atr).text(input.val());
        input.replaceWith(t);
        $(".prj-id").text(input.val().replace(/\s/g, '') + "-" + moment().format("DDMMYY"));
    });
});
// edit paragraph details
$(document).on('click', '.editableParagraph', function () {
    var t = $(this);
    var prjatt = t.data('prjatt');
    var input = $('<input>').attr('class', 'savableParagraph form-control').attr('data-prjatt', prjatt).val(t.text());
    t.replaceWith(input);
    input.focus();
});
// save paragraph details
$(document).on('blur', '.savableParagraph', function () {
    var input = $(this);
    if (input.val() == null || input.val() == "") {
        return false;
    }
    var atr = input.data("prjatt");
    var id = $("#Prj_Id").val();
    $.post("/ConstructProjectManagement/UpdateProjectDetails/", { PrjAttr: atr, Value: input.val(), Prj_Id: id }, function () {
        var t = $('<p>').attr('class', 'editableParagraph').attr('data-prjatt', atr).text(input.val());
        input.replaceWith(t);
    });
});
// Updating project budget
$(document).on('click', '.editablebudget', function () {
    var t = $(this);
    var prjatt = t.data('prjatt');
    var input = $('<input>').attr('class', 'savablebudget coma form-control').attr('data-prjatt', prjatt).val(t.text());
    t.replaceWith(input);
    input.focus();
});
// saving project budget
$(document).on('blur', '.savablebudget', function () {
    var input = $(this);
    if (input.val() == null || input.val() == "") {
        return false;
    }
    var atr = input.data("prjatt");
    var id = $("#Prj_Id").val();
    $.post("/ConstructProjectManagement/UpdateProjectDetails/", { PrjAttr: atr, Value: RemoveComma(input.val()), Prj_Id: id }, function () {
        var t = $('<h3>').attr('class', 'editablebudget').attr('data-prjatt', atr).text(Number(RemoveComma(input.val())).toLocaleString());
        input.replaceWith(t);
    });
});
// Updating Project Start date
$(document).on('click', '.editablestart', function () {
    var t = $(this);
    var prjatt = t.data('prjatt');
    var input = $('<input>').attr('class', 'savablestart form-control').attr('data-prjatt', prjatt).attr('data-provide', "datepicker").val(t.text());
    t.replaceWith(input);
    input.focus();
});
// saving project Start
$(document).on('blur', '.savablestart', function () {
    var input = $(this);
    if (input.val() == null || input.val() == "") {
        return false;
    }
    if ($(".datepicker")[0]) {
        return false
    }
    var atr = input.data("prjatt");
    var id = $("#Prj_Id").val();
    $.post("/ConstructProjectManagement/UpdateProjectDetails/", { PrjAttr: atr, Value: input.val(), Prj_Id: id }, function () {
        var t = $('<h3>').attr('class', 'editablestart').attr('data-prjatt', atr).text(moment(input.val()).format("DD-MMM-YYYY"));
        input.replaceWith(t);
    });
});
// Updating Project deadline
$(document).on('click', '.editabledead', function () {
    var t = $(this);
    var prjatt = t.data('prjatt');
    var input = $('<input>').attr('class', 'savabledead form-control').attr('data-prjatt', prjatt).attr('data-provide', "datepicker").val(t.text());
    t.replaceWith(input);
    input.focus();
});
// saving project deadline
$(document).on('blur', '.savabledead', function () {
    var input = $(this);
    if (input.val() == null || input.val() == "") {
        return false;
    }
    if ($(".datepicker")[0]) {
        return false
    }
    var atr = input.data("prjatt");
    var id = $("#Prj_Id").val();
    $.post("/ConstructProjectManagement/UpdateProjectDetails/", { PrjAttr: atr, Value: input.val(), Prj_Id: id }, function () {
        var t = $('<h3>').attr('class', 'editabledead').attr('data-prjatt', atr).text(moment(input.val()).format("DD-MMM-YYYY"));
        input.replaceWith(t);
    });
});
//
$(document).on("click", ".WbdRsrc", function () {
    var url = $(this).data("link");
    SASLoad($('#Wbddetails'));
    var id = $("#Prj_Id").val();
    $('#Wbddetails').load(url, { Prj_Id: id }, function () {
        SASUnLoad($('#Wbddetails'));
    });
});
$(document).on("click", ".prjlink", function () {
    var url = $(this).data("link");
    SASLoad($('#prjdetails'));
    var id = $("#Prj_Id").val();
    $('#prjdetails').load(url, { Prj_Id: id }, function () {
        SASUnLoad($('#prjdetails'));
    });
});
// create a milestone
$(document).on("click", ".cr-mile-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#mile-title").val();
    var desc = $("#mile-desc").val();
    var qty = RemoveComma($("#mile-qty").val());
    var rate = RemoveComma($("#mile-rate").val());
    var unit = $("#mile-unit").val();
    var total = RemoveComma($("#mile-total").val());
    var deadline = $("#mile-dead").val();
    var startdate = $("#mile-start").val();
    var mts_id = $("#mts_id").val();
    var Mts = $("#MTS").val();
    var dep = $("#Department").val();

    // Empty Validations
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (desc.trim() == null || desc.trim() == "" || desc.trim() == undefined) {
        alert("Please Enter the discription");
        return false;
    }
    if (deadline == "" || deadline == null || deadline == undefined) {
        alert("Please set the deadline first");
        return false;
    }
    if (startdate == "" || startdate == null || startdate == undefined) {
        alert("Please set the startdate first");
        return false;
    }
    if (total == "" || total == null || total == undefined) {
        alert("Please enter the Total Amount first");
    }

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_MTS/',
        data: { Prj_Id: prj_id, Title: title, Description: desc, Qty: qty, Rate: rate, Unit: unit, Amount: total, Deadline: deadline, MTS_Id: mts_id, MTS: Mts, Start_Date: startdate, DepId: dep },
        success: function (data) {
            if (data.Status) {
                alert(data.Msg);
                FormReset();
                $("#mts").load("/ConstructProjectManagement/Create_Milestones", { Prj_Id: prj_id });
            }
            else {
                alert(data.Msg);
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
// create a task
$(document).on("click", ".cr-tsk-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var desc = $("#tsk-desc").val();
    var qty = RemoveComma($("#tsk-qty").val());
    var rate = RemoveComma($("#tsk-rate").val());
    var total = RemoveComma($("#tsk-total").val());
    var unit = $("#tsk-unit").val();
    var startdate = $("#tsk-start").val();
    var deadline = $("#tsk-dead").val();
    var mts_id = $(this).closest("tr").attr("id");
    var Mts = "Task";

    // Empty Validations
    if (desc.trim() == null || desc.trim() == "" || desc.trim() == undefined) {
        alert("Please Enter the discription");
        return false;
    }
    if (deadline == "" || deadline == null || deadline == undefined) {
        alert("Please set the deadline first");
        return false;
    }
    if (startdate == "" || startdate == null || startdate == undefined) {
        alert("Please set the startdate first");
        return false;
    }
    if (total == "" || total == null || total == undefined) {
        alert("Please enter the Total Amount first");
    }

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_MTS/',
        data: { Prj_Id: prj_id, Description: desc, Qty: qty, Rate: rate, Amount: total, Unit: unit, Deadline: deadline, MTS_Id: mts_id, Start_Date: startdate, MTS: Mts },
        success: function (data) {
            if (data.Status) {
                alert(data.Msg);
                $("#mts").load("/ConstructProjectManagement/Create_Milestones", { Prj_Id: prj_id });
            }
            else {
                alert(data.Msg);
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
// create a sub task
$(document).on("click", ".cr-stsk-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var desc = $("#stsk-desc").val();
    var qty = RemoveComma($("#stsk-qty").val());
    var rate = RemoveComma($("#stsk-rate").val());
    var total = RemoveComma($("#stsk-total").val());
    var unit = $("#stsk-unit").val();
    var startdate = $("#stsk-start").val();
    var deadline = $("#stsk-dead").val();
    var mts_id = $(this).closest("tr").attr("id");
    var Mts = "SubTask";

    // Empty Validations
    if (desc.trim() == null || desc.trim() == "" || desc.trim() == undefined) {
        alert("Please Enter the discription");
        return false;
    }
    if (deadline == "" || deadline == null || deadline == undefined) {
        alert("Please set the deadline first");
        return false;
    }
    if (startdate == "" || startdate == null || startdate == undefined) {
        alert("Please set the startdate first");
        return false;
    }
    if (total == "" || total == null || total == undefined) {
        alert("Please enter the Total Amount first");
    }

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_MTS/',
        data: { Prj_Id: prj_id, Description: desc, Qty: qty, Rate: rate, Amount: total, Unit: unit, Deadline: deadline, Start_Date: startdate, MTS_Id: mts_id, MTS: Mts },
        success: function (data) {
            if (data.Status) {
                alert(data.Msg);
                $("#mts").load("/ConstructProjectManagement/Create_Milestones", { Prj_Id: prj_id });
            }
            else {
                alert(data.Msg);
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
        data: { Prj_Id: prj_id, Title: title, Description: desc  },
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


//add a Task Row
$(document).on("click", ".ad-tsk-row", function () {

    debugger
    if ($(document).find(".remove-tsk-row").length <= 0) {
        var counter = $('#mts-table tr').length - 2;
        var mileid = $(this).closest("tr").attr("id");
        var newRow = $('<tr id="' + mileid + '">');
        var milestone_amount = $(this).closest('tr').find('.budget_cts').attr("id");
        var milestone_startdate = $(this).closest('tr').find('.edit-start').attr("id");
        var milestone_deadline = $(this).closest('tr').find('.edit-dead').attr("id");

        var cols = "";
        cols += `<td><span class="fas fa-minus-square remove-tsk-row"></span><input type="hidden" value=${milestone_amount} id="budget_cts" /><input type="hidden" value=${milestone_startdate} id="edit-start" /></td>`;
        cols += `<td><input type="hidden" value=${milestone_deadline} id="edit-dead" /></td>`;
        cols += '<td style="padding:1px"><textarea rows="4" class=" form-control" id="tsk-desc" placeholder="Enter Description"></textarea></td>';
        cols += `<td style="padding:1px; margin:0px"><select class="form-control" id="tsk-unit" style="padding:0px !important; margin:0px !important"><option value="-">-</option><option value="Cft">Cft</option><option value="Rft">Rft</option><option value="Nos.">Nos.</option><option value="Each">Each</option><option value="Kg">Kg</option><option value="Job">Job</option><option value="Per/Hour">Per/Hour</option><option value="Sft">Sft</option><option value="Unit">Unit</option></select></td>`;

        cols += '<td style="padding:1px"><input type="text" class="form-control tsk-rate-cal coma" id="tsk-qty" placeholder="Qty"></td>';
        cols += '<td style="padding:1px"><input type="text" class="form-control tsk-rate-cal coma" id="tsk-rate" placeholder="Rate"></td>';
        cols += '<td style="padding:1px"><input type="text" class="form-control coma" id="tsk-total" placeholder="Total Amount" ></td>';
        cols += '<td style="padding:1px"><input type="text" class="form-control " id="tsk-start" data-provide="datepicker" placeholder="Startdate"></td>';
        cols += '<td style="padding:1px"><input type="text" class="form-control " id="tsk-dead" data-provide="datepicker" placeholder="Deadline"> <button style="float:right;margin-top:10px" class="btn btn-primary cr-tsk-btn">Save</button></td>';
        newRow.append(cols);
        newRow.insertAfter($(this).parents().closest('tr'));
        counter++;
    } else {
        alert("Cannot create another task fill the previous first");
    }

});


$(document).on("click", ".remove-tsk-row", function () {
    debugger
    $(this).closest("tr").remove();

});

//remove subtask

$(document).on("click", ".remove-stsk-row", function () {
    debugger

    $(this).closest("tr").remove();
});

//$(document).on("click", ".slct-Dependency", function (e) {
//    var prj_id = $("#Prj_Id").val();
//    var wbdid = $(this).closest("tr").attr("id");
//    debugger
//    $.ajax({
//        type: "POST",
//        url: '/ConstructProjectManagement/SelectActivityList/',
//        data: { prj_id: prj_id, wbdid: wbdid },
//        success: function (data) {
//            if (data) {
//                debugger
//                alert('Hello');
//            }
//            //debugger
//            //$('#slct-Dependency').html(' ');
//            //$('#slct-Dependency').append('<option>Select Activity</option>');
//            //$.each(data, function (key, value) {
//            //    $("#slct-Dependency").append('<option value=' + value.Id + '>' + value.Name + '</option>');
//            //});
//        }
//        , error: function (xmlhttprequest, textstatus, message) {
//            debugger
//            if (textstatus === "timeout") {
//                alert("got timeout");
//            } else {
//                alert(textstatus);
//            }
//        }
//    });
//});

// create a Work Break Down
$(document).on("click", ".slct-Dependency", function (e) {
    var prj_id = $("#Prj_Id").val();
    var wbdid = $(this).closest("tr").attr("id");
    debugger
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/SelectActivityList/',
        data: { Prj_Id: prj_id,Wbd_Id: wbdid },
        success: function (data) {
            debugger
            $('#slct-Dependency').html(' ');
            $('#slct-Dependency').append('<select class="form-control" id="slct-activity" style="width:140px"><option value="">Select Activity</option>');
            $.each(data, function (key, value) {
                $("#slct-activity").append('<option value=' + value.Id + '>' + value.Title + '</option>');
            });
            $('#slct-Dependency').append('</select>');
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




//add a Task Row
$(document).on("click", ".ad-wbd-tsk-row", function () {
   
    if ($(document).find(".remove-tsk-row").length <= 0) {
        var Actvid = $(this).closest("td").attr("id");
        var counter = $('#wbd-table tr').length - 2;
        var wbdid = $(this).closest("tr").attr("id");
        var newRow = $('<tr id="' + wbdid + '">');
        var cols = "";
        cols += `<td><span class="fas fa-minus-square remove-tsk-row"></span><input type="hidden" value=${wbdid} id="wbdid" /></td>`;
        cols += `<td style="padding:10px; margin:0px"><select class="form-control slct-Dependency check-type" id="wbd-type" style="padding:0px !important; margin:0px !important"><option class="" value="-">Select Type</option><option class="" value="WBS">WBS</option><option class="" value="Activity">Activity</option></select></td>`;
        newRow.append(cols);
        newRow.insertAfter($(this).parents().closest('tr'));
        counter++;
    } else {
        alert("Cannot create another task fill the previous first");
    }

});
$(document).on('change', '.check-type', function () {
    debugger
    var selectedType = $(this).children("option:selected").val();
    console.log(selectedType);
    if ((selectedType != undefined) && (selectedType != '')) {
        var wbdid = $(this).closest("tr").attr("id");
        if (selectedType == 'Activity') {
            var newtd = $('<tr style="width:500px" id="' + wbdid + '">');
                var Act = "";
                var wbdid = $(this).closest("tr").attr("id");
                var newtd = $('<tr id="' + wbdid + '">');
                //Act += '<td style="padding:1px"><input  type="hidden" ></td>';
                //Act += `<td><span class="fas fa-minus-square remove-wbd-row"></span><input type="hidden" value=${wbdid} id="wbdid" /></td>`;
            Act += '<td colspan="2" style="padding:10px;border: hidden;" class="removewbdact" ><textarea   rows="1"  class=" form-control" style="width:300px" id="wbd-title" placeholder="Enter Title"></textarea></td>';
            Act += '<td style="padding:10px;border: hidden;" class="removewbdact"><input type="number" class="form-control" style="width:120px" id="wbd-NoDays" placeholder="No of Days"></td>';
            Act += `<td style="margin:0px;border: hidden;" class="removewbdact"><select class="form-control"  id="slct-rltn" style="width:115px;padding:0px !important;!important"><option class="" value="-">Relation Type</option><option class="" value="FS">FS</option><option class="" value="SS">SS</option></select></td>`;
            Act += `<td style="margin:0px;border: hidden;" id="slct-Dependency" class="removewbdact"><select class="form-control" id="slct-activity" style="width:140px"><option value="_"></option></select></td>`;
            Act += '<td class="removewbdact" style="border: hidden;"><button style="float:left;" class="btn btn-primary cr-row-Actv-btn">Save</button> </td>';
            $(this).closest('tr').find('.removewbdact').remove();
                newtd.html(Act);
                newtd.insertAfter($(this).closest('td'));
        }
        if (selectedType == 'WBS') {
            var wbdid = $(this).closest("tr").attr("id");
            var newtd1 = $('<tr id="' + wbdid + '">');
            var wbd = "";
            //wbd += '<td style="padding:1px"><input  type="hidden" ></td>';
            //wbd += `<td><span class="fas fa-minus-square remove-wbd-row"></span><input type="hidden" value=${wbdid} id="wbdid" /></td>`;
            wbd += '<td colspan="3" style="padding:10px ;border: hidden;" class="removewbdact"><textarea rows="1" class=" form-control" style="width:300px" id="wbd-title" placeholder="Enter Title"></textarea></td>';
            wbd += '<td class="removewbdact"  style="border: hidden;"><button style="float:left;" class="btn btn-primary cr-row-WBD-btn">Save</button> </td>';
            var newtd1 = $('<tr id="' + wbdid + '">');
            newtd1.removeAttr(wbd);
            $(this).closest('tr').find('.removewbdact').remove();
            newtd1.html(wbd);
            newtd1.insertAfter($(this).closest('td'));
        }

    } else {
        console.log('Error');
        $('.main-ms').slideToggle(300);
    }
});

// create a Work Break Down
$(document).on("click", ".cr-row-WBD-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#wbd-title").val();
    var Type = $("#wbd-type").val();
    var Wbd_Id = $(this).closest("tr").attr("id");
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (Type == '0' || Type == "-" || Type == undefined) {
        alert("Please Select Type");
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_row_WBD/',
        data: { Prj_Id: prj_id, Title: title, Type: Type, Wbd_Id: Wbd_Id },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                $("#WBD").load("/ConstructProjectManagement/Create_WorkBreakDown", { Prj_Id: prj_id });
            }
            else {
                alert("Work BreakDown Schedules is Not Created");
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
$(document).on("click", ".cr-row-Actv-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#wbd-title").val();
    var Type = $("#wbd-type").val();
    var NoDays = $("#wbd-NoDays").val();
    var Wbd_Id = $(this).closest("tr").attr("id");
    var Relation = $("#slct-rltn").val();
    var ActivityId = $("#slct-activity").val();
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (Type == '0' || Type == "-" || Type == undefined) {
        alert("Please Select Type");
        return false;
    }
    if (NoDays.trim() == null || NoDays.trim() == "" || NoDays.trim() == undefined) {
        alert("Please Enter No Of Days");
        return false;
    }
    debugger
       if (Relation == "-") {
        var Relation = 'null';
    }
    //if (ActivityId == "-") {
    //    var ActivityId = 'null';
    //}
    if (confirm("Are you sure you want to Submit Activity")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/Add_row_Activity/',
            data: { Prj_Id: prj_id, Title: title, NoDays: NoDays, Type: Type, Wbd_Id: Wbd_Id, Relation: Relation, ActivityId: ActivityId },
            success: function (data) {
                if (data.Status == true) {
                    debugger
                    alert(data.Msg);
                    $("#WBD").load("/ConstructProjectManagement/Create_WorkBreakDown", { Prj_Id: prj_id });
                }
                else {
                    alert("Work BreakDown Activity is Not Created");
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





// create a Work Break Down
$(document).on("click", ".cr-WBD-btn", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#wbd-title").val();
    var Type = $("#wbd-type").val();
    // Empty Validations
    if (title.trim() == null || title.trim() == "" || title.trim() == undefined) {
        alert("Please Enter the Title");
        return false;
    }
    if (Type == '0' || Type == "-" || Type == undefined) {
        alert("Please Select Type");
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBD/',
        data: { Prj_Id: prj_id, Title: title,  Type: Type },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                $("#WBD").load("/ConstructProjectManagement/Create_WorkBreakDown", { Prj_Id: prj_id });
                //window.location.reload(); 
               
            }
            else {
                alert("Work BreakDown Schedule is Not Created");
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





//for title
$(document).on('click', '.edit-titl-wbd', function () {
    debugger
    var t = $(this);
    var input = '<td><input class="savabletitl  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savabletitl").focus();
});
$(document).on('blur', '.savabletitl', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid title enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/EditWBD_Title_Desc/', { Value: input.val(), Attr: 'title', Id: id }, function (data) {
            if (data) {
                input.parent().attr('class', 'edit-titl-wbd td');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});


//for title
$(document).on('click', '.edit-nodays-wbd', function () {

    //var NodaysId = $(this).closest("td").attr("id");
    //var NodaysId2 = $(this).closest(".nodaysId").attr("id");
    //var Proj_Id = $("#Proj_Id").val();
    //var wbdid = $(this).closest("tr").attr("id");
    //var Type = $(this).closest("tr").find('.TypeId').text();
    var Version = $(this).closest("tr").find('.Version').text();


    var t = $(this);
    var input = '<td><input class="savablenodays  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savablenodays").focus();
});
$(document).on('blur', '.savablenodays', function () {
   
    var Proj_Id = $("#Proj_Id").val();
    var Type = $(this).closest("tr").find('.TypeId').text();
    var Version = $(this).closest("tr").find('.Version').text();

    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid title enter again");
        return false;
    } else {
        debugger
        $.post('/ConstructProjectManagement/EditWBD_Title_nodays/', { Value: input.val(), Attr: 'nodays', Id: id, Prj_Id: Proj_Id, Type: Type, Wbd_Id:Version},
            function (data) {
            if (data) {
                input.parent().attr('class', 'edit-nodays-wbd td');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
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

// Rate & qty calc
$(document).on('keyup', '.mile-rate-cal', function () {
    var qty = RemoveComma($("#mile-qty").val());
    var rate = RemoveComma($("#mile-rate").val());

    $("#mile-total").val(Number(qty * rate).toLocaleString());
});
// Rate & qty calc
$(document).on('keyup', '.tsk-rate-cal', function () {
    var qty = RemoveComma($("#tsk-qty").val());
    var rate = RemoveComma($("#tsk-rate").val());

    $("#tsk-total").val(Number(qty * rate).toLocaleString());
});
// Rate & qty calc
$(document).on('keyup', '.stsk-rate-cal', function () {
    var qty = RemoveComma($("#stsk-qty").val());
    var rate = RemoveComma($("#stsk-rate").val());
    $("#stsk-total").val(Number(qty * rate).toLocaleString());
});
// reset form values
function FormReset() {
    $('.mts-form').find('input:text').val('');
    $('.mts-form').find('textarea').val('');
}
//milestone total click 
$(document).on("keyup", "#mile-total", function () {
    var tem = $(this).val();
    $("#mile-qty").val(null);
    $("#mile-rate").val(null);
});
//task total click
$(document).on("keyup", "#tsk-total", function () {
    var tem = $(this).val();
    $("#tsk-qty").val(null);
    $("#tsk-rate").val(null);
});
//subtask total click
$(document).on("keyup", "#stsk-total", function () {
    var tem = $(this).val();
    $("#stsk-qty").val(null);
    $("#stsk-rate").val(null);
});
//remove a Task & subtask row





$(document).on("click", ".WbdrscMaterial", function () {
    debugger
    var id = $("#Proj_Id").val();
    $('.Wbddetails').load('/ConstructProjectManagement/WbdResourcesMaterial/', { Prj_Id: id });
});

$(document).on("click", ".WbdrscLabour", function () {
    debugger

    var id = $("#Proj_Id").val();
    $('.Wbddetails').load('/ConstructProjectManagement/WbdResourcesLabour/', { Prj_Id: id });
});

$(document).on("click", ".WbdrscEquipment", function () {
    debugger

    var id = $("#Proj_Id").val();
    $('.Wbddetails').load('/ConstructProjectManagement/WbdResourcesEquipment/', { Prj_Id: id });
});


// create a Work Break Down Material Resource
$(document).on('click', '.cr-WBD-rsrc-btn-mtrl', function () {
    var prj_id = $("#Prj_Id").val();
    var Activity_Id = $("#ResourceActivity").val();
    var type = 'Material';
    var Inventory = $("#wbdresInventory").val();
    var qty = RemoveComma($("#wbd-res-qty").val());
    var rate = RemoveComma($("#wbd-res-rate").val());
    var total = RemoveComma($("#wbd-res-Amnt").val());
    var Uom = $("#wbd-res-uom").val();
    debugger
    if (Inventory.trim() == null || Inventory.trim() == ""|| Inventory.trim() == "null" || Inventory.trim() == undefined) {
        alert("Please Select Inventory Item");
        return false;
    }
    if (Uom.trim() == null || Uom.trim() == "" || Uom.trim() == undefined) {
        alert("Please Enter UOM");
        return false;
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
        url: '/ConstructProjectManagement/Add_WBDResourceMaterial/',
        data: { Prj_Id: prj_id, Activity_Id: Activity_Id, Type: type, Inventory: Inventory, Qty: qty, Rate: rate, Amount: total, UOM: Uom },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                //($('.main-ms').css('display') == 'none')
                $(".Wbddetails").load("/ConstructProjectManagement/WBDResourcesMaterial", { Prj_Id: data.Id });
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






// create a Work Break Down Labour Resource
$(document).on('click', '.cr-WBD-rsrc-btn-lbr', function () {
    var prj_id = $("#Prj_Id").val();
    var type = 'Labour';
    var Activity_Id = $("#ResourceActivity").val();
    var qty = RemoveComma($("#wbd-res-qty").val());
    var rate = RemoveComma($("#wbd-res-rate").val());
    var total = RemoveComma($("#wbd-res-Amnt").val());
    var phpd = $("#wbd-res-phpd").val();
    debugger
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
    if (phpd.trim() == null || phpd.trim() == 'Select One' || phpd.trim() == "" || phpd.trim() == undefined) {
        alert("Please Select UOM");
        return false;
    }
    debugger
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBDResourceLabour/',
        data: { Prj_Id: prj_id, Activity_Id: Activity_Id, Type: type,  Qty: qty, Rate: rate, Amount: total, Phpd: phpd},
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                //($('.main-ms').css('display') == 'none')
                $(".Wbddetails").load("/ConstructProjectManagement/WbdResourcesLabour", { Prj_Id: data.Id });
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




// create a Work Break Down Equipment Resource
$(document).on('click', '.cr-WBD-rsrc-btn-Eqmnt', function () {
    var prj_id = $("#Prj_Id").val();
    var Activity_Id = $("#ResourceActivity").val();
    var type = 'Equipment';

    var qty = RemoveComma($("#wbd-res-qty").val());
    var rate = RemoveComma($("#wbd-res-rate").val());
    var total = RemoveComma($("#wbd-res-Amnt").val());
    var phpd = $("#wbd-res-phpd").val();
    var mitem = $("#wbdresAssets").val();

    debugger
    
   
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
    if (phpd.trim() == null || phpd.trim() == 'Select One' || phpd.trim() == "" || phpd.trim() == undefined) {
        alert("Please Select UOM");
        return false;
    }
    if (mitem.trim() == null || mitem.trim() == "" || mitem.trim() == undefined) {
        alert("Please Enter Machinery item");
        return false;
    }
    debugger
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Add_WBDResourceEquipment/',
        data: { Prj_Id: prj_id, Activity_Id: Activity_Id, Type: type, Qty: qty, Rate: rate, Amount: total, Phpd: phpd, Mitem: mitem },
        success: function (data) {
            if (data.Status == true) {
                debugger
                alert(data.Msg);
                //($('.main-ms').css('display') == 'none')
                $(".Wbddetails").load("/ConstructProjectManagement/WbdResourcesEquipment", { Prj_Id: data.Id });
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





// add a sub Task Row
$(document).on("click", ".ad-stsk-row", function () {

    if ($(document).find(".remove-stsk-row").length <= 0) {
        var counter = $('#mts-table tr').length - 2;
        var mileid = $(this).closest("tr").attr("id");
        var newRow = $('<tr id="' + mileid + '">');
        var task_amount = $(this).closest('tr').find('.budget_cts').attr("id");
        var task_startdate = $(this).closest('tr').find('.edit-start').attr("id");
        var task_deadline = $(this).closest('tr').find('.edit-dead').attr("id");
        var cols = "";
        cols += `<td><span  class="fas fa-minus-square remove-stsk-row"></span><input type="hidden" value=${task_amount
            } id="budget_cts" /></td>`;
        cols += `<td><input type="hidden" value=${task_startdate} id="edit-start" /><input type="hidden" value=${
            task_deadline} id="edit-dead" /></td>`;
        cols +=
            '<td style="padding:1px"><textarea rows="4" class=" form-control" id="stsk-desc" placeholder="Enter Description"></textarea></td>';
        cols += `<td style="padding:1px;margin:0px"><select class="form-control" id="stsk-unit" style="padding:0px !important; margin:0px !important"><option value="-">-</option><option value = "Cft" >Cft</option><option value="Rft">Rft</option><option value="Nos.">Nos.</option><option value="Each">Each</option><option value="Kg">Kg</option><option value="Job">Job</option><option value="Per/Hour">Per/Hour</option><option value="Sft">Sft</option><option value="Unit">Unit</option></select></td>`;

        cols +=
            '<td style="padding:1px"><input type="text" class="form-control stsk-rate-cal coma" id="stsk-qty" placeholder="Qty"></td>';
        cols +=
            '<td style="padding:1px"><input type="text" class="form-control stsk-rate-cal coma" id="stsk-rate" placeholder="Rate"></td>';
        cols +=
            '<td style="padding:1px"><input type="text" class="form-control coma" id="stsk-total" placeholder="Total Amount" ></td>';
        cols +=
            '<td style="padding:1px"><input type="text" class="form-control " id="stsk-start" data-provide="datepicker" placeholder="Startdate"></td>';
        cols +=
            '<td style="padding:1px"><input type="text" class="form-control " id="stsk-dead" data-provide="datepicker" placeholder="Deadline"> <button style="float:right;margin-top:10px" class="btn btn-primary cr-stsk-btn">Save</button></td>';
        newRow.append(cols);
        newRow.insertAfter($(this).parents().closest('tr'));
        counter++;
    } else {
        alert("Cannot Create two subtask at a glance remove the previous first");
    }

});
// Send Project For approval
$(document).on('click', '.prj-app', function () {
    var id = $("#Prj_Id").val();
    var stat = $(this).data("stat");
    var nar = "";
    if (stat == "Pending") {
        nar = "Are you Sure you want to Send Back for Updation";
    }
    else if (stat == "Pending_Approval") {
        nar = "Are you Sure you want to send for Approval";
    }
    else if (stat == "Approved") {
        nar = "Are you Sure you want to Approve Project";
    }

    if (confirm(nar)) {
        $.post("/ConstructProjectManagement/Update_ProjectStatus/", { Id: id, Status: stat }, function () {
            window.location.reload();
        });
    }
});
// request payment voucher

//
function InitMile(i) {
    $("#add-inv-" + i + " .mile").append('<option value="">Select Milestones</option>');
    $.each(milest, function (key, value) {
        $("#add-inv-" + i + " .mile").append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}

function InitMileByEle(_ele, arr) {
    $(_ele).append('<option value="">Select Milestones</option>');
    $.each(arr, function (key, value) {
        $(_ele).append('<option value="' + value.Value + '">' + value.Text + '</option>');
    });
}

//
$(document).on("click", ".add-row-vou-req", function () {
    InventoryCounter++;
    var html = `<div class="form-row cal vou-req-row">
        <h6 class="lh-1 mB-0 add-inven-counter" style="margin-top: 35px;font-size: 15px;">${InventoryCounter}.</h6>
        <div class="col-md-7 form-row">
            <div class="form-group col-md-4">
                <label>Milestone</label>
                <select class="form-control mile"></select>
            </div>
            <div class="form-group col-md-8">
                <label>Description</label>
                <textarea class="form-control desc" rows="2"></textarea>
            </div>
        </div>
        <div class="col-md-4 form-row">
            <div class="form-group col-md-6">
                <label>Amount</label>
                <input type="text" class="form-control Rate Purchase_Rate" name="Purchase_Rate" data-id="1" placeholder="Total">
            </div>
        </div>
        <div class="col-md-1 form-row">
            <i class="ti-minus rm-vouc-req pointer" style="margin-top: 35px;font-size: 20px;font-weight: bold;margin-left: 2%;"></i>
        </div>
    </div>`;
    let newEle = $('#ad-voc-req').append(html);
    InitMileByEle($(newEle).find('.mile').last(), milest);
});
//
$(document).on("click", ".rm-vouc-req", function () {
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
$(document).on("click", ".req-pay-vouc", function (e) {
    InventoryArray = [];
    var Prj_Id = $('#Prj_Id').val();
    var g_id = $('#g-id').val();

    $('.vou-req-row').each(function () {

        var m_Id = $(this).find(".mile").val();
        var mile = $(this).find(".mile option:selected").text();
        var desc = $(this).find(".desc").val();
        var Quantity = $(this).find(".Quantity").val();
        var Unit = $(this).find(".unit").val();
        var Rate = $(this).find(".Rate").val();
        var total = $(this).find(".Purchase_Rate").val();
        var dataset =
            {
                Prj_Id: Prj_Id,
                Quantity: Quantity,
                Unit: Unit,
                Rate: Rate,
                Total: total,
                Description: desc,
                Milestone: mile,
                Mile_Id: m_Id
            }
        InventoryArray.push(dataset);
    });

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/RequestPaymentVoucher/',
        data: { VoucherDetails: InventoryArray, Groupid: g_id },
        success: function (data) {
            if (data) {
                alert("Requested")
                $('#vouch-list').load('/ConstructProjectManagement/RequestNewVoucher/', { Prj_Id: Prj_Id });
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
$(document).on("click", ".prj-vouch-link", function () {
    var url = $(this).data("link");
    $('#vouch-list').load(url);
});
//
$(document).on("click", ".vouc-req-det", function () {
    EmptyModel();
    var id = $(this).attr("id");
    $('#modalbody').load('/ConstructProjectManagement/PaymentVoucherDetails/', { GroupId: id });
});
//
$(document).on("click", ".upl-boq", function () {
    EmptyModel();
    $('#modalbody').load('/ConstructProjectManagement/UploadBOQ/');
});
// Send Project For approval
$(document).on('click', '#app-prj-pay-vou', function () {
    var groupid = $(this).data("group");
    if (confirm("Are you sure you want to approve Payment request Voucher")) {
        $.post("/ConstructProjectManagement/Update_Payment_Status/", { GroupId: groupid, Status: "Approved" }, function (data) {
            //window.location.reload();
            if (data == true) {
                window.open("/Vouchers/Payment_Voucher_Req?Id=" + groupid, '_blank');
            }
        });
    }
});
//
$(document).on("click", ".edit-mts", function () {
    EmptyModel();
    var id = $(this).attr("id");
    $('#modalbody').load('/ConstructProjectManagement/MTS_Update_Details/', { Id: id });
});
// Update Req a milestone
$(document).on("click", "#mts-up-req", function (e) {
    var prj_id = $("#Prj_Id").val();
    var title = $("#mile-title").val();
    var desc = $("#mile-desc").val();
    var qty = $("#mile-qty").val();
    var rate = RemoveComma($("#mile-rate").val());
    var unit = $("#mile-unit").val();
    var total = RemoveComma($("#mile-total").val());
    var deadline = $("#mile-dead").val();
    var startdate = $("#mile-start").val();
    var mts_id = $("#mts_id").val();
    var Mts = $("#MTS").val();
    var dep = $("#Department").val();
    if (confirm("Are you sure you want Request")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/Update_MTS_Request/',
            data: { Prj_Id: prj_id, Title: title, Description: desc, Qty: qty, Rate: rate, Unit: unit, Amount: total, Deadline: deadline, MTS_Id: mts_id, MTS: Mts, Start_Date: startdate, DepId: dep },
            success: function (data) {
                if (data === 0) {
                    alert("This Milestone already Exist");
                }
                else {
                    alert("Update Requested");
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
$(document).on("click", ".mts-stat-det", function () {
    EmptyModel();
    var prj_id = $("#Prj_Id").val();
    var type = $(this).data("type");
    var stat = $(this).data("stat");
    $('#modalbody').load('/ConstructProjectManagement/MTS_Reqs_Status/', { Prj_Id: prj_id, Type: type, Status: stat });
});
// Update Req a milestone
$(document).on("change", ".up-req-stat", function (e) {
    var req_id = $(this).data("reqid");
    var stat = $(this).val();
    if (confirm("Are you sure you want Update Request")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/Update_MTS_Status_Req/',
            data: { Req_Id: req_id, Status: stat },
            success: function (data) {
                //alert("Request Updated")
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

///// Discussion Post
// create a Post
$(document).on("click", "#ad-discussion", function () {
    var des = tinymce.get('description').getContent();
    var prjid = $('#Prj_Id').val();
    var token = 1;
    if (des == "" || des == null) {
        alert('Please Enter description');
        return false;
    }
    //var id = $('input[name=Id]').val();
    //id = id.split(',');

    var url = [];
    var id = [];
    //$('.fileurl').each(function () {
    //    var val = $(this).val();
    //    url.push(val)
    //});

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/AddPost/',
        data: { Description: des, Prj_Id: prjid, Id: id, URLs: url, Token: token },
        success: function (data) {
            if (data == true) {
                alert("Successfully Submitted");
                tinyMCE.activeEditor.setContent('');
                $("#posts").load("/ConstructProjectManagement/GetPosts/", { Prj_Id: prjid })
            }
            else if (data == 0) {
                alert("Post Already Exists")
            }
            else {
                alert("Please Enter the Details");
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
// Delete Post
$(document).on("click", ".deletePost", function () {
    var id = $(this).data('postid');
    var prjid = $('#Prj_Id').val();
    if (confirm("Are you sure you want to Delete this Post")) {
        $.ajax({
            type: "POST",
            url: "/ConstructProjectManagement/DeletePost/",
            data: { Post_Id: id },
            success: function (data) {
                alert("Successful deleted");
                $("#posts").load("/ConstructProjectManagement/GetPosts/", { Prj_Id: prjid })
            },
            error: (xmlhttprequest, textstatus, message) => {
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });

    }
});
// Add Comment
$(document).on("click", ".postComment", function () {
    var postid = $(this).data("postid");
    var com = $('#commentReplay-' + postid).val();
    if (com == "" || com == null) {
        alert('Please Enter the Comment first');
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/AddComment/',
        data: { Comment: com, Post_Id: postid },
        success: function (data) {
            if (data == true) {
                var markUp1 = "<div class='row mt-0'><div class='col-md-1 pr-0 mr-0'><img src='/assets/static/images/404.png' class='rounded-circle'  style='height: 30px !important; width: 30px !important;'></div><div class='col-md-11 pl-0 ml-0'><div class='jumbotron p-0'><div class='container pl-0 pr-0 pr-5'><p>" + com + "</p></div></div></div></div>"
                $("#addForComments-" + postid).append(markUp1);
                $('#commentReplay-' + postid).val('');
            }
            else {
                alert("Something went wrong try again");
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
$(document).on("change", ".mile-stat-up", function () {
    var $this = $(this)
    var chkstat = $(this).is(":checked");
    var id = $(this).val();
    var nar = (chkstat) ? "Are you sure you want to Mark this Milestone Complete" : "Are you sure you want to Mark this Milestone Pending";
    if (confirm(nar)) {
        $.post('/ConstructProjectManagement/UpdateMTS_Status/', { Mts_Id: id, Status: chkstat }, function (data) {
            if (!data) {
                alert("System Not Responding");
                $this.removeAttr("checked")
            }
            else {
                alert("Milestone Updated");

            }
        }).fail(function () {
            alert("System Not Responding");
            $($this).removeAttr("checked")
        });
    }
    else {
        $($this).removeAttr("checked")
    }

});
// For View of Edit Post
$(document).on("click", ".ed-post", function () {
    EmptyModel();
    var id = $(this).data("postid");
    $('#modalbody').load('/ConstructProjectManagement/EditPost/', { Post_Id: id });
});
// Add Comment
$(document).on("click", "#up-post", function () {
    var postid = $('#post-id').val();
    var com = tinyMCE.get('post id').getContent()
    if (com == "" || com == null) {
        alert('Please Enter the Comment first');
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/UpdatePost/',
        data: { Post: com, Post_Id: postid },
        success: function (data) {
            if (data == true) {
                //var markUp1 = "<div class='row mt-0'><div class='col-md-1 pr-0 mr-0'><img src='/assets/static/images/404.png' class='rounded-circle'  style='height: 30px !important; width: 30px !important;'></div><div class='col-md-11 pl-0 ml-0'><div class='jumbotron p-0'><div class='container pl-0 pr-0 pr-5'><p>" + com + "</p></div></div></div></div>"
                //$("#addForComments-" + postid).append(markUp1);
                //$('#commentReplay-' + postid).val('');
            }
            else {
                alert("Something went wrong try again");
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

//////////////////////// Validations for Edit


//new function for rate and quantity
$(document).on('keyup', '.tem', function () {
    var t = $(this).closest('tr').find('.savat');
    var r = RemoveComma($(this).closest('tr').find('.savar').val());
    var q = RemoveComma($(this).closest('tr').find('.savaq').val());
    t.val(Number(parseFloat(r * q).toFixed(2)).toLocaleString());
});
$(document).on("keyup", ".savat", function () {
    var tem = $(this).val();
    $(".savaq").val(null);
    $(".savar").val(null);
});

$(document).on('blur', '.tem, .savat', function () {
    var t = $(this).closest('tr').find('.savat');
    var q = $(this).closest('tr').find('.savaq');
    var r = $(this).closest('tr').find('.savar');
    var Id = parseInt($(this).closest('tr').attr('id'));

    var t_val = RemoveComma(t.val());
    var q_val = RemoveComma(q.val());
    var r_val = RemoveComma(r.val());

    if (isNaN(RemoveComma(t_val)) || t_val === "" || t_val == null) {
        alert("Invalid rate enter numbers only");
        return false;
    }
    else if (RemoveComma(t_val) === 0 || RemoveComma(t_val) < 0) {
        alert("Invalid rate cannot be zero or negative");
        return false;
    }
    {
        //if (isNaN(RemoveComma(r_val)) || r_val == "" || r_val == null) {
        //    alert("Invalid rate enter numbers only");
        //    return false;
        //}
        //else if (RemoveComma(r_val) === 0 || RemoveComma(r_val) < 0) {
        //    alert("Invalid rate cannot be zero or negative");
        //    return false;
        //}
        //else if (isNaN(RemoveComma(q_val)) || q_val == "" || q_val == null) {
        //    alert("Invalid quantity enter numbers only");
        //    return false;
        //}
        //else if (RemoveComma(q_val) === 0 || RemoveComma(q_val) < 0) {
        //    alert("Invalid quantity cannot be zero or negative");
        //    return false;
        //}
    }

    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/UpdateMTS/',
        data: { Rate: r_val, Qty: q_val, Amount: t_val, Id: Id },
        success: function (data) {
            if (data.Status == true) {
                //alert("Updated");
                t.parent().replaceWith('<td class="edit-total in st">' + t.val() + '</td>');
                q.parent().replaceWith('<td class="edit-qty in st">' + q.val() + '</td>');
                r.parent().replaceWith('<td class="edit-rate in st">' + r.val() + '</td>');
            }
            else {
                alert(data.Msg);
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
//new function for budget
$(document).on("click", ".st", function () {
    var $this = $(this);
    if ($(".savat")[0]) {
        return false
    }
    var q = $(this).closest('tr').find(".edit-qty");
    var r = $(this).closest('tr').find(".edit-rate");
    var t = $(this).closest('tr').find(".edit-total");
    var inputt = `<td class="t"><input type="text" class="savat form-control coma" value="` + t.text() + `" /></td>`;
    var inputr = `<td class="r"><input type="text" class="savar tem form-control coma" value="` + r.text() + `" /></td>`;
    var inputq = `<td class="q"><input type="text" class="savaq tem form-control coma" value="` + q.text() + `" /></td>`;
    t.replaceWith(inputt);
    r.replaceWith(inputr);
    q.replaceWith(inputq);
    if ($(this).hasClass('edit-qty')) {
        $(".savaq").focus();
    }
    else if ($(this).hasClass('edit-rate')) {
        $(".savar").focus();
    }
    else if ($(this).hasClass('edit-total')) {
        $(".savat").focus();
    }
});
$(document).on('keyup', '.savablebgt', function () {
    var t = $(this);
    var r = $(this).closest('tr').find('.savar');
    var q = $(this).closest('tr').find('.savaq');
    q.attr('disabled', true).val(0);
    r.attr('disabled', true).val(0);
});
$(document).on('blur', '.savablebgt', function () {
    var t = $(this);
    var tr = $(this).closest('tr').find(".i");
    var q = $(this).closest('tr').find(".savaq");
    var qr = $(this).closest('tr').find(".q");
    var r = $(this).closest('tr').find(".savar");
    var rr = $(this).closest('tr').find(".r");
    var input = $('<td>').attr('class', 'budget_cts in').attr('id', `${t.val()}`).text(t.val());
    var inputr = $('<td>').attr('class', 'edit-rate in st').text(r.val());
    var inputq = $('<td>').attr('class', 'edit-qty in st').text(q.val());
    var Id = parseInt($(this).closest('tr').attr('id'));
    if (isNaN(parseInt(t.val()))) {
        alert("Invalid amount enter numbers only");
        return false;
    } else if (parseInt(t.val()) === 0 || parseInt(t.val()) < 0) {
        alert("Invalid amount cannot be zero or negative");
        return false;
    } else if (t.val() == "" || t.val() == null || t.val() == undefined) {
        alert("Please enter the task amount first");
        return false;
    } else {
        $.post("/ConstructProjectManagement/getData/", { row_Id: Id }, function (data) { }).done(function (data) {
            data = JSON.parse(data);
            if (Object.keys(data).length == 3) {
                if ((parseFloat(RemoveComma(t.val())).toFixed(2)) > parseFloat(data.project.Budget).toFixed(2)) {
                    alert("Milestone Budget cannot exceed project budget");
                    return false;
                } else {
                    $.post("/ConstructProjectManagement/getDataForVal/", { row_Id: Id, type: data.mts.Type, Mts: data.mts.MTS_Id }, function (data) { }).done(
                        function (e) {
                        }).done(function (e) {
                            if (e == "true") {
                                tr.replaceWith(input);
                                qr.replaceWith(inputr);
                                rr.replaceWith(inputq);
                                alert("successfully updated the amount 5");
                                q.attr('disabled', false).val(0);
                                r.attr('disabled', false).val(0);
                                //write here
                            } else {
                                if ((parseFloat(data.project.Budget).toFixed(2) - (parseFloat(RemoveComma(e)).toFixed(2))) < (parseFloat(RemoveComma(t.val())).toFixed(2))) {
                                    alert("3 You can set maximun value of " +
                                        (parseFloat(data.project.Budget).toFixed(2) - parseFloat(e).toFixed(2)));
                                    return false;
                                } else {
                                    tr.replaceWith(input);
                                    qr.replaceWith(inputr);
                                    rr.replaceWith(inputq);
                                    alert("successfully updated the amount 6");
                                    q.attr('disabled', false).val(0);
                                    r.attr('disabled', false).val(0);
                                    //write here
                                }
                            }
                        });
                }
            } else if (Object.keys(data).length == 2) {
                if (parseFloat(data.parent.Amount).toFixed(2) < (parseFloat(RemoveComma(t.val())).toFixed(2))) {
                    alert(data.parent.Type + " amount cannot be greater than" + data.mts.Type);
                    return false;
                } else {
                    $.post("/ConstructProjectManagement/getDataForVal/", { row_Id: Id, type: data.mts.Type, Mts: data.mts.MTS_Id }, function (data) { }).done(
                        function (e) {

                        }).done(function (e) {
                            if (e == "true") {
                                tr.replaceWith(input);
                                qr.replaceWith(inputr);
                                rr.replaceWith(inputq);
                                alert("Successfully updated 7");
                                q.attr('disabled', false).val(0);
                                r.attr('disabled', false).val(0);
                                //write here for save
                            } else {
                                if ((parseFloat(RemoveComma(t.val())).toFixed(2)) > (parseFloat(data.parent.Amount).toFixed(2) - (parseFloat(RemoveComma(e)).toFixed(2)))) {

                                    alert("4 You can set maximum value of " +
                                        (parseFloat(data.parent.Amount).toFixed(2) - parseFloat(e).toFixed(2)));
                                    return false;
                                } else {
                                    tr.replaceWith(input);
                                    qr.replaceWith(inputr);
                                    rr.replaceWith(inputq);
                                    alert("Successfully updated 8");
                                    q.attr('disabled', false).val(0);
                                    r.attr('disabled', false).val(0);
                                    //write here for save
                                }
                            }
                        });
                }
            }
        });
    }
});
//Edit Start
$(document).on('click', '.edit-start', function () {
    var t = $(this);
    var startdate = t.attr('id');
    var input = `<td id="m-s"><input type="text" class="form-control " id="mile-start-edit" data-provide="datepicker" placeholder="Start Date" value=${startdate}></td>`;
    t.replaceWith(input);
    $('#mile-start-edit').focus();
});
$(document).on('blur', '#mile-start-edit', function () {
    var input = $(this);
    var date = $(this).val();
    if ($(".datepicker")[0]) {
        return false;
    }
    else if (input.val().trim() == null || input.val().trim() == undefined || input.val().trim() == '') {
        alert('Start date cannot be null or empty');
        return false;
    }
    var Id = parseInt($(this).closest('tr').attr('id'));
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Update_Start_End/',
        data: { Date: date, S_D: 'Start', Id: Id },
        success: function (data) {
            if (data.Status == true) {
                //alert("Updated");
                $("#m-s").replaceWith('<td id="' + date + '" class="edit-start">' + moment(date).format("DD-MMM-YYYY") + '</td>')
            }
            else {
                alert(data.Msg);
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
// Edit Deadline
$(document).on('click', '.edit-dead', function () {
    var t = $(this);
    var deaddate = t.attr('id');
    var input = `<td id="m-d"><input type="text" class="form-control " id="mile-dead-edit" data-provide="datepicker" placeholder="Start Date" value=${deaddate}></td>`;
    t.replaceWith(input);
    $('#mile-dead-edit').focus();
});
$(document).on('blur', '#mile-dead-edit', function () {
    var input = $(this);
    var date = $(this).val();
    if ($(".datepicker")[0]) {
        return false;
    } else if (input.val().trim() == null || input.val().trim() == undefined || input.val().trim() == '') {
        alert('Deadline cannot be null or empty');
        return false;
    }
    var Id = parseInt($(this).closest('tr').attr('id'));
    $.ajax({
        type: "POST",
        url: '/ConstructProjectManagement/Update_Start_End/',
        data: { Date: date, S_D: 'Deadline', Id: Id },
        success: function (data) {
            if (data.Status == true) {
                //alert("Updated");
                $("#m-d").replaceWith('<td id="' + date + '" class="edit-dead">' + moment(date).format("DD-MMM-YYYY") + '</td>')
                //
            }
            else {
                alert(data.Msg);
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
//for title
$(document).on('click', '.edit-titl', function () {
    var t = $(this);
    var input = '<td><input class="savabletitl  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savabletitl").focus();
});
$(document).on('blur', '.savabletitl', function () {

    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid title enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/EditMTS_Title_Desc/', { Value: input.val(), Attr: 'title', Id: id }, function (data) {
            if (data) {
                input.parent().attr('class', 'edit-titl td');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});

//for desc
$(document).on('click', '.edit-desc', function () {
    var t = $(this);
    var input = '<td><input class="savabledesc  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savabledesc").focus();
});
$(document).on('blur', '.savabledesc', function () {
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid discription enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/EditMTS_Title_Desc/', { Value: input.val(), Attr: 'desc', Id: id }, function (data) {
            if (data) {
                var t = input.val();
                input.parent().attr('class', 'edit-desc td');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }
        });
    }
});
// for unit
$(document).on('click', '.edit-unit', function () {
    var t = $(this);
    var input = `<td style="padding:1px; margin:0px" class="ur"><select class="form-control ud" style="padding:0px !important; margin:0px !important">` + unitlist + `</select></td>`;
    t.replaceWith(input);
    $('.ud').focus();
});
$(document).on('blur', '.ud', function () {

    var input = $(this);
    var id = $(this).closest('tr').attr("id");
    $.post('/ConstructProjectManagement/EditMTS_Title_Desc/', { Value: input.val(), Attr: 'unit', Id: id }, function (data) {
        if (data) {
            var t = input.val();
            input.parent().attr('class', 'edit-unit ui');
            input.replaceWith(input.val());
        }
        else {
            alert("Cannot save right now")
        }
    });

});


// For New Version
$(document).on("click", ".new-ver", function () {
    EmptyModel();
    var folderpath = $(this).data("url");
    var fname = $(this).data("fname");
    $('#modalbody').load('/FileHandling/NewVersion/', { FileName: fname, FolderPath: folderpath });
});

$(document).on("click", ".ad-ms-row", function () {
    //++countforrow;
    let countforrow = $('.msi-row').length + 1;
    //   var newaddition = `<div class="form-row msi-row"><div class="form-group col-md-1 ift-m justify-content-center align-items-center d-flex">
    //           <i class="fas fa-plus-square ad-ms-row fa-lg pointer"></i>
    //           <i class="fas fa-minus-square fa-trash remove-ms-row pointer"></i>
    //       </div>
    //       <div class="form-group col-md-4">
    //           <input type="text" class="form-control mile-item"  placeholder="Items" id="mile-nm-item-${countforrow}" autocomplete="off">
    //       </div>
    //       <div class="form-group col-md-4">
    //           <input type="text" class="form-control mile-qty-item" id="mile-qty-item-${countforrow}" placeholder="Item Quantity" autocomplete="off">
    //       </div>
    //       <div class="form-group col-md-3 ift">
    //           <select class="form-control mile-ms-unit" id="mile-ms-unit-${countforrow}"></select>
    //<input type="text" name="name" value="" class="form-control mt-2 other" id="other-${countforrow}" style="display:none;" />
    //       </div></div>`;

    var newaddition2 = `<div class="form-row msi-row">
        <div class="form-group col-md-1 ift-m justify-content-center align-items-center d-flex">
            <i class="fas fa-plus-square ad-ms-row fa-lg pointer"></i>
            <i class="fas fa-minus-square fa-lg remove-ms-row pointer ml-2"></i>
        </div>
        <div class="form-group col-md-4">
            <input type="text" class="form-control mile-item" id="mile-nm-item-${countforrow}" placeholder="Material" autocomplete="off">
        </div>

        <div class="form-group col-md-1">
            <input type="number" class="form-control mile-qty-item" id="mile-len-item-${countforrow}" placeholder="Length" autocomplete="off" maxLength = '5'>
        </div>
        <div class="form-group col-md-1">
            <input type="number" class="form-control mile-qty-item" id="mile-wid-item-${countforrow}" placeholder="Width" autocomplete="off" maxLength = '5'>
        </div>
        <div class="form-group col-md-1">
            <input type="number" class="form-control mile-qty-item" id="mile-hei-item-${countforrow}" placeholder="Height" autocomplete="off" maxLength = '5'>
        </div>
        <div class="form-group col-md-1">
            <input type="number" class="form-control mile-qty-item" id="mile-dia-item-${countforrow}" placeholder="Dia" autocomplete="off"  maxLength = '5'>
        </div>

        <div class="form-group col-md-1">
            <input type="number" class="form-control mile-qty-item" id="mile-qty-item-${countforrow}" placeholder="Quantity" autocomplete="off" maxLength = '5'>
        </div>
        <div class="form-group col-md-1 ift">
            <select class="form-control mile-ms-unit" id="mile-ms-unit-${countforrow}"></select>
        </div>
        <div class="form-group col-md-1 ift">
            <input type="text" name="name" value="" class="form-control mt-2 other in-active" id="other-${countforrow}" placeholder='UOM' maxLength = '5'>
        </div>
        <div class="col-md-1"></div>
        <div class="form-group col-md-4 ift">
            <textarea class="form-control mt-2" id = "mile-rem-item-${countforrow}" rows="10" placeholder="Remarks"></textarea>
        </div>
        <div class="form-group col-md-6 ift">
            <textarea class="form-control mt-2" rows="10" id = "mile-desc-item-${countforrow}" placeholder="Description"></textarea>
        </div>
    </div>`;

    $('.main-ms').find('.msi-row').last().after(newaddition2);
    $(`#mile-ms-unit-${countforrow}`).append(unitlist);
    PopulateDropDownList($('#mile-ms-unit-' + countforrow)[0]);
    $('#mile-ms-unit-' + countforrow).on('change', function () {
        console.log('change detected');
        console.log($(this).children("option:selected").val());
        if ($(this).children("option:selected").val() == "Other") {
            $(this).parent('.ift').next('.ift').children('.other').removeClass('in-active');
        }
        else {
            if ($(this).parent('.ift').next('.ift').children('.other').hasClass('in-active')) {
                return;
            }
            $(this).parent('.ift').next('.ift').children('.other').addClass('in-active')
        }
    });
});
$(document).on("click", ".remove-ms-row", function () {
    $(this).closest(".msi-row ").remove();
});

function PopulateDropDownList(dropDownElement) {
    //Build an array containing Customer records.
    var customers = [
        { Id: 1, Value: "KG" },
        { Id: 2, Value: "MG" },
        { Id: 3, Value: "Litre" },
        { Id: 4, Value: "ML" },
        { Id: 5, Value: "Bag" },
        { Id: 6, Value: "Bottle" },
        { Id: -999, Value: "Other" }
    ];

    var ddlCustomers = dropDownElement;

    //Add the Options to the DropDownList.
    for (var i = 0; i < customers.length; i++) {
        var option = document.createElement("OPTION");

        //Set Customer Name in Text part.
        option.innerHTML = customers[i].Value;

        //Set CustomerId in Value part.
        option.value = customers[i].Value;

        //Add the Option element to DropDownList.
        ddlCustomers.options.add(option);
    }
}
////
$(document).on("click", ".multi-dems", function () {
    EmptyModel();
    $('#ModalLabel').text("Demand Order");
    var id = $("#Prj_Id").val();
    $('#modalbody').load('/Inventory/MultiItemIssueRequest/', { Prj_Id: id });
});
////
$(document).on("click", ".multi-dems-com", function () {
    EmptyModel();
    $('#ModalLabel').text("Company Demand Issuance");
    var id = $("#Prj_Id").val();
    $('#modalbody').load('/Inventory/MultiItemIssueRequest_Comp/', { Prj_Id: id });
});
////
$(document).on("click", ".multi-dems-req", function () {
    EmptyModel();
    $('#ModalLabel').text("Demand Order");
    var req = $(this).data('req');
    $('#modalbody').load('/Inventory/ItemIssueDemRequest/', {  Req: req });
});


//
$(document).on("click", ".ser-req", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/NewService_Requisition?Id=' + id);
});
//
$(document).on("click", ".oth-exp", function () {
    var id = $("#Prj_Id").val();
    window.open('/Services/OtherExpense?Id=' + id);
});
//
$(document).on("click", "#save-multi-issue_rqst", function (e) {
    e.preventDefault();
    var sbmsnInfo = [];
    var notok = false;
    var dr = $("#dr").val();
    var prj = "", proj_name = "";

    prj = $("#Prj_Id").val();
    if (prj == null) {
        prj = $(".Project").val();
        if (prj == "") {
            alert("Select Project ");
            notok = true;
            return;
        }
    }

    proj_name = $("#Prj_Name").text();
    if (proj_name == null) {
        proj_name = $(".Project").text();
    }

    var grp = $("#grp-id").val();
    var empl = $(".emp option:selected").val();
    if (isBlank(empl) || isEmpty(empl)) {
        alert("Please Select Employee")
        notok = true;
        return false;
    }
    var emp_Name = $(".emp option:selected").text();
    var comp = $("#Company option:selected").val();
    var department = $(".Department option:selected").val();


    $('.inv-assign-row').each(function () {
        var item = $(this).find(".item-name").val();
        var product_name = $(this).find(".item-name").text();
        var iqty = $(this).find(".req-qty").val();
       
        
        if (iqty == "" || iqty == undefined || iqty == 0) {
            alert("Requested Quantity Cannot be Empty or 0")
            notok = true;
            return false;
        }
        var tqty = $(this).find('.ttl-qty').val();
        if (tqty == "" || tqty == undefined || tqty == 0) {
            alert("Items are not available.")
            notok = true;
            return false;
        }
        var rems = $(this).find('.item-rems').val();
        if (parseInt(iqty) > parseInt(tqty)) {
            alert('This item is not available in this quantity. Check again or submit a purchase request.');
            notok = true;
            return;
        }
        if (sbmsnInfo.filter(x => x.item == item).length > 0) {
            alert(product_name + " can not be repeat again");
            notok = true;
            return false;
        }

        sbmsnInfo.push({
            item: item,
            qty: iqty,
            rem: rems
        });
    });

    if (!notok) {
        if (confirm("Are you sure you want to forward this issue request?")) {
            $.ajax({
                type: "POST",
                url: '/Inventory/SaveMultiIssueRequest/',
                data: { items: sbmsnInfo, Group_Id: grp, proj: prj, prj_name: proj_name, Demand_Req: dr, Emp_Id: empl, Emp_Name: emp_Name, To_Comp_Id: comp, Department: department},
                success: function (data) {
                    if (data.Status) {
                        alert(data.Msg);
                        $("#save-multi-issue_rqst").prop("Disabled", true);
                        closeModal();
                    }
                    else {
                        alert(data.Msg);
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

////
//$(document).on("click", "#save-dem-req", function (e) {
//    e.preventDefault();
//    var sbmsnInfo = [];
//    var dr = $("#dr").val();
//    var prj = $("#Prj_Id").val();
//    var proj_name = $("#Prj_Name").text();
//    var grp = $("#grp-id").val();
//    $('.inv-assign-row').each(function () {
//        var item = $(this).find(".item-id").val();
//        var iqty = $(this).find(".req-qty").val();
//        var empl = $(this).find(".emp option:selected").val()
//        if (isBlank(empl) || isEmpty(empl)) {
//            alert("Please Select Employee")
//            return false;
//        }
//        if (isBlank(iqty) || isEmpty(iqty) || iqty == 0) {
//            alert("Requested Quantity Cannot be Empty or 0")
//            return false;
//        }
//        var emp_Name = $(this).find(".emp option:selected").text()
//        var tqty = $(this).find('.ttl-qty').val();
//        var rems = $(this).find('.item-rems').val();
//        if (parseInt(iqty) > parseInt(tqty)) {
//            alert('This item is not available in this quantity. Check again or submit a purchase request.');
//            return;
//        }
//        sbmsnInfo.push({
//            item: item,
//            emp: empl,
//            emp_Name: emp_Name,
//            qty: iqty,
//            rem: rems
//        });
//    });

//    var con = confirm("Are you sure you want to forward this issue request?");
//    if (con) {
//        $.ajax({
//            type: "POST",
//            url: '/Inventory/SaveMultiIssueRequest/',
//            data: { items: sbmsnInfo, Group_Id: grp, proj: prj, prj_name: proj_name, Demand_Req: dr},
//            success: function (data) {
//                if (data.Status) {
//                    alert(data.Msg);
//                }
//                else {
//                    alert(data.Msg);
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
$(document).on("click", ".del-mat-stat", function () {
    var Id = $(this).attr("id");
    var prj_id = $("#Prj_Id").val();
    if (confirm("Are you sure you want to delete this item.")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/DeleteMaterialStatument/',
            data: { Id: Id },
            success: function (data) {
                alert("Statment Deleted");
                /**/
                $("#prjdetails").load("/ConstructProjectManagement/NewMaterialStatement?proj=" + prj_id)
                /**/
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
$(document).on("click", ".del-prj-at", function () {
    var Id = $(this).attr("id");
    var prj_id = $("#Prj_Id").val();
    if (confirm("Are you sure you want to delete this Attendee.")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/DeletePrjAttendee/',
            data: { Id: Id },
            success: function (data) {
                alert("Attendee Deleted");
                $(".prj-attt").load("/ConstructProjectManagement/PrjDepartments", { Prj_Id: prj_id });
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
$(document).on("click", ".add-prj-attend", function () {
    EmptyModel();
    $('#ModalLabel').text("Add Attendee");
    var id = $("#Prj_Id").val();
    $('#modalbody').load('/ConstructProjectManagement/AddPrjAttendee/', function () {
        $('.modal-footer').append('<button class="btn btn-primary ad-prj-att-btn" type="button">Add Attendees</button>');
    });
});
//
$(document).on("click", ".ad-prj-att-btn", function () {
    var us = [];
    var ids = $("#deps").val();
    $.each(ids, function (a) {
        var id = $(".dep-sel-" + ids[a]).val();
        $.each(id, function (c) {
            us.push(id[c]);
        });
    });
    var prj_id = $("#Prj_Id").val();
    if (confirm("Are you sure you want to Add these Attendee.")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/AddAttendee/',
            data: { Emp: us, Prj_Id: prj_id },
            success: function (data) {
                alert("Attendee Added");
                $(".prj-attt").load("/ConstructProjectManagement/PrjDepartments", { Prj_Id: prj_id });
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
$(document).on("click", "#up-boq", function (e) {
    var prj = $('#Prj_Id').val();
    $.each(AllData, function (i, v) {
        v.Amount = $.trim(RemoveComma(v.Amount));
        v.Rate = $.trim(RemoveComma(v.Rate));
        v.Qty = $.trim(RemoveComma(v.Qty));
    });
    if (confirm("Are you sure you want to Upload the BOQ")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/Upload_BOQ/',
            data: { AllData: AllData, Prj_Id: prj },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
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

$(document).on("click", "#up-mtst", function (e) {
    var prj = $('#Prj_Id').val();
    $.each(AllData, function (i, v) {
        v.Amount = $.trim(RemoveComma(v.Amount));
        v.Rate = $.trim(RemoveComma(v.Rate));
        v.Qty = $.trim(RemoveComma(v.Qty));
    });
    if (confirm("Are you sure you want to Upload the Material Statement")) {
        $.ajax({
            type: "POST",
            url: '/ConstructProjectManagement/Upload_MaterialStatement/',
            data: { AllData: AllData, Prj_Id: prj },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
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


//for title
$(document).on('click', '.edit-u-wbd-blg', function () {
    var t = $(this);
    var cols = "";
    cols += `<td style="padding:0px; margin:0px">
    <select class="form-control saveUnitBilling" style="padding:0px !important; margin:0px !important">
    <option value="-">-</option>
    <option value="No">No</option>
    <option value="RFT">RFT</option>
    <option value="Pack">Pack</option>
    <option value="Each">Each</option>
    <option value="Litre">Litre</option>
    <option value="Kg">Kg</option>
    <option value="Bag">Bag</option>
    <option value="Foot">Foot</option>
    <option value="Bundle">Bundle</option>
    <option value="Unit">Unit</option>
    </select></td>`;
    debugger
    t.replaceWith(cols);
    $(".saveUnitBilling").focus();
});
$(document).on('blur', '.saveUnitBilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid UOM enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_u_wbd_blg/', { Value: input.val(), Attr: 'U', Id: id }, function (data) {
            if (data) {
                debugger
                input.parent().attr('class', 'edit-u-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});


//for title
$(document).on('click', '.edit-l-wbd-blg', function () {
    var t = $(this);
    var input = '<td><input class="savelengthbilling  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savelengthbilling").focus();
});
$(document).on('blur', '.savelengthbilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid Length enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_l_wbd_blg/', { Value: input.val(), Attr: 'L', Id: id }, function (data) {
            if (data) {
                debugger
                input.parent().attr('class', 'edit-l-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});




//for title
$(document).on('click', '.edit-w-wbd-blg', function () {
    var t = $(this);
    var input = '<td><input class="savewidthbilling  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savewidthbilling").focus();
});
$(document).on('blur', '.savewidthbilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid Width enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_w_wbd_blg/', { Value: input.val(), Attr: 'W', Id: id }, function (data) {
            if (data) {
                debugger
                input.parent().attr('class', 'edit-w-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});

//for title
$(document).on('click', '.edit-h-wbd-blg', function () {
    var t = $(this);
    var input = '<td><input class="saveheighthbilling  form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".saveheighthbilling").focus();
});
$(document).on('blur', '.saveheighthbilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid Height enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_h_wbd_blg/', { Value: input.val(), Attr: 'H', Id: id }, function (data) {
            if (data) {
                debugger
                input.parent().attr('class', 'edit-h-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});

//for title
$(document).on('click', '.edit-b-wbd-blg', function () {
    var t = $(this);
    var input = '<td><input class="savebbilling  form-control"  value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".savebbilling").focus();
});
$(document).on('blur', '.savebbilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid Bill enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_b_wbd_blg/', { Value: input.val(), Attr: 'B', Id: id }, function (data) {
            if (data) {
                debugger
                input.parent().attr('class', 'edit-b-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});

//for title
$(document).on('click', '.edit-a-wbd-blg', function () {
    var t = $(this);
    debugger
    var input = '<div class="WBDBilling"></div> <td><input class="saveamountbilling WBDBilling form-control" value="' + t.text() + '"></td>';
    t.replaceWith(input);
    $(".saveamountbilling").focus();
});
$(document).on('blur', '.saveamountbilling', function () {
    debugger
    var input = $(this);
    var id = $(this).closest('tr').attr('id');
    if (input.val().trim() == "" || input.val().trim() == null) {
        alert("Invalid Amount enter again");
        return false;
    } else {
        $.post('/ConstructProjectManagement/edit_a_wbd_blg/', { Value: input.val(), Attr: 'amount', Id: id }, function (data) {
            if (data) {
                debugger
                $(".WBDBilling").load("/ConstructProjectManagement/Get_WBDMeasurementBillingList");
                input.parent().attr('class', 'edit-a-wbd-blg');
                input.replaceWith(input.val());
            }
            else {
                alert("Cannot save right now")
            }

        });
    }
});

//// create a role
//$(document).on("click", ".Finalize-blng-prj-btn", function (e) {
//    var PrjId = $("#Prj_Id").val();
//    var Vendorid = $("#Vendors").val();
//    debugger
//    var Wbdcheck = $("#Wbdcheck").is(":checked");

 

//    if (confirm("Are your sure you want to Finalize ")) {
//        $.ajax({
//            type: "POST",
//            url: '/ConstructProjectManagement/CreateProject/',
//            data: { ProjectName: prjnam, Emp: us, PrjType: tpv, Offsite: os, AccountId: accHead },
//            success: function (data) {
//                if (data === 0) {
//                    alert("This Project already Exist");
//                }
//                else {
//                    alert("Project Created");
//                    window.location.reload();
//                    window.open("/ConstructProjectManagement/ProjectConfiguration?Id=" + data, '_blank');
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