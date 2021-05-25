using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Io.Bisq.Protobuffer;
using Google.Protobuf.Collections;
using BisqAnalyser.ClusterObjects;

namespace BisqAnalyser
{
    class SqlManager
    {
        string m_connectionString = string.Empty;
        string m_filePath = string.Empty;

        public SqlManager(string connectionString, string filePath)
        {
            m_connectionString = connectionString;
            m_filePath = filePath;
        }

        ~SqlManager() { }

        public DataTable ExecuteQuery(SqlCommand command)
        {
            using(SqlConnection connection = new SqlConnection(m_connectionString))
            {
                connection.Open();
                command.Connection = connection;
                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public void ExecuteNonQuery(SqlCommand command)
        {
            using (SqlConnection connection = new SqlConnection(m_connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteQueryScript(string folder, string file)
        {
            using (SqlConnection connection = new SqlConnection(m_connectionString))
            {
                string script = File.ReadAllText(Path.Combine(m_filePath, folder, file));
                SqlCommand command = new SqlCommand(script);
                return ExecuteQuery(command);
            }
        }

        public void ExecuteNonQueryScript(string folder, string file)
        {
            string script = File.ReadAllText(Path.Combine(m_filePath, folder, file));
            SqlCommand command = new SqlCommand(script);
            ExecuteNonQuery(command);
        }

        public void CreateTables()
        {
            ExecuteNonQueryScript("Tables", "CreateAllTables.sql");
        }

        public void DropTables()
        {
            ExecuteNonQueryScript("Drops", "DropAllTables.sql");
        }

        public void BulkInsert(DataTable dataTable, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(m_connectionString))
            using (var bulk = new SqlBulkCopy(connection))
            {
                connection.Open();
                bulk.DestinationTableName = tableName;
                bulk.WriteToServer(dataTable);
            }
        }

        #region Bisq_Blockchain_Data
        public void InsertAllBisqRecords(BisqData bisqData)
        {
            BulkInsertBlock(bisqData);
            BulkInsertTransaction(bisqData);
            BulkInsertTranactionInput(bisqData);
            BulkInsertTransactionOutput(bisqData);
        }

        public void InsertBlock(Block block)
        {
            string script = File.ReadAllText(Path.Combine(m_filePath, "Inserts", "InsertBlock.sql"));
            SqlCommand command = new SqlCommand(script);
            command.Parameters.AddWithValue("@id", block.Id);
            command.Parameters.AddWithValue("@hash", block.Hash);
            command.Parameters.AddWithValue("@timestamp", block.Timestamp);
            command.Parameters.AddWithValue("@priceopen", block.PriceInfo.Open);
            command.Parameters.AddWithValue("@pricehigh", block.PriceInfo.High);
            command.Parameters.AddWithValue("@pricelow", block.PriceInfo.Low);
            command.Parameters.AddWithValue("@priceclose", block.PriceInfo.Close);
            command.Parameters.AddWithValue("@pricetimestamp", block.PriceInfo.Timestamp);
            ExecuteNonQuery(command);
        }

        public void BulkInsertBlock(BisqData bisqData)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Hash", typeof(string));
            table.Columns.Add("Timestamp", typeof(long));
            table.Columns.Add("PriceOpen", typeof(long));
            table.Columns.Add("PriceHigh", typeof(long));
            table.Columns.Add("PriceLow", typeof(long));
            table.Columns.Add("PriceClose", typeof(long));
            table.Columns.Add("PriceTimestamp", typeof(long));
            foreach (Block block in bisqData.Blocks.Values)
            {
                var newRow = table.NewRow();
                newRow["Id"] = block.Id;
                newRow["Hash"] = block.Hash;
                newRow["Timestamp"] = block.Timestamp;
                newRow["PriceOpen"] = block.PriceInfo.Open;
                newRow["PriceHigh"] = block.PriceInfo.High;
                newRow["PriceLow"] = block.PriceInfo.Low;
                newRow["PriceClose"] = block.PriceInfo.Close;
                newRow["PriceTimestamp"] = block.PriceInfo.Timestamp;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "block");
        }

        public void InsertTransaction(Transaction transaction)
        {
            string script = File.ReadAllText(Path.Combine(m_filePath, "Inserts", "InsertTx.sql"));
            SqlCommand command = new SqlCommand(script);
            command.Parameters.AddWithValue("@id", transaction.Id);
            command.Parameters.AddWithValue("@hash", transaction.Hash);
            command.Parameters.AddWithValue("@version", transaction.Version);
            command.Parameters.AddWithValue("@locktime", transaction.LockTime);
            command.Parameters.AddWithValue("@bisqtype", transaction.BisqType);
            command.Parameters.AddWithValue("@bisqproposalaccepted", transaction.BisqProposalAccepted == 3 ? DBNull.Value : (object)transaction.BisqProposalAccepted);
            command.Parameters.AddWithValue("@blockid", transaction.Block.Id);
            ExecuteNonQuery(command);
        }

        public void BulkInsertTransaction(BisqData bisqData)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Hash", typeof(string));
            table.Columns.Add("Version", typeof(byte));
            table.Columns.Add("LockTime", typeof(long));
            table.Columns.Add("BisqType", typeof(byte));
            table.Columns.Add("BisqProposalAccepted", typeof(byte));
            table.Columns.Add("BlockId", typeof(long));
            foreach (Transaction transaction in bisqData.Transactions.Values)
            {
                var newRow = table.NewRow();
                newRow["Id"] = transaction.Id;
                newRow["Hash"] = transaction.Hash;
                newRow["Version"] = transaction.Version;
                newRow["LockTime"] = transaction.LockTime;
                newRow["BisqType"] = transaction.BisqType;
                newRow["BisqProposalAccepted"] = transaction.BisqProposalAccepted == 3 ? DBNull.Value : (object)transaction.BisqProposalAccepted;
                newRow["BlockId"] = transaction.Block.Id;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "tx");
        }

        public void InsertTransactionInput(TransactionInput input)
        {
            string script = File.ReadAllText(Path.Combine(m_filePath, "Inserts", "InsertTxIn.sql"));
            SqlCommand command = new SqlCommand(script);
            command.Parameters.AddWithValue("@id", input.Id);
            command.Parameters.AddWithValue("@scriptsig", input.ScriptSig);
            command.Parameters.AddWithValue("@seqnum", input.SeqNum);
            command.Parameters.AddWithValue("@txid", input.Transaction.Id);
            command.Parameters.AddWithValue("@txoutid", input.TransactionOutput == null ? DBNull.Value : (object)input.TransactionOutput.Id);
            ExecuteNonQuery(command);
        }

        public void BulkInsertTranactionInput(BisqData bisqData)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("ScriptSig", typeof(byte[]));
            table.Columns.Add("SeqNum", typeof(long));
            table.Columns.Add("TxId", typeof(long));
            table.Columns.Add("TxOutId", typeof(long));
            foreach (TransactionInput input in bisqData.TransactionInputs.Values)
            {
                var newRow = table.NewRow();
                newRow["Id"] = input.Id;
                newRow["ScriptSig"] = input.ScriptSig;
                newRow["SeqNum"] = input.SeqNum;
                newRow["TxId"] = input.Transaction.Id;
                newRow["TxOutId"] = (input.TransactionOutput == null ? DBNull.Value : (object)input.TransactionOutput.Id);
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "txIn");
        }

        public void InsertTranctionOutput(TransactionOutput output)
        {
            string script = File.ReadAllText(Path.Combine(m_filePath, "Inserts", "InsertTxOut.sql"));
            SqlCommand command = new SqlCommand(script);
            command.Parameters.AddWithValue("@id", output.Id);
            command.Parameters.AddWithValue("@scriptpubkey", output.ScriptPubKey);
            command.Parameters.AddWithValue("@val", output.Value);
            command.Parameters.AddWithValue("@bisqtype", output.BisqType);
            command.Parameters.AddWithValue("@txid", output.Transaction.Id);
            command.Parameters.AddWithValue("@txinid", output.TransactionInput == null ? DBNull.Value : (object)output.TransactionInput.Id);
            ExecuteNonQuery(command);
        }

        public void BulkInsertTransactionOutput(BisqData bisqData)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("ScriptPubKey", typeof(byte[]));
            table.Columns.Add("Val", typeof(long));
            table.Columns.Add("BisqType", typeof(byte));
            table.Columns.Add("TxId", typeof(long));
            table.Columns.Add("TxInId", typeof(long));
            foreach (TransactionOutput output in bisqData.TransactionOutputs.Values)
            {
                var newRow = table.NewRow();
                newRow["Id"] = output.Id;
                newRow["ScriptPubKey"] = output.ScriptPubKey;
                newRow["Val"] = output.Value;
                newRow["BisqType"] = output.BisqType;
                newRow["TxId"] = output.Transaction.Id;
                newRow["TxInId"] = output.TransactionInput == null ? DBNull.Value : (object)output.TransactionInput.Id;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "txOut");
        }
        #endregion

        #region Bisq_Protobuf_Data
        public void BulkInsertDaoStateStore(DaoStateStore daoStateStore)
        {
            BulkInsertDaoStateHash(daoStateStore.DaoStateHash);
            BulkInsertDaoStateCycles(daoStateStore.DaoState.Cycles);
            BulkInsertEvaluatedProposalList(daoStateStore.DaoState.EvaluatedProposalList);
            BulkInsertDecryptedBallotsList(daoStateStore.DaoState.DecryptedBallotsWithMeritsList);
            BulkInsertConfiscatedLockupTxList(daoStateStore.DaoState.ConfiscatedLockupTxList);
            BulkInsertIssuance(daoStateStore.DaoState.IssuanceMap);
            BulkInsertParamChangeList(daoStateStore.DaoState.ParamChangeList);
            BulkInsertSpentInfoMap(daoStateStore.DaoState.SpentInfoMap);
            BulkInsertUnspentTxOutput(daoStateStore.DaoState.UnspentTxOutputMap);
        }

        public void BulkInsertUnspentTxOutput(MapField<string, BaseTxOutput> unspentTxOutputMap)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("MapKey", typeof(string));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("BlockHeight", typeof(int));
            table.Columns.Add("OutputIndex", typeof(int));
            table.Columns.Add("MessageCase", typeof(string));
            table.Columns.Add("OpReturnData", typeof(byte[]));
            table.Columns.Add("PubKeyScriptAsm", typeof(string));
            table.Columns.Add("PubKeyScriptHex", typeof(string));
            table.Columns.Add("PubKeyScriptReqSigs", typeof(int));
            table.Columns.Add("PubKeyScriptScriptType", typeof(string));
            table.Columns.Add("TxId", typeof(string));
            table.Columns.Add("TxOutputLockTime", typeof(int));
            table.Columns.Add("TxOutputType", typeof(string));
            table.Columns.Add("TxOutputUnlockBlockHeight", typeof(int));
            table.Columns.Add("Value", typeof(long));

            var addressesTable = new DataTable();
            addressesTable.Columns.Add("Id", typeof(int));
            addressesTable.Columns.Add("Address", typeof(string));
            addressesTable.Columns.Add("UnspentTxOutputId", typeof(int));

            int counter = 0;
            int addressesCounter = 0;

            foreach(var unspentTxOutput in unspentTxOutputMap)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["MapKey"] = unspentTxOutput.Key;
                newRow["Address"] = unspentTxOutput.Value.Address;
                newRow["BlockHeight"] = unspentTxOutput.Value.BlockHeight;
                newRow["OutputIndex"] = unspentTxOutput.Value.Index;
                newRow["MessageCase"] = unspentTxOutput.Value.MessageCase.ToString();
                newRow["OpReturnData"] = unspentTxOutput.Value.OpReturnData.ToByteArray();
                newRow["PubKeyScriptAsm"] = unspentTxOutput.Value.PubKeyScript.Asm;
                newRow["PubKeyScriptHex"] = unspentTxOutput.Value.PubKeyScript.Hex;
                newRow["PubKeyScriptReqSigs"] = unspentTxOutput.Value.PubKeyScript.ReqSigs;
                newRow["PubKeyScriptScriptType"] = unspentTxOutput.Value.PubKeyScript.ScriptType.ToString();
                newRow["TxId"] = unspentTxOutput.Value.TxId;
                newRow["TxOutputLockTime"] = unspentTxOutput.Value.TxOutput.LockTime;
                newRow["TxOutputType"] = unspentTxOutput.Value.TxOutput.TxOutputType.ToString();
                newRow["TxOutputUnlockBlockHeight"] = unspentTxOutput.Value.TxOutput.UnlockBlockHeight;
                newRow["Value"] = unspentTxOutput.Value.Value;
                foreach (var address in unspentTxOutput.Value.PubKeyScript.Addresses)
                {
                    var newAddressRow = addressesTable.NewRow();
                    newAddressRow["Id"] = addressesCounter;
                    newAddressRow["Address"] = address;
                    newAddressRow["UnspentTxOutputId"] = counter;
                    addressesTable.Rows.Add(newAddressRow);
                    ++addressesCounter;
                }
                ++counter;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "unspentTxOutput");
            BulkInsert(addressesTable, "unspentTxOutputAddresses");
        }

        public void BulkInsertSpentInfoMap(MapField<string, SpentInfo> spentInfoMap)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("MapKey", typeof(string));
            table.Columns.Add("BlockHeight", typeof(long));
            table.Columns.Add("InputIndex", typeof(int));
            table.Columns.Add("TxId", typeof(string));
            int counter = 0;
            foreach(var spentInfo in spentInfoMap)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["MapKey"] = spentInfo.Key;
                newRow["BlockHeight"] = spentInfo.Value.BlockHeight;
                newRow["InputIndex"] = spentInfo.Value.InputIndex;
                newRow["TxId"] = spentInfo.Value.TxId;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "spentInfo");
        }

        public void BulkInsertParamChangeList(RepeatedField<ParamChange> paramChangeList)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ActivationHeight", typeof(int));
            table.Columns.Add("ParamName", typeof(string));
            table.Columns.Add("ParamValue", typeof(string));
            for(int i = 0; i < paramChangeList.Count; ++i)
            {
                var newRow = table.NewRow();
                newRow["Id"] = i;
                newRow["ActivationHeight"] = paramChangeList[i].ActivationHeight;
                newRow["ParamName"] = paramChangeList[i].ParamName;
                newRow["ParamValue"] = paramChangeList[i].ParamValue;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "paramChangeList");
        }

        public void BulkInsertIssuance(MapField<string, Issuance> issuanceMap)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("MapKey", typeof(string));
            table.Columns.Add("Amount", typeof(long));
            table.Columns.Add("ChainHeight", typeof(int));
            table.Columns.Add("IssuanceType", typeof(string));
            table.Columns.Add("PubKey", typeof(string));
            table.Columns.Add("TxId", typeof(string));
            int counter = 0;
            foreach(var issuance in issuanceMap)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["MapKey"] = issuance.Key;
                newRow["Amount"] = issuance.Value.Amount;
                newRow["ChainHeight"] = issuance.Value.ChainHeight;
                newRow["IssuanceType"] = issuance.Value.IssuanceType;
                newRow["PubKey"] = issuance.Value.PubKey;
                newRow["TxId"] = issuance.Value.TxId;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "issuance");
        }

        public void BulkInsertConfiscatedLockupTxList(RepeatedField<string> confiscatedLockupTxList)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("TxId", typeof(string));
            for (int i = 0; i < confiscatedLockupTxList.Count; ++i)
            {
                var newRow = table.NewRow();
                newRow["Id"] = i;
                newRow["TxID"] = confiscatedLockupTxList[i];
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "confiscatedLockupTx");
        }

        public void BulkInsertDecryptedBallotsList(RepeatedField<DecryptedBallotsWithMerits> decryptedBallotsList)
        {
            var decryptedBallotsListTable = new DataTable();
            decryptedBallotsListTable.Columns.Add("Id", typeof(int));
            decryptedBallotsListTable.Columns.Add("BlindVoteTxId", typeof(string));
            decryptedBallotsListTable.Columns.Add("HashOfBlindVoteList", typeof(byte[]));
            decryptedBallotsListTable.Columns.Add("Stake", typeof(long));
            decryptedBallotsListTable.Columns.Add("VoteRevealTxId", typeof(string));

            var individualBallotTable = new DataTable();
            individualBallotTable.Columns.Add("Id", typeof(int));
            individualBallotTable.Columns.Add("DecryptedBallotListId", typeof(int));
            individualBallotTable.Columns.Add("ProposalTxId", typeof(string));
            individualBallotTable.Columns.Add("VoteAccepted", typeof(bool));

            int listCounter = 0;
            int ballotCounter = 0;

            foreach (var decryptedBallots in decryptedBallotsList)
            {
                var newDecryptedListRow = decryptedBallotsListTable.NewRow();
                newDecryptedListRow["Id"] = listCounter;
                newDecryptedListRow["BlindVoteTxId"] = decryptedBallots.BlindVoteTxId;
                newDecryptedListRow["HashOfBlindVoteList"] = decryptedBallots.HashOfBlindVoteList.ToByteArray();
                newDecryptedListRow["Stake"] = decryptedBallots.Stake;
                newDecryptedListRow["VoteRevealTxId"] = decryptedBallots.VoteRevealTxId;
                decryptedBallotsListTable.Rows.Add(newDecryptedListRow);
                foreach(var ballot in decryptedBallots.BallotList.Ballot)
                {
                    var newBallotRow = individualBallotTable.NewRow();
                    newBallotRow["Id"] = ballotCounter;
                    newBallotRow["DecryptedBallotListId"] = listCounter;
                    newBallotRow["ProposalTxId"] = ballot.Proposal.TxId;
                    newBallotRow["VoteAccepted"] = ballot.Vote.Accepted;
                    individualBallotTable.Rows.Add(newBallotRow);
                    ++ballotCounter;
                }
                ++listCounter;
            }
            BulkInsert(decryptedBallotsListTable, "decryptedBallotList");
            BulkInsert(individualBallotTable, "ballot");
        }

        public void BulkInsertEvaluatedProposalList(RepeatedField<EvaluatedProposal> evaluatedProposals)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("IsAccepted", typeof(bool));
            table.Columns.Add("NumAcceptedVotes", typeof(int));
            table.Columns.Add("NumIgnoredVotes", typeof(int));
            table.Columns.Add("NumRejectedVotes", typeof(int));
            table.Columns.Add("StakeOfAcceptedVotes", typeof(long));
            table.Columns.Add("StakeOfRejectedVotes", typeof(long));
            table.Columns.Add("ProposalTxId", typeof(string));
            int counter = 0;
            foreach (var proposal in evaluatedProposals)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["IsAccepted"] = proposal.IsAccepted;
                newRow["NumAcceptedVotes"] = proposal.ProposalVoteResult.NumAcceptedVotes;
                newRow["NumIgnoredVotes"] = proposal.ProposalVoteResult.NumIgnoredVotes;
                newRow["NumRejectedVotes"] = proposal.ProposalVoteResult.NumRejectedVotes;
                newRow["StakeOfAcceptedVotes"] = proposal.ProposalVoteResult.StakeOfAcceptedVotes;
                newRow["StakeOfRejectedVotes"] = proposal.ProposalVoteResult.StakeOfRejectedVotes;
                newRow["ProposalTxId"] = proposal.ProposalVoteResult.Proposal.TxId;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "evaluatedProposal");
        }

        public void BulkInsertDaoStateCycles(RepeatedField<Cycle> cycles)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("CycleNumber", typeof(int));
            table.Columns.Add("Duration", typeof(int));
            table.Columns.Add("PhaseOrdinal", typeof(int));
            int cycleCounter = 0;
            int idCounter = 0;
            foreach (var cycle in cycles)
            {
                foreach(var daoPhase in cycle.DaoPhase)
                {
                    var newRow = table.NewRow();
                    newRow["Id"] = idCounter;
                    newRow["CycleNumber"] = cycleCounter;
                    newRow["Duration"] = daoPhase.Duration;
                    newRow["PhaseOrdinal"] = daoPhase.PhaseOrdinal;
                    table.Rows.Add(newRow);
                    ++idCounter;
                }
                ++cycleCounter;
            }
            BulkInsert(table, "cycle");
        }

        public void BulkInsertDaoStateHash(RepeatedField<DaoStateHash> daoStateHashes)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Height", typeof(int));
            table.Columns.Add("Hash", typeof(byte[]));
            table.Columns.Add("PrevHash", typeof(byte[]));
            int counter = 0;
            foreach(DaoStateHash daoStateHash in daoStateHashes)
            {
                var newRow = table.NewRow();
                newRow["Id"] = table.Rows.Count;
                newRow["Height"] = daoStateHash.Height;
                newRow["Hash"] = daoStateHash.Hash.ToByteArray();
                newRow["PrevHash"] = daoStateHash.PrevHash.ToByteArray();
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "daoStateHash");
        }

        public void BulkInsertBlindVoteStore(BlindVoteStore blindVoteStore)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Hash", typeof(byte[]));
            table.Columns.Add("Date", typeof(long));
            table.Columns.Add("EncryptedVotes", typeof(byte[]));
            table.Columns.Add("EncryptedMeritList", typeof(byte[]));
            table.Columns.Add("Stake", typeof(long));
            table.Columns.Add("TxId", typeof(string));
            long counter = 0;
            foreach(var vote in blindVoteStore.Items)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["Hash"] = vote.Hash.ToByteArray();
                newRow["Date"] = vote.BlindVote.Date;
                newRow["EncryptedVotes"] = vote.BlindVote.EncryptedVotes.ToByteArray();
                newRow["EncryptedMeritList"] = vote.BlindVote.EncryptedMeritList.ToByteArray();
                newRow["Stake"] = vote.BlindVote.Stake;
                newRow["TxId"] = vote.BlindVote.TxId;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "blindVote");
        }

        public void BulkInsertTradeStatistics2Store(TradeStatistics2Store tradeStatistics2Store)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Hash", typeof(byte[]));
            table.Columns.Add("BaseCurrency", typeof(string));
            table.Columns.Add("CounterCurrency", typeof(string));
            table.Columns.Add("DepositTxId", typeof(string));
            table.Columns.Add("Direction", typeof(string));
            table.Columns.Add("OfferAmount", typeof(long));
            table.Columns.Add("OfferDate", typeof(long));
            table.Columns.Add("OfferId", typeof(string));
            table.Columns.Add("OfferMarketPriceMargin", typeof(double));
            table.Columns.Add("OfferMinAmount", typeof(long));
            table.Columns.Add("OfferUseMarketBasedPrice", typeof(bool));
            table.Columns.Add("PaymentMethodId", typeof(string));
            table.Columns.Add("TradeAmount", typeof(long));
            table.Columns.Add("TradeDate", typeof(long));
            table.Columns.Add("TradePrice", typeof(long));
            long counter = 0;
            foreach(var tradeStat in tradeStatistics2Store.Items)
            {
                var newRow = table.NewRow();
                newRow["Id"] = table.Rows.Count;
                newRow["Hash"] = tradeStat.Hash.ToByteArray();
                newRow["BaseCurrency"] = tradeStat.BaseCurrency;
                newRow["CounterCurrency"] = tradeStat.CounterCurrency;
                newRow["DepositTxId"] = tradeStat.DepositTxId;
                newRow["Direction"] = tradeStat.Direction.ToString();
                newRow["OfferAmount"] = tradeStat.OfferAmount;
                newRow["OfferDate"] = tradeStat.OfferDate;
                newRow["OfferId"] = tradeStat.OfferId;
                newRow["OfferMarketPriceMargin"] = tradeStat.OfferMarketPriceMargin;
                newRow["OfferMinAmount"] = tradeStat.OfferMinAmount;
                newRow["OfferUseMarketBasedPrice"] = tradeStat.OfferUseMarketBasedPrice;
                newRow["PaymentMethodId"] = tradeStat.PaymentMethodId;
                newRow["TradeAmount"] = tradeStat.TradeAmount;
                newRow["TradeDate"] = tradeStat.TradeDate;
                newRow["TradePrice"] = tradeStat.TradePrice;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "tradeStatistics2Store");
        }

        // This function needs to be broken up
        public void BulkInsertProposalStore(ProposalStore proposalStore)
        {
            // Compensation
            var compensationTable = new DataTable();
            compensationTable.Columns.Add("Id", typeof(long));
            compensationTable.Columns.Add("BsqAddress", typeof(string));
            compensationTable.Columns.Add("RequestedBsq", typeof(long));

            // Reimbursement
            var reimbursementTable = new DataTable();
            reimbursementTable.Columns.Add("Id", typeof(long));
            reimbursementTable.Columns.Add("BsqAddress", typeof(string));
            reimbursementTable.Columns.Add("RequestedBsq", typeof(long));

            // Change Param
            var changeParamTable = new DataTable();
            changeParamTable.Columns.Add("Id", typeof(long));
            changeParamTable.Columns.Add("Param", typeof(string));
            changeParamTable.Columns.Add("ParamValue", typeof(string));

            // Role
            var roleTable = new DataTable();
            roleTable.Columns.Add("Id", typeof(long));
            roleTable.Columns.Add("RoleUid", typeof(string));
            roleTable.Columns.Add("RoleName", typeof(string));
            roleTable.Columns.Add("Link", typeof(string));
            roleTable.Columns.Add("BondedRoleType", typeof(string));
            roleTable.Columns.Add("RequiredBondUnit", typeof(long));

            // Overall Proposal
            var proposalTable = new DataTable();
            proposalTable.Columns.Add("Id", typeof(long));
            proposalTable.Columns.Add("Hash", typeof(byte[]));
            proposalTable.Columns.Add("Name", typeof(string));
            proposalTable.Columns.Add("Link", typeof(string));
            proposalTable.Columns.Add("Version", typeof(long));
            proposalTable.Columns.Add("TxId", typeof(string));
            proposalTable.Columns.Add("CreationDate", typeof(long));
            proposalTable.Columns.Add("CompensationProposalId", typeof(long));
            proposalTable.Columns.Add("ReimbursementProposalId", typeof(long));
            proposalTable.Columns.Add("ChangeParamProposalId", typeof(long));
            proposalTable.Columns.Add("RoleProposalId", typeof(long));
            long counter = 0;
            foreach (var proposal in proposalStore.Items)
            {
                var newRow = proposalTable.NewRow();
                newRow["Id"] = counter;
                newRow["Hash"] = proposal.Hash.ToByteArray();
                newRow["Name"] = proposal.Proposal.Name;
                newRow["Link"] = proposal.Proposal.Link;
                newRow["Version"] = proposal.Proposal.Version;
                newRow["TxId"] = proposal.Proposal.TxId;
                newRow["CreationDate"] = proposal.Proposal.CreationDate;
                newRow["CompensationProposalId"] = DBNull.Value;
                newRow["ReimbursementProposalId"] = DBNull.Value;
                newRow["ChangeParamProposalId"] = DBNull.Value;
                newRow["RoleProposalId"] = DBNull.Value;
                if (null != proposal.Proposal.CompensationProposal)
                {
                    var newCompensationRow = compensationTable.NewRow();
                    newCompensationRow["Id"] = compensationTable.Rows.Count;
                    newRow["CompensationProposalId"] = compensationTable.Rows.Count;
                    newCompensationRow["BsqAddress"] = proposal.Proposal.CompensationProposal.BsqAddress;
                    newCompensationRow["RequestedBsq"] = proposal.Proposal.CompensationProposal.RequestedBsq;
                    compensationTable.Rows.Add(newCompensationRow);
                }
                else if(null != proposal.Proposal.ReimbursementProposal)
                {
                    var newReimbursementRow = reimbursementTable.NewRow();
                    newReimbursementRow["Id"] = reimbursementTable.Rows.Count;
                    newRow["ReimbursementProposalId"] = reimbursementTable.Rows.Count;
                    newReimbursementRow["BsqAddress"] = proposal.Proposal.ReimbursementProposal.BsqAddress;
                    newReimbursementRow["RequestedBsq"] = proposal.Proposal.ReimbursementProposal.RequestedBsq;
                    reimbursementTable.Rows.Add(newReimbursementRow);
                }
                else if(null != proposal.Proposal.ChangeParamProposal)
                {
                    var newChangeParamRow = changeParamTable.NewRow();
                    newChangeParamRow["Id"] = changeParamTable.Rows.Count;
                    newRow["ChangeParamProposalId"] = changeParamTable.Rows.Count;
                    newChangeParamRow["Param"] = proposal.Proposal.ChangeParamProposal.Param;
                    newChangeParamRow["ParamValue"] = proposal.Proposal.ChangeParamProposal.ParamValue;
                    changeParamTable.Rows.Add(newChangeParamRow);
                }
                else if(null != proposal.Proposal.RoleProposal)
                {
                    var newRoleRow = roleTable.NewRow();
                    newRoleRow["Id"] = roleTable.Rows.Count;
                    newRow["RoleProposalId"] = roleTable.Rows.Count;
                    newRoleRow["RoleUid"] = proposal.Proposal.RoleProposal.Role.Uid;
                    newRoleRow["RoleName"] = proposal.Proposal.RoleProposal.Role.Name;
                    newRoleRow["Link"] = proposal.Proposal.RoleProposal.Role.Link;
                    newRoleRow["BondedRoleType"] = proposal.Proposal.RoleProposal.Role.BondedRoleType;
                    newRoleRow["RequiredBondUnit"] = proposal.Proposal.RoleProposal.RequiredBondUnit;
                    roleTable.Rows.Add(newRoleRow);
                }
                proposalTable.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(proposalTable, "proposal");
            BulkInsert(compensationTable, "compensationProposal");
            BulkInsert(reimbursementTable, "reimbursementProposal");
            BulkInsert(changeParamTable, "changeParamProposal");
            BulkInsert(roleTable, "roleProposal");
        }
        #endregion

        #region Cluster_Data
        public void InsertAllClusterData(ClusterData clusterData)
        {
            InsertAddresses(clusterData.Addresses);
            InsertClusters(clusterData.Clusters);
        }

        private void InsertAddresses(Dictionary<long, Address> addresses)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Hash", typeof(byte[]));
            table.Columns.Add("Type", typeof(byte));
            table.Columns.Add("ClusterId", typeof(long));
            foreach (Address address in addresses.Values)
            {
                DataRow newRow = table.NewRow();
                newRow["Id"] = address.Id;
                newRow["Hash"] = address.Hash;
                newRow["Type"] = address.Type;
                newRow["ClusterId"] = address.Cluster.Id;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "address");
        }

        private void InsertClusters(Dictionary<long, Cluster> clusters)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("TotalSize", typeof(long));
            table.Columns.Add("BisqSize", typeof(long));
            foreach (Cluster cluster in clusters.Values)
            {
                DataRow newRow = table.NewRow();
                newRow["Id"] = cluster.Id;
                newRow["TotalSize"] = cluster.Size;
                newRow["BisqSize"] = cluster.Addresses.Count;
                table.Rows.Add(newRow);
            }
            BulkInsert(table, "cluster");
        }
        #endregion

        #region Stage_2_Bisq_Protobuf_Data
        internal void BulkInsertAddressEntryList(AddressEntryList addressEntryList)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("CoinLockedInMultiSig", typeof(long));
            table.Columns.Add("Context", typeof(string)); // Look at this one
            table.Columns.Add("OfferId", typeof(string));
            table.Columns.Add("PubKey", typeof(byte[]));
            table.Columns.Add("PubKeyHash", typeof(byte[]));
            long counter = 0;
            foreach (AddressEntry addressEntry in addressEntryList.AddressEntry)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["CoinLockedInMultiSig"] = addressEntry.CoinLockedInMultiSig;
                newRow["Context"] = addressEntry.Context.ToString();
                newRow["OfferId"] = addressEntry.OfferId;
                newRow["PubKey"] = addressEntry.PubKey.ToByteArray();
                newRow["PubKeyHash"] = addressEntry.PubKeyHash.ToByteArray();
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "addressEntryList");
        }

        internal void BulkInsertNavigationPath(NavigationPath navigationPath)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Path", typeof(string));
            long counter = 0;
            foreach (var path in navigationPath.Path)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["Path"] = path;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "navigationPath");
        }

        internal void BulkInsertSignedWitnessStore(SignedWitnessStore signedWitnessStore)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("AccountAgeWitnessHash", typeof(byte[]));
            table.Columns.Add("Date", typeof(long));
            table.Columns.Add("Signature", typeof(byte[]));
            table.Columns.Add("SignerPubKey", typeof(byte[]));
            table.Columns.Add("TradeAmount", typeof(long));
            table.Columns.Add("VerificationMethod", typeof(string));
            table.Columns.Add("WitnessOwnerPubKey", typeof(byte[]));
            long counter = 0;
            foreach (SignedWitness signedWitness in signedWitnessStore.Items)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["AccountAgeWitnessHash"] = signedWitness.AccountAgeWitnessHash.ToByteArray();
                newRow["Date"] = signedWitness.Date;
                newRow["Signature"] = signedWitness.Signature.ToByteArray();
                newRow["SignerPubKey"] = signedWitness.SignerPubKey.ToByteArray();
                newRow["TradeAmount"] = signedWitness.TradeAmount;
                newRow["VerificationMethod"] = signedWitness.VerificationMethod.ToString(); // Look at this
                newRow["WitnessOwnerPubKey"] = signedWitness.WitnessOwnerPubKey.ToByteArray();
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "signedWitnessStore");
        }

        internal void BulkInsertSequenceNumberMap(SequenceNumberMap sequenceNumberMap)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Bytes", typeof(byte[]));
            table.Columns.Add("MapValueSequenceNr", typeof(int));
            table.Columns.Add("MapValueTimeStamp", typeof(long));
            long counter = 0;
            foreach (SequenceNumberEntry entry in sequenceNumberMap.SequenceNumberEntries)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["Bytes"] = entry.Bytes.Bytes.ToByteArray();
                newRow["MapValueSequenceNr"] = entry.MapValue.SequenceNr;
                newRow["MapValueTimeStamp"] = entry.MapValue.TimeStamp;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "sequenceNumberMap");
        }

        internal void BulkInsertPeerList(PeerList peerList)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Date", typeof(long));
            table.Columns.Add("NodeAddressHostName", typeof(string));
            table.Columns.Add("NodeAddressPort", typeof(int));
            table.Columns.Add("SupportedCapabilities", typeof(string));
            long counter = 0;
            foreach (Peer peer in peerList.Peer)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["Date"] = peer.Date;
                newRow["NodeAddressHostName"] = peer.NodeAddress.HostName;
                newRow["NodeAddressPort"] = peer.NodeAddress.Port;
                string capabilitiesList = string.Empty;
                foreach(int capability in peer.SupportedCapabilities)
                {
                    capabilitiesList += capability + ",";
                }
                newRow["SupportedCapabilities"] = capabilitiesList;
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "peerList");
        }

        internal void BulkInsertAccountAgeWitnessStore(AccountAgeWitnessStore accountAgeWitnessStore)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Date", typeof(long));
            table.Columns.Add("Hash", typeof(byte[]));
            long counter = 0;
            foreach (AccountAgeWitness entry in accountAgeWitnessStore.Items)
            {
                var newRow = table.NewRow();
                newRow["Id"] = counter;
                newRow["Date"] = entry.Date;
                newRow["Hash"] = entry.Hash.ToByteArray();
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "accountAgeWitnessStore");
        }
        #endregion

        #region New_Protobuf_Data
        public void BulkInsertTradeStatistics3Store(TradeStatistics3Store tradeStatistics3Store)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Currency", typeof(string));
            table.Columns.Add("Price", typeof(long));
            table.Columns.Add("Amount", typeof(long));
            table.Columns.Add("PaymentMethod", typeof(string));
            table.Columns.Add("Date", typeof(long));
            table.Columns.Add("Mediator", typeof(string));
            table.Columns.Add("RefundAgent", typeof(string));
            table.Columns.Add("Hash", typeof(byte[]));
            long counter = 0;
            foreach(var tradeStatistic3 in tradeStatistics3Store.Items)
            {
                var newRow = table.NewRow();
                newRow["Id"] = table.Rows.Count;
                newRow["Currency"] = tradeStatistic3.Currency;
                newRow["Price"] = tradeStatistic3.Price;
                newRow["Amount"] = tradeStatistic3.Amount;
                newRow["PaymentMethod"] = tradeStatistic3.PaymentMethod;
                newRow["Date"] = tradeStatistic3.Date;
                newRow["Mediator"] = tradeStatistic3.Mediator;
                newRow["RefundAgent"] = tradeStatistic3.RefundAgent;
                newRow["Hash"] = tradeStatistic3.Hash.ToByteArray();
                table.Rows.Add(newRow);
                ++counter;
            }
            BulkInsert(table, "tradeStatistics3Store");
        }
        #endregion
    }
}
