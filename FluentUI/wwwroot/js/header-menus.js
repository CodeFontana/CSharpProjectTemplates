// details menus stay open until the summary is clicked again.
// Close them on an outside click, Escape, or a menu link, and keep only one open.
document.addEventListener("click", (event) => {
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
}, true);

document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
        return;
    }

    document.querySelectorAll(".fluent-header-menu[open]").forEach((menu) => {
        menu.removeAttribute("open");
    });
});
