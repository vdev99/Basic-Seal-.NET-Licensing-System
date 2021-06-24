using BasicSealClient.Dtos;
using BasicSealClient.Helpers;
using BasicSealClient.Key;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BasicSealClient
{
    internal class Security
    {
        internal static bool licenseVerified = false;

        private string[] processList =
        {
            "ollydbg",
            "ida",
            "ida64",
            "idag",
            "idag64",
            "idaw",
            "idaw64",
            "idaq",
            "idaq64",
            "idau",
            "idau64",
            "scylla",
            "scylla_x64",
            "scylla_x86",
            "protection_id",
            "x64dbg",
            "x32dbg",
            "windbg",
            "reshacker",
            "importrec",
            "immunitydebugger",
            "megadumper",
            "megadumper 1.0 by codecracker / snd",
            "ollydbg",
            "ida",
            "disassembly",
            "immunity",
            "import constructor",
            "solarwinds",
            "paessler",
            "cpacket",
            "wireshark",
            "ethereal",
            "sectools",
            "riverbed",
            "tcpdump",
            "kismet",
            "etherape",
            "fiddler",
            "telerik",
            "glasswire",
            "httpdebuggersvc",
            "httpdebuggerui",
            "charles",
            "intercepter",
            "snpa",
            "dumcap",
            "comview",
            "netcheat",
            "cheat",
            "winpcap",
            "dnspy",
            "ilspy",
            "reflector"
        };

        public List<int> ScanForHarmingProcesses()
        {
            List<int> harmingProcessIds = new List<int>();

            foreach (Process proc in Process.GetProcesses())
            {
                foreach (string hProcName in processList)
                {
                    if (proc.ProcessName.ToLower().Contains(hProcName) || proc.MainWindowTitle.ToLower().Contains(hProcName))
                    {
                        harmingProcessIds.Add(proc.Id);
                    }
                }
            }

            return harmingProcessIds;
        }

        public void KillHarmingProcessOrExit(List<int> processIds)
        {
            foreach(int process in processIds)
            {
                try
                {
                    Process.GetProcessById(process).Kill(true);
                }
                catch(Exception)
                {
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }

        public void CheckAppIntegrity(string originalFileHash)
        {
            string assemblyLocation = Path.GetFullPath(Assembly.GetEntryAssembly().Location);

            string currentAssemblyHash = new Encryption().HashFile(assemblyLocation);

            if(!string.Equals(originalFileHash, currentAssemblyHash))
            {
                Environment.Exit(Environment.ExitCode); //the application has been modified, exiting
            }
        }

        public bool VerifyEncryptionKey(string key, KeyByteSet[] keyBytes, string licenseHash)
        {
            string assemblyLocation = Path.GetFullPath(Assembly.GetEntryAssembly().Location);

            string currentAssemblyHash = new Encryption().HashFile(assemblyLocation);

            int seed = new Encryption().generatePkvSeed(currentAssemblyHash, licenseHash);

            KeyVerifier keyver = new KeyVerifier();
            var result = keyver.VerifyKey(key, keyBytes, 8, seed.ToString("x8").ToUpper());

            if (result == PkvKeyVerificationResult.KeyIsValid) { return true; }

            return false;
        }

        public bool createDateFile(DateTime? servertime)
        {
            string computerName = Environment.MachineName;
            string currentAssemblyName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            string dataFileName = "bscdata.dat";

            Encryption enc = new Encryption();
            string encKey = enc.HashString(computerName + currentAssemblyName);

            string directory = BasicSeal.isWindows ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.CurrentDirectory;
            string fullDatePath = BasicSeal.isWindows ? directory + @"\" + dataFileName : directory + @"/" + dataFileName;

            Encryption.AES aes = new Encryption.AES();

            byte[] salt = enc.GetSalt(8);
            byte[] encrypted = aes.Encrypt(encKey, Encoding.ASCII.GetBytes(servertime.ToString()), salt);

            try
            {
                using (FileStream fs = new FileStream(fullDatePath, FileMode.Create))
                {
                    fs.Write(salt, 0, salt.Length);
                    fs.Write(encrypted, 0, encrypted.Length);
                    fs.Close();
                }
                var olderDate = DateTime.UtcNow.AddDays(-28).AddYears(-1);

                File.SetCreationTime(fullDatePath, olderDate);
                File.SetLastWriteTime(fullDatePath, olderDate);
                File.SetLastAccessTime(fullDatePath, olderDate);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public DateTime? getDateFromFile()
        {
            string computerName = Environment.MachineName;
            string currentAssemblyName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            string dataFileName = "bscdata.dat";

            Encryption enc = new Encryption();
            string encKey = enc.HashString(computerName + currentAssemblyName);

            string directory = BasicSeal.isWindows ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.CurrentDirectory;
            string fullDatePath = BasicSeal.isWindows ? directory + @"\" + dataFileName : directory + @"/" + dataFileName;

            if (File.Exists(fullDatePath))
            {
                try
                {
                    byte[] fileData = File.ReadAllBytes(fullDatePath);

                    Encryption.AES aes = new Encryption.AES();

                    byte[] decrypted = aes.Decrypt(encKey, fileData);

                    DateTime lastDate = DateTime.Parse(Encoding.ASCII.GetString(decrypted));

                    var olderDate = DateTime.UtcNow.AddDays(-28).AddYears(-1);
                    File.SetLastAccessTime(fullDatePath, olderDate);

                    return lastDate;
                }
                catch (Exception)
                {
                    return null;
                }
                
            }
            else { return null; }
        }

        public virtual void IfDebuggerAttachedExit() { }
        public virtual void RunSecurityThread() { }

        internal class Windows : Security
        {
            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            private static extern bool IsDebuggerPresent();

            public override void IfDebuggerAttachedExit()
            {
                if(IsManagedDebuggerAttached() || IsUnmanagedDebuggerAttached() || isRemoteDebuggerAttached())
                {
                    Environment.Exit(Environment.ExitCode); //the application is monitored by a debugger, exiting
                }
            }

            public override void RunSecurityThread()
            {
                Thread RunSecurityThread = new Thread(() =>
                {
                    while (!licenseVerified)
                    {
                        base.KillHarmingProcessOrExit(base.ScanForHarmingProcesses());
                        IfDebuggerAttachedExit();
                        Thread.Sleep(500);
                    }
                });

                RunSecurityThread.IsBackground = true;
                RunSecurityThread.Start();
            }

            private bool IsManagedDebuggerAttached()
            {
                if (Debugger.IsAttached)
                {
                    return true;
                }

                return false;
            }

            private bool IsUnmanagedDebuggerAttached()
            {
                if (IsDebuggerPresent())
                {
                    return true;
                }

                return false;
            }

            private bool isRemoteDebuggerAttached()
            {
                bool isDebuggerPresent = false;

                CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

                return isDebuggerPresent;
            }

        }

        internal class Linux : Security
        {
            public override void RunSecurityThread()
            {
                Thread RunSecurityThread = new Thread(() =>
                {
                    while (!licenseVerified)
                    {
                        base.KillHarmingProcessOrExit(base.ScanForHarmingProcesses());
                    }
                });

                RunSecurityThread.IsBackground = true;
                RunSecurityThread.Start();
            }
        }
    }
}
