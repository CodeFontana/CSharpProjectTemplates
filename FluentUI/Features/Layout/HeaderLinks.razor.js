// details menus stay open until the summary is clicked again.
// Close them on an outside click, Escape, or a menu link, and keep only one open.
function closeMenus(event) {
    if (!(event.target instanceof Element)) {
        return;
    }

    const link = event.target.closest(".fluent-header-menu a");
    if (link) {
        link.closest(".fluent-header-menu")?.removeAttribute("open");
    }

    const menu = event.target.closest(".fluent-header-menu");
    document.querySelectorAll(".fluent-header-menu[open]").forEach((openMenu) => {
        if (openMenu !== menu) {
            openMenu.removeAttribute("open");
        }
    });
}

function closeOnEscape(event) {
    if (event.key !== "Escape") {
        return;
    }

    document.querySelectorAll(".fluent-header-menu[open]").forEach((menu) => {
        menu.removeAttribute("open");
    });
}

export default class extends BlazorJSComponents.Component {
    setParameters() {
        // Document listeners survive the header being replaced on enhanced navigation.
        // setEventListener replaces the previous registration for this instance.
        this.setEventListener(document, "click", closeMenus, true);
        this.setEventListener(document, "keydown", closeOnEscape);
    }
}
