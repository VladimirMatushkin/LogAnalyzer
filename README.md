# LogAnalyzer
---
Parser - C# Visual Studio 2017

Параметры запуска 
1) Путь к файлу с логами
2) Путь к папке с файлом load_db_sample.sql

В параметрах не должно быть символов кириллицы.

Пример запуска: Parser.exe logs.txt D:\db\sql\

После запуска в папке с приложением создастся папка logs, содержащая файлы для загрузки в бд, так же в папке указанной вторым параметром появится файл load_db.sql.

---
В папке sql данного репозитория хранятся файлы:

* db_create.sql - содержащий скрипт для создания бд и таблиц
* load_db_sample.sql - содержащий скрипт для загрузки данных в бд
* после запуска парсера, создастся файл load_db.sql, являющийся копией load_db_sample.sql, с прописанными путями к файлам для загрузки в бд.
* для опредления страны по IP была использована готовая бд https://lite.ip2location.com/database/ip-country <br>
ip2location.sql скрипт для создания, в котором нужно прописать путь к файлу IP2LOCATION-LITE-DB1.CSV

Скрипт получения стран, takes time
```sql
UPDATE logs.user as u
SET u.Country = (SELECT ip.country_name 
                 FROM ip2location.ip2location_db1 as ip 
                 WHERE u.IP BETWEEN ip.ip_from AND ip.ip_to)
``` 
---
WebApp - сайт для работы с бд.
Запускался на xampp 7.3.0-0
БД - MariaDB
