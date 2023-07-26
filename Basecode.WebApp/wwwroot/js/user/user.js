$('.modal').on('shown.bs.modal', function () {
    var modal = $(this); // Get the modal element that has just been shown
    var form = modal.find('form'); 
    var formId = '#' + form.attr('id'); 

    $(formId).submit(function (e) {
        e.preventDefault();

        var form = $(this);

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: form.serialize(),
            success: () => window.location.reload(), 
            error: function (response) {
                if (response.status === 400) {
                    var errors = response.responseJSON.value;

                    // Iterate over the errors and display them
                    $.each(errors, function (key, value) {
                        if (formId == "#editUserForm") {
                            key = 'Update' + key;
                        }
                        $('#' + key + 'Error').text(value);
                    });
                }
                else {
                    console.log("Something went wrong.");
                }
            }
        });
    });
});

// Delete the modal's html if it has been closed
$('.modal').on('hidden.bs.modal', function () {
    $(this).parent().empty();
});

