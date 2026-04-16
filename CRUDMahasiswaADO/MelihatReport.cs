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

namespace CRUDMahasiswaADO
{
    public partial class MelihatReport : Form
    {
        private readonly string connectionString =
            "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";

        private readonly SqlConnection conn;

        public MelihatReport()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }


        private void MelihatReport_Load(object sender, EventArgs e)
        {
            LoadReport();
            SetupGrid();
        }

        private void SetupGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadReport()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"
                    SELECT 
                        p.PertemuanID,
                        m.Nama AS NamaMahasiswa,
                        p.JadwalID,
                        p.Status,
                        p.TanggalPermintaan,
                        p.CatatanPermintaan
                    FROM Pertemuan p
                    JOIN Mahasiswa m ON p.MahasiswaID = m.MahasiswaID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load report: " + ex.Message);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
