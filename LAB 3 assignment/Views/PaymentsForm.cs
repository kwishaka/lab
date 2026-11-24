using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;
public sealed class PaymentsForm : Form
{
    readonly Repository repo; readonly DataGridView grid = Ui.Grid();
    public PaymentsForm(Repository repo) { this.repo = repo; Text = "Payments and unpaid rentals"; Width = 1000; Height = 500; var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 }; top.Controls.Add(Ui.Button("Record payment", (_, _) => Pay())); top.Controls.Add(Ui.Button("Print receipt", (_, _) => PrintReceipt())); top.Controls.Add(Ui.Button("Show all rentals", (_, _) => RefreshGrid(false))); Controls.Add(grid); Controls.Add(top); RefreshGrid(true); }
    void RefreshGrid(bool unpaid) => grid.DataSource = repo.GetRentals(unpaid);
    void Pay() { if (grid.CurrentRow?.DataBoundItem is not Rental r) return; var amount = Microsoft.VisualBasic.Interaction.InputBox($"Amount received (full amount: {r.TotalPrice:N0} RWF):", "Payment", r.TotalPrice.ToString()); if (!decimal.TryParse(amount, out var value) || value <= 0) { MessageBox.Show("Enter a valid payment amount."); return; } using var method = new PaymentMethodDialog(); if (method.ShowDialog(this) == DialogResult.OK) { repo.AddPayment(r.Id, value, method.Method); MessageBox.Show("Payment recorded."); RefreshGrid(true); } }
    void PrintReceipt() { var selected = grid.CurrentRow?.DataBoundItem as Rental; if (selected is null || !selected.IsPaid) { MessageBox.Show("Select a fully paid rental first."); return; } using var document = new System.Drawing.Printing.PrintDocument(); document.PrintPage += (_, e) => { using var font = new Font("Arial", 12); e.Graphics.DrawString($"BICYCLE RENTAL RECEIPT\n\nRental: {selected.Id}\nCustomer: {selected.CustomerName}\nBicycle: {selected.BicycleCode}\nHours: {selected.StartHour} - {selected.EndHour}\nTotal paid: {selected.TotalPrice:N0} RWF\nDate: {DateTime.Now:g}", font, Brushes.Black, 80, 80); }; using var preview = new PrintPreviewDialog { Document = document, Width = 900, Height = 700 }; preview.ShowDialog(this); }
}
internal sealed class PaymentMethodDialog : Form
{
    public PaymentMethod Method => (PaymentMethod)combo.SelectedItem!; readonly ComboBox combo = new() { DataSource = Enum.GetValues<PaymentMethod>(), Dock = DockStyle.Top };
    public PaymentMethodDialog() { Text = "Payment method"; Width = 250; Height = 130; Controls.Add(combo); var b = new Button { Text = "OK", Dock = DockStyle.Bottom, DialogResult = DialogResult.OK }; Controls.Add(b); AcceptButton = b; }
}
