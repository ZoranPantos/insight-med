SET NOCOUNT ON;
ALTER TABLE [LabRequests] NOCHECK CONSTRAINT ALL;
ALTER TABLE [LabReports] NOCHECK CONSTRAINT ALL;
ALTER TABLE [Notifications] NOCHECK CONSTRAINT ALL;

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
    (10, 'Vitamin D');
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
    ('UID-1010', 'Patricia', 'Hernandez', '1993-01-08', 1, 2, 'patricia.hernandez@example.com', '+38766123123');

INSERT INTO [LabRequests] ([Created], [LabRequestState], [PatientId], [LabParameterIds])
VALUES
    ('2023-01-10 09:00:00', 1, 1, '[1,2,3]'),
    ('2023-02-15 10:30:00', 0, 2, '[4,5]'),
    ('2023-03-20 11:45:00', 1, 3, '[2,3,6]'),
    ('2023-04-25 14:00:00', 0, 4, '[1,7]'),
    ('2023-05-30 15:15:00', 1, 5, '[3,8,9]'),
    ('2023-06-05 08:45:00', 0, 6, '[2,5]'),
    ('2023-07-10 12:00:00', 1, 7, '[4,6,7]'),
    ('2023-08-15 13:30:00', 0, 8, '[1,3,5]'),
    ('2023-09-20 16:00:00', 1, 9, '[8,9]'),
    ('2023-10-25 17:45:00', 0, 10, '[2,4,6]');

INSERT INTO [LabReports] ([Content], [Created], [LabRequestId], [PatientId])
VALUES
    ('Hemoglobin: 14.5 g/dL (normal)', '2023-01-12 14:00:00', 1, 1),
    ('Glucose: 95 mg/dL (normal)', '2023-02-17 15:30:00', NULL, 2),
    ('Cholesterol: 180 mg/dL (normal)', '2023-03-22 16:45:00', 3, 3),
    ('Creatinine: 1.0 mg/dL (normal)', '2023-04-27 09:00:00', 4, 4),
    ('Urine pH: 6.5 (normal)', '2023-05-31 10:15:00', NULL, 5),
    ('TSH: 2.5 mIU/L (normal)', '2023-06-07 11:30:00', 6, 6),
    ('Vitamin D: 30 ng/mL (sufficient)', '2023-07-12 12:45:00', 7, 7),
    ('Bilirubin: 0.8 mg/dL (normal)', '2023-08-17 14:00:00', NULL, 8),
    ('Platelet Count: 250,000 /µL (normal)', '2023-09-22 15:15:00', 9, 9),
    ('White Blood Cell Count: 7,000 /µL (normal)', '2023-10-27 16:30:00', 10, 10);

INSERT INTO [Notifications] ([Message], [LabReportId], [Seen])
VALUES
    ('Lab report ready for review: Hemoglobin results available.', 1, 0),
    ('Notification: Glucose test completed.', 2, 0),
    ('Alert: Cholesterol levels updated.', 3, 0),
    ('New results: Creatinine test processed.', 4, 0),
    ('Urine pH report is now available.', 5, 0),
    ('TSH results ready - please check.', 6, 0),
    ('Vitamin D deficiency check completed.', 7, 0),
    ('Bilirubin test results uploaded.', 8, 0),
    ('Platelet count report available.', 9, 0),
    ('White blood cell count results ready for review.', 10, 0);

ALTER TABLE [LabRequests] CHECK CONSTRAINT ALL;
ALTER TABLE [LabReports] CHECK CONSTRAINT ALL;
ALTER TABLE [Notifications] CHECK CONSTRAINT ALL;