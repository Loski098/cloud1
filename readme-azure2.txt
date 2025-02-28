This text shows the steps to publish run AppWebApi from Azure App Services.
First preparation to publish is done by building and running production AppWebApi locally.
The the actual publishing of AppWebApi is done.
Finally the AppWebApi running on Azure is launched in the browser

You need to have done the steps in readme-azure1.txt before doing below steps.


Prepare to publish by building for production but running locally
-----------------------------------------------------------------

1. With Terminal in folder .scripts run
   ./prep-publish.sh AppWebApi

2. After completion you will see:
Run the webapi from the published directory...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/Martin/Development/goldenProjects/WebApiTemplate_branches/AppWebApi/publish
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.

2. open url: ttp://localhost:5000/swagger

3. Verify database seed with endpoint Guest/Info. You will see the overview of the local database content

4. Use endpoint Guest/LoginUser to login as sysadmin1
{
  "userNameOrEmail": "sysadmin1",
  "password": "sysadmin1"
}

5. Authorize using Swagger Authorize butto and paste in the encryptedToken recieved after login.
    NOTE!!: Copy and paste the encryptedToken WITHIN the quotation, i.e. WITHOUT the first and last quotation mark "

6. Verify connections and setup with endpoint Admin/Info. Output should be
   {
   "appEnvironment": "Production",
   "secretSource": "Azure: Tanzanite",
   "dataConnectionTag": "zooefc.azure",
   "defaultDataUser": "sysadmin",
   "migrationDataUser": "sysadmin",
   "dataConnectionServer": 0,
   "dataConnectionServerString": "SQLServer"
   }


Publish AppWebApi to Azure App Service
--------------------------------------

7. In Azure tab on VSC open App Services find you groups App Service, for example SYS6-Tanzanite-ws-6755B002D1BB

8. Right click on the App Service and select "Deploy to Web App...". Click deploy. 

9. Choose "skip for now" if VSC is asking to "Always to deploy to workspace" deployment


Launch AppWebApi running on Azure in the browser
------------------------------------------------

10. After deployment browse to website. E.g.
   https://sys6-tanzanite-ws-6755b002d1bb.azurewebsites.net/swagger

   You can rightclick on the App Service to deployed to and select "Browse Website"
   
   NOTE!!: remember to add /swagger at the end of the url to see the swagger interface.

