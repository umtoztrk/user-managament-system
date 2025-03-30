using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oop2_proje_testProject2;
using Personal_Information;
using salaryCalculator;

namespace logInFormTest1
{
    public partial class LogInMenu : Form
    {
        public static List<User> users = new List<User>(); // kullanıcı listesi
        public static int currUserIndex; // geçerli kullanıcı indeksi
        public LogInMenu()
        {
            InitializeComponent(); // bileşenleri başlat
        }

        public static void WriteToCsv()
        {
            string currentDirectory = Directory.GetCurrentDirectory(); // geçerli dizin
            string fileName = "bilgiler.csv"; // dosya adı
            string filePath = Path.Combine(currentDirectory, fileName); // dosya yolu oluştur

            if (!File.Exists(filePath)) // dosya yoksa oluştur
            {
                using (FileStream fs = File.Create(filePath))
                { }
            }

            using (StreamWriter sw = new StreamWriter(filePath)) // dosyayı yazma modunda aç
            {
                sw.WriteLine("Name,Surname,Phone,Email,Address,Password,Salary,UserType,Photo"); // başlık satırını yaz

                foreach (var user in users) // her kullanıcıyı csv dosyasına yaz
                {
                    sw.WriteLine(user.ToCsv());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register register = new Register(); // kayıt formunu aç
            this.Hide(); // bu formu gizle
            register.ShowDialog(); // kayıt formunu göster
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            users = new List<User>(); // kullanıcı listesi başlat
            string currentDirectory = Directory.GetCurrentDirectory(); // geçerli dizin
            string fileName = "bilgiler.csv"; // dosya adı
            string filePath = Path.Combine(currentDirectory, fileName); // dosya yolu
            if (File.Exists(filePath)) // dosya varsa
            {
                foreach (var line in File.ReadAllLines(filePath).Skip(1)) // başlık satırını atla
                {
                    if (!string.IsNullOrWhiteSpace(line)) // satır boş değilse
                    {
                        try
                        {
                            User user = User.FromCsv(line); // csv'den kullanıcı oluştur
                            users.Add(user); // kullanıcıyı listeye ekle
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hata: {ex.Message}"); // hata mesajı göster
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "") // boş alan kontrolü
            {
                MessageBox.Show("Please fill in all fields."); // uyarı mesajı
                return;
            }
            foreach (var user in users) // kullanıcıları kontrol et
            {
                if (user.Email == textBox1.Text && user.Password == textBox2.Text) // e-posta ve şifre kontrolü
                {
                    currUserIndex = users.IndexOf(user); // geçerli kullanıcıyı ayarla
                    MessageBox.Show("Login successful."); // başarılı giriş mesajı
                    MainMenu mainMenu = new MainMenu(); // ana menüye geçiş
                    this.Hide(); // bu formu gizle
                    mainMenu.ShowDialog(); // ana menüyü göster
                    return;
                }
            }
            MessageBox.Show("Invalid email or password."); // hatalı giriş mesajı
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // formu kullanıcı kapatıyorsa
            {
                DialogResult result = MessageBox.Show("Really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // çıkış onayı
                if (result == DialogResult.Yes) // evet ise
                {
                    Application.Exit(); // uygulamayı kapat
                }
                else
                {
                    e.Cancel = true; // iptal et
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar == '\0') textBox2.PasswordChar = '*'; // şifreyi gizle
            else textBox2.PasswordChar = '\0'; // şifreyi göster
        }
    }
}
