CREATE TABLE [ImageFiles] (
    [Id] uniqueidentifier NOT NULL,
    [RelativePath] nvarchar(max) NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [Extension] nvarchar(max) NOT NULL,
    [Hash] uniqueidentifier NOT NULL,
    [PathHash] uniqueidentifier NOT NULL,
    [Exists] bit NOT NULL,
    CONSTRAINT [PK_ImageFiles] PRIMARY KEY ([Id])
);
GO


