# UnsplashImage
Azure functions to retreive image stats from unsplash
The values for the apiKey, storageConnectionString and func-code should be stored in a key vault, the others parameters can be store in the function app settings.

The parameters to set the execution is configure to be executed at 4 am every day "cron_schedule": "0 4 * * *" in the deployed function that will be sent by email, and this parameters also can be modified in the function app settings, or by creating the parameter in the local.settings.json file and set the desired value or directly into the function. In the function is configured dependant from the "cron_schedule" variable.

An additional azure function were created in order to test the proper behavior and validate that the right information is been stored in the table storage.
