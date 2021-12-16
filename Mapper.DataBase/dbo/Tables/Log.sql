CREATE TABLE [dbo].[Log] (
    [data]       DATETIME       NOT NULL,
    [type]       INT            NOT NULL,
    [message]    NVARCHAR (MAX) NOT NULL,
    [user]       NVARCHAR (50)  NULL,
    [IP]         VARCHAR (50)   NULL,
    [request]    NVARCHAR (MAX) NULL,
    [user-agent] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([data] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_Log_LogType] FOREIGN KEY ([type]) REFERENCES [dbo].[LogType] ([id])
);

