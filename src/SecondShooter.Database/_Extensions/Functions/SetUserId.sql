CREATE FUNCTION [_Extensions].[SetUserId](
	@userId UNIQUEIDENTIFIER
)
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
	EXEC [sys].[sp_set_session_context] @key = 'EX_USER_ID', @value=@userId, @readonly = 1
	RETURN @userId
END