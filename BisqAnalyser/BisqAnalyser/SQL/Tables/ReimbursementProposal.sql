IF OBJECT_ID ('dbo.reimbursementProposal') IS NULL 
	CREATE TABLE dbo.reimbursementProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		BsqAddress VARCHAR(100),
		RequestedBsq BIGINT
	)