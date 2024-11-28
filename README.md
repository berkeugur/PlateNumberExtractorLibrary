# PlateNumberExtractorLibrary

A .NET library for extracting license plate numbers from base64-encoded images using OpenAI's API. Designed for seamless integration with any .NET application.

---

## Features

- Extract license plate numbers from base64 images.
- Dependency Injection support with `IHttpClientFactory`.
- Easy-to-use API for developers.
- Built-in error handling and validation.

---

## Requirements

- .NET 6.0 or later
- OpenAI API Key
- NuGet packages:
  - `Microsoft.Extensions.DependencyInjection`
  - `System.Text.Json`

---

## Installation

1. Add the library to your project:
   ```bash
   dotnet add package PlateNumberExtractorLibrary

dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package System.Text.Json


Usage
## STEP 1. Configure Dependencies
 Register IHttpClientFactory using Microsoft.Extensions.DependencyInjection.

## Step 2: Instantiate the Extractor
- Create an instance of PlateNumberExtractor with your API key and HTTP client factory.

Example Code:
    
    using PlateNumberExtractorLibrary;
    using Microsoft.Extensions.DependencyInjection;
    using System.Net.Http;

    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var apiKey = "your_openai_api_key";

            var extractor = new PlateNumberExtractor(httpClientFactory, apiKey);

            var base64Image = "your_base64_image_string";
            var plateNumber = await extractor.ExtractPlateNumberFromBase64Image(base64Image);

            Console.WriteLine($"Extracted Plate Number: {plateNumber}");
        }
    }