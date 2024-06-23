CREATE FUNCTION [_Extensions].[SetTimestamp](
	@timestamp DATETIMEOFFSET
)
RETURNS DATETIMEOFFSET
AS
BEGIN
	EXEC [sys].[sp_set_session_context] @key = 'EX_TIME_STAMP', @value=@timestamp, @readonly = 1
	RETURN @timestamp
END
