using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using logInFormTest1.Properties;
using Personal_Information;

namespace logInFormTest1
{
    public partial class Register : Form
    {
        private bool flag1 = false;
        private bool flag2 = false;
        public Register()
        {
            InitializeComponent();
        }

        public static void ConvertBase64ToImageAndDisplay(string base64String, PictureBox pictureBox)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String.Trim());

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentException)
            {
                MessageBox.Show("Base64 string'i geçerli bir görüntüye dönüştürülemedi: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görüntü dönüştürme sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Base64 string'i bayt dizisine dönüş
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Bayt dizisini bellek akışına yaz
                ms.Write(imageBytes, 0, imageBytes.Length);
                // Bellek akışından bir resim oluştur
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Fotoğrafı MemoryStream'e kaydet
                image.Save(ms, format);

                // MemoryStream'i byte array'e dönüştür
                byte[] imageBytes = ms.ToArray();

                // Byte array'i base64 string'e dönüştür
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            LogInMenu logInMenu = new LogInMenu();
            logInMenu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            else if (textBox6.Text != textBox7.Text)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }
            else if (!IsValidEmail(textBox3.Text))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }
            else if (textBox4.Text.Length != 15)
            {
                MessageBox.Show("Invalid phone number.");
                return;
            }
            else if(textBox1.Text.Contains(",") || textBox2.Text.Contains(",") || textBox3.Text.Contains(",") || textBox4.Text.Contains(",") || textBox5.Text.Contains(",") || textBox6.Text.Contains(",") || textBox7.Text.Contains(","))
            {
                MessageBox.Show("Fields cannot contain commas.");
                return;
            }
            foreach (var user in LogInMenu.users)
            {
                if (user.Email == textBox3.Text || user.Phone == textBox4.Text)
                {
                    MessageBox.Show("This email address or phone number is already registered.");
                    return;
                }
            }

            textBox4.Text = textBox4.Text.Replace("(", "").Replace(")", "").Replace(" ", "");
            User n = new User();
            n.Name = textBox1.Text;
            n.Surname = textBox2.Text;
            n.Email = textBox3.Text;
            n.Password = textBox6.Text;
            n.Phone = textBox4.Text;
            n.Address = textBox5.Text;
            n.Salary = "0";
            n.UserType = "User";
            n.Photo = ImageToBase64(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
            if (LogInMenu.users.Count == 0)
            {
                n.UserType = "Admin";
            }
            LogInMenu.users.Add(n);
            LogInMenu.WriteToCsv();
            MessageBox.Show("User registered successfully.");
            LogInMenu logInMenu = new LogInMenu();
            this.Hide();
            logInMenu.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Sadece resim dosyalarını göster
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                openFileDialog.Title = "Resim Dosyası Seç";

                // OpenFileDialog penceresini aç ve kullanıcı bir dosya seçerse devam et
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Seçilen dosyanın yolunu al
                    string selectedFilePath = openFileDialog.FileName;

                    // Resmi PictureBox'a yükle
                    pictureBox1.Image = Image.FromFile(selectedFilePath);

                    // Resmi Base64 string formatına çevir
                    string base64String = ImageToBase64(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
                    

                    // Base64 string'i kullanıcının Photo özelliğine ata
                    

                    // PictureBox'ı yenile
                    pictureBox1.Refresh();

                    // Güncellenmiş kullanıcı bilgilerini CSV dosyasına kaydet
                    LogInMenu.WriteToCsv();
                }
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Resources.profile_user_icon_isolated_on_white_background_eps10_free_vector;
        }

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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox6.PasswordChar == '\0') textBox6.PasswordChar = '*';
            else textBox6.PasswordChar = '\0';
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox7.PasswordChar == '\0') textBox7.PasswordChar = '*';
            else textBox7.PasswordChar = '\0';
        }
    }
}
