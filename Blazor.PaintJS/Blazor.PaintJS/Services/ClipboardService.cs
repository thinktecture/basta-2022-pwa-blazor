using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.PaintJS.Services
{
    public class ClipboardService: JSServiceBase
    {
        public ClipboardService(IJSRuntime jsRuntime)
            :base(jsRuntime)
        {
        }

        public async Task CopyCanavasAsync(ElementReference _clickBtn)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("copyCanavas", _clickBtn);
        }

        public async Task<bool> PasteAsync()
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("pasteImage");
        }

        public async Task CopyAsync(string dataUrl)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("copyImage", dataUrl);
        }
    }
}
