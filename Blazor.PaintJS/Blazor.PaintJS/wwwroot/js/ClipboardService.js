export async function copyImage(dataUrl) {
    try {
        // Safari treats user activation differently:
        // https://bugs.webkit.org/show_bug.cgi?id=222262.
        navigator.clipboard.write([
            new ClipboardItem({
                'image/png': new Promise(async (resolve) => {
                    const blob = await (await fetch(dataUrl)).blob();
                    resolve(blob)
                })
            }),
        ]);
    } catch {
        // Chromium
        const blob = await (await fetch(dataUrl)).blob();
        navigator.clipboard.write([
            new ClipboardItem({
                [blob.type]: blob,
            }),
        ]);
    }
}

export async function pasteImage() {
    cleanUpImage();

    const clipboardItems = await navigator.clipboard.read();
    for (let clipboardItem of clipboardItems) {
        for (let itemType of clipboardItem.types) {
            if (itemType === 'image/png') {
                const blob = await clipboardItem.getType(itemType);
                return createImageElement(blob, true)
            }
        }
    }

    return Promise.resolve(false);
}