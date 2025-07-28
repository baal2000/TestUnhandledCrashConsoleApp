# TestUnhandledCrashConsoleApp
Test for issue [#118049](https://github.com/dotnet/runtime/issues/118049) (Ubuntu Linux)

1. Clone TestUnhandledCrashConsoleApp source code
2. Modify TestUnhandledCrashConsoleApp.csproj to select the target framework 
```
    <TargetFramework>net10.0</TargetFramework>
```
Or
```
    <TargetFramework>net9.0</TargetFramework>
```
3. Open terminal session and change current directory to the root directory of the project
4. Publish the application as a self-contained executable for the Linux x64 runtime.
```
    dotnet publish -c Release -r linux-x64
```
5. Modify Docker file to pick either .NET 9 or .NET 10 by commenting/uncommenting either of:
```
   FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-preview.6-noble-amd64
```
   OR
```
   FROM mcr.microsoft.com/dotnet/aspnet:9.0
```
   and respectively either of:
```
   COPY ./bin/Release/net10.0/linux-x64/publish/ .
```
   OR
```
   COPY ./bin/Release/net9.0/linux-x64/publish/ .
```
6. Save Docker file
7. Execute   
```
   docker build -t pid1-crash-app .
```
8. Execute the following test cases:

A. **FAILS** in 9.0, **PASSES** in 10.0 preview 6: Application crashes, creates a dump, and then hangs.
```
docker run --rm -v ~/Downloads:/dumps -e "DOTNET_DbgEnableMiniDump=1" -e "DOTNET_DbgMiniDumpType=1" -e "DOTNET_DbgMiniDumpName=/dumps/coredump.%p" pid1-crash-app
```
B. **PASSES** for both 9.0 and 10.0: Application crashes and exits correctly (no dump created).
```
docker run --rm -v ~/Downloads:/dumps -e "DOTNET_DbgEnableMiniDump=0" -e "DOTNET_DbgMiniDumpType=1" -e "DOTNET_DbgMiniDumpName=/dumps/coredump.%p" pid1-crash-app
``` 
C. **PASSES** for both 9.0 and 10.0: Application is not PID 1, creates a dump, and exits correctly.
```
docker run --rm --init -v ~/Downloads:/dumps -e "DOTNET_DbgEnableMiniDump=1" -e "DOTNET_DbgMiniDumpType=1" -e "DOTNET_DbgMiniDumpName=/dumps/coredump.%p" pid1-crash-app
```   


