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
    VALUES
    (1, N'Hemoglobin', N'{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null,"Unit":"g/dL"}'),
    (2, N'White Blood Cell Count', N'{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null,"Unit":"10^3/µL"}'),
    (3, N'Glucose', N'{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null,"Unit":"mg/dL"}'),
    (4, N'Cholesterol', N'{"MinThreshold":125.0,"MaxThreshold":200.0,"Positive":null,"Unit":"mg/dL"}'),
    (5, N'Creatinine', N'{"MinThreshold":0.7,"MaxThreshold":1.3,"Positive":null,"Unit":"mg/dL"}'),
    (6, N'Urine pH', N'{"MinThreshold":4.5,"MaxThreshold":8.0,"Positive":null,"Unit":"pH"}'),
    (7, N'Bilirubin', N'{"MinThreshold":0.1,"MaxThreshold":1.2,"Positive":null,"Unit":"mg/dL"}'),
    (8, N'Platelet Count', N'{"MinThreshold":150.0,"MaxThreshold":400.0,"Positive":null,"Unit":"10^3/µL"}'),
    (9, N'Thyroid Stimulating Hormone (TSH)', N'{"MinThreshold":0.4,"MaxThreshold":4.0,"Positive":null,"Unit":"µIU/mL"}'),
    (10, N'Vitamin D', N'{"MinThreshold":30.0,"MaxThreshold":100.0,"Positive":null,"Unit":"ng/mL"}'),
    (11, N'Hepatitis B Surface Antigen', N'{"MinThreshold":null,"MaxThreshold":null,"Positive":false,"Unit":null}'),
    (12, N'HIV I/II Antibody', N'{"MinThreshold":null,"MaxThreshold":null,"Positive":false,"Unit":null}'),
    (13, N'Urine Ketones', N'{"MinThreshold":null,"MaxThreshold":null,"Positive":false,"Unit":null}'),
    (14, N'Fecal Occult Blood', N'{"MinThreshold":null,"MaxThreshold":null,"Positive":false,"Unit":null}'),
    (15, N'COVID-19 Antigen', N'{"MinThreshold":null,"MaxThreshold":null,"Positive":false,"Unit":null}');
END