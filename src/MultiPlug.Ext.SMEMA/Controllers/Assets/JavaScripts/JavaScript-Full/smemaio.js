﻿window.addEventListener("multiplugReconnecting", function (e) {
    $('#Connection-modal').modal('show');

    $("#Connection-modalTitle").text("Connection Disconnected");
    $("#Connection-modalDescription").text("The connection to the SMEMA interlock device has been lost. Attempts are being made to restore the connection.");
});

window.addEventListener("multiplugReconnected", function (e) {
    $("#Connection-modalTitle").text("Connection Restored");
    $("#Connection-modalDescription").text("The connection has now been restored, awaiting commands from the Server.");
});

function SMEMAEnabledIcon(theSelect, theValue) {
    theSelect.html(theValue == '1' ? '<i class="fas fa-lg fa-check"></i>' : '<i class="fas fa-lg fa-times"></i>');
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
    theSMEMADownlineBadBoardSubId) {
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
    });

    window.addEventListener("multiplugReady", function (e) {
        $.connection.wS.on("Send", function (id, Payload) {

            if (id == theSMEMAUplineMachineReadySubId) {
                SMEMAEnabledIcon($('#SMEMAUplineMachineReadyState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMAUplineGoodBoardSubId) {
                SMEMAEnabledIcon($('#SMEMAUplineGoodBoardState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMAUplineBadBoardSubId) {
                SMEMAEnabledIcon($('#SMEMAUplineBadBoardState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMAInterlockMachineReadySubId) {
                SMEMAEnabledIcon($('#SMEMAInterlockMachineReadyState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMAInterlockGoodBoardSubId) {
                SMEMAEnabledIcon($('#SMEMAInterlockGoodBoardState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMAInterlockBadBoardSubId) {
                SMEMAEnabledIcon($('#SMEMAInterlockBadBoardState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMADownlineMachineReadySubId) {
                SMEMAEnabledIcon($('#SMEMADownlineMachineReadyState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMADownlineGoodBoardSubId) {
                SMEMAEnabledIcon($('#SMEMADownlineGoodBoardState'), Payload.Pairs[0].Value);
            }
            else if (id == theSMEMADownlineBadBoardSubId) {
                SMEMAEnabledIcon($('#SMEMADownlineBadBoardState'), Payload.Pairs[0].Value);
            }
        });
    });
}