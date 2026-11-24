using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;
public sealed class UsersForm : Form
{
    readonly Repository repo; readonly DataGridView grid = Ui.Grid();
    public UsersForm(Repository repo) { this.repo = repo; Text = "Attendant accounts"; Width = 650; Height = 400; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 }; top.Controls.Add(Ui.Button("Add attendant", (_, _) => Add())); top.Controls.Add(Ui.Button("Delete selected", (_, _) => { if (grid.CurrentRow?.DataBoundItem is User u && u.Username != "admin") { repo.DeleteUser(u.Id); RefreshGrid(); } })); Controls.Add(grid); Controls.Add(top); RefreshGrid(); }
    void RefreshGrid() => grid.DataSource = repo.GetUsers().Where(u => u.Role == UserRole.Attendant).ToList();
    void Add() { using var f = new Form { Text = "New attendant", Width = 400, Height = 200, StartPosition = FormStartPosition.CenterParent }; var t = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), RowCount = 3, ColumnCount = 2 }; var u = Ui.Input("Username", f, 0, t); var p = Ui.Input("Password", f, 1, t); t.Controls.Add(Ui.Button("Save", (_, _) => { try { repo.AddUser(u.Text.Trim(), p.Text, UserRole.Attendant); f.DialogResult = DialogResult.OK; } catch (Exception ex) { Ui.Error(ex); } }), 1, 2); f.Controls.Add(t); if (f.ShowDialog(this) == DialogResult.OK) RefreshGrid(); }
}
