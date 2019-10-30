# iJoozEWallet.API
 ASPNETCORE_ENVIRONMENT should be changed to Staging or Production based on deployment environment.
# For Non-dev environment
  Read SigningKey(base64 encoded) from environment variable 
  <br/>Read DbConnectionString from environment variable
  
  
## Deploy to google cloud

Refer to https://codelabs.developers.google.com/codelabs/cloud-app-engine-aspnetcore/#1

0.Register your application for Cloud SQL Admin API in Google Cloud Platform
https://console.cloud.google.com/flows/enableapi?apiid=sqladmin&redirect=https:%2F%2Fconsole.cloud.google.com&_ga=2.244852229.-1628980075.1568042064&_gac=1.216462692.1569770225.Cj0KCQjwrMHsBRCIARIsAFgSeI0nvZjimGuKWdqhSpTUtXBFyzfCKZV3ObOk-r0GqVcIRGWRBbka0LMaAge9EALw_wcB

0. Install gcloud cli
1. dotnet publish -c Release
2. cd to publish folder, copy app.yaml to this folder
3. gcloud config set project fvmembership-ewallet
4. gcloud beta app deploy


## Connecting to Cloud SQL from App Engine
https://cloud.google.com/sql/docs/mysql/connect-app-engine