const storedTheme = localStorage.getItem('theme');

const getPreferredTheme = () => {
    if (storedTheme) {
        return storedTheme;
    }
    return 'light';
}

const setTheme = function(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem('theme', theme);
    showActiveTheme(theme);
}

const showActiveTheme = (theme) => {
    const themeSwitcher = document.getElementById('themeSwitch');
    if (theme === 'light') {
        themeSwitcher.classList.remove('bi-sun-fill');
        themeSwitcher.classList.add('bi-moon-stars');
        themeSwitcher.setAttribute('data-bs-theme-value', 'light');
    } else {
        themeSwitcher.classList.remove('bi-moon-stars');
        themeSwitcher.classList.add('bi-sun-fill');
        themeSwitcher.setAttribute('data-bs-theme-value', 'dark');
    }
}

window.addEventListener('DOMContentLoaded', () => {
    const currentTheme = getPreferredTheme();
    setTheme(currentTheme);

    const themeSwitcher = document.getElementById('themeSwitch');
    themeSwitcher.addEventListener('click', () => {
        let newTheme = themeSwitcher.getAttribute('data-bs-theme-value') === 'light' ? 'dark' : 'light';
        setTheme(newTheme);
    });
});