using System.Collections.Generic;

namespace BisqAnalyser
{
    class BisqData
    {
        private Dictionary<long, Block> m_blocks;
        private Dictionary<long, Transaction> m_transactions;
        private Dictionary<long, TransactionInput> m_transactionInputs;
        private Dictionary<long, TransactionOutput> m_transactionOutputs;
        private Dictionary<long, TransactionOutput> m_looseOutputs;

        public BisqData()
        {
            m_blocks = new Dictionary<long, Block>();
            m_transactions = new Dictionary<long, Transaction>();
            m_transactionInputs = new Dictionary<long, TransactionInput>();
            m_transactionOutputs = new Dictionary<long, TransactionOutput>();
            m_looseOutputs = new Dictionary<long, TransactionOutput>();
        }

        public Dictionary<long, Block> Blocks
        {
            get { return m_blocks; }
            set { m_blocks = value; }
        }

        public Dictionary<long, Transaction> Transactions
        {
            get { return m_transactions; }
            set { m_transactions = value; }
        }

        public Dictionary<long, TransactionInput> TransactionInputs
        {
            get { return m_transactionInputs; }
            set { m_transactionInputs = value; }
        }

        public Dictionary<long, TransactionOutput> TransactionOutputs
        {
            get { return m_transactionOutputs; }
            set { m_transactionOutputs = value; }
        }

        public Dictionary<long, TransactionOutput> LooseOutputs
        {
            get { return m_looseOutputs; }
            set { m_looseOutputs = value; }
        }
    }
}
