(function () {
    const override = localStorage.getItem("override-url");
    if (override) {
        const script = document.createElement("script");
        script.type = "module";
        script.src = override;

        document.head.appendChild(script);

        console.log("[OVERRIDE] Using remote module:", override);
    }
})();
