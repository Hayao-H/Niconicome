function showModal() {
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm=>{
        new bootstrap.Tooltip(elm)
    })
    const elm = document.querySelector("#addonModal");
    const modal = new bootstrap.Modal(elm, {});
    modal.show();
}