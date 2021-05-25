IF OBJECT_ID ('dbo.tradeStatistics3Store') IS NULL 
	CREATE TABLE dbo.tradeStatistics3Store (
		Id BIGINT NOT NULL PRIMARY KEY,
		Currency VARCHAR(300) NOT NULL,
		Price BIGINT NOT NULL,
		Amount BIGINT NOT NULL,
		PaymentMethod VARCHAR(300) NOT NULL,
		Date BIGINT NOT NULL,
		Mediator VARCHAR(300) NOT NULL,
		RefundAgent VARCHAR(300) NOT NULL,
		Hash VARBINARY(1000) NOT NULL
		)

IF OBJECT_ID ('dbo.accountAgeWitnessStore') IS NULL 
	CREATE TABLE dbo.accountAgeWitnessStore (
		Id BIGINT NOT NULL PRIMARY KEY,
		Date BIGINT NOT NULL,
		Hash VARBINARY(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.peerList') IS NULL 
	CREATE TABLE dbo.peerList (
		Id BIGINT NOT NULL PRIMARY KEY,
		Date BIGINT NOT NULL,
		NodeAddressHostName VARCHAR(1000) NOT NULL,
		NodeAddressPort INT NOT NULL,
		SupportedCapabilities VARCHAR(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.sequenceNumberMap') IS NULL 
	CREATE TABLE dbo.sequenceNumberMap (
		Id BIGINT NOT NULL PRIMARY KEY,
		Bytes VARBINARY(1000) NOT NULL,
		MapValueSequenceNr INT NOT NULL,
		MapValueTimeStamp BIGINT NOT NULL
	)

IF OBJECT_ID ('dbo.signedWitnessStore') IS NULL 
	CREATE TABLE dbo.signedWitnessStore (
		Id BIGINT NOT NULL PRIMARY KEY,
		AccountAgeWitnessHash VARBINARY(1000) NOT NULL,
		Date BIGINT NOT NULL,
		Signature VARBINARY(1000) NOT NULL,
		SignerPubKey VARBINARY(1000) NOT NULL,
		TradeAmount BIGINT NOT NULL,
		VerificationMethod VARCHAR(200) NOT NULL,
		WitnessOwnerPubKey VARBINARY(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.navigationPath') IS NULL 
	CREATE TABLE dbo.navigationPath (
		Id BIGINT NOT NULL PRIMARY KEY,
		Path VARCHAR(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.addressEntryList') IS NULL 
	CREATE TABLE dbo.addressEntryList (
		Id BIGINT NOT NULL PRIMARY KEY,
		CoinLockedInMultiSig BIGINT NOT NULL,
		Context VARCHAR(1000) NOT NULL,
		OfferId VARCHAR(1000) NOT NULL,
		PubKey VARBINARY(1000) NOT NULL,
		PubKeyHash VARBINARY(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.cluster') IS NULL 
	CREATE TABLE dbo.cluster (
		Id BIGINT NOT NULL PRIMARY KEY,
		TotalSize BIGINT NOT NULL,
		BisqSize BIGINT NOT NULL
	)

IF OBJECT_ID ('dbo.address') IS NULL 
	CREATE TABLE dbo.address (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARBINARY(200) NOT NULL,
		Type TINYINT NOT NULL,
		ClusterId BIGINT FOREIGN KEY REFERENCES cluster(Id) NOT NULL
	)

IF OBJECT_ID ('dbo.unspentTxOutputAddresses') IS NULL 
	CREATE TABLE dbo.unspentTxOutputAddresses (
		Id INT NOT NULL PRIMARY KEY,
		Address VARCHAR(300),
		UnspentTxOutputId INT,
	)

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

IF OBJECT_ID ('dbo.spentInfo') IS NULL 
	CREATE TABLE dbo.spentInfo (
		Id INT NOT NULL PRIMARY KEY,
		MapKey VARCHAR(300),
		BlockHeight BIGINT,
		InputIndex INT,
		TxId VARCHAR(300)
	)

IF OBJECT_ID ('dbo.paramChangeList') IS NULL 
	CREATE TABLE dbo.paramChangeList (
		Id INT NOT NULL PRIMARY KEY,
		ActivationHeight INT,
		ParamName VARCHAR(300),
		ParamValue VARCHAR(300)
	)

IF OBJECT_ID ('dbo.issuance') IS NULL 
	CREATE TABLE dbo.issuance (
		Id INT NOT NULL PRIMARY KEY,
		MapKey VARCHAR(300) NOT NULL,
		Amount BIGINT NOT NULL,
		ChainHeight INT NOT NULL,
		IssuanceType VARCHAR(300) NOT NULL,
		PubKey VARCHAR(300) NOT NULL,
		TxId VARCHAR(300) NOT NULL
	)

IF OBJECT_ID ('dbo.confiscatedLockupTx') IS NULL 
	CREATE TABLE dbo.confiscatedLockupTx (
		Id INT NOT NULL PRIMARY KEY,
		TxId VARCHAR(300) NOT NULL,
	)

IF OBJECT_ID ('dbo.decryptedBallotList') IS NULL 
	CREATE TABLE dbo.decryptedBallotList (
		Id INT NOT NULL PRIMARY KEY,
		BlindVoteTxId VARCHAR(300),
		HashOfBlindVoteList VARBINARY(2000),
		Stake BIGINT,
		VoteRevealTxId VARCHAR(300)
	)

IF OBJECT_ID ('dbo.ballot') IS NULL 
	CREATE TABLE dbo.ballot (
		Id INT NOT NULL PRIMARY KEY,
		DecryptedBallotListId INT NOT NULL,
		ProposalTxId VARCHAR(300),
		VoteAccepted BIT
	)

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

IF OBJECT_ID ('dbo.cycle') IS NULL 
	CREATE TABLE dbo.cycle (
		Id INT NOT NULL PRIMARY KEY,
		CycleNumber VARCHAR(64) NOT NULL,
		Duration INT NOT NULL,
		PhaseOrdinal INT NOT NULL
	)

IF OBJECT_ID ('dbo.daoStateHash') IS NULL 
	CREATE TABLE dbo.daoStateHash (
		Id BIGINT NOT NULL PRIMARY KEY,
		Height INT,
		Hash VARBINARY(2000),
		PrevHash VARBINARY(2000)
	)

IF OBJECT_ID ('dbo.tradeStatistics2Store') IS NULL 
	CREATE TABLE dbo.tradeStatistics2Store (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARBINARY(2000),
		BaseCurrency VARCHAR(300),
		CounterCurrency VARCHAR(300),
		DepositTxId VARCHAR(300),
		Direction VARCHAR(300),
		OfferAmount BIGINT,
		OfferDate BIGINT,
		OfferId VARCHAR(300),
		OfferMarketPriceMargin FLOAT,
		OfferMinAmount BIGINT,
		OfferUseMarketBasedPrice BIT,
		PaymentMethodId VARCHAR(300),
		TradeAmount BIGINT,
		TradeDate BIGINT,
		TradePrice BIGINT
	)

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

IF OBJECT_ID ('dbo.compensationProposal') IS NULL 
	CREATE TABLE dbo.compensationProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		BsqAddress VARCHAR(100),
		RequestedBsq BIGINT
	)

IF OBJECT_ID ('dbo.reimbursementProposal') IS NULL 
	CREATE TABLE dbo.reimbursementProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		BsqAddress VARCHAR(100),
		RequestedBsq BIGINT
	)

IF OBJECT_ID ('dbo.changeParamProposal') IS NULL 
	CREATE TABLE dbo.changeParamProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		Param VARCHAR(100),
		ParamValue VARCHAR(100)
	)

IF OBJECT_ID ('dbo.roleProposal') IS NULL 
	CREATE TABLE dbo.roleProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		RoleUid VARCHAR(200),
		RoleName VARCHAR(200),
		Link VARCHAR(200),
		BondedRoleType VARCHAR(200),
		RequiredBondUnit BIGINT
	)

IF OBJECT_ID ('dbo.blindVote') IS NULL 
	CREATE TABLE dbo.blindVote (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARBINARY(2000),
		Date BIGINT NOT NULL,
		EncryptedVotes VARBINARY(MAX),
		EncryptedMeritList VARBINARY(MAX),
		Stake BIGINT NOT NULL,
		TxId VARCHAR(1000) NOT NULL
	)

IF OBJECT_ID ('dbo.block') IS NULL 
	CREATE TABLE dbo.block (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARCHAR(64) NOT NULL,
		Timestamp BIGINT NOT NULL,
		PriceOpen BIGINT NOT NULL,
		PriceHigh BIGINT NOT NULL,
		PriceLow BIGINT NOT NULL,
		PriceClose BIGINT NOT NULL,
		PriceTimestamp BIGINT NOT NULL
	)

IF OBJECT_ID ('dbo.tx') IS NULL 
	CREATE TABLE dbo.tx (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARCHAR(64) NOT NULL,
		Version TINYINT NOT NULL,
		LockTime BIGINT NOT NULL,
		BisqType TINYINT NOT NULL,
		BisqProposalAccepted TINYINT,
		BlockId BIGINT FOREIGN KEY REFERENCES block(Id) NOT NULL
	)

IF OBJECT_ID ('dbo.txIn') IS NULL 
	CREATE TABLE dbo.txIn (
		Id BIGINT NOT NULL PRIMARY KEY,
		ScriptSig VARBINARY(200),
		SeqNum BIGINT NOT NULL,
		TxId BIGINT FOREIGN KEY REFERENCES tx(Id) NOT NULL,
		TxOutId BIGINT
	)

IF OBJECT_ID ('dbo.txOut') IS NULL 
	CREATE TABLE dbo.txOut (
		Id BIGINT NOT NULL PRIMARY KEY,
		ScriptPubKey VARBINARY(200),
		Val BIGINT NOT NULL,
		BisqType TINYINT NOT NULL,
		TxId BIGINT FOREIGN KEY REFERENCES tx(Id) NOT NULL,
		TxInId BIGINT
	)

IF OBJECT_ID ('FK_BlockId') IS NULL
	ALTER TABLE dbo.tx
	ADD CONSTRAINT FK_BlockId 
	FOREIGN KEY (BlockId) REFERENCES block(Id)

IF OBJECT_ID ('FK_TxOutId') IS NULL
	ALTER TABLE dbo.txIn
	ADD CONSTRAINT FK_TxOutId 
	FOREIGN KEY (txOutId) REFERENCES txOut(Id)

IF OBJECT_ID ('FK_TxInId') IS NULL
	ALTER TABLE dbo.txOut 
	ADD CONSTRAINT FK_TxInId 
	FOREIGN KEY (txInId) REFERENCES txIn(Id)

IF OBJECT_ID ('FK_ClusterId') IS NULL
	ALTER TABLE dbo.address 
	ADD CONSTRAINT FK_ClusterId 
	FOREIGN KEY (ClusterId) REFERENCES cluster(Id)
