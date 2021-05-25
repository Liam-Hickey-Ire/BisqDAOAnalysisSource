IF OBJECT_ID ('dbo.compensationProposal') IS NULL 
	CREATE TABLE dbo.compensationProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		BsqAddress VARCHAR(100),
		RequestedBsq BIGINT
	)