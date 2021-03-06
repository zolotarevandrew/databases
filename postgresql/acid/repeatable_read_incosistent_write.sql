drop table accounts;

CREATE TABLE accounts(
  id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
  number text UNIQUE,
  client text,
  amount numeric
);
INSERT INTO accounts values (1, '1001', 'alice', 1000.00), (2, '2001', 'bob', 100.00), (3, '2002', 'bob', 900.00);

BEGIN ISOLATION LEVEL REPEATABLE READ;
//1
SELECT sum(amount) FROM accounts WHERE client = 'bob';
//3
UPDATE accounts SET amount = amount - 600.00 WHERE id = 2;
COMMIT;


BEGIN ISOLATION LEVEL REPEATABLE READ;
//2
SELECT sum(amount) FROM accounts WHERE client = 'bob';
//4
UPDATE accounts SET amount = amount - 600.00 WHERE id = 3;
COMMIT;

SELECT * FROM accounts WHERE client = 'bob';
//will be -500 and 300
