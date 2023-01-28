document.addEventListener("keydown", e => {
    if (e.key == "F5") {
        e.preventDefault();
        location.href = "/";
    }
});

function showTooltip(){
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm=>{
        new bootstrap.Tooltip(elm);
    })
}

function showToast(){
    document.querySelectorAll(".toast").forEach(elm=>{
       const toast = new bootstrap.Toast(elm,{delay:4000});
       toast.show();
    })
}