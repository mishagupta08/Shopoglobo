$(document).ready(function () {
    $("#offline").hide();
    $("#dtcard").hide();
    $(".preloader").hide();

    $('a[name=deleteFunction]').unbind();
    $('a[name=deleteFunction]').click(function (e) {
        DeleteDetail(this);
    });

    $('a[name=updateQuantity]').unbind();
    $('a[name=updateQuantity]').click(function (e) {
        UpdateQuantityDetail(this);
    });

    $('#lnkOtp').click(function (e) {
        GenerateOtp(this);
    });

    //$('input[name=paymentmethod]').unbind();
    //$('input[name=paymentmethod]').click(function (e) {
    //    LoadPaymentPage(this);
    //});

    $('input[name = paymentmethod]').bind('change', function () {
        var value = $(this).val();
        $("#offline").hide();
        $("#dtcard").hide();
        $("#" + value).show();
    });
});

function GenerateOtp(thisvar) {
    $("#loginError").html("");
    //var passwordDetail = $('#lnkOtp').val();
    $(".preloader").show();
    $.ajax({
        url: '/Manage/GenerateOtpDetail',
        type: 'Post',
        datatype: 'Json',
        data: {}
    }).done(function (result) {
        $("#otpMessage").html(result);
        $(".preloader").hide();

    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
    });

    return false;
}


function SaveDetailFormOtp() {
    $("#loginError").html("");
    var loginDetail = $('#addForm1').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Manage/SaveDetailFormOtp',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {
        $("#loginError1").html(result);
        $(".preloader").hide();

    }).fail(function (error) {
        $("#loginError1").html(error.statusText);
        $(".preloader").hide();
    });

    return false;
}

function SaveDetailForm() {
    $("#loginError").html("");
    var loginDetail = $('#addForm').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Manage/UpdateOrderDetail',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {
        $("#loginError").html(result);
        $(".preloader").hide();

    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
    });

    return false;
}

function UpdateQuantityDetail(thisVar) {

    $(".preloader").show();
    var idVal = $(thisVar).attr("data-id");
    var qty = $("#productQuantity").val();

    $.ajax({
        url: '/Manage/UpdateProductQuantityDetail',
        type: 'Post',
        datatype: 'Json',
        data: { prodId: idVal, quantity: qty }
    }).done(function (result) {
        $(".preloader").hide();
        alert(result);
        //location.reload();
    }).fail(function (error) {
        $("#error").html(error.statusText);
        $(".preloader").hide();
    });

    return false;
}

function DeleteDetail(thisVar) {

    $(".preloader").show();
    var idVal = $(thisVar).attr("data-id");

    $.ajax({
        url: '/Manage/DeleteCartProduct',
        type: 'Post',
        datatype: 'Json',
        data: { prodId: idVal }
    }).done(function (result) {
        $(".preloader").hide();
        alert(result);
        location.reload();
    }).fail(function (error) {
        $("#error").html(error.statusText);
        $(".preloader").hide();
    });

    return false;
}