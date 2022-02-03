drop table test;
create table test(t text);

insert into test(t) values('b');
insert into test(t) values('b');
insert into test(t) values('a');
insert into test(t) values('a');

select * from test;


//this all will be a
begin transaction isolation level repeatable read;
update test set t = 'a' where t = 'b';
select * from test;

//this all will be b
begin transaction isolation level repeatable read;
update test set t = 'b' where t = 'a';
select * from test;

//then it will be a a b b



//this all will be a
begin transaction isolation level serializable;
update test set t = 'a' where t = 'b';
select * from test;
commit

//this all will be b
begin transaction isolation level serializable;
update test set t = 'b' where t = 'a';
select * from test;
commit
// ERROR: could not serialize access due to read/write dependencies among transactions Detail: Reason code: Canceled on identification as a pivot, during commit attempt. Hint: The transaction might succeed if retried.