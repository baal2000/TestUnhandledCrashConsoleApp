# Use the official .NET 10 runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-preview.6-noble-amd64

# OR the official .NET 9 runtime base image
#FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Set the working directory inside the container
WORKDIR /app

# Copy the 10.0 published application files into the container
COPY ./bin/Release/net10.0/linux-x64/publish/ .

# OR the 9.0 published application
#COPY ./bin/Release/net9.0/linux-x64/publish/ .

# Set the entrypoint to run the application
# Using this "exec" form ensures your app is PID 1
ENTRYPOINT ["./TestUnhandledCrashConsoleApp"]
