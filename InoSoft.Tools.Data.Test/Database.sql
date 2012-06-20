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

CREATE PROCEDURE AddHuman
	@id bigint,
	@firstName varchar(50),
	@lastName varchar(50)
AS
BEGIN
	INSERT INTO Human VALUES(@id, @firstName, @lastName)
END