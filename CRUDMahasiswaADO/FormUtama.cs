using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormUtama : Form
    {
        public FormUtama()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            BuildMenu();
        }

        private void BuildMenu()
        {
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem mnuData = new ToolStripMenuItem("Data Master");
            ToolStripMenuItem mnuMhs = new ToolStripMenuItem("Kelola Mahasiswa");
            ToolStripMenuItem mnuDosen = new ToolStripMenuItem("Kelola Dosen");
            ToolStripMenuItem mnuJadwal = new ToolStripMenuItem("Kelola Jadwal Dosen");
            ToolStripMenuItem mnuPertemuan = new ToolStripMenuItem("Kelola Jadwal Pertemuan");
            ToolStripMenuItem mnuReport = new ToolStripMenuItem("Report Pertemuan");
            ToolStripMenuItem mnuKeluar = new ToolStripMenuItem("Keluar");

            mnuMhs.Click += (s, e) => BukaForm(new FormMahasiswa());
            mnuDosen.Click += (s, e) => BukaForm(new FormDosen());
            mnuJadwal.Click += (s, e) => BukaForm(new Form());
            mnuPertemuan.Click += (s, e) => BukaForm(new JadwalPertemuan());
            mnuReport.Click += (s, e) => BukaForm(new MelihatReport());
            mnuKeluar.Click += (s, e) => {
                if (MessageBox.Show("Yakin ingin keluar?", "Konfirmasi",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Application.Exit();
            };

            mnuData.DropDownItems.AddRange(new ToolStripItem[] {
                mnuMhs, mnuDosen, mnuJadwal, mnuPertemuan, mnuReport,
                new ToolStripSeparator(), mnuKeluar
            });

            menu.Items.Add(mnuData);
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }

        private void BukaForm(Form form)
        {
            form.MdiParent = this;
            form.Show();
        }

        private void FormUtama_Load(object sender, EventArgs e)
        {

        }
    }
}
