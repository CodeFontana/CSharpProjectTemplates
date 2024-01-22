const storedTheme = localStorage.getItem('theme') || 'light';

const setTheme = function (theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem('theme', theme);

    const themeSwitches = document.querySelectorAll('.theme-switch');
    themeSwitches.forEach(switchElement => {
        if (theme === 'light') {
            switchElement.classList.replace('bi-sun-fill', 'bi-moon-stars');
        } else {
            switchElement.classList.replace('bi-moon-stars', 'bi-sun-fill');
        }
    });
}

window.addEventListener('DOMContentLoaded', () => {
    setTheme(storedTheme);

    const themeSwitches = document.querySelectorAll('.theme-switch');
    themeSwitches.forEach(switchElement => {
        switchElement.addEventListener('click', () => {
            const currentTheme = document.documentElement.getAttribute('data-bs-theme');
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';
            setTheme(newTheme);
        });
    });
});
