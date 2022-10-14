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

        private async void OnPointerDown(PointerEventArgs args)
        {
            // EX 4
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
            //await using var context = await _canvas!.GetContext2DAsync();
            //await context.FillStyleAsync("white");
            //await context.FillRectAsync(0, 0, 600, 480);
            //await context.FillStyleAsync("black");
        }
        #endregion
    }
}