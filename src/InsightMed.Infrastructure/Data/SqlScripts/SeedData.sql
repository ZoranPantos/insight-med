SET NOCOUNT ON;

ALTER TABLE [AspNetUsers] NOCHECK CONSTRAINT ALL;
ALTER TABLE [LabRequests] NOCHECK CONSTRAINT ALL;
ALTER TABLE [LabReports] NOCHECK CONSTRAINT ALL;
ALTER TABLE [Notifications] NOCHECK CONSTRAINT ALL;

INSERT INTO [dbo].[AspNetUsers]
(
    [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
    [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], 
    [LockoutEnd], [LockoutEnabled], [AccessFailedCount]
)
VALUES
(
    N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa',
    N'default',
    N'DEFAULT',
    N'default@test.com',
    N'DEFAULT@TEST.COM',
    1,
    N'AQAAAAIAAYagAAAAEPcKVF7pt3eA9QAb1gQByK444/GC7JS2DlpnYz88IUhte1lwsZz/xHbn8Wg7ViC+bA==',
    N'KJ2J76RSCOFHROXOYUTETXOZZF63VZO7',
    N'9584048b-3405-4a4d-805d-e590921a9b79',
    NULL, 0, 0, NULL, 1, 0
);

SET IDENTITY_INSERT LabParameters ON;
INSERT INTO [LabParameters] ([Id], [Name])
VALUES
    (1, 'Hemoglobin'),
    (2, 'White Blood Cell Count'),
    (3, 'Glucose'),
    (4, 'Cholesterol'),
    (5, 'Creatinine'),
    (6, 'Urine pH'),
    (7, 'Bilirubin'),
    (8, 'Platelet Count'),
    (9, 'Thyroid Stimulating Hormone (TSH)'),
    (10, 'Vitamin D'),
    (11, N'Hepatitis B Surface Antigen'),
    (12, N'HIV I/II Antibody'),
    (13, N'Urine Ketones'),
    (14, N'Fecal Occult Blood'),
    (15, N'COVID-19 Antigen');

SET IDENTITY_INSERT LabParameters OFF;

INSERT INTO [Patients] ([Uid], [FirstName], [LastName], [DateOfBirth], [Gender], [BloodGroup], [Email], [Phone])
VALUES
    ('UID-1001', 'John', 'Doe', '1985-06-15', 0, 0, 'john.doe@example.com', '+38766123123'),
    ('UID-1002', 'Jane', 'Smith', '1990-02-20', 1, 2, 'jane.smith@example.com', '+38766123123'),
    ('UID-1003', 'Michael', 'Johnson', '1975-11-30', 0, 4, 'michael.johanson@example.com', '+38766123123'),
    ('UID-1004', 'Emily', 'Davis', '2000-04-10', 1, 6, 'emily.davis@example.com', '+38766123123'),
    ('UID-1005', 'David', 'Wilson', '1960-08-25', 0, 1, 'david.wilson@example.com', '+38766123123'),
    ('UID-1006', 'Sarah', 'Martinez', '1988-12-05', 1, 3, 'sarah.martinez@example.com', '+38766123123'),
    ('UID-1007', 'Robert', 'Anderson', '1995-07-18', 0, 5, 'anderson.robert@example.com', '+38766123123'),
    ('UID-1008', 'Linda', 'Taylor', '1972-03-12', 1, 7, 'taylor.linda@example.com', '+38766123123'),
    ('UID-1009', 'James', 'Thomas', '1982-09-22', 0, 0, 'james.thomas@example.com', '+38766123123'),
    ('UID-1010', 'Patricia', 'Hernandez', '1993-01-08', 1, 2, 'patricia.hernandez@example.com', '+38766123123'),
    ('UID-1011', 'Alice', 'Brown', '1991-05-12', 1, 0, 'alice.brown@example.com', '+38766123123'),
    ('UID-1012', 'Tom', 'Clark', '1983-08-24', 0, 1, 'tom.clark@example.com', '+38766123123'),
    ('UID-1013', 'Sophia', 'Lewis', '1998-11-03', 1, 2, 'sophia.lewis@example.com', '+38766123123'),
    ('UID-1014', 'Daniel', 'Walker', '1979-01-19', 0, 3, 'daniel.walker@example.com', '+38766123123'),
    ('UID-1015', 'Olivia', 'Hall', '2001-07-29', 1, 4, 'olivia.hall@example.com', '+38766123123'),
    ('UID-1016', 'Matthew', 'Allen', '1965-03-14', 0, 5, 'matthew.allen@example.com', '+38766123123'),
    ('UID-1017', 'Emma', 'Young', '1993-09-08', 1, 6, 'emma.young@example.com', '+38766123123'),
    ('UID-1018', 'Christopher', 'King', '1987-12-01', 0, 7, 'chris.king@example.com', '+38766123123'),
    ('UID-1019', 'Ava', 'Wright', '1996-02-15', 1, 0, 'ava.wright@example.com', '+38766123123'),
    ('UID-1020', 'Andrew', 'Scott', '1970-04-22', 0, 1, 'andrew.scott@example.com', '+38766123123'),
    ('UID-1021', 'Isabella', 'Torres', '1992-06-30', 1, 2, 'isabella.torres@example.com', '+38766123123'),
    ('UID-1022', 'Joshua', 'Nguyen', '1989-10-11', 0, 3, 'joshua.nguyen@example.com', '+38766123123'),
    ('UID-1023', 'Mia', 'Hill', '1999-01-05', 1, 4, 'mia.hill@example.com', '+38766123123'),
    ('UID-1024', 'Ryan', 'Flores', '1981-08-17', 0, 5, 'ryan.flores@example.com', '+38766123123'),
    ('UID-1025', 'Charlotte', 'Green', '1994-03-25', 1, 6, 'charlotte.green@example.com', '+38766123123'),
    ('UID-1026', 'Nathan', 'Adams', '1976-11-18', 0, 7, 'nathan.adams@example.com', '+38766123123'),
    ('UID-1027', 'Amelia', 'Baker', '1997-09-09', 1, 0, 'amelia.baker@example.com', '+38766123123'),
    ('UID-1028', 'Samuel', 'Gonzalez', '1984-05-14', 0, 2, 'samuel.gonzalez@example.com', '+38766123123'),
    ('UID-1029', 'Harper', 'Nelson', '2002-02-28', 1, 4, 'harper.nelson@example.com', '+38766123123'),
    ('UID-1030', 'Benjamin', 'Carter', '1968-12-07', 0, 6, 'benjamin.carter@example.com', '+38766123123');

INSERT INTO [LabRequests] ([Created], [LabRequestState], [PatientId], [LabParameterIds])
VALUES
    ('2023-01-10 09:00:00', 1, 1, '[1,2,3,15]'), 
    ('2023-02-15 10:30:00', 0, 2, '[4,5,11]'), 
    ('2023-03-20 11:45:00', 1, 3, '[2,3,6]'), 
    ('2023-04-25 14:00:00', 0, 4, '[1,7]'), 
    ('2023-05-30 15:15:00', 1, 5, '[3,8,9,13]'), 
    ('2023-06-05 08:45:00', 0, 6, '[2,5,14]'), 
    ('2023-07-10 12:00:00', 1, 7, '[4,6,7]'), 
    ('2023-08-15 13:30:00', 0, 8, '[1,3,5]'), 
    ('2023-09-20 16:00:00', 1, 9, '[8,9,12]'), 
    ('2023-10-25 17:45:00', 0, 10, '[2,4,6]'),
    ('2023-11-05 08:30:00', 1, 11, '[1,2,3,4,5,15]'), 
    ('2023-11-12 09:15:00', 0, 12, '[5,6,7,11,13]'), 
    ('2023-11-20 14:00:00', 1, 13, '[1,2,8,9,10,12]'),
    ('2023-11-25 10:45:00', 0, 14, '[1,2,3,4,5,6,7]'),
    ('2023-12-01 11:30:00', 1, 15, '[3,4,13,14,15]'),
    ('2023-12-08 09:00:00', 0, 16, '[1,4,5,9,10,14]'),
    ('2023-12-15 15:20:00', 1, 17, '[1,2,11,12,15]'),
    ('2023-12-20 08:15:00', 0, 18, '[1,3,5,6,10]'),
    ('2024-01-05 10:00:00', 1, 19, '[1,2,3,4,5,6,7]'),
    ('2024-01-12 13:45:00', 0, 20, '[8,9,10,13,14]');

INSERT INTO [LabReports] ([Content], [Created], [LabRequestId], [PatientId])
VALUES
    (N'[
        {"Id":1,"Name":"Hemoglobin","IsPositive":null,"Measurement":14.5,"Reference":{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null}},
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":7.2,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":88.0,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":15,"Name":"COVID-19 Antigen","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-01-12 14:00:00', 1, 1),
    (N'[
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":4.0,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":92.0,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":6,"Name":"Urine pH","IsPositive":null,"Measurement":6.0,"Reference":{"MinThreshold":4.5,"MaxThreshold":8.0,"Positive":null}}
    ]', '2023-03-22 16:45:00', 3, 3),
    (N'[
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":100.0,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":8,"Name":"Platelet Count","IsPositive":null,"Measurement":250.0,"Reference":{"MinThreshold":150.0,"MaxThreshold":400.0,"Positive":null}},
        {"Id":9,"Name":"Thyroid Stimulating Hormone (TSH)","IsPositive":null,"Measurement":2.1,"Reference":{"MinThreshold":0.4,"MaxThreshold":4.0,"Positive":null}},
        {"Id":13,"Name":"Urine Ketones","IsPositive":true,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-05-31 10:15:00', 5, 5),
    (N'[
        {"Id":4,"Name":"Cholesterol","IsPositive":null,"Measurement":190.0,"Reference":{"MinThreshold":125.0,"MaxThreshold":200.0,"Positive":null}},
        {"Id":6,"Name":"Urine pH","IsPositive":null,"Measurement":6.5,"Reference":{"MinThreshold":4.5,"MaxThreshold":8.0,"Positive":null}},
        {"Id":7,"Name":"Bilirubin","IsPositive":null,"Measurement":0.9,"Reference":{"MinThreshold":0.1,"MaxThreshold":1.2,"Positive":null}}
    ]', '2023-07-12 12:45:00', 7, 7),
    (N'[
        {"Id":8,"Name":"Platelet Count","IsPositive":null,"Measurement":210.0,"Reference":{"MinThreshold":150.0,"MaxThreshold":400.0,"Positive":null}},
        {"Id":9,"Name":"Thyroid Stimulating Hormone (TSH)","IsPositive":null,"Measurement":3.5,"Reference":{"MinThreshold":0.4,"MaxThreshold":4.0,"Positive":null}},
        {"Id":12,"Name":"HIV I/II Antibody","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-09-22 15:15:00', 9, 9),
    (N'[
        {"Id":1,"Name":"Hemoglobin","IsPositive":null,"Measurement":18.2,"Reference":{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null}},
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":5.5,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":64.4,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":4,"Name":"Cholesterol","IsPositive":null,"Measurement":160.9,"Reference":{"MinThreshold":125.0,"MaxThreshold":200.0,"Positive":null}},
        {"Id":5,"Name":"Creatinine","IsPositive":null,"Measurement":0.9,"Reference":{"MinThreshold":0.7,"MaxThreshold":1.3,"Positive":null}},
        {"Id":15,"Name":"COVID-19 Antigen","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-11-06 14:00:00', 11, 11),
    (N'[
        {"Id":1,"Name":"Hemoglobin","IsPositive":null,"Measurement":14.0,"Reference":{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null}},
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":8.0,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":8,"Name":"Platelet Count","IsPositive":null,"Measurement":139.6,"Reference":{"MinThreshold":150.0,"MaxThreshold":400.0,"Positive":null}},
        {"Id":9,"Name":"Thyroid Stimulating Hormone (TSH)","IsPositive":null,"Measurement":5.2,"Reference":{"MinThreshold":0.4,"MaxThreshold":4.0,"Positive":null}},
        {"Id":10,"Name":"Vitamin D","IsPositive":null,"Measurement":35.0,"Reference":{"MinThreshold":30.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":12,"Name":"HIV I/II Antibody","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-11-21 09:30:00', 13, 13),
    (N'[
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":110.0,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":4,"Name":"Cholesterol","IsPositive":null,"Measurement":240.0,"Reference":{"MinThreshold":125.0,"MaxThreshold":200.0,"Positive":null}},
        {"Id":13,"Name":"Urine Ketones","IsPositive":true,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}},
        {"Id":14,"Name":"Fecal Occult Blood","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}},
        {"Id":15,"Name":"COVID-19 Antigen","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-12-02 10:00:00', 15, 15),
    (N'[
        {"Id":1,"Name":"Hemoglobin","IsPositive":null,"Measurement":12.5,"Reference":{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null}},
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":9.5,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":11,"Name":"Hepatitis B Surface Antigen","IsPositive":true,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}},
        {"Id":12,"Name":"HIV I/II Antibody","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}},
        {"Id":15,"Name":"COVID-19 Antigen","IsPositive":false,"Measurement":null,"Reference":{"MinThreshold":null,"MaxThreshold":null,"Positive":false}}
    ]', '2023-12-16 11:00:00', 17, 17),
    (N'[
        {"Id":1,"Name":"Hemoglobin","IsPositive":null,"Measurement":15.0,"Reference":{"MinThreshold":12.0,"MaxThreshold":17.5,"Positive":null}},
        {"Id":2,"Name":"White Blood Cell Count","IsPositive":null,"Measurement":6.0,"Reference":{"MinThreshold":4.5,"MaxThreshold":11.0,"Positive":null}},
        {"Id":3,"Name":"Glucose","IsPositive":null,"Measurement":82.0,"Reference":{"MinThreshold":70.0,"MaxThreshold":100.0,"Positive":null}},
        {"Id":4,"Name":"Cholesterol","IsPositive":null,"Measurement":180.0,"Reference":{"MinThreshold":125.0,"MaxThreshold":200.0,"Positive":null}},
        {"Id":5,"Name":"Creatinine","IsPositive":null,"Measurement":1.1,"Reference":{"MinThreshold":0.7,"MaxThreshold":1.3,"Positive":null}},
        {"Id":6,"Name":"Urine pH","IsPositive":null,"Measurement":7.0,"Reference":{"MinThreshold":4.5,"MaxThreshold":8.0,"Positive":null}},
        {"Id":7,"Name":"Bilirubin","IsPositive":null,"Measurement":0.5,"Reference":{"MinThreshold":0.1,"MaxThreshold":1.2,"Positive":null}}
    ]', '2024-01-06 09:15:00', 19, 19);

INSERT INTO [Notifications] ([Message], [LabReportId], [Seen], [RequesterId])
VALUES
    ('Report for patient John Doe UID-1001 is available. Date created UTC: 12/01/2023 14:00:00', 1, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Michael Johnson UID-1003 is available. Date created UTC: 22/03/2023 16:45:00', 2, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient David Wilson UID-1005 is available. Date created UTC: 31/05/2023 10:15:00', 3, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Robert Anderson UID-1007 is available. Date created UTC: 12/07/2023 12:45:00', 4, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient James Thomas UID-1009 is available. Date created UTC: 22/09/2023 15:15:00', 5, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Alice Brown UID-1011 is available. Date created UTC: 06/11/2023 14:00:00', 6, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Sophia Lewis UID-1013 is available. Date created UTC: 21/11/2023 09:30:00', 7, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Olivia Hall UID-1015 is available. Date created UTC: 02/12/2023 10:00:00', 8, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Emma Young UID-1017 is available. Date created UTC: 16/12/2023 11:00:00', 9, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa'),
    ('Report for patient Ava Wright UID-1019 is available. Date created UTC: 06/01/2024 09:15:00', 10, 0, N'a72ec20d-96b2-4fd1-852a-d43e4548c2fa');

ALTER TABLE [AspNetUsers] CHECK CONSTRAINT ALL;
ALTER TABLE [LabRequests] CHECK CONSTRAINT ALL;
ALTER TABLE [LabReports] CHECK CONSTRAINT ALL;
ALTER TABLE [Notifications] CHECK CONSTRAINT ALL;