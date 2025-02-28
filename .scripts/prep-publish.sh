#!/bin/bash
#To make the .sh file executable
#sudo chmod +x ./publish.sh
if [[ -z "$1" ]]; then
    printf "\nMissing parameter.\n Parameter should be directory name of application to publish\n"
    exit 1
fi
Application=$1

#get the current directory, which is the .scripts directory in the solution directory
PWDIR=$(pwd)

#set paths and project from the Configuration.csproj file
ApplicationProjectFile="$PWDIR/../Configuration/Configuration.csproj"
AzureScriptDirectory=$(sed -n 's:.*<AzureScriptDirectory>\(.*\)</AzureScriptDirectory>.*:\1:p' $ApplicationProjectFile)
AzureProjectSettings=$(sed -n 's:.*<AzureProjectSettings>\(.*\)</AzureProjectSettings>.*:\1:p' $ApplicationProjectFile)
ApplicationDirectory="$PWDIR/../$Application"
PublishDirectory="$PWDIR/../$Application/publish"

printf "\n\nSetup parameters\n"
echo "ApplicationProjectFile="$ApplicationProjectFile
echo "AzureScriptDirectory="$AzureScriptDirectory
echo "AzureProjectSettings="$AzureProjectSettings
echo "ApplicationDirectory="$ApplicationDirectory
echo "PublishDirectory="$PublishDirectory
#exit

#Step1: Set the Azure Keyvault access parameters as operating system environment variables.
cd $AzureScriptDirectory

printf "\n\nSetting the azure key vault access as environent variables"
export AZURE_TENANT_ID=$(./az-access.sh $AzureProjectSettings tenantId)
export AZURE_KeyVaultUri=$(./az-access.sh $AzureProjectSettings kvUri)
export AZURE_KeyVaultSecret=$(./az-access.sh $AzureProjectSettings kvSecret)

export AZURE_CLIENT_ID=$(./az-access-secrets.sh $AzureProjectSettings app appId)
export AZURE_CLIENT_SECRET=$(./az-access-secrets.sh $AzureProjectSettings app password)

#Set the environment variables to the ws application
printf "\n\nSet the environment variables to the azure web application"
./az-apps-set-env.sh $AzureProjectSettings

cd $PWDIR

#verify environment variables
echo "AZURE_TENANT_ID=" $AZURE_TENANT_ID
echo "AZURE_KeyVaultUri=" $AZURE_KeyVaultUri
echo "AZURE_KeyVaultSecret=" $AZURE_KeyVaultSecret
echo "AZURE_CLIENT_ID=" $AZURE_CLIENT_ID
echo "AZURE_CLIENT_SECRET=" $AZURE_CLIENT_SECRET

#Step2: Generate the release files
printf "\n\nPublish the webapi...\n"
# #remove any previous publish
rm -rf $PublishDirectory

cd $ApplicationDirectory
dotnet publish --configuration Release --output ./publish


#Step3: Run the application from the folder containing the release files.
printf "\n\nRun the webapi from the published directory...\n"
cd $PublishDirectory

exec ./$Application


