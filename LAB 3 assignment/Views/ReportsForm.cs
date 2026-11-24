using LAB_3_assignment.Data;

namespace LAB_3_assignment.Views;
public sealed class ReportsForm : Form
{
    readonly Repository repo; readonly Label result = new() { AutoSize = true, Padding = new Padding(10) };
    public ReportsForm(Repository repo) { this.repo = repo; Text = "Reports"; Width = 800; Height = 500; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50 }; top.Controls.Add(Ui.Button("Today's money", (_, _) => { var d = DateTime.Today; result.Text = $"Received today: {repo.GetReceived(d, d.AddDays(1)):N0} RWF"; })); top.Controls.Add(Ui.Button("This month's money", (_, _) => { var d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); result.Text = $"Received this month: {repo.GetReceived(d, d.AddMonths(1)):N0} RWF"; })); var grid = Ui.Grid(); grid.DataSource = repo.GetBicycles().Select(b => new { b.Code, b.Type, TimesRented = b.RentalCount }).ToList(); Controls.Add(grid); Controls.Add(result); Controls.Add(top); }
}
