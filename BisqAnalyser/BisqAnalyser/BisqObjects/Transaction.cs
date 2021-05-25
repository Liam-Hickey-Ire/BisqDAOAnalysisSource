using System.Collections.Generic;

namespace BisqAnalyser
{
    class Transaction
    {
        #region Members
        // Members
        private long m_number;
        private long m_id;
        private Block m_block;
        private string m_hash;
        private byte m_version;
        private long m_lockTime;
        private byte m_bisqType;
        private byte m_bisqProposalAccepted;
        private List<TransactionInput> m_inputs;
        private List<TransactionOutput> m_outputs;
        #endregion

        #region Methods
        // Methods
        public Transaction(long number, long id, string hash, byte version, long lockTime, byte bisqType, byte bisqProposalAccepted)
        {
            m_number = number;
            m_id = id;
            m_hash = hash;
            m_version = version;
            m_lockTime = lockTime;
            m_bisqType = bisqType;
            m_bisqProposalAccepted = bisqProposalAccepted;
            m_inputs = new List<TransactionInput>();
            m_outputs = new List<TransactionOutput>();
        }

        ~Transaction() { }

        public void AddInput(TransactionInput input)
        {
            m_inputs.Add(input);
        }

        public void AddOutput(TransactionOutput output)
        {
            m_outputs.Add(output);
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

        public byte Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        public long LockTime
        {
            get { return m_lockTime; }
            set { m_lockTime = value; }
        }

        public byte BisqType
        {
            get { return m_bisqType; }
            set { m_bisqType = value; }
        }

        public byte BisqProposalAccepted
        {
            get { return m_bisqProposalAccepted; }
            set { m_bisqProposalAccepted = value; }
        }

        public Block Block
        {
            get { return m_block; }
            set { m_block = value; }
        }

        public List<TransactionInput> Inputs
        {
            get { return m_inputs; }
        }

        public List<TransactionOutput> Outputs
        {
            get { return m_outputs; }
        }
        #endregion
    }
}
