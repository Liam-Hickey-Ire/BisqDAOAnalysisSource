using System.Collections.Generic;

namespace BisqAnalyser.ClusterObjects
{
    // Stores Bisq related cluster data
    class ClusterData
    {
        private Dictionary<long, Address> m_addresses;
        private Dictionary<long, Cluster> m_clusters;

        public ClusterData()
        {
            m_addresses = new Dictionary<long, Address>();
            m_clusters = new Dictionary<long, Cluster>();
        }

        public Dictionary<long, Address> Addresses
        {
            get { return m_addresses; }
            set { m_addresses = value; }
        }

        public Dictionary<long, Cluster> Clusters
        {
            get { return m_clusters; }
            set { m_clusters = value; }
        }
    }
}
