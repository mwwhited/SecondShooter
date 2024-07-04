IF SCHEMA_ID(N'Files') IS NULL EXEC(N'CREATE SCHEMA [Files];');
GO


CREATE TABLE [Files].[Entries] (
    [FileEntryID] UNIQUEIDENTIFIER DEFAULT newsequentialid() NOT NULL,
    [RelativePath] nvarchar(512) NOT NULL,
    [FileName] nvarchar(512) NOT NULL,
    [Extension] nvarchar(25) NULL,
    [Hash] uniqueidentifier NOT NULL,
    [PathHash] nvarchar(100) NOT NULL,
    [Exists] bit NOT NULL,
    CONSTRAINT [PK_Entries] PRIMARY KEY ([FileEntryID])
);
GO


CREATE TABLE [Files].[Contents] (
    [FileContentID] UNIQUEIDENTIFIER DEFAULT newsequentialid() NOT NULL,
    [FileEntryID] UNIQUEIDENTIFIER NOT NULL,
    [ContentType] nvarchar(25) NOT NULL,
    [Category] nvarchar(25) NOT NULL,
    [Container] nvarchar(100) NOT NULL,
    [Reference] nvarchar(512) NOT NULL,
    [Offset] bigint NOT NULL,
    [Length] bigint NOT NULL,
    CONSTRAINT [PK_Contents] PRIMARY KEY ([FileContentID]),
    CONSTRAINT [FK_Contents_Entries_FileEntryID] FOREIGN KEY ([FileEntryID]) REFERENCES [Files].[Entries] ([FileEntryID]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_Contents_FileEntryID] ON [Files].[Contents] ([FileEntryID]);
GO


