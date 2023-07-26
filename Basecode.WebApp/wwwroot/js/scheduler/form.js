$(document).ready(function () {
    $('#jobOpeningDropdown').select2({
        theme: "bootstrap-5",
        placeholder: "Select an assigned job opening",
    });

    $('#meetingTypeDropdown').select2({
        theme: "bootstrap-5",
        placeholder: "Select a meeting type",
        disabled: true,
        minimumResultsForSearch: Infinity,  // Hide the search box
    });

    $('#applicantDropdown').select2({
        theme: "bootstrap-5",
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

        // Show applicants whose statuses match the meeting type (ex. HR Interview) for the job opening
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

        // Show applicants whose statuses match the meeting type (ex. HR Interview) for the job opening
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
    <td> <input type="time" class="form-control-sm"> </td>
    <td data-applicant-id="${applicant.Id}">
    <button class="remove-button btn-close"></button>
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

function getCurrentDate() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0');
    var year = today.getFullYear();
    return year + '-' + month + '-' + day;
}

// Set the minimum date for the date input to the current date
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('date').setAttribute('min', getCurrentDate());
});

$('#scheduleForm').submit(function (event) {
    event.preventDefault();

    // Get the validation token from the form
    var token = $('input[name="__RequestVerificationToken"]').val();

    var applicantSchedules = [];
    $('#selectedApplicants').find('tr').each(function () {
        var applicantId = $(this).find('td:last').data('applicant-id');
        var time = $(this).find('td:eq(1)').find('input[type="time"]').val();

        applicantSchedules.push({ ApplicantId: applicantId, Time: time });
    });

    var formData = {
        JobOpeningId: $('#jobOpeningDropdown').val(),
        Type: $('#meetingTypeDropdown').val().split(' ').slice(1).join(' '), // Remove "For " from string
        Date: $('#date').val(),
        ApplicantSchedules: applicantSchedules 
    };

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

                // Display custom validation errors
                $.each(errors, function (key, value) {
                    $('#' + key + 'Error').text(value);
                });
            } else {
                console.log(response.status + " Something went wrong.");
            }
        }
    });
    return false;
});