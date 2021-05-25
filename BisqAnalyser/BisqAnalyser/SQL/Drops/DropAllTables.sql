IF OBJECT_ID ('FK_BlockId') IS NOT NULL 
	ALTER TABLE dbo.tx
	DROP CONSTRAINT FK_BlockId

IF OBJECT_ID ('FK_ClusterId') IS NOT NULL 
	ALTER TABLE dbo.address
	DROP CONSTRAINT FK_ClusterId

IF OBJECT_ID ('FK_TxOutId') IS NOT NULL 
	ALTER TABLE dbo.TxIn
	DROP CONSTRAINT FK_TxOutId

IF OBJECT_ID ('FK_TxInId') IS NOT NULL 
	ALTER TABLE dbo.txOut
	DROP CONSTRAINT FK_TxInId

IF OBJECT_ID ('dbo.tradeStatistics3Store') IS NOT NULL
	DROP TABLE dbo.tradeStatistics3Store

IF OBJECT_ID ('dbo.accountAgeWitnessStore') IS NOT NULL
	DROP TABLE dbo.accountAgeWitnessStore

IF OBJECT_ID ('dbo.peerList') IS NOT NULL
	DROP TABLE dbo.peerList

IF OBJECT_ID ('dbo.sequenceNumberMap') IS NOT NULL
	DROP TABLE dbo.sequenceNumberMap

IF OBJECT_ID ('dbo.signedWitnessStore') IS NOT NULL
	DROP TABLE dbo.signedWitnessStore

IF OBJECT_ID ('dbo.navigationPath') IS NOT NULL
	DROP TABLE dbo.navigationPath

IF OBJECT_ID ('dbo.addressEntryList') IS NOT NULL
	DROP TABLE dbo.addressEntryList

IF OBJECT_ID ('dbo.address') IS NOT NULL
	DROP TABLE dbo.address

IF OBJECT_ID ('dbo.cluster') IS NOT NULL
	DROP TABLE dbo.cluster
	
IF OBJECT_ID ('dbo.unspentTxOutputAddresses') IS NOT NULL
	DROP TABLE dbo.unspentTxOutputAddresses

IF OBJECT_ID ('dbo.unspentTxOutput') IS NOT NULL
	DROP TABLE dbo.unspentTxOutput

IF OBJECT_ID ('dbo.issuance') IS NOT NULL
	DROP TABLE dbo.issuance

IF OBJECT_ID ('dbo.spentInfo') IS NOT NULL
	DROP TABLE dbo.spentInfo

IF OBJECT_ID ('dbo.paramChangeList') IS NOT NULL
	DROP TABLE dbo.paramChangeList

IF OBJECT_ID ('dbo.issuanceMap') IS NOT NULL
	DROP TABLE dbo.issuanceMap

IF OBJECT_ID ('dbo.confiscatedLockupTx') IS NOT NULL
	DROP TABLE dbo.confiscatedLockupTx

IF OBJECT_ID ('dbo.decryptedBallotList') IS NOT NULL
	DROP TABLE dbo.decryptedBallotList

IF OBJECT_ID ('dbo.ballot') IS NOT NULL
	DROP TABLE dbo.ballot

IF OBJECT_ID ('dbo.evaluatedProposal') IS NOT NULL
	DROP TABLE dbo.evaluatedProposal

IF OBJECT_ID ('dbo.cycle') IS NOT NULL
	DROP TABLE dbo.cycle

IF OBJECT_ID ('dbo.daoStateHash') IS NOT NULL
	DROP TABLE dbo.daoStateHash

IF OBJECT_ID ('dbo.tradeStatistics2Store') IS NOT NULL
	DROP TABLE dbo.tradeStatistics2Store

IF OBJECT_ID ('dbo.blindVote') IS NOT NULL
	DROP TABLE dbo.blindVote

IF OBJECT_ID ('dbo.proposal') IS NOT NULL
	DROP TABLE dbo.proposal

IF OBJECT_ID ('dbo.compensationProposal') IS NOT NULL
	DROP TABLE dbo.compensationProposal

IF OBJECT_ID ('dbo.reimbursementProposal') IS NOT NULL
	DROP TABLE dbo.reimbursementProposal

IF OBJECT_ID ('dbo.changeParamProposal') IS NOT NULL
	DROP TABLE dbo.changeParamProposal

IF OBJECT_ID ('dbo.roleProposal') IS NOT NULL
	DROP TABLE dbo.roleProposal

IF OBJECT_ID ('dbo.txIn') IS NOT NULL 
	DROP TABLE dbo.txIn

IF OBJECT_ID ('dbo.txOut') IS NOT NULL 
	DROP TABLE dbo.txOut

IF OBJECT_ID ('dbo.tx') IS NOT NULL 
	DROP TABLE dbo.tx

IF OBJECT_ID ('dbo.block') IS NOT NULL 
	DROP TABLE dbo.block