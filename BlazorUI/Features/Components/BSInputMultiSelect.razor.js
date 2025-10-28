// Global guards to avoid duplicate listeners if the script is included multiple times
let __bsMs_onPointerDown;
let __bsMs_onKeyDown;

function isInsideMultiSelectDropdown(node) {
    const dropdown = node?.closest?.('.dropdown');
    if (!dropdown) return false;

    // Only treat it as ours if the dropdown has our toggle
    return !!dropdown.querySelector('.multi-select-toggle');
}

function closeAllMultiSelectDropdowns() {
    const toggles = document.querySelectorAll('.multi-select-toggle[aria-expanded="true"]');

    toggles.forEach(btn => {
        const dropdown = btn.closest('.dropdown');
        const menu = dropdown?.querySelector('.dropdown-menu');

        if (!dropdown || !menu) return;

        // Prefer Bootstrap API if present
        try {
            const bs = window.bootstrap;

            if (bs?.Dropdown) {
                const inst = bs.Dropdown.getInstance(btn) || new bs.Dropdown(btn);
                inst.hide();
            } else {
                // Fallback: manually close
                btn.setAttribute('aria-expanded', 'false');
                menu.classList.remove('show');
            }
        } catch {
            // Last-resort fallback
            btn.setAttribute('aria-expanded', 'false');
            menu.classList.remove('show');
        }
    });
}

export function onLoad() {
    if (__bsMs_onPointerDown || __bsMs_onKeyDown) {
        return; // already wired
    }

    // Use capture to run before other handlers; pointerdown is more reliable than click
    __bsMs_onPointerDown = (ev) => {
        const target = ev.target;
        if (!target) return;

        // Ignore interactions within any BSInputMultiSelect dropdown
        if (isInsideMultiSelectDropdown(target)) return;

        // Defer closing to end of event loop to avoid fighting Bootstrap's own handlers
        setTimeout(closeAllMultiSelectDropdowns, 0);
    };

    __bsMs_onKeyDown = (ev) => {
        if (ev.key === 'Escape' || ev.key === 'Esc') {
            setTimeout(closeAllMultiSelectDropdowns, 0);
        }
    };

    document.addEventListener('pointerdown', __bsMs_onPointerDown, true);
    document.addEventListener('keydown', __bsMs_onKeyDown, true);
}

export function onUpdate() {
    
}

export function onDispose() {
    if (__bsMs_onPointerDown) {
        document.removeEventListener('pointerdown', __bsMs_onPointerDown, true);
        __bsMs_onPointerDown = undefined;
    }

    if (__bsMs_onKeyDown) {
        document.removeEventListener('keydown', __bsMs_onKeyDown, true);
        __bsMs_onKeyDown = undefined;
    }
}