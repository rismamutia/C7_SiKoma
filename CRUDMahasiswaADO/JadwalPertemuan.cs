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
    public partial class JadwalPertemuan : Form
    {
        private readonly string connectionString =
     "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";

        private readonly SqlConnection conn;

        public JadwalPertemuan()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
        }

        private void LoadData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "SELECT * FROM Pertemuan";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load gagal: " + ex.Message);
            }
        }
        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MessageBox.Show("Koneksi berhasil");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"SELECT * FROM Pertemuan";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"INSERT INTO Pertemuan 
                (JadwalID, MahasiswaID, Status, CatatanPermintaan)
                VALUES
                (@JadwalID, @MahasiswaID, @Status, @Catatan)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@JadwalID", txtJadwalID.Text);
                cmd.Parameters.AddWithValue("@MahasiswaID", txtMahasiswaID.Text);
                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text); // Pending default
                cmd.Parameters.AddWithValue("@Catatan", txtCatatan.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Permintaan berhasil dibuat");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Insert gagal: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"UPDATE Pertemuan
                                 SET Status = @Status
                                 WHERE PertemuanID = @ID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);
                cmd.Parameters.AddWithValue("@ID", txtPertemuanID.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Status berhasil diupdate");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update gagal: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "DELETE FROM Pertemuan WHERE PertemuanID = @ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", txtPertemuanID.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Data berhasil dihapus");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete gagal: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtPertemuanID.Text = row.Cells["PertemuanID"].Value.ToString();
                txtJadwalID.Text = row.Cells["JadwalID"].Value.ToString();
                txtMahasiswaID.Text = row.Cells["MahasiswaID"].Value.ToString();
                cmbStatus.Text = row.Cells["Status"].Value.ToString();
                txtCatatan.Text = row.Cells["CatatanPermintaan"].Value.ToString();
            }
        }
    }
}
