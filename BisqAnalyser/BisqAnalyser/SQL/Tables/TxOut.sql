IF OBJECT_ID ('dbo.txOut') IS NULL 
	CREATE TABLE dbo.txOut (
		Id BIGINT NOT NULL PRIMARY KEY,
		ScriptPubKey VARBINARY(200),
		Val BIGINT NOT NULL,
		BisqType TINYINT NOT NULL,
		TxId BIGINT FOREIGN KEY REFERENCES tx(Id) NOT NULL,
		TxInId BIGINT FOREIGN KEY REFERENCES txIn(Id)
	)