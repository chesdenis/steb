// Client helpers for the STEB web app.
window.steb = {
    // Wires a drawing area to its .NET usgModelFrame view. Mouse events are sent in order;
    // consecutive "move" events are coalesced while the server is busy rendering.
    attachFrame(el, dotnet) {
        const queue = [];
        let busy = false;
        const pump = () => {
            if (busy || queue.length === 0) return;
            busy = true;
            const [kind, x, y, d] = queue.shift();
            dotnet.invokeMethodAsync('OnMouse', kind, x, y, d)
                .then(svg => { el.innerHTML = svg; })
                .catch(() => { })
                .finally(() => { busy = false; pump(); });
        };
        const send = (kind, x, y, d) => {
            if (kind === 'move' && queue.length && queue[queue.length - 1][0] === 'move') queue[queue.length - 1] = [kind, x, y, d];
            else queue.push([kind, x, y, d]);
            pump();
        };
        const pos = e => { const r = el.getBoundingClientRect(); return [Math.round(e.clientX - r.left), Math.round(e.clientY - r.top)]; };
        const h = {
            down: e => { if (e.button !== 0) return; e.preventDefault(); const [x, y] = pos(e); send('down', x, y, 0); },
            move: e => { const [x, y] = pos(e); send('move', x, y, 0); },
            up: e => { const [x, y] = pos(e); send('up', x, y, 0); },
            enter: () => send('enter', 0, 0, 0),
            leave: () => send('leave', 0, 0, 0),
            wheel: e => { e.preventDefault(); const [x, y] = pos(e); send('wheel', x, y, e.deltaY < 0 ? 120 : -120); }
        };
        el.addEventListener('mousedown', h.down);
        el.addEventListener('mousemove', h.move);
        el.addEventListener('mouseup', h.up);
        el.addEventListener('mouseenter', h.enter);
        el.addEventListener('mouseleave', h.leave);
        el.addEventListener('wheel', h.wheel, { passive: false });
        let t = null;
        const ro = new ResizeObserver(() => {
            clearTimeout(t);
            t = setTimeout(() => dotnet.invokeMethodAsync('OnResize', el.clientWidth, el.clientHeight).then(svg => { el.innerHTML = svg; }).catch(() => { }), 100);
        });
        ro.observe(el);
        el._steb = { h, ro };
        return [el.clientWidth, el.clientHeight];
    },
    detachFrame(el) {
        if (!el || !el._steb) return;
        const { h, ro } = el._steb;
        el.removeEventListener('mousedown', h.down);
        el.removeEventListener('mousemove', h.move);
        el.removeEventListener('mouseup', h.up);
        el.removeEventListener('mouseenter', h.enter);
        el.removeEventListener('mouseleave', h.leave);
        el.removeEventListener('wheel', h.wheel);
        ro.disconnect();
        delete el._steb;
    },
    setSvg(el, svg) { if (el) el.innerHTML = svg; },
    printFrame(frame) { try { frame.contentWindow.focus(); frame.contentWindow.print(); } catch (e) { } },
    download(name, mime, text) {
        const blob = new Blob([text], { type: mime });
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = name;
        document.body.appendChild(a);
        a.click();
        a.remove();
        setTimeout(() => URL.revokeObjectURL(a.href), 1000);
    }
};

// Same key filter as usgNumberField_KeyPress: digits, ',', '.', '-' and control keys.
window.stebNumericKey = function (e) {
    return e.key.length > 1 || /[0-9.,\-]/.test(e.key);
};

// Enter in a number field: commit the value first (fires "change"), so the server sees
// the new text before the Enter key handler runs - as in WinForms, where Text was already set.
document.addEventListener('keydown', function (e) {
    const el = e.target;
    if (e.key === 'Enter' && el && el.classList && el.classList.contains('usg-number')) {
        el.blur();
        el.focus();
    }
}, true);
