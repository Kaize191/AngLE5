CREATE PROCEDURE dbo.spPosts_Insert
    @Title NVARCHAR(200),
    @Body NVARCHAR(MAX),
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Posts (Title, Body, DateCreated, UserId)
    VALUES (@Title, @Body, GETDATE(), @UserId);
END
