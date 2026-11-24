using LAB_3_assignment.Data;
using LAB_3_assignment.Models;
using LAB_3_assignment.Services;

namespace LAB_3_assignment.Views;

public sealed class DashboardForm : Form
{
    private readonly User user; private readonly Repository repo = new();
    public DashboardForm(User user)
    {
        this.user = user; Text = $"Bicycle Rental Management - {user.Role}"; Width = 900; Height = 520; StartPosition = FormStartPosition.CenterScreen;
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), AutoScroll = true };
        AddButton(flow, "Customers", (_, _) => new CustomersForm(repo).ShowDialog());
        AddButton(flow, "Rentals", (_, _) => new RentalsForm(repo).ShowDialog());
        AddButton(flow, "Payments / Unpaid", (_, _) => new PaymentsForm(repo).ShowDialog());
        if (user.Role == UserRole.Administrator)
        {
            AddButton(flow, "Bicycles", (_, _) => new BicyclesForm(repo).ShowDialog());
            AddButton(flow, "Attendant Accounts", (_, _) => new UsersForm(repo).ShowDialog());
            AddButton(flow, "Hourly Prices", (_, _) => new PricesForm(repo).ShowDialog());
            AddButton(flow, "Reports", (_, _) => new ReportsForm(repo).ShowDialog());
        }
        AddButton(flow, "Log out", (_, _) => Close()); Controls.Add(flow);
    }
    private static void AddButton(Control parent, string text, EventHandler action) { var b = new Button { Text = text, Width = 180, Height = 55, Margin = new Padding(10) }; b.Click += action; parent.Controls.Add(b); }
}

internal static class Ui
{
    public static DataGridView Grid() => new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    public static Button Button(string text, EventHandler action) { var b = new Button { Text = text, AutoSize = true }; b.Click += action; return b; }
    public static TextBox Input(string label, Control parent, int row, TableLayoutPanel table) { table.Controls.Add(new Label { Text = label, AutoSize = true }, 0, row); var t = new TextBox { Dock = DockStyle.Fill }; table.Controls.Add(t, 1, row); return t; }
    public static void Error(Exception ex) => MessageBox.Show(ex.Message, "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
