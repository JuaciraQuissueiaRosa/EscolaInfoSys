// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function markAlertAsResolved(id) {
    fetch('/Alerts/Respond', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `__RequestVerificationToken=${token}&id=${id}&adminResponse=${encodeURIComponent(responseText)}`
    })

}

