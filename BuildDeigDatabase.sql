/* Create the tables: */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[aTime](
	[TimeID] [int] NOT NULL,
	[xTime] [nchar](40) NOT NULL,
	[HHMM] [char](5) NULL
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DOW](
	[DayID] [int] IDENTITY(1,1) NOT NULL,
	[DayName] [nchar](10) NOT NULL,
	[Id] [int] NULL,
	[dowid] [int] NULL,
 CONSTRAINT [PK_DOW] PRIMARY KEY CLUSTERED 
(
	[DayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[List](
	[ListID] [int] IDENTITY(1,1) NOT NULL,
	[DOW] [int] NULL,
	[TimeID] [int] NOT NULL,
	[Town] [varchar](25) NOT NULL,
	[GroupName] [varchar](60) NULL,
	[Information] [varchar](85) NULL,
	[Location] [varchar](50) NULL,
	[Type] [varchar](30) NOT NULL,
	[suspend] [bit] NULL,
	[district] [int] NULL,
	[TownId] [int] NULL,
 CONSTRAINT [PK_List] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[onlinemeetings](
	[zoomid] [int] IDENTITY(1,1) NOT NULL,
	[dayid] [int] NOT NULL,
	[timeid] [int] NOT NULL,
	[meetingid] [varchar](15) NULL,
	[pswd] [char](6) NOT NULL,
	[telephone] [char](10) NULL,
	[groupname] [varchar](50) NOT NULL,
	[notes] [varchar](50) NULL,
	[district] [int] NULL,
 CONSTRAINT [PK_onlinemeetings] PRIMARY KEY CLUSTERED 
(
	[zoomid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Groups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [varchar](50) NULL,
	[District] [int] NULL
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

CREATE TABLE [dbo].[Towns](
	[TownId] [int] IDENTITY(1,1) NOT NULL,
	[District] [int] NULL,
	[Town] [varchar](25) NOT NULL
) ON [PRIMARY]
GO

/* Stored Procedures */
CREATE PROCEDURE [dbo].[GetMeetingList]
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  
	Select 
	[suspend],
	TRIM(b.[DayName]) as 'Day',
	TRIM(substring(c.[xTime], 1, 5) + substring(c.[xTime], 9,3)) as 'Time',
	[Town],
	[GroupName] as 'Group Name',
	[Information],
	[Location],
	[Type]
	FROM [dbo].[List] a
	LEFT JOIN [dbo].[DOW] b ON a.[DOW] = b.[Id]
	LEFT JOIN [dbo].[aTime] c ON A.TimeID = c.[TimeID]
	WHERE a.[suspend] = 0
	ORDER BY a.DOW ASC, A.TimeID ASC, [Town] ASC
END


GO

CREATE PROCEDURE [dbo].[spCreateList]
	@Suspend BIT = NULL,
	@DOWID INT = NULL,
	@TimeID INT = NULL,
	@Town VARCHAR(25) = NULL,
	@GroupName VARCHAR (60)= NULL,
	@Information VARCHAR (85) = NULL,
	@Location VARCHAR (50) = NULL,
	@Type	VARCHAR (30) = NULL,
	@District INT = NULL,
	@new_id INT = NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO List
	([DOW], [TimeID], [Town], [GroupName], [Information], [Location], [Type], [suspend],[district])
	VALUES
	(@DOWID, @TimeID, @Town, @GroupName, @Information, @Location, @Type,@Suspend,@District)
	SET @new_id = SCOPE_IDENTITY()
	RETURN @new_id
END 
GO

CREATE PROCEDURE [dbo].[spCreateOnlineList]
	@DayId INT = NULL,
	@TimeId INT = NULL,
	@MeetingId VARCHAR (15),
	@Pswd CHAR (50) = NULL,
	@Telephone CHAR (10) = NULL,
	@GroupName VARCHAR (50),
	@Notes VARCHAR (50) = NULL,
	@District INT = NULL,
	@new_id INT = NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[onlinemeetings]
	([dayid], [timeid], [meetingid], [pswd], [telephone], [groupname], [notes],[district])
	VALUES
	(@DayId, @TimeId,@MeetingId, @Pswd, @Telephone, @GroupName, @Notes,@District)
	SET @new_id = SCOPE_IDENTITY()
	RETURN @new_id
END
GO

CREATE PROCEDURE [dbo].[spCreateTown]
	@District INT = NULL,
	@Town VARCHAR(25),
	@new_id INT = NULL OUTPUT
AS
BEGIN
	
	SET NOCOUNT ON;

	INSERT INTO Towns (District, Town) VALUES (@District, @Town)
	SET @new_id = SCOPE_IDENTITY()
	RETURN @new_id

END
GO

CREATE PROCEDURE [dbo].[spDEIGJson] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @result NVARCHAR(max);

    -- Insert statements for procedure here
	SET @result  = (SELECT 
	l.GroupName AS 'name',
	l.ListId AS 'slug',
	d.dowid AS 'day',
	t.[HHMM] AS 'time',
	JSON_QUERY('["' + replace(rtrim(ltrim(type)), ' ','","') + '"]')  as 'types',
	l.Location AS 'address',
	l.Town AS city,
	'ME' AS state,
	'US' AS country,
	l.Information AS 'location'
	FROM List l
	LEFT JOIN [dbo].[DOW] d ON l.[DOW] = d.[Id]
	LEFT JOIN [dbo].[aTime] t ON l.TimeID = t.[TimeID]
	WHERE l.[suspend] = 0
	ORDER BY d.dowid ASC, t.TimeID ASC, l.[Town] ASC
	FOR JSON PATH)

	SELECT @result;
END
GO

CREATE PROCEDURE [dbo].[spDeleteList]
	@ListId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   DELETE List WHERE ListID = @ListId
END
GO

CREATE PROCEDURE [dbo].[spDeleteOnlineList]
	@ZoomId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   DELETE [dbo].[onlinemeetings] WHERE zoomid = @ZoomId
END
GO

CREATE PROCEDURE  [dbo].[spDeleteTown] 
	
	@TownId INT


AS
BEGIN
	
	SET NOCOUNT ON;

	 DELETE Towns WHERE TownId = @TownId

END
GO


CREATE PROCEDURE [dbo].[spDistrict]

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT district  FROM Towns 
END
GO

CREATE PROCEDURE [dbo].[spDOW]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DayID, DayName  FROM DOW;
END
GO

CREATE PROCEDURE [dbo].[spGetOnlineList]
	@ZoomId INT = NULL,
	@DayId INT = NULL,
	@TimeId INT = NUll,
	@District INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [zoomid],
		c.[dayid],
		c.[DayName],
		a.[timeid],
		substring(b.[xTime], 1, 5) + substring(b.[xTime], 9,3) AS 'Time',
		[groupname],
		[meetingid],
		[pswd],
		[telephone],
		[notes],
		[district]
	FROM [dbo].[onlinemeetings] a
	INNER JOIN [dbo].[aTime] b ON a.[timeid] = b.TimeID
	INNER JOIN [dbo].[DOW] c ON a.[dayid] = c.DayID
	WHERE (a.[zoomid] = @ZoomId OR @ZoomId IS NULL)
	AND (a.[dayid] = @DayId OR @DayId IS NULL)
	AND (a.[timeid] = @TimeId OR @TimeId IS NULL)
	AND (a.[district] = @District OR @District IS NULL)
	ORDER BY c.[dayid],a.[timeid],a.[groupname]
END
GO

CREATE PROCEDURE [dbo].[spList] 
(
	@ListId int = NULL,
	@Suspend BIT = 0,
	@DOWID INTEGER = NULL,
	@TimeID INTEGER = NULL,
	@Town VARCHAR(25) = NULL,
	@District INT = NULL
)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT [ListID],
		a.[DOW], 
		b.[DayName] AS 'Day',
		a.[TimeID],
		substring(c.[xTime], 1, 5) + substring(c.[xTime], 9,3) AS 'Time',
		[Town],
		[GroupName],
		[Information],[Location],
		[Type],
		[suspend],
		[District]
	FROM [dbo].[List] a
	INNER JOIN [dbo].[DOW] b ON a.[DOW] = b.[DayID]
	INNER JOIN [dbo].[aTime] c ON a.[TimeID] = c.[TimeId]
	WHERE (a.ListID = @ListId OR @ListId IS NULL) 
	AND (a.suspend = @Suspend OR @Suspend IS NULL) 
	AND (a.DOW = @DOWID OR @DOWID IS NULL)
	AND (a.TimeID = @TimeID or @TimeID IS NULL)
	AND (Town = @Town or @Town IS NULL)
	AND (District = @District or @District IS NULL)
	-- AND a.[suspend] = @Suspend
	ORDER BY a.[DOW],a.[TimeID], [Town]

END
GO


CREATE PROCEDURE [dbo].[spMaintenanceList] 
(
	@ListId int = NULL,
	@Suspend BIT = NULL,
	@DOWID INTEGER = NULL,
	@TimeID INTEGER = NULL,
	@Town VARCHAR(25) = NULL,
	@District INT = NULL
)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT [ListID],
		a.[DOW], 
		b.[DayName] AS 'Day',
		a.[TimeID],
		substring(c.[xTime], 1, 5) + substring(c.[xTime], 9,3) AS 'Time',
		[Town],
		[GroupName],
		[Information],[Location],
		[Type],
		[suspend],
		[district]
	FROM [dbo].[List] a
	INNER JOIN [dbo].[DOW] b ON a.[DOW] = b.[DayID]
	INNER JOIN [dbo].[aTime] c ON a.[TimeID] = c.[TimeId]
	WHERE (a.ListID = @ListId OR @ListId IS NULL) 
	AND (a.suspend = @Suspend OR @Suspend IS NULL) 
	AND (a.DOW = @DOWID OR @DOWID IS NULL)
	AND (a.TimeID = @TimeID or @TimeID IS NULL)
	AND (Town = @Town or @Town IS NULL)	
	AND (District = @District or @District IS NULL)	
	ORDER BY a.[DOW],a.[TimeID], [Town]

END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spTableTowns]
AS
BEGIN
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TownId, Town FROM Towns ORDER BY Town
END
GO

CREATE PROCEDURE [dbo].[spTime]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TimeID, substring(xTime, 1, 5) + substring(xTime, 9,3) AS 'Time' FROM aTime ORDER BY TimeID;
END
GO

CREATE PROCEDURE [dbo].[spTowns]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT '-' AS 'Town' UNION SELECT DISTINCT [Town]  FROM List
END
GO

CREATE PROCEDURE [dbo].[spTownsAndDistricts]
(
	@TownId INT = NULL,	
	@District INT = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT TownId, District, Town FROM Towns 
	WHERE (TownId = @TownId or @TownId IS NULL)	
	AND (District = @District or @District IS NULL)
	ORDER BY Town
END
GO

CREATE PROCEDURE [dbo].[spUpdateList]
	@ListId INTEGER,
	@Suspend BIT = NULL,
	@DOWID INTEGER = NULL,
	@TimeID INTEGER = NULL,
	@Town VARCHAR(25) = NULL,
	@GroupName VARCHAR (60)= NULL,
	@Information VARCHAR (85) = NULL,
	@Location VARCHAR (50) = NULL,
	@Type	VARCHAR (30) = NULL,
	@District INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE List
		SET [DOW] = @DOWID,
		[TimeID] = @TimeID,
		[Town] = @Town,
		[GroupName] = @GroupName,
		[Information] = @Information,
		[Location] = @Location,
		[Type] = @Type,
		[suspend] = @Suspend,
		[district] = @District
	WHERE [ListID] = @ListId 
END
GO

CREATE PROCEDURE  [dbo].[spUpdateOnlineList]
	@DayId INT = NULL,
	@TimeId INT = NULL,
	@MeetingId VARCHAR (15),
	@Pswd CHAR (50) = NULL,
	@Telephone CHAR (10) = NULL,
	@GroupName VARCHAR (50),
	@Notes VARCHAR (50) = NULL,	
	@ZoomId INT,
	@District INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [dbo].[onlinemeetings]
		SET [dayid] = @DayId,
			[timeid] = @TimeId,
			[meetingid] = @MeetingId,
			[pswd] = @Pswd,
			[telephone] = @Telephone,
			[groupname] = @GroupName,
			[notes] = @Notes,
			[district] = @District
	WHERE [zoomid] = @ZoomId 
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bill Swanson
-- Create date: 11/21/2020
-- Description:	SP to update Towns
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateTowns] 
	@District INT = NULL,
	@Town VARCHAR(25),
	@TownId INT 
AS
BEGIN
	
	SET NOCOUNT ON;
	
	UPDATE Towns 
	SET  District = @District, Town = @Town
	WHERE TownId = @TownId

END
GO





















