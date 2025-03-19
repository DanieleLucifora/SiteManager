USE SiteManager;

CREATE TABLE cantieri (
    IdCantiere INT AUTO_INCREMENT PRIMARY KEY,
    Citta VARCHAR(100) NOT NULL,
    Committente VARCHAR(100) NOT NULL,
    DataInizio DATE NOT NULL,
    Scadenza DATE NOT NULL
);

CREATE TABLE operai (
    IdOperaio INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(30) NOT NULL,
    Cognome VARCHAR(30) NOT NULL,
    Mansione VARCHAR(50) NOT NULL,
    CostoOrario DOUBLE NOT NULL,
    DataNascita DATE NOT NULL,
    DataAssunzione DATE NOT NULL,
    CantiereId INT NULL,
    FOREIGN KEY (CantiereId) REFERENCES cantieri(IdCantiere)
);

CREATE TABLE tasks (
    IdTask INT AUTO_INCREMENT PRIMARY KEY,
    Descrizione VARCHAR(255) NOT NULL,
    Data DATE NOT NULL,
    CantiereId INT,
    FOREIGN KEY (CantiereId) REFERENCES cantieri(IdCantiere)
);

CREATE TABLE materiali (
    IdMateriale INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Quantita FLOAT NOT NULL,
    Unita VARCHAR(10) DEFAULT 'kg',
    CostoUnitario DOUBLE NOT NULL,
    CantiereId INT,
    FOREIGN KEY (CantiereId) REFERENCES cantieri(IdCantiere)
);

CREATE TABLE materialecantiere (
    IdMaterialeCantiere INT AUTO_INCREMENT PRIMARY KEY,
    IdMateriale INT,
    IdCantiere INT,
    `QuantitaUtilizzata` FLOAT NOT NULL,
    FOREIGN KEY (IdMateriale) REFERENCES materiali(IdMateriale),
    FOREIGN KEY (IdCantiere) REFERENCES cantieri(IdCantiere)
);

CREATE TABLE spesecantiere (
    IdSpesa INT AUTO_INCREMENT PRIMARY KEY,
    Descrizione VARCHAR(255) NOT NULL,
    Data DATE NOT NULL,
    Costo DOUBLE NOT NULL,
    CantiereId INT,
    FOREIGN KEY (CantiereId) REFERENCES cantieri(IdCantiere)
);

CREATE TABLE presenzecantiere (
    IdPresenza INT AUTO_INCREMENT PRIMARY KEY,
    OperaioId INT,
    Ore DOUBLE NOT NULL,
    Data DATE NOT NULL,
    CantiereId INT,
    FOREIGN KEY (OperaioId) REFERENCES operai(IdOperaio),
    FOREIGN KEY (CantiereId) REFERENCES cantieri(IdCantiere)
);