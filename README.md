# TaskTracker — Система управления задачами и проектами

Веб-приложение для управления проектами, задачами и командным взаимодействием (аналог Jira / трекер задач).

## Стек технологий

### Бэкенд
- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core** (Code First, PostgreSQL)
- **Clean Architecture** (Domain → Application → Infrastructure → API)
- **CQRS** (MediatR)
- **JWT-аутентификация** (ASP.NET Identity)
- **Swagger/OpenAPI**

### Фронтенд
- **React 19** + **TypeScript**
- **Vite** (сборщик)
- **React Router** (маршрутизация)
- **@hello-pangea/dnd** (Drag & Drop для Kanban-доски)
- **Recharts** (графики и диаграммы)
- **Axios** (HTTP-клиент)

### База данных
- **PostgreSQL 16**

## Структура проекта

```
task-tracker/
├── backend/
│   ├── src/
│   │   ├── TaskTracker.Domain/          # Сущности, перечисления
│   │   ├── TaskTracker.Application/     # CQRS команды/запросы, DTO, интерфейсы
│   │   ├── TaskTracker.Infrastructure/  # EF DbContext, JWT, Email
│   │   └── TaskTracker.API/             # Контроллеры, Swagger, конфигурация
│   └── tests/
│       └── TaskTracker.Tests/           # Unit-тесты (xUnit)
├── frontend/
│   └── src/
│       ├── api/                         # HTTP-клиенты
│       ├── components/                  # React-компоненты
│       ├── context/                     # AuthContext
│       └── types/                       # TypeScript типы
├── docker-compose.yml
└── README.md
```

## Функционал

### Управление пользователями и ролями
- Регистрация и авторизация (JWT)
- Роли: Администратор, Менеджер проектов, Разработчик, Тестировщик, Наблюдатель

### Управление задачами (Issues)
- Создание, редактирование, комментирование и удаление задач
- Типы задач: Баг, Задача, История, Эпик
- Настраиваемые поля: приоритет, дедлайн, ответственный, теги

### Рабочие процессы и доски (Workflows & Boards)
- Kanban-доска с Drag & Drop
- Настраиваемые статусы: «К выполнению», «В работе», «Тестирование», «Готово»
- Полный цикл перемещения задачи по колонкам

### Отчётность и аналитика
- Дашборд с графиками по статусу, приоритету и типу задач
- Круговые и столбчатые диаграммы
- Статистика прогресса проекта

### API
- RESTful API с полной документацией Swagger
- Эндпоинты: Auth, Projects, Issues, Boards, Comments

## Развёртывание

### Предварительные требования
- .NET SDK 10.0+
- Node.js 18+
- PostgreSQL 16+

### 1. База данных

Создайте БД PostgreSQL:
```sql
CREATE DATABASE tasktracker;
```

### 2. Бэкенд

```bash
cd task-tracker/backend

# Настройте строку подключения в appsettings.json при необходимости
# По умолчанию: Host=localhost;Port=5432;Database=tasktracker;Username=postgres;Password=postgres

# Применить миграции и запустить
dotnet run --project src/TaskTracker.API

# API доступен по адресу: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### 3. Фронтенд

```bash
cd task-tracker/frontend

npm install
npm run dev

# Приложение доступно: http://localhost:5173
```

### 4. Docker (альтернативный вариант)

```bash
cd task-tracker
docker-compose up -d

# Бэкенд: http://localhost:5000
# Фронтенд: http://localhost:3000
```

## Запуск тестов

```bash
cd task-tracker/backend
dotnet test
```

Тесты включают:
- Создание проекта с автоматическим созданием доски
- Получение проектов пользователя
- Удаление проекта
- Создание задачи с автоматическим назначением ключа
- Перемещение задачи по статусам
- Полный цикл задачи (Todo → InProgress → Testing → Done)
- Удаление задачи

## API Эндпоинты

| Метод  | URL                              | Описание                    |
|--------|----------------------------------|-----------------------------|
| POST   | /api/auth/register               | Регистрация                 |
| POST   | /api/auth/login                  | Авторизация                 |
| GET    | /api/auth/me                     | Текущий пользователь        |
| GET    | /api/auth/users                  | Список пользователей        |
| GET    | /api/projects                    | Список проектов             |
| GET    | /api/projects/{id}               | Проект по ID                |
| POST   | /api/projects                    | Создать проект              |
| PUT    | /api/projects/{id}               | Обновить проект             |
| DELETE | /api/projects/{id}               | Удалить проект              |
| GET    | /api/issues/project/{projectId}  | Задачи проекта              |
| GET    | /api/issues/{id}                 | Задача по ID                |
| POST   | /api/issues                      | Создать задачу              |
| PUT    | /api/issues/{id}                 | Обновить задачу             |
| PATCH  | /api/issues/{id}/move            | Переместить задачу          |
| DELETE | /api/issues/{id}                 | Удалить задачу              |
| GET    | /api/boards/project/{projectId}  | Доска проекта               |
| GET    | /api/comments/issue/{issueId}    | Комментарии задачи          |
| POST   | /api/comments/issue/{issueId}    | Добавить комментарий        |
