SELECT table_name
FROM
    `bigquery-pulic-ata`.census_bureau_usa.INFORMATION_SCHEMA.TABLES
WHERE
    table_name ="population_by_zip_2010"