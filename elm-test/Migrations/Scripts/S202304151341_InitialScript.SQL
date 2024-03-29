﻿USE [$DatabaseName$]
GO

IF 1=$ShouldCreateDbUser$
BEGIN
	IF SUSER_ID('$DbUserName$') IS NULL
	BEGIN
		CREATE LOGIN [$DbUserName$] WITH PASSWORD=N'$DbUserPassword$', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
	END
	IF USER_ID('$DbUserName$') IS NULL
	BEGIN
		CREATE USER [$DbUserName$] FOR LOGIN [$DbUserName$] WITH DEFAULT_SCHEMA=[dbo]
	END
	ALTER ROLE [db_owner] ADD MEMBER [$DbUserName$]
END
GO