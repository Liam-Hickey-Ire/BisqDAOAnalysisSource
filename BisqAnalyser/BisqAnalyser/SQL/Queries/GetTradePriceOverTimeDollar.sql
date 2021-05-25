SELECT CONVERT(DATE, DATEADD(S, OfferDate / 1000, '1-1-1970 00:00:00')) AS Date, (SUM(TradePrice) / COUNT(TradePrice)) AS AveragePrice FROM trade2Statistics
WHERE BaseCurrency = 'BTC'
AND CounterCurrency = 'USD'
GROUP BY CONVERT(DATE, DATEADD(S, OfferDate / 1000, '1-1-1970 00:00:00'))
ORDER BY Date