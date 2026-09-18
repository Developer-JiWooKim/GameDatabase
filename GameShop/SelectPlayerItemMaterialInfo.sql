SELECT p.name AS Player, m.name AS Material, pm.quantity, i.name AS item, im.upgradeValue
FROM PlayerMaterial pm
JOIN Player p ON p.playerId = pm.playerId
JOIN Material m ON m.materialId = pm.materialId
JOIN ItemMaterial im ON im.materialId = pm.materialId
JOIN Item i          ON i.itemId = im.itemId
WHERE pm.playerId = 1;