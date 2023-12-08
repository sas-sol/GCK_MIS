//
function GetReqCount() {
    $.ajax({
        type: "POST",
        url: '/Procurement/GetReqcount',
        success: function (data) {
            if (data.PendingApproval > 0) {
                $('#pen-ap').html('<span>Pending Requisitions</span><div class="req-count">' + data.PendingApproval + '</div>');
            }
            else {
                $('#pen-ap').html('<span>Pending Requisitions</span>');
            }
            if (data.Pending > 0) {
                $('#pen').html('<span>Approved Requisitions</span><div class="req-count">' + data.Pending + '</div>');
            }
            else {
                $('#pen').html('<span>Approved Requisitions</span>');
            }
            if (data.QuotationFinal > 0) {
                $('#q-f').html('<span>Comparative Statement</span><div class="req-count">' + data.QuotationFinal + '</div>');
            }
            else {
                $('#q-f').html('<span>Comparative Statement</span>');
            }
            if (data.Demand_Approval > 0) {
                $('#d-a').html('<span>Demand Approval</span><div class="req-count">' + data.Demand_Approval + '</div>');
            }
            else {
                $('#d-a').html('<span>Demand Approval</span>');
            }
            if (data.PurchaseOrder > 0) {
                $('#po').html('<span>Purchase Orders</span><div class="req-count">' + data.PurchaseOrder + '</div>');
            }
            else {
                $('#po').html('<span>Purchased Orders</span>');
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
//
//
function GetSerCount() {
    $.ajax({
        type: "POST",
        url: '/Services/GetSerCount',
        success: function (data) {
            if (data.PendingApproval > 0) {
                $('#pen-ap').html('<span>Pending Requisitions</span><div class="req-count">' + data.PendingApproval + '</div>');
            }
            else {
                $('#pen-ap').html('<span>Pending Requisitions</span>');
            }
            if (data.Pending > 0) {
                $('#pen').html('<span>Approved Requisitions</span><div class="req-count">' + data.Pending + '</div>');
            }
            else {
                $('#pen').html('<span>Approved Requisitions</span>');
            }
            if (data.QuotationFinal > 0) {
                $('#q-f').html('<span>Comparative Statement</span><div class="req-count">' + data.QuotationFinal + '</div>');
            }
            else {
                $('#q-f').html('<span>Comparative Statement</span>');
            }
            if (data.PurchaseOrder > 0) {
                $('#po').html('<span>Work Orders</span><div class="req-count">' + data.PurchaseOrder + '</div>');
            }
            else {
                $('#po').html('<span>Work Orders</span>');
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

// Add New Inventory
$(document).on("click", ".sav-invt", function () {
    var deps = [];
    var flag = false;
    var name = $("#name").val();
    if (name == "" || name == null) {
        flag = true;
        alert("Please Enter Name");
    }
    var cat = $("#Category").val();
    var catName = "";
    if (cat == "" || cat == null) {
        flag = true;
        alert("Please Select Category");
    }
    else {
        catName = $("#Category option:selected").text();
    }    
    var comp = $("#comp").val();
    var uom = $("#uom").val();
    var minstk = $("#min-stk").val();
    var pakin = $("#pking").val();
    var len = $("#len").val();
    var l_uom = $(".l-uom").val();
    var wid = $("#wid").val();
    var w_uom = $(".w-uom").val();
    var hei = $("#hei").val();
    var h_uom = $(".h-uom").val();
    var dia = $("#dia").val();
    var d_uom = $(".d-uom").val();
    var siz = $("#siz").val();
    var s_uom = $(".s-uom").val();
    var isper = $("#is-per").is(":checked");
    var isast = $("#is-ast").is(":checked");
    var head = $(".head").val();
    var desc = $("#desc").val();

    $(".inv-dep-uytjhg").each(function (i, v) {
        var id = $(this).attr("id");
        var nam = $(this).attr("data-deptName");
        deps.push({
            deptId: id,
            deptName: nam
        });
    })


    if (uom == null || uom == "") {
        alert("Please Select Unit of Measurement")
    }
    if (uom == "Each") {
        if (minstk % 1 != 0) {
            alert("Each item cannot be in Points")
        }
    }
    if (!flag){
        if (confirm("Are you sure you want to add inventory Item.")) {
            $.ajax({
                type: "POST",
                url: '/Inventory/AddInventory/',
                data: {
                    Category_Id: cat, Category_Name: catName, Company: comp, Item_Name: name, UOM: uom, Minimum_Qty: minstk, Packing: pakin,
                    Length: len, L_UOM: l_uom, Width: wid, W_UOM: w_uom, Heigth: hei, H_UOM: h_uom, Diameter: dia, D_UOM: d_uom, Size: siz, Size_UOM: s_uom,
                    Description: desc, Is_Perishable: isper, Asset: isast, depts: deps, Head_Code: head
                },
                success: function (data) {
                    if (data.Status == true) {
                        alert(data.Msg);
                        window.location.reload();
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
// Update Inventory
$(document).on("click", ".up-invt", function (e) {
    var deps = [];
    var id = $("#Id").val();
    var name = $("#name").val();
    if (name == "" || name == null) {
        alert("Please Enter Name");
    }
    var cat = $("#Category").val();
    var comp = $("#comp").val();
    var uom = $("#uom").val();
    var minstk = $("#min-stk").val();
    var pakin = $("#pking").val();
    var len = $("#len").val();
    var l_uom = $(".l-uom").val();
    var wid = $("#wid").val();
    var w_uom = $(".w-uom").val();
    var hei = $("#hei").val();
    var h_uom = $(".h-uom").val();
    var dia = $("#dia").val();
    var d_uom = $(".d-uom").val();
    var siz = $("#siz").val();
    var s_uom = $(".s-uom").val();
    var isper = $("#is-per").is(":checked");
    var hide = $("#is-hide").is(":checked");
    var desc = $("#desc").val();
    var head = $(".head").val();

    $(".inv-dep-uytjhg").each(function (i, v) {
        var id = $(this).attr("id");
        var nam = $(this).attr("data-deptName");
        deps.push({
            deptId: id,
            deptName: nam
        });
    })

    $.ajax({
        type: "POST",
        url: '/Inventory/InventoryUpdate/',
        data: {
            Id: id, Category_Id: cat, Company: comp, Item_Name: name, UOM: uom, Minimum_Qty: minstk, Packing: pakin,
            Length: len, L_UOM: l_uom, Width: wid, W_UOM: w_uom, Heigth: hei, H_UOM: h_uom, Diameter: dia, D_UOM: d_uom, Size: siz, Size_UOM: s_uom,
            Description: desc, Is_Perishable: isper, Hide: hide, Head_Code: head, depts: deps
        },
        success: function (data) {
            if (data.Status == true) {
                alert(data.Msg);
                closeModal();
                $(".table_fulltagger").load('/Inventory/FullTagger');
                //$('#moodalbody').blur("/Inventory/FullTagger/");
                //$('#tbl-full-tagger').reload("/Inventory/FullTagger/");
                //$('#tbl-full-tagger').load("/Inventory/FullTagger/");
                
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
    //Update View
});
// to search Demand order results
$(document).on("click", ".sea-do", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var type = $("#type").val();
    if (type == "Delivered") {
        SASLoad($("#report"));
        $("#report").load("/Inventory/ArchiveList/", { From: from, To: to }, function () {
            SASUnLoad($("#report"));
        });
    }
    else if (type == "Pending") {
        SASLoad($("#report"));
        $("#report").load("/Inventory/DemandOrdersList/", { From: from, To: to }, function () {
            SASUnLoad($("#report"));
        });
    }
});
//
$(document).on("click", ".sea-v-po-grn", function () {
    var from = $("#from-d").val();
    var to = $("#to-d").val();
    var Ven_Id = $('#ven-id').val();
    SASLoad($("#report"));
    $("#report").load("/Vendor/PurchaseOrders/", { Id: Ven_Id , From: from, To: to}, function () {
            SASUnLoad($("#report"));
        });
    });
// to search Demand order results
$(document).on("click", ".sea-grn", function () {
    var from = $("#from").val();
    var to = $("#to").val();
    var user = $("#User").val();
    $("#report").load("/Inventory/GRN_Search/", { From: from, To: to, Users: user });
});
////
//$(document).on("click", ".demor-link", function () {
//    var url = $(this).data("link");
//    $('#report').load(url);
//});
// Show the Demand Orde details
$(document).on("click", ".do-det", function () {
    var id = $(this).closest('tr').attr("id");
    EmptyModel();
    $('#ModalLabel').text("Demand Order Details");
    $('.modal-body').load('/Inventory/DemandOrder_Details/', { Group_Id: id }, function () { });
});

$(document).on("click", ".do-details", function () {
    var id = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Demand Order Details");
    $('.modal-body').load('/Inventory/DemandOrder_Details/', { Group_Id: id }, function () { });
});
$(document).on("click", ".is-vi", function () {
    var grp = $(this).attr('id');
    window.open("/Inventory/IssueNote?Id=" + grp, '_blank');

});
//
$(document).on("click", "#res-dem-or", function () {
    var grp = $('#grp').val();
    var dem_grp = $('#deman-grp-id').val();
    var item = [];
    var notok = false;
    $('.do-items').each(function () {
        var id = $(this).attr("id");
        var reqqty = Number(RemoveComma($(this).find('.reqqty').text()));
        var ttlqty = 0;
        $(this).find('.ware').each(function () {
            var $this = $(this);
            var wid = $($this).find('.wid').val();
            var wnam = $($this).find('.wnam').val();
            var w_q = Number(RemoveComma($($this).find('.ware-qty').text()));
            var w_r = Number(RemoveComma( $($this).find('.ware-re-qty').val()));
            if (w_q < w_r) {
                alert("Qty cannot exceed the Warehouse Qty");
                notok = true;
                return;
            }
            ttlqty = ttlqty + w_r;
            if (ttlqty > reqqty) {
                alert("Issue Qty cannot exceed the Requested Qty");
                notok = true;
                return;
            }
            item.push({
                Item_Id: id,
                Warehouse_Id: wid,
                Warehouse_Name: wnam,
                Qty: w_r,
            });
        });
    });

    var conf = confirm("Are you Sure You want to Release Demand Order")
    if (conf) {
        $.ajax({
            type: "POST",
            url: '/Inventory/UpdateStockout/',
            data: { Group_Id: grp, DemandOrder_Id: dem_grp, Items: item },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    window.open("/Inventory/IssueNote?Id=" + grp, '_blank');
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            },
            error: function () {
                alert("Error Requested");
            }
        });
    }
});
//// Inventory Update
$(document).on("click", ".isu-det ", function () {
    var ivn = $(this).closest('tr').attr('id');
    var invs = [];
    invs.push(ivn);
    EmptyModel();
    var dat = moment().format('YYYY-MM-DD');
    $('#ModalLabel').text("Details");
    $('#modalbody').load('/Inventory/Item_In_Out_Details/', {
        From: moment('2019-1-1').format('YYYY-MM-DD'),
        To: dat,
        Prj_Id: [],
        Vendor: [],
        Inv: invs
        , In_Out: 2
    });
});
//// Inventory Update
$(document).on("click", ".inven-crud-popup", function () {
    var name = $(this).closest('tr').attr('id');
    EmptyModel();
    $('#ModalLabel').text(" Inventory Update");
    $('#modalbody').load('/Inventory/InventoryByIdSearch/', { id: name });
});
//// Inventory Update
$(document).on("click", ".cha-code", function () {
    var name = $(this).closest('tr').attr('id');
    EmptyModel();
    $('#ModalLabel').text(" Inventory Update");
    $('#modalbody').load('/Inventory/ChangeCodeOfItem/', { id: name });
});
// Request for Demand Order 
$(document).on("click", "#save-issue_rqst", function (e) {
    e.preventDefault();
    var item = $("#item-id").val();
    var prj = $("#Project option:selected").val();
    var prj_name = $("#Project option:selected").text();
    var iqty = RemoveComma($("#item-req-qty").val());
    var empl = $("#Employees option:selected").val();
    var empl_name = $("#Employees option:selected").text();
    var tqty = RemoveComma($('#item-qty').text());
    var rems = $('#item-rems').val();
    var grp_id = $('#grp-id').val();
    if (parseFloat(iqty) > parseFloat(tqty)) {
        alert('This item is not available in this quantity. Check again or submit a purchase request.');
        return;
    }
    if (prj == null || prj == '') {
        alert("Please Select a Project")
        return false;
    }
    var con = confirm("Are you sure you want to forward this issue request?");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Inventory/SaveIssueRequest/',
            data: { item: item, proj: prj, prj_Name: prj_name, emp: empl, emp_Name: empl_name, qty: iqty, rem: rems, Group_Id: grp_id },
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
// Request for Demand Order 
$(document).on("click", ".sv-inv-dep", function (e) {
    e.preventDefault();
    var item = $("#Id").val();
    var dep_id = $('#Department').val();
    var dep_nam = $('#Department :selected').text();
    var con = confirm("Are you sure you want to Add This Department?");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Inventory/SaveItemDepartment/',
            data: { Id: item, Dep_Id: dep_id, Dep_Name: dep_nam},
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    var html = `<label id="${dep_id}" data-id="${data.Id}">${dep_nam}<i class="fa fa-trash del-invdep"></i></label><br />`;
                }
                else {
                    alert(data.Msg)
                }
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
$(document).on("click", ".del-invdep", function (e) {
    e.preventDefault();
    var item = $(this).parent().attr('id');
    var con = confirm("Are you sure you want to delete This Department?");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Inventory/DeleteItemDepartment/',
            data: { Id: item },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    $("#" + item).remove();
                }
                else {
                    alert(data.Msg)
                }
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

//
$(document).on('click', '#add-new-ser-row', function () {
    var _strct = `<div class="cal item-pur-req-row">
            <div class="form-row">
                <div class="col-md-1">
                    <div class="sr" style="margin-top:5px">1.</div>
                </div>
                <div class="form-group col-md-5">
                    <input type="text" class="form-control Product" id="Product" required placeholder="Services Name" />
                </div>
                <div class="form-group col-md-3">
                    <textarea class="form-control descr" rows="1" cols="5" placeholder="Services Details"></textarea>
                </div>
                
                <div class="col-md-1">
                    <i class="ti-trash rmv-pur-req-row rmv"></i>
                </div>
            </div>
        </div>`;

    var newRow = $('#ad-demand').append(_strct);
    resetsr();
});
//
$(document).on('click', '#add-new-pr-row', function () {
    var groupId = $('.grpId').val();
    var _strct = `<div class="cal item-pur-req-row" style="margin-top:5px; box-shadow: rgba(0,0,0,0.8) 0 0 3px;border-radius: 5px;">
  <div class="form-row col-md-12 dm-stat" style="justify-content:center">
            </div>
            <div class="form-row">
                    <div class="sr">1.</div>
                <div class="form-group col-md-2">
                    <label>Code</label>
                    <input class="form-control item-code" readonly>
                </div>
                   <div class="form-group col-md-4">
                    <label>Item</label>
                    <select class="form-control product"></select>
                </div>
             <div class="form-group col-md-1">
                        <label>UOM</label>
                        <input class="form-control item-uom" name="UOM" readonly/>
                    </div>
                <div class="form-group col-md-1">
                    <label>Available Qty</label>
                    <input class="form-control av-qty" readonly>
                </div>

                <div class="form-group col-md-1">
                    <label>Req Qty</label>
                    <input class="form-control coma" placeholder="12,345" required>
                    <input type="hidden" id="" name="Quantity" class="amt Quantity" required>
                </div>
                <div class="col-md-1" style="padding-top:2.5%">
                    <i class="ti-trash rmv-pur-req-row rmv" style="font-size:20px"></i> | <i class="fa fa-plus-circle toggle_icon" style="font-size:20px"></i>
                </div>
                <input type="hidden" class="stat" />

            </div>
            <div class="form-row toggle_text" style="display:none">
                <div class="col-md-1"></div>
                <div class="form-group row col-md-8">
                    <label class="col-sm-2 col-form-label">Details</label>
                    <textarea class="form-control descr col-md-10" rows="1" cols="5" placeholder="Details"></textarea>
                </div>
            </div>
            </div>
        </div>`;

    var newRow = $('#ad-demand').append(_strct);
    resetsr();
    var latestProd = $(newRow).find(".product").last();
    $(latestProd).select2({
        minimumInputLength: 3,
        dropdownParent: $(latestProd).parent(),
        placeholder: "Search for Inventory",
        ajax: {
            url: '/Inventory/GetInventoryItem/',
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
    $(latestProd).on('select2:select', function (e) {
        var s = e.params.data;
        var department = $(".Department option:selected").val();

        $.ajax({
            type: "POST",
            url: '/Inventory/GetInventoryItemById/',
            data: { Id: s.id, Dep_Id: department },
            success: function (data) {
                if (data.InvDeps.length == 0) {
                    alert("Contact Warehouse for Item Department Tagging");
                }
                else {
                    if (!data.Status) {
                        var html = `<span class="dem-div">Demand Requisition - <b> ${data.InvDeps[0].Dep_Name} </b></span>`;
                        $(latestProd).closest('.item-pur-req-row').find('.dm-stat').html(html);
                    }
                }

                $(latestProd).closest('.item-pur-req-row').find('.Product-id').val(data.Inventory.Id);
                $(latestProd).closest('.item-pur-req-row').find('.item-uom').val(data.Inventory.UOM);
                $(latestProd).closest('.item-pur-req-row').find('.item-code').val(data.Inventory.SKU);
                $(latestProd).closest('.item-pur-req-row').find('.stat').val(data.Status);
                $(latestProd).closest('.item-pur-req-row').find('.av-qty').val(Number(data.Inventory.Total_In_Qty - data.Inventory.Total_Out_Qty));
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

});

//<td width="10%">`+ i +` <input type="checkbox" class="fa fa-trash it-qu-ch"/></td>


$(document).on('click', '.AddChargesREQ', function () {
    var i = 1;
    $('.AddSrLoop').each(function () {
        i++;
    })

    var AddChargesFields = `<tr class="AddSrLoop AddOtherChargesData">

                                <td width="10%">`+ i +` <i class="fas fa-trash Remove"/> </td>
                                 <td width="35%">                      
                                        <select class="form-control ItemName">
                                            <option value="Transportation" class="transportation">Transportation</option>
                                            <option value="Loading" class="loading">Loading</option>
                                            <option value="UnLoading" class="unloading">UnLoading</option>
                                        </select>
                                </td>
                                <td width="10%" class=""zer-pad><input type="hidden" value="1" class="form-control coma qty qty-rate"/></td>
                                <td width="10%" class=""zer-pad><input type="hidden" value="1" class="form-control coma"/></td>
                                <td width="10%" class="zer-pad"><input type="text" style="border:0" class="form-control coma rate-p-uom qty-rate"/></td>
                                <td width="10%" class="zer-pad"><input type="hidden" value="1" class="form-control ttl-rate"/></td>
                                <td width="10%" class="zer-pad"></td>                            
                             </tr>`;
    $("#AddChargerREQs tbody").append(AddChargesFields);
});

//Add Terms and condition
$(document).on('click', '.AddTerm', function () {
    
    var s = 1;
    $('.ATABCondition').each(function () {
        s++;
    })

    var AddTermandCondition = `<div>
                                    <div class="ATABCondition">  `+ s + `  <input type="text" style="width: 51.5%; margin-left: 0.5%; margin-top: 2%;" class = "get-in-inv-Terms-Loop"/> <i class="fas fa-trash  RemoveItem" style = "padding-left:1%"/> </div> 
                               </div>`;
    $(".LoopOnTerms").append(AddTermandCondition);
});
//Get Terms and Conditions

$(document).on("click", ".Save-Terms-Conditions", function () {
    
    var TermsData = [];
    let grp = $('#Group_Id').val();
    let venId = $('#Vend_Id').val();

    $('.get-in-inv-Terms-Loop').each(function () {
        let AddTermsData = $(this).val();

        TermsData.push(AddTermsData);
        //TermsData.push({
        //    ATAcondition: AddTermsData,
        //});
    });

    if (confirm("Are you sure you want to add terms and conditions")) {
        $.ajax({
            type: "POST",
            url: "/Procurement/POAddTermsAndConditions",
            contentType: "application/json",
            traditional: true,
            
            data: JSON.stringify({ Terms: TermsData, Group_Id: grp, Vendor_Id: venId }),
        }).done(function (data) {
            if (data.Status == true) {
                alert(data.Msg);
                window.location.reload();
            }
            else {
                alert(data.Msg);
            }
        });
    }


});




//
$(document).on('click', '.add-new-ser-row', function () {
    var _strct = `<div class="item-pur-req-row">
                        <div class="form-row">
                            <div class="col-md-1">
                                <div class="sr" style="margin-top:5px">1.</div>
                            </div>
                            <div class="form-group col-md-5">
                                <input type="text" class="form-control Product" id="Product" required placeholder="Services Name" />
                            </div>
                            <div class="form-group col-md-3">
                                <textarea class="form-control descr" rows="1" cols="5" placeholder="Services Details"></textarea>
                            </div>
                            <div class="col-md-1">
                                <i class="fa fa-trash rmv-pur-req-row rmv"></i>
                                &nbsp;&nbsp;&nbsp;
                                <i class="fa fa-plus-circle add-new-ser-row"></i>
                            </div>
                        </div>
                         <div class="form-row">
                            <div class="form-group col-md-2">
                                <h6>Specifications</h6>
                                <select style="float:left" class="form-control slwhd">
                                   <option>Choose ..</option>
                                    <option value="l">Length</option>
                                    <option value="w">Width</option>
                                    <option value="h">Height</option>
                                    <option value="b">Breadth</option>
                                </select>
                            </div>
                            <div class="form-group col-md-1 l-div" style="display:none">
                                <label>Length</label>
                                <input class="form-control len" placeholder="0" required>
                            </div>
                            <div class="form-group col-md-1 l-div" style="display:none">
                                <label>Length UOM</label><i class="ti-close l-cros" style="float:right;font-size:9px"></i>
                                <select class="form-control len-uom isuom" name="UOM" data-id="1">
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
</select>
                            </div>
                            <div class="form-group col-md-1 w-div" style="display:none">
                                <label>Width</label>
                                <input class="form-control wid" placeholder="0" required>
                            </div>
                            <div class="form-group col-md-1 w-div" style="display:none">
                                <label>Width UOM</label><i class="ti-close w-cros" style="float:right;font-size:9px"></i>
                                <select class="form-control wid-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
                            </div>
                            <div class="form-group col-md-1 h-div" style="display:none">
                                <label>Height</label>
                                <input class="form-control hei" placeholder="0" required>
                            </div>
                            <div class="form-group col-md-1 h-div" style="display:none">
                                <label>Height UOM</label><i class="ti-close h-cros" style="float:right;font-size:9px"></i>
                                <select class="form-control hei-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
                            </div>
                            <div class="form-group col-md-1 b-div" style="display:none">
                                <label>Breadth</label>
                                <input class="form-control bre" placeholder="0" required>
                            </div>
                            <div class="form-group col-md-1 b-div" style="display:none">
                                <label>Breadth UOM</label><i class="ti-close b-cros" style="float:right;font-size:9px"></i>
                                <select class="form-control b-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
                            </div>
                        </div>
                    </div>`;
    var newRow = $(this).closest('.cal').append(_strct);
    resetsr();
});
//
$(document).on('click', '#add-new-serm-row', function () {

    var _strct = `<hr />
<div class="cal ">
                    <div class="item-pur-req-row">
        <div class="form-row">
            <div class="form-group col-md-4">
                <label>Milestone</label>
                <select class="form-control mil-st"></select>
            </div>
        </div>
        <div class="form-row ">
            <div class="col-md-1">
                <div class="sr" style="margin-top:5px">1.</div>
            </div>
            <div class="form-group col-md-5">
                <input type="text" class="form-control Product" id="Product" required placeholder="Services Name" />
            </div>
            <div class="form-group col-md-3">
                <textarea class="form-control descr" rows="1" cols="5" placeholder="Services Details"></textarea>
            </div>

            <div class="col-md-1">
                <i class="fa fa-trash rmv-pur-req-row rmv"></i>
                &nbsp;&nbsp;&nbsp;
                <i class="fa fa-plus-circle add-new-ser-row"></i>
            </div>
        </div>
        <div class="form-row">
            <div class="form-group col-md-2">
                <h6>Specifications</h6>
                <select style="float:left" class="form-control slwhd">
                    <option>Choose ..</option>
                    <option value="l">Length</option>
                    <option value="w">Width</option>
                    <option value="h">Height</option>
                    <option value="b">Breadth</option>
                </select>
            </div>
            <div class="form-group col-md-1 l-div" style="display:none">
                <label>Length</label>
                <input class="form-control len" placeholder="0" required>
            </div>
            <div class="form-group col-md-1 l-div" style="display:none">
                <label>Length UOM</label><i class="ti-close l-cros" style="float:right;font-size:9px"></i>
                <select class="form-control len-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
            </div>
            <div class="form-group col-md-1 w-div" style="display:none">
                <label>Width</label>
                <input class="form-control wid" placeholder="0" required>
            </div>
            <div class="form-group col-md-1 w-div" style="display:none">
                <label>Width UOM</label><i class="ti-close w-cros" style="float:right;font-size:9px"></i>
                <select class="form-control wid-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
            </div>
            <div class="form-group col-md-1 h-div" style="display:none">
                <label>Height</label>
                <input class="form-control hei" placeholder="0" required>
            </div>
            <div class="form-group col-md-1 h-div" style="display:none">
                <label>Height UOM</label><i class="ti-close h-cros" style="float:right;font-size:9px"></i>
                <select class="form-control hei-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
            </div>
            <div class="form-group col-md-1 b-div" style="display:none">
                <label>Breadth</label>
                <input class="form-control bre" placeholder="0" required>
            </div>
            <div class="form-group col-md-1 b-div" style="display:none">
                <label>Breadth UOM</label><i class="ti-close b-cros" style="float:right;font-size:9px"></i>
                <select class="form-control b-uom isuom" name="UOM" data-id="1"> <option value="Sq Ft">Sq Ft</option>
        <option value="Sq Metre">Sq Metre</option>
        <option value="Kilometre">Kilometre</option>
        <option value="Meter">Metre</option>
        <option value="Centimetre">Centimetre</option>
        <option value="Millimetre">Millimetre</option>
        <option value="Mile">Mile</option>
        <option value="Yard">Yard</option>
        <option value="Foot">Foot</option>
        <option value="Inch">Inch</option>
        <option value="Sooter">Sooter</option></select>
            </div>
        </div>
    </div>
                    </div>`;
    var newRow = $('#ad-demand').append(_strct);
    InitMileByEle($(newRow).find('.mil-st').last(), milesList);
    resetsr();
});
//
$(document).on('click', '.add-new-oth-exp-row', function () {
    var _strct = `<div class="item-pur-req-row">
                        <div class="form-row">
                            <div class="col-md-1">
                                <div class="sr" style="margin-top:5px">1.</div>
                            </div>
                            <div class="form-group col-md-5">
                                <input type="text" class="form-control Product" id="Product" required  />
                            </div>
                            <div class="form-group col-md-3">
                                <textarea class="form-control descr" rows="1" cols="5"></textarea>
                            </div>
                            <div class="form-group col-md-2">
                                <input type="text" class="coma form-control Amount" id="Amount" required  />
                            </div>
                            <div class="col-md-1">
                                <i class="fa fa-trash rmv-pur-req-row rmv"></i>
                            </div>
                        </div>
                    </div>`;
    var newRow = $('#ad-demand').append(_strct);
    resetsr();
});
$(document).on('click', '.rmv-pur-req-row', function () {
    let con = confirm("Are you sure you want to drop this row?");

    if (con) {
        $(this).closest('.item-pur-req-row').remove();
    }
    resetsr();
});
//
$(document).on("click", ".do-item", function () {
    var do_item = $(this).closest('tr');
    if ($(do_item).hasClass("bgc-green-50")) {
        $(do_item).removeClass("bgc-green-50");
    }
    else {
        $(do_item).addClass("bgc-green-50");
    }
});
// Add inventory View
$(document).on("click", "#add-invent-popup", function () {
    EmptyModel();
    $('#ModalLabel').text("New Inventory Item");
    $('#modalbody').load('/Inventory/AddInventory/');
})
// manual stock in 
$(document).on("click", ".save-to-inv", function () {
    var itemsData = [];
    allok = true;
    let trans = $('.trans-id').val();

    $('.main-inv-rec').each(function () {
        let bidId = $('.Bid_Number').val();
        let grp = $('.grpId').val();
        let venId = $('#vendId').val();
        let venName = $('#vendors').val();
        let dep_id = $('#dep-id').val();
        let dep_name = $('#dep-name').val();
        let prj_id = $('#prj-id').val();
        let prj_name = $('#prj-name').val();
        let invoice_no = $('#InvoiceNo').val();

        let emp = $('#Employees option:selected').val();
        let emp_nam = $('#Employees option:selected').text();


        let itemId = $(this).find('.invs').attr('data-slval');
        let itemnm = $.trim($(this).find('.invs').text());

        let comp_Id = $(this).find('.comp_Id').val();
        let expry = $(this).find('.expiry').val();
        let poid = $(this).find('.po-id').val();
        let poNum = $.trim($('.PO_Number').val());
        let qty = $(this).find('.qty').val();
        let rate = $(this).find('.inv-rate').val();
        let rems = $(this).find('.rems').val();

        let whId = $(this).find('.warehouse option:selected').val();
        let whText = $(this).find('.warehouse option:selected').text();

        let shelfId = $(this).find('.shelf option:selected').val();
        let shelfText = $(this).find('.shelf option:selected').text();

        if (emp == null || emp == "") {
            alert("Please Select A Quality Check Person")
            allok = false;
            return false;
        }
        if (invoice_no == null || invoice_no == "") {
            invoice_no = "";
        }
        if (qty > 0) {
            if (whId == null || whId == "") {
                allok = false;
                alert("Please Select a Warehouse")
                return false;
            }
        }
        if (whId == '' || whId == null) {
            alert("Select Warehouse");
            allok = false;
            return false;
        }
        itemsData.push({
            Item_Id: itemId,
            Group_Id: grp,
            Qty: qty,
            Rate: rate,
            Vendor_Id: venId,
            Vendor_Name: venName,
            Remarks: rems,
            PO_Id: poid,
            PO_Number: poNum,
            Expire_Date: expry,
            Bid_Id: bidId,
            Warehouse_Id: whId,
            ShelfId: shelfId,
            WarehouseName: whText,
            ShelfName: shelfText,
            Item_name: itemnm,
            Dep_Id: dep_id,
            Dep_Name: dep_name,
            Prj: prj_id,
            Prj_Name: prj_name,
            QualityCheck_By: emp,
            QualityCheck_By_Name: emp_nam,
            InvoiceNo: invoice_no,
            Comp_Id: comp_Id
        });

    });
    if (allok) {
        if (confirm("Are you sure you want to Generate GRN")) {
            $.ajax({
                type: "POST",
                url: "/Inventory/InventoryStockIn/",
                contentType: "application/json",
                traditional: true,
                beforeSend: function () {

                },
                complete: function () {

                },
                data: JSON.stringify({ items: itemsData, TransactionId: trans }),
            }).done(function (data1) {
                if (data1.Status == true) {
                    //
                    var $this = $(".ref-file-Invoice");
                    var form = $(".ref-file-Invoice").closest('form');
                    var data = new FormData();
                    var files = $(".ref-file-Invoice").get(0).files;
                    if (files.length != 0) {
                        data.append("Files", files[0]);
                        $.each(form.serializeArray(), function (key, input) {

                            data.append(input.name, input.value);
                        });
                        $.ajax({
                            type: "POST",
                            processData: false,
                            contentType: false,
                            url: $(form).attr('action'),
                            data: data,
                            success: function (data) {
                                if (data.Status) {
                                    $file = $($this).val();
                                    $file = $file.replace(/.*[\/\\]/, ''); //grab only the file name not the path
                                    //$($this).closest('.item-pur-req-row').find('.filename-container').empty().append($file + '<i class="fa fa-check"></i>').show();
                                    $($this).closest('.item-pur-req-row').find('.filename-container').empty().append('Uploaded <i class="fa fa-check" style="color:green"></i>').show();
                                    $($this).closest('.item-pur-req-row').find('.atch-id').val(data.Id);
                                }
                                else {
                                    alert(data.Msg)
                                }
                            }
                        });
                    }
                    
                    //
                    alert(data1.Msg);
                    window.open('/Inventory/GoodReceiveNotePrint?Id=' + data1.GRNS);
                    window.location.reload();
                }
                else {
                    alert(data1.Msg);
                }
            });
        }
    }

});

// manual stock in 
$(document).on("click", ".sup-grn-to", function () {
    var itemsData = [];
    allok = true;
    let trans = $('.trans-id').val();
    let grp = $('.grpId').val();
    let trs = $('.trs').val();
    let venId = $('#vendId').val();

    $('.main-inv-rec').each(function () {
        let grnid = $(this).attr('id');
        let isaset = $(this).find('.is-ast').is(":checked");
        let qty = RemoveComma( $(this).find('.rcvd-qty').text() );
        let rate = RemoveComma( $(this).find('.rcvd-rate').text() );
        let itemid = $(this).find('.invs').data("slval");
        let exp = $(this).find('.expiry').val();
        itemsData.push({
            Id: grnid,
            Item_Id: itemid,
            IsAsset: isaset,
            Qty: qty,
            Rate: rate,
            Expirey: exp
        });

    });
    if (allok) {
        if (confirm("Are you sure you want to Supervise GRN")) {
            $.ajax({
                type: "POST",
                url: "/Inventory/SuperviseGRN/",
                contentType: "application/json",
                traditional: true,
                beforeSend: function () {

                },
                complete: function () {

                },
                data: JSON.stringify({ items: itemsData, StockIn_Trans: trs, TransactionId: trans, Vendor_Id: venId, Group_Id: grp }),
            }).done(function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            });
        }
    }

});
//

$(document).on('change', '.inv-for-comp', function () {
    var id = $(this).val();
    var that = this;
    $.ajax({
        type: "POST",
        url: "/Company/GetCompanyDepartments/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id }),
        success: function (data) {
            $(that).parent().parent().find('.inv-for-dep').html(' ');
            //$('#inv-for-dep').append('<option>Select Department</option>');
            $.each(data.Departments, function (key, value) {
                $(that).parent().parent().find('.inv-for-dep').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("Please check internet connection.");
            } else {
                alert(textstatus);
            }
        }
    });
});
//
$(document).on('change', '.inv-for-comp', function () {
    var id = $(this).val();
    var that = this;
    $.ajax({
        type: "POST",
        url: "/Company/GetCompanyDepartments/",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ Id: id }),
        success: function (data) {
            $(that).parent().parent().find('.inv-for-dep').html(' ');
            //$('#inv-for-dep').append('<option>Select Department</option>');
            $.each(data.Departments, function (key, value) {
                $(that).parent().parent().find('.inv-for-dep').append('<option value=' + value.Id + '>' + value.Name + '</option>');
            });
        },
        error: function (xmlhttprequest, textstatus, message) {
            if (textstatus === "timeout") {
                alert("Please check internet connection.");
            } else {
                alert(textstatus);
            }
        }
    });
});
//

$(document).on("click", ".del-term", function () {
    var id = $(this).closest('tr').attr("id");
    if (confirm("Are you want to Delete this Terms")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/DeleteTerm/',
            data: { Term_Id: id },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    var bidid = $("#bid-id").val();
                    $('#po-terms').load('/Procurement/POTerms/', { Id: bidid });
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
$(document).on("click", ".ad-term", function () {
    var bidid = $("#Group_Id").val();
    var term = $("#term").val();
    var date = $("#date").val();
    var v = $(".vendor").val();
    if (confirm("Are you sure you want to Save this PO Terms")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/SaveTerm/',
            data: { Term: term, Date: date, Bid_Id: bidid, Vendor: v },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    $('#po-terms').load('/Procurement/POTerms/', { Id: bidid });
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
$(document).on('click', '.new-bid-req', function () {
    EmptyModel();
    var itemcode = $(this).data('code');
    if (itemcode.trim() == "" || itemcode.trim() == null) {
        alert("item code is empty please contact warehouse");
        return false;
    }
    $('#ModalLabel').text("Add New Quotation");
    var req = $('#Group_Id').val();
    var inv = $(this).closest('tr').find('.item-det').val();
    var itemid = $(this).data('itemid');
    $('#modalbody').load('/Procurement/AddNewQuotation/', { Req_Id: req, Item_Id: itemid, Item: inv });
});
//
$(document).on('click', '.add-oth-ch', function () {
    EmptyModel();
    var grp = $('#Group_Id').val();
    var v_id = $('#Vend_Id').val();
    var po = $('#po_num').val();
    var v_nam = $('#Vend_Name').val();
    var v_comp = $('#Vend_Comp').val();
    $('#ModalLabel').text("Add Other Charges");
    $('#modalbody').load('/Procurement/AddOtherCharge/', { Group_Id: grp, Vend_Id: v_id, Vend_Name: v_nam, Vend_Comp: v_comp, PO: po });
});
//
$(document).on('click', '.new-bid-serv', function () {
    EmptyModel();
    $('#ModalLabel').text("Add New Quotation");
    var grp = $('#Group_Id').val();
    var inv = $(this).closest('h6').find('.item-qty').text();
    var itemid = $(this).data('itemid');
    $('#modalbody').load('/Services/AddNewQuotation/', { GroupId: grp, Req_Id: itemid, Item: inv });
});
//
$(document).on("click", ".add-quot-btn", function () {
    //var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    //if ((msg == null || msg == "") && file.length == 0) {
    //    return false;
    //}
    var form = $("#ad-quo");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    if (confirm("Are you sure you want to Add a Quotation")) {
        $.ajax({
            type: "POST",
            processData: false,
            contentType: false,
            url: $("#ad-quo").attr('action'),
            data: data,
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
                    window.location.reload();
                }
                else {
                    alert(data.Msg)
                }
            }
        });

    }
});
//
//
$(document).on("click", ".add-ser-quot-btn", function () {
    //var msg = $(".fl-text").val();
    var file = $("#files").get(0).files;
    //if ((msg == null || msg == "") && file.length == 0) {
    //    return false;
    //}
    var form = $("#ad-quo");
    var data = new FormData();
    var files = $("#files").get(0).files;
    data.append("Files", files[0]);
    $.each(form.serializeArray(), function (key, input) {
        data.append(input.name, input.value);
    });
    if (confirm("Are you sure you want to Add a Quotation")) {
        $.ajax({
            type: "POST",
            processData: false,
            contentType: false,
            url: $("#ad-quo").attr('action'),
            data: data,
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg)
                    window.location.reload();
                }
                else {
                    alert(data.Msg)
                }
            }
        });

    }
});
//
$(document).on('click', '.up-quot', function () {
    EmptyModel();
    $('#ModalLabel').text("Update Quotations");

    var id = $(this).data('bididen');
    $('#modalbody').load('/Procurement/UpdateQuote/', { Id: id });
});
//
$(document).on("click", ".po-fin", function () {
    var chkstat = $(this).is(":checked");
    if (chkstat) {
        $(this).closest('td').addClass("bgc-green-50");
        $(this).closest('td').prev('td').addClass("bgc-green-50");
    }
    else {
        $(this).closest('td').removeClass("bgc-green-50");
        $(this).closest('td').prev('td').removeClass("bgc-green-50");
    }
});
//
$(document).on("click", ".gen-po", function () {
    var grp = $(this).attr("id");
    var ven = $("#Vend_Id").val();
    if (confirm("Are you sure you want Print Generate Purchase Order")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/PrintPO/',
            data: { Group_Id: grp, Vendor_Id: ven},
            success: function (data) {
                $.each(data, function (ind, val) {
                    window.open('/Inventory/PrintPurchaseOrder?poNum=' + val);
                });
                window.location.reload();
            },
            error: function (data) {

                alert('Error Occured');
            }
        })
    }

});
//
$(document).on("click", ".gen-po-ser", function () {
    var grp = $(this).attr("id");
    var po = $(this).attr("data-po");
    if (confirm("Are you sure you want Print Generate Purchase Order")) {
        $.ajax({
            type: 'Post',
            url: '/Services/PrintPO/',
            data: { Group_Id: grp, PO_Num: po },
            success: function (data) {
                $.each(data, function (ind, val) {
                    window.open('/Services/PrintPurchaseOrder?poNum=' + val);
                });
                window.location.reload();
            },
            error: function (data) {

                alert('Error Occured');
            }
        })
    }

});
//
$(document).on("click", ".all-po", function () {
    if (confirm("Are you sure you want to Generate All Purchase Order")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/GenerateAllPO/',
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    $.each(data.PoNum, function (ind, val) {
                        window.open('/Inventory/PrintPurchaseOrder?poNum=' + val);
                    });
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

$(document).on("click", ".dem-fina", function () {
    var ids = [];
    var allok = true;
    $(".it").each(function () {
        var $this = $(this);
        if ($($this).find('input:checked').length > 0) {
            allok = true;
        }
        else {
            allok = false;
        }
    });
    if (allok) {
        var grp = $("#Group_Id").val();
        if (confirm("Are you sure you want Finalize Request and Send to Requestor")) {
            $.ajax({
                type: 'Post',
                url: '/Procurement/UpdatePurchaseReq/',
                data: { Group_Id: grp, Status: 'Demand_Approval' },
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
    }
    else {
        alert("Please Select a Quotation from the Items.")
    }
});

$(document).on("click", ".quo-fina", function () {
    var ids = [];
    var allok = true;
    $(".it").each(function () {
        var $this = $(this);
        if ($($this).find('input:checked').length > 0) {
            allok = true;
        }
        else {
            allok = false;
        }
    });
    if (allok) {
        var grp = $("#Group_Id").val();
        if (confirm("Are you sure you want Finalize Request")) {
            $.ajax({
                type: 'Post',
                url: '/Procurement/GeneratePO/',
                data: { Group_Id: grp },
                success: function (data) {
                    if (data.Status == true) {
                        alert(data.Msg);
                        //$.each(data.PoNum, function (ind, val) {
                        //    window.open('/Inventory/PrintPurchaseOrder?poNum=' + val);
                        //});
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
    }
    else {
        alert("Please Select a Quotation from the Items.")
    }
});
//

$(document).on("click", ".quo-s-bk", function () {
    var grp = $("#Group_Id").val();
    var type = $(this).data("type");
    var status = $(this).data("status");
    var reason = prompt("Reason Why are you sending it Back to Procurement", "");
    if (isBlank(reason) || isEmpty(reason)) {
        alert("Please Enter the reason")
        return false;
    }

    if (confirm("Are you sure you want to Send Requisition Back")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/SendBackToProc/',
            data: { GroupId: grp, Type: type, Status: status, Reason: reason },
            success: function (data) {
                window.location.reload();
            },
            error: function (data) {

                alert('Error Occured');
            }
        })
    }
});



$(document).on("click", ".quo-fina-ser", function () {
    var ids = [];
    //var nook = false;
    //$(".quotes-rec").each(function () {
    //    if ($(this + ' input:checked').length > 0) {
    //        alert("tha");
    //        return false;
    //    }
    //});
    //if (nook) {
    //    alert("Please mark all Quotations");
    //}
    var grp = $("#Group_Id").val();
    if (confirm("Are you sure you want Finalize Request")) {
        $.ajax({
            type: 'Post',
            url: '/Services/GeneratePO/',
            data: { Group_Id: grp },
            success: function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    //$.each(data.PoNum, function (ind, val) {
                    //    window.open('/Inventory/PrintPurchaseOrder?poNum=' + val);
                    //});
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

$(document).on("click", ".quote-det", function (e) {
    var bid = $(this).closest('tr');
    e.preventDefault();
    var id = $(bid).data('bididen');
    var elem = $(bid).parent().find('.quot-deta-' + id)
    var g = $('#Group_Id').val();
    var div_term = $(elem).find('.term');
    var div_quo = $(elem).find('.quo');
    $(div_term).empty();
    $(div_quo).empty();
    if (!$(elem).is(":visible")) {
        $.ajax({
            type: 'Post',
            url: '/Procurement/PO_Terms/',
            data: { Id: id, Group_Id: g },
            success: function (data) {
                var count = 0;
                var line = '';
                $(data.Terms).each(function (a) {
                    count++;
                    $(div_term).append(`<p>${data.Terms[a].Terms}</p><br/>`)
                });
                var ext = data.Image.Quote_Img.split('.')[1];
                if (ext == 'pdf') {
                    $(div_quo).append(`<a href="../Repository/Procurement/${g}/${id}/${data.Image.Quote_Img}" target="_blank"><img src="../assets/static/images/pdf.png" width="200" height="200" /></a>`);
                }
                else {
                    $(div_quo).append(`<a href="../Repository/Procurement/${g}/${id}/${data.Image.Quote_Img}" target="_blank"> <img src="../Repository/Procurement/${g}/${id}/${data.Image.Quote_Img}" width="200" height="200" /></a>`);
                }
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
    }
    elem.toggle('slow');


});
//
$(document).on("click", ".s-quote-det", function (e) {
    var bid = $(this).closest('tr');
    e.preventDefault();
    var id = $(bid).data('bididen');
    var elem = $(bid).parent().find('.quot-deta-' + id)
    var g = $('#Group_Id').val();
    var div_term = $(elem).find('.term');
    var div_quo = $(elem).find('.quo');
    $(div_term).empty();
    $(div_quo).empty();
    if (!$(elem).is(":visible")) {
        $.ajax({
            type: 'Post',
            url: '/Services/PO_Terms/',
            data: { Id: id, Group_Id: g },
            success: function (data) {
                var count = 0;
                var line = '';
                $(data.Terms).each(function (a) {
                    count++;
                    $(div_term).append(`<p>${data.Terms[a].Terms}</p><br/>`)
                });
                var ext = data.Image.Quote_Img.split('.')[1];
                if (ext == 'pdf') {
                    $(div_quo).append(`<a href="../Repository/Procurement/Services/${g}/${id}/${data.Image.Quote_Img}" target="_blank"><img src="../assets/static/images/pdf.png" width="200" height="200" /></a>`);
                }
                else {
                    $(div_quo).append(`<img src="../Repository/Procurement/Services/${g}/${id}/${data.Image.Quote_Img}" width="200" height="200" />`);
                }
            },
            error: function (data) {
                alert('Error Occured');
            }
        })
    }
    elem.toggle('slow');


});

//
$(document).on('click', '.exst-bid-row-rmvr', function () {
    let con = confirm('Are you sure you want to remove this Quotation?');
    if (con) {
        //yahan ajax ki hit maaro or us ki success pe wo row remove krni hai
        let bidId = $(this).attr('data-bidIden');
        let that = this;
        $.ajax({
            type: "POST",
            url: '/Inventory/DeleteBid/',
            data: { bid: bidId },
            success: function (data) {
                if (data) {
                    $(that).closest('tr').fadeOut('slow', function () {
                        $(that).closest('tr').remove();
                    });
                }
                else {
                    alert("An Error occured Try Again");
                }
            },
            error: function (data) {
                alert("An Error occured Try Again");
            }
        });
    }
});
//
$(document).on('click', '.ser-bid-row-rmvr', function () {
    let con = confirm('Are you sure you want to remove this Quotation?');
    if (con) {
        //yahan ajax ki hit maaro or us ki success pe wo row remove krni hai
        let bidId = $(this).attr('data-bidIden');
        let that = this;
        $.ajax({
            type: "POST",
            url: '/Services/DeleteBid/',
            data: { bid: bidId },
            success: function (data) {
                if (data) {
                    $(that).closest('tr').fadeOut('slow', function () {
                        $(that).closest('tr').remove();
                    });
                }
                else {
                    alert("An Error occured Try Again");
                }
            },
            error: function (data) {
                alert("An Error occured Try Again");
            }
        });
    }
});
//
//Purchase Req For Other
$(document).on("click", "#reg-PurReq-for-other", function (e) {
    InventoryArray = [];
    var department = $(".Department option:selected").val();
    var Project_Id = $(".Project").val();
    var del_date = $("#del-date").val();
    var groupId = $('.grpId').val();
    var dr = $('#dr').val();
    var uname = $('#user-name').text();
    var notok = false;

    if (Project_Id == '' || Project_Id == null) {
        alert("Please select Project");
        return false;
    }
    if (del_date == '' || del_date == null) {
        alert("Please select Expected Delivery Date");
        return false;
    }

    $('.item-pur-req-row').each(function () {
        var product_name = $(this).find(".product :selected").text();
        var ProductId = $(this).find(".product :selected").val();
        var srnum = $(this).find(".sr").text();
        var Description = $(this).find(".descr").val().trim();
        var Quantity = $(this).find(".Quantity").val();
        var atch = $(this).find(".atch-id").val();
        var stat = $(this).find(".stat").val();

        if (Quantity == "" || Quantity == null) {
            alert("Please Enter Quantity in " + product_name + " at line number " + srnum);
            notok = true;
            return false;
        }

        if (stat == "false" && (Description == "" || Description == null)) {
            alert("Please Enter Details in " + product_name + " at line number : " + srnum);
            notok = true;
            return false;
        }

        if (InventoryArray.filter(x => x.Item_Id == Product).length > 0) {
            alert(product_name + " can not be repeat again at line number : " + srnum);
            notok = true;
            return false;
        }
        InventoryArray.push({
            Item_Id: ProductId,
            Item_Name: product_name,
            Description: Description,
            Qty: Quantity,
            Group_Id: groupId,
            Attachment_Id: atch,
            ReqStatus: stat
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Purchase Requisitions For Others")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/NewPurchaseRequisitionForOther/',
            data: { demand: InventoryArray, prj: Project_Id, Delivery_Date: del_date, Demand_Req: dr, Department: department },
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
$(document).on("click", "#reg-PurReq", function (e) {
    InventoryArray = [];
    var department = $(".Department option:selected").val();
    var Project_Id = $(".Project").val();
    var del_date = $("#del-date").val();
    var groupId = $('.grpId').val();
    var dr = $('#dr').val();
    var uname = $('#user-name').text();
    var notok = false;

    if (Project_Id == '' || Project_Id == null) {
        alert("Please select Project");
        return false;
    }
    if (del_date == '' || del_date == null) {
        alert("Please select Expected Delivery Date");
            return false;
    }

    $('.item-pur-req-row').each(function () {
       
        var product_name = $(this).find(".product :selected").text();
        var ProductId = $(this).find(".product :selected").val();
        var srnum = $(this).find(".sr").text();
        var Description = $(this).find(".descr").val().trim();
        var Quantity = $(this).find(".Quantity").val();
        var atch = $(this).find(".atch-id").val();
        var stat = $(this).find(".stat").val();

        if (Quantity == "" || Quantity == null) {
            alert("Please Enter Quantity in " + product_name + " at line number " + srnum);
            notok = true;
            return false;
        }

        if (stat == "false" && (Description == "" || Description == null)) {
            alert("Please Enter Details in " + product_name + " at line number : " + srnum);
            notok = true;
            return false;
        }

        if (InventoryArray.filter(x => x.Item_Id == ProductId).length > 0) {
            alert(product_name + " can not be repeat again at line number : " + srnum);
            notok = true;
            return false;
        }
        InventoryArray.push({
            Item_Id: ProductId,
            Item_Name: product_name,
            Description: Description,
            Qty: Quantity,
            Group_Id: groupId,
            Attachment_Id: atch,
            ReqStatus: stat
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Purchase Requisitions")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/NewPurchaseRequisition/',
            data: { demand: InventoryArray, prj: Project_Id, Delivery_Date: del_date, Demand_Req: dr, Department: department },
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

$(document).on("click", "#reg-edit-PurReq", function (e) {
    InventoryArray = [];
    var Project_Id = $(".Project").val();
    var del_date = $("#del-date").val();
    var groupId = $('.grpId').val();
    var dr = $('#dr').val();
    var uname = $('#user-name').text();
    var notok = false;

    if (del_date == '' || del_date == null) {
        alert("Please select Expected Delivery Date");
        return false;
    }

    $('.item-pur-req-row').each(function () {
        var Product = $(this).find(".Product").val();
        var ProductId = $(this).find(".Product-id").val();
        var product_name = $(this).find(".Product").val();
        var srnum = $(this).find(".sr").text();
        var Description = $(this).find(".descr").val().trim();
        var Quantity = $(this).find(".Quantity").val();
        var atch = $(this).find(".atch-id").val();
        var stat = $(this).find(".stat").val();

        if (Quantity == "" || Quantity == null) {
            alert("Please Enter Quantity in " + product_name + " at line number " + srnum);
            notok = true;
            return false;
        }

        if (stat == "false" && (Description == "" || Description == null)) {
            alert("Please Enter Details in " + product_name + " at line number : " + srnum);
            notok = true;
            return false;
        }

        if (InventoryArray.filter(x => x.Item_Id == Product).length > 0) {
            alert(product_name + " can not be repeat again at line number : " + srnum);
            notok = true;
            return false;
        }
        InventoryArray.push({
            Item_Id: ProductId,
            Item_Name: Product,
            Description: Description,
            Qty: Quantity,
            Group_Id: groupId,
            Attachment_Id: atch,
            ReqStatus: stat
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Purchase Requisitions")) {
        $.ajax({
            type: "POST",
            url: '/Inventory/savePurchaseRequisition/',
            data: { demand: InventoryArray, prj: Project_Id, Delivery_Date: del_date, Demand_Req: dr },
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
$(document).on("click", "#reg-SerReq", function (e) {
    InventoryArray = [];
    var Project_Id = $("#Prj_Id").val();
    var groupId = $('.grpId').val();
    var uname = $('#user-name').text();
    var notok = false;
    var mileid;
    var milenam;
    $('.mil-st').each(function () {
        var mi = $(this).val();
        if (mi == null || mi == '') {
            alert("Please Select a Milestone");
            notok = true;
            return false;
        }
    });

    $('.item-pur-req-row').each(function () {
        var Product = $(this).find(".Product").val();
        var Measurement = $(this).find(".Measurement option:selected").text();
        var Description = $(this).find(".descr").val();
        var Quantity = $(this).find(".Quantity").val();
        var Len = $(this).find('.len').val();
        var wid = $(this).find('.wid').val();
        var hei = $(this).find('.hei').val();
        var b = $(this).find('.bre').val();
        var len_uom = $(this).find('.len-uom option:selected').val();
        var wid_uom = $(this).find('.wid-uom option:selected').val();
        var hei_uom = $(this).find('.hei-uom option:selected').val();
        var b_uom = $(this).find('.b-uom option:selected').val();


        var milid = $(this).find('.mil-st').val();
        var milnam = $(this).find('.mil-st option:selected').text();

        if (milid != null || milid != '') {
            mileid = milid
            milenam = milnam
        }
        else {

        }
        if (Product == null) {
            alert("Please Enter Proper Service Name");
            notok = true;
            return false;
        }

        InventoryArray.push({
            Item_Name: Product,
            UOM: Measurement,
            Description: Description,
            Group_Id: groupId,
            Qty: Quantity,
            Length: Len,
            Width: wid,
            Heigth: hei,
            Breadth: b,
            Len_UOM: len_uom,
            Wid_UOM: wid_uom,
            Hei_UOM: hei_uom,
            B_UOM: b_uom,
            Milestone_Id: mileid,
            Milestone_Name: milenam
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Service Requisitions")) {
        $.ajax({
            type: "POST",
            url: '/Services/NewServiceRequisition/',
            data: { demand: InventoryArray, prj: Project_Id },
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
// service for other
$(document).on("click", "#reg-SerReq-other", function (e) {
    InventoryArray = [];
    var department = $(".Department option:selected").val(); 
    var Project_Id = $("#Prj_Id").val();
    var groupId = $('.grpId').val();
    var uname = $('#user-name').text();
    var notok = false;
    var mileid;
    var milenam;
    $('.mil-st').each(function () {
        var mi = $(this).val();
        if (mi == null || mi == '') {
            alert("Please Select a Milestone");
            notok = true;
            return false;
        }
    });


    $('.item-pur-req-row').each(function () {
        var Product = $(this).find(".Product").val();
        var Measurement = $(this).find(".Measurement option:selected").text();
        var Description = $(this).find(".descr").val();
        var Quantity = $(this).find(".Quantity").val();
        var Len = $(this).find('.len').val();
        var wid = $(this).find('.wid').val();
        var hei = $(this).find('.hei').val();
        var b = $(this).find('.bre').val();
        var len_uom = $(this).find('.len-uom option:selected').val();
        var wid_uom = $(this).find('.wid-uom option:selected').val();
        var hei_uom = $(this).find('.hei-uom option:selected').val();
        var b_uom = $(this).find('.b-uom option:selected').val();


        var milid = $(this).find('.mil-st').val();
        var milnam = $(this).find('.mil-st option:selected').text();

        if (milid != null || milid != '') {
            mileid = milid
            milenam = milnam
        }
        else {

        }
        if (Product == null) {
            alert("Please Enter Proper Service Name");
            notok = true;
            return false;
        }

        InventoryArray.push({
            Item_Name: Product,
            UOM: Measurement,
            Description: Description,
            Group_Id: groupId,
            Qty: Quantity,
            Length: Len,
            Width: wid,
            Heigth: hei,
            Breadth: b,
            Len_UOM: len_uom,
            Wid_UOM: wid_uom,
            Hei_UOM: hei_uom,
            B_UOM: b_uom,
            Milestone_Id: mileid,
            Milestone_Name: milenam
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Service Requisitions For Other")) {
        $.ajax({
            type: "POST",
            url: '/Services/NewServiceRequisitionForOther/',
            data: { demand: InventoryArray, prj: Project_Id, Department: department },
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
$(document).on("click", "#reg-othexp", function (e) {
    InventoryArray = [];
    var Project_Id = $("#Prj_Id").val();
    var groupId = $('.grpId').val();
    var uname = $('#user-name').text();
    var Currency = $('.ccy').val();
    var notok = false;

    $('.item-pur-req-row').each(function () {
        var Product = $(this).find(".Product").val();
        var Description = $(this).find(".descr").val();
        var Amount = RemoveComma($(this).find(".Amount").val());
        InventoryArray.push({
            Item_Name: Product,
            Description: Description,
            Group_Id: groupId,
            Amount: Amount,
            Currency: Currency
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Generate the Requisitions")) {
        $.ajax({
            type: "POST",
            url: '/Services/OtherExpenseSub/',
            data: { demand: InventoryArray, prj: Project_Id },
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
$(document).on("click", ".po-fin", function () {
    var $this = $(this);
    var ttl_val = 0;
    var id = $($this).val();
    var selrate = $($this).data('rates');
    var stat = $($this).is(":checked");
    var reason = "";
    var sendreq = false;
    $($this).closest(".it").find('.item-ttl').each(function () {
        var rate = Number((RemoveComma($(this).text())));
        if (selrate > rate) {
            if (stat == false) {
                sendreq = true;
                return false;
                /////////
            }
            else {
                reason = prompt("Reason Why you are sellecting Higher Amount Item", "");
                if (isBlank(reason) || isEmpty(reason)) {
                    alert("Please Enter the reason")
                    $($this).prop('checked', false);
                    $($this).closest('.exst-item-bid-info').removeClass("bgc-green-50");
                    sendreq = false;
                    return false;
                }
                else {
                    sendreq = true;
                    return false;
                    ////////////////
                }
            }
        }
        else {
            sendreq = true;
            /////////////////////
        }
    });

    if (sendreq) {
        $.post("/Procurement/MarkQuotation/", { Id: id, Status: stat, Remarks: reason }, function (data) {
            if (data.Status) {
                $(".po-fin").each(function () {
                    if ($(this).is(":checked")) {
                        var id = RemoveComma($(this).closest('td').find('.item-ttl').text());
                        ttl_val += Number(id);
                        console.log(id);
                    }
                });
                $(".total-val").text("Grand Total - Rs. " + Number(ttl_val).toLocaleString());
            }
            else {
                $($this).prop("checked", false);
                $($this).closest('tr').removeClass("bgc-green-50");
                alert(data.Msg);
            }
           
        });
    }

});
// Delete Project
$(document).on('click', '.del-item', function () {
    var id = $(this).closest(".it").attr("id");
    var mod = $(this).data("mod");
    if (confirm("Are you sure you want to delete this item")) {
        $.post("/Inventory/DeleteItem/", { Id: id, Module: mod }, function (data) {
            alert("Item deleted");
            $('#' + id).remove();
        });
    }
});
//
$(document).on('click', '.gen-dev-no', function () {
    let po = $(this).data('grpid');
    window.open("/Procurement/DeliverInAutoView?po=" + po);
});
//
$(document).on('click', '.gen-com-ser', function () {
    let grp = $(this).data('grpid');
    let ponum = $(this).data('po');
    window.open("/Services/WorkCompletion_SC?Group_Id=" + grp + "&poNum=" + ponum);
});
//
$(document).on('click', '.ser-po', function () {
    let po = $(this).closest('tr').attr('id');
    window.open("/Inventory/StockInAutoView?po=" + po);
});
//
$(document).on('click', '.sup-grn', function () {
    let grn = $(this).closest('tr').attr('id');
    window.open("/Inventory/GRNDetails?GRN=" + grn);
});
//
$(document).on('click', '.sup-iss', function () {
    let iss = $(this).closest('tr').attr('id');
    window.open("/Inventory/IssuanceDetails?isn=" + iss);
});
//
$(document).on('click', '.dir-is', function () {
    let po = $(this).closest('tr').attr('id');
    let grp = $(this).closest('tr').data('grp');
    window.open("/Inventory/DirectIssuanceDetails?PO=" + po + "&GroupId=" + grp);
});
//  
$(document).on("change", ".mark-all", function () {
    if (this.checked) {
        $('.do-items').addClass('bgc-green-50');
    }
    else {
        $('.do-items').removeClass('bgc-green-50');
    }
});
//
$(document).on("click", ".rem-prj", function () {
    if (confirm("Are you sure you want to remove this prj issuance ?")) {
        $(this).closest('.prj-row').remove();
    }
});
//
//
$(document).on("click", ".dir-is-item", function () {
    var grp = $('.grpId').val();
    var transid = $('.trans-id').val();
    var ponum = $('.PO_Number').val();
    var item = [];
    var notok = false;
    var issuto = $('#Employees').val();
    var issuto_nam = $('#Employees :selected').text();

    if (issuto == "") {
        alert("Please Select a Employee")
        notok = true;
        return false;
    }


    $('.prj-row').each(function () {
        var prj = $(this).find(".prj-se").val();
        var prj_text = $(this).find(".prj-se :selected").text();

        if (prj == "Select Project") {
            alert("Please Select a Project")
            notok = true;
            return false;
        }

        if (notok == true) {
            return false;
        }

        if (item.filter(x => x.Prj_Id == prj).length > 0) {
            alert(prj_text + " can not be repeat again ");
            notok = true;
            return false;
        }

        //var reqqty = Number($(this).find('.reqqty').text());
        var ttlqty = 0;
        $(this).find('.prj-items').each(function () {
            
            var $this = $(this);
            var id = $($this).find('.item-id').val();
            var issed_qty =  Number($($this).find('.issd-qty').val());
            var rem_qty =  Number($($this).find('.rem-qty').val());
            var w_r = Number( RemoveComma( $($this).find('.is-qty').val()));
            //var w_q = Number($($this).find('.ware-qty').text());
            //var w_r = Number($($this).find('.ware-re-qty').val());
            if (w_r > rem_qty) {
                alert("Qty cannot exceed the Remaining Qty");
                notok = true;
                return false;
            }
            //ttlqty = ttlqty + w_r;
            //if (ttlqty > reqqty) {
            //    alert("Issue Qty cannot exceed the Requested Qty");
            //    notok = true;
            //    return;
            //}
            item.push({
                Item_Id: id,
                Issue_To: issuto,
                IssueTo_Name: issuto_nam,
                Qty: w_r,
                Prj_Id: prj,
                Prj_Name: prj_text,
            });
        });
    });
    if (!notok) {
        if (confirm("Are you Sure You want to Issue Items")) {
            $.ajax({
                type: "POST",
                url: '/Inventory/DirectIssue/',
                data: { Group_Id: grp, Transaction_Id: transid, PO_Number: ponum, Items: item },
                success: function (data) {
                    if (data.Status) {
                        alert(data.Msg);
                        $.each(data.DI, function (i) {
                            window.open("/Inventory/DirectIssueNote?Id=" + data.DI[i] + "&TransactionId=" + transid, '_blank');
                        });
                        window.location.reload();
                    }
                    else {
                        alert(data.Msg);
                    }
                },
                error: function () {
                    alert("Error Requested");
                }
            });
        }

    }
});
//
$(document).on("click", ".up-inv-code", function (e) {
    e.preventDefault();
    var item_id= $('.product').val();
    var pr  = $('.pr-item').val();
    var con = confirm("Are you sure you want to Change this Item?");
    if (con) {
        $.ajax({
            type: "POST",
            url: '/Inventory/ChangeItemCode/',
            data: { PreItem: pr, Item: item_id },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                }
                else {
                    alert(data.Msg)
                }
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
//
$(document).on("click", ".in-qty", function () {
    var id = $(this).closest('tr').attr("id");
    var qty = RemoveComma($("#" + id + " .qty").val());
    var pre_qty = RemoveComma($("#" + id + " .pqty").val());
    var dp = $("#" + id + " .d-id").val();
    var dp_nam = $("#" + id + " .d-nam").val();
    var w = $("#" + id + " .w-h-id").val();
    var w_nam = $("#" + id + " .w-h-nam").val();
    if (confirm("Are you sure want to update the Qty")) {

        $.ajax({
            type: "POST",
            url: '/Inventory/UpdateInv/',
            data: { Id: id, Qty: qty, Pre_Qty: pre_qty, DepId: dp, Dep_Nam: dp_nam, Ware_Id: w, Ware_Name: w_nam },
            success: function (data) {
                $("#" + id + " .pqty").val(qty);

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
$(document).on("click", ".add-all-quot", function () {
    var itemsData = [];
    allok = true;
    //let trans = $('.trans-id').val();
    let grp = $('#Group_Id').val();
    let venId = $('.vendor').val();
    let quot_ref = $('.quot-ref').val();
    let curr = $('.ccy').val();
    let desc = $('.Description').val();
    let payment = $('.PaymentTime').val();

    $('.it-qu-ch:checked').each(function () {
        let reqid = $(this).closest('tr').data('id');
        let itemid = $(this).closest('tr').data('item');
        let qty = RemoveComma($(this).closest('tr').find('.qty').val());
        let rate = RemoveComma($(this).closest('tr').find('.rate-p-uom').val());
        let gst = RemoveComma($(this).closest('tr').find('.gst').val());
        let del_date = $(this).closest('tr').find('.deliv-date').val();
        itemsData.push({
            PurchaseReq_Id: reqid,
            Item_Id: itemid,
            Qty: qty,
            PurchaseRate: rate,
            Tax: gst,
            Deliver_Date: del_date
        });
    });
    if (allok) {
        if (confirm("Are you sure you want to Upload Quotations")) {
            $.ajax({
                type: "POST",
                url: "/Procurement/AddQuotation_All/",
                contentType: "application/json",
                traditional: true,
                beforeSend: function () {

                },
                complete: function () {

                },
                data: JSON.stringify({ items: itemsData, Vendor_Id: venId, Group_Id: grp, Quote_Ref: quot_ref, Currency: curr, Description: desc, PaymentTime: payment}),
                success: function (data) {
                    if (data.Status == true) {
                        alert(data.Msg);

                        var dat = new FormData();
                        var files = $('#quote-file').get(0).files;
                        if (files.length == 0) {
                            return;
                        }
                        dat.append("Files", files[0]);
                        dat.append("Group_Id", grp);
                        dat.append("QuoteId", data.Id);

                        $.ajax({
                            type: "POST",
                            processData: false,
                            contentType: false,
                            url: '/Procurement/UploadQuotation/',
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

                        //window.location.reload();
                    }
                }
            });
        }
    }

});
//

// manual stock in 
$(document).on("click", ".save-to-inv-man", function () {
    var itemsData = [];
    allok = true;
    let trans = $('.trans-id').val();

    $('.inv-item-wrap').each(function () {
        let bidId = $('.Bid_Number').val();
        let grp = $('#grpId').val();
        let venId = $('#vendId').val();
        let venName = $('#vendors').val();
        let dep_id = $('#dep-id').val();
        let dep_name = $('#dep-name').val();
        let prj_id = $('#prj-id').val();
        let prj_name = $('#prj-name').val();

        let emp = $('#Employees option:selected').val();
        let emp_nam = $('#Employees option:selected').text();


        let itemId = $(this).find('.invs-id').val();
        let itemnm = $.trim($(this).find('.invs').val());

        let expry = $(this).find('.expiry').val();
        let poid = $(this).find('.po-id').val();
        let poNum = $.trim($('.PO_Number').val());
        let qty = $(this).find('.qty').val();
        let rate = $(this).find('.inv-rate').val();
        let rems = $(this).find('.rems').val();

        let whId = $(this).find('.warehouse option:selected').val();
        let whText = $(this).find('.warehouse option:selected').text();

        let shelfId = $(this).find('.shelf option:selected').val();
        let shelfText = $(this).find('.shelf option:selected').text();

        if (emp == null || emp == "") {
            alert("Please Select A Quality Check Person")
            allok = false;
            return false;
        }
        if (qty > 0) {
            if (whId == null || whId == "") {
                allok = false;
                alert("Please Select a Warehouse")
                return false;
            }
        }
        if (whId == '' || whId == null) {
            alert("Select Warehouse");
            allok = false;
            return false;
        }
        itemsData.push({
            Item_Id: itemId,
            Group_Id: grp,
            Qty: qty,
            Rate: rate,
            Vendor_Id: venId,
            Vendor_Name: venName,
            Remarks: rems,
            PO_Id: poid,
            PO_Number: poNum,
            Expire_Date: expry,
            Bid_Id: bidId,
            Warehouse_Id: whId,
            ShelfId: shelfId,
            WarehouseName: whText,
            ShelfName: shelfText,
            Item_name: itemnm,
            Dep_Id: dep_id,
            Dep_Name: dep_name,
            Prj: prj_id,
            Prj_Name: prj_name,
            QualityCheck_By: emp,
            QualityCheck_By_Name: emp_nam,
        });

    });

    if (allok) {
        if (confirm("Are you sure you want to Save Stock")) {
            $.ajax({
                type: "POST",
                url: "/Inventory/Manual_InventoryStockIn/",
                contentType: "application/json",
                traditional: true,
                beforeSend: function () {

                },
                complete: function () {

                },
                data: JSON.stringify({ items: itemsData, TransactionId: trans }),
            }).done(function (data) {
                if (data.Status == true) {
                    alert(data.Msg);
                    window.open('/Inventory/ManualStockInPrint?Id=' + data.GRNS);
                    window.location.reload();
                }
                else {
                    alert(data.Msg);
                }
            });
        }
    }

});

//
$(document).on('click', '.add-new-ser-con-row', function () {

    var _strct = `<div class="form-control item-services-row">
                        <div class="form-row">
                            <div class="col-md-1">
                                <div class="sr">1</div>
                            </div>
                            <div class="form-group col-md-5">
                                <label>Name</label>
                                <input type="text" class="form-control Product" id="Product" required placeholder="Services Name" />
                            </div>
                            <div class="form-group col-md-3">
                                <label>Details</label>
                                <textarea class="form-control descr" rows="1" cols="5" placeholder="Services Details"></textarea>
                            </div>
                            <div class="col-md-2" style="padding-top:2%">
                                <i class="fa fa-trash rmv-pur-req-ser-row "></i>
                                &nbsp;&nbsp;&nbsp;
                                <i class="fa fa-plus-circle add-new-ser-con-row"></i>
                            </div>
                        </div>
                        <div class="add-row-for-specification">
                            <h6>Specifications</h6>
                            <div class="form-row rem-spec-row">
                                <div class="form-group col-md-1">
                                    <label>UOM</label>
                                    <select class="form-control len-uom wid-uom hei-uom b-uom isuom" name="UOM" data-id="1">
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
                                    </select>
                                </div>
                                <div class="form-group col-md-1">
                                    <label>Length</label>
                                    <input type="text" class="form-control len" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Width</label>
                                    <input type="text" class="form-control wid" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Height</label>
                                    <input type="text" class="form-control hei" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Breadth</label>
                                    <input type="text" class="form-control bre" placeholder="0" required>
                                </div>
                                <div class="form-group col-md-1">
                                    <label>No of Services</label>
                                    <input type="number" placeholder="NO." autocomplete="off" maxlength="5" class="No-of-service form-control" />
                                </div>
                                <div class="form-group col-md-1">
                                    <label>Quantity</label>
                                    <input type="number" placeholder="Quantity" autocomplete="off" maxlength="5" class="Qty-of-service form-control" readonly />
                                </div>
                                <div class="form-group col-md-1" style="padding-top:2%;">
                                    <label> </label>
                                    <i class="fas fa-plus-square fa-lg pointer add-service-pr-row"></i>
                                </div>
                            </div>
                        </div>
                    </div>`;
    var newRow = $(this).closest('.item-pur-req-row').append(_strct);
    resetsr();
});
$(document).on('click', '.rmv-pur-req-ser-row', function () {
    let con = confirm("Are you sure you want to drop this row?");

    if (con) {
        $(this).closest('.item-services-row').remove();
    }
    resetsr();
});
$(document).on('click', '#add-new-serv-dev-row', function () {
    var _strct = `<div class="cal ">
                <div class="item-pur-req-row">
                    <div class="form-row">
                        <div class="form-group col-md-4">
                            <label>Milestone</label>
                            <select class="form-control mil-st"></select>
                        </div>
                    </div>
                    <div class="form-control item-services-row">
                        <div class="form-row">
                            <div class="col-md-1">
                                <div class="sr">1</div>
                            </div>
                            <div class="form-group col-md-5">
                                <label>Name</label>
                                <input type="text" class="form-control Product" id="Product" required placeholder="Services Name" />
                            </div>
                            <div class="form-group col-md-3">
                                <label>Details</label>
                                <textarea class="form-control descr" rows="1" cols="5" placeholder="Services Details"></textarea>
                            </div>
                            <div class="col-md-2" style="padding-top:2%">
                                <i class="fa fa-trash rmv-pur-req-row "></i>
                                &nbsp;&nbsp;&nbsp;
                                <i class="fa fa-plus-circle add-new-ser-con-row"></i>
                            </div>
                        </div>
                        <div class="add-row-for-specification">
                            <h6>Specifications</h6>
                            <div class="form-row rem-spec-row">
                                <div class="form-group col-md-1">
                                    <label>UOM</label>
                                    <select class="form-control len-uom wid-uom hei-uom b-uom isuom" name="UOM" data-id="1">
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
                                    </select>
                                </div>
                                <div class="form-group col-md-1">
                                    <label>Length</label>
                                    <input type="text" class="form-control len" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Width</label>
                                    <input type="text" class="form-control wid" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Height</label>
                                    <input type="text" class="form-control hei" placeholder="0" required>
                                </div>

                                <div class="form-group col-md-1">
                                    <label>Breadth</label>
                                    <input type="text" class="form-control bre" placeholder="0" required>
                                </div>
                                <div class="form-group col-md-1">
                                    <label>No of Services</label>
                                    <input type="number" placeholder="NO." autocomplete="off" maxlength="5" class="No-of-service form-control" />
                                </div>
                                <div class="form-group col-md-1">
                                    <label>Quantity</label>
                                    <input type="number" placeholder="Quantity" autocomplete="off" maxlength="5" class="Qty-of-service form-control" readonly />
                                </div>
                                <div class="form-group col-md-2" style="padding-top: 2%;">
                                    <label> </label>
                                    <i class="fas fa-plus-square fa-lg pointer add-service-pr-row"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;
    var newRow = $('#ad-demand').append(_strct);
    InitMileByEle($(newRow).find('.mil-st').last(), milesList);
    resetsr();
});
$(document).on('click', '.add-service-pr-row', function () {
    var Mydiv = `                            <div class="form-row rem-spec-row">
                                <div class="form-group col-md-1">
                                    <select class="form-control len-uom wid-uom hei-uom b-uom isuom" name="UOM" data-id="1">
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
                                    </select>
                                </div>
                                <div class="form-group col-md-1">
                                    <input type="text" class="form-control len" placeholder="0" required>
                                </div>
                               
                                <div class="form-group col-md-1">
                                    <input type="text" class="form-control wid" placeholder="0" required>
                                </div>
                               
                                <div class="form-group col-md-1">
                                    <input type="text" class="form-control hei" placeholder="0" required>
                                </div>
                               
                                <div class="form-group col-md-1">
                                    <input type="text" class="form-control bre" placeholder="0" required>
                                </div>
                               


                                <div class="form-group col-md-1">
                                    <input type="number" placeholder="NO." autocomplete="off" maxlength="5" class="No-of-service form-control" />
                                </div>
                                <div class="form-group col-md-1">
                                    <input type="number" placeholder="Quantity" autocomplete="off" maxlength="5" class="Qty-of-service form-control" readonly />
                                </div>
                                <div class="form-group col-md-1" style="padding-top: 0.5%;">
                                    <label> </label>
                                    <i class="fas fa-plus-square fa-lg pointer add-service-pr-row"></i>
                                    <i class="fas fa-minus-square fa-lg pointer ml-2 remove-service-pr-row"></i>
                                </div>
                            </div>`;
    $(this).closest('.add-row-for-specification').append(Mydiv);
});