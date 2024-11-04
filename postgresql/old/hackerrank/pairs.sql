with f_ordered(x, y, rn)
as 
(
     select f1.x, f1.y, row_number() over (order by f1.x)  from functions f1
)

select distinct f1.x, f1.y from f_ordered f1
join f_ordered f2 on 
    f1.x = f2.y and 
    f2.x = f1.y and 
    f1.rn <> f2.rn
where f1.x <= f1.y
order by f1.x