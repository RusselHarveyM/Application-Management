function showAssignUsersModal(jobOpeningId) {

    $.get("/Dashboard/AssignUsersView/" + jobOpeningId, function (data) {
        $("#assignUsersModalContainer").html(data);
        $("#assignUsersModal").modal("show");
    });
}