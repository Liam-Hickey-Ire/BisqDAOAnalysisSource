﻿IF OBJECT_ID ('dbo.cluster') IS NULL 
	CREATE TABLE dbo.cluster (
		Id BIGINT NOT NULL PRIMARY KEY,
		TotalSize BIGINT NOT NULL,
		BisqSize BIGINT NOT NULL,
	)