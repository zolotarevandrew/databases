
select s.name from students s
join friends f on f.id = s.id
join packages p on p.id = s.id
join packages p1 on p1.id = f.friend_id
where p1.salary > p.salary
order by p1.salary