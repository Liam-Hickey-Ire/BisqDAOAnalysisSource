using System.Collections.Generic;

namespace BisqAnalyser
{
    class Block
    {
        // Members
        private long m_number;
        private long m_id;
        private string m_hash;
        private long m_timestamp; // Unix Timestamp
        private PriceInfo m_priceInfo;
        private List<Transaction> m_transactions;

        #region Methods
        // Constructor
        public Block()
        {
            m_transactions = new List<Transaction>();
        }

        public Block(long number, long id, string hash, long timestamp, PriceInfo priceInfo)
        {
            m_transactions = new List<Transaction>();
            m_number = number;
            m_id = id;
            m_hash = hash;
            m_timestamp = timestamp;
            m_priceInfo = priceInfo;
        }

        ~Block() { }

        public void AddTransaction(Transaction transaction)
        {
            m_transactions.Add(transaction);
        }
        #endregion

        #region Properties
        // Properties
        public long Number
        {
            get { return m_number; }
            set { m_number = value; }
        }

        public long Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string Hash
        {
            get { return m_hash; }
            set { m_hash = value; }
        }

        public long Timestamp
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

        public PriceInfo PriceInfo
        {
            get { return m_priceInfo; }
            set { m_priceInfo = value; }
        }

        public List<Transaction> Transactions
        {
            get { return m_transactions; }
        }
        #endregion
    }
}
