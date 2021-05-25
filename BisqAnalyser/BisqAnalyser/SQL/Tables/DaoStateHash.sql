IF OBJECT_ID ('dbo.daoStateHash') IS NULL 
	CREATE TABLE dbo.daoStateHash (
		Id BIGINT NOT NULL PRIMARY KEY,
		Height INT,
		Hash VARBINARY(2000),
		PrevHash VARBINARY(2000)
	)