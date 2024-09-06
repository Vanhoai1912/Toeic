const toggle = document.getElementById('toggleDark');
const body = document.querySelector('body');

// Khôi phục trạng thái màu từ localStorage khi trang được tải lại
window.onload = function () {
    const theme = localStorage.getItem('theme');
    if (theme === 'dark') {
        body.style.background = 'rgb(34,34,34)';
        body.style.color = '#fff';
        toggle.classList.add('bi-brightness-high-fill');
        toggle.classList.remove('bi-moon');
    } else {
        body.style.background = 'rgb(245,247,251)';
        body.style.color = 'black';
        toggle.classList.add('bi-moon');
        toggle.classList.remove('bi-brightness-high-fill');
    }
};

toggle.addEventListener('click', function () {
    this.classList.toggle('bi-moon');
    if (this.classList.toggle('bi-brightness-high-fill')) {
        body.style.background = 'rgb(245,247,251)';
        body.style.color = 'black';
        localStorage.setItem('theme', 'light');
    } else {
        body.style.background = 'rgb(34,34,34)';
        body.style.color = '#fff';
        localStorage.setItem('theme', 'dark');
    }
    body.style.transition = '1.5s';
});

const navlinkEls = document.querySelectorAll(".nav-linkH");
navlinkEls.forEach(navlinkEl => {
    navlinkEl.addEventListener("click", () => {
        document.querySelector(".active")?.classList.remove("active");
        navlinkEl.classList.add("active");
    });
});
