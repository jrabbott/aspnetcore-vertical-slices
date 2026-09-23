/**
 * Shared progressive enhancement — HTML forms work without this file.
 * When fetch is available, favourite command forms may post JSON instead of a full redirect.
 */
(function () {
    "use strict";

    if (!("fetch" in window) || !("FormData" in window)) {
        return;
    }

    /**
     * @param {ParentNode} [root]
     */
    function enhanceFavouriteCommands(root) {
        const scope = root || document;
        scope.querySelectorAll('form[data-enhance="favourite-command"]').forEach(function (form) {
            if (form.dataset.peBound === "true") {
                return;
            }

            form.dataset.peBound = "true";
            form.addEventListener("submit", onFavouriteSubmit);
        });
    }

    /**
     * @param {SubmitEvent} event
     */
    async function onFavouriteSubmit(event) {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        event.preventDefault();

        const submitter = event.submitter instanceof HTMLButtonElement ? event.submitter : null;
        setBusy(form, submitter, true);

        try {
            const response = await fetch(form.action, {
                method: (form.method || "post").toUpperCase(),
                body: new FormData(form),
                headers: {
                    Accept: "application/json",
                    "X-Requested-With": "fetch"
                },
                credentials: "same-origin"
            });

            if (!response.ok) {
                throw new Error("Favourite command failed with status " + response.status);
            }

            const result = await response.json();
            handleFavouriteResult(form, result);
        } catch {
            // Enhancement failed — fall back to a classic full-page submit.
            form.removeEventListener("submit", onFavouriteSubmit);
            form.submit();
        } finally {
            setBusy(form, submitter, false);
        }
    }

    /**
     * @param {HTMLFormElement} form
     * @param {{ succeeded?: boolean, Succeeded?: boolean, message?: string, Message?: string }} result
     */
    function handleFavouriteResult(form, result) {
        const succeeded = result.succeeded === true || result.Succeeded === true;
        const message = result.message || result.Message || "";
        const onSuccess = form.getAttribute("data-on-success") || "reload";

        showStatus(form, message, !succeeded);

        if (!succeeded) {
            return;
        }

        if (onSuccess === "remove-row") {
            const row = form.closest("li");
            if (row) {
                row.remove();
            }

            ensureEmptyFavouritesState();
            return;
        }

        if (onSuccess === "flash") {
            return;
        }

        window.location.reload();
    }

    /**
     * @param {HTMLFormElement} form
     * @param {string} message
     * @param {boolean} isError
     */
    function showStatus(form, message, isError) {
        if (!message) {
            return;
        }

        const page = form.closest(".page") || document.querySelector(".page");
        if (!page) {
            return;
        }

        let alert = page.querySelector("[data-pe-status]");
        if (!alert) {
            alert = document.createElement("div");
            alert.setAttribute("data-pe-status", "true");
            alert.setAttribute("role", "status");
            const header = page.querySelector(".page-header");
            if (header && header.nextSibling) {
                page.insertBefore(alert, header.nextSibling);
            } else {
                page.prepend(alert);
            }
        }

        alert.className = "alert " + (isError ? "alert-error" : "alert-success");
        alert.textContent = message;
    }

    function ensureEmptyFavouritesState() {
        const list = document.querySelector("ul.favourites-list");
        if (!list || list.children.length > 0) {
            return;
        }

        list.remove();

        const page = document.querySelector(".page");
        if (!page || page.querySelector("p.empty")) {
            return;
        }

        const empty = document.createElement("p");
        empty.className = "empty";
        empty.textContent = "You have no favourite cities yet. Add one below.";

        const addSection = page.querySelector(".add-favourite");
        if (addSection) {
            page.insertBefore(empty, addSection);
        } else {
            page.appendChild(empty);
        }
    }

    /**
     * @param {HTMLFormElement} form
     * @param {HTMLButtonElement | null} submitter
     * @param {boolean} busy
     */
    function setBusy(form, submitter, busy) {
        form.setAttribute("aria-busy", busy ? "true" : "false");
        form.querySelectorAll("button").forEach(function (button) {
            button.disabled = busy;
        });

        if (submitter) {
            submitter.disabled = busy;
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        enhanceFavouriteCommands(document);
    });

    window.WeatherApp = window.WeatherApp || {};
    window.WeatherApp.enhanceFavouriteCommands = enhanceFavouriteCommands;
})();
