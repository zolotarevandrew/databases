drop table test;
create table test(id serial primary key);

//first transaction
begin transaction;
insert into test values(21);
//here we blocked because other transaction do not commit
insert into test values(20);

//second transaction
begin transaction;
//here we obtain an exlusive lock
insert into test values(20);
//then we try insert 21
insert into test values(21);


//ERROR: deadlock detected
  Detail: Process 29106 waits for ShareLock on transaction 2596467; blocked by process 28909.
Process 28909 waits for ShareLock on transaction 2596468; blocked by process 29106.
  Hint: See server log for query details.
  Where: while inserting index tuple (0,4) in relation "test_pkey"
  