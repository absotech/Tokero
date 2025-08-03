# Tokero Trading - Interview Project

This is a mobile application developed as part of the interview process for Tokero. It's a simple investment simulation app that allows users to create investment plans and track the performance of selected cryptocurrencies.

## About The Project

The core functionality of this application is to simulate cryptocurrency investments. Users can create an account, define investment plans with specific start dates and amounts, and select from a list of cryptocurrencies. The application then fetches real-time price data to calculate the potential returns on these simulated investments.

The only external service used is the **CoinMarketCap API**, which provides up-to-date pricing for the cryptocurrencies.

### Built With

* [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* MAUI (for cross-platform UI)
* SQLite (for local database storage)

## Getting Started

To get a local copy up and running, please follow these simple steps.

### Prerequisites

* **.NET 8.0 SDK**
* **Android Environment**: An Android emulator (via Android Studio or Visual Studio) or a physical Android device is recommended for the best experience.

### Installation

1.  **Clone the repo**
    ```sh
    git clone [https://github.com/absotech/Tokero.git](https://github.com/absotech/Tokero.git)
    ```
2.  **Open the solution** in Visual Studio.
3.  **Restore NuGet packages**: Visual Studio should do this automatically. If not, right-click the solution and select "Restore NuGet Packages".
4.  **Run the application**: Select an Android emulator or a connected Android device from the debug target dropdown and press the "Run" button.

## Usage

Upon launching the application for the first time, you can log in with the default user credentials:

* **Username:** `admin`
* **Password:** `admin`

Once logged in, you can create new investment plans, add cryptocurrencies to them, and view their simulated performance.

## Database

The application utilizes one local SQLite database, with three tables:

1.  **User Table**: Stores user login information. It is initialized with a default `admin` user. Please note that for this test application, the default password (`admin`) is stored in plain text. I am aware of security best practices, such as hashing passwords with bcrypt (which is my go-to option on my Go, PHP, and .NET backends), but this was omitted to simplify and speed up development for this exercise.
2.  **Investment Plans Table**: Stores all the investment plans created by the user.
3.  **Configured Assets Table**: Stores all the cryptocurrencies associated with their Investment Plans created by the user.


The database is created locally on the device, requiring no external database setup.

## API Key Management

The CoinMarketCap API key is currently hardcoded within the application.

**This is a deliberate choice for this test project.** The key is a free, disposable one that I do not use for any other purpose. I am fully aware that for a production environment, best practices dictate that API keys and other secrets should be stored securely in a backend service, environment variables, or a secure key vault, and never committed directly into the source code. This approach was taken purely to expedite the development of this test application.

## Platform Specifics

### Android

* **Minimum Version:** Android 5.0 (API Level 21)
* **Target Version:** Android 14.0 (API Level 34)

The application has been primarily developed and tested on the Android platform and is expected to work smoothly on both emulators and physical devices.

### iOS

* **Minimum Version:** 11.0
* **Target Version:** Not specified.

**The application has not been tested on iOS.** Due to a lack of access to a Mac or an iPhone, I was unable to build, deploy, or debug the application for the iOS platform.

## Design Choices & Simplifications

During initial development, a feature for a recurring "Investment Day of the Month" was considered for each plan. This was ultimately removed to streamline the project's scope. This feature was a separate variable that added significant complexity to the Dollar-Cost Averaging (DCA) calculator service.

Instead, the application allows for each selected cryptocurrency within an investment plan to have its own distinct start date, providing flexibility without the overhead of managing recurring investment dates.
