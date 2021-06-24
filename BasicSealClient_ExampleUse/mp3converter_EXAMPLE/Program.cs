using System;
using System.Windows.Forms;
using BasicSealClient;

namespace mp3converter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BasicSeal.Start("1.0", "55E9503AB4", true, true);
            BasicSealResult checkResult = BasicSeal.VerifyLicense().Result;

            if (checkResult.successfulVerification)
            {
                Application.Run(new mp3ConverterForm()); //run mp3 converter user interface
            }
            else
            {
                Application.Run(new EnterKeyForm()); //ask for license key
            }
        }
    }
}
