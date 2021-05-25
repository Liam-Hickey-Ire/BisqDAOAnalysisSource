IF OBJECT_ID ('dbo.cycle') IS NULL 
	CREATE TABLE dbo.cycle (
		Id INT NOT NULL PRIMARY KEY,
		CycleNumber VARCHAR(64) NOT NULL,
		Duration INT NOT NULL,
		PhaseOrdinal INT NOT NULL
	)