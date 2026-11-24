using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;
public sealed class PricesForm : Form
{
    readonly Repository repo; readonly DataGridView grid = Ui.Grid();
    public PricesForm(Repository repo) { this.repo = repo; Text = "Hourly prices"; Width = 650; Height = 400; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 }; top.Controls.Add(Ui.Button("Edit selected price", (_, _) => Edit())); Controls.Add(grid); Controls.Add(top); RefreshGrid(); }
    void RefreshGrid() => grid.DataSource = repo.GetPrices();
    void Edit() { if (grid.CurrentRow?.DataBoundItem is not PriceBand band) return; var value = Microsoft.VisualBasic.Interaction.InputBox($"Price for hour {band.StartHour} to {band.EndHour} (RWF):", "Change price", band.PricePerHour.ToString()); if (decimal.TryParse(value, out var price) && price >= 0) { repo.UpdatePrice(band.StartHour, price); RefreshGrid(); } }
}
