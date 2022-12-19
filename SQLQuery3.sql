SELECT TOP(1) [l].[ListingID], [l].[ClaimedByID], [l].[ConditionID], [l].[CreatedByID], [l].[Description], [l].[ListingDTOListingID], [l].[Quantity], [l].[StatusID], [l].[StoreID], [l].[TypeID], [c].[CustomerID], [c].[Email], [c].[Name]
FROM [Listings] AS [l]
INNER JOIN [Customers] AS [c] ON [l].[CreatedByID] = [c].[CustomerID]
WHERE ([c].[CustomerID] = 2)  AND ([l].[ListingID] = 2)
