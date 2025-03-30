using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Personal_Information;
using System.IO;

namespace logInFormTest1
{
    public partial class UserManagement : Form
    {
        private bool flag1 = false;
        private bool flag2 = false;
        private bool check = false;
        private PictureBox pbtest = new PictureBox();
        private int progressUser = -1; // seçili kullanıcının indeksini tutar
        public UserManagement()
        {
            InitializeComponent();
        }

        // form yüklendiğinde kullanıcıları combobox'a ekler
        private void UserManagement_Load(object sender, EventArgs e)
        {
            label9.Visible = false;
            for (int i = 0; i < LogInMenu.users.Count; i++)
            {
                var user = LogInMenu.users[i];
                if (i == LogInMenu.currUserIndex) continue; // mevcut kullanıcıyı listeye ekleme
                comboBox1.Items.Add($"{user.Name} {user.Surname} {user.Email}");
            }
            pbtest.Size = pictureBox1.Size;
        }

        // seçili kullanıcı bilgilerini gösterir
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            progressBar1.Visible = false;
            label9.Visible = false;
            string[] selectedUser = comboBox1.SelectedItem.ToString().Split(' ');      
            foreach (var user in LogInMenu.users)
            {
                if (user.Email == selectedUser[2])
                {
                    textBox1.Text = user.Name;
                    textBox2.Text = user.Surname;
                    textBox3.Text = user.Email;
                    textBox4.Text = "(" + user.Phone.Substring(0, 3) + ") " + user.Phone.Substring(3, 3) + " " + user.Phone.Substring(6, 2) + " " + user.Phone.Substring(8, 2);
                    textBox5.Text = user.Address;
                    textBox6.Text = user.Password;
                    Register.ConvertBase64ToImageAndDisplay(user.Photo, pictureBox1);
                    foreach (string i in comboBox2.Items)
                    {
                        if (i == user.UserType) comboBox2.SelectedItem = i;
                    }
                    progressUser = LogInMenu.users.IndexOf(user);
                    break;
                }
            }
        }

        // kullanıcı bilgilerini günceller
        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            label9.Visible = false;
            textBox4.Text = textBox4.Text.Replace("(", "").Replace(")", "").Replace(" ", "");

            // alanların boş olup olmadığını kontrol eder
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            // e-posta adresinin geçerli olup olmadığını kontrol eder
            else if (!Register.IsValidEmail(textBox3.Text))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }
            // kullanıcı türünün seçili olup olmadığını kontrol eder
            else if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select user type.");
                return;
            }
            // herhangi bir bilgi güncellenmiş mi kontrol eder
            else if (LogInMenu.users[progressUser].Name == textBox1.Text && LogInMenu.users[progressUser].Surname == textBox2.Text && LogInMenu.users[progressUser].Email == textBox3.Text && LogInMenu.users[progressUser].Phone == textBox4.Text && LogInMenu.users[progressUser].Address == textBox5.Text && LogInMenu.users[progressUser].Password == textBox6.Text && LogInMenu.users[progressUser].UserType == comboBox2.SelectedItem.ToString() && checkImages(LogInMenu.users[progressUser]))
            {
                MessageBox.Show("Any info has been updated. Update Info.");
                return;
            }
            else if(textBox1.Text.Contains(",") || textBox2.Text.Contains(",") || textBox3.Text.Contains(",") || textBox4.Text.Contains(",") || textBox5.Text.Contains(",") || textBox6.Text.Contains(","))
            {
                MessageBox.Show("Fields cannot contain commas.");
                return;
            }
            // kullanıcı bilgilerini günceller
            LogInMenu.users[progressUser].Name = textBox1.Text;
            LogInMenu.users[progressUser].Surname = textBox2.Text;
            LogInMenu.users[progressUser].Email = textBox3.Text;
            LogInMenu.users[progressUser].Phone = textBox4.Text;
            LogInMenu.users[progressUser].Address = textBox5.Text;
            LogInMenu.users[progressUser].Password = textBox6.Text;
            LogInMenu.users[progressUser].UserType = comboBox2.SelectedItem.ToString();
            LogInMenu.users[progressUser].Photo = Register.ImageToBase64(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
            LogInMenu.WriteToCsv();
            textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = ""; textBox4.Text = ""; textBox5.Text = ""; textBox6.Text = ""; comboBox1.Text = ""; comboBox2.SelectedItem = null;
            progressUser = -1;
            MessageBox.Show("User updated successfully.");
            pictureBox1.Image = null;
            
        }

        // kullanıcıyı siler
        private void button4_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            label9.Visible = false;
            textBox4.Text = textBox4.Text.Replace("(", "").Replace(")", "").Replace(" ", "");

            // kullanıcı bilgileri değiştirilmiş mi kontrol eder
            if (LogInMenu.users[progressUser].Name == textBox1.Text && LogInMenu.users[progressUser].Surname == textBox2.Text && LogInMenu.users[progressUser].Phone == textBox4.Text && LogInMenu.users[progressUser].Address == textBox5.Text && LogInMenu.users[progressUser].Password == textBox6.Text && LogInMenu.users[progressUser].UserType == comboBox2.Text && checkImages(LogInMenu.users[progressUser]))
            {
                LogInMenu.users.Remove(LogInMenu.users[progressUser]);
                LogInMenu.WriteToCsv();
                comboBox1.Items.Remove(comboBox1.SelectedItem);
                textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = ""; textBox4.Text = ""; textBox5.Text = ""; textBox6.Text = ""; comboBox1.Text = ""; comboBox2.Text = "";
                progressUser = -1;
                MessageBox.Show("User deleted successfully.");
                pictureBox1.Image = null;
                comboBox1.Text = "";
                comboBox1.SelectedIndex = -1;
                return;
            }
            MessageBox.Show("Please dont do any change user's info after select a user from combobox.");
        }

        // ana menüye döner
        private void button1_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            this.Hide();
            mainMenu.Show();
        }

        // rastgele şifre üretir
        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        // yeni şifre gönderir
        private void button5_Click(object sender, EventArgs e)
        {
            if (progressUser != -1)
            {
                string newPassword = GenerateRandomString(8);
                progressBar1.Visible = true;
                timer1.Start();

                LogInMenu.users[progressUser].Password = newPassword;
                LogInMenu.WriteToCsv();
                try
                {
                    // SMTP sunucu bilgileri
                    int smtpPort = 587;
                    //string smtpServer = "smtp.office365.com";
                    //string smtpUsername = "norplychngepswrd@hotmail.com";
                    //string smtpPassword = "OopProjectTest1";
                    string smtpServer = "smtp-mail.outlook.com";
                    string smtpUsername = "oopproje0@hotmail.com";
                    string smtpPassword = "oop_Proje0.2";

                    // gönderici ve alıcı bilgileri
                    //string fromAddress = "norplychngepswrd@hotmail.com";
                    string fromAddress = "oopproje0@hotmail.com";
                    string toAddress = LogInMenu.users[progressUser].Email;
                    string subject = "New Password";
                    string body = "New password : " + newPassword;

                    // SMTPClient ve NetworkCredential kullanarak e-posta gönderme
                    SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
                    {
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = true
                    };

                    MailMessage mailMessage = new MailMessage(fromAddress, toAddress, subject, body);

                    smtpClient.Send(mailMessage);
                    check = true;
                    label9.Visible = true;
                    label9.Text = "New password : " + newPassword;
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
            }
            else MessageBox.Show("Please select user from combobox.");
        }

        // e-posta gönderim ilerlemesini gösterir
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (check)
            {
                progressBar1.Value = 100;
                Console.WriteLine("Email sent successfully!");
                timer1.Stop();
                return;
            }
            else if (progressBar1.Value == 100)
            {
                return;
            }
            progressBar1.Value = progressBar1.Value + 5;
        }

        // yeni resim yükleme
        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            label9.Visible = false;
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Please select user from combobox.");
                return;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|.jpg;.jpeg;*.png;";
                openFileDialog.Title = "Resim Dosyası Seç";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    pictureBox1.Image = Image.FromFile(selectedFilePath);
                    //string base64String = Register.ImageToBase64(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
                    //string[] selectedUser = comboBox1.SelectedItem.ToString().Split(' ');
                    //foreach (var user in LogInMenu.users)
                    //{
                    //    if (user.Email == selectedUser[2])
                    //    {
                    //        user.Photo = base64String;
                    //    }
                    //}
                    pictureBox1.Refresh();
                }
            }
        }

        // resimlerin aynı olup olmadığını kontrol eder
        private bool checkImages(User user)
        {
            Register.ConvertBase64ToImageAndDisplay(user.Photo, pbtest);
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pbtest.Image);

            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
                return false;

            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        return false;
                }
            }
            return true;
        }

        // telefon numarası formatlama
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length == 3 && !textBox4.Text.Contains("("))
            {
                textBox4.Text = "(" + textBox4.Text + ") ";
                textBox4.SelectionStart = textBox4.Text.Length;
            }
            else if (textBox4.Text.Length == 2 && textBox4.Text.Contains("("))
            {
                textBox4.Text = textBox4.Text.Replace("(", "").Replace(")", "");
                textBox4.SelectionStart = textBox4.Text.Length;
            }
            if (textBox4.Text.Length == 9 && !flag1)
            {
                flag1 = true;
                textBox4.Text = textBox4.Text + " ";
                textBox4.SelectionStart = textBox4.Text.Length;
            }
            else if (textBox4.Text.Length == 9 && flag1)
            {
                flag1 = false;
            }
            if (textBox4.Text.Length == 12 && !flag2)
            {
                flag2 = true;
                textBox4.Text = textBox4.Text + " ";
                textBox4.SelectionStart = textBox4.Text.Length;
            }
            else if (textBox4.Text.Length == 12 && flag2)
            {
                flag2 = false;
            }
        }

        // sadece harf girişine izin verir
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        // sadece rakam girişine izin verir
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox4.TextLength >= 15 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // sadece harf girişine izin verir
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (progressUser == -1)
            {
                MessageBox.Show("Please select user from combobox.");
                return;
            }
            else
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    try
                    {
                        saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"; // Dosya türü filtresi
                        saveFileDialog.Title = "CSV dosyasını kaydet";
                        saveFileDialog.FileName = "data.csv"; // Varsayılan dosya adı
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filePath = saveFileDialog.FileName;

                            // CSV içeriğini oluştur
                            using (StreamWriter sw = new StreamWriter(filePath))
                            {
                                // Başlık satırını yaz
                                sw.WriteLine("Name,Surname,Phone,Email,Address,Password,Salary,UserType,Photo");

                                // Her User nesnesini CSV dosyasına yaz
                                sw.WriteLine(LogInMenu.users[progressUser].ToCsv());
                            }
                            MessageBox.Show("CSV dosyası başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Beklenmeyen bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                try
                {
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"; // Dosya türü filtresi
                    saveFileDialog.Title = "CSV dosyasını kaydet";
                    saveFileDialog.FileName = "data.csv"; // Varsayılan dosya adı
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        // CSV içeriğini oluştur
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {
                            // Başlık satırını yaz
                            sw.WriteLine("Name,Surname,Phone,Email,Address,Password,Salary,UserType,Photo");

                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var user in LogInMenu.users)
                            {
                                sw.WriteLine(user.ToCsv());
                            }
                        }
                        MessageBox.Show("CSV dosyası başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Beklenmeyen bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}