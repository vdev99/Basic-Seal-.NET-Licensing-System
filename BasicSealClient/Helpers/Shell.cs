using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BasicSealClient.Helpers
{
    internal class PowerShell
    {
        public static string RunCommand(string command)
        {
            string result = String.Empty;

            try
            {
                var process = new Process
                {
                    StartInfo =
                        {
                            FileName = Path.GetPathRoot(Environment.SystemDirectory) + @"Windows\system32\WindowsPowerShell\v1.0\powershell.exe",
                            Arguments = command,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                };

                process.Start();
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit(3000);
                process.Kill();
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }
    }

    internal class Bash
    {
        public static string RunCommand(string command)
        {
            string result = String.Empty;

            try
            {
                var escapedArgs = command.Replace("\"", "\\\"");

                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    process.Start();
                    result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(3000);
                    process.Kill();
                };
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }
    }
}
