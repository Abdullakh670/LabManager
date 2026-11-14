# AITU Lab Resource Manager

Веб-приложение для бронирования лабораторного оборудования (Cisco-стенды, серверы, измерительные приборы). Проект на C# и Blazor Server.

Авторы: Orynbassar Abdullakh, Shalkar Tauyekeluly (группа IT 2510).

## Возможности

- Просмотр оборудования и его статуса (Доступно / Занято / На обслуживании)
- Бронирование с проверкой конфликтов по времени
- Страница «Мои бронирования» с отменой брони
- Обновление статусов у всех пользователей в реальном времени (SignalR)
- Вход в систему, роли Student и Admin
- Админ-панель: добавление, перевод на обслуживание, удаление оборудования
- Unit-тесты логики бронирования и авторизации (xUnit + Moq)

## Архитектура

| Проект | Назначение |
|---|---|
| `LabManager.Domain` | Модели данных (Equipment, Booking, User) и перечисления |
| `LabManager.Application` | Бизнес-логика, интерфейсы сервисов и репозиториев |
| `LabManager.Infrastructure` | Entity Framework Core, SQLite, реализации репозиториев |
| `LabManager.WebUI` | Blazor-приложение, SignalR Hub, страницы |
| `LabManager.Tests` | Unit-тесты |

## Требования

- .NET 8 SDK
- Git

## Запуск

```bash
git clone <ссылка на репозиторий>
cd LabManager
dotnet run --project src/LabManager.WebUI
```

Откройте в браузере `http://localhost:5290`.

## База данных

Используется SQLite. Файл `labmanager.db` создаётся автоматически при первом запуске, тестовое оборудование и пользователи добавляются сразу. Строка подключения находится в `src/LabManager.WebUI/appsettings.json`.

Чтобы сбросить базу, остановите приложение и удалите файлы `labmanager.db*` в `src/LabManager.WebUI`.

## Тестовые аккаунты

| Роль | Email | Пароль |
|---|---|---|
| Admin | admin@labmanager.local | Admin123 |
| Student | student@labmanager.local | Student123 |

## Тесты

```bash
dotnet test
```

## Диаграммы

ER-диаграмма базы данных: `docs/ER_diagram.png`