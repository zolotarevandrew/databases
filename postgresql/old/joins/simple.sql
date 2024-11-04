CREATE TABLE basket_a (
    a INT PRIMARY KEY,
    fruit_a VARCHAR (100) NOT NULL
);

CREATE TABLE basket_b (
    b INT PRIMARY KEY,
    fruit_b VARCHAR (100) NOT NULL
);

INSERT INTO basket_a (a, fruit_a)
VALUES
    (1, 'Apple'),
    (2, 'Orange'),
    (3, 'Banana'),
    (4, 'Cucumber');

INSERT INTO basket_b (b, fruit_b)
VALUES
    (1, 'Orange'),
    (2, 'Apple'),
    (3, 'Watermelon'),
    (4, 'Pear');

//will use hash join
explain analyze SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
INNER JOIN basket_b
    ON fruit_a = fruit_b;

//will use hash left join
SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
LEFT JOIN basket_b 
   ON fruit_a = fruit_b;

//left outer join, uses hash left join
explain analyze SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
left JOIN basket_b 
    ON fruit_a = fruit_b
WHERE b IS NULL;

//right join, uses hash left join
explain analyze SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
RIGHT JOIN basket_b ON fruit_a = fruit_b;

//outer right join, uses hash left join
explain analyze SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
RIGHT JOIN basket_b 
   ON fruit_a = fruit_b
WHERE a IS NULL;

//full join, uses hash full join
explain analyze SELECT
    a,
    fruit_a,
    b,
    fruit_b
FROM
    basket_a
FULL OUTER JOIN basket_b 
    ON fruit_a = fruit_b;
	
//cross join, uses nested loop	
explain analyze SELECT *
FROM basket_a
CROSS JOIN basket_b;

//anti join, uses hash anti join
EXPLAIN SELECT * 
FROM basket_a a 
WHERE NOT EXISTS (
  SELECT * FROM basket_b b WHERE b.fruit_b = a.fruit_a 
);

//semi join
EXPLAIN SELECT * FROM basket_a a 
WHERE EXISTS (
  SELECT * FROM basket_b b WHERE b.fruit_b = a.fruit_a 
);

