using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using logInFormTest1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using MainMenu = logInFormTest1.MainMenu;

namespace Reminder
{
    public partial class ReminderMain : Form
    {
        public static string dosyaYolu;
        string dosyaAdi = "kayıt.csv"; // dosya adı

        public static List<ReminderInfo> records = new List<ReminderInfo>();
        private static List<Reminder> reminders = new List<Reminder>();
        private int currindex = -1;
        private bool isShaking = false;

        public static void WriteToFile()
        {
            using (StreamWriter sw = new StreamWriter(dosyaYolu))
            {
                // başlık satırını yaz
                sw.WriteLine("Owner,Description,Summary,Event,DateTime");

                // her ReminderInfo nesnesini CSV dosyasına yaz
                foreach (ReminderInfo rec in records)
                {
                    sw.WriteLine(rec.toCsv());
                }
            }
        }

        public static void ReadFile()
        {
            records = new List<ReminderInfo>();
            var lines = File.ReadAllLines(dosyaYolu);
            records = lines.Select(line => ReminderInfo.fromCsv(line)).ToList();
            if (File.Exists(dosyaYolu) && records.Count > 0 && records[0].Owner == "Owner") records.RemoveAt(0); // başlık satırını kaldır
        }

        public ReminderMain()
        {
            InitializeComponent();
            dateTimePickerReminder.Format = DateTimePickerFormat.Custom;
            dateTimePickerReminder.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.timerReminder.Interval = 1000; // her saniye kontrol et
            this.timerReminder.Tick += new System.EventHandler(this.timerReminder_Tick);
            this.timerReminder.Start();
        }

        // listede bir hatırlatıcı seçildiğinde detaylarını göster
        private void listBoxReminders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxReminders.SelectedIndex == -1)
            {
                return;
            }
            if (listBoxReminders.SelectedIndex != -1)
            {
                Reminder selectedReminder = reminders[listBoxReminders.SelectedIndex];
                MessageBox.Show($"Event: {selectedReminder.ReminderType}\nSummary: {selectedReminder.Summary}\nDescription: {selectedReminder.Description}\nTime: {selectedReminder.ReminderDateTime}", "Reminder Details");
            }
            int a = 0;
            foreach (var record in records)
            {
                if (record.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname && record.Summary == listBoxReminders.SelectedItem.ToString())
                {
                    currindex = a;
                    break;
                }
                a++;
            }
        }

        // hatırlatıcı listesini güncelle
        private void UpdateReminderList()
        {
            listBoxReminders.Items.Clear();
            foreach (Reminder reminder in reminders)
            {
                listBoxReminders.Items.Add(reminder.Summary);
            }
        }

        // timer kontrolü ile hatırlatıcıları tetikle
        private void timerReminder_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (Reminder reminder in reminders)
            {
                if (reminder.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname && !reminder.HasTriggered && now >= reminder.ReminderDateTime && now <= reminder.ReminderDateTime.AddSeconds(1))
                {
                    reminder.ShowReminder();
                    reminder.HasTriggered = true;
                }
            }
        }

        // yeni hatırlatıcı ekleme butonu
        private void btnAddReminder_Click(object sender, EventArgs e)
        {
            if(listBoxReminders.Items.Count == 0)
            {
                button4.Visible = true;
            }
            if (txtBoxDescription.Text.Contains(",") || txtBoxSummary.Text.Contains(","))
            {
                MessageBox.Show("Description and Summary fields cannot contain commas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button5.Visible = true;
                return;
            }
            string owner = LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname;
            string summary = txtBoxSummary.Text;
            string description = txtBoxDescription.Text;
            string reminderType = comboBoxReminderType.Text;
            DateTime datetime = dateTimePickerReminder.Value;

            ReminderInfo newRecord = new ReminderInfo
            {
                Owner = owner,
                Description = description,
                Summary = summary,
                Event = reminderType,
                datetime = datetime.ToString()
            };

            records.Add(newRecord);

            WriteToFile(); // dosyaya kaydet

            groupBox2.Visible = false;
            groupBox1.Visible = true;

            DateTime reminderDateTime = dateTimePickerReminder.Value;

            ReminderFactory factory = null;

            // hatırlatıcı tipine göre fabrika oluştur
            if (reminderType == "Meeting")
            {
                factory = new MeetingReminderFactory();
            }
            else if (reminderType == "Task")
            {
                factory = new TaskReminderFactory();
            }

            if (factory != null)
            {
                Reminder reminder = factory.CreateReminder(owner, reminderDateTime, summary, reminderType, description, this);
                reminders.Add(reminder);
                UpdateReminderList();
            }
            button5.Visible = true;
        }

        // hatırlatıcı silme butonu
        private void btnDeleteReminder_Click(object sender, EventArgs e)
        {
            
            if(listBoxReminders.Items.Count == 1)
            {
                button4.Visible = false;
            }
            if (listBoxReminders.SelectedIndex != -1)
            {
                // seçilen öğeyi listelerden kaldır
                reminders.RemoveAt(listBoxReminders.SelectedIndex);
                listBoxReminders.Items.RemoveAt(listBoxReminders.SelectedIndex);

                // dosyadan seçilen öğeyi kaldır
                DeleteRecord(currindex);
            }
        }

        // kaydı sil
        private void DeleteRecord(int index)
        {
            if (index >= 0 && index < records.Count)
            {
                records.RemoveAt(index);
                WriteToFile();
            }
        }

        // aktif pencereyi salla
        public void ShakeActiveWindow()
        {
            const int shakeAmplitude = 10;
            const int shakeDuration = 500;

            if (!isShaking)
            {
                isShaking = true;
                Form activeForm = Form.ActiveForm;

                if (activeForm != null)
                {
                    Point startPos = activeForm.Location;
                    Random rnd = new Random();

                    DateTime startTime = DateTime.Now;
                    while ((DateTime.Now - startTime).TotalMilliseconds < shakeDuration)
                    {
                        activeForm.Location = new Point(startPos.X + rnd.Next(-shakeAmplitude, shakeAmplitude),
                                                        startPos.Y + rnd.Next(-shakeAmplitude, shakeAmplitude));
                        activeForm.Refresh(); // pencereyi yenile
                        System.Threading.Thread.Sleep(20); // küçük bir gecikme ekle
                    }
                    activeForm.Location = startPos;
                }
                isShaking = false;
            }
        }

        private void txtBoxDescription_TextChanged(object sender, EventArgs e) { }

        // form yüklendiğinde çalışır
        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (LogInMenu.users[LogInMenu.currUserIndex].UserType == "Admin") button3.Visible = true;   
            dosyaYolu = Path.Combine(Application.StartupPath, dosyaAdi);
            if (File.Exists(dosyaYolu))
            {
                ReadFile();
                reminders.Clear();
                listBoxReminders.Items.Clear();

                // kayıtları ve hatırlatıcıları yükle
                foreach (var record in records)
                {
                    if (record.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                    {
                        string owner = record.Owner;
                        string summary = record.Summary;
                        string description = record.Description;
                        string reminderType = record.Event;
                        DateTime datetime = DateTime.Parse(record.datetime);

                        ReminderFactory factory = null;
                        if (reminderType == "Meeting")
                        {
                            factory = new MeetingReminderFactory();
                        }
                        else if (reminderType == "Task")
                        {
                            factory = new TaskReminderFactory();
                        }

                        if (factory != null)
                        {
                            Reminder reminder = factory.CreateReminder(owner, datetime, summary, reminderType, description, this);
                            reminders.Add(reminder);
                            listBoxReminders.Items.Add(summary);
                        }
                    }
                }
            }
            else // dosya yoksa oluştur
            {
                File.Create(dosyaYolu).Dispose();
            }
            if (listBoxReminders.Items.Count == 0)
            {
                button4.Visible = false;
            }
            else
            {
                button4.Visible = true;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e) { }

        // hatırlatıcı ekleme formunu göster
        private void ekle_Click(object sender, EventArgs e)
        {
            btnAddReminder.Visible = true;
            button5.Visible = false;
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            txtBoxDescription.Text = "";
            txtBoxSummary.Text = "";
            comboBoxReminderType.SelectedIndex = 0;
        }

        // ana menüye dön butonu
        private void button2_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            this.Hide();
            mainMenu.Show();
        }

        private void button1_Click(object sender, EventArgs e)
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
                            sw.WriteLine("Owner,Description,Summary,Event,datetime");

                            // Her User nesnesini CSV dosyasına yaz
                            foreach (var record in records)
                            {
                                if (record.Owner == LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname)
                                {
                                    sw.WriteLine(record.toCsv());
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
                            // Başlık satırını yaz
                            sw.WriteLine("Owner,Description,Summary,Event,datetime");

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

        private void button4_Click(object sender, EventArgs e)
        {
            btnAddReminder.Visible = false;
            if (currindex != -1)
            {
                groupBox1.Visible = false;
                groupBox2.Visible = true;
                txtBoxDescription.Text = records[currindex].Description;
                txtBoxSummary.Text = records[currindex].Summary;
                if (records[currindex].Event == "Meeting") comboBoxReminderType.SelectedIndex = 0;
                else comboBoxReminderType.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show("Please select a reminder to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //update butonu
        private void button5_Click(object sender, EventArgs e)
        {
            if (txtBoxDescription.Text == reminders[listBoxReminders.SelectedIndex].Description && txtBoxSummary.Text == reminders[listBoxReminders.SelectedIndex].Summary && comboBoxReminderType.Text == reminders[listBoxReminders.SelectedIndex].ReminderType)
            {
                MessageBox.Show("No changes have been made.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBoxReminders.SelectedIndex != -1)
            {
                Reminder selectedReminder = reminders[listBoxReminders.SelectedIndex];

                string owner = LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname;
                string summary = txtBoxSummary.Text;
                string description = txtBoxDescription.Text;
                string reminderType = comboBoxReminderType.Text;
                DateTime datetime = dateTimePickerReminder.Value;

                // Hatırlatıcıyı güncelle
                selectedReminder.Owner = owner;
                selectedReminder.Summary = summary;
                selectedReminder.ReminderType = reminderType;
                selectedReminder.Description = description;
                selectedReminder.ReminderDateTime = datetime;
                selectedReminder.HasTriggered = false; // Güncellemeden sonra tetiklenmemiş olarak işaretle

                // Kayıtlar listesinde güncelle
                records[currindex] = new ReminderInfo
                {
                    Owner = owner,
                    Description = description,
                    Summary = summary,
                    Event = reminderType,
                    datetime = datetime.ToString()
                };

                WriteToFile(); // Dosyaya kaydet

                UpdateReminderList(); // Listeyi güncelle

                MessageBox.Show("Reminder updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a reminder to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnAddReminder.Visible = true;
            groupBox2.Visible = false;
            groupBox1.Visible = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            button5.Visible = true;
            groupBox2.Visible = false;
            groupBox1.Visible = true;
        }
    }

    // abstract fabrika sınıfı
    public abstract class ReminderFactory
    {
        public abstract Reminder CreateReminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm);
    }

    // toplantı hatırlatıcısı fabrikası
    public class MeetingReminderFactory : ReminderFactory
    {
        public override Reminder CreateReminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm)
        {
            return new MeetingReminder(owner, reminderDateTime, summary, remindertype, description, mainForm);
        }
    }

    // görev hatırlatıcısı fabrikası
    public class TaskReminderFactory : ReminderFactory
    {
        public override Reminder CreateReminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm)
        {
            return new TaskReminder(owner, reminderDateTime, summary, remindertype, description, mainForm);
        }
    }

    // abstract hatırlatıcı sınıfı
    public abstract class Reminder
    {
        public string Owner { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public string Summary { get; set; }
        public string ReminderType { get; set; }
        public string Description { get; set; }
        protected ReminderMain MainForm { get; set; }
        public bool HasTriggered { get; set; } = false;

        public Reminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm)
        {
            Owner = owner;
            ReminderDateTime = reminderDateTime;
            Summary = summary;
            ReminderType = remindertype;
            Description = description;
            MainForm = mainForm;
        }

        // hatırlatıcı göster metodu
        public abstract void ShowReminder();
    }

    // toplantı hatırlatıcısı
    public class MeetingReminder : Reminder
    {
        public MeetingReminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm)
            : base(owner, reminderDateTime, summary, remindertype, description, mainForm)
        {
        }

        public override void ShowReminder()
        {
            MainForm.ShakeActiveWindow();
            MessageBox.Show($"Meeting Reminder: {Summary}\nDescription: {Description}\nTime: {ReminderDateTime}", "Reminder");
            MainForm.ShakeActiveWindow();
        }
    }

    // görev hatırlatıcısı
    public class TaskReminder : Reminder
    {
        public TaskReminder(string owner, DateTime reminderDateTime, string summary, string remindertype, string description, ReminderMain mainForm)
            : base(owner, reminderDateTime, summary, remindertype, description, mainForm)
        {
        }

        public override void ShowReminder()
        {
            MainForm.ShakeActiveWindow();
            MessageBox.Show($"Task Reminder: {Summary}\nDescription: {Description}\nTime: {ReminderDateTime}", "Reminder");
            MainForm.ShakeActiveWindow();
        }
    }
}