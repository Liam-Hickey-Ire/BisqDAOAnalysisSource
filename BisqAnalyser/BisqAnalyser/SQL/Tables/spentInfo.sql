IF OBJECT_ID ('dbo.spentInfo') IS NULL 
	CREATE TABLE dbo.spentInfo (
		Id INT NOT NULL PRIMARY KEY,
		MapKey VARCHAR(300),
		BlockHeight BIGINT,
		InputIndex INT,
		TxId VARCHAR(300)
	)