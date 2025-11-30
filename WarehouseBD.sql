-- Удаление таблиц, если они существуют в БД
-- IF OBJECT_ID('Расход', 'U') IS NOT NULL DROP TABLE Расход;
-- IF OBJECT_ID('Приход', 'U') IS NOT NULL DROP TABLE Приход;
-- IF OBJECT_ID('Товар', 'U') IS NOT NULL DROP TABLE Товар;

-- Таблица Товаров
CREATE TABLE Товар (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Наименование NVARCHAR(255) NOT NULL UNIQUE
);

-- Таблица Прихода
CREATE TABLE Приход (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Дата DATE NOT NULL,
    ТоварID INT NOT NULL,
    Количество INT NOT NULL,
    FOREIGN KEY (ТоварID) REFERENCES Товар(ID)
);

-- Таблица Расхода
CREATE TABLE Расход (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Дата DATE NOT NULL,
    ТоварID INT NOT NULL,
    Количество INT NOT NULL,
    FOREIGN KEY (ТоварID) REFERENCES Товар(ID)
);