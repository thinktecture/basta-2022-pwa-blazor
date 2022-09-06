let image = null;

function createImageElement(blob, returnValue) {
    return new Promise((resolve, reject) => {
        const imageRef = new Image();
        imageRef.onload = () => {
            image = document.getElementById(imageRef.id);
            resolve(returnValue);
        };
        imageRef.onerror = () => {
            imageRef.remove();
            reject();
        };
        imageRef.src = URL.createObjectURL(blob);
        imageRef.style.display = 'none';
        imageRef.id = 'image';

        document.body.appendChild(imageRef);
    });
}

function cleanUpImage() {
    if (image) {
        image.remove();
        image = null;
    }
}