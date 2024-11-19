using ManticoreSearch.Business.Enums;
using ManticoreSearch.Business.Models;
using ManticoreSearch.Business.Services;
using ManticoreSearch.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ManticoreSearch.UI.Components.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private FilesService FilesService { get; set; } = default!;

        [Inject]
        private ManticoreService ManticoreService { get; set; } = default!;

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string _dragClass = DefaultDragClass;
        private MudFileUpload<IBrowserFile>? _fileUpload;

        private readonly string RegexForNaming = @"^[a-z][a-z1-9_]*$";

        private bool loading = false;
        private int? currentProgress = null;
        private CancellationTokenSource CancellationTokenSource { get; set; } = new();

        private UploadModel UploadModel { get; set; } = UploadModel.GetDefault();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task ClearAsync()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
            }

            UploadModel = UploadModel.GetDefault();
            ClearDragClass();
            await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);

            StateHasChanged();
        }

        private async Task OnInputFileChanged(InputFileChangeEventArgs e)
        {
            try
            {
                await ClearAsync();
                var file = e.File;

                if (!FileTypes.IsValid(file.Name))
                {
                    Snackbar.Add("Данный тип файла не поддерживается.", Severity.Warning);
                    return;
                }

                var fileType = FileTypes.Get(file.Name)!.Value;

                ClearDragClass();

                UploadModel.File = file;
                UploadModel.FileType = fileType;

                StateHasChanged();

                CancellationTokenSource = new CancellationTokenSource();

                UploadModel.FilePath = await FilesService.SaveFileAsync(file, GetProgress(file.Size), CancellationTokenSource.Token);

                await ParseExampleTable();
            }
            catch (OperationCanceledException)
            {
                //Snackbar.Add("Загрузка файла была отменена.", Severity.Info);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Warning);
            }
        }

        private async Task ParseExampleTable()
        {
            if (UploadModel.FilePath == null)
                return;

            loading = true;

            CancellationTokenSource = new CancellationTokenSource();

            UploadModel.ExampleTable = await FilesService.GetExampleTableAsync(UploadModel, CancellationTokenSource.Token);

            loading = false;

            StateHasChanged();
        }

        private async Task FirstRowHeaderValueChanged(bool value)
        {
            UploadModel.FirstRowHeader = value;
            await ParseExampleTable();
        }

        private void Upload()
        {
            // Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
            Snackbar.Add("TODO: Upload your files!");
        }

        private IProgress<long> GetProgress(long fileSize)
        {
            currentProgress = 0;
            IProgress<long> progress = new Progress<long>(value =>
            {
                decimal tmp = (decimal)(value * 100) / fileSize;

                if (tmp != currentProgress && tmp > currentProgress)
                {
                    currentProgress = (int)tmp;
                    StateHasChanged();
                }
            });

            return progress;
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;
    }
}
