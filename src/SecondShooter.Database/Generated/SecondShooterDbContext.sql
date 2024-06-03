CREATE TABLE [ImageFiles] (
    [_id] uniqueidentifier NOT NULL,
    [RelativePath] nvarchar(512) NOT NULL,
    [FileName] nvarchar(512) NOT NULL,
    [Extension] nvarchar(25) NULL,
    [Hash] uniqueidentifier NOT NULL,
    [PathHash] uniqueidentifier NOT NULL,
    [Exists] bit NOT NULL,
    CONSTRAINT [PK_ImageFiles] PRIMARY KEY ([_id])
);
GO


