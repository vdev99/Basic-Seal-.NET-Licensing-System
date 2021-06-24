using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BasicSealClient.Helpers
{
    internal interface IMessageBox
    {
        void Show(string message);
        bool isEnabled { get; set; }
    }

    internal class MessageboxWindows : IMessageBox
    {
        public bool isEnabled { get; set; }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public void Show(string message)
        {
            if (isEnabled)
            {
                MessageBox(IntPtr.Zero, message, "BasicSealClient", 0x00000040);
            }
        }
    }

    internal class MessageboxLinux : IMessageBox
    {
        public bool isEnabled { get; set; }

        public void Show(string message)
        {
            if (isEnabled)
            {
                string messageCommand = $"zenity --error --text = \"{message}\" --title = \"BasicSealClient\"";
                Bash.RunCommand(messageCommand);
            }
        }
    }
}
