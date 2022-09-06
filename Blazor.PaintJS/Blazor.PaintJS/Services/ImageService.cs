using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Blazor.PaintJS.Services
{
    public class ImageService : JSServiceBase
    {
        public ImageService(IJSRuntime jsRuntime)
            :base(jsRuntime)
        {
        }

        public async Task OpenAsync(Stream stream)
        {
            var module = await _moduleTask.Value;
            var currentStream = new DotNetStreamReference(stream);
            await module.InvokeVoidAsync("createImage", currentStream);
        }

        public async Task OpenFileAsync(IJSObjectReference reference)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("readImage", reference);
        }

        public async Task SaveAsync(string dataUrl)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("saveImage", dataUrl);
        }
        
        public async Task<byte[]> GetImageData(string canvasRefId)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<byte[]>("getCanvasImageData", canvasRefId);
        }
    }
}
