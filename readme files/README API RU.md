# Работа с API

## Введение
Каждый HTTP метод в случае корректного запроса возвращает json типа ViApiResponse\<T>, где T - value.

Пример метода POST localhost/api/register:

    {
        "success": true,
        "value": {
            "username": "test",
            "email": "test@mail.ru",
            "firstname": "Mr.Test",
            "password": "Password12345!&$"
        },
        "description": "Регистрация прошла успешно"
    }

Если что-то пошло не так, то поле "success" будет иметь значение "false", например:

    {
        "success": false,
        "value": {
            "username": "test",
            "email": "test@mail.ru",
            "firstname": "Mr.Test",
            "password": "Password12345!&$"
        },
        "description": "Регистрация не прошла, с таким email и/или username пользователь уже зарегистрирован"
    }

При этом HTTP код будет 400.

## Работа с пользователем

### Регистрация
**POST /api/register**, запрос в теле должен содержать следующие поля:

    {
        "Username": "test",
        "Email": "test@mail.ru",
        "Firstname": "Mr.Test",
        "Password": "Password12345!&$" 
}

### Авторизация

Авторизация пользователя происходит либо с помощью Cookie-файлов, либо с помощью JWT-токена, который должен быть в заголовке Authorization с схемой Bearer. 

Запрос в теле должен содержать json с полем username (или email) и password, для получения доступа.

**Результаты запросов ниже:**

**GET localhost/api/login/cookie**

    {
        "success": true,
        "value": "Успех!",
        "description": "В ваших куках появилась запись, вы можете пользоваться сервисом. Кука удалится при неактивности в течение получаса"
    }

**GET localhost/api/login/jwt**

    {
        "success": true,
        "value": {
            "jwtToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQ0ZTdjZTdhLTc1NDUtNDIzZi04OTM3LWNlNDBlNTI5MmJkYSIsImV4cCI6MTY5NDcxODQyOCwiaXNzIjoiVmlBcGlTZXJ2ZXIiLCJhdWQiOiJWaUFwaUNsaWVudCJ9.sfybe0R5LZdXYdkGvIB9Qx3wJVH8Gk4eqE_qW1pHxso"
        },
        "description": "Вы залогинились, вот ваш токен"
    }

**Обратите внимание, все дальнейшие запросы требуют чтобы пользователь был авторизован через Cookie или JWT Bearer.**

### Обновление учетных данных пользователя

Для обновления учетных данных необходимо подтвердить личность пользователя с помощью электронной почты (на данный момент времени, отправка email симулирована отправкой сообщения в консоль), на которую приходит JWT-токен, который действителен **2 часа**. Он, в свою очередь, должен быть в строке запроса как аргумент approvailJwtToken, а также в теле запроса должен быть json объект с данными пользователя с такими же полями, как вы вводили при регистрации. Пример далее:

Получение токена подтверждения запросом **GET localhost/api/user/getapproval**. Результат: 

    {
        "success": true,
        "value": "**st@mail.ru",
        "description": "Письмо отправлено на адрес в поле value"
    }

**POST localhost/api/user/update?approvailJwtToken={YOUR_TOKEN}**. Результат:

    {
        "success": false,
        "value": "OK",
        "description": "Данные обновлены"
    }

### Удаление пользователя и всей информации о нем

Для удаления нужно тоже получить JWT-токен подтверждения, как сделано было в примере выше.

**GET localhost/api/user/deleteme?approvailJwtToken={YOUR_TOKEN}** Результат:

    {
        "success": false,
        "value": "OK",
        "description": "Пользователь и все данные о нем удалены"
    }

## Работа со словарями

### Добавить словарь
**GET localhost/api/dict/add?newName={DICTIONRAY_NAME}**

### Получение списка словарей с их Guid
**GET localhost/api/dict/getlist**

### Переименовать словарь
**GET localhost/api/dict/rename?dictGuid={DICTIONARY_GUID}&newName={NEW_NAME}**

### Удалить словарь

**GET localhost/api/dict/delete?dictGuid={DICTIONARY_GUID}**

## Работа со словами

### Добавление
Чтобы добавить словосочетание, нужно отправить запрос **POST localhost/api/words/add** с телом:

    {
        "SourceWord": "hello",
        "TargetWord": "привет",
        "DictionaryGuid": "{DICTIONARY_GUID}"
    }

### Добавление списка слов
Чтобы добавить несколько словосочетаний, нужно отправить запрос **POST localhost/api/words/addwordlist?dictGuid={DICTIONARY_GUID}** с массивом json:

    [
        {
            "sourceWord": "hello",
            "targetWord": "привет"
        },
        {
            "sourceWord": "goodbye",
            "targetWord": "до свидания"
        }, 
        {
            "sourceWord": "world",
            "targetWord": "мир"
        }
    ]

### Просмотр списка слов

**GET localhost/api/words/getlist?dictGuid={DICTIONARY_GUID}**

### Удаление слова

**GET localhost/api/words/delete?wordGuid={WORD_GUID}**

### Изменение рейтинга слова
Для изменения рейтинга надо отправить запрос

Для увеличения рейтинга: **GET localhost/api/words/updaterating?ratingAction=Increase**
Для Уменьшения рейтинга: **GET localhost/api/words/updaterating?ratingAction=DEcrease** 
с телом:

    {
    "Guid": "{WORD_GUID}",
    "DictionaryGuid": "{DICTIONARY_GUID}"
    }


