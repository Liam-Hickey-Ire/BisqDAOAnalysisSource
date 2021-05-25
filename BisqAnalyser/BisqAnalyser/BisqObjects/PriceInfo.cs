using System;
using System.Collections.Generic;
using System.Text;

namespace BisqAnalyser
{
    class PriceInfo
    {
        private long m_priceOpen;
        private long m_priceHigh;
        private long m_priceLow;
        private long m_priceClose;
        private long m_priceTimestamp; // Unix timestamp

        public PriceInfo() { }

        public PriceInfo(long open, long high, long low, long close, long timestamp)
        {
            m_priceOpen = open;
            m_priceHigh = high;
            m_priceLow = low;
            m_priceClose = close;
            m_priceTimestamp = timestamp;
        }

        ~PriceInfo() { }

        public long Open
        {
            get { return m_priceOpen; }
            set { m_priceOpen = value; }
        }

        public long High
        {
            get { return m_priceHigh; }
            set { m_priceHigh = value; }
        }
        public long Low
        {
            get { return m_priceLow; }
            set { m_priceLow = value; }
        }
        public long Close
        {
            get { return m_priceClose; }
            set { m_priceClose = value; }
        }
        public long Timestamp
        {
            get { return m_priceTimestamp; }
            set { m_priceTimestamp = value; }
        }
    }
}
