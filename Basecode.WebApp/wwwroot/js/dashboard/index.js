function showAssignUsersModal(jobOpeningId) {

    $.get("/Dashboard/AssignUsersView/" + jobOpeningId, function (data) {
        $("#assignUsersModalContainer").html(data);
        $("#assignUsersModal").modal("show");
    });
}

function showShortlistViewModal(applicantId) {
    $.get("/Dashboard/ViewDetails/" + applicantId, function (data) {
        $("#showShortlistViewModalContainer").html(data);
        $("#viewApplicationModal").modal("show");
    });
}
