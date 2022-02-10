drop table seats;

create table seats(id serial primary key, is_booked boolean, name text);

insert into seats (id, is_booked, name) values (13, false, '');
insert into seats (id, is_booked, name) values (14, false, '');
insert into seats (id, is_booked, name) values (15, false, '');


solution

//first
begin transaction;
//1 - will be locked for 3 seconds
SET LOCAL lock_timeout = '3s';
select * from seats where id = 15 for update;
//4 - will be unblocked

//second
begin transaction;
//1 - will be selected
SET LOCAL lock_timeout = '3s';
select * from seats where id = 15 for update;
//SQL Error [55P03] ERROR: canceling statement due to lock timeout Where: while locking tuple (0,6) in relation "seats"


