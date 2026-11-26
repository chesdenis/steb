// 3D hull view (web addition, display only).
// World: X along the length, Y across, Z up. Projection: rotation matrix R = Rx(pitch) * Rz(yaw),
// then u = x', v = z', depth = y'. Screen = view matrix [s 0 tx; 0 -s ty] applied to (u, v), so
// zooming keeps the point under the cursor fixed (tx' = cx - (cx - tx) * k) and panning shifts tx, ty.
window.steb = window.steb || {};
window.steb.hull3d = (function () {
    const DEG = Math.PI / 180;
    const VIEWS = {
        iso: { yaw: -35, pitch: 25 },
        body: { yaw: -90, pitch: 0 },     // looking along X: Y horizontal, Z vertical
        profile: { yaw: 0, pitch: 0 },    // side view: X horizontal, Z vertical
        plan: { yaw: 0, pitch: 90 }       // top view: X horizontal, Y vertical
    };
    const COLORS = { station: '#2b55c9', waterline: '#2e8b57', sheer: '#1f2a37', keel: '#1f2a37', dwl: '#d0312d', axis: '#8a94a3' };
    // shaded solids: base colour [r, g, b] and opacity
    const SOLID = {
        compartment: { rgb: [242, 163, 58], alpha: 0.30, edge: 'rgba(160,95,10,0.35)' },
        tank: { rgb: [142, 107, 214], alpha: 0.30, edge: 'rgba(90,60,160,0.35)' },
        cargo: { rgb: [208, 49, 45], alpha: 0.75, edge: 'rgba(120,20,20,0.6)' },
        liquid: { rgb: [30, 136, 229], alpha: 0.70, edge: 'rgba(20,80,160,0.25)' }
    };
    // light comes from the viewer, slightly from the upper left (camera coordinates: u, depth, v)
    const LIGHT = (function () { const l = [-0.35, -1, 0.55], n = Math.hypot(l[0], l[1], l[2]); return [l[0] / n, l[1] / n, l[2] / n]; })();

    function rotation(yaw, pitch) {
        const cy = Math.cos(yaw * DEG), sy = Math.sin(yaw * DEG), cp = Math.cos(pitch * DEG), sp = Math.sin(pitch * DEG);
        // Rz(yaw) then Rx(pitch); rows map world (x, y, z) to camera (u, depth, v)
        return [
            [cy, -sy, 0],
            [cp * sy, cp * cy, -sp],
            [sp * sy, sp * cy, cp]
        ];
    }

    function create(canvas, data, labels) {
        const st = {
            canvas, data, labels: labels || {},
            yaw: VIEWS.iso.yaw, pitch: VIEWS.iso.pitch, s: 1, tx: 0, ty: 0,
            perspective: false,
            show: { stations: true, waterlines: true, sheer: true, dwl: true, axes: true, compartments: true, tanks: true, cargo: true, liquid: true, names: false },
            center: [(data.min[0] + data.max[0]) / 2, (data.min[1] + data.max[1]) / 2, (data.min[2] + data.max[2]) / 2],
            size: Math.max(data.max[0] - data.min[0], data.max[1] - data.min[1], data.max[2] - data.min[2]) || 1
        };
        st.project = function (p) {
            const R = rotation(st.yaw, st.pitch);
            const x = p[0] - st.center[0], y = p[1] - st.center[1], z = p[2] - st.center[2];
            let u = R[0][0] * x + R[0][1] * y + R[0][2] * z;
            const d = R[1][0] * x + R[1][1] * y + R[1][2] * z;
            let v = R[2][0] * x + R[2][1] * y + R[2][2] * z;
            if (st.perspective) { const f = (3 * st.size) / (3 * st.size + d); u *= f; v *= f; }
            return [u * st.s + st.tx, -v * st.s + st.ty, d];
        };
        return st;
    }

    function allPoints(st) {
        const d = st.data, pts = [];
        for (const s of d.stations) for (const c of s.contours) for (const p of c) pts.push([s.x, p[0], p[1]]);
        return pts;
    }

    function fit(st) {
        const c = st.canvas, w = c.clientWidth, h = c.clientHeight;
        const s0 = st.s, tx0 = st.tx, ty0 = st.ty;
        st.s = 1; st.tx = 0; st.ty = 0;
        let a = Infinity, b = Infinity, e = -Infinity, f = -Infinity;
        for (const p of allPoints(st)) { const q = st.project(p); a = Math.min(a, q[0]); b = Math.min(b, q[1]); e = Math.max(e, q[0]); f = Math.max(f, q[1]); }
        if (!isFinite(a)) { st.s = s0; st.tx = tx0; st.ty = ty0; return; }
        const k = 0.85 * Math.min(w / Math.max(e - a, 1e-6), h / Math.max(f - b, 1e-6));
        st.s = k;
        st.tx = w / 2 - k * (a + e) / 2;
        st.ty = h / 2 - k * (b + f) / 2;
    }

    function line(ctx, st, pts) {
        if (pts.length < 2) return;
        ctx.beginPath();
        pts.forEach((p, i) => { const q = st.project(p); i ? ctx.lineTo(q[0], q[1]) : ctx.moveTo(q[0], q[1]); });
        ctx.stroke();
    }

    function draw(st) {
        const c = st.canvas, dpr = window.devicePixelRatio || 1, w = c.clientWidth, h = c.clientHeight;
        if (c.width !== Math.round(w * dpr) || c.height !== Math.round(h * dpr)) { c.width = Math.round(w * dpr); c.height = Math.round(h * dpr); }
        const ctx = c.getContext('2d');
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.fillStyle = '#fff'; ctx.fillRect(0, 0, w, h);
        ctx.lineJoin = 'round';
        const d = st.data;
        if (st.show.waterlines) { ctx.strokeStyle = COLORS.waterline; ctx.lineWidth = 1; d.waterlines.forEach(l => line(ctx, st, l)); }
        if (st.show.stations) {
            ctx.strokeStyle = COLORS.station; ctx.lineWidth = 1.2;
            for (const s of d.stations) for (const cn of s.contours) line(ctx, st, cn.map(p => [s.x, p[0], p[1]]));
        }
        if (st.show.sheer) { ctx.strokeStyle = COLORS.sheer; ctx.lineWidth = 1.8; d.sheer.forEach(l => line(ctx, st, l)); d.keel.forEach(l => line(ctx, st, l)); }
        if (st.show.dwl) { ctx.strokeStyle = COLORS.dwl; ctx.lineWidth = 2; d.designWaterline.forEach(l => line(ctx, st, l)); }
        drawSolids(ctx, st);
        if (st.show.axes) drawAxes(ctx, st, h);
        // status: rotation angles and zoom
        ctx.fillStyle = '#1f3fbf'; ctx.font = '12px sans-serif';
        ctx.fillText(Math.round(st.yaw) + '° / ' + Math.round(st.pitch) + '°  ×' + st.s.toFixed(2), 8, h - 8);
    }

    // Shaded solids (compartments, tanks, cargo, liquid): flat shading by the angle between each face's normal
    // and the light, painted from the farthest face to the nearest (painter's algorithm) with transparency.
    function drawSolids(ctx, st) {
        const solids = st.data.solids || [];
        if (!solids.length) return;
        const R = rotation(st.yaw, st.pitch), faces = [];
        const want = { compartment: st.show.compartments, tank: st.show.tanks, cargo: st.show.cargo };
        const push = (poly, style) => {
            const q = poly.map(p => st.project(p));
            const a = poly[0], b = poly[1], c = poly[2] || poly[1];
            // world normal of the face, turned into camera coordinates
            const e1 = [b[0] - a[0], b[1] - a[1], b[2] - a[2]], e2 = [c[0] - a[0], c[1] - a[1], c[2] - a[2]];
            let n = [e1[1] * e2[2] - e1[2] * e2[1], e1[2] * e2[0] - e1[0] * e2[2], e1[0] * e2[1] - e1[1] * e2[0]];
            const nl = Math.hypot(n[0], n[1], n[2]) || 1; n = [n[0] / nl, n[1] / nl, n[2] / nl];
            const nc = [R[0][0] * n[0] + R[0][1] * n[1] + R[0][2] * n[2], R[1][0] * n[0] + R[1][1] * n[1] + R[1][2] * n[2], R[2][0] * n[0] + R[2][1] * n[1] + R[2][2] * n[2]];
            const light = 0.35 + 0.65 * Math.abs(nc[0] * LIGHT[0] + nc[1] * LIGHT[1] + nc[2] * LIGHT[2]);
            let depth = 0; for (const p of q) depth += p[2];
            faces.push({ q, depth: depth / q.length, light, style });
        };
        for (const s of solids) {
            if (!want[s.kind]) continue;
            for (const f of s.faces) push(f, SOLID[s.kind]);
            if (st.show.liquid && s.liquid) for (const f of s.liquid) push(f, SOLID.liquid);
        }
        faces.sort((a, b) => b.depth - a.depth);
        ctx.lineWidth = 0.6;
        for (const f of faces) {
            const [r, g, b] = f.style.rgb;
            ctx.fillStyle = 'rgba(' + Math.round(r * f.light) + ',' + Math.round(g * f.light) + ',' + Math.round(b * f.light) + ',' + f.style.alpha + ')';
            ctx.strokeStyle = f.style.edge;
            ctx.beginPath();
            f.q.forEach((p, i) => i ? ctx.lineTo(p[0], p[1]) : ctx.moveTo(p[0], p[1]));
            ctx.closePath(); ctx.fill(); ctx.stroke();
        }
        if (st.show.names) {
            ctx.font = '12px sans-serif'; ctx.textAlign = 'center';
            for (const s of solids) {
                if (!want[s.kind] || !s.label) continue;
                const p = st.project(s.label);
                const text = s.kind === 'cargo' ? s.name : s.name + ' — ' + (Math.round(s.percent * 10) / 10) + '%';
                ctx.lineWidth = 3; ctx.strokeStyle = 'rgba(255,255,255,0.85)'; ctx.strokeText(text, p[0], p[1] - 6);
                ctx.fillStyle = '#1f2a37'; ctx.fillText(text, p[0], p[1] - 6);
            }
            ctx.textAlign = 'start';
        }
    }

    function drawAxes(ctx, st, h) {
        const R = rotation(st.yaw, st.pitch), o = [50, h - 50], len = 32;
        const axes = [['X', [1, 0, 0], '#d0312d'], ['Y', [0, 1, 0], '#2e8b57'], ['Z', [0, 0, 1], '#2b55c9']];
        ctx.font = 'bold 12px sans-serif';
        for (const [name, a, col] of axes) {
            const u = R[0][0] * a[0] + R[0][1] * a[1] + R[0][2] * a[2];
            const v = R[2][0] * a[0] + R[2][1] * a[1] + R[2][2] * a[2];
            ctx.strokeStyle = col; ctx.fillStyle = col; ctx.lineWidth = 2;
            ctx.beginPath(); ctx.moveTo(o[0], o[1]); ctx.lineTo(o[0] + u * len, o[1] - v * len); ctx.stroke();
            ctx.fillText(name, o[0] + u * (len + 8) - 4, o[1] - v * (len + 8) + 4);
        }
    }

    function attach(st) {
        const c = st.canvas;
        let drag = null;
        const pos = e => { const r = c.getBoundingClientRect(); return [e.clientX - r.left, e.clientY - r.top]; };
        const h = {
            down: e => {
                e.preventDefault();
                const [x, y] = pos(e);
                drag = { x, y, mode: (e.button === 2 || e.button === 1 || e.shiftKey) ? 'pan' : 'rotate' };
            },
            move: e => {
                if (!drag) return;
                const [x, y] = pos(e), dx = x - drag.x, dy = y - drag.y;
                drag.x = x; drag.y = y;
                if (drag.mode === 'pan') { st.tx += dx; st.ty += dy; }
                else {
                    // rotate around the point at the centre of the hull, keeping it where it is on screen
                    const before = st.project(st.center);
                    st.yaw -= dx * 0.5;
                    st.pitch = Math.max(-90, Math.min(90, st.pitch + dy * 0.5));
                    const after = st.project(st.center);
                    st.tx += before[0] - after[0]; st.ty += before[1] - after[1];
                }
                draw(st);
            },
            up: () => { drag = null; },
            wheel: e => {
                e.preventDefault();
                const [x, y] = pos(e), k = e.deltaY < 0 ? 1.15 : 1 / 1.15;
                st.tx = x - (x - st.tx) * k;   // zoom around the cursor
                st.ty = y - (y - st.ty) * k;
                st.s *= k;
                draw(st);
            },
            dbl: () => { fit(st); draw(st); },
            menu: e => e.preventDefault()
        };
        c.addEventListener('mousedown', h.down);
        window.addEventListener('mousemove', h.move);
        window.addEventListener('mouseup', h.up);
        c.addEventListener('wheel', h.wheel, { passive: false });
        c.addEventListener('dblclick', h.dbl);
        c.addEventListener('contextmenu', h.menu);
        const ro = new ResizeObserver(() => draw(st));
        ro.observe(c);
        st.dispose = () => {
            c.removeEventListener('mousedown', h.down);
            window.removeEventListener('mousemove', h.move);
            window.removeEventListener('mouseup', h.up);
            c.removeEventListener('wheel', h.wheel);
            c.removeEventListener('dblclick', h.dbl);
            c.removeEventListener('contextmenu', h.menu);
            ro.disconnect();
        };
    }

    return {
        init(canvas, data) {
            if (canvas._hull3d) canvas._hull3d.dispose();
            const st = create(canvas, data);
            canvas._hull3d = st;
            attach(st);
            fit(st); draw(st);
        },
        view(canvas, name) {
            const st = canvas && canvas._hull3d, v = VIEWS[name];
            if (!st || !v) return;
            st.yaw = v.yaw; st.pitch = v.pitch; fit(st); draw(st);
        },
        fit(canvas) { const st = canvas && canvas._hull3d; if (st) { fit(st); draw(st); } },
        option(canvas, key, value) {
            const st = canvas && canvas._hull3d; if (!st) return;
            if (key === 'perspective') st.perspective = !!value; else st.show[key] = !!value;
            draw(st);
        },
        png(canvas, name) {
            const a = document.createElement('a');
            a.href = canvas.toDataURL('image/png'); a.download = name; a.click();
        },
        dispose(canvas) { if (canvas && canvas._hull3d) { canvas._hull3d.dispose(); delete canvas._hull3d; } }
    };
})();
