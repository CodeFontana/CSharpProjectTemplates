// FluentLayoutHamburger attaches its click handler in OnAfterRender.
// Static SSR does not run that method, so this opens the drawer the component already rendered.
// Capture phase is required because the button stops the click before it bubbles.
document.addEventListener("click", (event) => {
    if (!(event.target instanceof Element)) {
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
