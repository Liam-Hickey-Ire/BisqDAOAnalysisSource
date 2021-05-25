IF OBJECT_ID ('dbo.decryptedBallotList') IS NULL 
	CREATE TABLE dbo.decryptedBallotList (
		Id INT NOT NULL PRIMARY KEY,
		BlindVoteTxId VARCHAR(300),
		HashOfBlindVoteList VARBINARY(2000),
		Stake BIGINT,
		VoteRevealTxId VARCHAR(300)
	)