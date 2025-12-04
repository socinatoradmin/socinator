(function () {
    if (window.__mediaDownloaderInjected) return;
    window.__mediaDownloaderInjected = true;

    const added = new WeakMap(); // Track processed <a> tags

    /* --------------------------------------------------------
       Create and attach button inside <a>
    -------------------------------------------------------- */
    function attachButton(aTag) {
        if (added.has(aTag)) return;
        added.set(aTag, true);

        // Make container positionable
        const style = getComputedStyle(aTag);
        if (style.position === "static") {
            aTag.style.position = "relative";
        }

        // Create button element
        const btn = document.createElement("button");
        btn.innerHTML = "⭳";
        btn.className = "media-container-download-btn";

        Object.assign(btn.style, {
            position: "absolute",
            bottom: "6px",
            right: "6px",
            padding: "5px 8px",
            fontSize: "13px",
            background: "#007bff",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: "pointer",
            zIndex: "9999",
            pointerEvents: "auto"
        });

        btn.onclick = (e) => {
            e.stopPropagation();
            e.preventDefault();

            const url = aTag.href || aTag.src;
            if (!url) {
                showToast("This link has no URL");
                return;
            }

            const clean = url.split("?")[0].split("#")[0];
            const ext = clean.includes(".") ? clean.split(".").pop().toLowerCase() : null;

            showToast("Downloading...");
            window.hostBridge.__saveMediaToHost(url, ext);
        };

        // Add button inside the <a> element
        aTag.appendChild(btn);
    }

    /* --------------------------------------------------------
       Toast popup
    -------------------------------------------------------- */
    function showToast(msg) {
        let box = document.getElementById("mediaToastBox");
        if (!box) {
            box = document.createElement("div");
            box.id = "mediaToastBox";

            Object.assign(box.style, {
                position: "fixed",
                bottom: "20px",
                right: "20px",
                padding: "10px 15px",
                background: "#000",
                color: "white",
                borderRadius: "8px",
                zIndex: "9999999",
                opacity: "0",
                transition: "opacity .3s"
            });

            document.body.appendChild(box);
        }

        box.textContent = msg;
        box.style.opacity = "1";
        setTimeout(() => box.style.opacity = "0", 2000);
    }

    window.__downloadCompleteToast = function (path) {
        showToast("Downloaded: " + path);
    };
    window.__downloadFailedToast = function (errorText) {
        showToast("Failed to download due to: " + errorText);
    };
    /* --------------------------------------------------------
       Scan all <a> tags and attach buttons
    -------------------------------------------------------- */
    function scan() {
        document.querySelectorAll("a").forEach(attachButton);
        document.querySelectorAll("img").forEach(attachButton);
    }

    /* Run on load */
    scan();

    /* Re-scan on DOM changes */
    new MutationObserver(scan).observe(document.body, {
        childList: true,
        subtree: true
    });
})();