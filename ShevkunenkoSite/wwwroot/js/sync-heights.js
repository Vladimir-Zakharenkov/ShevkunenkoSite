(function () {
    function syncHeight() {
        const central = document.getElementById('blockForAllOptions');
        const left = document.getElementById('picturesFromLeft');
        const right = document.getElementById('picturesFromRight');

        if (!central) return;

        const maxHeight = central.offsetHeight * 0.94 + 'px';
        if (left) left.style.maxHeight = maxHeight;
        if (right) right.style.maxHeight = maxHeight;
    }

    document.addEventListener('DOMContentLoaded', function () {
        const central = document.getElementById('blockForAllOptions');
        if (!central) return;

        // Следим за изменением размера центральной колонки
        if (window.ResizeObserver) {
            const observer = new ResizeObserver(syncHeight);
            observer.observe(central);
        }

        // Первичный вызов
        syncHeight();
        setTimeout(syncHeight, 200);
        setTimeout(syncHeight, 500);
    });

    window.addEventListener('load', syncHeight);
    window.addEventListener('resize', syncHeight);
})();