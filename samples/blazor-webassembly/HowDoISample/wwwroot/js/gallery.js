window.howDoI = {
    highlightSource: source => {
        const code = document.getElementById('razor-source');
        if (!code) return;
        code.textContent = source;
        if (window.Prism && Prism.languages.csharp) {
            code.innerHTML = Prism.highlight(source, Prism.languages.csharp, 'csharp');
        }
    },
    beginSplit: event => {
        const output = document.getElementById('output');
        const splitter = event.currentTarget;
        if (!output || event.button !== 0) return;
        event.preventDefault();
        const startX = event.clientX;
        const start = parseFloat(getComputedStyle(output).getPropertyValue('--code-width')) || 45;
        const width = output.getBoundingClientRect().width;
        splitter.setPointerCapture(event.pointerId);
        const move = e => output.style.setProperty('--code-width',
            Math.min(85, Math.max(15, start + (e.clientX - startX) / Math.max(1, width) * 100)) + '%');
        const stop = () => {
            splitter.removeEventListener('pointermove', move);
            splitter.removeEventListener('lostpointercapture', stop);
        };
        splitter.addEventListener('pointermove', move);
        splitter.addEventListener('lostpointercapture', stop);
    }
};
