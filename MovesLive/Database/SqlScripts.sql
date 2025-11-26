-- Создание таблицы пользователей
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT UNIQUE NOT NULL,
    PasswordHash TEXT NOT NULL
);

-- Создание таблицы фильмов
CREATE TABLE IF NOT EXISTS Movies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Description TEXT,
    Category TEXT,
    Year INTEGER,
    Rating REAL,
    Duration INTEGER,
    PosterPath TEXT,
    StreamingUrl TEXT,
    LocalSourcePath TEXT
);

-- Индексы для ускорения поиска
CREATE INDEX IF NOT EXISTS idx_movies_title ON Movies(Title);
CREATE INDEX IF NOT EXISTS idx_movies_category ON Movies(Category);
CREATE INDEX IF NOT EXISTS idx_movies_year ON Movies(Year);