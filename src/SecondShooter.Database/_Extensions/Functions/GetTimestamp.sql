CREATE FUNCTION [_Extensions].[GetTimestamp]()
RETURNS DATETIMEOFFSET
AS
BEGIN
	RETURN CAST(SESSION_CONTEXT(N'EX_TIME_STAMP') AS DATETIMEOFFSET)
END
