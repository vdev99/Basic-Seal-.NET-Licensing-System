using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using BasicSealClient.Helpers;

namespace BasicSealClient
{
    internal interface ISystemInformation
    {
        public string getCPUId();
        public string getSystemUUID();
        public string getHDDSerial();


        internal class Windows : ISystemInformation
        {
            string getProcID = "-NoProfile -ExecutionPolicy UnRestricted Get-WmiObject -Class win32_processor | Select -ExpandProperty ProcessorID";

            public string getCPUId()
            {
                try
                {
                    string processorId = PowerShell.RunCommand(getProcID);

                    int indexofCh = processorId.IndexOf("\r");
                    processorId = processorId.Substring(0, indexofCh);

                    return processorId;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string getSystemUUID()
            {
                string getSystemUUID = "-NoProfile -ExecutionPolicy UnRestricted Get-WmiObject -Class Win32_ComputerSystemProduct | Select -ExpandProperty UUID";

                try
                {
                    string systemId = PowerShell.RunCommand(getSystemUUID);

                    int indexofCh = systemId.IndexOf("\r");
                    systemId = systemId.Substring(0, indexofCh);

                    return systemId;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string getHDDSerial()
            {
                string getHddId = "-NoProfile -ExecutionPolicy UnRestricted Get-WmiObject -Class win32_physicalmedia | Select -ExpandProperty SerialNumber";

                try
                {
                    string hddSerial = PowerShell.RunCommand(getHddId);

                    int indexofCh = hddSerial.IndexOf("\r");
                    hddSerial = hddSerial.Substring(0, indexofCh);

                    return hddSerial;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        internal class Linux : ISystemInformation
        {
            public string getCPUId()
            {
                string linuxProcessorId = "dmidecode -t processor | grep -E ID | sed 's/.*: //' | head -n 1";

                string cpuid = Bash.RunCommand(linuxProcessorId)?.Replace(" ", string.Empty);
                return cpuid;
            }

            public string getSystemUUID()
            {
                string linuxMachineId = "cat /var/lib/dbus/machine-id";

                string systemUUID = Bash.RunCommand(linuxMachineId)?.Replace(" ", string.Empty);
                return systemUUID;
            }

            public string getHDDSerial()
            {
                string hddSerial = "udevadm info --query=all --name=/dev/sda | grep ID_SERIAL_SHORT | tr -d \"ID_SERIAL_SHORT=:\"";

                string hddSer = Bash.RunCommand(hddSerial)?.Replace(" ", string.Empty);
                return hddSer;
            }
        }
    }
}
