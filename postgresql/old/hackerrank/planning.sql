with e_projects(task_id, start_date, end_date, rn)
as
(
     select *, row_number() over (order by start_date) from projects
),
partitions (start_date, end_date, partition_number)
as 
(
     select 
       p1.start_date, 
       p1.end_date, 
       sum(case when p2.rn is null then 1 else 0 end) over (order by p1.start_date) AS partition_number
   from e_projects p1
   left join e_projects p2 on p1.start_date = p2.end_date
)

select min(start_date), max(end_date) from partitions
group by partition_number
order by datediff(day, min(start_date), max(end_date)) - 1 asc, min(start_date)
