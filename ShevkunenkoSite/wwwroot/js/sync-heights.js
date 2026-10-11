(function () {
    function syncHeight() {
        const central = document.getElementById('blockForAllOptions');
        if (!central) return;

        const row = central.closest('.row');
        const left = document.getElementById('picturesFromLeft');
        const right = document.getElementById('picturesFromRight');

        // Сбрасываем, чтобы измерить «честную» высоту центра
        [left, right].forEach(el => {
            if (el) {
                el.style.maxHeight = '';
                el.style.height = '';
            }
        });

        const h = central.offsetHeight + 'px';

        // Ряд не должен растягиваться под боковые колонки
        if (row) row.style.alignItems = 'flex-start';

        [left, right].forEach(el => {
            if (!el) return;
            el.style.height = h;
            el.style.maxHeight = h;
        });
    }

    function initSync() {
        const central = document.getElementById('blockForAllOptions');
        const left = document.getElementById('picturesFromLeft');
        const right = document.getElementById('picturesFromRight');
        if (!central) return;

        if (window.ResizeObserver) {
            const ro = new ResizeObserver(syncHeight);
            ro.observe(central);
            if (left) ro.observe(left);
            if (right) ro.observe(right);
        }

        [left, right].forEach(col => {
            if (!col) return;
            col.querySelectorAll('img').forEach(img => {
                if (!img.complete) {
                    img.addEventListener('load', syncHeight, { once: true });
                    img.addEventListener('error', syncHeight, { once: true });
                }
            });
        });

        if (document.readyState === 'complete') {
            syncHeight();
        } else {
            window.addEventListener('load', syncHeight);
        }

        setTimeout(syncHeight, 300);
        setTimeout(syncHeight, 1000);

        window.addEventListener('resize', syncHeight);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSync);
    } else {
        initSync();
    }
})();