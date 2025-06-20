# Wijoyo_fdtest

## Prerequisites

1. Node
2. .Net SDK 9+
3. Docker

## How To Run?

### Backend

```bash
cd Wijoyo_fdtest
```

#### Then

```bash
docker compose up -d --force-recreate && docker exec -it wijoyo_fdtest-mssql-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "me@Passw0rdDatabase" -C -N -Q "CREATE DATABASE Wijoyo_fdtest"
```

#### Next

```bash
dotnet clean && dotnet run --project src/Web
```


### Frontend

```bash
cd wijoyo-fdtest-fe-react && npm run dev
```

### P.S

#### MinIO (File S3 Compatible)

localhost:9001
user + password = minioadmin

#### Papercut (Email Provider)

localhost:8080

#### Technology Used

Backend: ASP.Net Core, JWT (For easy authentication), MinIO (For storage S3 compatible), Papercut (email provider), MediatR (For middleware), EF Core + MSSQL Server (of course it's .NET), FluentValidation (Easy and neat validation for .NET)

Frontend: React (For dynamic website), TailwindCSS (Simple css), Vite (Modern build tools for frontend web apps), ReactRouter, axios (For hit backend endpoint)
