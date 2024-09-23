function initializeReloadHandler(helper) {
    document.addEventListener('keydown', async e => {
        if (e.key === "F5") {
            await helper.invokeMethodAsync('OnReload');
            location.href = "/";
        }
    });
}


function showTooltip() {
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(elm => {
        //new bootstrap.Tooltip(elm);
    })
}

function showToast() {
    document.querySelectorAll(".toast").forEach(elm => {
        const toast = new bootstrap.Toast(elm, { delay: 4000});
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

function initializeVideo() {
    videojs('video-player', {
        controls: true,
        autoplay: false,
        preload: 'auto',
    });
}

function setTheme(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
    if (theme === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
}