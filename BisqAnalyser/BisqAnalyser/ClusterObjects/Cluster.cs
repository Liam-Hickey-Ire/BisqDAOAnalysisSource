using System.Collections.Generic;

namespace BisqAnalyser.ClusterObjects
{
    class Cluster
    {
        // Members
        private long m_number;
        private long m_id;
        private long m_size;
        private long m_totalValue;
        private string m_tag;
        private Dictionary<long, Address> m_addresses;

        #region Methods
        // Methods
        public Cluster()
        {
            m_tag = string.Empty;
            m_addresses = new Dictionary<long, Address>();
        }

        public Cluster(long number, long id, long size)
        {
            m_number = number;
            m_id = id;
            m_size = size;
            m_tag = string.Empty;
            m_addresses = new Dictionary<long, Address>();
        }
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

        public long Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public long TotalValue
        {
            get { return m_totalValue; }
            set { m_totalValue = value; }
        }

        public string Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public Dictionary<long, Address> Addresses
        {
            get { return m_addresses; }
            set { m_addresses = value;  }
        }
        #endregion
    }
}
