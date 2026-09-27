async function openFolder(folderType, entityId) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const res = await fetch('/Folder/Open', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token ?? ''
        },
        body: `folderType=${encodeURIComponent(folderType)}&entityId=${encodeURIComponent(entityId)}`
    });

    if (!res.ok) {
        let errorMessage = res.status.toString();
        try {
            const data = await res.json();
            errorMessage = data.error ?? errorMessage;
        } catch {
            // сервер вернул не JSON (например, HTML-страницу ошибки)
        }
        alert('Не удалось открыть: ' + errorMessage);
    }
}

window.openFolder = openFolder;