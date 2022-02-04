drop table temp

create table temp (t int);

insert into temp (t) select random()*100 from generate_series(0,1000000)

select count(*) from temp;


create table employees( id serial primary key, name text);

create or replace function random_string(length integer) returns text as 
$$
declare
  chars text[] := '{0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z}';
  result text := '';
  i integer := 0;
  length2 integer := (select trunc(random() * length + 1));
begin
  if length2 < 0 then
    raise exception 'Given length cannot be less than 0';
  end if;
  for i in 1..length2 loop
    result := result || chars[1+random()*(array_length(chars, 1)-1)];
  end loop;
  return result;
end;
$$ language plpgsql;


insert into employees(name)(select random_string(10) from generate_series(0, 1000000));

explain analyze select id from employees where id = 20000;
//Index Only Scan using employees_pkey on employees  (cost=0.42..1.54 rows=1 width=4) (actual time=0.047..0.048 rows=1 loops=1)
//0.200ms

explain analyze select name from employees where id = 50000;
// Index Scan using employees_pkey on employees  (cost=0.42..2.64 rows=1 width=6) (actual time=0.088..0.090 rows=1 loops=1)
// 0.200ms

explain analyze select id from employees where name = 'Zs';
//Seq Scan on employees  (cost=0.00..17602.01 rows=6 width=4) (actual time=2.379..131.387 rows=21 loops=1)
//Execution Time: 131.441 ms
//full table scan

explain analyze select id, name from employees where name like '%Zs1%';
//Seq Scan on employees  (cost=0.00..17602.01 rows=90 width=10) (actual time=2.669..161.352 rows=28 loops=1)
//Execution Time: 161.414 ms
//full table scan

create index employees_name on employees(name);

explain analyze select id from employees where name = 'Zs';
//Index Scan using employees_name on employees  (cost=0.42..8.23 rows=6 width=4) (actual time=0.094..0.205 rows=21 loops=1)
//Execution Time: 0.243 ms
