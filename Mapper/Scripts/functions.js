$(document).ready(function () {

    $('.it-date-datepicker').datepicker({
        inputFormat: ["dd/MM/yyyy"],
        outputFormat: 'dd/MM/yyyy',
    });

    $('.required').prop('required', true);

    notificationShow('notifica');

    $('#msgAvvisoModale').modal('show');

    $('.fileUpload').change(function () {
        if ($(this).val()) {
            $(this).closest("div").find(".carica").show();
        }
    })
});


function ConfrontaEmail(sender, args) {
    args.IsValid = true;
    var emailOriginale = $("[id*='etbEmail']").val().toLowerCase();;
    var emailConfronto = $("[id*='etbConfermaEmail']").val().toLowerCase();;

    if (emailOriginale != emailConfronto) {
        $("[id*='etbConfermaEmail']").focus();
        args.IsValid = false;
    }
}

function chkbox_ClientValidate(sender, args) {
    args.IsValid = true;
    $(".required input:checkbox").each(function (index, el) {
        if (!$(el).is(':checked')) {
            $(el).focus();
            args.IsValid = false;
        }
    });
}

function date_compare(sender, args) {
    var startDt = $("[id*='dcDal']").val();
    var endDt = $("[id*='dcAl']").val();
    args.IsValid = true;
    if (startDt != null && endDt != null && (new Date(startDt).getTime() > new Date(endDt).getTime())) {
        $("[id*='dcDal']").focus();
        args.IsValid = false;
    }
}

var confirmed = false;
function BootstrapConfirm(msg, titolo, controlID) {
    if (confirmed) { return true; }
    bootbox.confirm({
        title: titolo,
        message: msg,
        backdrop: false,
        buttons: {
            confirm: {
                label: 'Conferma',
                className: 'btn-primary btn-sm'
            },
            cancel: {
                label: 'Annulla',
                className: 'btn-outline-primary btn-sm'
            }
        },
        callback: function (result) {
            if (result) {
                if (controlID != null) {
                    var controlToClick = document.getElementById(controlID);
                    if (controlToClick != null) {
                        confirmed = true;
                        controlToClick.click();
                        confirmed = false;
                    }
                }
            }
            else {
                setTimeout(function () {
                    $('#' + controlID).focus();
                }, 100);
            }
        }
    });
    return false;
}

//var prm2 = Sys.WebForms.PageRequestManager.getInstance();
//prm2.add_endRequest(function () {
//    $('.it-date-datepicker').datepicker({
//        inputFormat: ["dd/MM/yyyy"],
//        outputFormat: 'dd/MM/yyyy',
//    });
//});

(function () {
    'use strict';
    window.addEventListener('load', function () {
        var forms = document.getElementsByClassName('needs-validation');
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
            }, false);
            form.classList.add('was-validated');
        });
    }, false);
})();