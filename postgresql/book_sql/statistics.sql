explain select * from flights

select reltuples from pg_class where relname = 'flights'


explain (analyze, timing off, summary off) select * from flights
where status = 'Scheduled' 

explain select count(*) from seats;


explain select count(*) from bookings;