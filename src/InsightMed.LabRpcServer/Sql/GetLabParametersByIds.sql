SELECT Id, Name, LabParameterReferenceJson
FROM dbo.LabParameters
WHERE Id IN ({ID_LIST})
ORDER BY Id;