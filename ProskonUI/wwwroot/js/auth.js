window.proskon = window.proskon || {};
window.proskon.signIn = async function (model) {
    const resp = await fetch('/api/auth/signin?rememberMe=true', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(model),
        credentials: 'include'
    });
    let data = null;
    try { data = await resp.json(); } catch { }
    return { ok: resp.ok, startPage: data?.startPage };
}

window.proskon.signOut = async function () {
    try {
        const res = await fetch('/api/auth/signout', {
            method: 'GET',
            credentials: 'include' // cookie'yi gönder
        });
        return res.ok;
    } catch {
        return false;
    }
};
