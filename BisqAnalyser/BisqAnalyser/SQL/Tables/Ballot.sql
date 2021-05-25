IF OBJECT_ID ('dbo.ballot') IS NULL 
	CREATE TABLE dbo.ballot (
		Id INT NOT NULL PRIMARY KEY,
		DecryptedBallotListId INT NOT NULL,
		ProposalTxId VARCHAR(300),
		VoteAccepted BIT
	)