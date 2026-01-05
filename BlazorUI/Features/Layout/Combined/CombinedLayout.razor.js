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
    if (!layout) return;

    // Avoid duplicates if called repeatedly
    if (document.getElementById('offcanvas-fade')) return;

    const backdrop = document.createElement('div');
    backdrop.id = 'offcanvas-fade';
    backdrop.className = 'offcanvas-backdrop fade show';
    backdrop.style = 'z-index: 1020;';

    backdrop.addEventListener('click', () => {
        removeBackdrop();
        hideSidebar(sidebar, true);
    });

    layout.appendChild(backdrop);
}

function removeBackdrop() {
    const backdrop = document.getElementById('offcanvas-fade');
    backdrop?.remove();
}

function showSidebar(sidebar) {
    if (!sidebar) return;

    sidebar.classList.add('show');
    addBackdrop(sidebar);
}

function hideSidebar(sidebar, useTransition) {
    if (!sidebar) return;

    if (useTransition) {
        sidebar.classList.add('hiding');
        setTimeout(() => {
            sidebar.classList.remove('show');
            sidebar.classList.remove('hiding');
            removeBackdrop();
        }, 300);
    } else {
        sidebar.classList.remove('show');
        removeBackdrop();
    }
}

export default class extends BlazorJSComponents.Component {
    setParameters() {
        // Apply theme every render
        const theme = localStorage.getItem('theme') || 'light';
        setTheme(theme);

        // Ensure sidebar is closed after a render
        const sidebar = document.getElementById('sidebar');
        if (sidebar) {
            hideSidebar(sidebar, false);
        }

        // Wire event listeners in an idempotent way
        const sidebarButton = document.getElementById('sidebarButton');
        if (sidebarButton) {
            this.setEventListener(sidebarButton, 'click', () => toggleSidebar(sidebar));
        }

        // Theme toggles can be present in navbar/topbar; re-bind each render safely
        const themeSwitches = document.querySelectorAll('.theme-switch');
        themeSwitches.forEach(switchElement => {
            this.setEventListener(switchElement, 'click', () => {
                const currentTheme = localStorage.getItem('theme') || 'light';
                const newTheme = currentTheme === 'light' ? 'dark' : 'light';
                setTheme(newTheme);
            });
        });
    }

    dispose() {
        // setEventListener bindings are automatically removed by the base class.
        removeBackdrop();
    }
}