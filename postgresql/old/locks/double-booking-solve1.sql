drop table seats;

create table seats(id serial primary key, is_booked boolean, name text);

insert into seats (id, is_booked, name) values (13, false, '');
insert into seats (id, is_booked, name) values (14, false, '');
insert into seats (id, is_booked, name) values (15, false, '');



solution

//first
begin transaction;
//1 - will be locked
select * from seats where id = 14 for update;
//4 - will be unblocked

//second
begin transaction;
//1 - will be selected
select * from seats where id = 14 for update;
//2
update seats set is_booked = true, name = 'Andrew' where id = 14;
//3
commit;



