CREATE OR ALTER PROCEDURE dbo.spPosts_Insert
  @UserId      INT,
  @Title       NVARCHAR(150),
  @Body        NVARCHAR(MAX),
  @DateCreated DATETIME2
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.Posts (UserId, Title, Body, DateCreated)
  VALUES (@UserId, @Title, @Body, @DateCreated);

  -- optional if you ever want the new Id back:
  -- SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
