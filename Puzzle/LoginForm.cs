using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class LoginForm : Form
    {
        public string Username { get; private set; }
        public int UserID { get; private set; }

        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;

        public LoginForm()
        {
            InitializeComponent();
            InitializeLoginForm();
        }

        private void InitializeLoginForm()
        {
            this.Text = "Вход / Регистрация";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var usernameLabel = new Label
            {
                Text = "Имя пользователя:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            usernameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(160, 18),
                Width = 200,
                Font = new Font("Arial", 10)
            };

            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new System.Drawing.Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            passwordTextBox = new TextBox
            {
                Location = new System.Drawing.Point(160, 58),
                Width = 200,
                Font = new Font("Arial", 10),
                UseSystemPasswordChar = true
            };

            loginButton = new Button
            {
                Text = "Войти / Зарегистрироваться",
                Location = new System.Drawing.Point(20, 100),
                Width = 340,
                Height = 40,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            loginButton.Click += LoginButton_Click;

            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(loginButton);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=puzzleGame;Integrated Security=True;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, есть ли пользователь
                string query = "SELECT UserID, PasswordHash FROM Users WHERE Username=@Username";
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["PasswordHash"].ToString();
                            int id = Convert.ToInt32(reader["UserID"]);

                            if (storedPassword == password)
                            {
                                UserID = id;
                                Username = username;
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Неверный пароль!");
                            }
                            return;
                        }
                    }
                }

                // Если пользователь не найден — регистрация
                string insertQuery = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @Password); SELECT SCOPE_IDENTITY();";
                using (var insertCmd = new SqlCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@Password", password);
                    UserID = Convert.ToInt32(insertCmd.ExecuteScalar());
                    Username = username;
                    MessageBox.Show("Пользователь зарегистрирован!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}