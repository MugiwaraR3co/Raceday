

USE master;
GO

IF DB_ID('RaceDay') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDay SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDay;
END;
GO

CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

/* =========================================================
   1. ROLES
   ========================================================= */
CREATE TABLE dbo.Roles
(
    RoleID INT IDENTITY(1,1) NOT NULL,
    RoleName NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Roles PRIMARY KEY (RoleID),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
);
GO

/* =========================================================
   2. USERS
   ========================================================= */
CREATE TABLE dbo.Users
(
    UserID INT IDENTITY(1,1) NOT NULL,
    RoleID INT NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Users PRIMARY KEY (UserID),

    CONSTRAINT UQ_Users_Email UNIQUE (Email),

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleID)
        REFERENCES dbo.Roles(RoleID)
);
GO

/* =========================================================
   3. EVENTS
   ========================================================= */
CREATE TABLE dbo.Events
(
    EventID INT IDENTITY(1,1) NOT NULL,
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    Status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Events_Status DEFAULT ('Open'),
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Events_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Events PRIMARY KEY (EventID),

    CONSTRAINT FK_Events_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES dbo.Users(UserID),

    CONSTRAINT CK_Events_Status
        CHECK (Status IN ('Open', 'Closed', 'Cancelled', 'Completed'))
);
GO

/* =========================================================
   4. CATEGORIES
   ========================================================= */
CREATE TABLE dbo.Categories
(
    CategoryID INT IDENTITY(1,1) NOT NULL,
    CreatedByUserID INT NOT NULL,
    CategoryName NVARCHAR(150) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,

    CONSTRAINT PK_Categories PRIMARY KEY (CategoryID),

    CONSTRAINT FK_Categories_CreatedByUser
        FOREIGN KEY (CreatedByUserID)
        REFERENCES dbo.Users(UserID),

    CONSTRAINT CK_Categories_Distance
        CHECK (DistanceKm > 0)
);
GO

/* =========================================================
   5. EVENT_CATEGORIES
   Resolves the many-to-many relationship between
   Events and Categories.
   ========================================================= */
CREATE TABLE dbo.EventCategories
(
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL,
    Capacity INT NOT NULL,

    CONSTRAINT PK_EventCategories
        PRIMARY KEY (EventID, CategoryID),

    CONSTRAINT FK_EventCategories_Events
        FOREIGN KEY (EventID)
        REFERENCES dbo.Events(EventID),

    CONSTRAINT FK_EventCategories_Categories
        FOREIGN KEY (CategoryID)
        REFERENCES dbo.Categories(CategoryID),

    CONSTRAINT CK_EventCategories_EntryFee
        CHECK (EntryFee >= 0),

    CONSTRAINT CK_EventCategories_Capacity
        CHECK (Capacity > 0)
);
GO

/* =========================================================
   6. ENROLMENTS
   ========================================================= */
CREATE TABLE dbo.Enrolments
(
    EnrolmentID INT IDENTITY(1,1) NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    ParticipantID INT NOT NULL,
    EnrolledAt DATETIME2 NOT NULL
        CONSTRAINT DF_Enrolments_EnrolledAt DEFAULT SYSDATETIME(),
    Status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Enrolments_Status DEFAULT ('Confirmed'),

    CONSTRAINT PK_Enrolments PRIMARY KEY (EnrolmentID),

    CONSTRAINT FK_Enrolments_EventCategory
        FOREIGN KEY (EventID, CategoryID)
        REFERENCES dbo.EventCategories(EventID, CategoryID),

    CONSTRAINT FK_Enrolments_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES dbo.Users(UserID),

    CONSTRAINT UQ_Enrolments_Participant_Event_Category
        UNIQUE (ParticipantID, EventID, CategoryID),

    CONSTRAINT CK_Enrolments_Status
        CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled'))
);
GO

/* =========================================================
   7. RESULTS
   ========================================================= */
CREATE TABLE dbo.Results
(
    ResultID INT IDENTITY(1,1) NOT NULL,
    EnrolmentID INT NOT NULL,
    FinishTime TIME(0) NULL,
    Position INT NULL,
    ResultStatus NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Results_Status DEFAULT ('Recorded'),
    RecordedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Results_RecordedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Results PRIMARY KEY (ResultID),

    CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (EnrolmentID)
        REFERENCES dbo.Enrolments(EnrolmentID),

    CONSTRAINT UQ_Results_Enrolment UNIQUE (EnrolmentID),

    CONSTRAINT CK_Results_Position
        CHECK (Position IS NULL OR Position > 0),

    CONSTRAINT CK_Results_Status
        CHECK (ResultStatus IN ('Recorded', 'Disqualified', 'DidNotFinish'))
);
GO

/* =========================================================
   SAMPLE DATA
   Required roles:
   2 Organisers
   2 Participants
   ========================================================= */

INSERT INTO dbo.Roles (RoleName)
VALUES
    ('Organiser'),
    ('Participant');
GO

INSERT INTO dbo.Users
    (RoleID, FirstName, LastName, Email, PasswordHash)
VALUES
    (1, 'Thabo', 'Mokoena', 'thabo.mokoena@raceday.local', 'HASHED_PASSWORD_1'),
    (1, 'Lerato', 'Naidoo', 'lerato.naidoo@raceday.local', 'HASHED_PASSWORD_2'),
    (2, 'Sipho', 'Dlamini', 'sipho.dlamini@raceday.local', 'HASHED_PASSWORD_3'),
    (2, 'Aisha', 'Mthembu', 'aisha.mthembu@raceday.local', 'HASHED_PASSWORD_4');
GO

/* =========================================================
   3 EVENTS
   ========================================================= */

INSERT INTO dbo.Events
    (OrganiserID, EventName, Description, EventDate, Location, Status)
VALUES
    (
        1,
        'Johannesburg Spring Run',
        'Road running event with multiple race distances.',
        '2026-10-10',
        'Johannesburg',
        'Open'
    ),
    (
        2,
        'Pretoria Heritage Race',
        'Community road race and walking event.',
        '2026-11-07',
        'Pretoria',
        'Open'
    ),
    (
        1,
        'Gauteng Cycle Challenge',
        'Cycling event for recreational and competitive riders.',
        '2026-12-05',
        'Gauteng',
        'Open'
    );
GO

/* =========================================================
   6 CATEGORIES
   ========================================================= */

INSERT INTO dbo.Categories
    (CreatedByUserID, CategoryName, DistanceKm)
VALUES
    (1, '5 km Fun Run', 5.00),
    (1, '10 km Road Race', 10.00),
    (2, '21.1 km Half Marathon', 21.10),
    (2, '42.2 km Marathon', 42.20),
    (1, '20 km Cycle', 20.00),
    (2, '50 km Cycle', 50.00);
GO

/* =========================================================
   EVENT/CATEGORY OFFERINGS
   ========================================================= */

INSERT INTO dbo.EventCategories
    (EventID, CategoryID, EntryFee, Capacity)
VALUES
    (1, 1, 100.00, 500),
    (1, 2, 150.00, 500),
    (1, 3, 250.00, 300),
    (2, 1, 80.00, 400),
    (2, 2, 130.00, 400),
    (2, 3, 220.00, 250),
    (3, 5, 180.00, 300),
    (3, 6, 300.00, 250);
GO

/* =========================================================
   ENROLMENTS
   ========================================================= */

INSERT INTO dbo.Enrolments
    (EventID, CategoryID, ParticipantID, Status)
VALUES
    (1, 1, 3, 'Confirmed'),
    (1, 2, 4, 'Confirmed'),
    (2, 1, 4, 'Confirmed'),
    (2, 3, 3, 'Confirmed'),
    (3, 5, 3, 'Confirmed');
GO

/* =========================================================
   RESULTS
   ========================================================= */

INSERT INTO dbo.Results
    (EnrolmentID, FinishTime, Position, ResultStatus)
VALUES
    (1, '00:27:41', 14, 'Recorded'),
    (2, '00:52:18', 21, 'Recorded'),
    (3, '00:25:59', 8, 'Recorded');
GO

/* =========================================================
   VERIFICATION
   ========================================================= */

SELECT 'Roles' AS TableName, COUNT(*) AS RecordCount
FROM dbo.Roles
UNION ALL
SELECT 'Users', COUNT(*) FROM dbo.Users
UNION ALL
SELECT 'Events', COUNT(*) FROM dbo.Events
UNION ALL
SELECT 'Categories', COUNT(*) FROM dbo.Categories
UNION ALL
SELECT 'EventCategories', COUNT(*) FROM dbo.EventCategories
UNION ALL
SELECT 'Enrolments', COUNT(*) FROM dbo.Enrolments
UNION ALL
SELECT 'Results', COUNT(*) FROM dbo.Results;
GO

/* Relationship test */
SELECT
    e.EventName,
    c.CategoryName,
    u.FirstName + ' ' + u.LastName AS Participant,
    en.Status AS EnrolmentStatus,
    r.FinishTime,
    r.Position,
    r.ResultStatus
FROM dbo.Enrolments AS en
INNER JOIN dbo.Events AS e
    ON e.EventID = en.EventID
INNER JOIN dbo.Categories AS c
    ON c.CategoryID = en.CategoryID
INNER JOIN dbo.Users AS u
    ON u.UserID = en.ParticipantID
LEFT JOIN dbo.Results AS r
    ON r.EnrolmentID = en.EnrolmentID
ORDER BY e.EventDate, c.DistanceKm;
GO
