// =============================================================================
//  SidebarLayout — re-initializes Bootstrap offcanvas after each render.
// =============================================================================
//  Theme handling lives in ThemePicker.razor.js.
// =============================================================================

function reinitializeOffcanvas() {
    const offcanvasElementList = Array.from(document.querySelectorAll(".offcanvas"));

    offcanvasElementList.forEach(offcanvasEl => {
        const existingInstance = bootstrap.Offcanvas.getInstance(offcanvasEl);
        if (existingInstance) {
            existingInstance.dispose();
        }
        new bootstrap.Offcanvas(offcanvasEl);
    });
}

export default class extends BlazorJSComponents.Component {
    setParameters() {
        reinitializeOffcanvas();
    }
}
