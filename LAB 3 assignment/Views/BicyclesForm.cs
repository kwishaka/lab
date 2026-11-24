using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;

public sealed class BicyclesForm : Form
{
    readonly Repository repo; readonly DataGridView grid = Ui.Grid();
    public BicyclesForm(Repository repo) { this.repo = repo; Text = "Bicycles"; Width = 850; Height = 500; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 }; top.Controls.Add(Ui.Button("Add", (_, _) => Edit(null))); top.Controls.Add(Ui.Button("Edit", (_, _) => { if (grid.CurrentRow?.DataBoundItem is Bicycle b) Edit(b); })); top.Controls.Add(Ui.Button("Delete", (_, _) => { if (grid.CurrentRow?.DataBoundItem is Bicycle b) try { repo.DeleteBicycle(b.Id); RefreshGrid(); } catch (Exception ex) { Ui.Error(ex); } })); Controls.Add(grid); Controls.Add(top); RefreshGrid(); }
    void RefreshGrid() => grid.DataSource = repo.GetBicycles();

    private void InitializeComponent()
    {

    }

    void Edit(Bicycle? b) { using var f = new Form { Text = "Bicycle", Width = 400, Height = 250, StartPosition = FormStartPosition.CenterParent }; var t = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), RowCount = 4, ColumnCount = 2 }; var code = Ui.Input("Code", f, 0, t); var type = Ui.Input("Type", f, 1, t); var status = new ComboBox { DataSource = Enum.GetValues<BicycleStatus>(), Dock = DockStyle.Fill }; t.Controls.Add(new Label { Text = "Status" }, 0, 2); t.Controls.Add(status, 1, 2); code.Text = b?.Code ?? ""; type.Text = b?.Type ?? ""; if (b is not null) status.SelectedItem = b.Status; t.Controls.Add(Ui.Button("Save", (_, _) => { try { if (string.IsNullOrWhiteSpace(code.Text) || string.IsNullOrWhiteSpace(type.Text)) throw new ArgumentException("Code and type are required."); repo.SaveBicycle(new Bicycle(b?.Id ?? 0, code.Text.Trim(), type.Text.Trim(), (BicycleStatus)status.SelectedItem!, b?.RentalCount ?? 0)); f.DialogResult = DialogResult.OK; } catch (Exception ex) { Ui.Error(ex); } }), 1, 3); f.Controls.Add(t); if (f.ShowDialog(this) == DialogResult.OK) RefreshGrid(); }
}
