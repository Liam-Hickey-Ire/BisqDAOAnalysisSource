@ECHO OFF
ECHO Genrating Graphs...
gnuplot TradesOverTime.script
gnuplot IssuanceOverBlockHeight.script
gnuplot TradePriceOverTimeEuro.script
gnuplot TradePriceOverTimeDollar.script
gnuplot CounterCurrenciesPopularity.script
gnuplot BaseCurrenciesPopularity.script
gnuplot TradeValueOverTime.script
gnuplot PaymentMethodPopularity.script
ECHO Complete, plotted graphs now saved as PNG files, press any key to close the application