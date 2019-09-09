﻿$(document).ready(function () {
    $(".preloader").hide();

    $('a[name=deleteFunction]').unbind();
    $('a[name=deleteFunction]').click(function (e) {
        DeleteDetail(this);
    });

    $('a[name=updateQuantity]').unbind();
    $('a[name=updateQuantity]').click(function (e) {
        UpdateQuantityDetail(this);
    });
});

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