window.addEventListener("multiplugReconnecting", function (e) {
    $('#Connection-modal').modal('show');

    $("#Connection-modalTitle").text("Connection Disconnected");
    $("#Connection-modalDescription").text("The connection to the SMEMA interlock device has been lost. Attempts are being made to restore the connection.");
});

window.addEventListener("multiplugReconnected", function (e) {
    $("#Connection-modalTitle").text("Connection Restored");
    $("#Connection-modalDescription").text("The connection has now been restored, awaiting commands from the Server.");
});

function DrawEnabledIcon(theSelect, theValue) {
    theSelect.html(theValue == '1' ? '<i class="fas fa-lg fa-check"></i>' : '<i class="fas fa-lg fa-times"></i>');
}

function DrawLeftIcon(theSelect, theValue) {
    theSelect.html(theValue == '1' ? '<i class="fas fa-lg fa-arrow-left"></i>' : '<i class="fas fa-lg fa-times"></i>');

    UpdateColour(theSelect, theValue, "btn-smema-unblocked", "btn-smema-blocked");
}

function DrawRightIcon(theSelect, theValue) {
    theSelect.html(theValue == '1' ? '<i class="fas fa-lg fa-arrow-right"></i>' : '<i class="fas fa-lg fa-times"></i>');

    UpdateColour(theSelect, theValue, "btn-smema-unblocked", "btn-smema-blocked");
}

function DrawLatchedIcon(theSelect, theValue) {
    theSelect.html(theValue == '1' ? '<i class="fas fa-lg fa-lock"></i>' : '<i class="fas fa-lg fa-lock-open"></i>');
}

function DrawRightDivertedIcon(theSelect, theValue, isBadBoard)
{
    var Rotate = (theValue == '0') ? 0 : isBadBoard ? 315 : 45;

    theSelect.html("<i class=\"fas fa-lg fa-arrow-right divert-rotate-" + Rotate + "\"></i>");

    UpdateColour(theSelect, theValue, "btn-smema-diverted", "btn-smema-unblocked");
}

function DrawLeftDivertedIcon(theSelect, theValue, isBadBoard) {
    var Rotate = (theValue == '0') ? 0 : isBadBoard ? 45 : 315;

    theSelect.html("<i class=\"fas fa-lg fa-arrow-left divert-rotate-" + Rotate + "\"></i>");

    UpdateColour(theSelect, theValue, "btn-smema-diverted", "btn-smema-unblocked");
}

function UpdateColour(theSelect, theValue, theEnabledClass, theDisabledClass)
{
    if (theValue == '0') {
        theSelect.removeClass(theEnabledClass);
        theSelect.addClass(theDisabledClass);
    }
    else
    {
        theSelect.removeClass(theDisabledClass);
        theSelect.addClass(theEnabledClass);
    }
}

function UpdateApiHref(theSelect, theValue) {

    var Current = theSelect.attr("data-api-href");

    if (theValue == '1')
    {
        theSelect.attr("data-api-href", Current.replace("true", "false"));
    }
    else
    {
        theSelect.attr("data-api-href", Current.replace("false", "true"));
    }
}

function Initialise(theAPIPath,
    theLaneGuid,
    theSMEMAUplineMachineReadySubId,
    theSMEMAUplineGoodBoardSubId,
    theSMEMAUplineBadBoardSubId,
    
    theSMEMAInterlockMachineReadySubId,
    theSMEMAInterlockGoodBoardSubId,
    theSMEMAInterlockBadBoardSubId,   

    theSMEMADownlineMachineReadySubId,
    theSMEMADownlineGoodBoardSubId,
    theSMEMADownlineBadBoardSubId,
    
    theSMEMAUplineGoodBoardAlways,
    theSMEMAUplineBadBoardAlways,
    theSMEMADownlineMachineReadyAlways) {
    g_APIPath = theAPIPath;

    $(".btn-shutdown").click(function (event) {
        event.preventDefault()

        $.ajax({
            type: "POST",
            url: theAPIPath + 'power/shutdown/',
            success: function () {
                $('#Shutdown-modal').modal('hide');
            },
        });
        $('#Shutdown-modal').modal('hide');
    });

    $(".btn-restart").click(function (event) {
        event.preventDefault()

        $.ajax({
            type: "POST",
            url: theAPIPath + 'power/restart/',
            success: function () {
                $('#Shutdown-modal').modal('hide');
            },
        });
        $('#Shutdown-modal').modal('hide');
    });

    $(".btn-apicall").click(function (event) {
        event.preventDefault();

        $.post($(this).attr("data-api-href"), function (data) {

        })
        .done(function () {

        });
    });

    if (theSMEMAUplineGoodBoardAlways)
    {
        DrawLatchedIcon($('#SMEMAUplineGoodBoardState'), '1');
    }

    if (theSMEMAUplineBadBoardAlways)
    {
        DrawLatchedIcon($('#SMEMAUplineBadBoardState'), '1');
    }

    if (theSMEMADownlineMachineReadyAlways)
    {
        DrawLatchedIcon($('#SMEMADownlineMachineReadyState'), '1');
    }

    window.addEventListener("multiplugReady", function (e) {
        $.connection.wS.on("Send", function (id, Payload) {

            if (id == theSMEMAUplineMachineReadySubId) {
                DrawEnabledIcon($('#SMEMAUplineMachineReadyState'), Payload.Subjects[0].Value);
            }
            else if (id == theSMEMAUplineGoodBoardSubId) {
                if (theSMEMAUplineGoodBoardAlways){ return; }
                DrawEnabledIcon($('#SMEMAUplineGoodBoardState'), Payload.Subjects[0].Value);
            }
            else if (id == theSMEMAUplineBadBoardSubId) {
                if (theSMEMAUplineBadBoardAlways) { return; }
                DrawEnabledIcon($('#SMEMAUplineBadBoardState'), Payload.Subjects[0].Value);
            }
            else if (id == theSMEMAInterlockMachineReadySubId) {
                var item = $('#SMEMAInterlockMachineReadyState');

                if (item.hasClass("smema-righttoleft")){
                    DrawRightIcon(item, Payload.Subjects[0].Value);
                }
                else {
                    DrawLeftIcon(item, Payload.Subjects[0].Value);
                }

                UpdateApiHref(item, Payload.Subjects[0].Value);

                item = $('#SMEMAInterlockMachineReadyLatchState');
                DrawLatchedIcon(item, Payload.Subjects[2].Value);
                UpdateApiHref(item, Payload.Subjects[2].Value);
            }
            else if (id == theSMEMAInterlockGoodBoardSubId) {

                var item = $('#SMEMAInterlockGoodBoardState');
                if (item.hasClass("smema-righttoleft")) {
                    DrawLeftIcon(item, Payload.Subjects[0].Value);
                }
                else {
                    DrawRightIcon(item, Payload.Subjects[0].Value);
                }
                UpdateApiHref(item, Payload.Subjects[0].Value);

                item = $('#SMEMAInterlockGoodBoardLatchState');
                DrawLatchedIcon(item, Payload.Subjects[2].Value);
                UpdateApiHref(item, Payload.Subjects[2].Value);

                item = $('#SMEMAInterlockGoodBoardDivertState');
                if (item.hasClass("smema-righttoleft")) {
                    DrawLeftDivertedIcon(item, Payload.Subjects[3].Value, false);
                }
                else{
                    DrawRightDivertedIcon(item, Payload.Subjects[3].Value, false);
                }

                UpdateApiHref(item, Payload.Subjects[3].Value);

                item = $('#SMEMAInterlockGoodBoardDivertLatchState');
                DrawLatchedIcon(item, Payload.Subjects[4].Value);
                UpdateApiHref(item, Payload.Subjects[4].Value);
            }
            else if (id == theSMEMAInterlockBadBoardSubId) {

                var item = $('#SMEMAInterlockBadBoardState');
                if (item.hasClass("smema-righttoleft")) {
                    DrawLeftIcon(item, Payload.Subjects[0].Value);
                }
                else {
                    DrawRightIcon(item, Payload.Subjects[0].Value);
                }
                
                UpdateApiHref(item, Payload.Subjects[0].Value);

                item = $('#SMEMAInterlockBadBoardLatchState');
                DrawLatchedIcon(item, Payload.Subjects[2].Value);
                UpdateApiHref(item, Payload.Subjects[2].Value);

                item = $('#SMEMAInterlockBadBoardDivertState');
                if (item.hasClass("smema-righttoleft")) {
                    DrawLeftDivertedIcon(item, Payload.Subjects[3].Value, true);
                }
                else {
                    DrawRightDivertedIcon(item, Payload.Subjects[3].Value, true);
                }

                UpdateApiHref(item, Payload.Subjects[3].Value);

                item = $('#SMEMAInterlockBadBoardDivertLatchState');
                DrawLatchedIcon(item, Payload.Subjects[4].Value);
                UpdateApiHref(item, Payload.Subjects[4].Value);
            }
            else if (id == theSMEMADownlineMachineReadySubId) {
                if (theSMEMADownlineMachineReadyAlways) { return;}
                DrawEnabledIcon($('#SMEMADownlineMachineReadyState'), Payload.Subjects[0].Value);
            }
            else if (id == theSMEMADownlineGoodBoardSubId) {
                DrawEnabledIcon($('#SMEMADownlineGoodBoardState'), Payload.Subjects[0].Value);
            }
            else if (id == theSMEMADownlineBadBoardSubId) {
                DrawEnabledIcon($('#SMEMADownlineBadBoardState'), Payload.Subjects[0].Value);
            }
        });
    });
}