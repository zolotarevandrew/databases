DECLARE @i int = 20

WHILE @i <> 0
BEGIN  
    select REPLICATE('* ', @i)
    SET @i = @i - 1
END