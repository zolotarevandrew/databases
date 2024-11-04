drop table products;
drop table sales;

create table products (pid serial primary key, name text, price float, inventory integer);
create table sales(saleid serial primary key, pid integer, price float, quantity integer);

insert into products(name, price, inventory) values ('Phone', 999.999, 100);


begin transaction;
select * from products;
update products set inventory = inventory  - 10;
select * from products;

//should be 100
select * from products;


begin transaction;
update products set inventory = inventory  - 10;
insert into sales(pid, price, quantity) values (1, 999.99, 10);

select * from sales;
commit;


//different terminal
select * from products
//will give 100

select * from sales
//will be empty