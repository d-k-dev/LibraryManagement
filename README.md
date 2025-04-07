# 📚 Library Management API

<div align="center">
  
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-blue)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-purple)
![JWT](https://img.shields.io/badge/JWT-Authentication-yellow)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

</div>

Полнофункциональный RESTful API для управления библиотекой, разработанный с использованием ASP.NET Core.       
API позволяет управлять книгами, авторами, категориями, пользователями и заказами.

## 🔍 Содержание

- [Возможности](#-возможности)
- [Технологии](#-технологии)
- [Архитектура](#-архитектура)
- [API Endpoints](#-api-endpoints)
- [Установка и запуск](#-установка-и-запуск)

## ✨ Возможности

- **Управление книгами** - добавление, редактирование, поиск, удаление
- **Управление авторами** - с привязкой к книгам
- **Категоризация** - разделение книг по категориям
- **Пользователи и роли** - регистрация, авторизация, управление правами
- **Заказы книг** - создание заказов, изменение статусов, возврат
- **Безопасность** - JWT токены, хэширование паролей

## 🛠 Технологии

- **ASP.NET Core 9** - фреймворк для создания API
- **Entity Framework Core** - ORM для работы с базой данных
- **AutoMapper** - для преобразования объектов
- **JWT Authentication** - для безопасной аутентификации
- **Swagger/OpenAPI** - для автоматической документации API
- **xUnit & Moq** - для модульного тестирования
- **Serilog** - для продвинутого логирования
- **Docker** - для контейнеризации приложения

## 🏗 Архитектура

Проект построен с использованием многослойной архитектуры:

```
LibraryManagement/
├── LibraryManagement.Core/           # Доменная модель и интерфейсы
│   ├── Entities/                     # Сущности базы данных
│   ├── Interfaces/                   # Интерфейсы репозиториев и сервисов
│   └── Models/                       # DTO и другие модели
│
├── LibraryManagement.Infrastructure/ # Реализация инфраструктуры
│   ├── Data/                         # Контекст БД и конфигурации
│   ├── Repositories/                 # Реализация репозиториев
│   ├── Services/                     # Бизнес-логика приложения
│   └── Utils/                        # Вспомогательные классы
│
├── LibraryManagement.API/            # API контроллеры и настройки
│   ├── Controllers/                  # REST API контроллеры
│   ├── Middleware/                   # Middleware компоненты
│   └── Program.cs                    # Конфигурация приложения

```
## 🔌 API Endpoints

### 🔐 Авторизация и аутентификация

| Метод | Endpoint           | Описание                   | Роли               |
|-------|--------------------|-----------------------------|-------------------|
| POST  | /api/auth/register | Регистрация пользователя    | Все               |
| POST  | /api/auth/login    | Вход в систему              | Все               |

### 📖 Книги

| Метод   | Endpoint                       | Описание                         | Роли              |
|---------|--------------------------------|----------------------------------|-------------------|
| GET     | /api/books                     | Получение всех книг              | Все               |
| GET     | /api/books/{id}                | Получение книги по ID            | Все               |
| GET     | /api/books/author/{authorId}   | Получение книг по автору         | Все               |
| GET     | /api/books/category/{categoryId}| Получение книг по категории     | Все               |
| GET     | /api/books/available           | Получение доступных книг         | Все               |
| POST    | /api/books                     | Создание книги                   | Admin, Librarian  |
| PUT     | /api/books/{id}                | Обновление книги                 | Admin, Librarian  |
| DELETE  | /api/books/{id}                | Удаление книги                   | Admin             |

### ✒️ Авторы

| Метод   | Endpoint                      | Описание                        | Роли              |
|---------|-------------------------------|---------------------------------|-------------------|
| GET     | /api/authors                  | Получение всех авторов          | Все               |
| GET     | /api/authors/{id}             | Получение автора по ID          | Все               |
| GET     | /api/authors/book/{bookId}    | Получение авторов книги         | Все               |
| POST    | /api/authors                  | Создание автора                 | Admin, Librarian  |
| PUT     | /api/authors/{id}             | Обновление автора               | Admin, Librarian  |
| DELETE  | /api/authors/{id}             | Удаление автора                 | Admin             |

### 🏷️ Категории

| Метод   | Endpoint                    | Описание                       | Роли              |
|---------|----------------------------|---------------------------------|-------------------|
| GET     | /api/categories            | Получение всех категорий        | Все               |
| GET     | /api/categories/{id}       | Получение категории по ID       | Все               |
| POST    | /api/categories            | Создание категории              | Admin, Librarian  |
| PUT     | /api/categories/{id}       | Обновление категории            | Admin, Librarian  |
| DELETE  | /api/categories/{id}       | Удаление категории              | Admin             |

### 👥 Пользователи

| Метод   | Endpoint                 | Описание                        | Роли              |
|---------|--------------------------|---------------------------------|-------------------|
| GET     | /api/users               | Получение всех пользователей    | Admin, Librarian  |
| GET     | /api/users/{id}          | Получение пользователя по ID    | Admin, Librarian  |
| GET     | /api/users/profile       | Получение своего профиля        | Авторизованные    |
| GET     | /api/users/email/{email} | Получение пользователя по email | Admin, Librarian  |
| POST    | /api/users               | Создание пользователя           | Admin             |
| PUT     | /api/users/{id}          | Обновление пользователя         | Admin             |
| DELETE  | /api/users/{id}          | Удаление пользователя           | Admin             |

### 📋 Заказы

| Метод   | Endpoint                  | Описание                       | Роли                  |
|---------|---------------------------|---------------------------------|----------------------|
| GET     | /api/orders               | Получение всех заказов          | Admin, Librarian     |
| GET     | /api/orders/{id}          | Получение заказа по ID          | Admin, Librarian     |
| GET     | /api/orders/user/{userId} | Получение заказов пользователя  | Admin, Librarian     |
| GET     | /api/orders/overdue       | Получение просроченных заказов  | Admin, Librarian     |
| GET     | /api/orders/my-orders     | Получение своих заказов         | Авторизованные       |
| POST    | /api/orders               | Создание заказа                 | Все                  |
| PUT     | /api/orders/{id}/status   | Обновление статуса заказа       | Admin, Librarian     |
| PUT     | /api/orders/{id}/return   | Возврат книги                   | Admin, Librarian     |

## 🚀 Установка и запуск

### Предварительные требования

- .NET 9 SDK
- SQL Server (или LocalDB)
- Git

### Локальный запуск

1. Клонировать репозиторий:
```bash
git clone https://github.com/d-k-dev/LibraryManagement.git
cd LibraryManagement
```

2. Настроить строку подключения в `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibraryManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
}
```

3. Применить миграции:
```bash
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.API
```

4. Запустить API:
```bash
dotnet run --project LibraryManagement.API
```

5. Открыть Swagger UI:
```
https://localhost:5001/swagger
```

---

<div align="center">
  
Разработано [Dmitry Khoroshkeev](https://github.com/d-k-dev) | &copy; 2025
  
</div>
