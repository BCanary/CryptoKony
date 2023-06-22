# CryptoKony
![image](https://github.com/BCanary/CryptoKony/assets/59798021/ecaf0473-5294-45b2-91ac-484c91655485)

Инструмент создания и проверки электронной подписи документов
> Проектная работа

Требования:
- Windows 10
- Установленный .NET 6.0

## Что такое CryptoKony?
Это абсолютно бесплатный инструмент для работы с ЭП и сертификатами основанный на RSA-2048bit.

![image](https://github.com/BCanary/CryptoKony/assets/59798021/b0e73644-3013-40d4-8916-3e15d3a14e86)

## Какими ещё сертификатами?
Абсолютно новый революционный русский стандарт X.510 (Читается как "Хэ точка 510", не путать с "Икс") содержит в себе информацию о том: 
- Кто выдал сертификат;
- Кому выдан сертифкат;
- Когда был создан сертификат;
- Когда истекает сертификат;
- Открытый ключ сертификата.

![image](https://github.com/BCanary/CryptoKony/assets/59798021/d84cc3e9-1263-45b3-84ae-ddd5dc288610)
> Корневой сертификат CryptoKony

## А кому доверять?
А я поддерживаю я. Доверяйте мне. Я подписал сам себе корневой сертификат - подпишу сертификат и вам. Потом каждый у кого установлен корневой сертификат CryptoKony сможет подтвердить владение вашей подписи. Пишите письма.

### Не хочу тебе доверять.
Ну тогда доверяй сам себе! Просто создай свой личный корневой сертификат.

### А как его создать?
1. Создаёшь открытый и закрытый ключ, экспортируешь куда-нибудь (закрытый никому не давать!);
2. Во вкладке генерация сертификата указываешь эти два ключа, срок какой хочется, имя кому и от кого пишешь одинаковое (самоподпись);

![image](https://github.com/BCanary/CryptoKony/assets/59798021/0a1fab45-52c4-4c4c-9db2-17e33dc873b4)
> Пример создания
3. Переходишь в папку документы-CryptoKony-Trusted, добавляешь туда свой корневой сертификат

### А как подписать моим корневым сертификатом сертификат друга?
1. Просишь создать его открытый и закрытый ключ, открытый ключ он должен прислать тебе;
2. При генерации указываешь имя родительского сертификата (в данном случае корневого), в имени кому выдан - что попросит друг, срок какой хочется (не старше корневого);

![image](https://github.com/BCanary/CryptoKony/assets/59798021/93cb6042-6cf1-41ad-a3ad-6166ee554747)
> Сертификат друга

## А как проверить что сертификат действителен?
В первой вкладке можно подробно увидеть цепочку наследования сертификатов для указанного сертификата и удостовериться что сертификат валидный.

![image](https://github.com/BCanary/CryptoKony/assets/59798021/d18abc6d-8f33-49d6-911c-6fb528bc48d9)
> Цепь сертификата

![image](https://github.com/BCanary/CryptoKony/assets/59798021/61c71be0-3af3-462e-ae03-9a435a08a5cc)
> Валидность сертификата

## Ну и что с этим делать?
Подписывать документы. Во вкладке подписи нужно выбрать:
1. Закрытый ключ сертификата;
2. Сам сертификат который "прилипнет" к docx;
3. Сам docx файл;

![image](https://github.com/BCanary/CryptoKony/assets/59798021/a8327184-7089-4b62-88c3-62a856ff7871)
> Пример генерации подписи
4. Сохраняешь .signed.docx куда угодно и кидаешь кому угодно у кого есть твой корневой сертификат которым он может проверить валидность подписи.

### А как проверить?
Вкладка "Проверить подпись". Нужно просто указать файл и тыкнуть "Проверить":
1. Если сертификат не истёк;
2. Если подпись сертификата проверена родителем и совпадает с подписью сертификата (он не был изменён);
3. Если файл не был изменён (подпись расшифрована открытой подписью сертификата);
4. То файл не изменялся! Поздравляю, документ проверен. Можно посмотреть кем был подписан документ и когда;

![image](https://github.com/BCanary/CryptoKony/assets/59798021/d50d92ea-34bb-4ce6-a562-ce4ff0a46357)
> Пример проверки
5. Если файл был изменён хоть на один бит - проверка выдаст ошибку.

Потом при желании можно проверить цепочку сертификатов для полученного из документа сертификата.

# Проверка после установки
В директории находится файл test.signed.docx, его можно проверить

![image](https://github.com/BCanary/CryptoKony/assets/59798021/dacf1eb0-54a7-4a95-8d8f-1ccc8af611ab)
> Содержание документа

![image](https://github.com/BCanary/CryptoKony/assets/59798021/5d5d3f1e-6384-484d-a3d8-526a97052791)
> Проверка подписи

# А оно... Безопасно?..
Кто знает, друг! Одному Богу известно! Используй на свой страх и риск!

# Что дальше?
Можно добавить другие форматы документов, улучшить интерфейс и т.п. Делать я этого конечно же не буду. Основная цель моей работы достигнута: подпись и проверка документов, создание и проверка цепочки сертификатов.
