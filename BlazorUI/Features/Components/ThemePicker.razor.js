// =============================================================================
//  ThemePicker — applies and persists a 3-state theme (light / dark / auto).
// =============================================================================
//  - Resolves "auto" against the OS preference (prefers-color-scheme).
//  - Writes [data-bs-theme] on <html> so Bootstrap 5.3's CSS variables flip.
//  - Keeps every ThemePicker on the page in sync (icon, label, active item).
// =============================================================================

const STORAGE_KEY = "theme";

function storedTheme() {
    return localStorage.getItem(STORAGE_KEY);
}

function preferredTheme() {
    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

function resolveTheme(value) {
    return value === "auto" ? preferredTheme() : value;
}

function applyTheme(value) {
    document.documentElement.setAttribute("data-bs-theme", resolveTheme(value));
}

function syncAllPickers(value) {
    document.querySelectorAll(".app-theme-picker").forEach(picker => {
        // Mark the active dropdown item, hide check on the others.
        picker.querySelectorAll("[data-bs-theme-value]").forEach(btn => {
            const isActive = btn.getAttribute("data-bs-theme-value") === value;
            btn.classList.toggle("active", isActive);
            const check = btn.querySelector("[data-theme-active]");
            if (check) {
                check.classList.toggle("opacity-0", !isActive);
            }
        });

        // Update the toggle button's icon + label.
        const icon = picker.querySelector("[data-theme-icon]");
        const label = picker.querySelector("[data-theme-label]");
        if (icon) {
            icon.className = "fs-5 bi " + (
                value === "dark" ? "bi-moon-stars-fill" :
                value === "light" ? "bi-sun-fill" :
                "bi-circle-half"
            );
        }
        if (label) {
            label.textContent =
                value === "dark" ? "Dark" :
                value === "light" ? "Light" :
                "Auto";
        }
    });
}

function setTheme(value) {
    localStorage.setItem(STORAGE_KEY, value);
    applyTheme(value);
    syncAllPickers(value);
}

// Re-resolve "auto" when the OS preference flips, but only if the user hasn't
// pinned a specific theme.
window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", () => {
    const value = storedTheme() || "auto";
    if (value === "auto") applyTheme("auto");
});

export default class extends BlazorJSComponents.Component {
    setParameters() {
        const value = storedTheme() || "auto";

        // Always re-apply on render so server-rendered chrome stays in sync.
        applyTheme(value);
        syncAllPickers(value);

        // Bind click handlers for every theme-picker dropdown item on the page.
        // setEventListener is idempotent across renders for this component
        // instance, and setTheme(...) is safe to invoke repeatedly with the
        // same value, so duplicate bindings from multiple pickers are harmless.
        document.querySelectorAll(".app-theme-picker [data-bs-theme-value]").forEach(btn => {
            this.setEventListener(btn, "click", () => {
                setTheme(btn.getAttribute("data-bs-theme-value"));
            });
        });
    }

    dispose() {
        // setEventListener registrations are cleaned up by the base class.
    }
}
