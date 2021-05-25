SELECT convert(date, dateadd(S, OfferDate / 1000, '1-1-1970 00:00:00')) AS Date, 
SUM(TradeAmount) AS TotalTradeAmount,
SUM (TradePrice) AS TotalTradePrice
FROM trade2Statistics 
GROUP BY convert(date, dateadd(S, OfferDate / 1000, '1-1-1970 00:00:00')) 
ORDER BY Date