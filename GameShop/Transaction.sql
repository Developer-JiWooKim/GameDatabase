BEGIN TRANSACTION;

UPDATE Player
SET Gold = Gold - 30
WHERE playerId = 1 AND GOLD >= 30;

UPDATE Inventory
SET quantity = quantity + 1
WHERE playerId = 1 AND itemId = 2;

COMMIT;