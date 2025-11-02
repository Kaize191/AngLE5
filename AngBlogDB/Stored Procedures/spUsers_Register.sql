CREATE PROCEDURE [dbo].[spUsers_Register]
    @Username NVARCHAR(16),
    @Password NVARCHAR(50),
    @Firstname NVARCHAR(50),
    @Lastname NVARCHAR(16)
AS
BEGIN
    INSERT INTO dbo.Users (Username, [Password], Firstname, Lastname)
    VALUES (@Username, @Password, @Firstname, @Lastname);
END
