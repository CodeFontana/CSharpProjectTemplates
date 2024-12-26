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

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    if (sidebar) {
        if (sidebar.classList.contains('show')) {
            sidebar.classList.add('hiding');
            setTimeout(() => {
                sidebar.classList.remove('show');
                sidebar.classList.remove('hiding');
                removeBackdrop();
            }, 300); // Match the CSS transition
        } else {
            sidebar.classList.add('show');
            addBackdrop();
        }
    }
}

function addBackdrop() {
    const layout = document.getElementById('combined-layout');
    const backdrop = document.createElement('div');
    backdrop.id = 'offcanvas-fade';
    backdrop.className = 'offcanvas-backdrop fade show';
    backdrop.style = 'z-index: 1020;';
    backdrop.addEventListener('click', () => {
        removeBackdrop();
        toggleSidebar();
    });
    if (layout && backdrop) {
        layout.appendChild(backdrop);
    }
}

function removeBackdrop() {
    const backdrop = document.getElementById('offcanvas-fade');
    const mainLayout = document.getElementById('combined-layout');
    if (backdrop && mainLayout) {
        mainLayout.removeChild(backdrop);
        backdrop.removeEventListener('click', backdrop);
    }
}

export function onLoad() {
    // Add click listeners to theme switches
    const themeSwitches = document.querySelectorAll('.theme-switch');
    themeSwitches.forEach(switchElement => {
        switchElement.addEventListener('click', () => {
            const currentTheme = localStorage.getItem('theme') || 'light';
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';
            setTheme(newTheme);
        });
    });

    // Add click listener to sidebar button
    const sidebarButton = document.getElementById('sidebarButton');
    if (sidebarButton) {
        sidebarButton.addEventListener('click', () => {
            toggleSidebar();
        });
    }
}

export function onUpdate() {
    // Ensure theme is applied
    const theme = localStorage.getItem('theme') || 'light';
    setTheme(theme);

    // Remove the sidebar (is showing), without transition
    const sidebar = document.getElementById('sidebar');
    if (sidebar) {
        sidebar.classList.remove('show');
    }
}

export function onDispose() {

}