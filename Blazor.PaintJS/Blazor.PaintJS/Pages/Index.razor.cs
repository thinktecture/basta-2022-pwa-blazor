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
using Thinktecture.Blazor.AsyncClipboard;
using Thinktecture.Blazor.AsyncClipboard.Models;

namespace Blazor.PaintJS.Pages
{
    public partial class Index
    {
        [Inject] private PaintService _paintService { get; set; } = default!;
        [Inject] private ImageService _imageService { get; set; } = default!;
        [Inject] private AsyncClipboardService _asyncClipboardService { get; set; } = default!;
        [Inject] private WebShareService _shareService { get; set; } = default!;
        [Inject] private FileSystemAccessService _fileSystemAccessService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private IJSObjectReference? _module;
        private DotNetObjectReference<Index>? _selfReference;

        protected FileSystemFileHandle? _fileHandle;

        private bool _fileSystemAccessSupported = true;
        private bool _clipboardApiSupported = true;
        private bool _shareApiSupported = true;
        private Point? _previousPoint;

        // EX 1
        private Canvas? _canvas;

        // EX 12
        private static FilePickerAcceptType[] _acceptTypes = new FilePickerAcceptType[]
        {
            new FilePickerAcceptType
            {
                Accept = new Dictionary<string, string[]> {
                 { "image/png", new[] {".png" } }
                },
                Description = "PNG files"
            }
        };

        private SaveFilePickerOptionsStartInWellKnownDirectory _savePickerOptions = new SaveFilePickerOptionsStartInWellKnownDirectory
        {
            StartIn = WellKnownDirectory.Pictures,
            Types = _acceptTypes
        };

        private readonly OpenFilePickerOptionsStartInWellKnownDirectory _filePickerOptions = new()
        {
            Multiple = false,
            StartIn = WellKnownDirectory.Pictures,
            Types = _acceptTypes
        };


        [JSInvokable]
        public async Task DrawImageAsync()
        {
            //EX 16
        }

        protected override async Task OnInitializedAsync()
        {
            //EX 17
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    //EX 2
                    //EX 3
                    await using var context = await _canvas!.GetContext2DAsync(desynchronized: true);

                    await context.FillStyleAsync("white");
                    await context.FillRectAsync(0, 0, 600, 480);
                    await context.FillStyleAsync("black");

                    _selfReference = DotNetObjectReference.Create(this);
                    if (_module == null)
                    {
                        _module = await JS.InvokeAsync<IJSObjectReference>(
                            "import", "./Pages/Index.razor.js");
                    }


                    //EX16
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnPointerMove(PointerEventArgs args)
        {
            //EX 4
            if (_previousPoint != null)
            {
                var currentPoint = new Point
                {
                    X = (int)Math.Floor(args.OffsetX),
                    Y = (int)Math.Floor(args.OffsetY)
                };

                var points = _paintService.BrensenhamLine(_previousPoint.Value, currentPoint);
                await using var context = await _canvas!.GetContext2DAsync(desynchronized: true);
                foreach (var point in points)
                {
                    await context.FillRectAsync(point.X, point.Y, 2, 2);
                }
                _previousPoint = currentPoint;
            }
            //EX 5
        }

        private async void OnPointerDown(PointerEventArgs args)
        {
            // EX 4
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

        private async void OnColorChange(ChangeEventArgs args)
        {
            //EX 6
            await using var context = await _canvas!.GetContext2DAsync(desynchronized: true);
            await context.FillStyleAsync(args.Value?.ToString());
        }

        private async Task SaveFileLocal()
        {
            try
            {
                //EX 11
                if (_fileHandle == null)
                {
                    _fileHandle = await _fileSystemAccessService
                        .ShowSaveFilePickerAsync(_savePickerOptions);
                }

                var writeable = await _fileHandle.CreateWritableAsync();
                var image = await _imageService.GetImageDataAsync("canvas");
                await writeable.WriteAsync(image);
                await writeable.CloseAsync();

                await _fileHandle.JSReference.DisposeAsync();
                _fileHandle = null;
            }
            catch
            {
                Console.WriteLine("Save file failed");
            }
            finally
            {
                _fileHandle = null;
            }
        }

        private async Task OpenLocalFile()
        {
            // EX 12
            try
            {
                var fileHandles = await _fileSystemAccessService.ShowOpenFilePickerAsync(_filePickerOptions);
                _fileHandle = fileHandles.Single();
            }
            catch (JSException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (_fileHandle is not null)
                {
                    var file = await _fileHandle.GetFileAsync();
                    await _imageService.OpenFileAccessAsync(file.JSReference);
                    await using var context = await _canvas!.GetContext2DAsync();
                    await context.DrawImageAsync("image", 0, 0);
                }
            }
        }

        private async void Copy()
        {
            //EX 13
            var imagePromise = _asyncClipboardService.GetObjectReference(
                _module!, "getCanvasBlob", "canvas");

            var clipboardItem = new ClipboardItem(
                new Dictionary<string, IJSObjectReference>
                 {
                        { "image/png", imagePromise }
                 });

            await _asyncClipboardService.WriteAsync(
                new[] { clipboardItem }
            );
        }

        private async Task Paste()
        {
            //EX 14
            var clipboardItems = await _asyncClipboardService.ReadAsync();
            var pngItem = clipboardItems.FirstOrDefault(
                            c => c.Types.Contains("image/png"));
            if (pngItem is not null)
            {
                var blob = await pngItem.GetTypeAsync("image/png");
                await _imageService.OpenFileAccessAsync(blob);
                await using var context = await _canvas!.GetContext2DAsync();
                await context.DrawImageAsync("image", 0, 0);
            }
        }

        private async Task Share()
        {
            //EX 15
            var fileReference = await _imageService.GenerateFileReferenceAsync(
                    await _canvas!.ToDataURLAsync());

            await _shareService.ShareAsync(
                new WebShareDataModel
                {
                    Files = new[] { fileReference }
                }
            );
        }

        private async Task SaveFile()
        {
            //EX 18
        }

        private async Task OpenFile(InputFileChangeEventArgs args)
        {
            // EX 19
        }

        #region Utils
        // Method which is JSInvokable must be public
        [JSInvokable]
        public void OnPointerUp()
        {
            _previousPoint = null;
        }

        private async Task ResetCanvas()
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await context.FillStyleAsync("white");
            await context.FillRectAsync(0, 0, 600, 480);
            await context.FillStyleAsync("black");
        }
        #endregion
    }
}