function showUpdateModal() {
    showTooltip();
    const elm = document.querySelector("#addonUpdateModal");
    const modal = new bootstrap.Modal(elm, {});
    modal.show();
}

function showUninstallModal() {
    showTooltip();
    const elm = document.querySelector("#addonUninstallModal");
    const modal = new bootstrap.Modal(elm, {});
    modal.show();
}

function showTooltip(){
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm=>{
        new bootstrap.Tooltip(elm);
    })
}