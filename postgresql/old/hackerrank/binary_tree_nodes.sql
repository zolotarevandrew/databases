SELECT bs.n, 
CASE
    WHEN p is null
        THEN 'Root'
    WHEN NOT EXISTS (SELECT p from bst where p = bs.n )
        THEN 'Leaf'
    ELSE 'Inner'
END AS type
FROM bst as bs
ORDER BY bs.n;