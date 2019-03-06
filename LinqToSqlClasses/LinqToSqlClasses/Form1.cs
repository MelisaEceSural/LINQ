using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqToSqlClasses
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NorthwindDataContext data = new NorthwindDataContext();
        private void Form1_Load(object sender, EventArgs e)
        {
            //NorthwindDataContext data = new NorthwindDataContext();
            dataGridView1.DataSource = data.Urunlers.Where(x=>x.Sonlandi==false);// burada bir select sorgusu çalıştı.

            #region Joinli Hali
            //dataGridView1.DataSource = (from urun in data.Urunlers
            //                           join kategori in data.Kategorilers
            //                           on urun.KategoriID equals kategori.KategoriID
            //                           join tedarikci in data.Tedarikcilers
            //                           on urun.TedarikciID equals tedarikci.TedarikciID
            //                            select new
            //                            {
            //                                ID = urun.UrunID,
            //                                Adi = urun.UrunAdi,
            //                                Fiyat = urun.BirimFiyati,
            //                                Stok = urun.HedefStokDuzeyi,
            //                                Kategori = kategori.KategoriAdi,
            //                                Tedarikci = tedarikci.SirketAdi
            //                            }).OrderBy(x => x.Adi); 
            #endregion

            cmbKategori.DataSource = data.Kategorilers;
            cmbKategori.DisplayMember = "KategoriAdi";
            cmbKategori.ValueMember = "KategoriID";

            cmbTedarikci.DataSource = data.Tedarikcilers;
            cmbTedarikci.DisplayMember = "SirketAdi";
            cmbTedarikci.ValueMember = "TedarikciID";
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtUrunAdii.Text) || 
                numFiyat.Value==0 || numStok.Value == 0)
            {
                MessageBox.Show("Lütfen alanları boş bırakmayın.");
                return;
            }


            Urunler yeniUrun = new Urunler();
            yeniUrun.UrunAdi = txtUrunAdii.Text;
            yeniUrun.BirimFiyati = numFiyat.Value;
            yeniUrun.HedefStokDuzeyi = (short)numStok.Value;
            yeniUrun.KategoriID = (int)cmbKategori.SelectedValue;
            yeniUrun.TedarikciID = (int)cmbTedarikci.SelectedValue;

            //önce dataContext'e yeni ürünü ekleyelim
            //NorthwindDataContext data = new NorthwindDataContext();
            data.Urunlers.InsertOnSubmit(yeniUrun);
            // yeni ürünü contexte ekledik, şimdi de veritabanına yazalım.
            data.SubmitChanges();

            //değişikliği görmek için ürünleri tekrar listeleyelim
            dataGridView1.DataSource = data.Urunlers.Where(x => x.Sonlandi == false);
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            //NorthwindDataContext data = new NorthwindDataContext();
            int id = (int)dataGridView1.CurrentRow.Cells["UrunID"].Value;
            Urunler silinecekUrun = data.Urunlers.SingleOrDefault(x => x.UrunID == id);
            data.Urunlers.DeleteOnSubmit(silinecekUrun);
            data.SubmitChanges();
            dataGridView1.DataSource = data.Urunlers.Where(x => x.Sonlandi == false);
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = data.Urunlers.Where(x => x.UrunAdi.Contains(txtAra.Text));
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow satir = dataGridView1.CurrentRow;
            txtUrunAdii.Text = satir.Cells["UrunAdi"].Value.ToString();
            txtUrunAdii.Tag = (int)satir.Cells["UrunID"].Value;
            numFiyat.Value = (decimal)satir.Cells["BirimFiyati"].Value;
            numStok.Value = (short)satir.Cells["HedefStokDuzeyi"].Value;
            cmbKategori.SelectedValue = (int)satir.Cells["KategoriID"].Value;
            cmbTedarikci.SelectedValue = (int)satir.Cells["TedarikciID"].Value;
            
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if(txtUrunAdii.Tag==null)
            {
                MessageBox.Show("Güncellenecek ürün bulunamadı.");
                return;
            }

            int id = (int)txtUrunAdii.Tag;
            Urunler urun = data.Urunlers.SingleOrDefault(x => x.UrunID == id);
            urun.UrunAdi = txtUrunAdii.Text;
            urun.BirimFiyati = numFiyat.Value;
            urun.HedefStokDuzeyi = (short)numStok.Value;
            urun.KategoriID = (int)cmbKategori.SelectedValue;
            urun.TedarikciID = (int)cmbTedarikci.SelectedValue;

            data.SubmitChanges();

            dataGridView1.DataSource = data.Urunlers.Where(x => x.Sonlandi == false);
            txtUrunAdii.Tag = null;
        }
    }
}
