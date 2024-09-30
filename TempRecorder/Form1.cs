using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProiectPNS
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private Timer timer = new Timer();
        private string connectionString = "Data Source=DESKTOP-U3BQF73\\SQLEXPRESS;Initial Catalog=BazaDateTutorial;Integrated Security=True;Encrypt=False";
        public Form1()
        {
            InitializeComponent();

            chart1.Series.Add("Temperatura");
            chart1.Series["Temperatura"].ChartType = SeriesChartType.Line;
            chart1.Series["Temperatura"].BorderWidth = 2;

            timer.Interval = 1000; 
            timer.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            double temperatura = random.Next(15, 31);

            InsertTemperatureIntoDatabase(temperatura);

            int temperaturaRotunjita = (int)Math.Round(temperatura);

            chart1.Series["Temperatura"].Points.AddY(temperaturaRotunjita);
            LoadDataIntoDataGridView();

            chart1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }
        private void InsertTemperatureIntoDatabase(double temperatura)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO TemperatureData (Temperatura, Data) VALUES (@Temperatura, @Data)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Temperatura", temperatura);
                command.Parameters.AddWithValue("@Data", DateTime.Now);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la inserarea datelor în baza de date: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }
        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Temperatura, Data FROM TemperatureData";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la încărcarea datelor în DataGridView: " + ex.Message);
                }
            }
        }
        private void DeleteAllDataFromDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM TemperatureData";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Datele au fost sterse cu succes.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la ștergerea datelor din baza de date: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteAllDataFromDatabase();
            LoadDataIntoDataGridView();
        }
    }
}
