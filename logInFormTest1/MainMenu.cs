using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oop2_proje_testProject2;
using Personal_Information;
using Reminder;
using salaryCalculator;
using WindowsFormsApp15;

namespace logInFormTest1
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent(); // bileşenleri başlat
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            // admin kullanıcıları için belirli etiketleri görünür yap
            if (LogInMenu.users[LogInMenu.currUserIndex].UserType == "Admin")
            {
                label6.Visible = true;
                label8.Visible = true;
            }
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // kullanıcı formu kapatıyorsa
            {
                DialogResult result = MessageBox.Show("Really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // çıkış onayı
                if (result == DialogResult.Yes) // evet ise
                {
                    Application.Exit(); // uygulamayı kapat
                }
                else
                {
                    e.Cancel = true; // kapatma işlemini iptal et
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LogInMenu logInMenu = new LogInMenu(); // giriş menüsüne dön
            this.Hide(); // mevcut formu gizle
            logInMenu.Show(); // giriş menüsünü göster
        }

        private void label1_Click(object sender, EventArgs e)
        {
            PhoneBookMain form = new PhoneBookMain(); // telefon rehberi formunu aç
            this.Hide(); // mevcut formu gizle
            form.Show(); // telefon rehberi formunu göster
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Notes notes = new Notes(); // notlar formunu aç
            this.Hide(); // mevcut formu gizle
            notes.Show(); // notlar formunu göster
        }

        private void label5_Click(object sender, EventArgs e)
        {
            ReminderMain reminderMain = new ReminderMain(); // hatırlatıcı formunu aç
            this.Hide(); // mevcut formu gizle
            reminderMain.Show(); // hatırlatıcı formunu göster
        }

        private void label4_Click(object sender, EventArgs e)
        {
            SalaryMain form = new SalaryMain(); // maaş hesaplayıcı formunu aç
            this.Hide(); // mevcut formu gizle
            form.Show(); // maaş hesaplayıcı formunu göster
        }

        private void label3_Click(object sender, EventArgs e)
        {
            PersonalInformation form = new PersonalInformation(); // kişisel bilgi formunu aç
            this.Hide(); // mevcut formu gizle
            form.Show(); // kişisel bilgi formunu göster
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.Hide(); // mevcut formu gizle
            UserManagement userManagement = new UserManagement(); // kullanıcı yönetim formunu aç
            userManagement.Show(); // kullanıcı yönetim formunu göster
        }
    }
}
