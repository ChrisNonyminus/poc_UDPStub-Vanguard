namespace NetStub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using RTCV.Common;
    using RTCV.CorruptCore;
    using RTCV.NetCore;
    //using RTCV.ProcessCorrupt;
    using Vanguard;

    public static class Hook
    {
        public static string NetStubVersion = "0.0.2";
        public static string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static string ProtocolType = "UDP";
        public static string NetStubMode = "Linux";
        public static bool PS3_AccessProcess = false;
        public static Process p;
        public static bool UseFiltering = true;
        public static bool UseExceptionHandler = false;
        public static bool UseBlacklist = true;
        public static bool SuspendProcess = false;
        public static bool DontChangeMemoryProtection = false;
        public static object CorruptLock = new object();

        public static ProgressForm progressForm;
        public static volatile System.Timers.Timer AutoHookTimer;
        public static volatile System.Timers.Timer AutoCorruptTimer;
        public static ImageList ProcessIcons = new ImageList();

        //public static ProcessExtensions.MemoryProtection ProtectMode = ProcessExtensions.MemoryProtection.ReadWrite;

        public static void Start()
        {
            RTCV.Common.Logging.StartLogging(VanguardCore.logPath);
            //AutoHookTimer = new System.Timers.Timer();
            //AutoHookTimer.Interval = 5000;
            //AutoHookTimer.Elapsed += AutoHookTimer_Elapsed;

            //AutoCorruptTimer = new System.Timers.Timer();
            //AutoCorruptTimer.Interval = 16;
            //AutoCorruptTimer.AutoReset = false;
            //AutoCorruptTimer.Elapsed += CorruptTimer_Elapsed;
            //AutoCorruptTimer.Start();

            if (VanguardCore.vanguardConnected)
                UpdateDomains();

            //ProcessWatch.currentFileInfo = new NetStubFileInfo();

            DisableInterface();
            RtcCore.EmuDirOverride = true; //allows the use of this value before vanguard is connected

            string paramsPath = Path.Combine(Hook.currentDir, "PARAMS");

            if (!Directory.Exists(paramsPath))
                Directory.CreateDirectory(paramsPath);


            var protectionMode = Params.ReadParam("PROTECTIONMODE");
            try
            {
                if (protectionMode != null)
                    //ProtectMode = (ProcessExtensions.MemoryProtection)Enum.Parse(typeof(ProcessExtensions.MemoryProtection), protectionMode);
                    ;
            }
            catch (Exception)
            {
                Params.RemoveParam("PROTECTIONMODE");
                //ProtectMode = ProcessExtensions.MemoryProtection.ReadWrite;
            }

            UseExceptionHandler = Params.ReadParam("USEEXCEPTIONHANDLER") == "True";
            UseBlacklist = Params.ReadParam("USEBLACKLIST") != "False";
            SuspendProcess = Params.ReadParam("SUSPENDPROCESS") == "True";
            UseFiltering = Params.ReadParam("USEFILTERING") != "False";
        }

        //private static void CorruptTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    lock (CorruptLock)
        //    {
        //        if (!VanguardCore.vanguardConnected || AllSpec.CorruptCoreSpec == null || (p?.HasExited ?? true))
        //        {
        //            AutoCorruptTimer.Start();
        //            return;
        //        }

        //        try
        //        {
        //            if (!DontChangeMemoryProtection)
        //            {
        //                foreach (var m in MemoryDomains.MemoryInterfaces?.Values ?? Enumerable.Empty<MemoryDomainProxy>())
        //                {
        //                    if (m.MD is ProcessMemoryDomain pmd)
        //                    {
        //                        pmd.SetMemoryProtection(ProcessExtensions.MemoryProtection.ExecuteReadWrite);
        //                        if (p?.HasExited ?? false)
        //                        {
        //                            Console.WriteLine($"Bad! {pmd.Name}");
        //                        }
        //                    }
        //                }
        //            }

        //            try
        //            {
        //                RtcClock.StepCorrupt(true, true);

        //                if (p?.HasExited ?? false)
        //                {
        //                    Console.WriteLine($"Bad2!");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"STEP_CORRUPT Error!\n{ex.Message}\n{ex.StackTrace}");
        //            }
        //        }
        //        finally
        //        {
        //            if (!DontChangeMemoryProtection)
        //            {
        //                foreach (var m in MemoryDomains.MemoryInterfaces?.Values ?? Enumerable.Empty<MemoryDomainProxy>())
        //                {
        //                    if (m.MD is ProcessMemoryDomain pmd)
        //                    {
        //                        pmd.ResetMemoryProtection();
        //                        pmd.FlushInstructionCache();
        //                    }

        //                    if (p?.HasExited ?? false)
        //                    {
        //                        Console.WriteLine($"Bad3!");
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (p.HasExited)
        //    {
        //        Console.WriteLine($"Bad4!");
        //    }
        //    AutoCorruptTimer.Start();
        //}

        //private static void AutoHookTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        if (p?.HasExited == false)
        //            return;
        //        SyncObjectSingleton.FormExecute(() => S.GET<StubForm>().lbTargetStatus.Text = "Waiting...");
        //        var procToFind = S.GET<StubForm>().tbClientAddr.Text;
        //        if (string.IsNullOrWhiteSpace(procToFind))
        //            return;

        //        SyncObjectSingleton.FormExecute(() => S.GET<StubForm>().lbTargetStatus.Text = "Hooking...");
        //        var _p = Process.GetProcesses().First(x => x.ProcessName == procToFind);
        //        if (_p != null)
        //        {
        //            Thread.Sleep(2000); //Give the process 2 seconds
        //            SyncObjectSingleton.FormExecute(() =>
        //            {
        //                LoadTarget(_p);

        //                if (!VanguardCore.vanguardConnected)
        //                    VanguardCore.Start();

        //                S.GET<StubForm>().EnableTargetInterface();
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"AutoHook failed.\n{ex.Message}\n{ex.StackTrace}");
        //    }
        //    AutoHookTimer.Start();
        //}

        //internal static bool LoadTarget(Process _p = null)
        //{
        //    lock (CorruptLock)
        //    {
        //        if (_p == null)
        //        {
        //            using (var f = new HookProcessForm())
        //            {
        //                if (f.ShowDialog() != DialogResult.OK)
        //                    return false;

        //                if (f.RequestedProcess == null || (f.RequestedProcess?.HasExited ?? true))
        //                {
        //                    return false;
        //                }

        //                if (IsProcessBlacklisted(f.RequestedProcess))
        //                {
        //                    MessageBox.Show("Blacklisted process");
        //                    return false;
        //                }

        //                p = f.RequestedProcess;
        //            }
        //        }
        //        else
        //            p = _p;
        //        /*
        //        if (UseExceptionHandler)
        //        {
        //            ProcessExtensions.IsWow64Process(p.Handle, out bool is32BitProcess); //This method is stupid and returns the inverse
        //            string path = is32BitProcess
        //                ? Path.Combine(currentDir, "ExceptionHandler_x86.dll")
        //                : Path.Combine(currentDir, "ExceptionHandler_x64.dll");
        //            if (File.Exists(path))
        //            {
        //                try
        //                {
        //                    using (var i = new Injector(InjectionMethod.CreateThread, p.Id, path))
        //                    {
        //                        if ((ulong) i.InjectDll() != 0)
        //                        {
        //                            Console.WriteLine("Injected exception helper successfully");
        //                        }
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine($"Injection failed! {e}");
        //                }
        //            }
        //        }*/

        //        Action<object, EventArgs> action = (ob, ea) =>
        //        {
        //            if (VanguardCore.vanguardConnected)
        //                UpdateDomains();
        //        };

        //        Action<object, EventArgs> postAction = (ob, ea) =>
        //        {
        //            if (p == null)
        //            {
        //                MessageBox.Show("Failed to load target");
        //                S.GET<StubForm>().DisableTargetInterface();
        //                return;
        //            }

        //            S.GET<StubForm>().lbTarget.Text = p.ProcessName;
        //            S.GET<StubForm>().lbTargetStatus.Text = "Hooked!";

        //            //Refresh the UI
        //            //RefreshUIPostLoad();
        //        };
        //        S.GET<StubForm>().RunProgressBar($"Loading target...", 0, action, postAction);
        //    }

        //    return true;
        //}

        //internal static bool CloseTarget(bool updateDomains = true)
        //{
        //    p = null;
        //    if (updateDomains)
        //        UpdateDomains();
        //    return true;
        //}

        public static void UpdateDomains()
        {
            if (!VanguardCore.vanguardConnected)
                return;
            try
            {
                PartialSpec gameDone = new PartialSpec("VanguardSpec");
                if (NetStubMode == "Linux")
                {
                    gameDone[VSPEC.SYSTEM] = "Linux";
                    gameDone[VSPEC.GAMENAME] = "IGNORE";
                    gameDone[VSPEC.SYSTEMPREFIX] = "Linux";
                    gameDone[VSPEC.SYSTEMCORE] = "Linux";
                    gameDone[VSPEC.OPENROMFILENAME] = "IGNORE";
                    gameDone[VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS] = Array.Empty<string>();
                    gameDone[VSPEC.MEMORYDOMAINS_INTERFACES] = GetInterfaces();
                    gameDone[VSPEC.CORE_DISKBASED] = false;
                }
                if (NetStubMode == "PS3")
                {
                    gameDone[VSPEC.SYSTEM] = "PS3";
                    gameDone[VSPEC.GAMENAME] = "IGNORE";
                    gameDone[VSPEC.SYSTEMPREFIX] = "PS3";
                    gameDone[VSPEC.SYSTEMCORE] = "PS3";
                    gameDone[VSPEC.OPENROMFILENAME] = "IGNORE";
                    gameDone[VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS] = Array.Empty<string>();
                    gameDone[VSPEC.MEMORYDOMAINS_INTERFACES] = GetInterfaces();
                    gameDone[VSPEC.CORE_DISKBASED] = false;
                }
                AllSpec.VanguardSpec.Update(gameDone);

                //This is local. If the domains changed it propgates over netcore
                LocalNetCoreRouter.Route(RTCV.NetCore.Endpoints.CorruptCore, RTCV.NetCore.Commands.Remote.EventDomainsUpdated, true, true);

                //Asks RTC to restrict any features unsupported by the stub
                LocalNetCoreRouter.Route(RTCV.NetCore.Endpoints.CorruptCore, RTCV.NetCore.Commands.Remote.EventRestrictFeatures, true, true);
            }
            catch (Exception ex)
            {
                if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
                    throw new RTCV.NetCore.AbortEverythingException();
            }
        }

        public static MemoryDomainProxy[] GetInterfaces()
        {
            String connectormessage = "";
            try
            {
                if (NetStubMode == "Linux")
                {
                    Console.WriteLine($"getInterfaces()");
                    List<MemoryDomainProxy> interfaces = new List<MemoryDomainProxy>();
                    Connector.SendMessage("GetMaxMaps");
                    connectormessage = Connector.RecMessage();
                    int maxmaps = int.Parse(connectormessage.Substring(connectormessage.IndexOf("GetMaxMaps: ")));
                    Console.WriteLine("Max amount of maps is " + maxmaps);
                    for (int i = 0; i <= maxmaps; i++)
                    {
                        LinuxMemoryDomain lmd = new LinuxMemoryDomain(i);
                        interfaces.Add(new MemoryDomainProxy(lmd));
                    }
                    Console.WriteLine("Done adding domains");
                    Thread.Sleep(1000);
                    return interfaces.ToArray();
                }
                if (NetStubMode == "PS3")
                {
                    Console.WriteLine($"getInterfaces()");
                    List<MemoryDomainProxy> interfaces = new List<MemoryDomainProxy>();
                    PS3_LV2MemoryDomain lv2 = new PS3_LV2MemoryDomain();
                    if (PS3_AccessProcess == true) 
                    { 
                        PS3_ProcessMemoryDomain proc = new PS3_ProcessMemoryDomain(); 
                        interfaces.Add(new MemoryDomainProxy(proc));
                    }
                    interfaces.Add(new MemoryDomainProxy(lv2));
                    Console.WriteLine("Done adding domains");
                    Thread.Sleep(1000);
                    return interfaces.ToArray();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void EnableInterface()
        {
        }

        public static void DisableInterface()
        {
        }

        public static bool IsProcessBlacklisted(Process _p)
        {
            if (!UseBlacklist)
                return false;

            try
            {
                return IsModuleBlacklisted(_p.MainModule);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"IsProcessBlacklisted threw exception {e}");
                return true;
            }
        }

        public static bool IsModuleBlacklisted(ProcessModule pm)
        {
            if (!UseBlacklist)
                return false;
            try
            {
                var filename = "";
                if (!string.IsNullOrWhiteSpace(pm?.FileName))
                    filename = Path.GetFileName(pm.FileName);

                if (IsExecutableNameBlacklisted(filename))
                    return true;

                if (IsPathBlacklisted(filename))
                    return true;

                if (pm?.FileVersionInfo?.ProductName != null)
                    if (IsProductNameBlacklisted(pm.FileVersionInfo?.ProductName))
                        return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"IsModuleBlacklisted failed!\n{e.Message}\n{e.StackTrace}");
                return false;
            }

            return false;
        }

        public static bool IsPathBlacklisted(string path)
        {
            if (!UseBlacklist)
                return false;

            var blacklisted = new string[]
            {
                "\\Windows\\",
                "\\windows\\",
            };
            if (blacklisted.Any(x => path.Contains(x)))
                return true;
            return false;
        }
        public static bool IsProductNameBlacklisted(string productName)
        {
            if (!UseBlacklist)
                return false;

            var blacklisted = new string[]
            {
                "Microsoft® Windows® Operating System",
            };
            if (blacklisted.Any(x => productName.Equals(x, StringComparison.OrdinalIgnoreCase)))
                return true;
            return false;
        }

        public static bool IsExecutableNameBlacklisted(string executableName)
        {
            //We have certain files we never want corrupted because we know people are going to be stupid and corrupt these online games
            var hardBlacklisted = new string[]
            {
                "r5apex", //Apex Legends
                "Roblox", //Roblox
                "RobloxPlayerLauncher", //Roblox
                "FortniteLauncher",
                "FortniteClient-Win64-Shipping_EAC",
                "FortniteClient-Win64-Shipping_BE",
                "FortniteClient-Win64-Shipping",
                "TsLGame", //pubg
                "SteamService", //Hosts VAC
                "Steam",
                "steamwebhelper",
                "Origin",
                "OriginWebHelperService",
                "Discord",
            };
            if (hardBlacklisted.Any(x => executableName.Equals(x, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (!UseBlacklist)
                return false;

            var blacklisted = Array.Empty<string>();
            if (blacklisted.Any(x => executableName.Equals(x, StringComparison.OrdinalIgnoreCase)))
                return true;
            
            return false;
        }
    }
}
