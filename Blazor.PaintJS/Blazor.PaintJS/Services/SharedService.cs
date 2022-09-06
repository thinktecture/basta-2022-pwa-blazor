using Excubo.Blazor.Canvas;
using Microsoft.JSInterop;
using System.Reflection;

namespace Blazor.PaintJS.Services
{
    public class ShareService : JSServiceBase
    {
        public ShareService(IJSRuntime jSRuntime)
            :base(jSRuntime)
        {
        }

        public async Task ShareAsync(string dataUrl)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("shareImage", dataUrl);
        }
    }
}
