// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function markAlertAsResolved(id) {
    fetch(`/Alerts/MarkAsResolvedAjax`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']").value
        },
        body: new URLSearchParams({ id })
    })
        .then(res => {
            if (res.ok) {
                document.querySelector(`#alert-row-${id}`)?.remove(); // remove from table
            }
        });
}

