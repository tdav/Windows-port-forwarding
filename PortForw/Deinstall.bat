@echo off
net stop pfService
c:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u PortForw.exe
