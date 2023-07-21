$(document).ready(function () {
    $('#jobOpeningDropdown').select2({
        placeholder: "Select an assigned job opening",
    });

    $('#meetingTypeDropdown').select2({
        placeholder: "Select a meeting type",
        disabled: true,
        minimumResultsForSearch: Infinity,  // Hide the search box
    });

    $('#applicantDropdown').select2({
        placeholder: "Select an applicant",
        disabled: true,
        language: {
            noResults: () => "No applicants found",
        },
    });
});

$('#jobOpeningDropdown').on('change', function () {
    var jobOpeningId = $('#jobOpeningDropdown').val();

    if (jobOpeningId) {
        $('#selectedApplicants').empty();
        $('#applicantDropdown').empty().trigger("change");      // Clear options
        $('#applicantDropdown').append('<option></option>');    // Add placeholder

        // Show applicants whose statuses match the meeting type (ex. HR Interview)
        applicants.forEach(function (applicant) {
            if (applicant.JobOpeningId == jobOpeningId && applicant.Status == $('#meetingTypeDropdown').val())
                $('#applicantDropdown').append(`<option value=${applicant.Id}>${applicant.Firstname} ${applicant.Lastname}</option>`);
        });

        $('#meetingTypeDropdown').prop('disabled', false);

    } else {
        $('#meetingTypeDropdown').prop('disabled', true);
    }
});

$('#meetingTypeDropdown').on('change', function () {
    var meetingType = $('#meetingTypeDropdown').val();

    if (meetingType) {
        $('#selectedApplicants').empty();
        $('#applicantDropdown').empty().trigger("change");      // Clear options
        $('#applicantDropdown').append('<option></option>');    // Add placeholder

        // Show applicants whose statuses match the meeting type (ex. HR Interview)
        applicants.forEach(function (applicant) {
            if (applicant.Status == meetingType && applicant.JobOpeningId == $('#jobOpeningDropdown').val())
                $('#applicantDropdown').append(`<option value=${applicant.Id}>${applicant.Firstname} ${applicant.Lastname}</option>`);
        });

        $('#applicantDropdown').prop('disabled', false);

    } else {
        $('#applicantDropdown').prop('disabled', true);
    }
});

$('#applicantDropdown').on('change', function () {
    if ($('#applicantDropdown').val()) {
        $('#addApplicantButton').prop('disabled', false);
    } else {
        $('#addApplicantButton').prop('disabled', true);
    }
});

$('#addApplicantButton').click(function () {
    var selectedApplicantId = $('#applicantDropdown').val();
    if (selectedApplicantId) {
        var selectedApplicant = applicants.find(function (applicant) {
            return applicant.Id == selectedApplicantId;
        });

        if (selectedApplicant) {
            AddToRow(selectedApplicant);
            RemoveFromDropdown(selectedApplicant);

            // Clear the selection in the dropdown
            $('#applicantDropdown').val(null).trigger('change');
        }
    }
});

function AddToRow(applicant) {
    const newRow = document.createElement('tr');
    newRow.innerHTML = `
    <td>${applicant.Firstname} ${applicant.Lastname}</td>
    <td> <input type="time"> </td>
    <td data-applicant-id="${applicant.Id}">
    <button class="remove-button"><i class="fa-solid fa-xmark"></i></button>
    </td>
    `;
    $('#selectedApplicants').append(newRow);
}

function RemoveFromDropdown(applicant) {
    $('#applicantDropdown option[value="' + applicant.Id + '"]').remove();
}

$('#selectedApplicants').on("click", ".remove-button", function (event) {
    event.stopPropagation(); // Prevent event propagation
    var applicantId = $(this).parent().data('applicant-id');

    var applicant = applicants.find(function (applicant) {
        return applicant.Id == applicantId;
    });

    if (applicant) {
        $(this).closest('tr').remove();
        AddToDropdown(applicant);
    }
});

function AddToDropdown(applicant) {
    var optionHtml = '<option value="' + applicant.Id + '">' + applicant.Firstname + " " + applicant.Lastname + '</option>';
    $('#applicantDropdown').append(optionHtml);
}

$('#scheduleForm').submit(function (event) {
    event.preventDefault();

    // Get the validation token from the form
    var token = $('input[name="__RequestVerificationToken"]').val();

    var applicantData = [];
    $('#selectedApplicants').find('tr').each(function () {
        var applicantId = $(this).find('td:last').data('applicant-id');
        var time = $(this).find('td:eq(1)').find('input[type="time"]').val();

        applicantData.push({ ApplicantId: applicantId, Time: time });
    });

    var formData = {
        JobOpeningId: $('#jobOpeningDropdown').val(),
        Type: $('#meetingTypeDropdown').val(),
        Date: $('#date').val(),
        ApplicantData: applicantData 
    };

    console.log(formData);

    // Make an AJAX POST request to the controller endpoint
    $.ajax({
        url: $('#scheduleForm').attr('action'),
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            formData: formData
        },
        success: () => window.location.reload(),
        error: function (response) {
            if (response.status === 400) {
                var errors = response.responseJSON.value;

                // Iterate over the errors and display them
                $.each(errors, function (key, value) {
                    $('#' + key + 'Error').text(value);
                    console.log(key + value);
                });
            } else {
                console.log(response.status + " Something went wrong.");
            }
        }
    });
    return false;
});