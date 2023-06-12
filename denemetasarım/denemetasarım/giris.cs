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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Bunifu.UI.WinForms;
using Bunifu.Framework.UI;

namespace denemetasarım
{
    public partial class giris : Form
    {

        public static SqlConnection baglanti=new SqlConnection("Data Source=DESKTOP-0V8VD7E\\SQLEXPRESS; Initial Catalog=otomasyon; Integrated Security=TRUE");
        public giris()
        {
            InitializeComponent();
        }
        //formun hareket etmesi için
        int Move;
        int Mouse_X;
        int Mouse_Y;

        bool isthere;

        public string adi = "";
        public string soyadi = "";
        public string Id = "";
        public double Bakiye;
        public double DövizDegeri;
        public double Dolar;
        public double Euro;
        public double Sterlin;
        public double Ruble;
        public double Yen;




        public static BunifuTextBox statikButon;//giriş formundaki bunifutextbox2 yi anasayfa formuna eriştirdik şifreye göre bakiye çekimi için


        public void GirisYap()
        {
            if (true)
            {

            }
            string username = bunifuTextBox1.Text;
            string pass = bunifuTextBox2.Text;

            baglanti.Open();

            SqlCommand command = new SqlCommand("Select *from login_register", baglanti);
            SqlDataReader reader = command.ExecuteReader();//tablodaki tm değerli okur

            while (reader.Read())//veriyi okuyorken
            {

                if (username == reader["username"].ToString().TrimEnd() && pass == reader["password"].ToString().TrimEnd())//boşluklar için trim end
                {
                    //kayıt varmı
                    isthere = true;
                    break;
                }
                /* else
                 {
                     isthere = false;
                 }*/
                else if (!(username == reader["username"].ToString().TrimEnd() && pass == reader["password"].ToString().TrimEnd()))
                {
                    isthere = false;
                }

            }
            


            if (isthere)
            {

                bunifuTextBox1.BackColor = Color.Green;
                bunifuTextBox2.BackColor = Color.Green;

                MessageBox.Show("başarıyla giriş yapıldı!!", "Program");

                baglanti.Close();

                baglanti.Open();//bu kısımda lbel 1 ve label 7 ye anasayfada kullanıcı sifresine göre ad soyad getirtmeyi yazıyyoruz ve Id
                SqlCommand AdSoyad = new SqlCommand("Select adi,soyadi,User_Id,bakiye,döviz,dolar,euro,sterlin,ruble,yen from login_register where password=@deger", baglanti);
                AdSoyad.Parameters.AddWithValue("@deger", bunifuTextBox2.Text);
                SqlDataReader AdSoyadOku = AdSoyad.ExecuteReader();//tablodaki tm değerli okur

                while (AdSoyadOku.Read())
                {
                    adi = AdSoyadOku["adi"].ToString();
                    soyadi = AdSoyadOku["soyadi"].ToString();
                    Id = AdSoyadOku["User_Id"].ToString();
                   Bakiye = Convert.ToDouble( AdSoyadOku["bakiye"]); //bakiye anasayfada getirilmiyordu bunun için giriş kısmına da getirmeyi yaptık oldu.
                    Dolar = Convert.ToDouble(AdSoyadOku["dolar"]);
                    Euro = Convert.ToDouble(AdSoyadOku["euro"]);
                    Sterlin = Convert.ToDouble(AdSoyadOku["sterlin"]);
                    Ruble = Convert.ToDouble(AdSoyadOku["ruble"]);
                    Yen = Convert.ToDouble(AdSoyadOku["yen"]);


                    //DövizDegeri = Convert.ToDouble(AdSoyadOku["döviz"]);


                }


                Anasayfa anasayfa = new Anasayfa();
                anasayfa.Show();
                anasayfa.label1.Text = adi;
                anasayfa.label7.Text = soyadi;
                anasayfa.label9.Text = "Müşteri Id: " + Id;
                anasayfa.label11.Text = Bakiye.ToString()+" "+"TL";
                anasayfa.label28.Text = Dolar.ToString() + "$";
                anasayfa.label27.Text = Euro.ToString() + "€";
                anasayfa.label29.Text = Sterlin.ToString() + "£";
                anasayfa.label30.Text = Ruble.ToString() + "₽";
                anasayfa.label31.Text = Yen.ToString() + "¥";

                //  anasayfa.label25.Text=DövizDegeri.ToString();
                // anasayfa.label28.Text = DövizDegeri.ToString();
                this.Hide();
            }
            else if (string.IsNullOrEmpty(bunifuTextBox1.Text) || string.IsNullOrEmpty(bunifuTextBox2.Text)) //null mu yoksa boş mu
            {
                MessageBox.Show("boş alanları doldurun!!", "Program");

                bunifuTextBox1.BackColor = Color.Red;
                bunifuTextBox2.BackColor = Color.Red;
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya Şifre yanlış!!", "Program");

            }

            /*else
            {
                MessageBox.Show("giriş başarısız!!", "Program");

            }*/

            baglanti.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            statikButon = bunifuTextBox2;//textbox ı bunifuTextBox2 yaptık 
        }

       

        private void bunifuIconButton1_Click(object sender, EventArgs e)
        {

              Application.Exit();



        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_kayitol_Click(object sender, EventArgs e)
        {

            kayit kayit = new kayit();

            kayit.Show();

            this.Hide();

        }

        private void btn_giris_Click(object sender, EventArgs e)
        {



            GirisYap();
        
            
      
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            Move = 0;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Move = 1;
            Mouse_X = e.X;
            Mouse_Y = e.Y;

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Move == 1)
            {
                this.SetDesktopLocation(MousePosition.X - Mouse_X, MousePosition.Y - Mouse_Y);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                bunifuTextBox2.PasswordChar = '\0';
                checkBox1.ImageIndex = 1;


            }
            else
            {
                bunifuTextBox2.PasswordChar = '*';
                checkBox1.ImageIndex = 0;
            }
        }

        private void btn_sifremiUnuttum_Click(object sender, EventArgs e)
        {

            SifremiUnuttum sifremiUnuttum = new SifremiUnuttum();

            sifremiUnuttum.Show();

            this.Hide();
        }

        private void bunifuIconButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

      
    }
}
