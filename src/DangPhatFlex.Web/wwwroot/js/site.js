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

// Hero image slideshow: crossfades through product photos, with clickable dots.
// Auto-rotation pauses on hover and is disabled when the user prefers reduced motion
// (dots still work for manual browsing). Degrades to a single static image without JS.
(function () {
    "use strict";

    var reduce = window.matchMedia &&
        window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    function initSlider(el) {
        var slides = Array.prototype.slice.call(el.querySelectorAll(".hero-slide"));
        if (slides.length < 2) {
            return;
        }

        var dotsWrap = el.querySelector(".hero-slider__dots");
        var interval = parseInt(el.getAttribute("data-interval"), 10) || 4000;
        var idx = 0;
        var timer = null;
        var dots = [];

        if (dotsWrap) {
            slides.forEach(function (_, i) {
                var b = document.createElement("button");
                b.type = "button";
                b.setAttribute("aria-label", "Ảnh " + (i + 1));
                if (i === 0) {
                    b.className = "is-active";
                }
                b.addEventListener("click", function () {
                    go(i);
                    restart();
                });
                dotsWrap.appendChild(b);
                dots.push(b);
            });
        }

        function go(n) {
            slides[idx].classList.remove("is-active");
            if (dots[idx]) {
                dots[idx].classList.remove("is-active");
            }
            idx = (n + slides.length) % slides.length;
            slides[idx].classList.add("is-active");
            if (dots[idx]) {
                dots[idx].classList.add("is-active");
            }
        }

        function start() {
            if (!reduce && timer === null) {
                timer = setInterval(function () { go(idx + 1); }, interval);
            }
        }

        function stop() {
            if (timer !== null) {
                clearInterval(timer);
                timer = null;
            }
        }

        function restart() {
            stop();
            start();
        }

        el.addEventListener("mouseenter", stop);
        el.addEventListener("mouseleave", start);
        start();
    }

    function boot() {
        Array.prototype.forEach.call(
            document.querySelectorAll(".hero-slider"),
            initSlider
        );
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();

// Zalo greeting bubble: pops up shortly after load to invite a chat. The floating
// button itself works with or without this; the bubble just adds a gentle nudge and
// stays dismissed for the rest of the browsing session once closed.
(function () {
    "use strict";

    function boot() {
        var pop = document.getElementById("zaloPop");
        var close = document.getElementById("zaloClose");
        if (!pop || !close) {
            return;
        }

        var dismissed = false;
        try {
            dismissed = window.sessionStorage.getItem("zaloPopClosed") === "1";
        } catch (e) { /* sessionStorage may be unavailable */ }

        if (!dismissed) {
            setTimeout(function () { pop.hidden = false; }, 2500);
        }

        close.addEventListener("click", function () {
            pop.hidden = true;
            try { window.sessionStorage.setItem("zaloPopClosed", "1"); } catch (e) { /* ignore */ }
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
