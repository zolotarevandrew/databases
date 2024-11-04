select w.id, tbl.age, tbl.min_coins, tbl.power from wands w
join wands_property wp on wp.code = w.code
join (select 
    wp.age as age, 
    min(w.coins_needed) as min_coins, 
    w.power as power 
from wands w
join wands_property wp on wp.code = w.code
where wp.is_evil = 0
group by wp.age, w.power) tbl
on tbl.min_coins = w.coins_needed and w.power = tbl.power and wp.age = tbl.age
order by tbl.power desc, tbl.age desc;


TBD - rank partition