using Blazor.PaintJS.Services;
using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Drawing;
using KristofferStrube.Blazor.FileSystemAccess;
using Thinktecture.Blazor.WebShare;
using Thinktecture.Blazor.WebShare.Models;

namespace Blazor.PaintJS.Pages
{
    public partial class Index
    {
        [Inject] private PaintService _paintService { get; set; } = default!;
        [Inject] private ImageService _imageService { get; set; } = default!;
        // [Inject] private ClipboardService _clipboardService { get; set; } = default!;
        [Inject] private WebShareService _shareService { get; set; } = default!;
        [Inject] private FileSystemAccessService _fileSystemAccessService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private IJSObjectReference? _module;
        private DotNetObjectReference<Index>? _selfReference;

        protected FileSystemFileHandle? _fileHandle;

        private bool _fileSystemAccessSupported = false;
        private Canvas? _canvas;
        private Point? _previousPoint;

        // Method which is JSInvokable must be public
        [JSInvokable]
        public void OnPointerUp()
        {
            _previousPoint = null;
        }

        [JSInvokable]
        public async Task DrawImageAsync()
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await context.DrawImageAsync("image", 0, 0);
        }

        protected override async Task OnInitializedAsync()
        {
            _fileSystemAccessSupported = await _fileSystemAccessService.IsSupported();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await using var context = await _canvas!.GetContext2DAsync();
                    await context.FillStyleAsync("white");
                    await context.FillRectAsync(0, 0, 600, 480);
                    await context.FillStyleAsync("black");

                    _selfReference = DotNetObjectReference.Create(this);
                    if (_module == null)
                    {
                        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Index.razor.js");
                        await _module.InvokeVoidAsync("initializeLaunchQueue", _selfReference);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnPointerDown(PointerEventArgs args)
        {
            if (_module != null && _canvas!.AdditionalAttributes.TryGetValue("id", out var id))
            {
                await _module.InvokeVoidAsync("registerEvents", id, _selfReference);
            }

            _previousPoint = new Point
            {
                X = (int)Math.Floor(args.OffsetX),
                Y = (int)Math.Floor(args.OffsetY)
            };
        }

        private async Task OnPointerMove(PointerEventArgs args)
        {
            if (_previousPoint != null)
            {
                var currentPoint = new Point
                {
                    X = (int)Math.Floor(args.OffsetX),
                    Y = (int)Math.Floor(args.OffsetY)
                };

                var points = _paintService.BrensenhamLine(_previousPoint.Value, currentPoint);
                await using var context = await _canvas.GetContext2DAsync();
                foreach (var point in points)
                {
                    await context.FillRectAsync(point.X, point.Y, 2, 2);
                }

                _previousPoint = currentPoint;
            }
        }

        private async Task OpenFile(InputFileChangeEventArgs args)
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await _imageService.OpenAsync(args.File.OpenReadStream(1024 * 15 * 1000));
            await context.DrawImageAsync("image", 0, 0);
        }

        private async Task OpenLocalFile()
        {
            if (_fileHandle != null)
            {
                await _fileHandle.JSReference.DisposeAsync();
                _fileHandle = null;
            }

            try
            {
                OpenFilePickerOptionsStartInWellKnownDirectory options = new()
                {
                    Multiple = false,
                    StartIn = WellKnownDirectory.Pictures
                };
                var fileHandles = await _fileSystemAccessService.ShowOpenFilePickerAsync(options);
                _fileHandle = fileHandles.Single();
            }
            catch (JSException ex)
            {
                // Handle Exception or cancelation of File Access prompt
                Console.WriteLine(ex);
            }
            finally
            {
                if (_fileHandle is not null)
                {
                    var file = await _fileHandle.GetFileAsync();
                    Console.WriteLine(file.Name);
                    await _imageService.OpenFileAccessAsync(file.JSReference);
                    await using var context = await _canvas!.GetContext2DAsync();
                    await context.DrawImageAsync("image", 0, 0);
                }
            }
        }

        private async Task DownloadFile()
        {
            await _imageService.SaveAsync(await _canvas!.ToDataURLAsync());
        }

        private async Task SaveFileLocal()
        {
            if (_fileHandle != null)
            {
                var writeable = await _fileHandle.CreateWritableAsync();
                var test = await _imageService.GetImageData("paint-canvas");
                await writeable.WriteAsync(test);
                await writeable.CloseAsync();

                await _fileHandle.JSReference.DisposeAsync();
                _fileHandle = null;
            }
        }

        private async void Copy()
        {
            var dataUrl = await _canvas!.ToDataURLAsync();
            // await _clipboardService.CopyAsync(dataUrl);
        }

        private async Task Paste()
        {
            // var success = await _clipboardService.PasteAsync();
            //if (success)
            //{
            //    await using var context = await _canvas!.GetContext2DAsync();
            //    await context.DrawImageAsync("image", 0, 0);
            //}
        }

        private async Task Share()
        {
            var fileReference = await _imageService.GenerateFileReference(await _canvas!.ToDataURLAsync());
            await _shareService.ShareAsync(new WebShareDataModel
            {
                Files = new[] { fileReference }
            });
        }

        private async Task Cleanup()
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await context.FillStyleAsync("white");
            await context.FillRectAsync(0, 0, 600, 480);
            await context.FillStyleAsync("black");
        }

        private async void OnColorChange(ChangeEventArgs args)
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await context.FillStyleAsync(args.Value?.ToString());
        }       
    }
}