SELECT CounterCurrency, COUNT(CounterCurrency) as Count FROM trade2Statistics 
WHERE CounterCurrency <> 'BTC' 
GROUP By CounterCurrency