using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using GetSystemDeviceInfo;

namespace iBot.Core.Utils
{
    public class MachineInfoHelper
    {

        private const string WIN32_PROCESSOR = "Win32_Processor";//Name ProcessorId
        private const string WIN32_BASEBOARD = "Win32_BaseBoard";//SerialNumber Manufacturer Product
        private const string WIN32_BIOS = "Win32_BIOS";//SerialNumber
        private const string Win32_OPERATINGSYSTEM = "Win32_OperatingSystem";//系统信息

        private const string CPU_NAME = "Name";//名称
        private const string PROCESSORID = "ProcessorId";//序列号
        private const string SERIALNUMBER = "SerialNumber";//序列号
        private const string MANUFACTURER = "Manufacturer";//制造商
        private const string PRODUCT = "Product";//型号
        private const string INSTALLDATE = "InstallDate";//系统初装时间
        /// <summary>
        /// 获取CPU信息
        /// </summary>
        /// <param name="Name">名称（型号）</param>
        /// <param name="ProcessorId">序列号（唯一）</param>
        public static void GetCPUInfo(out string Name, out string ProcessorId)
        {
            var Infos = GetMachineInfoFromManagementClass(WIN32_PROCESSOR, new List<string> { CPU_NAME, PROCESSORID });
            if (!Infos.ContainsKey(CPU_NAME))
            {
                var error_info = $"not found CPU Name MachineInfoHelper0000000001 CPU_NAME:[{CPU_NAME}]";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            Name = Infos[CPU_NAME];
            if (!Infos.ContainsKey(PROCESSORID))
            {
                var error_info = $"not found CPU ProcessorId MachineInfoHelper0000000002 PROCESSORID:[{PROCESSORID}]";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            ProcessorId = Infos[PROCESSORID];
        }
        /// <summary>
        /// 获取主板信息
        /// </summary>
        /// <param name="Product">型号（不存在名称）</param>
        /// <param name="SerialNumber">序列号（唯一）</param>
        public static void GetBaseBoardInfo(out string Product, out string SerialNumber)
        {
            var Infos = GetMachineInfoFromManagementClass(WIN32_BASEBOARD, new List<string> { SERIALNUMBER, PRODUCT });
            if (!Infos.ContainsKey(PRODUCT))
            {
                var error_info = $"not found BaseBoard Name MachineInfoHelper0000000003 PRODUCT:[{PRODUCT}]";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            Product = Infos[PRODUCT];
            if (!Infos.ContainsKey(SERIALNUMBER))
            {
                var error_info = $"not found BaseBoard ProcessorId MachineInfoHelper0000000004 SERIALNUMBER:[{SERIALNUMBER}]";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            SerialNumber = Infos[SERIALNUMBER];
        }
        public static string GetCPUPROCESSORID(bool throwException = true)
        {
            return GetDiviceInfo(WIN32_PROCESSOR, PROCESSORID, throwException);
        }
        public static string GetBaseBoardInfo(bool throwException = true)
        {
            return GetDiviceInfo(WIN32_BASEBOARD, SERIALNUMBER, throwException);
        }
        public static string GetWindowsSystemInstallTime(bool throwException = true)
        {
            return GetDiviceInfo(Win32_OPERATINGSYSTEM, INSTALLDATE, throwException);
        }
        private static string GetDiviceInfo(string ClassName, string Key, bool throwException = true)
        {
            var logValue = string.Empty;
            try
            {
                ManagementClass mc = new ManagementClass(ClassName);
                var moc = mc.GetInstances();
                if (moc == null)
                {
                    var error_info = $"Error when get machine info from ManagementObjectCollection is ManagementObjectCollection Null  MachineInfoHelper0000000007 ";
                    Log.Error(error_info);
                    throw new Exception(error_info);
                }
                foreach (ManagementObject mo in moc)
                {
                    return mo.Properties[Key].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                var error_info = $"Error when get machine info from ManagementObjectCollection  ClassName:{ClassName}  lastlogKey={Key} lastlogValue={logValue}.MachineInfoHelper0000000008 exception:{ex}";
                Log.Error(error_info);
                if (throwException)
                {
                    throw new Exception(error_info);
                }
                return "";
            }
            return "";
        }

        public static string GetDiviceInfo(string ClassName, bool throwException = true)
        {
            var logValue = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                ManagementClass mc = new ManagementClass(ClassName);
                var moc = mc.GetInstances();
                if (moc == null)
                {
                    var error_info = $"Error when get machine info from ManagementObjectCollection is ManagementObjectCollection Null  MachineInfoHelper0000000007 ";
                    Log.Error(error_info);
                    throw new Exception(error_info);
                }
                foreach (ManagementObject mo in moc)
                {
                    foreach (var item in mo.Properties)
                    {

                        var strValue = string.Empty;
                        if (item.Value != null && item.Value is string[] xxxx)
                        {
                            strValue = "[" + string.Join(",", xxxx) + "]";
                        }
                        else
                        {
                            strValue = item.Value?.ToString();
                        }
                        //PropertyData
                        string temp =
                            item.Name + "\t\t\t" +
                            strValue + "\t\t\t" +
                            item.Type.ToString() + "\t\t\t" +
                            item.Origin + "\t\t\t" + Environment.NewLine;
                        stringBuilder.Append(temp);
                    }
                    //return mo.Properties[Key].Value.ToString();
                }
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                var error_info = $"Error when get machine info from ManagementObjectCollection  ClassName:{ClassName}  lastlogValue={logValue}.MachineInfoHelper0000000008 exception:{ex}";
                Log.Error(error_info);
                if (throwException)
                {
                    throw new Exception(error_info);
                }
                return "";
            }
            return "";
        }
        /// <summary>
        /// 从 ManagementClass 中获取硬件信息
        /// </summary>
        /// <param name="ClassName">大类名称（见文末列表）</param>
        /// <param name="KeyList">元素属性名称（具体的属性名称）</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetMachineInfoFromManagementClass(string ClassName, List<string> KeyList)
        {
            Dictionary<string, string> ResultList = new Dictionary<string, string>();
            try
            {
                ManagementClass mc = new ManagementClass(ClassName);
                GetInfoManagementObjectCollection(mc.GetInstances(), KeyList, ref ResultList);
            }
            catch (Exception ex)
            {
                var error_info = $"Error when get machine info from ManagementClass  ClassName {ClassName} TargetNames {string.Join(",", KeyList)} .MachineInfoHelper0000000005 exception:{ex}";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            return ResultList;
        }

        /// <summary>
        /// 从 ManagementObjectSearcher 中获取硬件信息
        /// </summary>
        /// <param name="ClassName">大类名称（见文末列表）</param>
        /// <param name="KeyList">元素属性名称（具体的属性名称）</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetMachineInfoFromManagementSearcher(string ClassName, List<string> KeyList)
        {
            Dictionary<string, string> ResultList = new Dictionary<string, string>();
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher($"select * from {ClassName}");
                GetInfoManagementObjectCollection(mos.Get(), KeyList, ref ResultList);
            }
            catch (Exception ex)
            {
                var error_info = $"Error when get machine info from ManagementSearcher ClassName {ClassName} TargetNames {string.Join(",", KeyList)} .MachineInfoHelper0000000006 exception:{ex}";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
            return ResultList;
        }
        /// <summary>
        /// 从 ManagementObjectCollection 中获取硬件信息（中间方法）
        /// </summary>
        /// <param name="ClassName">大类名称（见文末列表）</param>
        /// <param name="KeyList">元素属性名称（具体的属性名称）</param>
        /// <returns></returns>
        private static void GetInfoManagementObjectCollection(ManagementObjectCollection moc, List<string> KeyList, ref Dictionary<string, string> ResultList)
        {
            var logKey = string.Empty;
            var logValue = string.Empty;
            try
            {
                if (moc == null)
                {
                    var error_info = $"Error when get machine info from ManagementObjectCollection is ManagementObjectCollection Null  MachineInfoHelper0000000007 ";
                    Log.Error(error_info);
                    throw new Exception(error_info);
                }
                foreach (ManagementObject mo in moc)
                {
                    foreach (var item in KeyList)
                    {
                        logKey = item;
                        logValue = mo.Properties[item].Value.ToString();
                        ResultList.Add(logKey, logValue);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                var error_info = $"Error when get machine info from ManagementObjectCollection  TargetNames:{string.Join(",", KeyList)}  lastlogKey={logKey} lastlogValue={logValue}.MachineInfoHelper0000000008 exception:{ex}";
                Log.Error(error_info);
                throw new Exception(error_info);
            }
        }


        //参考 https://blog.csdn.net/da_keng/article/details/50589145

        //为了获取硬件信息，你还需要创建一个ManagementObjectSearcher 对象。
        //ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);


        public enum DeviceInforType
        {
            Win32_USBControllerDevice,
            Win32_PnPEntity,
            Win32_Processor, // CPU 处理器 
            Win32_PhysicalMemory, // 物理内存条 
            Win32_Keyboard, // 键盘 
            Win32_PointingDevice, // 点输入设备，包括鼠标。 
            Win32_FloppyDrive, // 软盘驱动器 
            Win32_DiskDrive, // 硬盘驱动器 
            Win32_CDROMDrive, // 光盘驱动器 
            Win32_BaseBoard, // 主板 
            Win32_BIOS, // BIOS 芯片 
            Win32_ParallelPort, // 并口 
            Win32_SerialPort, // 串口 
            Win32_SerialPortConfiguration, // 串口配置 
            Win32_SoundDevice, // 多媒体设置，一般指声卡。 
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP) 
            Win32_USBController, // USB 控制器 
            Win32_NetworkAdapter, // 网络适配器 
            Win32_NetworkAdapterConfiguration, // 网络适配器设置 
            Win32_Printer, // 打印机 
            Win32_PrinterConfiguration, // 打印机设置 
            Win32_PrintJob, // 打印机任务 
            Win32_TCPIPPrinterPort, // 打印机端口 
            Win32_POTSModem, // MODEM 
            Win32_POTSModemToSerialPort, // MODEM 端口 
            Win32_DesktopMonitor, // 显示器 
            Win32_DisplayConfiguration, // 显卡 
            Win32_DisplayControllerConfiguration, // 显卡设置 
            Win32_VideoController, // 显卡细节。 
            Win32_VideoSettings, // 显卡支持的显示模式。 

            Win32_TimeZone, // 时区 
            Win32_SystemDriver, // 驱动程序 
            Win32_DiskPartition, // 磁盘分区 
            Win32_LogicalDisk, // 逻辑磁盘 
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。 
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置 
            Win32_PageFile, // 系统页文件信息 
            Win32_PageFileSetting, // 页文件设置 
            Win32_BootConfiguration, // 系统启动配置 
            Win32_ComputerSystem, // 计算机信息简要 
            Win32_OperatingSystem, // 操作系统信息 
            Win32_StartupCommand, // 系统自动启动程序 
            Win32_Service, // 系统安装的服务 
            Win32_Group, // 系统管理组 
            Win32_GroupUser, // 系统组帐号 
            Win32_UserAccount, // 用户帐号 
            Win32_Process, // 系统进程 
            Win32_Thread, // 系统线程 
            Win32_Share, // 共享 
            Win32_NetworkClient, // 已安装的网络客户端 
            Win32_NetworkProtocol, // 已安装的网络协议 
        };



        // 硬件 
        //Win32_Processor, // CPU 处理器 
        //Win32_PhysicalMemory, // 物理内存条 
        //Win32_Keyboard, // 键盘 
        //Win32_PointingDevice, // 点输入设备，包括鼠标。 
        //Win32_FloppyDrive, // 软盘驱动器 
        //Win32_DiskDrive, // 硬盘驱动器 
        //Win32_CDROMDrive, // 光盘驱动器 
        //Win32_BaseBoard, // 主板 
        //Win32_BIOS, // BIOS 芯片 
        //Win32_ParallelPort, // 并口 
        //Win32_SerialPort, // 串口 
        //Win32_SerialPortConfiguration, // 串口配置 
        //Win32_SoundDevice, // 多媒体设置，一般指声卡。 
        //Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP) 
        //Win32_USBController, // USB 控制器 
        //Win32_NetworkAdapter, // 网络适配器 
        //Win32_NetworkAdapterConfiguration, // 网络适配器设置 
        //Win32_Printer, // 打印机 
        //Win32_PrinterConfiguration, // 打印机设置 
        //Win32_PrintJob, // 打印机任务 
        //Win32_TCPIPPrinterPort, // 打印机端口 
        //Win32_POTSModem, // MODEM 
        //Win32_POTSModemToSerialPort, // MODEM 端口 
        //Win32_DesktopMonitor, // 显示器 
        //Win32_DisplayConfiguration, // 显卡 
        //Win32_DisplayControllerConfiguration, // 显卡设置 
        //Win32_VideoController, // 显卡细节。 
        //Win32_VideoSettings, // 显卡支持的显示模式。 

        //// 操作系统 
        //Win32_TimeZone, // 时区 
        //Win32_SystemDriver, // 驱动程序 
        //Win32_DiskPartition, // 磁盘分区 
        //Win32_LogicalDisk, // 逻辑磁盘 
        //Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。 
        //Win32_LogicalMemoryConfiguration, // 逻辑内存配置 
        //Win32_PageFile, // 系统页文件信息 
        //Win32_PageFileSetting, // 页文件设置 
        //Win32_BootConfiguration, // 系统启动配置 
        //Win32_ComputerSystem, // 计算机信息简要 
        //Win32_OperatingSystem, // 操作系统信息 
        //Win32_StartupCommand, // 系统自动启动程序 
        //Win32_Service, // 系统安装的服务 
        //Win32_Group, // 系统管理组 
        //Win32_GroupUser, // 系统组帐号 
        //Win32_UserAccount, // 用户帐号 
        //Win32_Process, // 系统进程 
        //Win32_Thread, // 系统线程 
        //Win32_Share, // 共享 
        //Win32_NetworkClient, // 已安装的网络客户端 
        //Win32_NetworkProtocol, // 已安装的网络协议 

    }
}
