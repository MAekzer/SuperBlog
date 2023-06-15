# SuperBlog

### На данный момент выполнена работа по первому шагу итоговой стажировки:
* Создана база данных со всеми необходимыми сущностями
* Созданы бизнес-модели и соответствующие классы-репозитории
* Созданы необходимые контроллеры с логикой CRUD операций
* Добавлены роли: админ, модератор и пользователь. Для каждой роли добавлен пользователь
* Роль модератора (по задумке) всегда подразумевает роль пользователя. Аналогично роль админа также включает в себя роль модератора
* Для демострации работоспособности авторизации, основанной на ролях, к методу MyFeed контроллера User подключена авторизация под роль админа
* Для работы приложения подразумевается, что в базе данных уже есть роли, а также учетные записи админа и модератора. Подробности закомментированы в классе SecurityRepository

### Выполнена работа для второго шага итоговой стажировки
* Созданы представления для работы со всеми сущностями
* Добавлены условия для авторизированных пользователей и различных ролей
* В представления с просмотром всех пользователей и статей добавлена функция поиска
* Частично реализована валидация пользовательского ввода
