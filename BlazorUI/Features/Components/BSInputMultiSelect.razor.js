// Global guards to avoid duplicate listeners if the script is included multiple times
let __bsMs_onPointerDown;
let __bsMs_onKeyDown;
let __bsMs_observer;

function isInsideMultiSelectDropdown(node) {
    const dropdown = node?.closest?.(".dropdown");
    if (!dropdown) return false;

    // Only treat it as ours if the dropdown has our toggle
    return !!dropdown.querySelector(".multi-select-toggle");
}

function closeAllMultiSelectDropdowns() {
    const toggles = document.querySelectorAll('.multi-select-toggle[aria-expanded="true"]');

    toggles.forEach(btn => {
        const dropdown = btn.closest(".dropdown");
        const menu = dropdown?.querySelector(".dropdown-menu");

        if (!dropdown || !menu) return;

        // Prefer Bootstrap API if present
        try {
            const bs = window.bootstrap;

            if (bs?.Dropdown) {
                const inst = bs.Dropdown.getInstance(btn) || new bs.Dropdown(btn);
                inst.hide();
            } else {
                // Fallback: manually close
                btn.setAttribute("aria-expanded", "false");
                menu.classList.remove("show");
            }
        } catch {
            // Last-resort fallback
            btn.setAttribute("aria-expanded", "false");
            menu.classList.remove("show");
        }
    });
}

function attachMultiSelectSizing(btn, optionsContainer, maxVisibleOptions) {
    const dropdown = btn.closest(".dropdown");
    const menu = dropdown?.querySelector(".dropdown-menu");
    if (!menu) return;

    const options = [...optionsContainer.querySelectorAll(".form-check")];
    if (options.length === 0) return;

    const numToMeasure = Math.min(maxVisibleOptions, options.length);

    let totalHeight = 0;
    for (let i = 0; i < numToMeasure; i++) {
        totalHeight += options[i].offsetHeight;

        if (i < numToMeasure - 1) {
            const style = window.getComputedStyle(options[i]);
            totalHeight += parseFloat(style.marginBottom) || 0;
        }
    }

    optionsContainer.style.maxHeight = `${totalHeight}px`;
    optionsContainer.style.overflowY = "auto";

    // Fix widths to prevent resizing during selections
    const initialWidth = btn.offsetWidth;
    btn.style.width = `${initialWidth}px`;
    menu.style.width = `${initialWidth}px`;
}

export default class extends BlazorJSComponents.Component {
    attach() {
        // Capture to run before other handlers; pointerdown is more reliable than click
        this._onPointerDown = (ev) => {
            const target = ev.target;
            if (!target) return;

            if (isInsideMultiSelectDropdown(target)) return;

            setTimeout(closeAllMultiSelectDropdowns, 0);
        };

        this._onKeyDown = (ev) => {
            if (ev.key === "Escape" || ev.key === "Esc") {
                setTimeout(closeAllMultiSelectDropdowns, 0);
            }
        };

        document.addEventListener("pointerdown", this._onPointerDown, true);
        document.addEventListener("keydown", this._onKeyDown, true);
    }

    setParameters(maxVisibleOptions, refs) {
        const { toggle, options } = refs ?? {};
        if (!toggle || !options) return;

        const maxVisible = parseInt(maxVisibleOptions) || 10;

        this.setEventListener(toggle, "shown.bs.dropdown", () => {
            attachMultiSelectSizing(toggle, options, maxVisible);
        });

        this.setEventListener(toggle, "hidden.bs.dropdown", () => {
            toggle.style.width = "";
        });
    }

    dispose() {
        if (this._onPointerDown) {
            document.removeEventListener("pointerdown", this._onPointerDown, true);
            this._onPointerDown = null;
        }

        if (this._onKeyDown) {
            document.removeEventListener("keydown", this._onKeyDown, true);
            this._onKeyDown = null;
        }
    }
}