drop table posts;

create table posts(
	id serial primary key, 
	category_id integer,
	context text,
	rating integer not null
);

create index concurrently posts_category_id on posts using btree(category_id);

insert into posts(category_id, context, rating)
select floor(100*random()),
'hello word ' || id,
floor(random() * (10 + 1))
from generate_series(1, 10000) gs(id);

explain select count(*) from posts;
//  ->  Index Only Scan using posts_category_id on posts  (cost=0.29..173.38 rows=10000 width=0)


//reltuples - number of rows, relpages - number of pages
select reltuples, relpages from pg_class where relname = 'posts'

//remove data from index, optimizing size and query speed.
create index concurrently partial on foo using btree(bar_id, id) where bar_id not in (20,26,31,73)

//by table statistics
select * from pg_stat_user_tables;

//by index statistics
select * from pg_stat_user_indexes

//read some data from pg_statistics
select * from pg_catalog.pg_stats ps where tablename = 'posts';