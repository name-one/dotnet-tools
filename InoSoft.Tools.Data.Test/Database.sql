USE [InoSoft.Tools.Data.Test]

CREATE TABLE [dbo].[Human](
	[Id] [bigint] PRIMARY KEY NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL
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