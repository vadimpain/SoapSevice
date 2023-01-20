# SoapSevice
## Порядок запуска микросервиса средствами ServiceRunner (только для Windows):
1. Опубликуйте микросервис. Параметры для публикации:
	* Конфигурация: Release
	* Целевая платформа: netcoreapp3.1
	* Режим развертывания: Автономный
	* Целевая среда выполнения: win-x64
2. Перейдите в папку с опубликованными файлами, выделите их все и соберите .zip архив. После, переименуйте его в SoapService
3. Перенесите .zip архив в папку с DirectumRX в \etc\_builds_package
4. Перейдите в папку с настройками \etc\\_services_config. Создайте папку SoapService и перенесите в нее файл _ConfigSettings.xml из проекта.
5. Подкорректируйте параметры в _ConfigSettings.xml
6. Добавьте строчку для запуска микросервиса в ServiceRunner'е, для этого:
	* Перейдите в папку с конфигом \etc\\_services_config\ServiceRunner\_ConfigSettings.xml
	* Добавьте строчку в block:
	```
	<ServiceSetting Name="SoapService" Config="SoapService\_ConfigSettings.xml" Package="SoapService.zip" ConfigWatcherEnabled="false" />
	```
	* Сохраните _ConfigSettings.
7. Перезапустите службу ServiceRunner. Проверьте, что папка SoapService создалась в \etc\\_builds_bin. Создается не сразу после перезапуска, возможно стоит подождать пару минут.
8. Для проверки работоспособности сервиса, можно открыть его по адресу http://localhost:<Порт>/api/Soap/Service.asmx
