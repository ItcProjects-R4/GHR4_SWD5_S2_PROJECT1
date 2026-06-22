/* Pathway MVC — global client JS */
(function () {
    'use strict';

    // ── Theme ─────────────────────────────────────────────────────────────────
    var THEME_KEY = 'pathway:theme';

    function applyTheme(t) {
        document.documentElement.setAttribute('data-theme', t);
        try { localStorage.setItem(THEME_KEY, t); } catch (_) { }
    }

    // Restore on load before paint
    var saved = 'light';
    try { saved = localStorage.getItem(THEME_KEY) || 'light'; } catch (_) { }
    applyTheme(saved);

    document.addEventListener('DOMContentLoaded', function () {

        // Theme toggle button
        var themeBtn = document.getElementById('themeBtn');
        if (themeBtn) {
            themeBtn.addEventListener('click', function () {
                var cur = document.documentElement.getAttribute('data-theme');
                applyTheme(cur === 'dark' ? 'light' : 'dark');
            });
        }

        // Mobile sidebar toggle
        var menuBtn = document.getElementById('menuBtn');
        var sidebar = document.getElementById('sidebar');
        if (menuBtn && sidebar) {
            menuBtn.addEventListener('click', function () {
                sidebar.classList.toggle('open');
            });
            document.addEventListener('click', function (e) {
                if (window.innerWidth <= 900 &&
                    sidebar.classList.contains('open') &&
                    !sidebar.contains(e.target) &&
                    e.target !== menuBtn) {
                    sidebar.classList.remove('open');
                }
            });
        }

        // Auto-hide flash messages after 4 s
        var flash = document.querySelectorAll('.flash');
        flash.forEach(function (el) {
            setTimeout(function () {
                el.style.transition = 'opacity .4s';
                el.style.opacity    = '0';
                setTimeout(function () { el.remove(); }, 500);
            }, 4000);
        });
    });

    // ── Progress slider helper (used in MyCourses / TrackCourses) ────────────
    window.updateProgress = function (enrollmentId, value) {
        var token = (document.querySelector('input[name="__RequestVerificationToken"]') || {}).value || '';
        fetch('/Student/UpdateProgress', {
            method:  'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
            body:    JSON.stringify({ enrollmentId: parseInt(enrollmentId), progress: parseInt(value) })
        }).catch(function (err) { console.warn('Progress update failed', err); });
    };

    // ── Opportunity filter (used in Opportunities view) ───────────────────────
    window.initOppFilter = function () {
        var currentFilter = 'All';

        window.setOppFilter = function (f, btn) {
            currentFilter = f;
            document.querySelectorAll('.opp-filter-btn').forEach(function (b) {
                b.classList.remove('btn-primary');
                b.classList.add('btn-outline');
            });
            btn.classList.remove('btn-outline');
            btn.classList.add('btn-primary');
            filterOpps();
        };

        function filterOpps() {
            var q = (document.getElementById('oppSearch') || {}).value || '';
            q = q.toLowerCase();
            document.querySelectorAll('.opp-card').forEach(function (card) {
                var cat  = (card.dataset.category || '').toLowerCase();
                var text = (card.dataset.text    || '').toLowerCase();
                var show = (currentFilter === 'All' || cat === currentFilter.toLowerCase())
                        && (q === '' || text.indexOf(q) !== -1);
                card.style.display = show ? '' : 'none';
            });
        }

        var searchBox = document.getElementById('oppSearch');
        if (searchBox) searchBox.addEventListener('input', filterOpps);
    };

}());
