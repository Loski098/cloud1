This text shows the steps to run AppWebApi locally using .NET User Secrets and SQL server running in docker

1. Make sure that belwo two keys in appsettings.json in the folders AppWebApi and DbContext are set to following:
      "UseAzureKeyVault": false
      "UseDataSetWithTag": "zooefc.localhost.docker"
   This ensures you will use user secrets and docker on your local development computer.

2. Make sure you have the SQLServer2022 container running in Docker desktop

3. With Terminal in folder .scripts 
   ./database-rebuild-all.sh local
   Ensure no errors from build, migration or database update

4. From Azure Data Studio you can now connect to the database
   Use connection string from user secrets:
   connection string corresponding to Tag
   "zooefc.localhost.docker"

5. Use Azure Data Studio to execute SQL script DbContext/SqlScripts/initDatabase.sql on the database zooefc

6. Run AppWebApi with or without debugger

   Without debugger:   
   Open a Terminal in folder AppWebApi run: 
   dotnet run -lp https 
   open url: https://localhost:7066/swagger

   Verify output from endpoint Admin/Info. Output should be
   {
   "appEnvironment": "Development",
   "secretSource": "Usersecret: 126a42a7-3b4f-429f-997d-1849782efbb0",
   "dataConnectionTag": "zooefc.localhost.docker",
   "defaultDataUser": "sysadmin",
   "migrationDataUser": "sysadmin",
   "dataConnectionServer": 0,
   "dataConnectionServerString": "SQLServer"
   }

   Verify database seed with endpoint Guest/Info. You will see the overview of the local database content
   {
   "item": {
      "db": {
         "title": "Guest user database overview",
         "nrSeededZoos": 0,
         "nrUnseededZoos": 0,
         "nrSeededAnimals": 0,
         "nrUnseededAnimals": 0,
         "nrSeededEmployees": 0,
         "nrUnseededEmployees": 0,
         "nrSeededCreditCards": 0,
         "nrUnseededCreditCards": 0
      },
      "zoos": [],
      "animals": [],
      "employees": []
   },
   "dbConnectionKeyUsed": "SQLServer-zooefc-docker-sysadmin"
   }

7. Use endpoint Admin/SeedUsers to seed users into the the database

8. Use endpoint Guest/LoginUser to login as sysadmin1
{
  "userNameOrEmail": "sysadmin1",
  "password": "sysadmin1"
}

9. Authorize using Swagger Authorize butto and paste in the encryptedToken recieved after login.
    NOTE!!: Copy and paste the encryptedToken WITHIN the quotation, i.e. WITHOUT the first and last quotation mark "

10. Use endpoint Admin/Seed to seed the database, Admin/RemoveSeed to remove the seed
   Verify database seed with endpoint Guest/Info

11. As sysadmin you can now use and play with all endpoints


