using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTCV.CorruptCore;

namespace NetStub
{
    class PS3_LV2MemoryDomain : IMemoryDomain
    {
        public string Name { get; }
        public bool BigEndian => true;
        public long Size { get; }
        private IntPtr baseAddr { get; }
        public int WordSize => 4;
        private int errCount = 0;
        private int maxErrs = 5;
        public override string ToString()
        {
            return Name;
        }

        public PS3_LV2MemoryDomain()
        {
            try
            {
                //{
                //    if (_p == null || _p.HasExited)
                //    {
                //        throw new Exception("Process doesn't exist or has exited");
                //    }
                //String connectormessage = "";
                //MapNum = map;
                //Connector.SendMessage("GetProcMapSize|" + map);
                //connectormessage = Connector.RecMessage();
                //Size = int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapSize: ")));
                //Connector.SendMessage("GetProcMapStartAddr|" + map);
                //connectormessage = Connector.RecMessage();
                //baseAddr = (IntPtr)int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapStartAddr: ")));
                //Connector.SendMessage("GetProcMapName|" + map);
                //connectormessage = Connector.RecMessage();
                //var path = connectormessage.Substring(connectormessage.IndexOf("GetProcMapName: "));
                //if (!string.IsNullOrWhiteSpace(path))
                //    path = path;
                //else
                //    path = "UNKNOWN";
                //Name = $"{baseAddr.ToString("X8")} : {Size.ToString("X8")} {path}";
                Size = 8 * 1024 * 1024;
                Name = $"LV2 Memory";
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to create ProcessInterface!\nMessage: {e.Message}");
            }
        }

        public void PokeByte(long address, byte val)
        {
            //if (p == null || errCount > maxErrs)
            //    return;
            try
            {
                Connector.SendMessage("lv2_write8|" + address + "|" + val + "|" + 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PokeByte failed!\n{e.Message}\n{e.StackTrace}");
                errCount++;
            }
        }

        public byte PeekByte(long address)
        {
            String connectormessage = "";
            //if (p == null || errCount > maxErrs)
            //    return 0;
            try
            {
                Connector.SendMessage("lv2_read8|" + address + "|" + 0);
                connectormessage = Connector.RecMessage();
                return Convert.ToByte(int.Parse(connectormessage.Substring(connectormessage.IndexOf("lv2_read8: "))));
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PeekByte failed!\n{e.Message}\n{e.StackTrace}");
                errCount++;
            }
            return 0;
        }
        public byte[] PeekBytes(long address, int length)
        {
            String connectormessage = "";
            byte[] bytes = new byte[length];
            //if (p == null || errCount > maxErrs)
            //    return null;
            try
            {
                var returnArray = new byte[length];
                for (var i = 0; i < length; i++)
                    returnArray[i] = PeekByte(address + i);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PeekBytes failed!\n{e.Message}");
                errCount++;
            }
            return null;
        }
        public byte[] GetDump()
        {
            throw new NotImplementedException();
        }

        //public bool SetMemoryProtection(ProcessExtensions.MemoryProtection memoryProtection)
        //{
        //    var result = ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, memoryProtection, out var _mp);
        //    if (result)
        //        mp = _mp;
        //    return result;
        //}
        //public bool ResetMemoryProtection()
        //{
        //    if (mp != ProcessExtensions.MemoryProtection.Empty)
        //        return ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, mp, out _);
        //    return false;
        //}

        //public void FlushInstructionCache()
        //{
        //    ProcessExtensions.FlushInstructionCache(p.Handle, baseAddr, (UIntPtr)Size);
        //}
    }
    class PS3_ProcessMemoryDomain : IMemoryDomain
    {
        public string Name { get; }
        public bool BigEndian => true;
        public long Size { get; }
        private IntPtr baseAddr { get; }
        public int WordSize => 4;
        private int errCount = 0;
        private int maxErrs = 5;
        public override string ToString()
        {
            return Name;
        }

        public PS3_ProcessMemoryDomain()
        {
            try
            {
                //{
                //    if (_p == null || _p.HasExited)
                //    {
                //        throw new Exception("Process doesn't exist or has exited");
                //    }
                String connectormessage = "";
                //MapNum = map;
                //Connector.SendMessage("GetProcMapSize|" + map);
                //connectormessage = Connector.RecMessage();
                //Size = int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapSize: ")));
                //Connector.SendMessage("GetProcMapStartAddr|" + map);
                //connectormessage = Connector.RecMessage();
                //baseAddr = (IntPtr)int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapStartAddr: ")));
                Connector.SendMessage("GetProcName");
                connectormessage = Connector.RecMessage();
                var path = connectormessage.Substring(connectormessage.IndexOf("GetProcName: "));
                if (!string.IsNullOrWhiteSpace(path))
                    path = path;
                else
                    path = "UNKNOWN";
                //Name = $"{baseAddr.ToString("X8")} : {Size.ToString("X8")} {path}";
                Size = 256 * 1024 * 1024;
                Name = $"Current Process : {path}";
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to create ProcessInterface!\nMessage: {e.Message}");
            }
        }

        public void PokeByte(long address, byte val)
        {
            //if (p == null || errCount > maxErrs)
            //    return;
            try
            {
                Connector.SendMessage("proc_write8|" + address + "|" + val);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PokeByte failed!\n{e.Message}\n{e.StackTrace}");
                errCount++;
            }
        }

        public byte PeekByte(long address)
        {
            String connectormessage = "";
            //if (p == null || errCount > maxErrs)
            //    return 0;
            try
            {
                Connector.SendMessage("proc_read8|" + address);
                connectormessage = Connector.RecMessage();
                return Convert.ToByte(int.Parse(connectormessage.Substring(connectormessage.IndexOf("proc_read8: "))));
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PeekByte failed!\n{e.Message}\n{e.StackTrace}");
                errCount++;
            }
            return 0;
        }
        public byte[] PeekBytes(long address, int length)
        {
            String connectormessage = "";
            byte[] bytes = new byte[length];
            //if (p == null || errCount > maxErrs)
            //    return null;
            try
            {
                var returnArray = new byte[length];
                for (var i = 0; i < length; i++)
                    returnArray[i] = PeekByte(address + i);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessInterface PeekBytes failed!\n{e.Message}");
                errCount++;
            }
            return null;
        }
        public byte[] GetDump()
        {
            throw new NotImplementedException();
        }

        //public bool SetMemoryProtection(ProcessExtensions.MemoryProtection memoryProtection)
        //{
        //    var result = ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, memoryProtection, out var _mp);
        //    if (result)
        //        mp = _mp;
        //    return result;
        //}
        //public bool ResetMemoryProtection()
        //{
        //    if (mp != ProcessExtensions.MemoryProtection.Empty)
        //        return ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, mp, out _);
        //    return false;
        //}

        //public void FlushInstructionCache()
        //{
        //    ProcessExtensions.FlushInstructionCache(p.Handle, baseAddr, (UIntPtr)Size);
        //}
    }
}
    //class PS3MemoryDomain : IMemoryDomain
    //{
    //    public string Name { get; }
    //    public bool BigEndian => false;
    //    public long Size { get; }
    //    private IntPtr baseAddr { get; }
    //    public int WordSize => 4;
    //    public int MapNum { get; }
    //    private int errCount = 0;
    //    private int maxErrs = 5;

    //    public override string ToString()
    //    {
    //        return Name;
    //    }

    //    public PS3MemoryDomain(int map)
    //    {
    //        try
    //        {
    //            //{
    //            //    if (_p == null || _p.HasExited)
    //            //    {
    //            //        throw new Exception("Process doesn't exist or has exited");
    //            //    }
    //            String connectormessage = "";
    //            MapNum = map;
    //            Connector.SendMessage("GetProcMapSize|" + map);
    //            connectormessage = Connector.RecMessage();
    //            Size = int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapSize: ")));
    //            Connector.SendMessage("GetProcMapStartAddr|" + map);
    //            connectormessage = Connector.RecMessage();
    //            baseAddr = (IntPtr)int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetProcMapStartAddr: ")));
    //            Connector.SendMessage("GetProcMapName|" + map);
    //            connectormessage = Connector.RecMessage();
    //            var path = connectormessage.Substring(connectormessage.IndexOf("GetProcMapName: "));
    //            if (!string.IsNullOrWhiteSpace(path))
    //                path = path;
    //            else
    //                path = "UNKNOWN";
    //            Name = $"{baseAddr.ToString("X8")} : {Size.ToString("X8")} {path}";
    //        }
    //        catch (Exception e)
    //        {
    //            MessageBox.Show($"Failed to create ProcessInterface!\nMessage: {e.Message}");
    //        }
    //    }

    //    public void PokeByte(long address, byte val)
    //    {
    //        //if (p == null || errCount > maxErrs)
    //        //    return;
    //        try
    //        {
    //            Connector.SendMessage("write8|" + address + "|" + Convert.ToInt32(val) + "|" + MapNum);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine($"ProcessInterface PokeByte failed!\n{e.Message}\n{e.StackTrace}");
    //            errCount++;
    //        }
    //    }

    //    public byte PeekByte(long address)
    //    {
    //        String connectormessage = "";
    //        //if (p == null || errCount > maxErrs)
    //        //    return 0;
    //        try
    //        {
    //            Connector.SendMessage("read8|" + address + "|" + 0 + "|" + MapNum);
    //            connectormessage = Connector.RecMessage(); 
    //            return Convert.ToByte(int.Parse(connectormessage.Substring(connectormessage.IndexOf("read8: "))));
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine($"ProcessInterface PeekByte failed!\n{e.Message}\n{e.StackTrace}");
    //            errCount++;
    //        }
    //        return 0;
    //    }
    //    public byte[] PeekBytes(long address, int length)
    //    {
    //        String connectormessage = "";
    //        byte[] bytes = new byte[length];
    //        //if (p == null || errCount > maxErrs)
    //        //    return null;
    //        try
    //        {
    //            var returnArray = new byte[length];
    //            for (var i = 0; i < length; i++)
    //                returnArray[i] = PeekByte(address + i);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine($"ProcessInterface PeekBytes failed!\n{e.Message}");
    //            errCount++;
    //        }
    //        return null;
    //    }
    //    public byte[] GetDump()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    //public bool SetMemoryProtection(ProcessExtensions.MemoryProtection memoryProtection)
    //    //{
    //    //    var result = ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, memoryProtection, out var _mp);
    //    //    if (result)
    //    //        mp = _mp;
    //    //    return result;
    //    //}
    //    //public bool ResetMemoryProtection()
    //    //{
    //    //    if (mp != ProcessExtensions.MemoryProtection.Empty)
    //    //        return ProcessExtensions.VirtualProtectEx(p, baseAddr, (IntPtr)Size, mp, out _);
    //    //    return false;
    //    //}

    //    //public void FlushInstructionCache()
    //    //{
    //    //    ProcessExtensions.FlushInstructionCache(p.Handle, baseAddr, (UIntPtr)Size);
    //    //}
    //}
