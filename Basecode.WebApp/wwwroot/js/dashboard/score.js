function showInputScoreModal(examId, applicantId) {
    $("#applicantId").text(applicantId);
    $('#applicantScore').val('');
    $('#perfectScore').val('');
    $('#percentage').val('');
    $('#examinationId').val(examId);
    $("#inputScoreModal").modal("show");
}

const inputScoreModal = document.getElementById('inputScoreModal')
inputScoreModal.addEventListener('show.bs.modal', function () {
    var $applicantScoreInput = $('#applicantScore');
    var $perfectScoreInput = $('#perfectScore');
    var $submitScoreButton = $('#submitScoreButton');

    // When the #applicantScore input changes
    $applicantScoreInput.on('change', function () {
        validatePerfectScore();
    });

    // When the #perfectScore input changes
    $perfectScoreInput.on('change', function () {
        validatePerfectScore();
    });

    function calculatePercentage(applicantScore, perfectScore) {
        if (perfectScore === 0) {
            return 0; // Handle division by zero
        }

        var percentage = (applicantScore / perfectScore) * 100;
        return parseInt(percentage.toFixed(0)); 
    }

    function validatePerfectScore() {
        var applicantScore = parseInt($applicantScoreInput.val());
        var perfectScore = parseInt($perfectScoreInput.val());

        var isApplicantScoreValid = !isNaN(applicantScore) && applicantScore <= perfectScore && applicantScore >= 0;
        var isPerfectScoreValid = !isNaN(perfectScore) && perfectScore >= applicantScore && perfectScore > 0;

        if (!isApplicantScoreValid || !isPerfectScoreValid) {
            // If applicantScore is greater than perfectScore or any input is empty
            $('#applicantScoreError').text(isApplicantScoreValid ? '' : "Applicant's score cannot be greater than perfect score.");
            $('#perfectScoreError').text(isPerfectScoreValid ? '' : "Perfect score cannot be smaller than applicant's score.");
            $('#percentage').val(''); // Clear the percentage value

            // Disable the submit button
            $submitScoreButton.prop('disabled', true);
        } else {
            $('#applicantScoreError').text('');
            $('#perfectScoreError').text('');

            // Calculate the percentage
            var percentage = calculatePercentage(applicantScore, perfectScore);
            $('#percentage').val(percentage);

            // Enable the submit button
            $submitScoreButton.prop('disabled', false);
        }
    }

    $('#submitScoreButton').click(function () {
        var examinationId = $('#examinationId').val();
        var applicantScore = $('#applicantScore').val();
        var perfectScore = $('#perfectScore').val();
        var percentage = calculatePercentage(applicantScore, perfectScore);

        var formData = {
            examinationId: examinationId,
            percentage: percentage
        };

        $submitScoreButton.prop('disabled', true);

        $.ajax({
            type: 'POST',
            url: $('#inputScoreForm').attr('action'),
            data: formData,
            success: () => window.location.assign(window.location.href),
            error: function (response) {
                window.location.assign(window.location.href);
            },
            complete: function () {
                // Re-enable the submit button after AJAX is complete
                $submitScoreButton.prop('disabled', false);
            }
        });
    });

    // When the modal is hidden
    inputScoreModal.addEventListener('hidden.bs.modal', function () {
        $("#applicantScoreError").text("");
        $("#perfectScoreError").text("");
        // Remove the input change listeners
        $applicantScoreInput.off('change', validatePerfectScore);
        $perfectScoreInput.off('change', validatePerfectScore);
    });
});