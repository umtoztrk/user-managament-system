using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using logInFormTest1;
using MainMenu = logInFormTest1.MainMenu;

namespace salaryCalculator
{
    public partial class SalaryMain : Form
    {
        double bazUcret = 40005; // başlangıç ücreti
        int yas1;
        int yas2;

        public SalaryMain()
        {
            InitializeComponent();
            // deneyim süresi combobox'a öğeler ekleme
            comboBox1.Items.Add("2-4");
            comboBox1.Items.Add("5-9");
            comboBox1.Items.Add("10-14");
            comboBox1.Items.Add("15-20");
            comboBox1.Items.Add("20 yıl üstü");
            // şehir combobox'a öğeler ekleme
            comboBox2.Items.Add("İstanbul");
            comboBox2.Items.Add("Ankara");
            comboBox2.Items.Add("İzmir");
            comboBox2.Items.Add("Kocaeli, Sakarya, Düzce, Bolu, Yalova");
            comboBox2.Items.Add("Edirne, Kırklareli, Tekirdağ");
            comboBox2.Items.Add("Trabzon, Ordu, Giresun, Rize, Artvin, Gümüşhane");
            comboBox2.Items.Add("Bursa, Eskişehir, Bilecik");
            comboBox2.Items.Add("Aydın, Denizli, Muğla");
            comboBox2.Items.Add("Adana, Mersin");
            comboBox2.Items.Add("Balıkesir, Çanakkale");
            comboBox2.Items.Add("Antalya, Isparta, Burdur");
            comboBox2.Items.Add("Diğer İller");
            // eğitim combobox'a öğeler ekleme
            comboBox3.Items.Add("Meslek alanı ile ilgili yüksek lisans");
            comboBox3.Items.Add("Meslek alanı ile ilgili doktora");
            comboBox3.Items.Add("Meslek alanı ile ilgili doçentlik");
            comboBox3.Items.Add("Meslek alanı ile ilgili olmayan yüksek lisans");
            comboBox3.Items.Add("Meslek alanı ile ilgili olmayan doktora/doçentlik");
            // pozisyon combobox'a öğeler ekleme
            comboBox4.Items.Add("Takım Lideri/Grup Yöneticisi/Teknik Yönetici/Yazılım Mimarı");
            comboBox4.Items.Add("Proje Yöneticisi");
            comboBox4.Items.Add("Direktör/Projeler Yöneticisi");
            comboBox4.Items.Add("CTO/Genel Müdür");
            comboBox4.Items.Add("Bilgi İşlem Sorumlusu/Müdürü(En çok 5 bilişim personeli varsa)");
            comboBox4.Items.Add("Bilgi İşlem Sorumlusu/Müdürü(5'ten çok bilişim personeli varsa)");
        }

        // deneyim süresine göre katsayı döndüren metot
        public double deneyim()
        {
            if (comboBox1.SelectedItem == null)
            {
                return 0;
            }
            else
            {
                if (comboBox1.SelectedItem.ToString() == "2-4")
                {
                    return 0.6;
                }
                else if (comboBox1.SelectedItem.ToString() == "5-9")
                {
                    return 1;
                }
                else if (comboBox1.SelectedItem.ToString() == "10-14")
                {
                    return 1.2;
                }
                else if (comboBox1.SelectedItem.ToString() == "15-20")
                {
                    return 1.35;
                }
                else if (comboBox1.SelectedItem.ToString() == "20 yıl üstü")
                {
                    return 1.5;
                }
            }
            return 0;
        }

        // şehre göre katsayı döndüren metot
        public double sehir()
        {
            if (comboBox2.SelectedItem == null)
            {
                return 0;
            }
            else
            {
                if (comboBox2.SelectedItem.ToString() == "İstanbul")
                {
                    return 0.3;
                }
                else if (comboBox2.SelectedItem.ToString() == "Ankara" || comboBox2.SelectedItem.ToString() == "İzmir")
                {
                    return 0.2;
                }
                else if (comboBox2.SelectedItem.ToString() == "Kocaeli, Sakarya, Düzce, Bolu, Yalova" || comboBox2.SelectedItem.ToString() == "Edirne, Kırklareli, Tekirdağ")
                {
                    return 0.1;
                }
                else if (comboBox2.SelectedItem.ToString() == "Trabzon, Ordu, Giresun, Rize, Artvin, Gümüşhane" || comboBox2.SelectedItem.ToString() == "Bursa, Eskişehir, Bilecik" || comboBox2.SelectedItem.ToString() == "Aydın, Denizli, Muğla" || comboBox2.SelectedItem.ToString() == "Adana, Mersin" || comboBox2.SelectedItem.ToString() == "Balıkesir, Çanakkale" || comboBox2.SelectedItem.ToString() == "Antalya, Isparta, Burdur")
                {
                    return 0.05;
                }
                else if (comboBox2.SelectedItem.ToString() == "Diğer İller")
                {
                    return 0;
                }
            }
            return 0;
        }

        // eğitime göre katsayı döndüren metot
        public double egitim()
        {
            if (comboBox3.SelectedItem == null)
            {
                return 0;
            }
            else
            {
                if (comboBox3.SelectedItem.ToString() == "Meslek alanı ile ilgili yüksek lisans")
                {
                    return 0.1;
                }
                else if (comboBox3.SelectedItem.ToString() == "Meslek alanı ile ilgili doktora")
                {
                    return 0.3;
                }
                else if (comboBox3.SelectedItem.ToString() == "Meslek alanı ile ilgili doçentlik")
                {
                    return 0.35;
                }
                else if (comboBox3.SelectedItem.ToString() == "Meslek alanı ile ilgili olmayan yüksek lisans")
                {
                    return 0.05;
                }
                else if (comboBox3.SelectedItem.ToString() == "Meslek alanı ile ilgili olmayan doktora/doçentlik")
                {
                    return 0.15;
                }
            }
            return 0;
        }

        // pozisyona göre katsayı döndüren metot
        public double pozisyon()
        {
            if (comboBox4.SelectedItem == null)
            {
                return 0;
            }
            else
            {
                if (comboBox4.SelectedItem.ToString() == "Takım Lideri/Grup Yöneticisi/Teknik Yönetici/Yazılım Mimarı")
                {
                    return 0.5;
                }
                else if (comboBox4.SelectedItem.ToString() == "Proje Yöneticisi")
                {
                    return 0.75;
                }
                else if (comboBox4.SelectedItem.ToString() == "Direktör/Projeler Yöneticisi")
                {
                    return 0.85;
                }
                else if (comboBox4.SelectedItem.ToString() == "CTO/Genel Müdür")
                {
                    return 1;
                }
                else if (comboBox4.SelectedItem.ToString() == "Bilgi İşlem Sorumlusu/Müdürü(En çok 5 bilişim personeli varsa)")
                {
                    return 0.4;
                }
                else if (comboBox4.SelectedItem.ToString() == "Bilgi İşlem Sorumlusu/Müdürü(5'ten çok bilişim personeli varsa)")
                {
                    return 0.6;
                }
            }
            return 0;
        }

        // aile durumuna göre katsayı döndüren metot
        public double aile()
        {
            double katsayi1 = 0;
            double katsayi2 = 0;
            if (checkBox1.Checked == true)
            {
                katsayi1 += 0.2;
            }
            if (textBox1.Text != "")
            {
                yas1 = Convert.ToInt32(textBox1.Text);
                if (0 < yas1 && yas1 <= 6)
                {
                    katsayi1 += 0.2;
                }
                else if (7 <= yas1 && yas1 < 18)
                {
                    katsayi1 += 0.3;
                }
                else if (18 <= yas1)
                {
                    katsayi1 += 0.4;
                }
            }
            if (textBox2.Text != "")
            {
                yas2 = Convert.ToInt32(textBox2.Text);
                if (0 < yas2 && yas2 <= 6)
                {
                    katsayi2 += 0.2;
                }
                else if (7 <= yas2 && yas2 < 18)
                {
                    katsayi2 += 0.3;
                }
                else if (18 <= yas2)
                {
                    katsayi2 += 0.4;
                }
            }
            return katsayi1 + katsayi2;
        }

        // dil durumuna göre katsayı döndüren metot
        public double dil()
        {
            double katsayi1 = 0;
            double dilSayisi = 0;

            if (checkBox3.Checked == true)
            {
                katsayi1 += 0.2;
            }
            if (checkBox4.Checked == true)
            {
                katsayi1 += 0.2;
            }
            if (textBox3.Visible == true && textBox3.Text != "")
            {
                dilSayisi = Convert.ToInt32(textBox3.Text);
                katsayi1 += dilSayisi * 0.05;
            }
            return katsayi1;
        }

        // hesapla butonunun tıklanma olayı
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir öğe seçin.");
            }
            else
            {
                double deneyimSuresi = deneyim();
                double il = sehir();
                double egitimDerecesi = egitim();
                double pozisyonDurumu = pozisyon();
                double aileDurumu = aile();
                double dilDurumu = dil();
                double toplam = bazUcret * (1 + deneyimSuresi + il + egitimDerecesi + pozisyonDurumu + aileDurumu + dilDurumu);
                if (LogInMenu.users[LogInMenu.currUserIndex].UserType == "Part-Time User")
                {
                    toplam /= 2;
                }
                label2.Text = toplam.ToString();
                label2.Text = label2.Text.Replace(",", ".");
                label11.Visible = true;
                LogInMenu.users[LogInMenu.currUserIndex].Salary = label2.Text;
                LogInMenu.WriteToCsv();
            }
        }

        // çocuk var mı checkbox'ı değiştiğinde
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                label9.Visible = true;
                label10.Visible = true;
                textBox1.Visible = true;
            }
            if (checkBox2.Checked == false)
            {
                label9.Visible = false;
                label10.Visible = false;
                textBox1.Visible = false;
                if (textBox2.Visible == true)
                {
                    textBox2.Visible = false;
                }
            }
        }

        // ilk çocuğun yaşına göre ikinci çocuk textbox'ını görünür yapar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Visible == true && textBox1.Text != "")
            {
                textBox2.Visible = true;
            }
            if (textBox1.Visible == true && textBox1.Text == "")
            {
                textBox2.Visible = false;
            }
        }

        // yabancı dil checkbox'ı değiştiğinde
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                label8.Visible = true;
                textBox3.Visible = true;
            }
            if (checkBox5.Checked == false)
            {
                label8.Visible = false;
                textBox3.Visible = false;
            }
        }

        // sadece rakam girişine izin verir
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // sadece rakam girişine izin verir
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // sadece rakam girişine izin verir
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        // ana menüye dön butonu
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenu mainMenu = new MainMenu();
            mainMenu.ShowDialog();
        }
    }
}