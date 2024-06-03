

SET DATABASE_PROJECT=SecondShooter.Database
SET PRESISTANCE_PROJECT=SecondShooter.Persistance
SET EF_CONTEXT=SecondShooterDbContext

sqllocaldb create secondshooter
sqllocaldb start secondshooter

dotnet build %PRESISTANCE_PROJECT%

dotnet ef dbcontext script ^
--project %PRESISTANCE_PROJECT% ^
--output ..\%DATABASE_PROJECT%\Generated\%EF_CONTEXT%.sql

dotnet build %DATABASE_PROJECT% ^
--output .\publish\database

dotnet publish %DATABASE_PROJECT%


REM BACKUP DATABASE [secondshooter] TO  DISK = N'C:\Repos\Storage\backup\secondshooter.bak' 