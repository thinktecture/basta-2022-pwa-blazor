using Microsoft.JSInterop;

namespace Blazor.PaintJS.Services
{
    public abstract class JSServiceBase : IAsyncDisposable
    {
        protected readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public JSServiceBase(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", $"./js/{GetType().Name}.js").AsTask());
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
