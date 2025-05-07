# Обновление readme файла от 07.05
До этого я немного сумбурно описала что у меня реализовано, просто оставив эндпоинты внизу, поэтому интересые фичи поясню сейяас
1) Возможность выгружать xlsx отчет по товарам `GET /api/ProductLocations/report`. Ответ приходит в base64, если скопировать строку, закинуть в какой-то парсер в инете, то файл будет выглядеть примерно так:
   ![{650E707D-C649-40D9-8BF3-C948B65BC321}](https://github.com/user-attachments/assets/2b431187-904e-4120-a849-f2a1a8a39c22)
    (Возможно, пявится вопрос: почему печеньки поделены на 2 строчки? Потому что внутри склада есть разные места хранения, но вывести конкретные позиции в отчет я во время хакатона забыла, и увидела только щас 0_0 )
2) Вторая фича, это автогенерация этикеток в пдф для товаров `GET /api/ProductLocations/generate-label/{productId}`  Тоже приходит в base64. (Я сюда хотела добавить еще и qr код, но что есть). Название склада там не отображается, потому что либа через которую я делала не работает с юникодом по умолчанию, это надо ставить шрифты, а делать это в веб апи странно а на использование другой либы уже не было времени на переделки)
   ![{8443FE3D-9CB5-46CD-89FA-2129F855FC54}](https://github.com/user-attachments/assets/aa4e0ef4-d3d1-461f-a6d5-614b02eef1f5)
3) Также у меня есть контроллер InventoryOperations ( у него тоже много эндпоинтов, описаны ниже). Здесь получается информация о товаре, сколько штук, кто и когда, и тип операции (1 или 0 - поступление или убавление). Это типа для отслеживания данных по товарам, история перемешений, действий, предполагалось развивать как что-то типа логгера.
![{2F703726-3086-4D08-B6C7-2EAF890912FE}](https://github.com/user-attachments/assets/d73a9ba7-c92a-4035-8b2e-2e1d8faf336c)
P.s. авторизацию я и вправду брала из своих прошлых работ, где пыталась изучать asp.net.core web api, ну а что поделать, так работают хахатоны
## Впечатления мысли и все такое
Вообще тема достаточно объемная получилось, непонятно было лучше много, но плохо или мало но хорошо. Лучше не сделать вообще базовый функционал, но сделать интересные фичи или все-таки оставить фичи на оставшееся время? Короче на будущее приятнее было бы немного понимать приоритеты оценивания на следующих хахатонах.
В целом было достаточно интересно, я пробовала писать веб апи в августе, когда было свободное время, а дальше получилась ситуация где самостоятельно собирать информацию в инете стало сложно и долго, а времени из-за учебы и работы было мало, так что учавствую ради того, чтобы если попаду на курс собрать знания во что-то целое, разобрать кашу в голове и набраться опыта


# WEB API для системы управления складом
Этот проект представляет собой систему управления складом, разработанную в рамках хакатона. Проект реализован с использованием asp.net core web api. Позволяет управлять товарами, местами хранения товаров и складами с упором на отслеживание запасов. В приложении реализована автоматическая генерация этикеток для товаров, а также возможность выгружать отчет по товарам в видду xlsx. К проекту подключен SwaggerUI поэтому протестировать методы можно на локалхосте перейдя по адресу /swagger/index.html

## Необходимое окружение
В рамках проекта использовалось подключение к базе данных sqlServer строка подключения указана в файле appsettings.json. Также были установлены nuget пакеты, полный список представлен ниже: 
| Пакет | Версия | Команда |
|-------|--------|---------|
|EPPlus |	7.0.0	| dotnet add package EPPlus --version 7.0.0|
|itext7	| 8.0.2	| dotnet add package itext7 --version 8.0.2| 
|itext7.bouncy-castle-adapter|	8.0.2|	dotnet add package itext7.bouncy-castle-adapter --version 8.0.2|
|Microsoft.AspNetCore.Authentication.JwtBearer|	9.0.4	|dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.4|
|Microsoft.AspNetCore.Identity.EntityFrameworkCore|	9.0.4|	dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.4|
|Microsoft.AspNetCore.OpenApi|	9.0.2|	dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.2|
|Microsoft.EntityFrameworkCore|	9.0.4|	dotnet add package Microsoft.EntityFrameworkCore --version 9.0.4|
|Microsoft.EntityFrameworkCore.Design|9.0.4|	dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.4|
|Microsoft.EntityFrameworkCore.SqlServer|	9.0.4|	dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.4|
|Microsoft.Extensions.Identity.Stores|	9.0.4|	dotnet add package Microsoft.Extensions.Identity.Stores --version 9.0.4|
|Swashbuckle.AspNetCore|	8.1.1|	dotnet add package Swashbuckle.AspNetCore --version 8.1.1|

## API Endpoints
### Auth
<details>
  <summary>/Auth/register (POST)</summary>

  Система реализует безопасную аутентификацию и регистрацию с использованием ASP.NET Core Identity и JWT (веб-токены JSON).

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "email": "string",
    "password": "string",
    "firstName": "string",
    "lastName": "string",
    "role": "string"
  }
```

  Доступные значения для свойства роль: Admin, Manager, WarehouseWorker.
</details>
<details>
    <summary>/Auth/login (POST)</summary>
    Для авторизации необходимо передать: 
  
  ```json
{
  "email": "string",
  "password": "string"
}
```
Ответ приходит такого формата: 
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImtyemhrQG1haWwucnUiLCJuYW1laWQiOiIxNjZlOGEwYi01YzE2LTQwYjQtOTlmMS0zMGMwN2Y2YzAyMmYiLCJqdGkiOiIwNjVlZWZiZS04OGY5LTQxMjktYjJiZC1mYTRkNGVhNDdmMjMiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE3NDYzNDQ4MTQsImV4cCI6MTc0NjM1NTYxNCwiaWF0IjoxNzQ2MzQ0ODE0fQ.tcmRcNLK1K8xmHJx71QCJkChp0pDmy68ZoC2KCAF0T0",
  "email": "1111@mail.ru",
  "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
  "roles": [
    "Admin"
  ],
  "expiration": "2025-05-04T10:46:54.4050774Z"
}
```
Пришедший токен необходимо скопировать и указать в сваггере для авторизации в формате "Bearer <токен>"
</details>

### Categories

<details>
  <summary>/Categories (POST)</summary>
  Создает новую категорию товаров. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

Для отправки запроса необходимо передать следующее тело:
```json
{
  "name": "string",
  "description": "string"
}
```

Пример ответа при успешном создании (HTTP 201 Created):
```json
{
  "id": 1,
  "name": "Electronics",
  "description": "Category for electronic devices",
  "products": []
}
```
Если данные категории отсутствуют, возвращается HTTP 400 Bad Request с сообщением: "Category data is null."
</details>
<details>
  <summary>/Categories/{id} (GET)</summary>
  Получает категорию по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

Пример запроса: `GET /api/Categories/1`

Пример ответа при успешном запросе (HTTP 200 OK):
```json
{
  "id": 1,
  "name": "Electronics",
  "description": "Category for electronic devices",
  "products": []
}
```
Если категория не найдена, возвращается HTTP 404 Not Found.
</details>
<details>
  <summary>/Categories (GET)</summary>
  олучает список всех категорий. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

Пример запроса: `GET /api/Categories`

Пример ответа при успешном запросе (HTTP 200 OK):
```json
[
  {
    "id": 1,
    "name": "Electronics",
    "description": "Category for electronic devices",
    "products": []
  },
  {
    "id": 2,
    "name": "Clothing",
    "description": "Category for clothing items",
    "products": []
  }
]
```
</details>

### Inventory Operations

<details>
  <summary>/InventoryOperations (GET)</summary>

  Получает список всех операций с инвентарем. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Пример запроса: `GET /api/InventoryOperations`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
      "operationType": "Add",
      "quantity": 10,
      "date": "2025-05-04T10:00:00Z"
    },
    {
      "id": 2,
      "productId": 6,
      "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
      "operationType": "Remove",
      "quantity": 3,
      "date": "2025-05-04T11:00:00Z"
    }
  ]
  ```

  Если пользователь не авторизован или не имеет нужной роли, возвращается HTTP 401 Unauthorized или HTTP 403 Forbidden.
</details>

<details>
  <summary>/InventoryOperations/{id} (GET)</summary>

  Получает операцию с инвентарем по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/InventoryOperations/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "productId": 5,
    "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
    "operationType": "Add",
    "quantity": 10,
    "date": "2025-05-04T10:00:00Z"
  }
  ```

  Если операция не найдена, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/InventoryOperations/product/{productId} (GET)</summary>

  Получает список операций с инвентарем, связанных с конкретным продуктом. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/InventoryOperations/product/5`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
      "operationType": "Add",
      "quantity": 10,
      "date": "2025-05-04T10:00:00Z"
    }
  ]
  ```
</details>

<details>
  <summary>/InventoryOperations/user/{userId} (GET)</summary>

  Получает список операций с инвентарем, выполненных конкретным пользователем. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Пример запроса: `GET /api/InventoryOperations/user/166e8a0b-5c16-40b4-99f1-30c07f6c022f`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
      "operationType": "Add",
      "quantity": 10,
      "date": "2025-05-04T10:00:00Z"
    }
  ]
  ```

  Если пользователь не авторизован или не имеет нужной роли, возвращается HTTP 401 Unauthorized или HTTP 403 Forbidden.
</details>

<details>
  <summary>/InventoryOperations/my-operations (GET)</summary>

  Получает список операций с инвентарем, выполненных текущим авторизованным пользователем. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/InventoryOperations/my-operations`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
      "operationType": "Add",
      "quantity": 10,
      "date": "2025-05-04T10:00:00Z"
    }
  ]
  ```

  Если пользователь не авторизован, возвращается HTTP 401 Unauthorized.
</details>

<details>
  <summary>/InventoryOperations (POST)</summary>

  Создает новую операцию с инвентарем. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "productId": 5,
    "operationType": "Add",
    "quantity": 10
  }
  ```

  Доступные значения для свойства operationType: Add, Remove.

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 1,
    "productId": 5,
    "userId": "166e8a0b-5c16-40b4-99f1-30c07f6c022f",
    "operationType": "Add",
    "quantity": 10,
    "date": "2025-05-04T10:00:00Z"
  }
  ```

  Возможные ошибки:
  - Если пользователь не авторизован, возвращается HTTP 401 Unauthorized.
  - Если продукт не найден, возвращается HTTP 404 Not Found с сообщением об ошибке.
  - Если операция недопустима (например, недостаточное количество для удаления), возвращается HTTP 400 Bad Request с сообщением об ошибке.
</details>

### Locations

<details>
  <summary>/Locations (GET)</summary>

  Получает список всех локаций. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Locations`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse"
    },
    {
      "id": 2,
      "locationCode": "B-02",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse"
    }
  ]
  ```
</details>

<details>
  <summary>/Locations/{id} (GET)</summary>

  Получает локацию по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Locations/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "locationCode": "A-01",
    "warehouseId": 1,
    "warehouseName": "Main Warehouse"
  }
  ```

  Если локация не найдена, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Locations/warehouse/{warehouseId} (GET)</summary>

  Получает список локаций для конкретного склада. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Locations/warehouse/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse"
    },
    {
      "id": 2,
      "locationCode": "B-02",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse"
    }
  ]
  ```
</details>

<details>
  <summary>/Locations (POST)</summary>

  Создает новую локацию. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "locationCode": "C-03",
    "warehouseId": 1
  }
  ```

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 3,
    "locationCode": "C-03",
    "warehouseId": 1,
    "warehouseName": "Main Warehouse"
  }
  ```

  Если склад не найден, возвращается HTTP 404 Not Found с сообщением об ошибке.
</details>

<details>
  <summary>/Locations/{id} (PUT)</summary>

  Обновляет локацию по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "locationCode": "C-03-Updated"
  }
  ```

  Пример ответа при успешном обновлении (HTTP 204 No Content):

  (Нет тела ответа)

  Если локация не найдена, возвращается HTTP 404 Not Found с сообщением об ошибке.
</details>

<details>
  <summary>/Locations/{id} (DELETE)</summary>

  Удаляет локацию по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для роли Admin.

  Пример запроса: `DELETE /api/Locations/1`

  Пример ответа при успешном удалении (HTTP 204 No Content):

  (Нет тела ответа)

  Если локация не найдена, возвращается HTTP 404 Not Found.
</details>

### ProductLocations

<details>
  <summary>/ProductLocations (GET)</summary>

  Получает список всех локаций продуктов. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "productName": "Laptop",
      "locationId": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 10
    },
    {
      "id": 2,
      "productId": 6,
      "productName": "Phone",
      "locationId": 2,
      "locationCode": "B-02",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 5
    }
  ]
  ```
</details>

<details>
  <summary>/ProductLocations/{id} (GET)</summary>

  Получает локацию продукта по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "productId": 5,
    "productName": "Laptop",
    "locationId": 1,
    "locationCode": "A-01",
    "warehouseId": 1,
    "warehouseName": "Main Warehouse",
    "quantity": 10
  }
  ```

  Если локация продукта не найдена, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/ProductLocations/product/{productId} (GET)</summary>

  Получает список локаций для конкретного продукта. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations/product/5`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "productName": "Laptop",
      "locationId": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 10
    }
  ]
  ```
</details>

<details>
  <summary>/ProductLocations/location/{locationId} (GET)</summary>

  Получает список локаций продуктов для конкретной локации. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations/location/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "productName": "Laptop",
      "locationId": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 10
    }
  ]
  ```
</details>

<details>
  <summary>/ProductLocations/warehouse/{warehouseId} (GET)</summary>

  Получает список локаций продуктов для конкретного склада. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations/warehouse/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "productId": 5,
      "productName": "Laptop",
      "locationId": 1,
      "locationCode": "A-01",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 10
    },
    {
      "id": 2,
      "productId": 6,
      "productName": "Phone",
      "locationId": 2,
      "locationCode": "B-02",
      "warehouseId": 1,
      "warehouseName": "Main Warehouse",
      "quantity": 5
    }
  ]
  ```
</details>

<details>
  <summary>/ProductLocations (POST)</summary>

  Создает новую локацию продукта. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "productId": 5,
    "locationId": 1,
    "quantity": 10
  }
  ```

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 1,
    "productId": 5,
    "productName": "Laptop",
    "locationId": 1,
    "locationCode": "A-01",
    "warehouseId": 1,
    "warehouseName": "Main Warehouse",
    "quantity": 10
  }
  ```

  Если продукт или локация не найдены, возвращается HTTP 404 Not Found с сообщением об ошибке.
</details>

<details>
  <summary>/ProductLocations/{id} (PUT)</summary>

  Обновляет локацию продукта по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "quantity": 15
  }
  ```

  Пример ответа при успешном обновлении (HTTP 204 No Content):

  (Нет тела ответа)

  Если локация продукта не найдена, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/ProductLocations/{id} (DELETE)</summary>

  Удаляет локацию продукта по её ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Пример запроса: `DELETE /api/ProductLocations/1`

  Пример ответа при успешном удалении (HTTP 204 No Content):

  (Нет тела ответа)

  Если локация продукта не найдена, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/ProductLocations/transfer (POST)</summary>

  Выполняет перенос продукта между локациями. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "productId": 5,
    "sourceLocationId": 1,
    "destinationLocationId": 2,
    "quantity": 5
  }
  ```

  Пример ответа при успешном выполнении (HTTP 204 No Content):

  (Нет тела ответа)

  Возможные ошибки:
  - Если продукт, исходная или целевая локация не найдены, возвращается HTTP 404 Not Found с сообщением об ошибке.
  - Если количество для переноса превышает доступное, возвращается HTTP 400 Bad Request с сообщением об ошибке.
</details>

<details>
  <summary>/ProductLocations/report (GET)</summary>

  Генерирует отчет по складу в формате Base64 (PDF). Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/ProductLocations/report`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "report": "base64-encoded-pdf-string"
  }
  ```
</details>

<details>
  <summary>/ProductLocations/generate-label/{productId} (GET)</summary>

  Генерирует этикетку для товара в формате Base64 (PDF). Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Не баг а фича: если название товара и/или склада на русском языке, то строка будет пустая. 
  
  Пример запроса: `GET /api/ProductLocations/generate-label/5`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "label": "base64-encoded-pdf-string"
  }
  ```
</details>

### Products

<details>
  <summary>/Products (GET)</summary>

  Получает список всех продуктов. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Products`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "name": "Laptop",
      "categoryId": 1,
      "categoryName": "Electronics",
      "supplierId": 1,
      "supplierName": "Tech Supplier"
    },
    {
      "id": 2,
      "name": "Phone",
      "categoryId": 1,
      "categoryName": "Electronics",
      "supplierId": 1,
      "supplierName": "Tech Supplier"
    }
  ]
  ```
</details>

<details>
  <summary>/Products/{id} (GET)</summary>

  Получает продукт по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Products/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "name": "Laptop",
    "categoryId": 1,
    "categoryName": "Electronics",
    "supplierId": 1,
    "supplierName": "Tech Supplier"
  }
  ```

  Если продукт не найден, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Products (POST)</summary>

  Создает новый продукт. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "name": "Tablet",
    "categoryId": 1,
    "supplierId": 1
  }
  ```

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 3,
    "name": "Tablet",
    "categoryId": 1,
    "categoryName": "Electronics",
    "supplierId": 1,
    "supplierName": "Tech Supplier"
  }
  ```
</details>

<details>
  <summary>/Products/{id} (PUT)</summary>

  Обновляет продукт по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "name": "Laptop Updated"
  }
  ```

  Пример ответа при успешном обновлении (HTTP 204 No Content):

  (Нет тела ответа)

  Если продукт не найден, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Products/{id} (DELETE)</summary>

  Удаляет продукт по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для роли Admin.

  Пример запроса: `DELETE /api/Products/1`

  Пример ответа при успешном удалении (HTTP 204 No Content):

  (Нет тела ответа)

  Если продукт не найден, возвращается HTTP 404 Not Found.
</details>

### Suppliers

<details>
  <summary>/Suppliers (POST)</summary>

  Создает нового поставщика. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "name": "Tech Supplier",
    "contactPerson": "John Doe",
    "email": "contact@techsupplier.com",
    "phone": "+1234567890",
    "address": "123 Tech Street"
  }
  ```

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 1,
    "name": "Tech Supplier",
    "contactPerson": "John Doe",
    "email": "contact@techsupplier.com",
    "phone": "+1234567890",
    "address": "123 Tech Street",
    "products": []
  }
  ```

  Если данные поставщика отсутствуют, возвращается HTTP 400 Bad Request с сообщением: "Supplier data is null."
</details>

<details>
  <summary>/Suppliers/{id} (GET)</summary>

  Получает поставщика по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Suppliers/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "name": "Tech Supplier",
    "contactPerson": "John Doe",
    "email": "contact@techsupplier.com",
    "phone": "+1234567890",
    "address": "123 Tech Street",
    "products": []
  }
  ```

  Если поставщик не найден, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Suppliers (GET)</summary>

  Получает список всех поставщиков. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Suppliers`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "name": "Tech Supplier",
      "contactPerson": "John Doe",
      "email": "contact@techsupplier.com",
      "phone": "+1234567890",
      "address": "123 Tech Street",
      "products": []
    },
    {
      "id": 2,
      "name": "Gadget Supplier",
      "contactPerson": "Jane Smith",
      "email": "contact@gadgetsupplier.com",
      "phone": "+0987654321",
      "address": "456 Gadget Avenue",
      "products": []
    }
  ]
  ```
</details>

### Warehouses

<details>
  <summary>/Warehouses (GET)</summary>

  Получает список всех складов. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Warehouses`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  [
    {
      "id": 1,
      "name": "Main Warehouse",
      "locationCount": 2
    },
    {
      "id": 2,
      "name": "Secondary Warehouse",
      "locationCount": 1
    }
  ]
  ```
</details>

<details>
  <summary>/Warehouses/{id} (GET)</summary>

  Получает склад по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login).

  Пример запроса: `GET /api/Warehouses/1`

  Пример ответа при успешном запросе (HTTP 200 OK):

  ```json
  {
    "id": 1,
    "name": "Main Warehouse",
    "locationCount": 2
  }
  ```

  Если склад не найден, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Warehouses (POST)</summary>

  Создает новый склад. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "name": "New Warehouse"
  }
  ```

  Пример ответа при успешном создании (HTTP 201 Created):

  ```json
  {
    "id": 3,
    "name": "New Warehouse",
    "locationCount": 0
  }
  ```
</details>

<details>
  <summary>/Warehouses/{id} (PUT)</summary>

  Обновляет склад по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для ролей Admin и Manager.

  Для отправки запроса необходимо передать следующее тело:

  ```json
  {
    "name": "Main Warehouse Updated"
  }
  ```

  Пример ответа при успешном обновлении (HTTP 204 No Content):

  (Нет тела ответа)

  Если склад не найден, возвращается HTTP 404 Not Found.
</details>

<details>
  <summary>/Warehouses/{id} (DELETE)</summary>

  Удаляет склад по его ID. Требуется авторизация с помощью JWT-токена (см. /Auth/login). Доступно только для роли Admin.

  Пример запроса: `DELETE /api/Warehouses/1`

  Пример ответа при успешном удалении (HTTP 204 No Content):

  (Нет тела ответа)

  Если склад не найден, возвращается HTTP 404 Not Found.
</details>


