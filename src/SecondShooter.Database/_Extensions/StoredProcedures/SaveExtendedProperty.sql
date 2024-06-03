
CREATE PROCEDURE [_Extensions].[SaveExtendedProperty]
	@name sysname,
	@value sql_variant			= NULL,
	@level0type	varchar(128)	= NULL,
	@level0name	sysname			= NULL,
	@level1type	varchar(128)	= NULL,
	@level1name	sysname			= NULL,
	@level2type	varchar(128)	= NULL,
	@level2name	sysname			= NULL
AS
BEGIN
	IF EXISTS (
	SELECT *
	FROM fn_listextendedproperty (@name, @level0type,@level0name,@level1type,@level1name,@level2type,@level2name)
	)
	BEGIN
		IF (@value IS NULL)
		BEGIN
			EXEC sp_dropextendedproperty @name,@level0type,@level0name,@level1type,@level1name,@level2type,@level2name;
		END
		ELSE 
		BEGIN
			EXEC sp_updateextendedproperty @name,@value,@level0type,@level0name,@level1type,@level1name,@level2type,@level2name;
		END
	END
	ELSE 
	BEGIN
		EXEC sp_addextendedproperty @name,@value,@level0type,@level0name,@level1type,@level1name,@level2type,@level2name;
	END
END
