(function () {
    const override = localStorage.getItem("override-url");

    const script = document.createElement("script");
    script.type = "module";

    if (override) {
        console.log("[OVERRIDE] Using remote module:", override);
        script.src = override;
    } else {
        script.src = "/src/main.tsx";
    }

    document.head.appendChild(script);
})();
