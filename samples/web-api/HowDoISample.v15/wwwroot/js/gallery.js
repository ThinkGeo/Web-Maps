// The gallery's shell: the sidebar from menus.json, the sample in a frame, and its two sources
// beside it. A sample is addressed as #/<id>.
(async () => {
    const menus = await (await fetch('menus.json')).json();
    const menuBar = document.getElementById('menu-bar');
    const frame = document.getElementById('result-frame');
    const output = document.getElementById('output');
    const codeBox = document.getElementById('razor-box');
    const splitter = document.getElementById('splitter');
    const codeText = document.getElementById('code-text');
    const codeFile = document.getElementById('code-file');
    const tabs = { client: document.getElementById('tab-client'), server: document.getElementById('tab-server') };
    const open = new Set();
    let current = null;
    let sources = null;
    let shown = null;   // 'client' | 'server' | null
    let filter = '';

    const idFromHash = () => decodeURIComponent(location.hash.replace(/^#\/?/, ''));
    const menuOf = (id) => menus.find(m => m.id === id) || menus[0];
    const terms = () => filter.split(' ').filter(Boolean);
    const has = (text, term) => !!text && text.toLowerCase().includes(term.toLowerCase());
    const matches = (m) => terms().every(t => has(m.title, t) || has(m.description, t) || has(m.group, t));

    function renderMenu() {
        const shownMenus = terms().length ? menus.filter(matches) : menus;
        menuBar.innerHTML = '';
        const search = document.createElement('li');
        search.className = 'sidebar-search';
        search.innerHTML = `<input id="sidebar-search" type="search" placeholder="Search samples">` + (terms().length ? `<span class="hits">${shownMenus.length} of ${menus.length}</span>` : '');
        menuBar.appendChild(search);
        const input = search.querySelector('input');
        input.value = filter;
        input.addEventListener('input', () => { filter = input.value; const at = input.selectionStart; renderMenu(); const again = document.getElementById('sidebar-search'); again.focus(); again.setSelectionRange(at, at); });

        const groups = [];
        for (const m of shownMenus) if (m.group && !groups.includes(m.group)) groups.push(m.group);
        for (const group of groups) {
            const isOpen = open.has(group) || terms().length > 0;
            const li = document.createElement('li');
            li.className = 'menu-group' + (isOpen ? ' open' : '');
            li.innerHTML = `<a class="menu-group-title"><span class="sub-arrow ${isOpen ? 'fas fa-chevron-down' : 'fas fa-chevron-right'}">${group}</span></a>`;
            li.querySelector('a').addEventListener('click', () => { if (!open.delete(group)) open.add(group); renderMenu(); });
            menuBar.appendChild(li);
            if (!isOpen) continue;
            for (const m of shownMenus.filter(x => x.group === group)) {
                const item = document.createElement('li');
                item.className = 'menu-group-item' + (current && current.id === m.id ? ' active' : '');
                item.innerHTML = `<a href="#/${m.id}"><span class="sub-arrow ${m.preicon}">${m.title}</span></a>`;
                menuBar.appendChild(item);
            }
        }
    }

    async function show(id) {
        const m = menuOf(id);
        if (!m) return;
        current = m;
        if (m.group) open.add(m.group);
        document.getElementById('sample-title').textContent = m.title;
        document.getElementById('sample-description').textContent = m.description;
        document.title = m.title + ' - ThinkGeo Web Maps for WebAPI Samples';
        frame.src = `samples/${m.id}.html`;
        sources = null;
        tabs.server.parentElement.hidden = !m.source;
        if (shown === 'server' && !m.source) shown = 'client';
        if (shown) await showCode(shown);
        renderMenu();
    }

    async function showCode(which) {
        if (!sources) sources = await (await fetch(`source/${current.id}`)).json();
        shown = which;
        codeText.textContent = which === 'client' ? sources.client : sources.server;
        codeFile.textContent = which === 'client' ? `samples/${current.id}.html` : `Samples/${sources.serverFile}`;
        codeBox.hidden = false; splitter.hidden = false;
        output.classList.add('split');
        for (const key in tabs) tabs[key].classList.toggle('active', key === which);
    }

    function hideCode() {
        shown = null;
        codeBox.hidden = true; splitter.hidden = true;
        output.classList.remove('split');
        for (const key in tabs) tabs[key].classList.remove('active');
    }

    for (const key in tabs) {
        tabs[key].addEventListener('click', (e) => { e.preventDefault(); shown === key ? hideCode() : showCode(key); });
    }

    // The divider between the code and the sample is dragged; the code's share is a percentage.
    let dragging = null;
    splitter.addEventListener('pointerdown', (e) => { dragging = { x: e.clientX, width: output.getBoundingClientRect().width, start: parseFloat(getComputedStyle(output).getPropertyValue('--code-width')) || 45 }; splitter.setPointerCapture(e.pointerId); });
    splitter.addEventListener('pointermove', (e) => { if (!dragging) return; const pct = Math.min(85, Math.max(15, dragging.start + (e.clientX - dragging.x) / dragging.width * 100)); output.style.setProperty('--code-width', pct + '%'); });
    splitter.addEventListener('pointerup', () => { dragging = null; });

    document.getElementById('menu-toggle').addEventListener('click', () => document.getElementById('sidebar').classList.toggle('collapse'));
    window.addEventListener('hashchange', () => show(idFromHash()));
    show(idFromHash());
})();
