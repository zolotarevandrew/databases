SELECT pg_relation_filepath('accounts');
//base/24320/25233

base - pg_default catalog


SELECT oid FROM pg_database WHERE datname = 'test';
//24320

SELECT relfilenode FROM pg_class WHERE relname = 'accounts';
//25233


CREATE EXTENSION pageinspect;
SELECT lower, upper, special, pagesize FROM page_header(get_raw_page('accounts',0));


//attribute storage
SELECT attname, atttypid::regtype, CASE attstorage
  WHEN 'p' THEN 'plain'
  WHEN 'e' THEN 'external'
  WHEN 'm' THEN 'main'
  WHEN 'x' THEN 'extended'
END AS storage
FROM pg_attribute
WHERE attrelid = 'accounts'::regclass AND attnum > 0


ALTER TABLE accounts ALTER COLUMN number SET STORAGE external


