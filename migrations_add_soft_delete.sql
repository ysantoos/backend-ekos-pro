IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [CatalogBooks] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(500) NOT NULL,
    [Author] nvarchar(500) NOT NULL,
    [Isbn] nvarchar(500) NOT NULL,
    [Category] nvarchar(500) NOT NULL,
    [Publisher] nvarchar(500) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [PublicationYear] int NULL,
    [TotalCopies] int NOT NULL DEFAULT 0,
    [CoverColor] nvarchar(500) NULL,
    [AvailabilityStatus] nvarchar(500) NULL,
    [AvailableCopies] int NULL DEFAULT 0,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(500) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(500) NULL,
    CONSTRAINT [PK_CatalogBooks] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_CatalogBook_AvailableCopies_LessThanOrEqualTotal] CHECK ([AvailableCopies] <= [TotalCopies]),
    CONSTRAINT [CK_CatalogBook_AvailableCopies_NonNegative] CHECK ([AvailableCopies] >= 0),
    CONSTRAINT [CK_CatalogBook_PublicationYear_Valid] CHECK ([PublicationYear] IS NULL OR ([PublicationYear] >= 1000 AND [PublicationYear] <= YEAR(GETDATE()))),
    CONSTRAINT [CK_CatalogBook_TotalCopies_NonNegative] CHECK ([TotalCopies] >= 0)
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'Catalog of books available in the system';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks';
SET @description = N'Book title';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Title';
SET @description = N'Book author';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Author';
SET @description = N'International Standard Book Number';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Isbn';
SET @description = N'Book category or genre';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Category';
SET @description = N'Publisher name';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Publisher';
SET @description = N'Book description';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'Description';
SET @description = N'Year of publication';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'PublicationYear';
SET @description = N'Total number of copies';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'TotalCopies';
SET @description = N'Cover color of the book';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'CoverColor';
SET @description = N'Current availability status';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'AvailabilityStatus';
SET @description = N'Number of available copies';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'CatalogBooks', 'COLUMN', N'AvailableCopies';

CREATE INDEX [IX_CatalogBook_Author] ON [CatalogBooks] ([Author]);

CREATE INDEX [IX_CatalogBook_Category] ON [CatalogBooks] ([Category]);

CREATE INDEX [IX_CatalogBook_Category_Author] ON [CatalogBooks] ([Category], [Author]);

CREATE INDEX [IX_CatalogBook_CreatedAt] ON [CatalogBooks] ([CreatedAt]);

CREATE INDEX [IX_CatalogBook_Publisher] ON [CatalogBooks] ([Publisher]);

CREATE INDEX [IX_CatalogBook_Title] ON [CatalogBooks] ([Title]);

CREATE UNIQUE INDEX [UX_CatalogBook_Isbn] ON [CatalogBooks] ([Isbn]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260815224153_InitialCreate', N'9.0.19');

ALTER TABLE [CatalogBooks] DROP CONSTRAINT [CK_CatalogBook_AvailableCopies_LessThanOrEqualTotal];

ALTER TABLE [CatalogBooks] DROP CONSTRAINT [CK_CatalogBook_AvailableCopies_NonNegative];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogBooks]') AND [c].[name] = N'AvailabilityStatus');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [CatalogBooks] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [CatalogBooks] DROP COLUMN [AvailabilityStatus];

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogBooks]') AND [c].[name] = N'AvailableCopies');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CatalogBooks] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CatalogBooks] DROP COLUMN [AvailableCopies];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260815224858_RemoveCalculatedFields', N'9.0.19');

ALTER TABLE [CatalogBooks] DROP CONSTRAINT [CK_CatalogBook_TotalCopies_NonNegative];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogBooks]') AND [c].[name] = N'TotalCopies');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [CatalogBooks] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [CatalogBooks] DROP COLUMN [TotalCopies];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260815225950_RemoveTotalCopiesField', N'9.0.19');

CREATE TABLE [LoanHistoryEntries] (
    [Id] uniqueidentifier NOT NULL,
    [BookId] nvarchar(500) NOT NULL,
    [UserName] nvarchar(500) NOT NULL,
    [LoanDate] datetime2 NULL,
    [ReturnDate] datetime2 NULL,
    [IsReturned] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(500) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(500) NULL,
    CONSTRAINT [PK_LoanHistoryEntries] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema4 AS sysname;
SET @defaultSchema4 = SCHEMA_NAME();
DECLARE @description4 AS sql_variant;
SET @description4 = N'Historical records of book loans and returns';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries';
SET @description4 = N'Reference id of the book (no FK)';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'BookId';
SET @description4 = N'Name of the user who borrowed the book';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'UserName';
SET @description4 = N'Date when the book was loaned';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'LoanDate';
SET @description4 = N'Date when the book was returned';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'ReturnDate';
SET @description4 = N'Whether the book has been returned';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'IsReturned';

CREATE INDEX [IX_LoanHistory_BookId] ON [LoanHistoryEntries] ([BookId]);

CREATE INDEX [IX_LoanHistory_LoanDate] ON [LoanHistoryEntries] ([LoanDate]);

CREATE INDEX [IX_LoanHistory_UserName] ON [LoanHistoryEntries] ([UserName]);

CREATE INDEX [IX_LoanHistoryEntry_CreatedAt] ON [LoanHistoryEntries] ([CreatedAt]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260815233936_AddLoanHistoryEntry', N'9.0.19');

ALTER TABLE [CatalogBooks] ADD [TotalCopies] int NOT NULL DEFAULT 0;
DECLARE @defaultSchema5 AS sysname;
SET @defaultSchema5 = SCHEMA_NAME();
DECLARE @description5 AS sql_variant;
SET @description5 = N'Total number of copies';
EXEC sp_addextendedproperty 'MS_Description', @description5, 'SCHEMA', @defaultSchema5, 'TABLE', N'CatalogBooks', 'COLUMN', N'TotalCopies';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260816005552_ReAddTotalCopies', N'9.0.19');

DROP INDEX [IX_LoanHistory_UserName] ON [LoanHistoryEntries];

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LoanHistoryEntries]') AND [c].[name] = N'UserName');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [LoanHistoryEntries] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [LoanHistoryEntries] DROP COLUMN [UserName];

ALTER TABLE [LoanHistoryEntries] ADD [Email] nvarchar(500) NULL;
DECLARE @defaultSchema7 AS sysname;
SET @defaultSchema7 = SCHEMA_NAME();
DECLARE @description7 AS sql_variant;
SET @description7 = N'Email of the user who borrowed the book';
EXEC sp_addextendedproperty 'MS_Description', @description7, 'SCHEMA', @defaultSchema7, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'Email';

ALTER TABLE [LoanHistoryEntries] ADD [FullName] nvarchar(500) NOT NULL DEFAULT N'';
DECLARE @defaultSchema8 AS sysname;
SET @defaultSchema8 = SCHEMA_NAME();
DECLARE @description8 AS sql_variant;
SET @description8 = N'Full name of the user who borrowed the book';
EXEC sp_addextendedproperty 'MS_Description', @description8, 'SCHEMA', @defaultSchema8, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'FullName';

ALTER TABLE [LoanHistoryEntries] ADD [MobilePhone] nvarchar(500) NULL;
DECLARE @defaultSchema9 AS sysname;
SET @defaultSchema9 = SCHEMA_NAME();
DECLARE @description9 AS sql_variant;
SET @description9 = N'Mobile phone number of the user who borrowed the book';
EXEC sp_addextendedproperty 'MS_Description', @description9, 'SCHEMA', @defaultSchema9, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'MobilePhone';

CREATE INDEX [IX_LoanHistory_FullName] ON [LoanHistoryEntries] ([FullName]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260816010439_UpdateLoanHistoryFields', N'9.0.19');

ALTER TABLE [LoanHistoryEntries] ADD [DeletedAt] datetime2 NULL;
DECLARE @defaultSchema10 AS sysname;
SET @defaultSchema10 = SCHEMA_NAME();
DECLARE @description10 AS sql_variant;
SET @description10 = N'Timestamp when the entity was soft-deleted';
EXEC sp_addextendedproperty 'MS_Description', @description10, 'SCHEMA', @defaultSchema10, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'DeletedAt';

ALTER TABLE [LoanHistoryEntries] ADD [DeletedBy] nvarchar(500) NULL;
DECLARE @defaultSchema11 AS sysname;
SET @defaultSchema11 = SCHEMA_NAME();
DECLARE @description11 AS sql_variant;
SET @description11 = N'User who soft-deleted the entity';
EXEC sp_addextendedproperty 'MS_Description', @description11, 'SCHEMA', @defaultSchema11, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'DeletedBy';

ALTER TABLE [LoanHistoryEntries] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
DECLARE @defaultSchema12 AS sysname;
SET @defaultSchema12 = SCHEMA_NAME();
DECLARE @description12 AS sql_variant;
SET @description12 = N'Indicates whether the entity has been soft-deleted';
EXEC sp_addextendedproperty 'MS_Description', @description12, 'SCHEMA', @defaultSchema12, 'TABLE', N'LoanHistoryEntries', 'COLUMN', N'IsDeleted';

ALTER TABLE [CatalogBooks] ADD [DeletedAt] datetime2 NULL;
DECLARE @defaultSchema13 AS sysname;
SET @defaultSchema13 = SCHEMA_NAME();
DECLARE @description13 AS sql_variant;
SET @description13 = N'Timestamp when the entity was soft-deleted';
EXEC sp_addextendedproperty 'MS_Description', @description13, 'SCHEMA', @defaultSchema13, 'TABLE', N'CatalogBooks', 'COLUMN', N'DeletedAt';

ALTER TABLE [CatalogBooks] ADD [DeletedBy] nvarchar(500) NULL;
DECLARE @defaultSchema14 AS sysname;
SET @defaultSchema14 = SCHEMA_NAME();
DECLARE @description14 AS sql_variant;
SET @description14 = N'User who soft-deleted the entity';
EXEC sp_addextendedproperty 'MS_Description', @description14, 'SCHEMA', @defaultSchema14, 'TABLE', N'CatalogBooks', 'COLUMN', N'DeletedBy';

ALTER TABLE [CatalogBooks] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
DECLARE @defaultSchema15 AS sysname;
SET @defaultSchema15 = SCHEMA_NAME();
DECLARE @description15 AS sql_variant;
SET @description15 = N'Indicates whether the entity has been soft-deleted';
EXEC sp_addextendedproperty 'MS_Description', @description15, 'SCHEMA', @defaultSchema15, 'TABLE', N'CatalogBooks', 'COLUMN', N'IsDeleted';

CREATE INDEX [IX_LoanHistoryEntry_IsDeleted] ON [LoanHistoryEntries] ([IsDeleted]);

CREATE INDEX [IX_CatalogBook_IsDeleted] ON [CatalogBooks] ([IsDeleted]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260816135136_AddSoftDeleteToBaseEntity', N'9.0.19');

COMMIT;
GO

