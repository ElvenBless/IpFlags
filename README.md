# IpFlags

Небольшое приложение для Windows: показывает флаг страны по вашему IP в системном трее. Иконка обновляется автоматически каждые 30 секунд или по двойному клику / пункту «Update» в меню.

## Скриншоты

![Иконка в трее](Assets/1.png)

![Иконка в трее](Assets/2.png)

## Требования

- Windows
- .NET 8 (или скачайте готовый exe из [Releases](https://github.com/ElvenBless/IpFlags/releases))

## Сборка

```bash
cd Src
dotnet publish IpFlags/IpFlags.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

Готовый файл будет в папке `Publish`.

## Лицензия

MIT