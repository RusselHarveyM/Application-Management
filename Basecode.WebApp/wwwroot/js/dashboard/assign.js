$(document).ready(function () {
    // Populate the userDropdown with options from the ViewModel
    var usersData = JSON.parse($('#userData').val());

    $('.modal').on('shown.bs.modal', function () {
        UpdateTotalAssignedUsers();

        // Initialize Select2 for the userDropdown
        $('#userDropdown').select2({
            theme: "bootstrap-5",
            placeholder: "Search for users...",
            maximumInputLength: 50,
            dropdownParent: $("#assignUsersModal"),
            allowClear: true,
        });

        usersData.forEach(function (user) {
            if (!user.isLinkedToJobOpening)
                $('#userDropdown').append('<option value="' + user.aspId + '">' + user.email + '</option>');
        });
    });

    function UpdateTotalAssignedUsers() {
        const tableRows = document.querySelectorAll('#usersList tr').length;
        document.getElementById("totalAssignedUsers").textContent = tableRows;
    }

    $('#userDropdown').on('change', function () {
        if ($('#userDropdown').val()) {
            $('#confirmButton').prop('disabled', false);
        } else {
            $('#confirmButton').prop('disabled', true);
        }
    });

    // Handle the confirmButton click event
    $('#confirmButton').click(function () {
        var selectedUserId = $('#userDropdown').val();
        if (selectedUserId) {
            var selectedUser = usersData.find(function (user) {
                return user.aspId == selectedUserId;
            });

            if (selectedUser) {
                AddUserToRow(selectedUser);
                RemoveUserFromDropdown(selectedUser);

                // Clear the selection in the dropdown
                $('#userDropdown').val(null).trigger('change');
            }
        }
    });

    function AddUserToRow(selectedUser) {
        const newRow = document.createElement('tr');
        newRow.innerHTML = `
        <td>${selectedUser.fullname}</td>
        <td>${selectedUser.email}</td>
        <td data-user-id="${selectedUser.aspId}">
        <button class="remove-button btn-close"></button>
        </td>
        `;
        $('#usersList').append(newRow);
        UpdateTotalAssignedUsers();
    }

    function RemoveUserFromDropdown(user) {
        $('#userDropdown option[value="' + user.aspId + '"]').remove();
    }

    // Use event delegation to handle click events on "remove-button" elements
    $('#usersList').on("click", ".remove-button", function (event) {
        event.stopPropagation(); // Prevent event propagation

        var userId = $(this).parent().data('user-id');
        var user = usersData.find(function (user) {
            return user.aspId == userId;
        });

        if (user) {
            $(this).closest('tr').remove();
            AddUserToDropdown(user);
            UpdateTotalAssignedUsers();
        }
    });

    function AddUserToDropdown(user) {
        var optionHtml = '<option value="' + user.aspId + '">' + user.email + '</option>';
        $('#userDropdown').append(optionHtml);
    }

    // Handle the "SAVE CHANGES" button click event
    $('#saveChangesButton').click(function () {
        var jobOpeningId = $('#hiddenJobOpeningId').val();
        var assignedUserIds = [];

        // Get the IDs of the users assigned to the JobOpening from the table
        $('#usersList').find('tr').each(function () {
            var userId = $(this).find('td:last').data('user-id');
            console.log(userId);
            assignedUserIds.push(userId);
        });

        // Make an AJAX POST request to the controller endpoint
        $.ajax({
            url: '/Dashboard/AssignUserViewUpdate',
            type: 'POST',
            data: {assignedUserIds: assignedUserIds, jobOpeningId: jobOpeningId},
            success: () => window.location.reload(),
            error: function (response) {
                console.log(response.status + " Something went wrong.");
            }
        });
    });

    // Delete the modal's html if it has been closed
    $('.modal').on('hidden.bs.modal', function () {
        $(this).parent().empty();
    });
});
