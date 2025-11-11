export function setupDevtools() {
    const enabled = localStorage.getItem("devtools") === "true";
    if (!enabled) return;

    const btn = document.createElement("div");
    btn.textContent = "{...}";
    btn.style.position = "fixed";
    btn.style.bottom = "10px";
    btn.style.right = "10px";
    btn.style.padding = "6px 10px";
    btn.style.background = "rgba(0,0,0,0.6)";
    btn.style.color = "white";
    btn.style.borderRadius = "4px";
    btn.style.cursor = "pointer";
    btn.style.zIndex = "9999";

    btn.onclick = () => {
        const url = prompt("URL override:", "");

        if (url) {
            localStorage.setItem("override-url", url);
            location.reload();
        }
    };

    document.body.appendChild(btn);
}
