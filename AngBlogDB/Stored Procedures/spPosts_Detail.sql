CREATE PROCEDURE dbo.spPosts_Detail
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id,
        p.Title,
        p.Body,
        p.DateCreated,
        u.UserName,
        u.FirstName,
        u.LastName
    FROM dbo.Posts p
    INNER JOIN dbo.Users u ON u.Id = p.UserId
    WHERE p.Id = @Id;
END
