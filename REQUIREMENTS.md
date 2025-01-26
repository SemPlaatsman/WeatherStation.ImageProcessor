# Requirements
This project featured a wide array of requirements that were either Must- or Could-have requirements. In the following section is explained where the implementation of this requirement can be found and/or a quick explanation on how this is fulfilled. 

## Must
* `Expose publicly accessible API for requesting a set of fresh images with current weather data using a HttpTrigger.`
    * ✅ Implemented in the [RequestWeatherImagesFunction](./WeatherStation.ImageProcessor.Functions/Functions/RequestWeatherImagesFunction.cs) class, a `HttpTrigger` is employed for requesting a set of fresh images.
* `Employ QueueTrigger to process the job in the background so the initial call stays fast.`
    * ✅ Implemented in the [InitiateImageGenerationFunction](./WeatherStation.ImageProcessor.Functions/Functions/InitiateImageGenerationFunction.cs) class, a `QueueTrigger` is employed to initiate the request, instead of doing this in the initial `HttpTrigger`. The initial `HttpTrigger` uses the [InitiationQueueService](./WeatherStation.ImageProcessor.Infrastructure/Services/InitiationQueueService.cs) to send a message to this queue.
* `Employ Blob Storage to store all generated images and to expose the files.`
    * ✅ Implemented in the [ImageRepository](./WeatherStation.ImageProcessor.Infrastructure/Repositories/ImageRepository.cs), a `BlobContainerClient` is utilized to communicate with blob storage and store all generated images in one folder. To expose the files, a SAS token is generated and returned in the blob storage URI.
* `Employ Buienrader api to get weather station data: https://data.buienradar.nl/2.0/feed/json.`
    * ✅ Implemented through a [BuienradarClient](./WeatherStation.ImageProcessor.Infrastructure/External/Clients/BuienradarClient.cs) for fetching weather station data through the Buienradar API.
* `Employ any public api for retrieving an image to write the weather data on: e.g. https://unsplash.com/developers.`
    * ✅ Implemented through an [UnsplashClient](./WeatherStation.ImageProcessor.Infrastructure/External/Clients/UnsplashClient.cs) for fetching an image through the Unsplash API.
* `Expose a publicly accessible API for fetching the generated images using HttpTriggers.`
    * ✅ Implemented through two endpoints:
        1. `/RequestWeatherImages`: Request a set of weather images; Returns a jobId.
        2. `/jobs/{jobId}/status`: Get the current status of the weather image processing queue. Can have status `Initiated`, `Processing`, or `Completed`. Will show the completed jobs.
* `Provide HTTP files as API documentation.`
    * ✅ Implemented through the [`req.http`](./req.http) file.
* `Create a fitting Bicep template (include the queues as well).`
    * ✅ Implemented in [`deploy/main.bicep`](./deploy/main.bicep), which includes all required resources including queues, storage accounts, and function app.
* `Add all files to Azure DevOps repo and add *REDACTED* to organization and project.`
    * ❌ Not completed due to Azure subscription access limitations. Code is currently maintained in GitHub. Azure DevOps organization creation requires an active Azure subscription.
* `Create a deploy.ps1 script that publishes your code using the dotnet cli, creates the resources in azure using the Bicep template and deploys the function using the azure cli.`
    * ✅ Implemented in [`deploy/deploy.ps1`](./deploy/deploy.ps1), which handles building, publishing, and deploying the application.
* `Employ multiple queues one for starting the job and one for writing text on a single image.`
    * ✅ Implemented through two queues:
        1. [`InitiateImageGenerationFunction`](./WeatherStation.ImageProcessor.Functions/Functions/InitiateImageGenerationFunction.cs): Used for starting the image processing job. 
        2. [`ProcessWeatherImageFunction`](./WeatherStation.ImageProcessor.Functions/Functions/ProcessWeatherImageFunction.cs): Used for processing a weather image (which includes writing text on a single image).
* `Deploy the code to azure and have a working endpoint.`
    * ❌ Not completed due to Azure subscription access limitations. All infrastructure code and deployment scripts are prepared and tested locally.

## Could
* `Use SAS token instead of publicly accessible blob storage for fetching finished image directly from Blob.`
    * ✅ Implemented in the [`ImageRepository`](./WeatherStation.ImageProcessor.Infrastructure/Repositories/ImageRepository.cs) which creates a client to communicate with Blob Storage and sets the Public Access Type to None. It also provides a `GenerateSasUrl` method which can be used to add a Read-Only SAS token to the URL, which provides the user with access to the images.
* `Build and deploy the code automatically from Azure DevOps.`
    * ❌ Not implemented due to Azure subscription access limitations.
* `Use authentication on request API. (Be sure to provide me with credentials)`
    * ❌ Not implemented.
* `Provide a status endpoint for fetching progress status and saving status in Table.`
    * ✅ Implemented through `/jobs/{jobId}/status` endpoint which gets the current status of the weather image processing queue. Can have status `Initiated`, `Processing`, or `Completed` which is saved in the `WeatherJobs` table in Table Storage.

> [!IMPORTANT]
> The application is fully functional in the local development environment with Azurite storage emulator. Deployment-related requirements are blocked due to temporary Azure subscription access limitations. All necessary code, infrastructure definitions, and deployment scripts are prepared for immediate deployment once access is restored.