# iJoozEWallet.API
 ASPNETCORE_ENVIRONMENT should be changed to Staging or Production based on deployment environment.
# For Non-dev environment
  Read SigningKey(base64 encoded) from environment variable 
  <br/>Read DbConnectionString from environment variable
  
  
##Deploy to google cloud

Refer to https://codelabs.developers.google.com/codelabs/cloud-app-engine-aspnetcore/#1

0. Install gcloud cli
1. dotnet publish -c Release
2. cd to publish folder, copy app.yaml to this folder
3. gcloud config set project fvmembership-ewallet
4. gcloud beta app deploy
