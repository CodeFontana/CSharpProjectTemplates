// FluentLayoutHamburger attaches its click handler in OnAfterRender.
// Static SSR does not run that method, so this opens the drawer the component already rendered.
// In the combined layout the hamburger shows the sidebar as an overlay instead of the empty drawer.
// Capture phase is required because the button stops the click before it bubbles.
function setNavOpen(layout, open) {
    layout.dataset.navOpen = open ? "true" : "false";
    layout.querySelectorAll(".fluent-layout-hamburger").forEach((button) => {
        button.setAttribute("aria-expanded", open ? "true" : "false");
    });
}

function closeOpenNavs() {
    document.querySelectorAll(".combined-layout[data-nav-open='true']").forEach((layout) => {
        setNavOpen(layout, false);
    });
}

document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
        closeOpenNavs();
    }
});

document.addEventListener("click", (event) => {
    if (!(event.target instanceof Element)) {
        return;
    }

    const overlayLayout = event.target.closest(".combined-layout[data-nav-open='true']");
    if (overlayLayout && (event.target.closest("[data-nav-backdrop]") || event.target.closest(".fluent-layout-item[area='nav'] a"))) {
        setNavOpen(overlayLayout, false);
        return;
    }

    const drawerLink = event.target.closest("fluent-drawer a");
    if (drawerLink) {
        const openDrawer = drawerLink.closest("fluent-drawer");
        if (openDrawer && typeof openDrawer.hide === "function") {
            openDrawer.hide();
        }

        const owner = document.querySelector(".fluent-layout-hamburger[data-drawer-open='true']");
        if (owner) {
            owner.dataset.drawerOpen = "false";
            owner.setAttribute("aria-expanded", "false");
        }

        return;
    }

    const hamburger = event.target.closest(".fluent-layout-hamburger");
    if (!hamburger || !hamburger.id) {
        return;
    }

    // Fluent UI also wires this button for static SSR. Stop the click here so that handler does not toggle the drawer a second time.
    event.stopPropagation();

    const combinedLayout = hamburger.closest(".combined-layout");
    if (combinedLayout) {
        setNavOpen(combinedLayout, combinedLayout.dataset.navOpen !== "true");
        return;
    }

    const drawer = document.getElementById(`${hamburger.id}-drawer`);
    if (!drawer || typeof drawer.show !== "function" || typeof drawer.hide !== "function") {
        return;
    }

    if (hamburger.dataset.drawerOpen === "true") {
        drawer.hide();
        hamburger.dataset.drawerOpen = "false";
        hamburger.setAttribute("aria-expanded", "false");
    }
    else {
        drawer.show();
        hamburger.dataset.drawerOpen = "true";
        hamburger.setAttribute("aria-expanded", "true");
    }
}, true);