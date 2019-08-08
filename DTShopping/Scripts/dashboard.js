$(document).ready(function () {
    $(".preloader").hide();

    $('#stateList').unbind();
    $('#stateList').change(function (e) {
        GetCityByState();
    });
});

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