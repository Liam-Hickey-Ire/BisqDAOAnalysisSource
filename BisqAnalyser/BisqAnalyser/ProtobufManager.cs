using System.IO;
using Google.Protobuf;
using Io.Bisq.Protobuffer;

namespace BisqAnalyser
{
    class ProtobufManager
    {
        string m_dataPath;
        string m_legacyDataPath;

        public ProtobufManager(string filePath, string legacyFilePath)
        {
            m_dataPath = filePath;
            m_legacyDataPath = legacyFilePath;
        }

        public PersistableEnvelope Load(string filename, bool legacy = false)
        {
            using (var input = File.OpenRead(Path.Combine(legacy ? m_legacyDataPath : m_dataPath, filename)))
            {
                PersistableEnvelope persistableEnvelope = new PersistableEnvelope();
                persistableEnvelope.MergeDelimitedFrom(input);
                return persistableEnvelope;
            }
        }

        public BallotList LoadBallotList()
        {
            return Load("BallotList").BallotList;
        }

        public BlindVoteStore LoadBlindVoteStore()
        {
            return Load("BlindVoteStore").BlindVoteStore;
        }

        public DaoStateStore LoadDaoStateStore()
        {
            return Load("DaoStateStore").DaoStateStore;
        }

        public NavigationPath LoadNavigationPath()
        {
            return Load("NavigationPath").NavigationPath;
        }

        public ProposalStore LoadProposalStore()
        {
            return Load("ProposalStore").ProposalStore;
        }

        public TempProposalStore LoadTempProposalStore()
        {
            return Load("TempProposalStore").TempProposalStore;
        }

        public TradeStatistics2Store LoadTradeStatictics2Store()
        {
            return Load("TradeStatistics2Store", true).TradeStatistics2Store;
        }

        public TradeStatistics3Store LoadTradeStatistics3Store()
        {
            // Retrieve trades
            TradeStatistics3Store tradeStatistics3_Base = Load("TradeStatistics3Store").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_140 = Load("TradeStatistics3Store_1.4.0").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_150 = Load("TradeStatistics3Store_1.5.0").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_152 = Load("TradeStatistics3Store_1.5.2").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_155 = Load("TradeStatistics3Store_1.5.5").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_157 = Load("TradeStatistics3Store_1.5.7").TradeStatistics3Store;
            TradeStatistics3Store tradeStatistics3_160 = Load("TradeStatistics3Store_1.6.0").TradeStatistics3Store;

            // Concatenate trades
            TradeStatistics3Store tradeStatistics3Store = tradeStatistics3_Base;
            tradeStatistics3Store.Items.Add(tradeStatistics3_140.Items);
            tradeStatistics3Store.Items.Add(tradeStatistics3_150.Items);
            tradeStatistics3Store.Items.Add(tradeStatistics3_152.Items);
            tradeStatistics3Store.Items.Add(tradeStatistics3_155.Items);
            tradeStatistics3Store.Items.Add(tradeStatistics3_157.Items);
            tradeStatistics3Store.Items.Add(tradeStatistics3_160.Items);

            return tradeStatistics3Store;
        }

        public UnconfirmedBsqChangeOutputList LoadUnconfirmedBsqChangeOutputList()
        {
            return Load("UnconfirmedBsqChangeOutputList").UnconfirmedBsqChangeOutputList;
        }

        public AccountAgeWitnessStore LoadAccountAgeWitnessStore()
        {
            return Load("AccountAgeWitnessStore").AccountAgeWitnessStore;
        }

        public AddressEntryList LoadAddressEntryList()
        {
            return Load("AddressEntryList").AddressEntryList;
        }

        public PeerList LoadPeerList()
        {
            return Load("PeerList").PeerList;
        }

        public PreferencesPayload LoadPreferencesPayload()
        {
            return Load("PreferencesPayload").PreferencesPayload;
        }

        public SequenceNumberMap LoadSequenceNumberMap()
        {
            return Load("SequenceNumberMap").SequenceNumberMap;
        }

        public SignedWitnessStore LoadSignedWitnessStore()
        {
            return Load("SignedWitnessStore").SignedWitnessStore;
        }

        public UserPayload LoadUserPayload()
        {
            return Load("UserPayload").UserPayload;
        }
    }
}
