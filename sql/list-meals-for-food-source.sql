SELECT      m.Id,
            m.Food_Source_Id,
            m.Name,
            fs.Name
FROM        MEALS m
INNER JOIN  FOOD_SOURCES fs ON fs.Id = m.food_source_id
WHERE       fs.Name = 'Home';
