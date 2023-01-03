document.addEventListener("keydown", e => {
    if (e.key == "F5") {
        e.preventDefault();
        location.href = "/";
    }
});