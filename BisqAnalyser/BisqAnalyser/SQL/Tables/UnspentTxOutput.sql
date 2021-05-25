IF OBJECT_ID ('dbo.unspentTxOutput') IS NULL 
	CREATE TABLE dbo.unspentTxOutput (
		Id INT NOT NULL PRIMARY KEY,
		MapKey VARCHAR(300),
		Address VARCHAR(300),
		BlockHeight INT,
		OutputIndex INT,
		MessageCase VARCHAR(1000),
		OpReturnData VARBINARY(2000),
		PubKeyScriptAsm VARCHAR(1000),
		PubKeyScriptHex VARCHAR(1000),
		PubKeyScriptReqSigs INT,
		PubKeyScriptScriptType VARCHAR(1000),
		TxId VARCHAR(1000),
		TxOutputLockTime INT,
		TxOutputType VARCHAR(1000),
		TxOutputUnlockBlockHeight INT,
		Value BIGINT
	)