using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using logInFormTest1;
using oop2_proje_testProject2;
using MainMenu = logInFormTest1.MainMenu;

namespace Personal_Information
{
    public partial class PersonalInformation : Form
    {
        private bool showPassword = false;
        private Stack<User> undoStack = new Stack<User>();
        private Stack<User> redoStack = new Stack<User>();

        public PersonalInformation()
        {
            InitializeComponent();
            if (LogInMenu.users.Count > 0) // users listesinin boş olmadığını kontrol et
            {
                BilgileriAta();
            }
            else
            {
                MessageBox.Show("User listesi boş.");
            }

            textBox1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            hidePswd();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo();
                WriteToCsv();
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo();
                WriteToCsv();
            }
        }

        public void ConvertBase64ToImageAndDisplay(string base64String, PictureBox pictureBox)
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

        public void BilgileriAta()
        {
            if (LogInMenu.users.Count > 0)
            {
                label1.Text = LogInMenu.users[LogInMenu.currUserIndex].Name;
                label2.Text = LogInMenu.users[LogInMenu.currUserIndex].Surname;
                label3.Text = LogInMenu.users[LogInMenu.currUserIndex].Phone;
                label4.Text = LogInMenu.users[LogInMenu.currUserIndex].Address;
                label5.Text = LogInMenu.users[LogInMenu.currUserIndex].Email;
                label7.Text = LogInMenu.users[LogInMenu.currUserIndex].Salary;
                ConvertBase64ToImageAndDisplay(LogInMenu.users[LogInMenu.currUserIndex].Photo, pictureBox1);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Refresh();
                pictureBox1.Show();

                // Şifrenin gizlenmesi için hidePswd metodunu çağırın
                hidePswd();
            }
            else
            {
                MessageBox.Show("User listesi boş.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
            button2.Visible = true;
            label10.Visible = false;
            if (radioButton1.Checked)
            {
                button3.Visible = false;
                label8.Text = "Lütfen yeni isim bilgisini giriniz.";
                label8.Visible = true;
            }
            else if (radioButton2.Checked)
            {
                button3.Visible = false;
                label8.Text = "Lütfen yeni soyisim bilgisini giriniz.";
                label8.Visible = true;
            }
            else if (radioButton3.Checked)
            {
                label10.Visible = true;
                button3.Visible = false;
                label8.Text = "Lütfen yeni telefon numaranızı giriniz.";
                label8.Visible = true;
            }
            else if (radioButton4.Checked)
            {
                button3.Visible = false;
                label8.Text = "Lütfen yeni adres bilginizi giriniz.";
                label8.Visible = true;
            }
            else if (radioButton5.Checked)
            {
                button3.Visible = false;
                label8.Text = "Lütfen yeni e-posta adresinizi giriniz.";
                label8.Visible = true;
            }
            else if (radioButton6.Checked)
            {
                button3.Visible = false;
                label8.Text = "Lütfen yeni şifrenizi giriniz.";
                label8.Visible = true;
            }
            else if (radioButton7.Checked)
            {
                button3.Visible = true;
                button2.Visible = false;
                textBox1.Visible = false;
                label8.Text = "Lütfen yeni profil fotoğrafınızı yükleyiniz.";
                label8.Visible = true;
            }
            else
            {
                textBox1.Visible = false;
                button2.Visible = false;
                MessageBox.Show("Lütfen değiştirmek istediğiniz bilgiyi seçiniz.","Hata");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
                    SaveState();
                    LogInMenu.users[LogInMenu.currUserIndex].Photo = base64String;

                    // PictureBox'ı yenile
                    pictureBox1.Refresh();

                    // Güncellenmiş kullanıcı bilgilerini CSV dosyasına kaydet
                    WriteToCsv();
                }
            }
        }

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Fotoğrafı MemoryStream'e kaydedin
                image.Save(ms, format);

                // MemoryStream'i byte array'e dönüştürün
                byte[] imageBytes = ms.ToArray();

                // Byte array'i base64 string'e dönüştürün
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private void showPswd()
        {
            label9.Text = LogInMenu.users[LogInMenu.currUserIndex].Password;
            showPassword = true;
        }

        private void hidePswd()
        {
            label9.Text = new string('*', LogInMenu.users[LogInMenu.currUserIndex].Password.Length);
            showPassword = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (showPassword)
            {
                hidePswd();
            }
            else
            {
                showPswd();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains(",")) {
                MessageBox.Show("Field cannot contains commas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveState();

            if (radioButton1.Checked)
            {
                label10.Visible = false;
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir isim giriniz.");
                }
                else
                {
                    LogInMenu.users[LogInMenu.currUserIndex].Name = textBox1.Text;
                    label1.Text = textBox1.Text;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton2.Checked)
            {
                label10.Visible = false;
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir soyisim giriniz.");
                }
                else
                {
                    LogInMenu.users[LogInMenu.currUserIndex].Surname = textBox1.Text;
                    label2.Text = textBox1.Text;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton3.Checked)
            {
                
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir telefon giriniz.");
                }
                else if (textBox1.Text.Length != 10)
                {
                    MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz.");
                }
                else
                {
                    label10.Visible = false;
                    LogInMenu.users[LogInMenu.currUserIndex].Phone = textBox1.Text;
                    label3.Text = textBox1.Text;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton4.Checked)
            {
                label10.Visible = false;
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir adres giriniz.");
                }
                else
                {
                    LogInMenu.users[LogInMenu.currUserIndex].Address = textBox1.Text;
                    label4.Text = textBox1.Text;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton5.Checked)
            {
                label10.Visible = false;
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir e-posta giriniz.");
                }
                else
                {
                    LogInMenu.users[LogInMenu.currUserIndex].Email = textBox1.Text;
                    label5.Text = textBox1.Text;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton6.Checked)
            {
                label10.Visible = false;
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Lütfen bir şifre giriniz.");
                }
                else
                {
                    LogInMenu.users[LogInMenu.currUserIndex].Password = textBox1.Text;
                    hidePswd(); // Şifreyi gizle
                    textBox1.Clear();
                    textBox1.Visible = false;
                    button2.Visible = false;
                    label8.Visible = false;
                    WriteToCsv();
                }
            }
            else if (radioButton7.Checked)
            {
                label10.Visible = false;
                if (pictureBox1.Image != null)
                {
                    string base64String = ImageToBase64(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
                    LogInMenu.users[LogInMenu.currUserIndex].Photo = base64String;
                    pictureBox1.Refresh();
                    WriteToCsv();
                }
                else
                {
                    MessageBox.Show("Lütfen bir fotoğraf seçiniz.");
                }
            }
        }

        private void WriteToCsv()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "bilgiler.csv";
            string filePath = Path.Combine(currentDirectory, fileName);

            // Dosyayı yazma modunda aç
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
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton3.Checked)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }

                if (textBox1.Text.Length >= 10 && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void textBox1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (radioButton5.Checked)
            {
                string email = textBox1.Text;

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Lütfen geçerli bir e-posta adresi girin.", "Geçersiz E-posta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Odaktan çıkmayı iptal et
                }
            }
        }

        private void SaveState()
        {
            if (LogInMenu.users.Count > 0)
            {
                // Kullanıcının mevcut durumunu yığın üzerine kaydet
                User currentUser = CloneUser(LogInMenu.users[LogInMenu.currUserIndex]);
                undoStack.Push(currentUser);
                redoStack.Clear(); // Her yeni değişiklikte ileri yığını temizle
            }
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                // Şu anki durumu ileri yığınına kaydet
                SaveStateForRedo();

                // Geri alma işlemi
                User previousState = undoStack.Pop();
                redoStack.Push(CloneUser(LogInMenu.users[LogInMenu.currUserIndex])); // Mevcut durumu ileri yığınına kaydet
                LogInMenu.users[LogInMenu.currUserIndex] = previousState;

                // Güncellenmiş bilgileri formda göster
                BilgileriAta();
            }
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                // Şu anki durumu geri yığınına kaydet
                SaveStateForUndo();

                // İleri alma işlemi
                User nextState = redoStack.Pop();
                LogInMenu.users[LogInMenu.currUserIndex] = nextState;

                // Güncellenmiş bilgileri formda göster
                BilgileriAta();
            }
        }

        private void SaveStateForRedo()
        {
            if (LogInMenu.users.Count > 0)
            {
                // Kullanıcının mevcut durumunu yığın üzerine kaydet
                User currentUser = CloneUser(LogInMenu.users[LogInMenu.currUserIndex]);
                redoStack.Push(currentUser);
            }
        }

        private void SaveStateForUndo()
        {
            if (LogInMenu.users.Count > 0)
            {
                // Kullanıcının mevcut durumunu yığın üzerine kaydet
                User currentUser = CloneUser(LogInMenu.users[LogInMenu.currUserIndex]);
                undoStack.Push(currentUser);
            }
        }

        private User CloneUser(User user)
        {
            return new User
            {
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                Password = user.Password,
                Salary = user.Salary,
                UserType = user.UserType,
                Photo = user.Photo
            };
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            this.Hide();
            mainMenu.Show();
        }

        private void button6_Click(object sender, EventArgs e)
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
                                if(user.Email == LogInMenu.users[LogInMenu.currUserIndex].Email && user.Phone == LogInMenu.users[LogInMenu.currUserIndex].Phone) sw.WriteLine(user.ToCsv());
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
