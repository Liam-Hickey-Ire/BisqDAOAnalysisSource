IF OBJECT_ID ('dbo.changeParamProposal') IS NULL 
	CREATE TABLE dbo.changeParamProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		Param VARCHAR(100),
		ParamValue VARCHAR(100)
	)