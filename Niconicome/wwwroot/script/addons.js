function showModal() {
    showTooltip();
    const elm = document.querySelector("#addonModal");
    const modal = new bootstrap.Modal(elm, {});
    modal.show();
}

function showTooltip(){
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm=>{
        new bootstrap.Tooltip(elm);
    })
}