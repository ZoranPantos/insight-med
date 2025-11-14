IF OBJECT_ID(N'dbo.LabParameters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LabParameters
    (
        Id INT NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        LabParameterReferenceJson NVARCHAR(MAX) NOT NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.LabParameters)
BEGIN
    INSERT INTO dbo.LabParameters (Id, Name, LabParameterReferenceJson)
    VALUES (1, N'test_name', N'{"MinThreshold":10.0,"MaxThreshold":100.0,"Positive":null}');
END