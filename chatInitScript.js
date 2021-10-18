// noinspection CssUnusedSymbol

(() => {
    const head = document.head || document.getElementsByTagName('head') || document.body;
    if (head) {
        const style = document.createElement('style');
        // language=CSS
        style.append(`
            /* Hide Steam header */
            div[class^="main_SteamPageHeader_"] {
                display: none !important;
            }

            body {
                margin-top: 28px !important;
                background: transparent !important;
            }

            .friendListHeaderContainer {
                background-color: transparent !important;
            }

            .statusHeaderGlow {
                margin-top: -48px;
                height: calc(132px + 48px);
                width: calc(100% + 32px);
            }

            .singleWindowDivider {
                margin-top: -48px;
                width: 1px;
                min-width: 1px;
                border-left: none;
            }

            .titleBarContainer.ChatTabs {
                background-color: transparent;
            }

            .chatRoomListContainer {
                background: radial-gradient(ellipse farthest-corner at 20% 30%, #222b35 0%, transparent 10%, transparent 100%);
            }

            .friendlistListContainer {
                background: radial-gradient(ellipse farthest-corner at 50% 30%, #212329 0%, transparent 50%, transparent 100%);
            }
        `);
        head.append(style);
    }
})();