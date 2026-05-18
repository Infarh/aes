# AES File Encryptor

Небольшая консольная утилита на .NET 10 для шифрования и расшифровки файлов с помощью AES.

## Что делает проект

- Шифрует файл и создает рядом новый файл с расширением `.aes`.
- Расшифровывает файл с расширением `.aes` обратно в исходный файл.
- Может обрабатывать несколько файлов за один запуск.
- Показывает прогресс копирования в консоли.

## Как работает

Программа принимает список путей к файлам через аргументы командной строки.

- Если файл **не** имеет расширение `.aes`, выполняется шифрование.
- Если файл имеет расширение `.aes`, выполняется расшифровка.

Ключ и IV для AES вычисляются через PBKDF2 (SHA-512) на основе:

- имени файла (без расширения),
- внутренней `salt`, формируемой из имени процесса и фиксированных байтов.

## Требования

- .NET SDK 10+
- Windows/Linux/macOS (для запуска через `dotnet`)

## Сборка

Из корня репозитория:

```bash
dotnet build FileEncryptor.Console/FileEncryptor.Console.csproj -c Release
```

## Запуск

### Вариант 1: через `dotnet run`

```bash
dotnet run --project FileEncryptor.Console -- "C:\data\report.pdf"
```

```bash
dotnet run --project FileEncryptor.Console -- "C:\data\report.pdf.aes"
```

Несколько файлов сразу:

```bash
dotnet run --project FileEncryptor.Console -- "C:\data\a.txt" "C:\data\b.jpg" "C:\data\c.zip"
```

### Вариант 2: после публикации

```bat
cd FileEncryptor.Console
release.bat
```

После публикации исполняемый файл будет в папке `.Release`.

Пример запуска опубликованной версии:

```bat
FileEncryptor.Console.exe "C:\data\report.pdf"
FileEncryptor.Console.exe "C:\data\report.pdf.aes"
```

## Примеры сценариев

### Шифрование

Вход:

- `photo.png`

Команда:

```bash
dotnet run --project FileEncryptor.Console -- "photo.png"
```

Результат:

- создается `photo.png.aes`

### Расшифровка

Вход:

- `photo.png.aes`

Команда:

```bash
dotnet run --project FileEncryptor.Console -- "photo.png.aes"
```

Результат:

- создается `photo.png`

## Ограничения и замечания

- Пароль не вводится пользователем: он вычисляется из имени файла.
- Если переименовать файл, расшифровка может завершиться ошибкой (`File name error`).
- Алгоритм рассчитан на простую утилиту и не является заменой полноценных систем управления ключами.

## Структура проекта

- `FileEncryptor.Console/Program.cs` — точка входа, разбор аргументов, выбор режима (encrypt/decrypt).
- `FileEncryptor.Console/FileInfoEx.cs` — операции шифрования/дешифрования файла.
- `FileEncryptor.Console/AesEx.cs` — создание криптопотоков AES.
- `FileEncryptor.Console/StreamEx.cs` — копирование потоков с выводом прогресса.
- `FileEncryptor.Console/Constants.cs` — константы и генерация salt.
