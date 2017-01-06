@echo off
setlocal
setlocal enabledelayedexpansion
setlocal enableextensions
set errorlevel=0
mkdir ..\nuget
erase /s ..\nuget\Microsoft.Bot.CognitiveServices.QnAMaker*nupkg
msbuild /property:Configuration=release Microsoft.Bot.Builder.CognitiveServices.QnAMaker.csproj 
for /f %%v in ('powershell -noprofile "(Get-Command .\bin\release\Microsoft.Bot.Builder.dll).FileVersionInfo.FileVersion"') do set builder=%%v
for /f %%v in ('powershell -noprofile "(Get-Command .\bin\release\Microsoft.Bot.Builder.CognitiveServices.QnAMaker.dll).FileVersionInfo.FileVersion"') do set version=%%v
..\packages\NuGet.CommandLine.3.4.3\tools\NuGet.exe pack Microsoft.Bot.Builder.CognitiveServices.QnAMaker.nuspec -symbols -properties version=%version%;builder=%builder% -OutputDirectory ..\nuget