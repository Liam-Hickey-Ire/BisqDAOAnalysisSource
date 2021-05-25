SELECT BaseCurrency, COUNT(BaseCurrency) as Count FROM trade2Statistics 
WHERE BaseCurrency <> 'BTC' 
GROUP By BaseCurrency