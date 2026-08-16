SELECT      m.Name, COUNT( fs.Id ) AS "Count"
FROM        MEALS m
INNER JOIN  FOOD_SOURCES fs ON fs.Id = m.food_source_id
WHERE       fs.Id IN ( 0 )
GROUP BY    m.Name
HAVING      COUNT( fs.Id ) > 1;
    