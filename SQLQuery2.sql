CREATE TABLE tuotteet (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi TEXT, hinta INTEGER);

CREATE TABLE asiakkaat (id INTEGER IDENTITY (1,1) PRIMARY KEY, nimi VARCHAR(50), puhelinnumero VARCHAR(50));

INSERT INTO tuotteet (nimi, hinta) VALUES ('maito', 2);

SELECT * FROM asiakkaat;