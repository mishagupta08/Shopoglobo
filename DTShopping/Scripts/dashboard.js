﻿$(document).ready(function () {
    $(".preloader").hide();

    $('#stateList').unbind();
    $('#stateList').change(function (e) {
        GetCityByState();
    });

    $('#addToCart').click(function (e) {
        var prodId = $("#product_id").val();
        var quantity = $("#quantity").val();
        AddProductInCart(prodId, quantity);
    });
});

function AddProductInCart(prodId, quantity) {

    $(".preloader").show();
    $.ajax({
        url: '/Manage/AddProductInToCart',
        type: 'Post',
        datatype: 'Json',
        data: { ProductId: prodId, Quantity: quantity }
    }).done(function (result) {
        if (result == null || result == undefined || result == "") {
            alert("Something went wrong, Please try again later.");
        }
        else if (result.Status == false) {
            alert(result.ResponseValue);
        }
        else {
            $("#cartCount").val(result.CartProductCount);
        }

        $(".preloader").hide();
    }).fail(function (error) {
        alert(error.statusText);
        $(".preloader").hide();
    });

    return false;
}

function GetCityByState() {
    var id = $("#stateList").val();
    $(".preloader").show();
    $.ajax({
        url: '/Account/GetCityListByState',
        type: 'Post',
        datatype: 'Json',
        data: { Id: id }
    }).done(function (result) {
        if (result == null || result == undefined || result == "") {
            $("#error").html(result);
        }
        else {

            $("#cityList").html("");
            $.each(result, function (key, value) {
                $("#cityList").append($("<option></option>").val(value.cityID).html(value.cityName));
            });
        }

        $(".preloader").hide();
    });
}

function SaveDetailForm() {
    $("#loginError").html("");
    var loginDetail = $('#addForm').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Account/SaveDetail',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {

        $("#loginError").html(result);
        $(".preloader").hide();
        $("#closeError").show();

    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
        $("#closeError").show();
        $(".preloader").hide();
    });

    return false;
}


function Login_Account() {
    $("#loginError").html("");
    var loginDetail = $('#login_account').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Account/Login',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {
        if (result == "Success") {
            window.location.href = "/Home/Index";
        }
        else {
            $("#loginError").html(result);
        }
        $(".preloader").hide();
        $("#closeError").show();
    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
        $("#closeError").show();
        $(".preloader").hide();
    });

    return false;
}