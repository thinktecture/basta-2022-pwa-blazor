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

        // EX 12

        

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

            //EX 5
        }

        private async void OnColorChange(ChangeEventArgs args)
        {
            //EX 6
        }

        private async Task SaveFileLocal()
        {
            try
            {
                //EX 11
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
        }

        private async void Copy()
        {
            //EX 13
        }

        private async Task Paste()
        {
            //EX 14
        }

        private async Task Share()
        {
            //EX 15
        }

        private async Task OpenFile(InputFileChangeEventArgs args)
        {
            await using var context = await _canvas!.GetContext2DAsync();
            await _imageService.OpenAsync(args.File.OpenReadStream(1024 * 15 * 1000));
            await context.DrawImageAsync("image", 0, 0);
        }

        private async Task DownloadFile()
        {
            //EX 18
        }



        

        #region Utils
        // Method which is JSInvokable must be public
        [JSInvokable]
        public void OnPointerUp()
        {
            _previousPoint = null;
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

        private async Task ResetCanvas()
        {
            //await using var context = await _canvas!.GetContext2DAsync();
            //await context.FillStyleAsync("white");
            //await context.FillRectAsync(0, 0, 600, 480);
            //await context.FillStyleAsync("black");
        }
        #endregion
    }
}