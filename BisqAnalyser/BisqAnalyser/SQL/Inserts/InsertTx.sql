INSERT INTO dbo.tx (Id, Hash, Version, LockTime, BisqType, BisqProposalAccepted, BlockId) 
VALUES
(@id, @hash, @version, @locktime, @bisqtype, @bisqproposalaccepted, @blockid)