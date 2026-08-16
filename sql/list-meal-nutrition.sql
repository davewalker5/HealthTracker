SELECT      m.Id,
            m.Name,
            fs.Name AS "Source",
            1 AS "Quantity",
            n.Calories / m.Portions,
            n.Fat / m.Portions,
            n.Saturated_Fat / m.Portions,
            n.Protein / m.Portions,
            n.Carbohydrates / m.Portions,
            n.Sugar / m.Portions,
            n.Fibre / m.Portions
FROM        MEALS m
INNER JOIN  FOOD_SOURCES fs ON fs.Id = m.Food_Source_Id
INNER JOIN  NUTRITIONAL_VALUES n ON n.Id = m.Nutritional_Value_Id
WHERE       m.Name IN ( '' );
