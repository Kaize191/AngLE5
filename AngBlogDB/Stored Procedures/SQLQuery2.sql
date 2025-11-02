CREATE OR ALTER PROCEDURE dbo.spUsers_Register
  @UserName  NVARCHAR(50),
  @Password  NVARCHAR(50),
  @FirstName NVARCHAR(50),
  @LastName  NVARCHAR(50)
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.Users (UserName, [Password], FirstName, LastName)
  VALUES (@UserName, @Password, @FirstName, @LastName);
END
