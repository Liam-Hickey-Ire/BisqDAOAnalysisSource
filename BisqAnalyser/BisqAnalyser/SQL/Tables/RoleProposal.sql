IF OBJECT_ID ('dbo.roleProposal') IS NULL 
	CREATE TABLE dbo.roleProposal (
		Id BIGINT NOT NULL PRIMARY KEY,
		RoleUid VARCHAR(200),
		RoleName VARCHAR(200),
		Link VARCHAR(200),
		BondedRoleType VARCHAR(200),
		RequiredBondUnit BIGINT
	)