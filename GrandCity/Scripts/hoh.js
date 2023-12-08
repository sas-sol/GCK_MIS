// Shop Creation
$(document).on("submit", "#h-c", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#h-c").attr('action'),
        data: $("#h-c").serialize(),
        success: function (data) {
            if (data) {
                alert("Shop Has been Created");
                window.location.reload();
            }
            else {
                alert("This Shop is already Exists");
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
// Shop Edit
$(document).on("submit", "#h-c-e", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: $("#h-c-e").attr('action'),
        data: $("#h-c-e").serialize(),
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
});
//
$(document).on("click", ".del-shp", function (e) {
    var id = $(this).closest('tr').attr('id');
    if (confirm("Are you sure you want Remove this shop")) {

    $.ajax({
        type: "POST",
        url: '/HOH/DeleteShop/',
        data: { Id: id },
        success: function (data) {
            if (data.Status) {
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
});
//
$(document).on('click', '#add-new-pr-row', function () {
    var groupId = $('.grpId').val();
    var _strct = `
<div class="cal item-pur-req-row">
    <div class="form-row col-md-12 dm-stat" style="justify-content:center">
    </div>
    <div class="form-row">
<span class="sr" style="margin: 1% 10px 0 0;">1</span>
        <div class="form-group col-md-1">
            <input class="form-control item-code" readonly>
        </div>
      <div class="form-group col-md-4">
        <select class="form-control product"></select>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control item-uom" name="UOM" readonly />
        </div>
        <div class="form-group col-md-1">
            <input class="form-control av-qty" readonly>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control coma qty ratcal" name="Quantity" placeholder="12,345" required>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control coma rate ratcal" name="Rate" placeholder="12,345" required>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control coma amt" name="Amount" placeholder="12,345" readonly>
        </div>
 <div class="form-group col-md-1">
                    <input class="form-control date" data-provide="datepicker"  style="padding:6px 5px" placeholder="mm/dd/yyyy" readonly>
                </div>
            <i class="ti-trash rmv-pur-req-row rmv" style="margin: 1% 0 0 2%;"></i>
    </div>
</div>`;

    var newRow = $('#ad-demand').append(_strct);
    resetsr();
    var latestProd = $(newRow).find(".product").last();
    var sid = $('.shop_id').val();
    $(latestProd).select2({
        minimumInputLength: 3,
        dropdownParent: $(latestProd).parent(),
        placeholder: "Search for Inventory",
        ajax: {
            url: '/HOH/GetInventoryItem/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term, Shop_Id: sid// search term
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
        var sid = $('.shop_id').val();
        $.ajax({
            type: "POST",
            url: '/HOH/GetInventoryItemById/',
            data: { Id: s.id, Shop_Id: sid },
            success: function (data) {
                $(latestProd).closest('.item-pur-req-row').find('.item-code').val('');
                $(latestProd).closest('.item-pur-req-row').find('.item-uom').val('');
                $(latestProd).closest('.item-pur-req-row').find('.rate').val('');
                $(latestProd).closest('.item-pur-req-row').find('.av-qty').val('');

                $(latestProd).closest('.item-pur-req-row').find('.item-code').val(data.Inventory.SKU);
                $(latestProd).closest('.item-pur-req-row').find('.item-uom').val(data.Inventory.UOM);
                $(latestProd).closest('.item-pur-req-row').find('.rate').val(data.Inventory.Sale_Rate);
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
//
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
            data: { demand: InventoryArray, Shop_Id: shopid},
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
$(document).on('click', '.rmv-pur-req-row', function () {
    let con = confirm("Are you sure you want to drop this row?");

    if (con) {
        $(this).closest('.item-pur-req-row').remove();
    }
    resetsr();
    AllCalc();
});
//
function resetsr() {
    var sr = 1;
    $('.sr').each(function () {
        $(this).text(sr);
        sr++;
    });
}
//
$(document).on('keyup', '.ratcal', function () {
    var rate = Number(RemoveComma( $(this).closest(".item-pur-req-row").find('.rate').val()));
    var qty =   Number(RemoveComma($(this).closest(".item-pur-req-row").find('.qty').val()));
    var amt = $(this).closest(".item-pur-req-row").find('.amt').val(Number(rate * qty).toLocaleString());
    AllCalc();
});
//
function AllCalc() {
    var ttl = 0
    $('.amt').each(function () {
        ttl += Number(RemoveComma($(this).val()))
    });
    $('.total-amt').text(ttl.toLocaleString());
}
//
//
$(document).on("click", "#reg-sale", function (e) {
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
            Sale_Date: date
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Submit the Sales")) {
        $.ajax({
            type: "POST",
            url: '/HOH/RecordSales/',
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
// Add inventory View
$(document).on("click", "#add-shop", function () {
    EmptyModel();
    $('#ModalLabel').text("New Shop");
    $('#modalbody').load('/HOH/CreateShops/');
})
//
$(document).on("click", ".sav-hoh-invt", function () {
    var name = $("#name").val();
    var Shop_Id = $("#ShopId").val();
    var siz = $("#siz").val();
    if (name == "" || name == null) {
        alert("Please Enter Product Name");
        return false;
    }
    var cat = $("#Category").val();
    if (cat == "" || cat == null) {
        alert("Please Select Category ");
        return false;
    }
    var comp = $("#comp").val();
    var uom = $("#uom").val();
    var minstk = $("#min-stk").val();
    var pakin = $("#pking").val();
    var desc = $("#desc").val();
    var specific = $("#specific").val();
    var SRate = $("#SRate").val();
    if (SRate == "" || SRate == null) {
        alert("Please Enter Sale Rate");
        return false;
    }

    if (confirm("Are you sure you want to add inventory Item.")) {
        $.ajax({
            type: "POST",
            url: '/HOH/AddInventory/',
            data: {
                Category_Id: cat, Company: comp, Item_Name: name, UOM: uom, Minimum_Qty: minstk, Packing: pakin,
                Description: desc, specific: specific, Size: siz, Shop_Id: Shop_Id, SRate: SRate
            },
            success: function (data) {
                debugger

                if (data.Status == true) {
                    alert(data.Msg);
                    window.location.reload();
                }

            }
            , error: function (xmlhttprequest, textstatus, message) {
                debugger
                if (textstatus === "timeout") {
                    alert("got timeout");
                } else {
                    alert(textstatus);
                }
            }
        });
    }

});


$(document).on("click", ".edit-expns-rprt-hoh", function () {
    var id = $(this).attr("id");
    debugger
    EmptyModel();
    $('#ModalLabel').text("Update Expense Report");
    $('#modalbody').load('/HOH/UpdateExpensereport/', { Id: id });
});


$(document).on("click", ".up-Othr-expns-hoh", function () {
    var Id = $(this).attr("id");
    var Desc = $("#Desc").val();
    var Amount = $("#amnt").val();
    var Date = $("#Date").val();
    if (confirm("Are you sure you want to Update this Expense.")) {
        $.ajax({
            type: "POST",
            url: '/HOH/UpdateOtherExpense/',
            data: { Id: Id, Desc: Desc, Amount: Amount, Date: Date },
            success: function (data) {
                alert("Expense Updated");
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
$(document).on('click', '#add-new-pr-row-f', function () {
    var groupId = $('.grpId').val();
    var _strct = `
<div class="cal item-pur-req-row">
    <div class="form-row col-md-12 dm-stat" style="justify-content:center">
    </div>
    <div class="form-row">
<span class="sr" style="margin: 1% 10px 0 0;">1</span>
        <div class="form-group col-md-1">
            <input class="form-control item-code" readonly>
        </div>
      <div class="form-group col-md-4">
        <select class="form-control product"></select>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control item-uom" name="UOM" readonly />
        </div>
        
        <div class="form-group col-md-1">
            <input class="form-control coma qty ratcal" name="Quantity" placeholder="12,345" required>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control coma rate ratcal" name="Rate" placeholder="12,345" required>
        </div>
        <div class="form-group col-md-1">
            <input class="form-control coma amt" name="Amount" placeholder="12,345" readonly>
        </div>
 <div class="form-group col-md-1">
                    <input class="form-control date" data-provide="datepicker"  style="padding:6px 5px" placeholder="mm/dd/yyyy" readonly>
                </div>
            <i class="ti-trash rmv-pur-req-row rmv" style="margin: 1% 0 0 2%;"></i>
    </div>
</div>`;

    var newRow = $('#ad-demand').append(_strct);
    resetsr();
    var latestProd = $(newRow).find(".product").last();
    var sid = $('.shop_id').val();

    $(latestProd).select2({
        minimumInputLength: 3,
        dropdownParent: $(latestProd).parent(),
        placeholder: "Search for Inventory",
        ajax: {
            url: '/HOH/GetInventoryItem/',
            dataType: 'json',
            params: {
                contentType: 'application/json; charset=utf-8'
            },
            quietMillis: 100,
            data: function (params) {
                return {
                    q: params.term, Shop_Id: sid// search term
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
         var sid = $('.shop_id').val();
        $.ajax({
            type: "POST",
            url: '/HOH/GetInventoryItemById/',
            data: { Id: s.id, Shop_Id: sid },
            success: function (data) {
                $(latestProd).closest('.item-pur-req-row').find('.item-code').val('');
                $(latestProd).closest('.item-pur-req-row').find('.item-uom').val('');
                $(latestProd).closest('.item-pur-req-row').find('.rate').val('');
                $(latestProd).closest('.item-pur-req-row').find('.av-qty').val('');

                $(latestProd).closest('.item-pur-req-row').find('.item-code').val(data.Inventory.SKU);
                $(latestProd).closest('.item-pur-req-row').find('.rate').val(data.Inventory.Sale_Rate);
                $(latestProd).closest('.item-pur-req-row').find('.item-uom').val(data.Inventory.UOM);
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
//
//
$(document).on("click", "#reg-sale-f", function (e) {
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
            Sale_Date: date
        });
    });
    if (notok) {
        return false;
    }
    if (confirm("Are you sure you want to Submit the Sales")) {
        $.ajax({
            type: "POST",
            url: '/HOH/RecordSalesFood/',
            data: { demand: InventoryArray, Shop_Id: shopid },
            success: function (data) {
                if (data.Status == false) {
                    alert(data.Msg);
                }
                else {
                    $('#reg-DemndOrdr').prop('disabled', true);
                    alert(data.Msg);
                    window.open("/HOH/SaleReceipt?Token=" + data.Token, '_blank');
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
$(document).on('blur', '.saveSalerate', function () {
    var itemrate = RemoveComma($(this).val());
    var shop_id = $('#Shop_Id').val();
    var id = $(this).closest('tr').attr('id');
    $.post('/HOH/Edit_Salerate/', { Value: itemrate, Id: id, shop_Id: shop_id }, function (data) {

    });
});
//
$(document).on("click", ".del-inv-hoh", function (e) {
    var id = $(this).closest('tr').attr('id');
    if (confirm("Are you sure you want Remove this Inventory")) {
        $.ajax({
            type: "POST",
            url: '/HOH/DeleteInvHoh/',
            data: { Id: id },
            success: function (data) {
                if (data.Status) {
                    alert(data.Msg);
                    $('#' + id).remove();
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
});
$(document).on("click", ".edit-Inventory-hoh", function () {
    var id = $(this).closest('tr').attr("id");
    var ShopID = $(this).attr("id");
    EmptyModel();
    $('#ModalLabel').text("Update Invenroty");
    $('#modalbody').load('/HOH/UpdateInventoryreport/', { Id: id, ShopID: ShopID });
});

$(document).on("click", ".up-inv-hoh", function () {
    var Id = $(this).attr("id");
    var Name = $("#Name").val();
    var ShopID = $("#ShopID").val();
    var UOM = $("#UOM").val();
    //var Code = $("#Code").val();
    if (confirm("Are you sure you want to Update this Expense.")) {
        $.ajax({
            type: "POST",
            url: '/HOH/UpdateInventory/',
            data: { Id: Id, Name: Name, UOM: UOM, ShopID: ShopID },
            success: function (data) {
                debugger
                $(".Inv-list-hoh").load("/HOH/InventoryList", { ShopID: ShopID });
                //window.location.reload();
                alert("Expense Updated");
                closeModal();

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
