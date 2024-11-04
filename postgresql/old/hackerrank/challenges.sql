WITH HackerCounts AS (
    SELECT hacker_id, COUNT(*) AS cnt
    FROM challenges
    GROUP BY hacker_id
),
MaxHackerCount AS (
    SELECT MAX(cnt) AS max_cnt
    FROM HackerCounts
),
Excluded AS (
    select cnt from HackerCounts
   where cnt < (select max_cnt from MaxHackerCount)
   group by cnt
   having count(cnt) >1
)

select 
    h.hacker_id, 
    h.name, 
    cnt.cnt 
from hackers h
join HackerCounts cnt on cnt.hacker_id = h.hacker_id
where cnt.cnt not in (select cnt from excluded)
order by cnt.cnt desc, h.hacker_id;