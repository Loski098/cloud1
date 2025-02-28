
#!/bin/bash
#To make the .sh file executable
#sudo chmod +x ./database-rebuild-all.sh

if [[ -z "$1" ]]; then
    printf "\nMissing parameter:\n  ./database-rebuild-all.sh [local | azure]\n"
    exit 1
fi

if [[ $1 == "local" ]]; then
    #drop any database
    #NOTE - SQL on Azure do NOT drop the database to prevent extra charging
    #Instead drop the tables, views, stored procedures, schemas etc
    dotnet ef database drop -f -c SqlServerDbContext -p ../DbContext -s ../DbContext
fi

#remove any migration
rm -rf ../DbContext/Migrations

#make a full new migration
dotnet ef migrations add miInitial -c SqlServerDbContext -p ../DbContext -s ../DbContext -o ../DbContext/Migrations/SqlServerDbContext

#update the database from the migration
dotnet ef database update -c SqlServerDbContext -p ../DbContext -s ../DbContext

#to initialize the database you need to run the sql scripts from Azure Data Studio
#../DbContext/SqlScripts/initDatabase.sql
