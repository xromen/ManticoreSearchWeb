using ManticoreSearch.Business.Models;
using ManticoreSearch.Business.Services;
using ManticoreSearch.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ManticoreSearch.UI.Components.Layout
{
    public partial class NavMenu : ComponentBase, IDisposable
    {
        [Inject]
        private ManticoreService ManticoreService { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        private CancellationTokenSource CancellationTokenSource { get; set; } = new();

        private List<Table> Tables { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Tables = await ManticoreService.GetAllTablesAsync(CancellationTokenSource.Token);

            await base.OnInitializedAsync();
        }

        private async Task DeleteTable(Table table)
        {
            var parameters = new DialogParameters<ConfirmDialog>
            {
                { x => x.ContentText, "Таблица будет удалена со всеми содержащимися в ней данными. Отменить данную операцию будет невозможно!" }
            };
            var result = await DialogService.Show<ConfirmDialog>($"Уверены что хотите удалить таблицу {table.Name}?", parameters).Result;

            if (result == null || result.Canceled)
                return;

            try
            {
                await ManticoreService.DeleteTableAsync(table, CancellationTokenSource.Token);

                Tables.Remove(table);

                Snackbar.Add($"Таблица {table.Name} была удалена успешно.");
            }
            catch(Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Warning);
                return;
            }
            finally
            {
                StateHasChanged();
            }

        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
        }
    }
}
