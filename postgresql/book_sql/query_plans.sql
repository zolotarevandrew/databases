prepare plane(text) as select * from aircrafts where aircraft_code = $1;

select name, statement, parameter_types from pg_catalog.pg_prepared_statements 

execute plane('433');


create index on bookings(total_amount);

explain select * from bookings where total_amount > 1000000;
--попадаем в индекс

explain select * from bookings where total_amount > 100;
--попадают все бронирования целиком



select reltuples, relpages, relallvisible  from pg_catalog.pg_class 
where relname = 'flights'


create table flights_copy(like flights)
with (autovacuum_enabled = false);

select reltuples, relpages, relallvisible  from pg_catalog.pg_class 
where relname = 'flights_copy'
--статистики нету


insert into flights_copy select * from flights;

analyze flights_copy;


select reltuples, relpages, relallvisible  from pg_catalog.pg_class 
where relname = 'flights_copy'
--статистика появилась


explain select * from boarding_passes where seat_no > '30B'