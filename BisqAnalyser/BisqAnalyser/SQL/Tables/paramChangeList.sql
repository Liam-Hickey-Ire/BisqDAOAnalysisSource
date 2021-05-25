IF OBJECT_ID ('dbo.paramChangeList') IS NULL 
	CREATE TABLE dbo.paramChangeList (
		Id INT NOT NULL PRIMARY KEY,
		ActivationHeight INT,
		ParamName VARCHAR(300),
		ParamValue VARCHAR(300)
	)