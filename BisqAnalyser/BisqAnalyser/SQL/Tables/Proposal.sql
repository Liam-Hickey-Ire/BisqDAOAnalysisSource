IF OBJECT_ID ('dbo.proposal') IS NULL 
	CREATE TABLE dbo.proposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARBINARY(2000),
		Name VARCHAR(100),
		Link VARCHAR(300),
		Version TINYINT,
		TxId VARCHAR(1000) NOT NULL,
		CreationDate BIGINT,
		CompensationProposalId BIGINT,
		ReimbursementProposalId BIGINT,
		ChangeParamProposalId BIGINT,
		RoleProposalId BIGINT
	)