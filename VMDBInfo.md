# Virtual Machine #

## Introduction ##

Our virtual machine is running Windows Server 2003 Web Edition SP2. The .vhd file for the VM is approximately 6Gb in size. It was created in Microsoft Virtual PC 2007 ([Download](http://www.microsoft.com/downloads/details.aspx?FamilyId=04D26402-3199-48A3-AFA2-2DC0B40A73B6&displaylang=en)).

## Information ##
  * Computer Name: POLYTECH-DEV
  * Username: Administrator
  * Password: polytech

## Set-up ##

  1. Go to [here](http://www.microsoft.com/downloads/details.aspx?FamilyId=04D26402-3199-48A3-AFA2-2DC0B40A73B6&displaylang=en) to download the file. Scroll to the bottom of the download page and select the appropriate x86 or x64 version.
  1. Obtain a copy of the .vhd(and .vmc if you are using Virtual PC).
  1. Start Virtual PC 2007.
  1. Click New, then Next.
  1. Select "Add an existing virtual machine" and click Next.
  1. Navigate to the Windows Server2003.vmc file on your computer and click Next.
  1. If the .vhd and .vmc are in the same folder, it should automatically use the .vhd. To verify this, click Windows Server 2003 in the Virtual PC Console and click Settings. Click Hard Disk 1 and verifiy that is is using the Virtual Hard Disk file "Windows Server 2003 Hard Disk.vhd".
  1. To allow your computer to talk to the virtual machine you may need to install and configure the loopback adapter. See below.

## Installing the loopback adapter ##

  1. Go to [here](http://www.windowsreference.com/windows-7/how-to-install-a-loopback-adapter-in-windows-7/) for instructions to install the loopback adapter in Windows Vista/7.
  1. Click Start and type "Network Connections" and hit enter.
  1. Right click the loopback adapter and click properties.
  1. Double click Internet Protocol Version 4.
  1. Set the IP address to 10.200.1.101, subnet 255.0.0.0 and click ok.
  1. In the virtual machine settings, click network, and set the only adapter to Microsoft Loopback Adapter.
  1. Start the virtual machine, click Start -> Control Panel -> Local Area Connection.
  1. Click properties, double click Internet Protocol Version 4.
  1. Set the IP address to 10.200.1.102, subnet 255.0.0.0, default gateway 10.200.1.101.
  1. Click OK.

## Virtual PC Shortcuts ##

  * On the log in screen, press right-alt + delete to send Ctrl+Alt+Del to the VM. This works at any time you need to use Ctrl+Alt+Del in the VM.
  * Press Right-alt+Enter to enter full screen mode in the VM. This is easier to use than windowed mode if you will be in the VM for extended periods of time.
  * If at any time the VM "steals" your mouse and you can't move it out of the window, press Right-alt to free it.

# Databases #

## Installed Databases ##
The following databases are installed on the virtual machine:
  * MySQL 5.1.42
  * Oracle 10 Express Edition
  * Microsoft SQL 2005

Tables names are the same across all databases and correspond to the file names of the data files given to us by ERDC.
  * msustudentdatalocks
  * LOCKLocations

### MySQL ###
  * Installation path: Install path: C:\Program Files\MySQL\MySQL Server 5.1
  * Username: root
  * Password: polytech
  * Port: 3306
  * Database name: test

The easiest(?) way to access MySQL manually is the MySQL command prompt(Start -> Programs -> MySQL -> MySQL Server 5.1 -> MySQL Server Command Line Client). Use standard SQL commands to perform queries, etc.

### Oracle ###
  * Installation path: C:\oraclexe
  * Username: POLYTECH
  * Password: polytech
  * Port: 1521
  * Database name: POLYTECH

Oracle's server has a PHPMyAdmin-like web based configuration client which can be found at Start -> Programs -> Oracle Database 10g Express Edition -> Go to database home page. You can perform queries using the web client.

### MS SQL ###
  * Installation path: C:\Program Files\Microsoft SQL Server
  * User Windows username and password OR
  * Username: sa
  * Password: polytech
  * Port: 1433
  * Database name: Polytech

Microsoft SQL Server Management Studio Express allows you to view the data in a table and perform SQL commands. You can right click a table and

## Installed Tools ##
The following tools are installed on the virtual machine:
  * Microsoft SQL Server Management Studio Express
  * Oracle SQL Developer
  * SQL Importer (30 day limited trial)