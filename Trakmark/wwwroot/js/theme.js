globalThis.setTheme = (theme) => {
    document.querySelector("[data-cf-theme]").dataset.cfTheme = theme;
};