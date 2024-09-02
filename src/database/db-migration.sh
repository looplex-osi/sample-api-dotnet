#!/bin/bash

echo "Running migrations"

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P !ooplex_D0tNet!s@ -d master -i db-dump.sql

for file in "migrations"/*.sql; do
    if [ -f "$file" ]; then
        echo "$file"
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P !ooplex_D0tNet!s@ -d master -i "$file"
    fi
done