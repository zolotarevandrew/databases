drop table products;
drop table sales;

create table products (pid serial primary key, name text, price float, inventory integer);
create table sales(saleid serial primary key, pid integer, price float, quantity integer);


insert into sales(pid, price, quantity) values(1, 999.99, 10);
insert into sales(pid, price, quantity) values(1, 999.99, 5);
insert into sales(pid, price, quantity) values(1, 999.99, 4);
insert into sales(pid, price, quantity) values(1, 999.99, 3);
insert into sales(pid, price, quantity) values(1, 999.99, 2);

//now we have 5 sales
begin transaction;
select pid, sum(price) from sales group by pid;
//this will be 1, 4999.95

select pid, sum(price) from sales group by pid;
//this willbe 1, 5999.94

//same time 
insert into sales(pid, price, quantity) values(1, 999.99, 10);


//we can solve this by serializable or repeatable read isolation level

begin transaction isolation level serializable; // or repeatable_read
select pid, sum(price) from sales group by pid;
//this willbe 1, 5999.94
select pid, sum(price) from sales group by pid;
//this willbe 1, 5999.94
rollback;

//same time 
insert into sales(pid, price, quantity) values(1, 999.99, 10);


