
# RostamBot
Twitter Bot to help fight fake accounts with Identifying and Group blocking them

[![Build Status](https://github.com/hameds/RostamBot/workflows/ASP.NET%20Core%20CI/badge.svg)](https://github.com/Hameds/RostamBot/actions)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/e6f5e765d78347d485e5b6e9dc66d019)](https://www.codacy.com/app/Hameds/RostamBot?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Hameds/RostamBot&amp;utm_campaign=Badge_Grade)

## Getting Started

This project is based on ASP.NET Core. so you shouldn't have any problem running this in linux, windows or mac. 

### Prerequisites

- [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2)
- [Twitter Developer Account](https://developer.twitter.com)
- SQL Server. If you are on linux, you can use [SQL Server container images](https://hub.docker.com/_/microsoft-mssql-server) with Docker

### How to run

clone the source code and then run following command in root folder of project.

```
dotnet build
```

then you need to change settings in `appsettings.json` file inside `RostamBot.Web` folder. note that you can use [ASP.NET Core user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.0&tabs=windows) too. then you can run project with following command

```
dotnet run -p RostamBot.Web
```

