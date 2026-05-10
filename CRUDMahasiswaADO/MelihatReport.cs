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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CRUDMahasiswaADO
{
    public partial class MelihatReport : Form
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";

        private readonly SqlConnection conn;

        public MelihatReport()
        {
            InitializeComponent();
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
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
            System.Drawing.Color.AliceBlue;
        }

        private void LoadReport(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT
                    p.PertemuanID  AS [ID],
                    m.NIM          AS [NIM],
                    m.Nama         AS [Nama Mahasiswa],
                    d.Nama         AS [Nama Dosen],
                    j.Tanggal      AS [Tanggal],
                    j.WaktuMulai   AS [Jam Mulai],
                    j.WaktuSelesai AS [Jam Selesai],
                    p.Status       AS [Status],
                    p.CatatanPermintaan AS [Catatan]
                FROM Pertemuan p
                JOIN Mahasiswa   m ON p.MahasiswaID = m.MahasiswaID
                JOIN JadwalDosen j ON p.JadwalID    = j.JadwalID
                JOIN Dosen       d ON j.DosenID     = d.DosenID
                WHERE
                    m.Nama   LIKE @kw OR
                    d.Nama   LIKE @kw OR
                    m.NIM    LIKE @kw OR
                    p.Status LIKE @kw
                ORDER BY j.Tanggal DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns["ID"] != null)
                        dataGridView1.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load report: " + ex.Message);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadReport();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MelihatReport_Load_1(object sender, EventArgs e)
        {
            SetupGrid();
            LoadReport();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.AliceBlue;
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            LoadReport(txtSearch.Text.Trim());
        }
    }
}
