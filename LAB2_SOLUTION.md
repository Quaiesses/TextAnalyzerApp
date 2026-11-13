# Лабораторная работа №2 — поэтапное решение

**Тема:** Системы контроля версий как основа промышленной разработки и процессов CI/CD

**Базовый проект (ЛР №1):** `TextAnalyzerApp` (C# / .NET 10) — система анализа текста со слабой связностью через интерфейс `ITextAnalyzer`, логированием Serilog и сериализацией Newtonsoft.Json.

> В методичке примеры даны для Python (`pip`, `pytest`, `logger.py`). Ниже все шаги **адаптированы под ваш C#-проект**, чтобы задание выполнялось на реальном коде ЛР №1.

---

## Цель работы

- Изучить назначение и классификацию СКВ
- Освоить базовые и продвинутые команды Git
- Научиться работать с ветками, слияниями и конфликтами
- Освоить работу с удалённым репозиторием GitHub
- Понять основы Pull Request и code review
- Подготовить проект из ЛР №1 к интеграции с CI/CD

---

## Текущее состояние проекта

| Что есть | Статус |
|----------|--------|
| Код ЛР №1 (`Program.cs`, анализаторы, тесты xUnit) | ✅ |
| Git-репозиторий | ❌ ещё не инициализирован |
| `.gitignore`, `README.md`, `CHANGELOG.md` | ❌ нет |
| GitHub remote / PR / Releases / Actions | ❌ нет |

---

## Часть 1. Настройка Git

### Шаг 1.1. Проверить установку

```powershell
git --version
```

### Шаг 1.2. Настроить имя и email

```powershell
git config --global user.name "Ваше Имя"
git config --global user.email "ваш@email.com"
```

### Шаг 1.3. Проверить настройки

```powershell
git config --list
```

**Скриншот для отчёта:** вывод `git --version` и `git config --list`.

---

## Часть 2. Инициализация репозитория

### Шаг 2.1. Перейти в папку проекта

```powershell
cd c:\Users\user\source\repos\TextAnalyzerApp
```

### Шаг 2.2. Инициализировать репозиторий

```powershell
git init
```

### Шаг 2.3. Создать `.gitignore` (для .NET / Visual Studio)

Создайте файл `.gitignore` со содержимым:

```gitignore
## Build
bin/
obj/
[Bb]uild/
[Ll]og/
[Ll]ogs/

## IDE / OS
.vs/
.idea/
*.user
*.suo
*.user
.DS_Store
Thumbs.db

## NuGet
*.nupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/

## Test / coverage
TestResults/
coverage*
*.coverage
*.coveragexml

## App output
analyzers_report.json
*.log

## Lab materials (опционально — можно не коммитить методички)
# *.pdf
# *.docx
```

### Шаг 2.4. Создать `README.md`

```markdown
# TextAnalyzerApp (ЛР №1)

Система анализа текста со слабой связностью модулей через интерфейсы.

## Автор

ФИО, группа ИСТ-XX

## Описание

Проект демонстрирует слабую связность через интерфейс `ITextAnalyzer`.
Модули: `WordCounter`, `SentimentAnalyzer`, `KeywordExtractor`, `LanguageDetector`.
Результаты сериализуются в JSON (Newtonsoft.Json), логирование — Serilog.

## Запуск

```bash
dotnet restore
dotnet run --project TextAnalyzerApp.csproj
```

## Тесты

```bash
dotnet test
```
```

### Шаг 2.5. Проверить статус

```powershell
git status
```

### Шаг 2.6–2.7. Первый коммит

```powershell
git add .
git commit -m "Initial commit: проект ЛР №1"
```

**Скриншот для отчёта:** `git status` до и после коммита.

---

## Часть 3. Работа с ветками

### Шаг 3.1. Текущая ветка

```powershell
git branch
```

### Шаг 3.2. Переименовать в `main`

```powershell
git branch -M main
```

### Шаг 3.3. Создать feature-ветку

```powershell
git checkout -b feature/add-logging
```

### Шаг 3.4. Внести изменения (логирование)

В проекте уже есть Serilog. Для выполнения задания добавьте, например, простой helper `LoggingHelper.cs`:

```csharp
using Serilog;

namespace TextAnalyzerApp
{
    public static class LoggingHelper
    {
        public static void Info(string message) => Log.Information(message);
        public static void Error(string message) => Log.Error(message);
    }
}
```

И вызовите его один раз из `Main` (например, `LoggingHelper.Info("LoggingHelper активен");`).

> Альтернатива по методичке (`logger.py`) для Python-проекта. У вас — C#, поэтому достаточно отдельного модуля логирования, как выше.

### Шаг 3.5. Коммит в ветке

```powershell
git add .
git commit -m "Добавлена функция логирования"
```

### Шаг 3.6–3.8. Слияние в main и удаление ветки

```powershell
git checkout main
git merge feature/add-logging
git branch -d feature/add-logging
```

### Шаг 3.9. История

```powershell
git log --oneline --graph --all
```

**Скриншот для отчёта:** вывод `git log --oneline --graph --all`.

---

## Часть 4. Разрешение конфликта

### Шаг 4.1–4.3. Ветка + изменение README

```powershell
git checkout -b feature/conflict-test
```

В `README.md` заголовок:

```markdown
# Проект ЛР №1 (версия из ветки)
```

```powershell
git add README.md
git commit -m "Изменён заголовок в ветке"
```

### Шаг 4.4–4.6. Конфликтное изменение в main

```powershell
git checkout main
```

Заголовок:

```markdown
# Проект ЛР №1 (версия из main)
```

```powershell
git add README.md
git commit -m "Изменён заголовок в main"
```

### Шаг 4.7–4.9. Слияние → конфликт

```powershell
git merge feature/conflict-test
```

Ожидаемый маркер в `README.md`:

```text
<<<<<<< HEAD
# Проект ЛР №1 (версия из main)
=======
# Проект ЛР №1 (версия из ветки)
>>>>>>> feature/conflict-test
```

### Шаг 4.10–4.11. Разрешить и зафиксировать

1. Выберите итоговый заголовок (например, версию из `main` или объединённый текст).
2. Удалите маркеры `<<<<<<<`, `=======`, `>>>>>>>`.
3. Сохраните файл.

```powershell
git add README.md
git commit -m "Разрешён конфликт в README"
```

Опционально удалите ветку:

```powershell
git branch -d feature/conflict-test
```

**Для отчёта:** опишите причину конфликта и выбранное решение + скриншот маркеров/итогового файла.

---

## Часть 5. Работа с GitHub

### Шаг 5.1. Создать репозиторий на GitHub

- Название: `lab2-vcs` (или `TextAnalyzerApp` — по требованиям методички: `lab2-vcs`)
- Тип: **Public**
- **Не** инициализировать README / .gitignore / license

### Шаг 5.2. Связать и запушить

```powershell
git remote add origin https://github.com/ВАШ-ЛОГИН/lab2-vcs.git
git branch -M main
git push -u origin main
```

### Шаг 5.3. Проверить на GitHub

Все исходники должны появиться на странице репозитория.

### Шаг 5.4–5.5. `CHANGELOG.md`

Создайте файл:

```markdown
# История изменений

## [0.1.0] - 2026-11-13
### Добавлено
- Инициализация проекта ЛР №1 (TextAnalyzerApp)
- Модули анализа текста через ITextAnalyzer
- Функция логирования (Serilog / LoggingHelper)
- Unit-тесты (xUnit)
```

```powershell
git add CHANGELOG.md
git commit -m "Добавлен CHANGELOG"
git push
```

**Скриншот для отчёта:** страница репозитория на GitHub.

---

## Часть 6. Pull Request и ревью

### Шаг 6.1. Ветка для PR

```powershell
git checkout -b feature/update-readme
```

### Шаг 6.2. Раздел «Установка» в README

Добавьте (адаптация под .NET вместо `pip`):

```markdown
## Установка

```bash
dotnet restore
dotnet build
```
```

### Шаг 6.3. Push ветки

```powershell
git add README.md
git commit -m "Добавлен раздел Установка"
git push -u origin feature/update-readme
```

### Шаг 6.4–6.9. PR на GitHub

1. Нажмите **Compare & pull request**
2. Title: `Добавлен раздел «Установка»`
3. Description: кратко — зачем раздел (как запустить проект после клонирования)
4. **Create pull request**
5. Во вкладке **Files changed** просмотрите diff (это code review)
6. **Merge pull request** → **Confirm merge**
7. **Delete branch** на GitHub

### Шаг 6.10. Обновить локальный main

```powershell
git checkout main
git pull
```

**Скриншот для отчёта:** страница Pull Request.

---

## Часть 7. Теги и релизы

### Шаг 7.1–7.2. Тег

```powershell
git tag -a v0.1.0 -m "Первая версия проекта"
git push origin v0.1.0
```

### Шаг 7.3–7.5. Release на GitHub

1. **Releases** → **Create a new release**
2. Tag: `v0.1.0`
3. Описание:

```text
Первая версия проекта ЛР №1
- Реализована слабая связность (ITextAnalyzer)
- Добавлено логирование
- Добавлен CHANGELOG
```

4. **Publish release**

**Скриншот для отчёта:** страница релиза.

---

## Часть 8. Подготовка к CI/CD

### Шаг 8.1. Создать `.github/workflows/ci.yml`

Для **C# / .NET** (вместо Python из методички):

```yaml
name: CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore TextAnalyzerSolution.slnx

      - name: Build
        run: dotnet build TextAnalyzerSolution.slnx --no-restore --configuration Release

      - name: Test
        run: dotnet test TextAnalyzerApp.Tests/TextAnalyzerApp.Tests.csproj --no-build --configuration Release --verbosity normal
```

> Если `slnx` не подхватится в CI, используйте:
> `dotnet restore` / `dotnet build TextAnalyzerApp.csproj` и отдельно `dotnet test` для тестового проекта.
> Перед push убедитесь, что локально проходит: `dotnet test`.

### Шаг 8.2. (Опционально) Файл зависимостей

В .NET зависимости уже в `.csproj` / NuGet — отдельный `requirements.txt` **не нужен**.  
Если преподаватель требует файл «как в методичке», можно добавить пустой/поясняющий `requirements.txt` **не стоит** — лучше указать в отчёте адаптацию под .NET.

### Шаг 8.3. Коммит и push

```powershell
git add .github/
git commit -m "Добавлен CI workflow"
git push
```

### Шаг 8.4. Проверить Actions

GitHub → вкладка **Actions** — должен появиться зелёный (или красный) запуск workflow.

**Скриншот для отчёта:** успешный (или разобранный) Actions workflow.

---

## Задание для всех вариантов (дополнительно)

### 1. Ветка `feature/new-module` с новой функцией

Пример: новый анализатор `CharacterCounter : ITextAnalyzer`.

```powershell
git checkout main
git pull
git checkout -b feature/new-module
# добавить CharacterCounter.cs + подключить в Program.cs + тест
git add .
git commit -m "Добавлен модуль CharacterCounter"
git push -u origin feature/new-module
```

### 2. Слить через Pull Request

Создайте PR → Merge → Delete branch → локально `git checkout main && git pull`.

### 3. Тег `v0.2.0` и релиз

```powershell
# обновите CHANGELOG.md секцией [0.2.0]
git add CHANGELOG.md
git commit -m "Обновлён CHANGELOG для v0.2.0"
git push

git tag -a v0.2.0 -m "Вторая версия: новый модуль"
git push origin v0.2.0
```

На GitHub создайте Release для `v0.2.0`.

### 4. Fork + PR в чужой open-source проект

1. Найдите небольшой публичный репозиторий (документация с опечаткой)
2. **Fork** → клонируйте fork
3. Исправьте опечатку → commit → push
4. Откройте Pull Request в оригинальный репозиторий
5. В отчёт вставьте ссылку на ваш PR

---

## Структура отчёта (чек-лист)

- [ ] 1. Титульный лист
- [ ] 2. Цель работы
- [ ] 3. Краткое описание проекта из ЛР №1 (TextAnalyzerApp, интерфейсы, модули)
- [ ] 4. Таблица основных команд Git с пояснениями
- [ ] 5. Скриншоты:
  - [ ] `git log --oneline --graph --all`
  - [ ] `git status`
  - [ ] страница репозитория на GitHub
  - [ ] Pull Request
  - [ ] релиз
  - [ ] Actions workflow
- [ ] 6. Описание конфликта и его разрешения
- [ ] 7. Выводы
- [ ] 8. Список литературы

### Пример таблицы команд для отчёта

| Команда | Назначение |
|---------|------------|
| `git init` | Создать репозиторий |
| `git clone URL` | Клонировать репозиторий |
| `git status` | Состояние репозитория |
| `git add file` | Добавить в индекс |
| `git commit -m "..."` | Зафиксировать изменения |
| `git log` | История коммитов |
| `git branch` | Список веток |
| `git checkout -b name` | Создать и переключиться на ветку |
| `git merge name` | Слить ветку |
| `git push` | Отправить на сервер |
| `git pull` | Забрать с сервера |
| `git tag -a` | Создать аннотированный тег |
| `git remote add` | Привязать удалённый репозиторий |

---

## Контрольные вопросы (краткие ответы)

1. **Распределённая vs централизованная СКВ**  
   В централизованной (SVN) одна общая копия на сервере. В распределённой (Git) у каждого разработчика полный клон истории; сервер — один из remote.

2. **Коммит, ветка, слияние**  
   Коммит — снимок изменений. Ветка — указатель на цепочку коммитов. Слияние — объединение историй двух веток.

3. **Какие команды использовали**  
   `init`, `add`, `commit`, `branch`, `checkout`/`switch`, `merge`, `status`, `log`, `remote`, `push`, `pull`, `tag`.

4. **Конфликт и разрешение**  
   Конфликт — одновременное изменение одних и тех же строк в разных ветках. Разрешается вручную: выбрать/объединить код, убрать маркеры, `git add` + `git commit`.

5. **Pull Request**  
   Предложение влить изменения из ветки в целевую (обычно `main`) с обсуждением и code review до слияния.

6. **`git pull` vs `git fetch`**  
   `fetch` только скачивает изменения remote. `pull` = `fetch` + merge/rebase в текущую ветку.

7. **Тег и релиз**  
   Тег — именованная метка коммита (версия). Релиз на GitHub — публикация тега с описанием/артефактами.

8. **Git и CI/CD**  
   Push/PR в Git запускает webhook → CI собирает и тестирует → CD может задеплоить успешную сборку. Без истории в Git современный CI/CD practically невозможен.

---

## Рекомендуемый порядок выполнения (кратко)

```text
1. config Git
2. git init + .gitignore + README → Initial commit
3. main + feature/add-logging → merge
4. conflict-test → конфликт → resolve
5. GitHub repo lab2-vcs → push
6. CHANGELOG → push
7. feature/update-readme → PR → merge
8. tag v0.1.0 → Release
9. .github/workflows/ci.yml → push → проверить Actions
10. feature/new-module → PR → tag v0.2.0 → fork+PR
11. Оформить отчёт со скриншотами
```

---

## Замечания по адаптации методички

| В PDF (Python) | У вас (C# / .NET) |
|----------------|-------------------|
| `logger.py` | `LoggingHelper.cs` + уже есть Serilog |
| `.gitignore` с `__pycache__`, `venv` | `.gitignore` для `bin/`, `obj/`, `.vs/` |
| `pip install -r requirements.txt` | `dotnet restore` / `dotnet build` |
| `python -m pytest` | `dotnet test` |
| `actions/setup-python` | `actions/setup-dotnet` |

Если преподаватель требует **буквально** Python-файлы из методички — уточните: базовый проект ЛР №1 у вас на C#, поэтому корректная адаптация уместна и логична.
