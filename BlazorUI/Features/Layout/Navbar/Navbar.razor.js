function setTheme(theme) {
    document.documentElement.setAttribute("data-bs-theme", theme);
    localStorage.setItem("theme", theme);

    const themeSwitches = document.querySelectorAll(".theme-switch");
    themeSwitches.forEach(switchElement => {
        if (theme === "light") {
            switchElement.classList.replace("bi-sun-fill", "bi-moon-stars");
        } else {
            switchElement.classList.replace("bi-moon-stars", "bi-sun-fill");
        }
    });
}

export default class extends BlazorJSComponents.Component {
    setParameters() {
        // Ensure theme is applied
        const theme = localStorage.getItem("theme") || "light";
        setTheme(theme);

        // Add click listeners to theme switches
        const themeSwitches = document.querySelectorAll(".theme-switch");
        themeSwitches.forEach(switchElement => {
            this.setEventListener(switchElement, "click", () => {
                const currentTheme = localStorage.getItem("theme") || "light";
                const newTheme = currentTheme === "light" ? "dark" : "light";
                setTheme(newTheme);
            });
        });
    }

    dispose() {
        // setEventListener registrations are automatically cleaned up by the base class.
    }
}