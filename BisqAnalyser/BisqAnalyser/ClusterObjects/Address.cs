using System;
using System.Collections.Generic;
using System.Text;

namespace BisqAnalyser.ClusterObjects
{
    class Address
    {
        // Members
        private long m_number;
        private long m_id;
        private byte[] m_hash;
        private byte m_type;
        private long m_totalValue;
        private Cluster m_cluster;

        #region Methods
        // Default Constructor
        public Address() {}

        // Constructor
        public Address(long number, long id, byte[] hash, byte type)
        {
            m_number = number;
            m_id = id;
            m_hash = hash;
            m_type = type;
            m_totalValue = 0;
        }

        ~Address() { }
        #endregion

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

        public byte[] Hash
        {
            get { return m_hash; }
            set { m_hash = value; }
        }

        public byte Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public long TotalValue
        {
            get { return m_totalValue; }
            set { m_totalValue = value; }
        }

        public Cluster Cluster
        {
            get { return m_cluster; }
            set { m_cluster = value; }
        }
        #endregion
    }
}
