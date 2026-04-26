// =============================================================================
//  CombinedLayout — slide-out offcanvas sidebar wiring.
// =============================================================================
//  Theme handling lives in ThemePicker.razor.js. This module is now only
//  responsible for the offcanvas sidebar (open/close/backdrop).
// =============================================================================

function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    if (!sidebar) return;

    if (sidebar.classList.contains("show")) {
        hideSidebar(sidebar, true);
    } else {
        showSidebar(sidebar);
    }
}

function showSidebar(sidebar) {
    if (!sidebar) return;
    sidebar.classList.add("show");
    addBackdrop(sidebar);
}

function hideSidebar(sidebar, useTransition) {
    if (!sidebar) return;

    if (useTransition) {
        sidebar.classList.add("hiding");
        setTimeout(() => {
            sidebar.classList.remove("show");
            sidebar.classList.remove("hiding");
            removeBackdrop();
        }, 300);
    } else {
        sidebar.classList.remove("show");
        removeBackdrop();
    }
}

function addBackdrop(sidebar) {
    const layout = document.getElementById("combined-layout");
    if (!layout) return;
    if (document.getElementById("offcanvas-fade")) return;

    const backdrop = document.createElement("div");
    backdrop.id = "offcanvas-fade";
    backdrop.className = "offcanvas-backdrop fade show";
    backdrop.style = "z-index: 1020;";

    backdrop.addEventListener("click", () => {
        hideSidebar(sidebar, true);
    });

    layout.appendChild(backdrop);
}

function removeBackdrop() {
    document.getElementById("offcanvas-fade")?.remove();
}

export default class extends BlazorJSComponents.Component {
    setParameters() {
        // Ensure the sidebar starts hidden after every render.
        const sidebar = document.getElementById("sidebar");
        if (sidebar) {
            hideSidebar(sidebar, false);
        }

        // Idempotent click binding for the hamburger button.
        const sidebarButton = document.getElementById("sidebarButton");
        if (sidebarButton) {
            this.setEventListener(sidebarButton, "click", () => toggleSidebar());
        }
    }

    dispose() {
        removeBackdrop();
    }
}
