create table products (pid serial primary key, name text, price float, inventory integer);
create table sales(saleid serial primary key, pid integer, price float, quantity integer);


insert into products(name, price, inventory) values ('Phone', 999.999, 100);
insert into products(name, price, inventory) values ('Airpods', 99.99, 10);

insert into sales(pid, price, quantity) values(1, 999.99, 10);
insert into sales(pid, price, quantity) values(1, 999.99, 5);
insert into sales(pid, price, quantity) values(1, 999.99, 5);
insert into sales(pid, price, quantity) values(2, 99.99, 10);
insert into sales(pid, price, quantity) values(2, 89.99, 10);
insert into sales(pid, price, quantity) values(2, 79.99, 20);

//now we have 2 products and 6 sales

begin transaction;
select pid, count(pid) from sales group by pid;
//this will be 2, 3 and 1,3


select pid, price, quantity from sales;
//this will be 7 rows, but was 6.

//same time 
begin transaction;
insert into sales(pid, price, quantity) values(1, 999.99, 10);
update product set inventory = inventory - 10 where pid = 1;
commit;