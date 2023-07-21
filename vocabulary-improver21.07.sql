-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3306
-- Время создания: Июл 21 2023 г., 11:56
-- Версия сервера: 8.0.30
-- Версия PHP: 8.1.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `vocabulary-improver`
--

-- --------------------------------------------------------

--
-- Структура таблицы `dictionaries`
--

CREATE TABLE `dictionaries` (
  `Id` int UNSIGNED NOT NULL,
  `Guid` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `UserGuid` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `dictionaries`
--

INSERT INTO `dictionaries` (`Id`, `Guid`, `Name`, `UserGuid`) VALUES
(1, 'e9ffae28-8959-4878-863a-8407f4c58786', 'slovarik', 'be14da61-d87f-406b-9f40-7d16fcb8f355'),
(3, '0197e4c1-ab19-43a4-8d75-0c7866755ea0', 'dict3', 'ca57fa5c-0593-414a-adeb-32eff24424fb'),
(4, '373c43bb-9c67-4b9b-9d43-e04bf9475b16', 'megapon', 'be14da61-d87f-406b-9f40-7d16fcb8f355'),
(6, '2b383b60-0bf0-45cf-bdd4-e12f72afeb56', 'Мой Словарик', '78add685-eec6-4e00-a013-691f17d97ea8');

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `dictsview`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `dictsview` (
`DictName` varchar(255)
,`DictGuid` char(36)
,`WordsCount` bigint
,`Firstname` varchar(255)
,`TelegramId` bigint unsigned
,`Username` varchar(255)
,`UserGuid` char(36)
);

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `Id` int UNSIGNED NOT NULL,
  `Guid` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Firstname` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Discriminator` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Hash` char(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TelegramId` bigint UNSIGNED DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`Id`, `Guid`, `Firstname`, `Discriminator`, `Username`, `Email`, `Hash`, `TelegramId`) VALUES
(1, 'ca57fa5c-0593-414a-adeb-32eff24424fb', 'Masha', 'RegistredUser', 'maria', 'masha@mylo.ru', '$2a$11$yQwA0m6Xgdt0ybirGd/1Y.wPRCKVgtGKVS0SYkTYAEfMimbwlC8gW', NULL),
(2, '6a15bf51-1e74-401b-938b-d71051ec8008', 'Nikolas', 'RegistredUser', 'kolyan', 'kolya@mylo.ru', '423472346238423', NULL),
(3, 'be14da61-d87f-406b-9f40-7d16fcb8f355', 'dima', 'TelegramUser', NULL, NULL, NULL, 777),
(4, '9a577810-eaad-4cee-9cf0-44966eac8ed3', 'diva', 'TelegramUser', NULL, NULL, NULL, 365756812),
(5, '39a40bae-ed12-4a0a-843e-82dc705de877', 'Кузя', 'TelegramUser', NULL, NULL, NULL, 124234231),
(6, '2bcfd3ed-af82-4c45-8bdf-2bfc0dd361ce', 'Masha', 'RegistredUser', 'mariakiller', 'samd@fs.ru', '$2a$11$lKI51L11LaGolMiKt9prh.B3NudnfTGbr5aiATNk.z3cHGdbdN1GW', NULL),
(7, '78add685-eec6-4e00-a013-691f17d97ea8', 'Roman', 'RegistredUser', 'pussykiller', 'romik@kok.ru', '$2a$11$tdPCO/.dZKvLU9WTqyNFr.e2LAha6PJveQhxBvhKtl.DnymvfeW0C', NULL);

-- --------------------------------------------------------

--
-- Структура таблицы `words`
--

CREATE TABLE `words` (
  `Id` int UNSIGNED NOT NULL,
  `Guid` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SourceWord` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TargetWord` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Rating` int NOT NULL DEFAULT '0',
  `DictionaryGuid` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL
) ;

--
-- Дамп данных таблицы `words`
--

INSERT INTO `words` (`Id`, `Guid`, `SourceWord`, `TargetWord`, `Rating`, `DictionaryGuid`) VALUES
(1, '172b0852-2523-4e9d-959f-349a94c43e4c', 'figure out', 'разобраться', 10, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(2, '824439a4-fea8-48fb-8ee9-73d316a3b3da', 'as well', 'а также', 5, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(3, 'ba3aa53f-1688-4c08-b670-4602c6eda71b', 'on-demand', 'по требованию', 9, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(4, 'f88f354e-52ef-4b93-9156-815bd643341c', 'distraction', 'отвлечение', 9, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(5, '564b5512-3692-4959-8d8c-cd30bb0db45b', 'up to date', 'актуальный', 8, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(9, '95b7eb62-5d3f-4beb-b688-6fad298c723e', 'due to', 'благодаря ', 2, '0197e4c1-ab19-43a4-8d75-0c7866755ea0'),
(11, '5008a993-db22-42e5-8a09-4b41bf5f6b0b', 'put it another way', 'другими словами', 6, '0197e4c1-ab19-43a4-8d75-0c7866755ea0'),
(12, '16fce7eb-83ea-4132-a633-37237d1c6bdb', 'referred to as', 'называется', 4, '0197e4c1-ab19-43a4-8d75-0c7866755ea0'),
(17, '6c736e04-06a8-490c-8fb1-ae90cdc0d734', 'test', 'test', 0, 'e9ffae28-8959-4878-863a-8407f4c58786'),
(18, '9edb683f-d882-4517-9049-f110045f4aa3', 'качалка', 'gym', 0, '2b383b60-0bf0-45cf-bdd4-e12f72afeb56');

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `wordsview`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `wordsview` (
`WordGuid` char(36)
,`SourceWord` varchar(512)
,`TargetWord` varchar(512)
,`DictinaryGuid` char(36)
,`DictionaryName` varchar(255)
,`UserGuid` char(36)
,`Firstname` varchar(255)
);

-- --------------------------------------------------------

--
-- Структура для представления `dictsview`
--
DROP TABLE IF EXISTS `dictsview`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `dictsview`  AS SELECT `dictionaries`.`Name` AS `DictName`, `dictionaries`.`Guid` AS `DictGuid`, (select count(0) from `words` where (`words`.`DictionaryGuid` = `dictionaries`.`Guid`)) AS `WordsCount`, `users`.`Firstname` AS `Firstname`, `users`.`TelegramId` AS `TelegramId`, `users`.`Username` AS `Username`, `dictionaries`.`UserGuid` AS `UserGuid` FROM (`dictionaries` join `users` on((`users`.`Guid` = `dictionaries`.`UserGuid`)))  ;

-- --------------------------------------------------------

--
-- Структура для представления `wordsview`
--
DROP TABLE IF EXISTS `wordsview`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`%` SQL SECURITY DEFINER VIEW `wordsview`  AS SELECT `words`.`Guid` AS `WordGuid`, `words`.`SourceWord` AS `SourceWord`, `words`.`TargetWord` AS `TargetWord`, `words`.`DictionaryGuid` AS `DictinaryGuid`, `dictionaries`.`Name` AS `DictionaryName`, `dictionaries`.`UserGuid` AS `UserGuid`, `users`.`Firstname` AS `Firstname` FROM ((`words` join `dictionaries` on((`dictionaries`.`Guid` = `words`.`DictionaryGuid`))) join `users` on((`users`.`Guid` = `dictionaries`.`UserGuid`))) ORDER BY `dictionaries`.`UserGuid` ASC  ;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `dictionaries`
--
ALTER TABLE `dictionaries`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `GuidKey` (`Guid`),
  ADD KEY `userguid-unique` (`UserGuid`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `GuidKey` (`Guid`),
  ADD UNIQUE KEY `email-unique` (`Email`),
  ADD UNIQUE KEY `tgId-unique` (`TelegramId`),
  ADD UNIQUE KEY `username-unique` (`Username`);

--
-- Индексы таблицы `words`
--
ALTER TABLE `words`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `GuidKey` (`Guid`),
  ADD KEY `dictguid-unique` (`DictionaryGuid`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `dictionaries`
--
ALTER TABLE `dictionaries`
  MODIFY `Id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `Id` int UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT для таблицы `words`
--
ALTER TABLE `words`
  MODIFY `Id` int UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `dictionaries`
--
ALTER TABLE `dictionaries`
  ADD CONSTRAINT `DictionaryToUser` FOREIGN KEY (`UserGuid`) REFERENCES `users` (`Guid`) ON DELETE CASCADE;

--
-- Ограничения внешнего ключа таблицы `words`
--
ALTER TABLE `words`
  ADD CONSTRAINT `WordToDictionary` FOREIGN KEY (`DictionaryGuid`) REFERENCES `dictionaries` (`Guid`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
