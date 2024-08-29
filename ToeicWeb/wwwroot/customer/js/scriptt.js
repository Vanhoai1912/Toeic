const toggle = document.getElementById('toggleDark');
const body = document.querySelector('body,bgC');
/*const a = document.querySelectorAll('a')*/
/*const bgC = document.getElementsByClassName('bgC');*/


toggle.addEventListener('click', function(){
    this.classList.toggle('bi-moon');
    if (this.classList.toggle('bi-brightness-high-fill')) {
        body.style.background = 'rgb(245,247,251)';
        body.style.color = 'black ';
        body.style.transition = '1.5s';
    } else {
        body.style.background = 'rgb(34,34,34)';
        body.style.color = '#fff ';
        body.style.transition = '1.5s';
    }
});