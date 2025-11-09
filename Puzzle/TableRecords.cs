using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class TableRecords : Form
    {
        private DataGridView recordsGrid;
        private string databaseName = "puzzleGame";

        public TableRecords()
        {
            InitializeComponent();
            InitializeRecordsTable();
        }

        private void InitializeRecordsTable()
        {
            this.Text = "Таблица рекордов";
            this.Size = new Size(900, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            recordsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            this.Controls.Add(recordsGrid);

            LoadRecords();
        }

        private void LoadRecords()
        {
            string connectionString =
                $@"Server=(localdb)\MSSQLLocalDB;Database={databaseName};
                   Integrated Security=True;TrustServerCertificate=True;";

            string query = "SELECT * FROM RecordsView";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        recordsGrid.DataSource = dt;

                        if (recordsGrid.Columns.Contains("RecordID"))
                            recordsGrid.Columns["RecordID"].HeaderText = "ID записи";
                        if (recordsGrid.Columns.Contains("Username"))
                            recordsGrid.Columns["Username"].HeaderText = "Игрок";
                        if (recordsGrid.Columns.Contains("Title"))
                            recordsGrid.Columns["Title"].HeaderText = "Название пазла";
                        if (recordsGrid.Columns.Contains("Score"))
                            recordsGrid.Columns["Score"].HeaderText = "Очки";
                        if (recordsGrid.Columns.Contains("TimeSpent"))
                            recordsGrid.Columns["TimeSpent"].HeaderText = "Время (сек)";
                        if (recordsGrid.Columns.Contains("MovesCount"))
                            recordsGrid.Columns["MovesCount"].HeaderText = "Количество ходов";
                        if (recordsGrid.Columns.Contains("DifficultyLevel"))
                            recordsGrid.Columns["DifficultyLevel"].HeaderText = "Сложность";
                        if (recordsGrid.Columns.Contains("RecordDate"))
                            recordsGrid.Columns["RecordDate"].HeaderText = "Дата";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при загрузке данных:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}