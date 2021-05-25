SELECT CONVERT(DATE, DATEADD(S, OfferDate / 1000, '1-1-1970 00:00:00')) AS Date, SUM(TradeAmount) / COUNT(TradeAmount) AS TotalTradeValue 
FROM trade2Statistics 
WHERE CounterCurrency = 'USD'
GROUP BY CONVERT(DATE, DATEADD(S, OfferDate / 1000, '1-1-1970 00:00:00')) 
ORDER BY Date