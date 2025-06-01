(function() {
    'use strict';
    
    console.log("AniStream: identify-shows.js loaded");

    function addIdentifyButton(view, params) {
        console.log("AniStream: addIdentifyButton called", params);
        
        if (!params || !params.item || params.item.CollectionType !== "tvshows") {
            return;
        }

        const item = params.item;
        
        // Check if button already exists
        if (view.querySelector('.anistream-identify-btn')) {
            return;
        }

        const btn = document.createElement("button");
        btn.className = "raised button-submit block anistream-identify-btn";
        btn.textContent = "Identify Anime";
        btn.style.marginLeft = "10px";

        btn.addEventListener("click", () => {
            // Use Jellyfin's built-in dialog system
            require(['dialogHelper'], function(dialogHelper) {
                const dlg = dialogHelper.createDialog({
                    removeOnClose: true,
                    size: 'small'
                });

                dlg.innerHTML = `
                    <div class="formDialogHeader">
                        <button is="paper-icon-button-light" class="btnCancel autoSize" tabindex="-1">
                            <span class="material-icons arrow_back"></span>
                        </button>
                        <h3 class="formDialogHeaderTitle">Identify Anime</h3>
                    </div>
                    <div class="formDialogContent smoothScrollY">
                        <div class="dialogContentInner dialog-content-centered">
                            <p>Are you sure you want to identify this anime collection?</p>
                            <p><strong>${item.Name}</strong></p>
                        </div>
                    </div>
                    <div class="formDialogFooter">
                        <button is="emby-button" type="button" class="btnCancel">Cancel</button>
                        <button is="emby-button" type="button" class="raised button-submit btnIdentify">Identify</button>
                    </div>
                `;

                // Add event listeners for dialog buttons
                dlg.querySelector('.btnCancel').addEventListener('click', () => {
                    dialogHelper.close(dlg);
                });

                dlg.querySelector('.btnIdentify').addEventListener('click', () => {
                    console.log('Identifying anime for item:', item);
                    // TODO: Add your identification logic here
                    dialogHelper.close(dlg);
                });

                dialogHelper.open(dlg);
            });
        });

        const container = view.querySelector(".headerRight, .page-header .headerRight, .detailPageHeaderRight");
        if (container) {
            container.appendChild(btn);
            console.log("AniStream: Button added to container");
        } else {
            console.log("AniStream: No suitable container found for button");
        }
    }

    // Hook into Jellyfin's page navigation
    function initializePageHook() {
        if (window.Emby && window.Emby.Page) {
            const originalShow = window.Emby.Page.show;
            window.Emby.Page.show = function(page, params) {
                const result = originalShow.apply(this, arguments);
                setTimeout(() => addIdentifyButton(page, params), 100);
                return result;
            };
        }
        
        // Also try hooking into the router if available
        if (window.Emby && window.Emby.Router) {
            document.addEventListener('viewshow', function(e) {
                setTimeout(() => addIdentifyButton(e.detail.view, e.detail.options), 100);
            });
        }
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializePageHook);
    } else {
        initializePageHook();
    }

    console.log("AniStream: Script initialization complete");
})();
