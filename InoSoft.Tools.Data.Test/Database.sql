USE [InoSoft.Tools.Data.Test]

CREATE TABLE [dbo].[Human](
	[Id] [bigint] NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL
) ON [PRIMARY]
GO

CREATE PROCEDURE [dbo].[GetHumansCount]
AS
BEGIN
	SELECT COUNT(*) FROM Human
END
GO

CREATE PROCEDURE [dbo].[GetHumans]
AS
BEGIN
	SELECT * FROM Human
END
GO

CREATE PROCEDURE [dbo].[GetHumanById]
	@id bigint
AS
BEGIN
	SELECT * FROM Human WHERE Id = @id
END
GO

CREATE PROCEDURE AddHuman
	@id bigint,
	@firstName varchar(50),
	@lastName varchar(50)
AS
BEGIN
	INSERT INTO Human VALUES(@id, @firstName, @lastName)
END
GO

CREATE PROCEDURE GetHumanViaOutput
	@id bigint,
	@firstName varchar(50) output,
	@lastName varchar(50) output
AS
BEGIN
	SELECT @firstName = FirstName, @lastName = LastName
	FROM Human WHERE Id = @id
END
GO

CREATE PROCEDURE GetRandomHumanViaOutput
	@id bigint output,
	@firstName varchar(50) output,
	@lastName varchar(50) output
AS
BEGIN
	SELECT @id = Id, @firstName = FirstName, @lastName = LastName	FROM Human
END