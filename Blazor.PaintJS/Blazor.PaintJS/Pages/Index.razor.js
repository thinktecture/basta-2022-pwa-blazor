﻿// TODO: Add comment
export function registerEvents(id, component) {
    const target = document.getElementById(id);
    document.querySelector('body').addEventListener('pointerup', (pointerEvent) => {
        if (pointerEvent.target !== target) {
            component.invokeMethodAsync('OnPointerUp');
            unregisterEvents();
        } else {
            const currentElement = document.elementFromPoint(pointerEvent.clientX, pointerEvent.clientY)
            if (currentElement !== target) {
                component.invokeMethodAsync('OnPointerUp');
                unregisterEvents();
            }
        }
    });
}

export function getCanvasBlob(id) {
    return new Promise((resolve) => document.getElementById(id).toBlob((blob) => resolve(blob)));
}

export async function initializeLaunchQueue(component) {
    //EX16
}

function unregisterEvents() {
    document.querySelector('body').removeEventListener('pointerup', () => console.log('pointerup unregistered'));
}
