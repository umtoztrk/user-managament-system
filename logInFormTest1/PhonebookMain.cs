using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using logInFormTest1;
using MainMenu = logInFormTest1.MainMenu;

namespace oop2_proje_testProject2
{
    public partial class PhoneBookMain : Form
    {
        // CSV dosyasının yolu
        public static string dosyaYolu = "PhonebookRecords.csv";

        // Kayıtların listesi
        public static List<Record> records = new List<Record>();

        // Kayıtları dosyaya yazan metot
        public static void WriteToFile()
        {
            using (StreamWriter sw = new StreamWriter(dosyaYolu))
            {
                sw.WriteLine("Owner,Name,Surname,Phone,Address,Description,Mail");
                foreach (Record rec in records)
                {
                    sw.WriteLine(rec.toCsv());
                }
            }
        }

        // Dosyadan kayıtları okuyan metot
        public static void ReadFile()
        {
            records = new List<Record>();
            var notes = File.ReadAllLines(dosyaYolu);
            records = notes.Select(line => Record.fromCsv(line)).ToList();
        }

        // Mevcut satırın indeksini tutar
        public int currRowIndex;

        // Formun yapıcı metodu
        public PhoneBookMain()
        {
            InitializeComponent();
        }

        // Form yüklendiğinde çalışacak metot
        private void Form1_Load(object sender, EventArgs e)
        {
            if (LogInMenu.users[LogInMenu.currUserIndex].UserType == "Admin") button3.Visible = true;
            if (File.Exists(dosyaYolu))
            {
                ReadFile();
            }
            else
            {
                using (var writer = new StreamWriter(dosyaYolu))
                {
                    writer.WriteLine("Owner,Name,Surname,Phone,Address,Description,Mail");
                }
            }
        }

        // Kayıtları listeleyen butonun click olayı
        private void btnListRecords_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (records.Count == 0)
            {
                MessageBox.Show("There is no records to list.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (records[0].Address == "Address") records.RemoveAt(0);
            txtSearch.Enabled = true;
            dataGridView1.Enabled = true;
            foreach (Record rec in records)
            {
                if (rec.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                {
                    dataGridView1.Rows.Add(rec.Name, rec.Surname, rec.Phone, rec.Address, rec.Description, rec.Email);
                }
            }
            dataGridView1.ClearSelection();
        }

        // Yeni kayıt oluşturma butonunun click olayı
        private void btnCreateRecord_Click(object sender, EventArgs e)
        {
            PhoneBookAddUser addUser = new PhoneBookAddUser();
            addUser.ShowDialog();
        }

        // DataGridView hücresine tıklanma olayı
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count) // Geçerli satır indeksi olup olmadığını kontrol eder
            {
                currRowIndex = e.RowIndex;
                foreach (Record rc in records)
                {
                    // Tıklanan satırdaki verilerle kayıtları eşleştirir
                    if (rc.Name == dataGridView1.Rows[currRowIndex].Cells[0].Value.ToString() &&
                        rc.Surname == dataGridView1.Rows[currRowIndex].Cells[1].Value.ToString() &&
                        rc.Phone == dataGridView1.Rows[currRowIndex].Cells[2].Value.ToString() &&
                        rc.Address == dataGridView1.Rows[currRowIndex].Cells[3].Value.ToString() &&
                        rc.Description == dataGridView1.Rows[currRowIndex].Cells[4].Value.ToString() &&
                        rc.Email == dataGridView1.Rows[currRowIndex].Cells[5].Value.ToString())
                    {
                        currRowIndex = records.IndexOf(rc);
                        break;
                    }
                }
                // TextBox'ları günceller
                txtName.Text = records[currRowIndex].Name;
                txtSurname.Text = records[currRowIndex].Surname;
                txtNumber.Text = records[currRowIndex].Phone;
                txtAddress.Text = records[currRowIndex].Address;
                txtDescription.Text = records[currRowIndex].Description;
                txtMail.Text = records[currRowIndex].Email;
            }
        }

        // Kayıt düzenleme butonunun click olayı
        private void btnEdit_Click(object sender, EventArgs e)
        {
            string newTxtNumber = txtNumber.Text.Replace("(", "").Replace(")", "");
            if(txtName.Text.Contains(",") || txtSurname.Text.Contains(",") || txtNumber.Text.Contains(",") || txtAddress.Text.Contains(",") || txtDescription.Text.Contains(",") || txtMail.Text.Contains(","))
            {
                MessageBox.Show("Fields cannot contain commas."); 
                return;
            }
            if (records.Count == 0)
            {
                MessageBox.Show("There is no record to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtName.Text == "" || txtSurname.Text == "" || txtNumber.Text == "" || txtAddress.Text == "" || txtDescription.Text == "" || txtMail.Text == "")
            {
                MessageBox.Show("Please fill all boxes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtName.Text == records[currRowIndex].Name && txtSurname.Text == records[currRowIndex].Surname &&
                     newTxtNumber == records[currRowIndex].Phone && txtAddress.Text == records[currRowIndex].Address &&
                     txtDescription.Text == records[currRowIndex].Description && txtMail.Text == records[currRowIndex].Email)
            {
                MessageBox.Show("There is no change.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!IsValidEmail(txtMail.Text))
            {
                MessageBox.Show("Please enter a valid e-mail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtNumber.Text.Length < 12)
            {
                MessageBox.Show("Please enter a valid phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Kayıt güncellemesi
            Record record = new Record();
            record.Owner = LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname;
            record.Name = txtName.Text;
            record.Surname = txtSurname.Text;
            record.Phone = newTxtNumber;
            record.Address = txtAddress.Text;
            record.Description = txtDescription.Text;
            record.Email = txtMail.Text;
            records[currRowIndex] = record;
            WriteToFile();
            MessageBox.Show("Record has been updated succesfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // DataGridView'i güncelle
            dataGridView1.Rows.Clear();
            foreach (Record rec in records)
            {
                if (rec.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                {
                    dataGridView1.Rows.Add(rec.Name, rec.Surname, rec.Phone, rec.Address, rec.Description, rec.Email);
                }
            }
            txtSearch.Text = "";
        }

        // Kayıt silme butonunun click olayı
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string newTxtNumber = txtNumber.Text.Replace("(", "").Replace(")", "");
            if (records.Count == 0)
            {
                MessageBox.Show("There is no record to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtName.Text == "" || txtSurname.Text == "" || txtNumber.Text == "" || txtAddress.Text == "" || txtDescription.Text == "" || txtMail.Text == "")
            {
                MessageBox.Show("Please select a record to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtName.Text != records[currRowIndex].Name || txtSurname.Text != records[currRowIndex].Surname ||
                     newTxtNumber != records[currRowIndex].Phone || txtAddress.Text != records[currRowIndex].Address ||
                     txtDescription.Text != records[currRowIndex].Description || txtMail.Text != records[currRowIndex].Email)
            {
                MessageBox.Show("There is no record to match this infos.\nCheck your info above.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dg = MessageBox.Show("Are you sure to delete this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dg == DialogResult.No) return;
            else
            {
                records.RemoveAt(currRowIndex);
                WriteToFile();
                MessageBox.Show("Record has benn deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtName.Text = ""; txtSurname.Text = ""; txtNumber.Text = ""; txtAddress.Text = ""; txtDescription.Text = ""; txtMail.Text = "";

                // DataGridView'i güncelle
                dataGridView1.Rows.Clear();
                foreach (Record rec in records)
                {
                    if (rec.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                    {
                        dataGridView1.Rows.Add(rec.Name, rec.Surname, rec.Phone, rec.Address, rec.Description, rec.Email);
                    }
                }
            }
            txtSearch.Text = "";
        }

        // Telefon numarası textbox'ında değişiklik olayı
        private void txtNumber_TextChanged(object sender, EventArgs e)
        {
            // Telefon numarası biçimlendirme
            if (txtNumber.Text.Length == 3 && !txtNumber.Text.Contains("("))
            {
                txtNumber.Text = "(" + txtNumber.Text + ")";
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            else if (txtNumber.Text.Length == 2 && txtNumber.Text.Contains("("))
            {
                txtNumber.Text = txtNumber.Text.Replace("(", "").Replace(")", "");
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            else if (txtNumber.Text.Length > 3 && !txtNumber.Text.Contains("(") && !txtNumber.Text.Contains(")"))
            {
                string temp = txtNumber.Text;
                txtNumber.Text = "(" + temp.Substring(0, 3) + ")" + temp.Substring(3);
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
        }

        // E-posta adresi doğrulama metodu
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        // Telefon numarası textbox'ına giriş kontrolü
        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yalnızca rakam ve kontrol karakterlerine izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
            // Telefon numarası uzunluğunu sınırla
            if (txtNumber.TextLength >= 12 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // İsim textbox'ına giriş kontrolü
        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yalnızca harf, kontrol karakteri ve boşluk karakterlerine izin ver
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        // Soyisim textbox'ına giriş kontrolü
        private void txtSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yalnızca harf, kontrol karakteri ve boşluk karakterlerine izin ver
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        // Arama textbox'ına giriş kontrolü
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yalnızca harf, kontrol karakteri ve boşluk karakterlerine izin ver
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        // Arama textbox'ında değişiklik olayı
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string word = txtSearch.Text;
            string word2;
            if (word.Length > 1)
            {
                if (word[index: 0] == 'i')
                {
                    word2 = "I" + word.Substring(1);
                }
                else word2 = char.ToUpper(word[0]) + word.Substring(1);
            }
            else
            {
                if (word == "i") word2 = "I";
                else word2 = word.ToUpper();
            }

            // Arama sonucunu DataGridView'e ekle
            dataGridView1.Rows.Clear();
            foreach (var name in records)
            {
                if (name.Name.StartsWith(word2) || name.Name.StartsWith(word2.ToLower()) || name.Name.StartsWith(word2.ToUpper()))
                {
                    dataGridView1.Rows.Add(name.Name, name.Surname, name.Phone, name.Address, name.Description, name.Email);
                }
            }
            txtName.Text = ""; txtSurname.Text = ""; txtNumber.Text = ""; txtAddress.Text = ""; txtDescription.Text = ""; txtMail.Text = "";
            dataGridView1.ClearSelection();
        }

        // Ana menüye dönüş butonunun click olayı
        private void btnMail_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
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
                            sw.WriteLine("Owner,Name,Surname,Phone,Address,Description,Mail");
                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var record in records)
                            {
                                if(record.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname) sw.WriteLine(record.toCsv());
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

        private void button3_Click(object sender, EventArgs e)
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
                            sw.WriteLine("Owner,Name,Surname,Phone,Address,Description,Mail");

                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var record in records)
                            {
                                sw.WriteLine(record.toCsv());
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