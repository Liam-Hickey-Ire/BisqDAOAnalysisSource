IF OBJECT_ID ('dbo.confiscatedLockupTx') IS NULL 
	CREATE TABLE dbo.confiscatedLockupTx (
		Id INT NOT NULL PRIMARY KEY,
		TxId VARCHAR(300) NOT NULL,
	)