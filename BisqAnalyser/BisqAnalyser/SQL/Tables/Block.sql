﻿IF OBJECT_ID ('dbo.block') IS NULL 
	CREATE TABLE dbo.block (
		Id BIGINT NOT NULL PRIMARY KEY,
		Hash VARCHAR(64) NOT NULL,
		Timestamp BIGINT NOT NULL,
		PriceOpen BIGINT NOT NULL,
		PriceHigh BIGINT NOT NULL,
		PriceLow BIGINT NOT NULL,
		PriceClose BIGINT NOT NULL,
		PriceTimestamp BIGINT NOT NULL
	)