IF OBJECT_ID ('dbo.evaluatedProposal') IS NULL 
	CREATE TABLE dbo.evaluatedProposal (
		Id INT NOT NULL PRIMARY KEY,
		IsAccepted BIT NOT NULL,
		NumAcceptedVotes INT NOT NULL,
		NumIgnoredVotes INT NOT NULL,
		NumRejectedVotes INT NOT NULL,
		StakeOfAcceptedVotes BIGINT NOT NULL,
		StakeOfRejectedVotes BIGINT NOT NULL,
		ProposalTxId VARCHAR(300) NOT NULL,
	)