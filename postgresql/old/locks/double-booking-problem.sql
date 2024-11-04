drop table seats;

create table seats(id serial primary key, is_booked boolean, name text);

insert into seats (id, is_booked, name) values (13, false, '');
insert into seats (id, is_booked, name) values (14, false, '');
insert into seats (id, is_booked, name) values (15, false, '');


//first tran
begin transaction;
select * from seats where id = 13;
//2
update seats set is_booked = true, name = 'Viktor' where id = 13;
//5 - will be Viktor
select * from seats where id = 13;

//second tran
begin transaction;
select * from seats where id = 13;
//1
update seats set is_booked = true, name = 'Andrew' where id = 13;
//3
commit;
//4 - will be Andrew
select * from seats where id = 13;






