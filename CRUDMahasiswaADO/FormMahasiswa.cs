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
    public partial class FormMahasiswa : Form
    {
        private int selectedID = 0;
        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";
        public FormMahasiswa()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
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
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("NIM", "NIM");
                dataGridView1.Columns.Add("Nama", "Nama");
                dataGridView1.Columns.Add("Email", "Email");

                string query = "SELECT * FROM Mahasiswa";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["NIM"].ToString(),
                        reader["Nama"].ToString(),
                        reader["Email"].ToString()
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Mahasiswa (NIM, Nama, Email) " +
                                   "VALUES (@nim, @nama, @email)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nim", txtNIM.Text.Trim());
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data mahasiswa berhasil ditambahkan!");
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            { MessageBox.Show("Pilih data di tabel dulu!"); return; }
            if (!Validasi()) return;
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Mahasiswa SET NIM=@nim, Nama=@nama, " +
                                   "Email=@email WHERE MahasiswaID=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nim", txtNIM.Text.Trim());
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", selectedID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data mahasiswa berhasil diperbarui!");
                    ClearForm();
                    LoadData();
                }
            }
            
         
        
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            { MessageBox.Show("Pilih data di tabel dulu!"); return; }
            if (MessageBox.Show("Yakin hapus data ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Mahasiswa WHERE MahasiswaID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", selectedID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data mahasiswa berhasil dihapus!");
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            selectedID = Convert.ToInt32(row.Cells["MahasiswaID"].Value);
            txtNIM.Text = row.Cells["NIM"].Value?.ToString();
            txtNama.Text = row.Cells["Nama"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
        }
        

        private void ClearForm()
        {
            txtNIM.Clear(); txtNama.Clear(); txtEmail.Clear();
            selectedID = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.CellClick += dataGridView1_CellContentClick;
        }

        private void FormMahasiswaFix_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT MahasiswaID, NIM, Nama, Email 
                                     FROM Mahasiswa
                                     WHERE Nama LIKE @kw OR NIM LIKE @kw";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["MahasiswaID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

             private bool Validasi()
        {
            if (txtNIM.Text.Trim() == "")
            { MessageBox.Show("NIM harus diisi!"); txtNIM.Focus(); return false; }
            if (txtNama.Text.Trim() == "")
            { MessageBox.Show("Nama harus diisi!"); txtNama.Focus(); return false; }
            if (txtEmail.Text.Trim() == "")
            { MessageBox.Show("Email harus diisi!"); txtEmail.Focus(); return false; }
            return true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtSearch.Clear();
        }
    }
    }

