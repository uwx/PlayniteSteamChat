// noinspection CssUnusedSymbol

(() => {
    function headCallback(head) {
        const style = document.createElement('style');
        // language=CSS
        style.append(`
            /* Hide Steam header */
            div[class^="main_SteamPageHeader_"] {
                display: none !important;
            }

            .singleWindowDivider {
                width: 1px;
                min-width: 1px;
                border-left: none;
            }
        `);

        head.append(style);
    }
    
    let head = document.head || document.getElementsByTagName('head')[0] || document.body;
    if (head) {
        headCallback(head);
    } else {
        window.addEventListener('DOMContentLoaded', () => {
            head = document.head || document.getElementsByTagName('head')[0] || document.body;
            if (head) {
                headCallback(head);
            } else {
                console.log('NO HEAD, NO BODY, AND PROBABLY NO DOCUMENT BECAUSE THIS GUY FEEDS ON LOADING ERRORS');
            }
        });
    }
})();