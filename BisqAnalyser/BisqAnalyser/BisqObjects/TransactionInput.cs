using System;
using System.Collections.Generic;
using System.Text;

namespace BisqAnalyser
{
    class TransactionInput
    {
        private long m_number;
        private long m_id;
        private Transaction m_transaction;
        private TransactionOutput m_transactionOutput;
        private byte[] m_scriptSig;
        private long m_seqNum;

        public TransactionInput(long number, long id, byte[] scriptSig, long seqNum)
        {
            m_number = number;
            m_id = id;
            m_scriptSig = scriptSig;
            m_seqNum = seqNum;
            m_transactionOutput = null;
        }

        ~TransactionInput() { }

        public void SetOutput(TransactionOutput output)
        {
            m_transactionOutput = output;
        }

        #region Properties
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

        public Transaction Transaction
        {
            get { return m_transaction; }
            set { m_transaction = value; }
        }

        public TransactionOutput TransactionOutput
        {
            get { return m_transactionOutput; }
            set { m_transactionOutput = value; }
        }

        public byte[] ScriptSig
        {
            get { return m_scriptSig; }
            set { m_scriptSig = value; }
        }

        public long SeqNum
        {
            get { return m_seqNum; }
            set { m_seqNum = value; }
        }
        #endregion
    }
}
