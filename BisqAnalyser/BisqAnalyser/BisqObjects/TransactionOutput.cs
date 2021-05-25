using System;
using System.Text;
using System.Collections.Generic;
using BisqAnalyser.ClusterObjects;

namespace BisqAnalyser
{
    class TransactionOutput
    {
        #region Members
        private long m_number;
        private long m_id;
        private Transaction m_transaction;
        private TransactionInput m_transactionInput;
        private byte[] m_scriptPubKey;
        private long m_value;
        private byte m_bisqType;
        private long m_addressId; // Used as link between blockchain data and cluster data

        #endregion Members

        #region Methods
        public TransactionOutput(long id)
        {
            m_id = id;
            m_value = -1;
            m_scriptPubKey = null;
            m_bisqType = 200; // This is invalid
            m_transactionInput = null;
            m_addressId = -1; // -1 means that this address ID hasn't been set
        }

        public TransactionOutput(long number, long id, long value, byte[] scriptPubKey, byte bisqType)
        {
            m_number = number;
            m_id = id;
            m_value = value;
            m_scriptPubKey = scriptPubKey;
            m_bisqType = bisqType;
            m_transactionInput = null;
            m_addressId = -1; // -1 means that this address ID hasn't been set
        }

        ~TransactionOutput() { }

        public void SetInput(TransactionInput input)
        {
            m_transactionInput = input;
        }
        #endregion Methods

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

        public TransactionInput TransactionInput
        {
            get { return m_transactionInput; }
            set { m_transactionInput = value; }
        }

        public byte[] ScriptPubKey
        {
            get { return m_scriptPubKey; }
            set { m_scriptPubKey = value; }
        }

        public long Value
        {
            get { return m_value; }
            set { m_value = value; }
        }
        public byte BisqType
        {
            get { return m_bisqType; }
            set { m_bisqType = value; }
        }

        public long AddressId
        {
            get { return m_addressId; }
            set { m_addressId = value; }
        }
        #endregion
    }
}
