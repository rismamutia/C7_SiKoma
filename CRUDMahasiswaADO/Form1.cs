using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace CRUDMahasiswaADO
{
    public partial class FormDosen : Form
    {
        int SelectedID = 0;
        int SelectedDosenID = 0;

        public int userID;
        public string nama;
        public string email;
        public string role;

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=.;Initial Catalog=DBJadwalKoordinasi;Integrated Security=True";
        public FormDosen()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += dataGridView1_CellClick;

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
                {
                    conn.Open();
                }

                string query = @"
                            SELECT j.JadwalID, j.DosenID, d.NIDN, d.Nama, 
                                   j.Tanggal, j.WaktuMulai, j.WaktuSelesai, j.Status
                            FROM JadwalDosen j
                            JOIN Dosen d ON j.DosenID = d.DosenID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                // VALIDASI
                if (txtNIDN.Text.Trim() == "")
                {
                    MessageBox.Show("NIDN harus diisi");
                    txtNIDN.Focus();
                    return;
                }

                if (txtNama.Text.Trim() == "")
                {
                    MessageBox.Show("Nama harus diisi");
                    txtNama.Focus();
                    return;
                }

                if (cmbStatus.Text == "")
                {
                    MessageBox.Show("Status harus diisi");
                    cmbStatus.Focus();
                    return;
                }

                // =========================
                // 1. INSERT / CEK DOSEN
                // =========================
                string checkDosen = "SELECT DosenID FROM Dosen WHERE NIDN = @NIDN";
                SqlCommand cmdCheck = new SqlCommand(checkDosen, conn);
                cmdCheck.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());

                object resultCheck = cmdCheck.ExecuteScalar();

                int dosenID;

                if (resultCheck != null)
                {
                    dosenID = Convert.ToInt32(resultCheck);
                }
                else
                {
                    string insertDosen = @"
            INSERT INTO Dosen (NIDN, Nama)
            OUTPUT INSERTED.DosenID
            VALUES (@NIDN, @Nama)";

                    SqlCommand cmdDosen = new SqlCommand(insertDosen, conn);
                    cmdDosen.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());
                    cmdDosen.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());

                    dosenID = (int)cmdDosen.ExecuteScalar();
                }

                // =========================
                // 2. INSERT JADWAL
                // =========================
                string queryJadwal = @"
        INSERT INTO JadwalDosen 
        (DosenID, Tanggal, WaktuMulai, WaktuSelesai, Status)
        VALUES (@DosenID, @Tanggal, @Mulai, @Selesai, @Status)";

                SqlCommand cmd = new SqlCommand(queryJadwal, conn);
                cmd.Parameters.AddWithValue("@DosenID", dosenID);
                cmd.Parameters.AddWithValue("@Tanggal", dtpTanggalKetersediaan.Value.Date);
                cmd.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Jadwal Dosen berhasil ditambahkan");
                    ClearForm();
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan data");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validasi: pastikan baris sudah dipilih
                if (SelectedID == 0)
                {
                    MessageBox.Show("Pilih data di tabel terlebih dahulu!");
                    return;
                }

                // Validasi input
                if (txtNIDN.Text.Trim() == "")
                {
                    MessageBox.Show("NIDN harus diisi");
                    txtNIDN.Focus();
                    return;
                }

                if (txtNama.Text.Trim() == "")
                {
                    MessageBox.Show("Nama harus diisi");
                    txtNama.Focus();
                    return;
                }

                if (cmbStatus.Text == "")
                {
                    MessageBox.Show("Status harus diisi");
                    cmbStatus.Focus();
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // UPDATE tabel Dosen (Nama dan NIDN)
                string queryDosen = @"
            UPDATE Dosen
            SET NIDN = @NIDN,
                Nama = @Nama
            WHERE DosenID = @DosenID";

                SqlCommand cmdDosen = new SqlCommand(queryDosen, conn);
                cmdDosen.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());  // pakai .Text
                cmdDosen.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());  // pakai .Text
                cmdDosen.Parameters.AddWithValue("@DosenID", SelectedDosenID);   // ID dosen yang dipilih
                cmdDosen.ExecuteNonQuery();

                // UPDATE tabel JadwalDosen (Tanggal, WaktuMulai, WaktuSelesai, Status)
                string queryJadwal = @"
            UPDATE JadwalDosen
            SET Tanggal      = @Tanggal,
                WaktuMulai   = @Mulai,
                WaktuSelesai = @Selesai,
                Status       = @Status
            WHERE JadwalID = @JadwalID";

                SqlCommand cmdJadwal = new SqlCommand(queryJadwal, conn);
                cmdJadwal.Parameters.AddWithValue("@Tanggal", dtpTanggalKetersediaan.Value.Date);
                cmdJadwal.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmdJadwal.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmdJadwal.Parameters.AddWithValue("@Status", cmbStatus.Text);
                cmdJadwal.Parameters.AddWithValue("@JadwalID", SelectedID);

                int result = cmdJadwal.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Jadwal Dosen berhasil diperbarui!");
                    ClearForm();
                    SelectedID = 0;
                    SelectedDosenID = 0;
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui data. Pastikan data sudah dipilih.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();

                    if (SelectedID == 0)
                    {
                        MessageBox.Show("Pilih data dulu di tabel!");
                        return;
                    }
                }

                DialogResult resultConfirm = MessageBox.Show(
                    "Apakah Anda yakin ingin menghapus data ini?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM JadwalDosen WHERE JadwalID = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", SelectedID);
                    cmd.Parameters.AddWithValue("@tgl", dtpTanggalKetersediaan.Value.Date);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data dosen berhasil dihapus");
                        ClearForm();
                        btnLoad.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menghapus data");
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                SelectedID = Convert.ToInt32(row.Cells["JadwalID"].Value);
                SelectedDosenID = Convert.ToInt32(row.Cells["DosenID"].Value);

                txtNIDN.Text = row.Cells["NIDN"].Value?.ToString();
                txtNama.Text = row.Cells["Nama"].Value?.ToString();
                cmbStatus.Text = row.Cells["Status"].Value?.ToString();

                if (row.Cells["Tanggal"].Value != DBNull.Value)
                    dtpTanggalKetersediaan.Value = Convert.ToDateTime(row.Cells["Tanggal"].Value);

                if (row.Cells["WaktuMulai"].Value != DBNull.Value)
                    dtpWaktuMulai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuMulai"].Value);

                if (row.Cells["WaktuSelesai"].Value != DBNull.Value)
                    dtpWaktuSelesai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuSelesai"].Value);

                txtNIDN.Text = row.Cells["NIDN"].Value?.ToString();

            }
        }

        private void ClearForm()
        {
            txtNIDN.Clear();
            txtNama.Clear();
            dtpTanggalKetersediaan.Value = DateTime.Now;
            dtpWaktuMulai.Value = DateTime.Now;
            dtpWaktuSelesai.Value = DateTime.Now;
            cmbStatus.SelectedIndex = -1;
        }

        private void FormDosen_Load(object sender, EventArgs e)
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Tersedia");
            cmbStatus.Items.Add("Tidak Tersedia");
            cmbStatus.Items.Add("Terisi");

            cmbStatus.SelectedIndex = 0;


            dtpTanggalKetersediaan.Value = DateTime.Now;
            dtpWaktuMulai.Value = DateTime.Now;
            dtpWaktuSelesai.Value = DateTime.Now;
        }

        private void cmbJK_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void FormDosen_Load_1(object sender, EventArgs e)
        {

        }
    }
}