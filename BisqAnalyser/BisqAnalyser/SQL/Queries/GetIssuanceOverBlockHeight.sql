SELECT ChainHeight, SUM(Amount) FROM issuance
GROUP BY ChainHeight
ORDER BY ChainHeight