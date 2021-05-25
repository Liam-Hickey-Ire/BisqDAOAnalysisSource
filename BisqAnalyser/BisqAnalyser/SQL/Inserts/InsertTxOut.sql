INSERT INTO dbo.txOut (Id, ScriptPubKey, Val, BisqType, TxId, TxInId) 
VALUES
(@id, @scriptpubkey, @val, @bisqtype, @txid, @txinid)