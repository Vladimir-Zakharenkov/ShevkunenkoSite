async function openTextFolder(textInfoId) {

    console.log('openTextFolder вызван с:', textInfoId);

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const res = await fetch('/Folder/OpenText', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token ?? ''
        },
        body: 'textInfoId=' + encodeURIComponent(textInfoId)
    });

    if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        alert('Не удалось открыть: ' + (data.error ?? res.status));
    }
}

window.openTextFolder = openTextFolder;