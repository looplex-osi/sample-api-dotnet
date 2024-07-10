#!/bin/bash

#wait for the SQL Server to come up
sleep 30s

echo "running set up script"
#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P !ooplex_D0tNet!s@ -d master -i db-dump.sql


if [ -n "$RUN_MIGRATION" ] && [ "$RUN_MIGRATION" == "true" ]; then
    echo "running migration = true"

    for file in "migrations"/*.sql; do
        if [ -f "$file" ]; then
            echo "$file"
            /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P !ooplex_D0tNet!s@ -d master -i "$file"
        fi
    done

else
    echo "running migration = false"
fi