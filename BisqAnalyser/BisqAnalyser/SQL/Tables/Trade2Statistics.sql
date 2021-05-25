IF OBJECT_ID ('dbo.trade2Statistics') IS NULL 
	CREATE TABLE dbo.trade2Statistics (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARBINARY(2000),
		BaseCurrency VARCHAR(300),
		CounterCurrency VARCHAR(300),
		DepositTxId VARCHAR(300),
		Direction VARCHAR(300),
		OfferAmount BIGINT,
		OfferDate BIGINT,
		OfferId VARCHAR(300),
		OfferMarketPriceMargin FLOAT,
		OfferMinAmount BIGINT,
		OfferUseMarketBasedPrice BIT,
		PaymentMethodId VARCHAR(300),
		TradeAmount BIGINT,
		TradeDate BIGINT,
		TradePrice BIGINT
	)