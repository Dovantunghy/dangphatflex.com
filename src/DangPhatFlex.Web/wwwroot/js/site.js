// Progressive scroll-reveal: fades content up as it scrolls into view.
// Fully optional — if JS is disabled, IntersectionObserver is missing, or the user
// prefers reduced motion, nothing is hidden and the page renders normally.
(function () {
    "use strict";

    var prefersReduced = window.matchMedia &&
        window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    if (prefersReduced || !("IntersectionObserver" in window)) {
        return;
    }

    function init() {
        var selectors = [
            ".value-card",
            ".product-card",
            ".trust-bar__card",
            ".section-title",
            ".section-subtitle",
            ".cta-banner",
            ".contact-info-card",
            ".spec-table",
            ".news-card"
        ];

        var nodes = document.querySelectorAll(selectors.join(","));
        if (!nodes.length) {
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12, rootMargin: "0px 0px -40px 0px" });

        nodes.forEach(function (el, index) {
            el.classList.add("dpf-reveal");
            // Gentle cascade for items sharing a row (product/value grids).
            el.style.transitionDelay = (index % 4) * 70 + "ms";
            observer.observe(el);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
