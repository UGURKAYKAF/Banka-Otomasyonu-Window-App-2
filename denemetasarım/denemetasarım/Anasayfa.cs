using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Bunifu.UI.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using iTextSharp.text.pdf;//pdf ekleme
using iTextSharp.text;
using System.IO;

namespace denemetasarım
{
    public partial class Anasayfa : Form
    {

        SqlConnection baglanti = giris.baglanti;

        public Anasayfa()
        {
            InitializeComponent();

            bunifuFormDock1.SubscribeControlToDragEvents(panel1);//panel kontrol


            label2.Text = GetRate("USD").ToString();

            label3.Text = GetRate("EUR").ToString();

            label4.Text = GetRate("GBP").ToString();

            label5.Text = GetRate("CNY").ToString();

            label6.Text = GetRate("RUB").ToString();

            label23.Text = GetRate("USD").ToString();
            label22.Text = GetRate("EUR").ToString();
            label20.Text = GetRate("CNY").ToString();
            label21.Text = GetRate("GBP").ToString();
            label19.Text = GetRate("RUB").ToString();






        }
        public double EklenmisBakiye;//güncellenen bakiyeyi getirmek için
        public double GüncelBakiye; // vt deki bakiyeye göre parra yollama kontrolü
        public bool isthere;
        public void BakiyeEkle()
        {

            try
            {
                baglanti.Open();
                SqlCommand BakiyeEkle = new SqlCommand("Update  login_register  set bakiye+= ('" + BakiyeGirtxt.Text + "' )where password=@deger;", baglanti);
                BakiyeEkle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                BakiyeEkle.ExecuteNonQuery();//birden fazla veriyle işlem yapmak için
                if (string.IsNullOrEmpty(BakiyeGirtxt.Text))
                {
                    // MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                    errorProvider1.SetError(BakiyeGirtxt, "Lütfen Geçerli bir değer giriniz !!");


                }
                else if (BakiyeGirtxt.Text == "0")
                {
                    errorProvider1.Clear();
                    // MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                    errorProvider1.SetError(BakiyeGirtxt, "Lütfen Geçerli bir değer giriniz !!");
                }
                else
                {
                    MessageBox.Show("Para yatırma işleminiz başarılı", "Tekbirkler!!");
                    errorProvider1.Clear();


                    SqlCommand YeniBakiye = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                    YeniBakiye.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader YeniBakiyeOku = YeniBakiye.ExecuteReader();//tablodaki tm değerli okur

                    while (YeniBakiyeOku.Read())
                    {
                        EklenmisBakiye = Convert.ToDouble(YeniBakiyeOku["bakiye"]);
                    }

                    label11.Text = EklenmisBakiye.ToString() + " " + "TL";


                    // listBox1.Items.Add("Tarih:"+""+dt+"\n"+"YATIRILAN TUTAR:" + "" + bunifuTextBox1.Text+"TL" + "\n" + "GÜNCEL BAKİYENİZ:" + "" + label11.Text+"TL"+"\n");
                    //listBox1.Items.Add("Tarih:" + " " + dt);
                    TarihSaatEkleme();
                    listBox1.Items.Add("Yatırılan Tutar:" + " " + BakiyeGirtxt.Text + "TL");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");
                }


            }
            catch (Exception)
            {
                errorProvider1.Clear();
                // MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                errorProvider1.SetError(BakiyeGirtxt, "Lütfen Geçerli bir değer giriniz !!");


            }
            finally { baglanti.Close(); }

        }
        public void ibanaYolla()
        {

            baglanti.Open();
            SqlCommand Bakiye = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
            Bakiye.Parameters.AddWithValue("@deger", giris.statikButon.Text);
            SqlDataReader BakiyeOku = Bakiye.ExecuteReader();//tablodaki tm değerli okur

            while (BakiyeOku.Read())
            {
                GüncelBakiye = Convert.ToDouble(BakiyeOku["bakiye"]);
            }
            baglanti.Close();


            baglanti.Open();


            if (string.IsNullOrEmpty(ibn_bunifuTextBox7.Text) || string.IsNullOrEmpty(bnf_btnParayolla.Text))
            {
                errorProvider1.SetError(ibn_bunifuTextBox7, "Lütfen Geçerli bir değer giriniz !!");
                errorProvider1.SetError(bnf_btnParayolla, "Lütfen Geçerli bir değer giriniz !!");
            }
            else
            {
                int ibanim = Convert.ToInt32(ibn_bunifuTextBox7.Text);
                SqlCommand iban = new SqlCommand("Select iban from login_register;", baglanti);
                SqlDataReader ibn = iban.ExecuteReader();//tablodaki tm değerli okur


                while (ibn.Read())
                {
                    if (ibanim == Convert.ToInt32(ibn["iban"]))
                    {
                        isthere = true;
                        break;
                    }
                    else if (!(ibanim == Convert.ToInt32(ibn["iban"])))
                    {
                        isthere = false;
                    }


                }
            }




            // ibn.Close();    
            baglanti.Close();


            baglanti.Open();

            if (isthere && GüncelBakiye >= Convert.ToDouble(bnf_btnParayolla.Text))//iban ve bakiye büyüklüğü kontrolü
            {

                SqlCommand ParaYolla = new SqlCommand("Update  login_register  set bakiye-= ('" + bnf_btnParayolla.Text + "' )where password=@deger;", baglanti);
                ParaYolla.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                ParaYolla.ExecuteNonQuery();

                SqlCommand paraCek = new SqlCommand("Update  login_register  set bakiye+= ('" + bnf_btnParayolla.Text + "' )where iban=@iban;", baglanti);
                paraCek.Parameters.AddWithValue("@iban", ibn_bunifuTextBox7.Text);
                paraCek.ExecuteNonQuery();

                MessageBox.Show("para gönderme işleminiz başarılı");
                errorProvider1.Clear();


            }
            else if (string.IsNullOrEmpty(ibn_bunifuTextBox7.Text) || string.IsNullOrEmpty(bnf_btnParayolla.Text))
            {
                MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                /*errorProvider1.SetError(ibn_bunifuTextBox7, "Lütfen Geçerli bir değer giriniz !!");
                errorProvider1.SetError(bnf_btnParayolla, "Lütfen Geçerli bir değer giriniz !!");*/



            }


            else if (isthere == false) //iban hatalıysa
            {

                MessageBox.Show("iban hatalı", "İban yhatalı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            }

            else//geriye kalan tek hata
            {
                MessageBox.Show("Bakiyeniz para yollamanız için yetersizdir lütfen tekrar deneyiniz", "Bakiye yetersiz!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            SqlCommand YeniBakiye = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
            YeniBakiye.Parameters.AddWithValue("@deger", giris.statikButon.Text);
            SqlDataReader YeniBakiyeOku = YeniBakiye.ExecuteReader();//tablodaki tm değerli okur

            while (YeniBakiyeOku.Read())
            {
                EklenmisBakiye = Convert.ToDouble(YeniBakiyeOku["bakiye"]);
            }


            label11.Text = EklenmisBakiye.ToString() + " " + "TL";
            try
            {

                if (GüncelBakiye >= Convert.ToDouble(bnf_btnParayolla.Text))
                {
                    TarihSaatEkleme();
                    listBox1.Items.Add("Gönderilen Tutar:" + " " + bnf_btnParayolla.Text + "TL");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");
                }
            }
            catch (Exception)
            {
                TarihSaatEkleme();
                listBox1.Items.Add("Yetersiz bakiye işlemi:" + " " + bnf_btnParayolla.Text + "TL");
                listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                listBox1.Items.Add("----------------------------------------------");
            }






            baglanti.Close();


        }
        public void PdfOlustur()
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "PDF DOSYALARI(*.pdf)|*.pdf";
            file.Title = "İSLEMLER";
            if (file.ShowDialog() == DialogResult.OK)
            {
                FileStream dosya = File.Open(file.FileName, FileMode.Create);
                Document pdf = new Document();
                PdfWriter.GetInstance(pdf, dosya);
                pdf.Open();
                pdf.AddAuthor("Ciftlik Bank");
                pdf.AddCreator("Serhat KILIÇ CEO");
                pdf.AddTitle("Dökümasyon");
                pdf.AddSubject("İşlem dökümasyonunuz");
                pdf.AddCreationDate();

                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    Paragraph paragraph = new Paragraph(listBox1.Items[i].ToString());
                    pdf.Add(paragraph);

                }




                pdf.Close();
                MessageBox.Show("PDF Oluşturuldu");
            }
        }

        public void ResimGetir()
        {
            try
            {
                baglanti.Open();
                SqlCommand YeniResimGetir = new SqlCommand("Select resim from login_register where password= @deger", baglanti);
                YeniResimGetir.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                SqlDataReader YeniResimOku = YeniResimGetir.ExecuteReader();//tablodaki tm değerli okur

                while (YeniResimOku.Read())
                {
                    //system.drawing eklenmesinin sebebi using iTextSharp.text; kütüphanesi eklendiğinde hata oluştu nereden erişmemiz gerektiğini belirledik
                    bunifuPictureBox6.Image = System.Drawing.Image.FromFile(YeniResimOku["resim"].ToString()); //veritabanında resim kolonu boş olduğunda giriş yapılmıyor hata veriyor //bir resim uzantısı olduğunda başarıyle giriş yapılabiliyor
                    bunifuPictureBox7.Image = System.Drawing.Image.FromFile(YeniResimOku["resim"].ToString());
                }
            }
            catch (Exception)
            {


            }
            finally { baglanti.Close(); }

        }

        public void SifreGüncelle()
        {
            if (!string.IsNullOrEmpty(bunifuTextBox4.Text) && !string.IsNullOrEmpty(bunifuTextBox5.Text) && (bunifuTextBox4.Text == giris.statikButon.Text))
            {
                baglanti.Open();
                SqlCommand YeniSifre = new SqlCommand("Update login_register set password=@sifre1,re_password=@sifre2 where password=@deger AND password=@deger2;", baglanti);
                YeniSifre.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                YeniSifre.Parameters.AddWithValue("@deger2", bunifuTextBox4.Text);
                YeniSifre.Parameters.AddWithValue("@sifre1", bunifuTextBox5.Text);
                YeniSifre.Parameters.AddWithValue("@sifre2", bunifuTextBox5.Text);
                YeniSifre.ExecuteNonQuery();
                MessageBox.Show("Şifreniz başarıyla değiştirilmiştir");
                errorProvider1.Clear();

            }
            else if (!(bunifuTextBox4.Text == giris.statikButon.Text))
            {
                errorProvider1.SetError(bunifuTextBox4, "Lütfen eski şifrenizi giriniz!!");

            }
            else if (bunifuTextBox5.Text == "")
            {
                errorProvider1.SetError(bunifuTextBox5, "Lütfen Geçerli bir Şifre değeri giriniz !!");

            }
            else
            {
                errorProvider1.SetError(bunifuTextBox4, "Lütfen eski şifrenizi giriniz!!");
                errorProvider1.SetError(bunifuTextBox5, "Lütfen Geçerli bir Şifre değeri giriniz !!");

            }

            baglanti.Close();

        }


        public double bakiye;




        public void BakiyeGetir()
        {

            try
            {

                baglanti.Open();//bu kısımda lbel 1 ve label 7 ye anasayfada kullanıcı sifresine göre ad soyad getirtmeyi yazıyyoruz ve Id
                SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger", baglanti);
                BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text); //giriş formunda static buton degeri bunifuTextBox2'ydi bunun textini alıp girilen değere göre
                                                                                     //veritabanından veri aldık yani kimin şifresi ise o kişinin değerlerini çekebiliyoruz
                SqlDataReader oku = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                while (oku.Read())
                {
                    bakiye = Convert.ToDouble(oku["bakiye"]);
                }

                label11.Text = bakiye.ToString() + " " + "TL";

            }
            catch (Exception)
            {

            }
            finally { baglanti.Close(); }

        }

        public void PostaTelUpdate()
        {
            if (!string.IsNullOrEmpty(bunifuTextBox6.Text) && !string.IsNullOrEmpty(bunifuTextBox3.Text))
            {
                errorProvider1.Clear();
                baglanti.Open();
                SqlCommand YeniTelPosta = new SqlCommand("Update login_register set email=@mail,phone=@number where password=@deger;", baglanti);
                YeniTelPosta.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                YeniTelPosta.Parameters.AddWithValue("@mail", bunifuTextBox6.Text);
                YeniTelPosta.Parameters.AddWithValue("@number", bunifuTextBox3.Text);
                YeniTelPosta.ExecuteNonQuery();
                MessageBox.Show("Güncelleme işleminiz başarılı");
            }
            else if (bunifuTextBox6.Text == "")
            {
                errorProvider1.SetError(bunifuTextBox6, "Lütfen Geçerli bir değer giriniz !!");

            }
            else if (bunifuTextBox3.Text == "")
            {
                errorProvider1.SetError(bunifuTextBox3, "Lütfen Geçerli bir değer giriniz !!");

            }
            else
            {
                errorProvider1.SetError(bunifuTextBox6, "Lütfen Geçerli bir değer giriniz!!");
                errorProvider1.SetError(bunifuTextBox3, "Lütfen Geçerli bir değer giriniz!!");

            }

            baglanti.Close();

        }


        private string addZero(int p)
        {
            if (p.ToString().Length == 1)
                return "0" + p;
            return p.ToString();
        }

        private decimal GetRate(string code)
        {
            string url = string.Empty;
            var date = DateTime.Now;
            if (date.Date == DateTime.Today)
                url = "http://www.tcmb.gov.tr/kurlar/today.xml";
            else
                url = string.Format("http://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", date.Year, addZero(date.Month), addZero(date.Day));

            System.Xml.Linq.XDocument document = System.Xml.Linq.XDocument.Load(url);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var result = document.Descendants("Currency")
            .Where(v => v.Element("ForexBuying") != null && v.Element("ForexBuying").Value.Length > 0)
            .Select(v => new Currency
            {
                Code = v.Attribute("Kod").Value,
                Rate = decimal.Parse(v.Element("ForexBuying").Value.Replace('.', ','))
            }).ToList();
            return result.FirstOrDefault(s => s.Code == code).Rate;
        }

        public class Currency
        {
            public string Code { get; set; }
            public decimal Rate { get; set; }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            gezentipcr.Top = ((Control)sender).Top;
            bunifuPages1.SetPage("tabPage1");
        }


        private void bunifuButton3_Click(object sender, EventArgs e)
        {

            gezentipcr.Top = ((Control)sender).Top;
            bunifuPages1.SetPage("tabPage3");
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {

            gezentipcr.Top = ((Control)sender).Top;
            bunifuPages1.SetPage("tabPage4");
        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {

            gezentipcr.Top = ((Control)sender).Top;
            bunifuPages1.SetPage("tabPage5");
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {

            gezentipcr.Top = ((Control)sender).Top;
            bunifuPages1.SetPage("tabPage6");
        }



        private void Anasayfa_Load(object sender, EventArgs e)
        {
             BakiyeGetir();
            ResimGetir();





        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuIconButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton7_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton9_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton11_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void bunifuIconButton1_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void bunifuIconButton4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {

            }

        }

        private void bunifuIconButton6_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {

            }

        }

        private void bunifuIconButton8_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {

            }

        }

        private void bunifuIconButton10_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {

            }

        }

        private void bunifuIconButton12_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Programı kapatmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();

            }
            else if (dialogResult == DialogResult.No)
            {

            }

        }

        public void TarihSaatEkleme()/*fonksiyonsuz 376.satıra tarih ve saat eklediğimizde
                                 her bakiye ekledimizde ayni dakika ve sanıyeyi veriyordu 
                                 fonksiyonlu bir şekilde sürekli güncellenecek bir method oluşturuldu*/
        {

            string dt = DateTime.Now.ToLongTimeString();
            string dt2 = DateTime.Now.ToLongDateString();
            listBox1.Items.Add("Tarih:" + " " + dt2 + "/" + dt);

        }









        private void ResimEkle_btn_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            bunifuPictureBox6.ImageLocation = openFileDialog1.FileName;
            bunifuPictureBox7.ImageLocation = openFileDialog1.FileName;
            bunifuTextBox2.Text = openFileDialog1.FileName;
        }
        private void ResimKaydet_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(bunifuTextBox2.Text))
            {
                errorProvider1.Clear();
                baglanti.Open();
                SqlCommand YeniResim = new SqlCommand("Update  login_register  set resim= ('" + bunifuTextBox2.Text + "' )where password=@deger;", baglanti);
                YeniResim.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                YeniResim.ExecuteNonQuery();
                MessageBox.Show("kayit eklendi");
            }
            else
            {
                errorProvider1.SetError(bunifuTextBox2, "Lütfen Geçerli bir resim ekleyiniz !!");

            }
            baglanti.Close();
        }



        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                bunifuTextBox4.PasswordChar = '\0';
                bunifuTextBox5.PasswordChar = '\0';
                checkBox1.ImageIndex = 1;


            }
            else
            {
                bunifuTextBox4.PasswordChar = '*';
                bunifuTextBox5.PasswordChar = '*';

                checkBox1.ImageIndex = 0;
            }
        }

        private void bnf_btn_SifreGüncelle_Click(object sender, EventArgs e)
        {
            SifreGüncelle();
        }

        private void bnf_btn_postaTelUpdate_Click(object sender, EventArgs e)
        {
            PostaTelUpdate();
        }
        private void bnf_btn_PdfOlustur_Click(object sender, EventArgs e)
        {
            PdfOlustur();
        }

        private void bunifuTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);/*e.KeyChar propertisi ile basılan tuşun ne olduğunu öğrendik, char.IsDigit() ve char.IsControl()
                                                                                * fonksiyonları da basılan tuş bi karakter yani char tipinde değer olduğu için arka planda ASCII kodlarını karşılaştırıp
                                                                                * basılan tuş sayısal bi ifade ise true, değilse false döndürmektedir.
                                                                                * e.Handled propertisi ile de engelleme işlemini yapmış oluyoruz. Yani kodun sağ tarafından true gelirse engelleme yapılacak,
                                                                                * false gelirse engelleme yapılmayacaktır, yani tam olarak istediğimiz işlem yapılacaktır.*/
        }



        private void Ciksi_Yap_Click(object sender, EventArgs e)  //kullanıcı çıkışı
        {


            DialogResult dialogResult = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz!", "BankaOtomasyon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
                giris giris = new giris();

                giris.Show();

            }
            else if (dialogResult == DialogResult.No)
            {

            }
        }

        private void bnf_btn_ibnYolla_Click_1(object sender, EventArgs e)
        {



            ibanaYolla();




        }
        //sadece sayı girişleri
        private void ibn_bunifuTextBox7_KeyPress(object sender, KeyPressEventArgs e)
        {

            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }

        private void bnf_btnParayolla_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void bunifuTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // bunifuLabel1.Text = label2.Text + "" + label3.Text + "" + label4.Text + "" + label5.Text + "" + label6.Text;
            // bunifuLabel1.Text = GetRate("USD").ToString();
            //  bunifuLabel1.Text = "selamlar";
            //bunifuLabel1.TextAlignment=ContentAlignment.MiddleCenter;           

            bunifuLabel1.Text = bunifuLabel1.Text.Substring(1) + bunifuLabel1.Text.Substring(0, 1);
            //bunifuLabel1.Text = bunifuLabel1.Text.Substring(1);


        }

        private void BakiyeEkleBtn_Click_1(object sender, EventArgs e)
        {
            BakiyeEkle();

        }

        public double DövizDegeri;


        private void DövizAl_btn_Click(object sender, EventArgs e)
        {



            string DövizSec = DövizSec_cp.Text;

            switch (DövizSec)
            {
                case "Dolar":




                    if (string.IsNullOrEmpty(bnf_TutarGir_txt.Text))
                    {
                        MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                        errorProvider1.SetError(bnf_TutarGir_txt, "Lütfen Geçerli bir değer giriniz !!");



                    }
                  
                    
                    else if (GüncelBakiye >= Convert.ToDouble(bnf_TutarGir_txt.Text))//iban ve bakiye büyüklüğü kontrolü
                    {
                        double dolar;
                        dolar = double.Parse(label23.Text);
                        double GirilenTutar;
                        GirilenTutar = double.Parse(bnf_TutarGir_txt.Text);


                        double kisaltilmisMiktar = Math.Round(GirilenTutar / dolar, 2); // İki ondalık basamağa yuvarla
                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye -= @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", GirilenTutar);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set dolar += @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", kisaltilmisMiktar);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");
                        label11.Text = GüncelBakiye.ToString() + " " + "TL";
                       /* label25.Text = DövizDegeri.ToString() + " " + "$";
                            label28.Text = DövizDegeri.ToString() + " " + "$";*/



                            errorProvider1.Clear();

                      




                    }
                    else
                    {
                        MessageBox.Show("bakiyenizi kontrol edin ve bu işlemi tekrar deneyin");
                    }

                    baglanti.Open();
                    SqlCommand Bakiye = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                    Bakiye.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader BakiyeOku = Bakiye.ExecuteReader();//tablodaki tm değerli okur

                    while (BakiyeOku.Read())
                    {
                        GüncelBakiye = Convert.ToDouble(BakiyeOku["bakiye"]);
                    }
                    baglanti.Close();
                    baglanti.Open();
                    SqlCommand Döviz = new SqlCommand("Select dolar from login_register where password=@deger;", baglanti);
                    Döviz.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader DövizOku = Döviz.ExecuteReader();//tablodaki tm değerli okur

                    while (DövizOku.Read())
                    {
                        DövizDegeri = Convert.ToDouble(DövizOku["dolar"]);
                    }
                    baglanti.Close();

                    label11.Text = GüncelBakiye.ToString() + " " + "TL";
                        label28.Text=DövizDegeri.ToString()+ "$";
                    /*label25.Text = DövizDegeri.ToString() + " " + "$";
                        label28.Text = DövizDegeri.ToString() + " " + "$";*/



                        TarihSaatEkleme();
                    listBox1.Items.Add("EKlenen döviz:" + " " + bnf_TutarGir_txt.Text + "$");
                    listBox1.Items.Add("Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label28.Text+ "$");
                    listBox1.Items.Add("----------------------------------------------");



                    break;

                case "Euro":
                    if (string.IsNullOrEmpty(bnf_TutarGir_txt.Text))
                    {
                        MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                        errorProvider1.SetError(bnf_TutarGir_txt, "Lütfen Geçerli bir değer giriniz !!");



                    }

                    
                                    
                        
                        else if (GüncelBakiye >= Convert.ToDouble(bnf_TutarGir_txt.Text))//iban ve bakiye büyüklüğü kontrolü
                        {
                            double euro;
                            euro = double.Parse(label22.Text);
                            double GirilenTutar;
                            GirilenTutar = double.Parse(bnf_TutarGir_txt.Text);


                            double kisaltilmisMiktar = Math.Round(GirilenTutar / euro, 2); // İki ondalık basamağa yuvarla
                            baglanti.Open();
                            SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye -= @GirilenTutar where password = @deger;", baglanti);
                            BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", GirilenTutar);
                            BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            BakiyeAzalt.ExecuteNonQuery();

                            SqlCommand DövizGüncelle = new SqlCommand("Update login_register set euro += @Miktar where password = @deger;", baglanti);
                            DövizGüncelle.Parameters.AddWithValue("@Miktar", kisaltilmisMiktar);
                            DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            DövizGüncelle.ExecuteNonQuery();
                            baglanti.Close();
                            MessageBox.Show("Para gönderme işleminiz başarılı.");
                            label11.Text = GüncelBakiye.ToString() + " " + "TL";
                          /*  label25.Text = DövizDegeri.ToString() + " " + "€";
                            label28.Text = DövizDegeri.ToString() + " " + "€";*/

                            errorProvider1.Clear();



                        }
                        else
                        {
                            MessageBox.Show("bakiyenizi kontrol edin ve bu işlemi tekrar deneyin");
                        }

                        baglanti.Open();
                        SqlCommand Bakiye1 = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        Bakiye1.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeOku1 = Bakiye1.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeOku1.Read())
                        {
                            GüncelBakiye = Convert.ToDouble(BakiyeOku1["bakiye"]);
                        }
                        baglanti.Close();
                        baglanti.Open();
                        SqlCommand Döviz1 = new SqlCommand("Select euro from login_register where password=@deger;", baglanti);
                        Döviz1.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DövizOku1 = Döviz1.ExecuteReader();//tablodaki tm değerli okur

                        while (DövizOku1.Read())
                        {
                            DövizDegeri = Convert.ToDouble(DövizOku1["euro"]);
                        }
                        baglanti.Close();

                        label11.Text = GüncelBakiye.ToString() + " " + "TL";
                        label27.Text = DövizDegeri.ToString()+ "€";

                        /* label25.Text = DövizDegeri.ToString() + " " + "€";
                         label28.Text = DövizDegeri.ToString() + " " + "€";*/

                    

                    TarihSaatEkleme();
                    listBox1.Items.Add("EKlenen döviz:" + " " + bnf_TutarGir_txt.Text + "€");
                    listBox1.Items.Add("Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label27.Text+ "€");
                    listBox1.Items.Add("----------------------------------------------");
                    break;


                case "Sterlin":
                    if (string.IsNullOrEmpty(bnf_TutarGir_txt.Text))
                    {
                        MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                        errorProvider1.SetError(bnf_TutarGir_txt, "Lütfen Geçerli bir değer giriniz !!");



                    }
                

                        else if (GüncelBakiye >= Convert.ToDouble(bnf_TutarGir_txt.Text))//iban ve bakiye büyüklüğü kontrolü
                        {
                            double sterlin;
                            sterlin = double.Parse(label21.Text);
                            double GirilenTutar;
                            GirilenTutar = double.Parse(bnf_TutarGir_txt.Text);


                            double kisaltilmisMiktar = Math.Round(GirilenTutar / sterlin, 2); // İki ondalık basamağa yuvarla
                            baglanti.Open();
                            SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye -= @GirilenTutar where password = @deger;", baglanti);
                            BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", GirilenTutar);
                            BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            BakiyeAzalt.ExecuteNonQuery();

                            SqlCommand DövizGüncelle = new SqlCommand("Update login_register set sterlin += @Miktar where password = @deger;", baglanti);
                            DövizGüncelle.Parameters.AddWithValue("@Miktar", kisaltilmisMiktar);
                            DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            DövizGüncelle.ExecuteNonQuery();
                            baglanti.Close();
                            MessageBox.Show("Para gönderme işleminiz başarılı.");
                            label11.Text = GüncelBakiye.ToString() + " " + "TL";
                           /* label25.Text = DövizDegeri.ToString() + " " + "£";
                            label28.Text = DövizDegeri.ToString() + " " + "£";*/

                            errorProvider1.Clear();



                        }
                        else
                        {
                            MessageBox.Show("bakiyenizi kontrol edin ve bu işlemi tekrar deneyin");
                        }

                        baglanti.Open();
                        SqlCommand Bakiye2 = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        Bakiye2.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeOku2 = Bakiye2.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeOku2.Read())
                        {
                            GüncelBakiye = Convert.ToDouble(BakiyeOku2["bakiye"]);
                        }
                        baglanti.Close();
                        baglanti.Open();
                        SqlCommand Döviz2 = new SqlCommand("Select sterlin from login_register where password=@deger;", baglanti);
                        Döviz2.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DövizOku2 = Döviz2.ExecuteReader();//tablodaki tm değerli okur

                        while (DövizOku2.Read())
                        {
                            DövizDegeri = Convert.ToDouble(DövizOku2["sterlin"]);
                        }
                        baglanti.Close();

                        label11.Text = GüncelBakiye.ToString() + " " + "TL";
                        label29.Text = DövizDegeri.ToString()+ "£";

                    /*  label25.Text = DövizDegeri.ToString() + " " + "£";
                      label28.Text = DövizDegeri.ToString() + " " + "£";*/



                    TarihSaatEkleme();
                    listBox1.Items.Add("EKlenen döviz:" + " " + bnf_TutarGir_txt.Text + "£");
                    listBox1.Items.Add("Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label29.Text + "£");
                    listBox1.Items.Add("----------------------------------------------");
                    break;


                case "Ruble":
                    if (string.IsNullOrEmpty(bnf_TutarGir_txt.Text))
                    {
                        MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                        errorProvider1.SetError(bnf_TutarGir_txt, "Lütfen Geçerli bir değer giriniz !!");



                    }
                   
                        
                        else if (GüncelBakiye >= Convert.ToDouble(bnf_TutarGir_txt.Text))//iban ve bakiye büyüklüğü kontrolü
                        {
                            double ruble;
                            ruble = double.Parse(label19.Text);
                            double GirilenTutar;
                            GirilenTutar = double.Parse(bnf_TutarGir_txt.Text);


                            double kisaltilmisMiktar = Math.Round(GirilenTutar / ruble, 2); // İki ondalık basamağa yuvarla
                            baglanti.Open();
                            SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye -= @GirilenTutar where password = @deger;", baglanti);
                            BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", GirilenTutar);
                            BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            BakiyeAzalt.ExecuteNonQuery();

                            SqlCommand DövizGüncelle = new SqlCommand("Update login_register set ruble += @Miktar where password = @deger;", baglanti);
                            DövizGüncelle.Parameters.AddWithValue("@Miktar", kisaltilmisMiktar);
                            DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            DövizGüncelle.ExecuteNonQuery();
                            baglanti.Close();
                            MessageBox.Show("Para gönderme işleminiz başarılı.");
                            label11.Text = GüncelBakiye.ToString() + " " + "TL";
                           /* label25.Text = DövizDegeri.ToString() + " " + "₽";
                            label28.Text = DövizDegeri.ToString() + " " + "₽";*/

                            errorProvider1.Clear();



                        }
                        else
                        {
                            MessageBox.Show("bakiyenizi kontrol edin ve bu işlemi tekrar deneyinsiz");
                        }

                        baglanti.Open();
                        SqlCommand Bakiye3 = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        Bakiye3.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeOku3 = Bakiye3.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeOku3.Read())
                        {
                            GüncelBakiye = Convert.ToDouble(BakiyeOku3["bakiye"]);
                        }
                        baglanti.Close();
                        baglanti.Open();
                        SqlCommand Döviz3 = new SqlCommand("Select ruble from login_register where password=@deger;", baglanti);
                        Döviz3.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DövizOku3 = Döviz3.ExecuteReader();//tablodaki tm değerli okur

                        while (DövizOku3.Read())
                        {
                            DövizDegeri = Convert.ToDouble(DövizOku3["ruble"]);
                        }
                        baglanti.Close();

                        label11.Text = GüncelBakiye.ToString() + " " + "TL";
                          label30.Text = DövizDegeri.ToString()+ "₽";

                    /* label25.Text = DövizDegeri.ToString() + " " + "₽";
                     label28.Text = DövizDegeri.ToString() + " " + "₽";
                    */

                    TarihSaatEkleme();
                    listBox1.Items.Add("EKlenen döviz:" + " " + bnf_TutarGir_txt.Text + "₽");
                    listBox1.Items.Add("Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label30.Text + "₽");
                    listBox1.Items.Add("----------------------------------------------");
                    break;


                case "Yang":
                    if (string.IsNullOrEmpty(bnf_TutarGir_txt.Text))
                    {
                        MessageBox.Show("lütfen geçerli bir birim giriniz", "Uyarı!!");
                        errorProvider1.SetError(bnf_TutarGir_txt, "Lütfen Geçerli bir değer giriniz !!");



                    }
                  
                        
                        else if (GüncelBakiye >= Convert.ToDouble(bnf_TutarGir_txt.Text))//iban ve bakiye büyüklüğü kontrolü
                        {
                            double yang;
                            yang = double.Parse(label20.Text);
                            double GirilenTutar;
                            GirilenTutar = double.Parse(bnf_TutarGir_txt.Text);


                            double kisaltilmisMiktar = Math.Round(GirilenTutar / yang, 2); // İki ondalık basamağa yuvarla
                            baglanti.Open();
                            SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye -= @GirilenTutar where password = @deger;", baglanti);
                            BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", GirilenTutar);
                            BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            BakiyeAzalt.ExecuteNonQuery();

                            SqlCommand DövizGüncelle = new SqlCommand("Update login_register set yen += @Miktar where password = @deger;", baglanti);
                            DövizGüncelle.Parameters.AddWithValue("@Miktar", kisaltilmisMiktar);
                            DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                            DövizGüncelle.ExecuteNonQuery();
                            baglanti.Close();
                            MessageBox.Show("Para gönderme işleminiz başarılı.");
                            label11.Text = GüncelBakiye.ToString() + " " + "TL";
                           /* label25.Text = DövizDegeri.ToString() + " " + "¥";
                            label28.Text = DövizDegeri.ToString() + " " + "¥";*/


                            errorProvider1.Clear();



                        }
                        else
                        {
                            MessageBox.Show("bakiyenizi kontrol edin ve bu işlemi tekrar deneyin");
                        }

                        baglanti.Open();
                        SqlCommand Bakiye4 = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        Bakiye4.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeOku4 = Bakiye4.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeOku4.Read())
                        {
                            GüncelBakiye = Convert.ToDouble(BakiyeOku4["bakiye"]);
                        }
                        baglanti.Close();
                        baglanti.Open();
                        SqlCommand Döviz4 = new SqlCommand("Select yen from login_register where password=@deger;", baglanti);
                        Döviz4.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DövizOku4 = Döviz4.ExecuteReader();//tablodaki tm değerli okur

                        while (DövizOku4.Read())
                        {
                            DövizDegeri = Convert.ToDouble(DövizOku4["yen"]);
                        }
                        baglanti.Close();

                        label11.Text = GüncelBakiye.ToString() + " " + "TL";
                         label31.Text = DövizDegeri.ToString()+ "¥";

                    /* label25.Text = DövizDegeri.ToString() + " " + "¥";
                     label28.Text = DövizDegeri.ToString() + " " + "¥";*/


                    TarihSaatEkleme();
                    listBox1.Items.Add("EKlenen döviz:" + " " + bnf_TutarGir_txt.Text + "¥");
                    listBox1.Items.Add("Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label31.Text + "¥");
                    listBox1.Items.Add("----------------------------------------------");
                    break;

            }
        }

        public double DolarMiktari;
        public double EuroMiktari;
        public double SterlinMiktari;
        public double RubleMiktari;
        public double YenMiktari;

        public double BakiyeDegeri;
        public double KontrolDövizMiktar;


        private void DövizSat_btn_Click(object sender, EventArgs e)
        {
            string DövizSat = DövizSat_cb.Text;


            switch (DövizSat)
            {
                case"Dolar":


                    baglanti.Open();
                    SqlCommand KontrolDöviz = new SqlCommand("Select dolar from login_register where password=@deger;", baglanti);
                    KontrolDöviz.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader KontrolEt = KontrolDöviz.ExecuteReader();//tablodaki tm değerli okur

                    while (KontrolEt.Read())
                    {
                        KontrolDövizMiktar = Convert.ToDouble(KontrolEt["dolar"]);
                    }
                    baglanti.Close();



                    if (KontrolDövizMiktar==0)
                    {
                        MessageBox.Show("Yetersiz Döviz tutarı");
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand DövizOku = new SqlCommand("Select dolar from login_register where password=@deger;", baglanti);
                        DövizOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DolarOku = DövizOku.ExecuteReader();//tablodaki tm değerli okur

                        while (DolarOku.Read())
                        {
                            DolarMiktari = Convert.ToDouble(DolarOku["dolar"]);
                        }
                        baglanti.Close();

                        double DolarDeger;
                        DolarDeger = double.Parse(label23.Text);

                        double DolarTL_Dönüstür = Math.Round(DolarDeger * DolarMiktari, 2); // İki ondalık basamağa yuvarla
                        double SonDolar_Deger = 0;

                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye += @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", DolarTL_Dönüstür);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set dolar = @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", SonDolar_Deger);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");

                        baglanti.Open();
                        SqlCommand DövizOku1 = new SqlCommand("Select dolar from login_register where password=@deger;", baglanti);
                        DövizOku1.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader DolarOku1 = DövizOku1.ExecuteReader();//tablodaki tm değerli okur

                        while (DolarOku1.Read())
                        {
                            DolarMiktari = Convert.ToDouble(DolarOku1["dolar"]);
                        }
                        baglanti.Close();

                        baglanti.Open();
                        SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeGetir = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeGetir.Read())
                        {
                            BakiyeDegeri = Convert.ToDouble(BakiyeGetir["bakiye"]);
                        }
                        baglanti.Close();

                        label28.Text = DolarMiktari.ToString() + "$";
                        label11.Text = BakiyeDegeri.ToString() + " " + "TL";


                    }
                    TarihSaatEkleme();
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label28.Text + "$");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");





                    break;

                case "Euro":

                    baglanti.Open();
                    SqlCommand KontrolDöviz1 = new SqlCommand("Select euro from login_register where password=@deger;", baglanti);
                    KontrolDöviz1.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader KontrolEt1 = KontrolDöviz1.ExecuteReader();//tablodaki tm değerli okur

                    while (KontrolEt1.Read())
                    {
                        KontrolDövizMiktar = Convert.ToDouble(KontrolEt1["euro"]);
                    }
                    baglanti.Close();



                    if (KontrolDövizMiktar == 0)
                    {
                        MessageBox.Show("Yetersiz Döviz tutarı");
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand DövizOku = new SqlCommand("Select euro from login_register where password=@deger;", baglanti);
                        DövizOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader EuroOku = DövizOku.ExecuteReader();//tablodaki tm değerli okur

                        while (EuroOku.Read())
                        {
                            EuroMiktari = Convert.ToDouble(EuroOku["euro"]);
                        }
                        baglanti.Close();

                        double EuroDeger;
                        EuroDeger = double.Parse(label22.Text);

                        double EuroTL_Dönüstür = Math.Round(EuroDeger * EuroMiktari, 2); // İki ondalık basamağa yuvarla
                        double SonEuro_Deger = 0;

                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye += @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", EuroTL_Dönüstür);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set euro = @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", SonEuro_Deger);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");

                        baglanti.Open();
                        SqlCommand DövizOku2 = new SqlCommand("Select euro from login_register where password=@deger;", baglanti);
                        DövizOku2.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader EuroOku1 = DövizOku2.ExecuteReader();//tablodaki tm değerli okur

                        while (EuroOku1.Read())
                        {
                            EuroMiktari = Convert.ToDouble(EuroOku1["euro"]);
                        }
                        baglanti.Close();

                        baglanti.Open();
                        SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeGetir = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeGetir.Read())
                        {
                            BakiyeDegeri = Convert.ToDouble(BakiyeGetir["bakiye"]);
                        }
                        baglanti.Close();

                        label27.Text = DolarMiktari.ToString() + "€";
                        label11.Text = BakiyeDegeri.ToString() + " " + "TL";


                    }
                    TarihSaatEkleme();
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label27.Text + "€");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");




                    break;
                case "Sterlin":

                    baglanti.Open();
                    SqlCommand KontrolDöviz2 = new SqlCommand("Select sterlin from login_register where password=@deger;", baglanti);
                    KontrolDöviz2.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader KontrolEt2 = KontrolDöviz2.ExecuteReader();//tablodaki tm değerli okur

                    while (KontrolEt2.Read())
                    {
                        KontrolDövizMiktar = Convert.ToDouble(KontrolEt2["sterlin"]);
                    }
                    baglanti.Close();



                    if (KontrolDövizMiktar == 0)
                    {
                        MessageBox.Show("Yetersiz Döviz tutarı");
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand DövizOku = new SqlCommand("Select sterlin from login_register where password=@deger;", baglanti);
                        DövizOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader SterlinOku = DövizOku.ExecuteReader();//tablodaki tm değerli okur

                        while (SterlinOku.Read())
                        {
                            SterlinMiktari = Convert.ToDouble(SterlinOku["sterlin"]);
                        }
                        baglanti.Close();

                        double SterlinDeğer;
                        SterlinDeğer = double.Parse(label21.Text);

                        double SterlinTL_Dönüstür = Math.Round(SterlinDeğer * SterlinMiktari, 2); // İki ondalık basamağa yuvarla
                        double SonSterlin_Deger = 0;

                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye += @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", SterlinTL_Dönüstür);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set sterlin = @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", SonSterlin_Deger);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");

                        baglanti.Open();
                        SqlCommand DövizOku3 = new SqlCommand("Select sterlin from login_register where password=@deger;", baglanti);
                        DövizOku3.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader SterlinOku2 = DövizOku3.ExecuteReader();//tablodaki tm değerli okur

                        while (SterlinOku2.Read())
                        {
                            EuroMiktari = Convert.ToDouble(SterlinOku2["sterlin"]);
                        }
                        baglanti.Close();

                        baglanti.Open();
                        SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeGetir = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeGetir.Read())
                        {
                            BakiyeDegeri = Convert.ToDouble(BakiyeGetir["bakiye"]);
                        }
                        baglanti.Close();

                        label29.Text = DolarMiktari.ToString() + "£";
                        label11.Text = BakiyeDegeri.ToString() + " " + "TL";


                    }
                    TarihSaatEkleme();
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label29.Text + "£");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");



                    break;
                case "Ruble":
                    baglanti.Open();
                    SqlCommand KontrolDöviz3 = new SqlCommand("Select ruble from login_register where password=@deger;", baglanti);
                    KontrolDöviz3.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader KontrolEt3 = KontrolDöviz3.ExecuteReader();//tablodaki tm değerli okur

                    while (KontrolEt3.Read())
                    {
                        KontrolDövizMiktar = Convert.ToDouble(KontrolEt3["ruble"]);
                    }
                    baglanti.Close();



                    if (KontrolDövizMiktar == 0)
                    {
                        MessageBox.Show("Yetersiz Döviz tutarı");
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand DövizOku = new SqlCommand("Select ruble from login_register where password=@deger;", baglanti);
                        DövizOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader RubleOku = DövizOku.ExecuteReader();//tablodaki tm değerli okur

                        while (RubleOku.Read())
                        {
                            RubleMiktari = Convert.ToDouble(RubleOku["ruble"]);
                        }
                        baglanti.Close();

                        double RubleDeğer;
                        RubleDeğer = double.Parse(label19.Text);

                        double RubleTL_Dönüstür = Math.Round(RubleDeğer * RubleMiktari, 2); // İki ondalık basamağa yuvarla
                        double SonRuble_Deger = 0;

                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye += @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", RubleTL_Dönüstür);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set ruble = @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", SonRuble_Deger);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");

                        baglanti.Open();
                        SqlCommand DövizOku4 = new SqlCommand("Select ruble from login_register where password=@deger;", baglanti);
                        DövizOku4.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader RubleOku3 = DövizOku4.ExecuteReader();//tablodaki tm değerli okur

                        while (RubleOku3.Read())
                        {
                            EuroMiktari = Convert.ToDouble(RubleOku3["ruble"]);
                        }
                        baglanti.Close();

                        baglanti.Open();
                        SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeGetir = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeGetir.Read())
                        {
                            BakiyeDegeri = Convert.ToDouble(BakiyeGetir["bakiye"]);
                        }
                        baglanti.Close();

                        label30.Text = DolarMiktari.ToString() + "₽";
                        label11.Text = BakiyeDegeri.ToString() + " " + "TL";


                    }
                    TarihSaatEkleme();
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label30.Text + "₽");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");



                    break;
                case "Yang":


                    baglanti.Open();
                    SqlCommand KontrolDöviz4 = new SqlCommand("Select yen from login_register where password=@deger;", baglanti);
                    KontrolDöviz4.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                    SqlDataReader KontrolEt4 = KontrolDöviz4.ExecuteReader();//tablodaki tm değerli okur

                    while (KontrolEt4.Read())
                    {
                        KontrolDövizMiktar = Convert.ToDouble(KontrolEt4["yen"]);
                    }
                    baglanti.Close();



                    if (KontrolDövizMiktar == 0)
                    {
                        MessageBox.Show("Yetersiz Döviz tutarı");
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand DövizOku = new SqlCommand("Select yen from login_register where password=@deger;", baglanti);
                        DövizOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader YenOku = DövizOku.ExecuteReader();//tablodaki tm değerli okur

                        while (YenOku.Read())
                        {
                            YenMiktari = Convert.ToDouble(YenOku["yen"]);
                        }
                        baglanti.Close();

                        double YenDeğer;
                        YenDeğer = double.Parse(label20.Text);

                        double YenTL_Dönüstür = Math.Round(YenDeğer * YenMiktari, 2); // İki ondalık basamağa yuvarla
                        double SonYen_Deger = 0;

                        baglanti.Open();
                        SqlCommand BakiyeAzalt = new SqlCommand("Update login_register set bakiye += @GirilenTutar where password = @deger;", baglanti);
                        BakiyeAzalt.Parameters.AddWithValue("@GirilenTutar", YenTL_Dönüstür);
                        BakiyeAzalt.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        BakiyeAzalt.ExecuteNonQuery();

                        SqlCommand DövizGüncelle = new SqlCommand("Update login_register set yen = @Miktar where password = @deger;", baglanti);
                        DövizGüncelle.Parameters.AddWithValue("@Miktar", SonYen_Deger);
                        DövizGüncelle.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        DövizGüncelle.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Para gönderme işleminiz başarılı.");

                        baglanti.Open();
                        SqlCommand DövizOku5 = new SqlCommand("Select yen from login_register where password=@deger;", baglanti);
                        DövizOku5.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader YenOku4 = DövizOku5.ExecuteReader();//tablodaki tm değerli okur

                        while (YenOku4.Read())
                        {
                            YenMiktari = Convert.ToDouble(YenOku4["yen"]);
                        }
                        baglanti.Close();

                        baglanti.Open();
                        SqlCommand BakiyeOku = new SqlCommand("Select bakiye from login_register where password=@deger;", baglanti);
                        BakiyeOku.Parameters.AddWithValue("@deger", giris.statikButon.Text);
                        SqlDataReader BakiyeGetir = BakiyeOku.ExecuteReader();//tablodaki tm değerli okur

                        while (BakiyeGetir.Read())
                        {
                            BakiyeDegeri = Convert.ToDouble(BakiyeGetir["bakiye"]);
                        }
                        baglanti.Close();

                        label31.Text = DolarMiktari.ToString() + "¥";
                        label11.Text = BakiyeDegeri.ToString() + " " + "TL";


                    }
                    TarihSaatEkleme();
                    listBox1.Items.Add("Güncel Döviziniz:" + " " + label31.Text + "¥");
                    listBox1.Items.Add("Güncel Bakiyeniz:" + " " + label11.Text);
                    listBox1.Items.Add("----------------------------------------------");




                    break;
            }
        }
    }
    }
    
    

