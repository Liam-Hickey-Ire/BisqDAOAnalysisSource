using System;
using System.Collections.Generic;
using BisqAnalyser.ClusterObjects;
using LightningDB;
using TransactionBeginFlags = LightningDB.TransactionBeginFlags;

namespace BisqAnalyser
{
    class LmdbReader
    {
        /// <summary>
        /// Load data from LMDB file
        /// </summary>
        /// <param name="envFilePath">File path for LMDB file</param>
        /// <param name="maxDbs">Maximum allowed databses for LMDB environment</param>
        /// <param name="verbose">Boolean representing verbose output</param>
        /// <returns>BisqData object containing data loaded from LMDB file</returns>
        public static BisqData LoadBisqBlockchainData(string envFilePath, int maxDbs, bool verbose)
        {
            BisqData bisqData = new BisqData();
            LightningEnvironment env = new LightningEnvironment(envFilePath);
            env.MaxDatabases = maxDbs;
            env.Open(EnvironmentOpenFlags.NoSubDir | EnvironmentOpenFlags.ReadOnly);
            if(!env.IsOpened)
            {
                Console.WriteLine(string.Format("Error: Failed to Open LMDB Environment at Location {0}", envFilePath));
            }
            else
            {
                using (LightningTransaction tx = env.BeginTransaction(TransactionBeginFlags.ReadOnly))
                {
                    Console.Write("Sucessfully Opened Blockchain LMDB Environment, Loading...");
                    bisqData.Blocks = LoadBlocks(tx, verbose);
                    bisqData.Transactions = LoadTransactions(tx, verbose);
                    bisqData.TransactionInputs = LoadTransactionInputs(tx, verbose);
                    bisqData.TransactionOutputs = LoadTransactionOutputs(tx, verbose);
                    LinkObjects(tx, ref bisqData, verbose);
                    tx.Dispose();
                }
                Console.WriteLine("Complete");
            }
            env.Dispose();
            return bisqData;
        }

        /// <summary>
        /// Read all data from a specified LMDB database
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="dbName">Name of the database to read</param>
        /// <returns>List<Tuple<byte[], byte[]>> representing every key-value pair in database</returns>
        private static List<Tuple<byte[], byte[]>> ReadDatabase(LightningTransaction tx, string dbName)
        {
            List<Tuple<byte[], byte[]>> kvPairs = new List<Tuple<byte[], byte[]>>();
            using (LightningDatabase db = tx.OpenDatabase(dbName))
            {
                long entryCount = tx.GetEntriesCount(db);
                using (LightningCursor cursor = tx.CreateCursor(db))
                {
                    cursor.MoveToFirst();
                    for (long i = 0; i < entryCount; ++i)
                    {
                        kvPairs.Add(new Tuple<byte[], byte[]>(cursor.Current.Key, cursor.Current.Value));
                        cursor.MoveNext();
                    }
                    cursor.Dispose();
                }
                db.Dispose();
            }
            return kvPairs;
        }

        /// <summary>
        /// Read value for a key in a specified LMDB database
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="key">Key for desired value</param>
        /// <param name="dbName">Name of the database to read</param>
        /// <returns>byte[] representing value for specified key</returns>
        private static byte[] GetValueByKey(LightningTransaction tx, byte[] key, string dbName)
        {
            using (LightningDatabase db = tx.OpenDatabase(dbName))
            {
                return tx.Get(db, key);
            }
        }

        #region Block_Loading
        /// <summary>
        /// Load blocks from LMDB environment
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <returns>Dictionary containing all blocks by their IDs</returns>
        private static Dictionary<long, Block> LoadBlocks(LightningTransaction tx, bool verbose)
        {
            Dictionary<long, Block> blocks = new Dictionary<long, Block>();
            List<Tuple<byte[], byte[]>> blockHashData = ReadDatabase(tx, "block->hash");
            long number = 0;
            foreach (var entry in blockHashData)
            {
                // Block Info
                long id = BitConverter.ToInt64(entry.Item1, 0);
                string hash = Util.ByteArrayToHex(entry.Item2);
                long blockTimestamp = BitConverter.ToUInt32(GetValueByKey(tx, entry.Item1, "block->timestamp"), 0);

                // Price Info
                long open = BitConverter.ToInt64(GetValueByKey(tx, entry.Item1, "block->price-open"), 0);
                long high = BitConverter.ToInt64(GetValueByKey(tx, entry.Item1, "block->price-high"), 0);
                long low = BitConverter.ToInt64(GetValueByKey(tx, entry.Item1, "block->price-low"), 0);
                long close = BitConverter.ToInt64(GetValueByKey(tx, entry.Item1, "block->price-close"), 0);
                long priceTimestamp = BitConverter.ToUInt32(GetValueByKey(tx, entry.Item1, "block->price-timestamp"), 0);

                PriceInfo priceInfo = new PriceInfo(open, high, low, close, priceTimestamp);
                Block block = new Block(number, id, hash, blockTimestamp, priceInfo);
                blocks.Add(id, block);
                if (verbose)
                {
                    Console.WriteLine(string.Format("block(number: {0} - id: {1} - hash: {2} - timestamp: {3})", number, id, hash, blockTimestamp));
                    Console.WriteLine(string.Format("price(open: {0} - high: {1} - low: {2} - close: {3} - timestamp: {4})", open, high, low, close, priceTimestamp));
                }
                ++number;
            }
            return blocks;
        }
        #endregion

        #region Transaction_Loading
        /// <summary>
        /// Load transactions from LMDB environment
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <returns>Dictionary containg all transactions by their IDs</returns>
        private static Dictionary<long, Transaction> LoadTransactions(LightningTransaction tx, bool verbose)
        {
            Dictionary<long, Transaction> transactions = new Dictionary<long, Transaction>();
            List<Tuple<byte[], byte[]>> transactionHashData = ReadDatabase(tx, "tx->hash");
            long transactionNumber = 0;
            foreach (var entry in transactionHashData)
            {
                long id = BitConverter.ToInt64(entry.Item1, 0);
                string hash = Util.ByteArrayToHex(entry.Item2);
                byte version = GetValueByKey(tx, entry.Item1, "tx->version")[0];
                long lockTime = BitConverter.ToUInt32(GetValueByKey(tx, entry.Item1, "tx->lock-time"), 0);
                byte bisqType = GetValueByKey(tx, entry.Item1, "tx->bisq-type")[0];
                byte[] resultArray = GetValueByKey(tx, entry.Item1, "tx->bisq-proposal-accepted");
                byte bisqProposalAccepted = (null == resultArray ? (byte)3 : resultArray[0]);
                Transaction transaction = new Transaction(transactionNumber, id, hash, version, lockTime, bisqType, bisqProposalAccepted);
                transactions.Add(id, transaction);
                if (verbose)
                {
                    Console.WriteLine(string.Format("tx(number: {0} - id: {1} - hash: {2} - version: {3} - lock-time: {4} - bisq-type: {5})", 
                        transactionNumber, id, hash, version, lockTime, bisqType));
                }
                ++transactionNumber;
            }
            return transactions;
        }
        #endregion

        #region TransactionIn_Loading
        /// <summary>
        /// Load transaction inputs from LMDB environment
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <returns>Dictionary containing all transaction inputs by their IDs</returns>
        private static Dictionary<long, TransactionInput> LoadTransactionInputs(LightningTransaction tx, bool verbose)
        {
            Dictionary<long, TransactionInput> transactionInputs = new Dictionary<long, TransactionInput>();
            List<Tuple<byte[], byte[]>> transactionInputScriptSigData = ReadDatabase(tx, "tx-in->script-sig");
            long number = 0;
            foreach (var entry in transactionInputScriptSigData)
            {
                long id = BitConverter.ToInt64(entry.Item1, 0);
                byte[] scriptSig = entry.Item2;
                long seqNum = BitConverter.ToInt64(GetValueByKey(tx, entry.Item1, "tx-in->seq-num"), 0);
                TransactionInput transactionInput = new TransactionInput(number, id, scriptSig, seqNum);
                transactionInputs.Add(id, transactionInput);
                if (verbose)
                {
                    Console.WriteLine(string.Format("tx(number: {0} - id: {1} - script-sig(hex): {2} - seq-num: {3} - bisq-type: {4})", 
                        number, id, Util.ByteArrayToHex(scriptSig), seqNum));
                }
                ++number;
            }
            return transactionInputs;
        }
        #endregion

        #region TransactionOut_Loading
        /// <summary>
        /// Load transaction outputs from LMDB environment
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <returns>Dictionary containing all transaction outputs by their IDs</returns>
        private static Dictionary<long, TransactionOutput> LoadTransactionOutputs(LightningTransaction tx, bool verbose)
        {
            Dictionary<long, TransactionOutput> transactionOutputs = new Dictionary<long, TransactionOutput>();
            List<Tuple<byte[], byte[]>> transactionOutputValueData = ReadDatabase(tx, "tx-out->value");
            long number = 0;
            foreach (var entry in transactionOutputValueData)
            {
                long id = BitConverter.ToInt64(entry.Item1, 0);
                long value = BitConverter.ToInt64(entry.Item2, 0);
                byte[] scriptPubKey = GetValueByKey(tx, entry.Item1, "tx-out->script-pubkey");
                byte bisqType = GetValueByKey(tx, entry.Item1, "tx-out->bisq-type")[0];
                TransactionOutput transactionOutput = new TransactionOutput(number, id, value, scriptPubKey, bisqType);
                transactionOutputs.Add(id, transactionOutput);
                if (verbose)
                {
                    Console.WriteLine(string.Format("tx-out(number: {0} - id: {1} - value: {2} - script-pubkey(hex): {3} - bisq-type: {4})", 
                        number, id, value, Util.ByteArrayToHex(scriptPubKey), bisqType));
                }
                ++number;
            }
            return transactionOutputs;
        }
        #endregion

        #region Linking
        /// <summary>
        /// Link all objects within BisqData object
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="bisqData">BisqData object containing all objects to be linked</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        private static void LinkObjects(LightningTransaction tx, ref BisqData bisqData, bool verbose)
        {
            LinkBlocksToTransactions(tx, ref bisqData, verbose);
            LinkTransactionToInput(tx, ref bisqData, verbose);
            LinkTransactionToOutput(tx, ref bisqData, verbose);
            LinkInputsToOutputs(tx, ref bisqData, verbose);
        }

        /// <summary>
        /// Link blocks to transactions in BisqData object
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="bisqData">BisqData object containing blocks and transactions to be linked</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        private static void LinkBlocksToTransactions(LightningTransaction tx, ref BisqData bisqData, bool verbose)
        {
            if(verbose)
            {
                Console.WriteLine("Linking blocks to transactions...");
            }
            List<Tuple<byte[], byte[]>> data = ReadDatabase(tx, "tx->block");
            foreach (var entry in data)
            {
                long transactionId = BitConverter.ToInt64(entry.Item1, 0);
                long blockId = BitConverter.ToInt64(entry.Item2, 0);
                Block block = bisqData.Blocks[blockId];
                Transaction transaction = bisqData.Transactions[transactionId];
                block.AddTransaction(transaction);
                transaction.Block = block;
            }
        }

        /// <summary>
        /// Link transactions to transaction inputs
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="bisqData">BisqData object containing transactions and transaction inputs to be linked</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        private static void LinkTransactionToInput(LightningTransaction tx, ref BisqData bisqData, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("Linking transactions to inputs...");
            }
            List<Tuple<byte[], byte[]>> data = ReadDatabase(tx, "tx-in->tx");
            foreach (var entry in data)
            {
                long transactionInputId = BitConverter.ToInt64(entry.Item1, 0);
                long transactionId = BitConverter.ToInt64(entry.Item2, 0);
                Transaction transaction = bisqData.Transactions[transactionId];
                TransactionInput transactionInput = bisqData.TransactionInputs[transactionInputId];
                transaction.AddInput(transactionInput);
                transactionInput.Transaction = transaction;
            }
        }

        /// <summary>
        /// Link transactions to transaction outputs
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="bisqData">BisqData object containing transactions and transaction outputs to be linked</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        private static void LinkTransactionToOutput(LightningTransaction tx, ref BisqData bisqData, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("Linking transactions to outputs...");
            }
            List<Tuple<byte[], byte[]>> data = ReadDatabase(tx, "tx-out->tx");
            foreach (var entry in data)
            {
                long transactionOutputId = BitConverter.ToInt64(entry.Item1, 0);
                long transactionId = BitConverter.ToInt64(entry.Item2, 0);
                Transaction transaction = bisqData.Transactions[transactionId];
                TransactionOutput transactionOutput = bisqData.TransactionOutputs[transactionOutputId];
                transaction.AddOutput(transactionOutput);
                transactionOutput.Transaction = transaction;
            }
        }

        /// <summary>
        /// Link transaction inputs to transaction outputs
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="bisqData">BisqData object containing transaction inputs and transaction outputs to be linked</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        private static void LinkInputsToOutputs(LightningTransaction tx, ref BisqData bisqData, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("Linking inputs to outputs...");
            }
            Dictionary<long, TransactionOutput> looseOutputs = new Dictionary<long, TransactionOutput>();
            List<Tuple<byte[], byte[]>> data = ReadDatabase(tx, "tx-in->tx-out");
            foreach (var entry in data)
            {
                long inputId = BitConverter.ToInt64(entry.Item1, 0);
                long outputId = BitConverter.ToInt64(entry.Item2, 0);
                TransactionInput input = bisqData.TransactionInputs[inputId];
                if(bisqData.TransactionOutputs.ContainsKey(outputId))
                {
                    input.SetOutput(bisqData.TransactionOutputs[outputId]);
                    bisqData.TransactionOutputs[outputId].SetInput(input);
                }
                else
                {
                    TransactionOutput looseOutput = new TransactionOutput(outputId);
                    looseOutput.SetInput(input);
                    looseOutputs.Add(outputId, looseOutput);
                    input.SetOutput(looseOutput);
                }
            }
            bisqData.LooseOutputs = looseOutputs;
            if (verbose)
            {
                Console.WriteLine("Non-existent / Loose output count: " + looseOutputs.Count);
            }
        }
        #endregion

        /// <summary>
        /// Load clusters from LMDB environment
        /// NOTE: These aren't the clusters generated using Bisq DAO specific clustering heuristic,
        /// rather, these are clusters applying to the broader Bitcoin blockchain using multi-input </summary>
        /// heuristic to be used for comparison purposes
        /// <param name="envFilePath">File path for LMDB file</param>
        /// <param name="maxDbs">Maximum allowed databses for LMDB environment</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <param name="bd">BisqData object containing all Bisq txes</param>
        /// <returns></returns>
        public static ClusterData LoadClusterData(string envFilePath, int maxDbs, bool verbose, BisqData bd)
        {
            ClusterData clusterData = new ClusterData();
            LightningEnvironment env = new LightningEnvironment(envFilePath);
            env.MaxDatabases = maxDbs;
            env.Open(EnvironmentOpenFlags.NoSubDir | EnvironmentOpenFlags.ReadOnly);
            if (!env.IsOpened)
            {
                Console.WriteLine(string.Format("Error: Failed to Open LMDB Environment at Location {0}", envFilePath));
            }
            else
            {
                using (LightningTransaction tx = env.BeginTransaction(TransactionBeginFlags.ReadOnly))
                {
                    Console.Write("Sucessfully Opened Cluster LMDB Environment, Loading...");
                    Dictionary<long, byte[]> addressIds = GetBisqClusterAddresses(tx, verbose, bd);
                    clusterData.Addresses = LoadAddresses(tx, verbose, addressIds);
                    clusterData.Clusters = LoadClusters(tx, verbose, addressIds, clusterData);
                    tx.Dispose();
                }
                Console.WriteLine("Complete");
            }
            env.Dispose();
            return clusterData;
        }

        /// <summary>
        /// This function is used to assign addresses to BSQ tx inputs and tx outputs, basically, we retrieve BSQ 
        /// addresses using this method
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <param name="bd">BisqData object containing all Bisq txes</param>
        /// <returns></returns>
        private static Dictionary<long, byte[]> GetBisqClusterAddresses(LightningTransaction tx, bool verbose, BisqData bd)
        {
            // This function hinges on reationship between tx-outs and addresses
            Dictionary<long, byte[]> addressIds = new Dictionary<long, byte[]>();
            foreach (var txOut in bd.TransactionOutputs.Values)
            {
                byte[] addrBytes = GetValueByKey(tx, BitConverter.GetBytes(txOut.Id), "tx-out->addrs");
                if(null != addrBytes) // TODO: This can often be null, look into this
                {
                    long addressId = BitConverter.ToInt64(addrBytes);
                    addressIds[addressId] = addrBytes;
                    txOut.AddressId = addressId;
                }
            }
            foreach(var txIn in bd.TransactionInputs.Values)
            {
                var txOut = txIn.TransactionOutput;
                if(null != txOut)
                {
                    byte[] addrBytes = GetValueByKey(tx, BitConverter.GetBytes(txOut.Id), "tx-out->addrs");
                    if (null != addrBytes)
                    {
                        long addressId = BitConverter.ToInt64(addrBytes);
                        addressIds[addressId] = addrBytes;
                        txOut.AddressId = addressId;
                    }
                }
            }
            foreach(var looseOutput in bd.LooseOutputs)
            {
                byte[] addrBytes = GetValueByKey(tx, BitConverter.GetBytes(looseOutput.Key), "tx-out->addrs");
                if (null != addrBytes)
                {
                    long addressId = BitConverter.ToInt64(addrBytes);
                    addressIds[addressId] = addrBytes;
                }
            }
            return addressIds;
        }

        #region Address_Loading
        /// <summary>
        /// Function loads all address data from LMDB
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <param name="addressBytes">Dictionary in which addresses are stored</param>
        /// <returns></returns>
        private static Dictionary<long, Address> LoadAddresses(LightningTransaction tx, bool verbose, Dictionary<long, byte[]> addressBytes)
        {
            Dictionary<long, Address> addresses = new Dictionary<long, Address>();
            long number = 0;
            foreach(var address in addressBytes)
            {
                long id = address.Key;
                byte[] hash = GetValueByKey(tx, address.Value, "addr->hash");
                byte type = GetValueByKey(tx, address.Value, "addr->type")[0];
                Address newAddress = new Address(number, id, hash, (byte)type);
                addresses.Add(id, newAddress);
                ++number;
            }
            return addresses;
        }

        /// <summary>
        /// Function loads clusters from LMDB cluster environment and links addresses to clusters, 
        /// NOTE: These aren't the clusters generated using Bisq DAO specific clustering heuristic,
        /// rather, these are clusters applying to the broader Bitcoin blockchain using multi-input 
        /// heuristic to be used for comparison purposes
        /// </summary>
        /// <param name="tx">LMDB transaction with which to read data</param>
        /// <param name="verbose">Boolean representing whether verbose output should be written to the console</param>
        /// <param name="addressBytes">Dictionary in which addresses are stored</param>
        /// <param name="clusterData">ClusterData object containing cluster information</param>
        /// <returns></returns>
        private static Dictionary<long, Cluster> LoadClusters(LightningTransaction tx, bool verbose, Dictionary<long, byte[]> addressBytes, ClusterData clusterData)
        {
            Dictionary<long, Cluster> clusters = new Dictionary<long, Cluster>();
            long number = 0;
            foreach(var address in addressBytes)
            {
                byte[] clusterIdBytes = GetValueByKey(tx, address.Value, "addr->cluster");
                long id = BitConverter.ToInt64(clusterIdBytes);
                if(!clusters.ContainsKey(id))
                {
                    long size = BitConverter.ToInt64(GetValueByKey(tx, clusterIdBytes, "cluster->size"));
                    if(0 != size)
                    {
                        Cluster cluster = new Cluster(number, id, size);
                        cluster.Addresses.Add(address.Key, clusterData.Addresses[address.Key]);
                        clusters.Add(id, cluster);
                        clusterData.Addresses[address.Key].Cluster = cluster;
                        ++number;
                    }
                }
                else
                {
                    clusters[id].Addresses.Add(address.Key, clusterData.Addresses[address.Key]);
                    clusterData.Addresses[address.Key].Cluster = clusters[id];
                }
            }
            return clusters;
        }
        #endregion
    }
}
