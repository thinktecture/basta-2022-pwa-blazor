export async function shareImage(dataUrl) {
    const blob = await (await fetch(dataUrl)).blob();
    const file = new File([blob], 'paint.png', { type: 'image/png' });
    const items = { files: [file], title: 'paint.png' }
    if (navigator.canShare(items)) {
        try {
            await navigator.share(items);
        } catch (e) {
            console.log(e);
        }
    }
}