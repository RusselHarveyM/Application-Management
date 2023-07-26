function showAddModal() {
    $.get("/User/AddView", function (data) {
        $("#addModalContainer").html(data);
        $("#addUserModal").modal("show");
    });
}

function showEditModal(id) {
    $.get("/User/UpdateView/" + id, function (data) {
        $("#editModalContainer").html(data);
        $("#editUserModal").modal("show");
    });
}

function showDeleteModal(id) {
    $.get("/User/DeleteView/" + id, function (data) {
        $("#deleteModalContainer").html(data);
        $("#deleteUserModal").modal("show");
    });
}