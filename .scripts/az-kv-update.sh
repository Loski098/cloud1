#!/bin/bash
#To make the .sh file executable
#sudo chmod +x ./az-kv-update.sh

#get the current directory, which is the .scripts directory in the solution directory
PWDIR=$(pwd)

#set paths and project from the Configuration.csproj file
ApplicationProjectFile="$PWDIR/../Configuration/Configuration.csproj"
AzureScriptDirectory=$(sed -n 's:.*<AzureScriptDirectory>\(.*\)</AzureScriptDirectory>.*:\1:p' $ApplicationProjectFile)
AzureProjectSettings=$(sed -n 's:.*<AzureProjectSettings>\(.*\)</AzureProjectSettings>.*:\1:p' $ApplicationProjectFile)

printf "\n\nSetup parameters\n"
echo "ApplicationProjectFile="$ApplicationProjectFile
echo "AzureScriptDirectory="$AzureScriptDirectory
echo "AzureProjectSettings="$AzureProjectSettings

printf "\n\Updating Keyvault with usersecrets"
#move into the Azure scripts directory
cd $AzureScriptDirectory

#execite the scripts to login and update the keyvault with the user secrets
./az-login.sh $AzureProjectSettings
./az-kv-sec-copy.sh $AzureProjectSettings $ApplicationProjectFile

#come back to this directory
cd $PWDIR