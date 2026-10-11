const STORAGE_KEY = "fluentui-blazor:theme-settings";
const COLOR_NAME_KEY = "fluentui-blazor:theme-color-name";

function readStored() {
    try {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (!raw) {
            return {};
        }

        const parsed = JSON.parse(raw);
        return parsed && typeof parsed === "object" ? parsed : {};
    }
    catch {
        return {};
    }
}

function currentMode(stored) {
    if (stored.mode === "dark" || stored.mode === "light") {
        return stored.mode;
    }

    return "system";
}

function themeApi() {
    return globalThis.Blazor && globalThis.Blazor.theme;
}

function whenThemeReady(action) {
    const api = themeApi();
    if (api) {
        action(api);
        return;
    }

    let remaining = 40;
    const timer = setInterval(() => {
        const ready = themeApi();
        remaining -= 1;
        if (ready || remaining <= 0) {
            clearInterval(timer);
        }

        if (ready) {
            action(ready);
        }
        else if (remaining <= 0) {
            console.error("Blazor.theme is not available.");
        }
    }, 50);
}

function colorName(colors, stored) {
    if (!colors || stored.base !== "brand" || typeof stored.color !== "string") {
        return "Default";
    }

    const color = stored.color.toUpperCase();
    const named = localStorage.getItem(COLOR_NAME_KEY);
    if (named) {
        const namedButton = colors.querySelector(`[data-theme-color="${CSS.escape(named)}"]`);
        const namedHex = namedButton?.getAttribute("data-theme-hex") || "";
        if (namedHex.toUpperCase() === color) {
            return named;
        }
    }

    const match = [...colors.querySelectorAll("[data-theme-color]")].find((button) => {
        const hex = button.getAttribute("data-theme-hex") || "";
        return hex.toUpperCase() === color;
    });

    return match ? match.getAttribute("data-theme-color") : "Default";
}

function syncPicker(mode, colors) {
    const stored = readStored();
    if (mode) {
        mode.value = currentMode(stored);
    }

    if (!colors) {
        return;
    }

    const selected = colorName(colors, stored);
    colors.querySelectorAll("[data-theme-color]").forEach((button) => {
        const isSelected = button.getAttribute("data-theme-color") === selected;
        button.setAttribute("aria-selected", isSelected ? "true" : "false");
    });
}

function applyMode(mode) {
    whenThemeReady((api) => {
        api.setThemeMode(mode);
    });
}

function applyColor(button) {
    const name = button.getAttribute("data-theme-color") || "Default";
    const hex = button.getAttribute("data-theme-hex") || "";
    const mode = currentMode(readStored());
    localStorage.setItem(COLOR_NAME_KEY, name);

    whenThemeReady((api) => {
        if (hex.length === 0) {
            api.setWebTheme();
            api.setThemeMode(mode);
            return;
        }

        api.setBrandThemeFromSettings({
            color: hex,
            hueTorsion: 0,
            vibrancy: 0,
            mode: mode === "system" ? null : mode,
            isExact: false
        });
    });
}

let themeListener = false;
let activeMode = null;
let activeColors = null;

function ensureThemeListener() {
    if (themeListener) {
        return;
    }

    themeListener = true;
    document.body.addEventListener("themeChanged", () => {
        syncPicker(activeMode, activeColors);
    });
}

export default class extends BlazorJSComponents.Component {
    setParameters({ open, dialog, close, mode, colors }) {
        if (!open || !dialog || !close || !mode || !colors) {
            return;
        }

        activeMode = mode;
        activeColors = colors;
        syncPicker(mode, colors);
        ensureThemeListener();

        this.setEventListener(open, "click", () => {
            syncPicker(mode, colors);
            if (dialog.open) {
                dialog.close();
            }
            else {
                dialog.showModal();
            }
        });

        this.setEventListener(close, "click", () => {
            dialog.close();
        });

        this.setEventListener(dialog, "click", (event) => {
            if (event.target === dialog) {
                dialog.close();
            }
        });

        this.setEventListener(dialog, "toggle", () => {
            open.setAttribute("aria-expanded", dialog.open ? "true" : "false");
        });

        this.setEventListener(mode, "change", () => {
            applyMode(mode.value);
        });

        colors.querySelectorAll("[data-theme-color]").forEach((button) => {
            this.setEventListener(button, "click", () => {
                applyColor(button);
            });
        });
    }
}
