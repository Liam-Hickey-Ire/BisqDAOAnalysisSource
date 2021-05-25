INSERT INTO dbo.block (Id, Hash, Timestamp, PriceOpen, PriceHigh, PriceLow, PriceClose, PriceTimestamp) 
VALUES
(@id, @hash, @timestamp, @priceopen, @pricehigh, @pricelow, @priceclose, @pricetimestamp)