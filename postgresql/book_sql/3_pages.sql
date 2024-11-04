create extension pageinspect;

select lower, upper, special, pagesize from page_header(get_raw_page('account', 0));

create table paddding (b1 boolean, i1 integer, b2 boolean, i2 integer)
insert into padding values(true, 1, false, 2);

select lp_len from heap_page_items(get_raw_page('padding', 0));

create table t(id integer generated always as identity, s text);
create index on t(s);

BEGIN;
insert into t(s) values('foo');
select pg_current_xact_id();


select xmin, xmax, * from t