using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using logInFormTest1;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace oop2_proje_testProject2
{
    public partial class PhoneBookAddUser : Form
    {
        // telefon numarası formatı için bayraklar
        private bool flag1 = false; // ilk boşluğu eklemek için bayrak
        private bool flag2 = false; // ikinci boşluğu eklemek için bayrak

        // geçerli bir e-posta adresi olup olmadığını kontrol eden metod
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        public PhoneBookAddUser()
        {
            InitializeComponent(); // bileşenleri başlat
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // giriş alanları için doğrulama kontrolleri
            if (txtName.Text == "" || txtName.Text.Length < 3 || txtSurname.Text.Length < 3 || txtSurname.Text == "" || txtNumber.Text == "" || txtAddress.Text == "" || txtDescription.Text == "" || txtMail.Text == "")
            {
                MessageBox.Show("lütfen metin kutularını eksiksiz ve doğru bir şekilde doldurunuz", "hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!IsValidEmail(txtMail.Text))
            {
                MessageBox.Show("lütfen geçerli bir e-posta adresi giriniz.", "hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtNumber.Text.Length < 13)
            {
                MessageBox.Show("lütfen geçerli bir telefon numarası giriniz.", "hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if(txtName.Text.Contains(",") || txtSurname.Text.Contains(",") || txtNumber.Text.Contains(",") || txtAddress.Text.Contains(",") || txtDescription.Text.Contains(",") || txtMail.Text.Contains(","))
            {
                MessageBox.Show("Fields cannot contain commas.");
                return;
            }
            // telefon numarasındaki formatlama karakterlerini kaldır
            txtNumber.Text = txtNumber.Text.Replace("(", "").Replace(")", "").Replace(" ", "");

            // giriş verileriyle yeni bir record nesnesi oluştur
            Record rc = new Record
            {
                Owner = LogInMenu.users[LogInMenu.currUserIndex].Name + " " + LogInMenu.users[LogInMenu.currUserIndex].Surname,
                Name = txtName.Text,
                Surname = txtSurname.Text,
                Phone = txtNumber.Text,
                Address = txtAddress.Text,
                Description = txtDescription.Text,
                Email = txtMail.Text
            };

            // record'u PhoneBookMain'in kayıtlar listesine ekle ve dosyaya yaz
            PhoneBookMain.records.Add(rc);
            PhoneBookMain.WriteToFile();

            // başarı mesajını göster ve formu kapat
            MessageBox.Show("kayıt başarıyla oluşturuldu", "başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // isim girişi için sadece harfler, kontrol tuşları, boşluklar ve noktaları izin ver
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            // soyisim girişi için sadece harfler, kontrol tuşları, boşluklar ve noktaları izin ver
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // telefon numarası girişi için sadece rakamlar, kontrol tuşları ve boşlukları izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
            // telefon numarası girişini 15 karakterle sınırla
            if (txtNumber.TextLength >= 15 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNumber_TextChanged(object sender, EventArgs e)
        {
            // telefon numarasını parantezler ve boşluklarla biçimlendir
            if (txtNumber.Text.Length == 3 && !txtNumber.Text.Contains("("))
            {
                txtNumber.Text = "(" + txtNumber.Text + ") ";
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            else if (txtNumber.Text.Length == 2 && txtNumber.Text.Contains("("))
            {
                txtNumber.Text = txtNumber.Text.Replace("(", "").Replace(")", "");
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            if (txtNumber.Text.Length == 9 && !flag1)
            {
                flag1 = true;
                txtNumber.Text = txtNumber.Text + " ";
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            else if (txtNumber.Text.Length == 9 && flag1)
            {
                flag1 = false;
            }
            if (txtNumber.Text.Length == 12 && !flag2)
            {
                flag2 = true;
                txtNumber.Text = txtNumber.Text + " ";
                txtNumber.SelectionStart = txtNumber.Text.Length;
            }
            else if (txtNumber.Text.Length == 12 && flag2)
            {
                flag2 = false;
            }
        }
    }
}
