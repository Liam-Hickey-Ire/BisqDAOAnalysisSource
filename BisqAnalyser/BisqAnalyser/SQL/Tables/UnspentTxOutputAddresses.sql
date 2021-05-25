IF OBJECT_ID ('dbo.unspentTxOutputAddresses') IS NULL 
	CREATE TABLE dbo.unspentTxOutputAddresses (
		Id INT NOT NULL PRIMARY KEY,
		Address VARCHAR(300),
		UnspentTxOutputId INT,
	)