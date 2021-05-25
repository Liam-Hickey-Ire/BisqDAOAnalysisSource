using System;
using System.IO;
using System.Data;
using System.Text;

namespace BisqAnalyser
{
    class DataFileManager
    {
        SqlManager m_sqlManager;
        string m_filePath;
        const string m_dataDir = "Data";
        const string m_plotDir = "Plots";

        public DataFileManager(SqlManager sqlManager, string filePath)
        {
            m_sqlManager = sqlManager;
            m_filePath = filePath;
        }

        public void CreateDataFiles()
        {
            Directory.CreateDirectory(Path.Combine(m_filePath, m_dataDir));
            Directory.CreateDirectory(Path.Combine(m_filePath, m_plotDir));
            CreateFile("GetTradesOverTime.sql", "TradesOverTime.dat");
            CreateFile("GetIssuanceOverBlockHeight.sql", "IssuanceOverBlockHeight.dat");
            CreateFile("GetTradeVolumeOverTime.sql", "TradeVolumeOverTime.dat");
            CreateFile("GetTradePriceOverTimeEuro.sql", "TradePriceOverTimeEuro.dat");
            CreateFile("GetTradePriceOverTimeDollar.sql", "TradePriceOverTimeDollar.dat");
            CreateFile("GetTradePriceOverTimeDollar.sql", "TradePriceOverTimeDollar.dat");
            CreateFile("GetBaseCurrencies.sql", "BaseCurrencies.dat");
            CreateFile("GetCounterCurrencies.sql", "CounterCurrencies.dat");
            CreateFile("GetTradeValueOverTime.sql", "TradeValueOverTime.dat");
            CreateFile("GetPaymentMethods.sql", "PaymentMethods.dat");
            PlotGraphs();
        }

        private void CreateFile(string sqlScript, string filename)
        {
            SaveData(m_sqlManager.ExecuteQueryScript("Queries", sqlScript), filename);
        }

        private void SaveData(DataTable dataTable, string filename)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(DataRow row in dataTable.Rows)
            {
                int length = row.ItemArray.Length;
                for(int i = 0; i < length; ++i)
                {
                    // This line is poor, must be a better way of doing this
                    string item = typeof(DateTime) == row.ItemArray[i].GetType() ? ((DateTime)row.ItemArray[i]).Date.ToString("dd/MM/yyyy") : row.ItemArray[i].ToString();
                    stringBuilder.Append(item);
                    if(i + 1 != length)
                    {
                        stringBuilder.Append(' ');
                    }
                }
                stringBuilder.AppendLine(string.Empty);
            }
            File.WriteAllText(Path.Combine(m_filePath, m_dataDir, filename), stringBuilder.ToString());
        }

        private void PlotGraphs()
        {
            Directory.SetCurrentDirectory(m_filePath);
            System.Diagnostics.Process.Start(Path.Combine(m_filePath, "PlotGraphs.bat"));
        }
    }
}
