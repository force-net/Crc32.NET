::C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe /t:Rebuild /p:Configuration=Build Crc32.NET\Crc32.NET.csproj
dotnet build -c BuildCore Crc32.NET\Crc32.NET.Core.csproj
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe" -R Crc32.NET\bin\BuildCore\net20\Crc32.NET.dll private.snk 
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe" -R Crc32.NET\bin\BuildCore\netstandard1.3\Crc32.NET.dll private.snk
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe" -R Crc32.NET\bin\BuildCore\netstandard2.0\Crc32.NET.dll private.snk
.tools\nuget.exe pack
xcopy *.nupkg .tools
del *.nupkg
