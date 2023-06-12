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
using System.Net;
using System.Net.Mail;


namespace denemetasarım
{
    public partial class SifremiUnuttum : Form
    {

       
        SqlConnection baglanti = giris.baglanti;
        public SifremiUnuttum()
        {
            InitializeComponent();
        }

        int Move;
        int Mouse_X;
        int Mouse_Y;
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

        private void btn_yolla_Click(object sender, EventArgs e)
        {

            baglanti.Open();
            SqlCommand command = new SqlCommand("Select *from login_register Where username='"+bunifuTextBox1.Text.ToString()+    
                "'and email= '"+bunifuTextBox4.Text.ToString()+ "'" , baglanti);
            
            SqlDataReader oku=command.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    if (baglanti.State==ConnectionState.Closed)
                    {
                        baglanti.Open();
                    }
                    SmtpClient smtpserver = new SmtpClient("outlook.com");
                    MailMessage mail=new MailMessage();
                    string tarih = DateTime.Now.ToLongDateString();
                    string mailadresin = ("serhat.kilic@outlook.com");
                    string sifre = ("deneme12345");
                    string smptserver = "smtp.outlook.com";
                    string kime = (oku["email"].ToString());
                    string konu = ("Şifre hatırlatma Maili");
                    string yaz = ("Sayın" + oku["adi"].ToString() + " " + oku["soyadi"].ToString() + "\n" + "bizden" + tarih + "tarihinde şifre hatırlatma talebinde " +
                        "bulundunuz" + "\n" + "parolanız:" + oku["password"].ToString() + "\n İyi günler");
                    smtpserver.UseDefaultCredentials = false;
                    smtpserver.Credentials = new NetworkCredential(mailadresin, sifre);
                    smtpserver.Port = 587;
                    smtpserver.Host=smptserver;
                    smtpserver.EnableSsl = true;
                    mail.From = new MailAddress(mailadresin);
                    mail.To.Add(kime);
                    mail.Subject = konu;
                    mail.Body = yaz;
                    smtpserver.Send(mail);
                    DialogResult bilgi = new DialogResult();
                    bilgi = MessageBox.Show("girmiş olduğunuz bilgiler uyuşuyor şifreniz mail adresinize gönderilmiştir");
                    this.Hide();
                }               
                catch (Exception Hata)
                {
                    MessageBox.Show("Mail gönderme hatası!",Hata.Message);

                }
                
            }
            baglanti.Close();
        }
    }
}
