UPDATE rows

1) BEGIN 
update accounts set amount = amount - 100 where id = 1;

2) BEGIN
update accounts set amount = amount - 10 where id = 2;

3) UPDATE accounts set amount = amount + 100 where id = 2;

4) update accounts set amount = amount + 10 where id = 1;

правильный способ, блокирование ресурсов в одном и том же порядке - блокировать счета в порядке возрастания их номеров.


TWO UPDATE

update блокирует строки по мере их обновления.
Поэтому если одна команда update обновляет несколько строк в одном порядке, а другая в другом, они могу взаимозаблокироваться.


CREATE index on accounts (amount desc);


1) update accounts set amount = inc_slow(amount);

При последовательном сканировании строки будут обновляться в том же порядке (для больших таблиц это невсегда верно)

2) set enable_seqscan = OFF
выключаем использование последовательного сканирования.
в этом случае планировщик будет использовать сканирование индекса.

update accounts set amount = inc_slow(amount)
where amount > 100

3) 
