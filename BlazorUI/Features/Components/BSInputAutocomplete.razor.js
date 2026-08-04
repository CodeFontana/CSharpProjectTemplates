const HIDDEN = "d-none";
const HIGHLIGHT = "active";
const OPEN = "show";

export default class extends BlazorJSComponents.Component {
    attach() {
        this.highlight = -1;
    }

    setParameters(refs) {
        const { root, input, toggle, menu } = refs ?? {};
        if (!root || !input || !toggle || !menu) {
            return;
        }

        this.input = input;
        this.menu = menu;

        // setEventListener replaces whatever it registered previously for the same element and event,
        // so re-running on every parent render cannot stack up duplicates.
        this.setEventListener(input, "input", () => {
            this.applyFilter();
            this.open();
        });

        this.setEventListener(input, "keydown", (ev) => this.onKeyDown(ev));

        // Suppressing the mousedown default keeps focus on the input, so focusout does not close the
        // menu before the click that follows is delivered.
        this.setEventListener(toggle, "mousedown", (ev) => ev.preventDefault());
        this.setEventListener(toggle, "click", () => {
            if (this.isOpen()) {
                this.close();
                return;
            }

            // The chevron means "show me everything", so it ignores whatever is already typed. Pulling
            // focus in also means clicking away raises focusout here and closes the menu again.
            input.focus();
            this.showAll();
            this.open();
        });

        this.setEventListener(menu, "mousedown", (ev) => {
            // Narrower than suppressing the whole menu, which would also break dragging its scrollbar.
            if (ev.target.closest("[data-value]")) {
                ev.preventDefault();
            }
        });

        this.setEventListener(menu, "click", (ev) => {
            const item = ev.target.closest("[data-value]");
            if (item) {
                this.select(item);
            }
        });

        // relatedTarget is where focus is heading, so moving between the input and its own menu does
        // not read as leaving the control. It is null when focus lands somewhere unfocusable, which
        // is how a click on empty page background closes the menu.
        this.setEventListener(root, "focusout", (ev) => {
            if (!root.contains(ev.relatedTarget)) {
                this.close();
            }
        });

        // The options may have just been re-rendered underneath us, in which case they came back
        // unfiltered and without a highlight.
        this.applyFilter();
    }

    items() {
        return [...this.menu.querySelectorAll("[data-value]")];
    }

    visibleItems() {
        return this.items().filter(item => !item.classList.contains(HIDDEN));
    }

    isOpen() {
        return this.menu.classList.contains(OPEN);
    }

    open() {
        if (this.visibleItems().length === 0) {
            // Nothing to offer is the same as closed, so typing a value that matches nothing leaves
            // the menu out of the way instead of hanging an empty box under the input.
            this.close();
            return;
        }

        this.menu.classList.add(OPEN);
        this.input.setAttribute("aria-expanded", "true");
    }

    close() {
        this.menu.classList.remove(OPEN);
        this.input.setAttribute("aria-expanded", "false");
        this.setHighlight(-1);
    }

    showAll() {
        for (const item of this.items()) {
            item.classList.remove(HIDDEN);
        }

        this.setHighlight(-1);
    }

    applyFilter() {
        const query = this.input.value.trim().toLowerCase();

        for (const item of this.items()) {
            const value = (item.dataset.value ?? "").toLowerCase();
            const label = (item.textContent ?? "").trim().toLowerCase();

            // Matching the label as well as the value is what lets someone find VDM001197 by typing
            // "Optum", which is the point of annotating the options.
            const matches = query === "" || value.includes(query) || label.includes(query);
            item.classList.toggle(HIDDEN, !matches);

            // Blazor renders this too, from the same comparison. Repeating it here keeps it honest
            // between renders, and under static rendering there is no later render to fix it up.
            item.setAttribute("aria-selected", value === query ? "true" : "false");
        }

        this.setHighlight(-1);
    }

    setHighlight(index) {
        for (const item of this.items()) {
            item.classList.remove(HIGHLIGHT);
        }

        this.input.removeAttribute("aria-activedescendant");
        this.highlight = index;

        const item = index >= 0 ? this.visibleItems()[index] : null;
        if (!item) {
            return;
        }

        item.classList.add(HIGHLIGHT);
        this.input.setAttribute("aria-activedescendant", item.id);
        item.scrollIntoView({ block: "nearest" });
    }

    moveHighlight(delta) {
        const count = this.visibleItems().length;
        if (count === 0) {
            return;
        }

        const next = this.highlight < 0
            ? (delta > 0 ? 0 : count - 1)
            : (this.highlight + delta + count) % count;

        this.setHighlight(next);
    }

    select(item) {
        this.input.value = item.dataset.value ?? "";
        this.close();
        this.applyFilter();

        // Assigning value in script raises no event of its own, and a Blazor binding is only listening
        // for the real thing. Under static rendering nobody is listening and the input simply holds
        // the new value.
        this.input.dispatchEvent(new Event("change", { bubbles: true }));
    }

    onKeyDown(ev) {
        switch (ev.key) {
            case "ArrowDown":
                // Otherwise the caret jumps to the end of the text as the highlight moves.
                ev.preventDefault();
                if (!this.isOpen()) {
                    this.open();
                }

                this.moveHighlight(1);
                break;

            case "ArrowUp":
                ev.preventDefault();
                this.moveHighlight(-1);
                break;

            case "Enter":
                if (this.isOpen() && this.highlight >= 0) {
                    // Only swallowed when it actually picks something, so Enter still submits a
                    // surrounding form when the menu is closed.
                    ev.preventDefault();
                    this.select(this.visibleItems()[this.highlight]);
                } else {
                    this.close();
                }

                break;

            case "Escape":
                this.close();
                break;
        }
    }
}
