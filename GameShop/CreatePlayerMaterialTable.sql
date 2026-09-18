CREATE TABLE IF NOT EXISTS PlayerMaterial(
	playerId INTEGER NOT NULL,
	materialId INTEGER NOT NULL,
	quantity INTEGER NOT NULL CHECK (quantity >= 0),
	PRIMARY KEY (playerId, materialId),
	FOREIGN KEY (playerId) REFERENCES Player(playerId),
	FOREIGN KEY (materialId) REFERENCES Material(materialId)	
);