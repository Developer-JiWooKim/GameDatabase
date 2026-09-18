CREATE TABLE IF NOT EXISTS ItemMaterial(
	itemId INTEGER NOT NULL,
	materialId INTEGER NOT NULL,
	upgradeValue INTEGER NOT NULL CHECK ( upgradeValue > 0),
	PRIMARY KEY (itemId, materialId),
	FOREIGN KEY (itemId) REFERENCES Item(itemId),
	FOREIGN KEY (materialId) REFERENCES Material(materialId)
);