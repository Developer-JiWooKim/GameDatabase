CREATE TABLE IF NOT EXISTS Inventory(
	playerId INTEGER NOT NULL,
	itemId INTEGER NOT NULL,
	quantity INTEGER NOT NULL CHECK (quantity >= 0),
	PRIMARY KEY (playerId, itemId),
	FOREIGN KEY (PlayerId) REFERENCES Player(PlayerId),
	FOREIGN KEY (itemId) REFERENCES Item(itemId)
);
	