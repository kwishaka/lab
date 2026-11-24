using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Views;

public sealed class LoginForm : Form
{
    private readonly TextBox username = new(), password = new();
    private readonly Repository repository = new();

    public LoginForm()
    {
        Text = "Bicycle Rental Management - Login"; Width = 420; Height = 260; StartPosition = FormStartPosition.CenterScreen;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), RowCount = 4, ColumnCount = 2 };
        layout.Controls.Add(new Label { Text = "Username", AutoSize = true }, 0, 0); layout.Controls.Add(username, 1, 0);
        layout.Controls.Add(new Label { Text = "Password", AutoSize = true }, 0, 1); layout.Controls.Add(password, 1, 1); password.UseSystemPasswordChar = true;
        var login = new Button { Text = "Log in", Width = 100 }; login.Click += Login;
        layout.Controls.Add(login, 1, 2); AcceptButton = login;
        layout.Controls.Add(new Label { Text = "Demo: admin/admin123 or attendant/attendant123", AutoSize = true, ForeColor = Color.Gray }, 0, 3); layout.SetColumnSpan(layout.GetControlFromPosition(0, 3)!, 2);
        Controls.Add(layout);
    }

    private void Login(object? sender, EventArgs e)
    {
        var user = repository.Authenticate(username.Text.Trim(), password.Text);
        if (user is null) { MessageBox.Show("Invalid username or password.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error); password.Clear(); return; }
        Hide(); using (var dashboard = new DashboardForm(user)) dashboard.ShowDialog(); Show(); password.Clear();
    }
}
