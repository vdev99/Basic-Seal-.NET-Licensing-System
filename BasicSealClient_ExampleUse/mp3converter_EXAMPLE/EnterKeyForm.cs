using System;
using System.Windows.Forms;
using BasicSealClient;

namespace mp3converter
{
    public partial class EnterKeyForm : Form
    {
        public EnterKeyForm()
        {
            InitializeComponent();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            string licKey = textBox1.Text;

            BasicSealResult result = await BasicSeal.ActivateLicense(licKey);

            if (result.successfulVerification)
            {
                this.Hide();
                var form2 = new mp3ConverterForm();
                form2.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("License Verification Not Passed");
            }
        }
    }
}
