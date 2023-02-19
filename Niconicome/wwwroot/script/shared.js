
function showTooltip() {
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm => {
        //new bootstrap.Tooltip(elm);
    })
}

function showToast() {
    document.querySelectorAll(".toast").forEach(elm => {
        const toast = new bootstrap.Toast(elm, { delay: 4000 });
        toast.show();
    })
}

function getBodyHeight() {
    return document.body.clientHeight;
}

function showModal() {
    const elm = document.querySelector("#Modal");
    const modal = new bootstrap.Modal(elm);
    modal.show();
}