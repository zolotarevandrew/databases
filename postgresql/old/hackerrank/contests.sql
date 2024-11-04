select contest_id, hacker_id, name, sum(total_submissions), sum(total_accepted_submissions), sum(total_views), sum(total_unique_views)   from
(select distinct 
  c.contest_id as contest_id, 
  c.hacker_id as hacker_id, 
  c.name as name, 
  cg.challenge_id as challenge_id,
  CASE WHEN ss.t_s is null THEN 0 else ss.t_s END as total_submissions, 
  CASE WHEN ss.t_a_s is null THEN 0 else ss.t_a_s END as total_accepted_submissions, 
  CASE WHEN vs.t_v is null THEN 0 else vs.t_v END as  total_views, 
  CASE WHEN vs.t_u_v is null THEN 0 else vs.t_u_v END as total_unique_views
from 
  contests c 
  left join colleges cs on cs.contest_id = c.contest_id 
  left join challenges cg on cg.college_id = cs.college_id 
  left join (select challenge_id,
              sum(total_submissions) as t_s,
              sum(total_accepted_submissions) as t_a_s from submission_stats group by challenge_id) ss on ss.challenge_id = cg.challenge_id
  left join (select challenge_id,
              sum(total_views) as t_v,
              sum(total_unique_views) as t_u_v from view_stats group by challenge_id) vs on vs.challenge_id = cg.challenge_id
where cg.challenge_id is not null
) tbl
group by  contest_id, hacker_id, name
having  sum(total_submissions) + sum(total_accepted_submissions) +  sum(total_views) + sum(total_unique_views) > 0
order by contest_id;