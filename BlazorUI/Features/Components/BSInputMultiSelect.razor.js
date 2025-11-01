// Global guards to avoid duplicate listeners if the script is included multiple times
let __bsMs_onPointerDown;
let __bsMs_onKeyDown;
let __bsMs_observer;

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

function attachMultiSelectListener(btn) {
    if (btn.dataset.multiSelectAttached) return;
    btn.dataset.multiSelectAttached = 'true';

    function onShown() {
        const dropdown = btn.closest('.dropdown');
        const menu = dropdown.querySelector('.dropdown-menu');
        const optionsContainer = menu.querySelector('.multi-select-options');
        if (!optionsContainer) return;

        const options = [...optionsContainer.querySelectorAll('.form-check')];
        if (options.length === 0) return;

        const maxVisible = parseInt(btn.dataset.maxVisible) || 10;
        let totalHeight = 0;
        const numToMeasure = Math.min(maxVisible, options.length);

        for (let i = 0; i < numToMeasure; i++) {
            totalHeight += options[i].offsetHeight;
            if (i < numToMeasure - 1) {
                const style = window.getComputedStyle(options[i]);
                totalHeight += parseFloat(style.marginBottom) || 0;
            }
        }

        optionsContainer.style.maxHeight = `${totalHeight}px`;
        optionsContainer.style.overflowY = 'auto';

        // Fix widths to prevent resizing during selections
        const initialWidth = btn.offsetWidth;
        btn.style.width = `${initialWidth}px`;
        menu.style.width = `${initialWidth}px`;
    }

    function onHidden() {
        btn.style.width = '';
    }

    btn.addEventListener('shown.bs.dropdown', onShown);
    btn.addEventListener('hidden.bs.dropdown', onHidden);
}

function initializeMultiSelects() {
    // Attach to existing toggles
    const toggles = document.querySelectorAll('.multi-select-toggle');
    toggles.forEach(attachMultiSelectListener);

    // Observe for new toggles
    __bsMs_observer = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === 1) { // Element node
                        if (node.matches('.multi-select-toggle')) {
                            attachMultiSelectListener(node);
                        }
                        node.querySelectorAll('.multi-select-toggle').forEach(attachMultiSelectListener);
                    }
                });
            }
        });
    });

    __bsMs_observer.observe(document.body, { childList: true, subtree: true });
}

export function onLoad() {
    initializeMultiSelects();

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
    if (__bsMs_observer) {
        __bsMs_observer.disconnect();
        __bsMs_observer = null;
    }

    if (__bsMs_onPointerDown) {
        document.removeEventListener('pointerdown', __bsMs_onPointerDown, true);
        __bsMs_onPointerDown = undefined;
    }

    if (__bsMs_onKeyDown) {
        document.removeEventListener('keydown', __bsMs_onKeyDown, true);
        __bsMs_onKeyDown = undefined;
    }
}