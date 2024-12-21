# Requirements
This project featured a wide array of requirements to were either a Must- or a Could-have requirement. In the following section is explained where the implementation of this requirement can be found and/or a quick explanation on how this is fulfilled. 

## Must
* `Expose publicly accessible API for requesting a set of fresh images with current weather data using a HttpTrigger.`
    * In the [RequestWeatherImagesFunction](./WeatherStation.ImageProcessor.Functions/Functions/RequestWeatherImagesFunction.cs) class, a `HttpTrigger` is employed for requesting a set of fresh images.
* `Employ QueueTrigger to process the job in the background so the initial call stays fast.`
    * In the [InitiateImageGenerationFunction](./WeatherStation.ImageProcessor.Functions/Functions/InitiateImageGenerationFunction.cs) class, a `QueueTrigger` is employed to initiate the request, instead of doing this in the initial `HttpTrigger`. The initial `HttpTrigger` uses the [InitiationQueueService](./WeatherStation.ImageProcessor.Infrastructure/Services/InitiationQueueService.cs) to send a message to this queue.
* `Employ Blob Storage to store all generated images and to expose the files.`
    * In the [ImageRepository](./WeatherStation.ImageProcessor.Infrastructure/Repositories/ImageRepository.cs), a `BlobContainerClient` is utilized to communicate with blob storage and store all generated images in one folder. To expose the files, a SAS token is generated and returned in the blob storage URI.
* `Employ Buienrader api to get weather station data: https://data.buienradar.nl/2.0/feed/json.`
    * A [BuienradarClient](./WeatherStation.ImageProcessor.Infrastructure/External/Clients/BuienradarClient.cs) was created for fetching weather station data through the Buienradar API.
* `Employ any public api for retrieving an image to write the weather data on: e.g. https://unsplash.com/developers.`
    * An [UnsplashClient](./WeatherStation.ImageProcessor.Infrastructure/External/Clients/UnsplashClient.cs) was created for fetching an image through the Unsplash API.
* `Expose a publicly accessible API for fetching the generated images using HttpTriggers.`
    * Three endpoints provided to the user:
        1. [`/RequestWeatherImages`](TODO_REPLACE_LATER_WITH_URL): Request a set of weather images; Returns a jobId.
        2. [`/jobs/{jobId}/status`](TODO_REPLACE_LATER_WITH_URL): Get the current status of the weather image processing queue. Can have status `Initiated`, `Processing`, or `Completed`. Will show the completed jobs.
* `Provide HTTP files as API documentation.`
* `Create a fitting Bicep template (include the queues as well).`
* `Add all files to Azure DevOps repo and add *REDACTED* to organization and project.`
* `Create a deploy.ps1 script that publishes your code using the dotnet cli, creates the resources in azure using the Bicep template and deploys the function using the azure cli.`
* `Employ multiple queues one for starting the job and one for writing text on a single image.`
    * Two queues are present:
        1. [`InitiateImageGenerationFunction`](./WeatherStation.ImageProcessor.Functions/Functions/InitiateImageGenerationFunction.cs): Used for starting the image processing job. 
        2. [`ProcessWeatherImageFunction`](./WeatherStation.ImageProcessor.Functions/Functions/ProcessWeatherImageFunction.cs): Used for processing a weather image (which includes writing text on a single image).
* `Deploy the code to azure and have a working endpoint.`

## Could

* `Use SAS token instead of publicly accessible blob storage for fetching finished image directly from Blob.`
    * The [`ImageRepository`](./WeatherStation.ImageProcessor.Infrastructure/Repositories/ImageRepository.cs) creates a client to communicate with Blob Storage and sets the Public Access Type to None. It also provides a `GenerateSasUrl` method which can be used to add a Read-Only SAS token to the URL, which provides the user with access to the images.
* `Build and deploy the code automatically from Azure DevOps.`
* `Use authentication on request API. (Be sure to provide me with credentials)`
* `Provide a status endpoint for fetching progress status and saving status in Table.`
    * [`/jobs/{jobId}/status`](TODO_REPLACE_LATER_WITH_URL): Get the current status of the weather image processing queue. Can have status `Initiated`, `Processing`, or `Completed` which is saved in the `WeatherJobs` table in Table Storage.
