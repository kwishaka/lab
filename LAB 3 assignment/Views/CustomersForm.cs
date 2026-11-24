using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;
public sealed class CustomersForm : Form
{
    readonly Repository repo; readonly DataGridView grid = Ui.Grid(); readonly TextBox search = new();
    public CustomersForm(Repository repo) { this.repo = repo; Text = "Customers"; Width = 850; Height = 500; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 }; top.Controls.Add(new Label { Text = "Search name/phone:", AutoSize = true }); top.Controls.Add(search); top.Controls.Add(Ui.Button("Search", (_, _) => RefreshGrid())); top.Controls.Add(Ui.Button("Add", (_, _) => Edit(null))); top.Controls.Add(Ui.Button("Edit", (_, _) => { if (grid.CurrentRow?.DataBoundItem is Customer c) Edit(c); })); Controls.Add(grid); Controls.Add(top); RefreshGrid(); }
    void RefreshGrid() { grid.DataSource = repo.GetCustomers(search.Text); }
    void Edit(Customer? c) { using var f = new Form { Text = c is null ? "New customer" : "Edit customer", Width = 400, Height = 250, StartPosition = FormStartPosition.CenterParent }; var t = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), RowCount = 4, ColumnCount = 2 }; var name = Ui.Input("Name", f, 0, t); var phone = Ui.Input("Phone", f, 1, t); var id = Ui.Input("ID number", f, 2, t); name.Text = c?.Name ?? ""; phone.Text = c?.Phone ?? ""; id.Text = c?.IdentificationNumber ?? ""; var save = Ui.Button("Save", (_, _) => { try { if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(phone.Text) || string.IsNullOrWhiteSpace(id.Text)) throw new ArgumentException("All customer fields are required."); repo.SaveCustomer(new Customer(c?.Id ?? 0, name.Text.Trim(), phone.Text.Trim(), id.Text.Trim())); f.DialogResult = DialogResult.OK; } catch (Exception ex) { Ui.Error(ex); } }); t.Controls.Add(save, 1, 3); f.Controls.Add(t); if (f.ShowDialog(this) == DialogResult.OK) RefreshGrid(); }
}
