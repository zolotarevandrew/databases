create table grades (
id serial primary key, 
 g int,
 name text 
); 


insert into grades (g,
name  ) 
select 
random()*100,
substring(md5(random()::text ),0,floor(random()*31)::int)
 from generate_series(0, 1000000);;


explain select * from grades;
//Seq Scan on grades  (cost=0.00..14821.40 rows=808440 width=40)

0.00 - how many milliseconds took to get first page
14821.40 - estimated milliseconds
rows - quick number rows approximate by statistics.
width - sum bytes of all columns

explain select * from grades order by g;
//Sort  (cost=128407.96..130907.96 rows=1000001 width=22)

explain select * from grades order by name;
//Sort  (cost=128407.96..130907.96 rows=1000001 width=22)

explain select id from grades;
//Seq Scan on grades  (cost=0.00..16737.01 rows=1000001 width=4)

explain select name from grades;
//Seq Scan on grades  (cost=0.00..16737.01 rows=1000001 width=14)

explain analyze select name from grades where id = 7
//Index Scan using grades_pkey on grades  (cost=0.42..2.64 rows=1 width=14) (actual time=0.030..0.031 rows=1 loops=1)

create index id_idx on grades(id);

explain analyze select name from grades where id = 7
//Index Scan using id_idx on grades  (cost=0.42..2.64 rows=1 width=14) (actual time=0.102..0.103 rows=1 loops=1)

explain analyze select id from grades where id = 7
//Index Only Scan using id_idx on grades  (cost=0.42..1.54 rows=1 width=4) (actual time=0.105..0.106 rows=1 loops=1)

drop index id_idx;
create index id_idx on grades(id) include (name);

explain analyze select name from grades where id = 8
//Index Only Scan using id_idx on grades  (cost=0.42..1.54 rows=1 width=14) (actual time=0.032..0.033 rows=1 loops=1)