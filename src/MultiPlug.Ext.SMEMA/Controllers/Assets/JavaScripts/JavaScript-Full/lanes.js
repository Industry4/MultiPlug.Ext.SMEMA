$("#btn-newlane").click(function () {
    $('#lanesTable tr:last').before(NewLane());

    $(".btn-deletelanetemp").click(function (event) {
        event.preventDefault();
        $(this).closest("tr").remove();
    });

});

$(".btn-deletelane").click(function (event) {
    event.preventDefault();

    var theRow = $(this).closest("tr");

    $.post($(this).attr('href'), function (data) {

    })
    .done(function () {
        theRow.remove();
    });
});

function NewLane() {
    return '<tr>\
                <td class="span4"><input type="text" name="LaneId" value=""></td>\
                <td class="span4"><input type="text" name="MachineName" value=""></td>\
                <td class="span4"><a class="btn btn-red btn-deletelanetemp" href="#"><i class="icon-trash"></i></a></td>\
            </tr>'
}