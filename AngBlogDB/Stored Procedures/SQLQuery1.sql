CREATE OR ALTER PROCEDURE dbo.spUsers_Authenticate
  @UserName NVARCHAR(50),
  @Password NVARCHAR(50)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT TOP (1) Id, UserName, FirstName, LastName
  FROM dbo.Users
  WHERE UserName = @UserName AND Password = @Password;
END
