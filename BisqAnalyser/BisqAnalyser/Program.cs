using BisqAnalyser.ClusterObjects;
using Io.Bisq.Protobuffer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BisqAnalyser
{
    class Program
    {
        static int maxDbs = 0;
        static bool verbose = false;
        static bool loadClusterData = false;
        static bool createGraphs = false;
        static bool insertSqlData = false;
        static bool runClustering = false;

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">Main arguments</param>
        static void Main(string[] args)
        {
            // Get Appsettings
            string lmdbFilePath = ConfigurationManager.AppSettings["bitcoin-bisq-lmdb"];
            string clusterDataFilePath = ConfigurationManager.AppSettings["bitcoin-cluster-lmdb"];
            string protoDataPath = ConfigurationManager.AppSettings["proto-data-path"];
            string legacyProtoDataPath = ConfigurationManager.AppSettings["legacy-proto-data-path"];
            string sqlFilePath = ConfigurationManager.AppSettings["sql-file-path"];
            string sqlConnectionString = ConfigurationManager.AppSettings["connection-string"];
            string gnuplotFilePath = ConfigurationManager.AppSettings["gnuplot-file-path"];
            string tagDbPath = ConfigurationManager.AppSettings["tag-db-path"];

            // Ensure non-string appsettings are parsed
            if(!Int32.TryParse(ConfigurationManager.AppSettings["lmdb-max-dbs"], out maxDbs)
                || !Boolean.TryParse(ConfigurationManager.AppSettings["verbose"], out verbose)
                || !Boolean.TryParse(ConfigurationManager.AppSettings["load-cluster-data"], out loadClusterData)
                || !Boolean.TryParse(ConfigurationManager.AppSettings["create-graphs"], out createGraphs)
                || !Boolean.TryParse(ConfigurationManager.AppSettings["insert-sql-data"], out insertSqlData)
                || !Boolean.TryParse(ConfigurationManager.AppSettings["run-clustering"], out runClustering))
            {
                Console.WriteLine("Error: Failed to parse some appsettings");
                Console.ReadKey();
                return;
            }

            // Load Bisq Data from LMDB
            BisqData bd = LmdbReader.LoadBisqBlockchainData(lmdbFilePath, maxDbs, verbose);

            // Load Cluster Data from LMDB
            ClusterData cd = loadClusterData ? LmdbReader.LoadClusterData(clusterDataFilePath, maxDbs, verbose, bd) : new ClusterData();

            // Load Protobuf Data
            Console.Write("Loading Protobuf Data...");
            ProtobufManager protobufManager = new ProtobufManager(protoDataPath, legacyProtoDataPath);
            DaoStateStore daoStateStore = protobufManager.LoadDaoStateStore(); // This is a very large object
            BlindVoteStore blindVoteStore = protobufManager.LoadBlindVoteStore();
            ProposalStore proposalStore = protobufManager.LoadProposalStore();
            TradeStatistics2Store tradeStatistics2Store = protobufManager.LoadTradeStatictics2Store();

            AccountAgeWitnessStore accountAgeWitnessStore = protobufManager.LoadAccountAgeWitnessStore();
            AddressEntryList addressEntryList = protobufManager.LoadAddressEntryList();
            BallotList ballotList = protobufManager.LoadBallotList();
            NavigationPath navigationPath = protobufManager.LoadNavigationPath();
            PeerList peerList = protobufManager.LoadPeerList();
            PreferencesPayload preferencesPayload = protobufManager.LoadPreferencesPayload();
            SequenceNumberMap sequenceNumberMap = protobufManager.LoadSequenceNumberMap();
            SignedWitnessStore signedWitnessStore = protobufManager.LoadSignedWitnessStore();
            TempProposalStore tempProposalStore = protobufManager.LoadTempProposalStore();
            UserPayload userPayload = protobufManager.LoadUserPayload();

            TradeStatistics3Store tradeStatistics3Store = protobufManager.LoadTradeStatistics3Store();

            Console.WriteLine("Complete");

            if(insertSqlData)
            {
                // Create SQL Tables
                Console.Write("Creating SQL Tables...");
                SqlManager sqlManager = new SqlManager(sqlConnectionString, sqlFilePath);
                sqlManager.DropTables();
                sqlManager.CreateTables();
                Console.WriteLine("Complete");

                // Insert LMDB Data Into SQL
                Console.Write("Inserting LMDB Data into SQL DB...");
                sqlManager.InsertAllBisqRecords(bd);
                sqlManager.InsertAllClusterData(cd);
                Console.WriteLine("Complete");

                // Insert Protobuf Data Into SQL
                Console.Write("Inserting Protobuf Data into SQL DB...");
                sqlManager.BulkInsertBlindVoteStore(blindVoteStore);
                sqlManager.BulkInsertProposalStore(proposalStore);
                sqlManager.BulkInsertTradeStatistics2Store(tradeStatistics2Store);
                sqlManager.BulkInsertDaoStateStore(daoStateStore);

                // Insert Stage 2 Protobuf Data into SQL
                sqlManager.BulkInsertAccountAgeWitnessStore(accountAgeWitnessStore);
                sqlManager.BulkInsertAddressEntryList(addressEntryList);
                sqlManager.BulkInsertNavigationPath(navigationPath);
                sqlManager.BulkInsertPeerList(peerList);
                sqlManager.BulkInsertSequenceNumberMap(sequenceNumberMap);
                sqlManager.BulkInsertSignedWitnessStore(signedWitnessStore);
                Console.WriteLine("Complete");

                // Insert New Protobuf Data into SQL
                sqlManager.BulkInsertTradeStatistics3Store(tradeStatistics3Store);

                // Create Graph Data
                if (createGraphs)
                {
                    Console.Write("Creating data files for graphing...");
                    DataFileManager dataFileManager = new DataFileManager(sqlManager, gnuplotFilePath);
                    dataFileManager.CreateDataFiles();
                    Console.WriteLine("Complete");
                }
            }

            if(runClustering)
            {
                Dictionary<long, Cluster> bisqSpecificClusters = BisqClusterManager.CreateBisqSpecificClusters(bd, cd);
                BisqClusterManager.TagClustersFromProposals(bd, proposalStore);
                BisqClusterManager.TagClustersFromJson(cd, tagDbPath);
            }

            Console.WriteLine("Press any key to close application...");
            Console.ReadKey();
        }
    }
}
