using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using MainMenu = logInFormTest1.MainMenu;
using logInFormTest1;
using oop2_proje_testProject2;
using static System.Windows.Forms.LinkLabel;

namespace WindowsFormsApp15
{
    public partial class Notes : Form
    {
        public static List<NotesInfo> notesInfos = new List<NotesInfo>(); // not bilgileri listesi
        private string filePath = "notlar.csv"; // csv dosya yolu
        private int selectedIndex; // seçili notun index'i
        public Notes()
        {
            InitializeComponent();
            this.BackColor = Color.Wheat; // arka plan rengini ayarla
            textBox1.Multiline = true; // textBox1'i çok satırlı yap
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (LogInMenu.users[LogInMenu.currUserIndex].UserType == "Admin") button6.Visible = true;
            LoadNotes(); // notları yükle
        }

        private void LoadNotes()
        {
            listBox1.Items.Clear(); // listbox'ı temizle

            if (File.Exists(filePath)) // dosya varsa
            {
                notesInfos.Clear(); // not listesi temizle
                var notes = File.ReadAllLines(filePath);
                notesInfos = notes.Skip(1)
                    .Select(line => NotesInfo.FromCsv(line))
                    .ToList(); // dosyadan notları yükle
                foreach (var note in notesInfos)
                {
                    if (note.Owner == "Owner") continue; // sahip adı "Owner" olan notları atla
                    else if(note.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname) listBox1.Items.Add($"{note.Date},\"{note.Note}\""); // notları listbox'a ekle
                }
            }
            else
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Owner,Date,Note"); // csv başlık satırını yaz
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(textBox1.Text.Contains(",")) // metin içinde virgül varsa
            {
                MessageBox.Show("Metin içinde virgül kullanılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string text = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(text)) // metin boş değilse
            {
                string csvLine = $"{LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname},{DateTime.Now},\"{text}\"";
                notesInfos.Add(new NotesInfo
                {
                    Owner = LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname,
                    Date = DateTime.Now,
                    Note = text
                });
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        string header = "Owner,Date,Note";
                        writer.WriteLine(header); // başlık satırını yaz
                        foreach (var user in notesInfos)
                        {
                            writer.WriteLine(user.ToCsv()); // notları csv dosyasına yaz
                        }
                    }

                    listBox1.Items.Add($"{DateTime.Now},\"{text}\""); // yeni notu listbox'a ekle
                    textBox1.Clear(); // textbox'ı temizle

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Not kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); // hata mesajı göster
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir metin girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); // uyarı mesajı göster
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) // seçili not yoksa
            {
                MessageBox.Show("Lütfen silmek istediğiniz notu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); // uyarı mesajı göster
                return;
            }
            try
            {
                notesInfos.RemoveAt(selectedIndex); // notu listeden kaldır
                string[] notes = new string[notesInfos.Count + 1];
                notes[0] = "Owner,Date,Note"; // csv başlık satırı
                for (int i = 0; i < notesInfos.Count; i++)
                {
                    notes[i + 1] = notesInfos[i].ToCsv();
                }

                File.WriteAllLines(filePath, notes); // dosyayı güncelle
                LoadNotes(); // notları yeniden yükle
                textBox1.Clear(); // textbox'ı temizle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Not silinirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); // hata mesajı göster
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) // seçili not yoksa
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz notu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); // uyarı mesajı göster
                return;
            }
            if (textBox1.Text.Contains(",")) // metin içinde virgül varsa
            {
                MessageBox.Show("Metin içinde virgül kullanılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string newText = textBox1.Text;
            if (string.IsNullOrWhiteSpace(newText)) // yeni metin boşsa
            {
                MessageBox.Show("Lütfen bir metin girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); // uyarı mesajı göster
                return;
            }
            if(textBox1.Text == listBox1.SelectedItem.ToString().Split(new[] { ',' }, 2)[1].Trim('\"')) // metin değişmediyse
            {
                MessageBox.Show("Metin değişmedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return;
            }

            try
            {
                notesInfos[selectedIndex].Note = newText; // notu güncelle
                notesInfos[selectedIndex].Date = DateTime.Now; // tarihi güncelle
                string[] notes = new string[notesInfos.Count + 1];
                notes[0] = "Owner,Date,Note"; // csv başlık satırı
                for (int i = 0; i < notesInfos.Count; i++)
                {
                    notes[i + 1] = notesInfos[i].ToCsv();
                }

                File.WriteAllLines(filePath, notes); // dosyayı güncelle
                LoadNotes(); // notları yeniden yükle
                textBox1.Clear(); // textbox'ı temizle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Not güncellenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); // hata mesajı göster
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedNote = listBox1.SelectedItem.ToString();
                string[] noteParts = selectedNote.Split(new[] { ',' }, 2);
                if (noteParts.Length > 1)
                {
                    textBox1.Text = noteParts[1].Trim('\"'); // notun metin kısmını al
                }
            }
            foreach(var note in notesInfos)
            {
                if(note.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname && note.Note == textBox1.Text)
                {
                    selectedIndex = notesInfos.IndexOf(note); // seçili notun index'ini al
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu(); // ana menüye dön
            this.Hide(); // mevcut formu gizle
            mainMenu.Show(); // ana menüyü göster
        }

        private void button5_Click(object sender, EventArgs e)
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
                            sw.WriteLine("Owner,Date,Note");

                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var note in notesInfos)
                            {
                                if (note.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                                {
                                    sw.WriteLine(note.ToCsv());
                                }
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
                            sw.WriteLine("Owner,Date,Note");

                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var note in notesInfos)
                            {
                                sw.WriteLine(note.ToCsv());
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Enter tuşunu işleme al
            }
        }
    }
}
