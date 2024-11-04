DO
 LANGUAGE plpgsql $$
DECLARE
  row_index int := -1;
BEGIN
    SELECT AVG(subq.amount) as median_value from
(SELECT row_index=row_index + 1 AS idx, amount
  FROM accounts a 
  ORDER BY amount) as subq
WHERE subq.idx 
  IN (FLOOR(idx / 2) , CEIL(idx / 2));
END;
$$;