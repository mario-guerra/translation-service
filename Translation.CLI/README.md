# Translation Service CLI

This is a command-line interface (CLI) tool for interacting with the Translation Service API. It allows you to process payments, upload audio files, and download artifacts.

## Prerequisites

*   .NET SDK 7.0 or later installed.
*   The `TranlsationService.dll` client library (built for .NET 9.0) located at `../clients/csharp/src/bin/Debug/net9.0/TranlsationService.dll` relative to the `Translation.CLI` directory.

## Installation

1.  Clone or download this repository.
2.  Navigate to the `Translation.CLI` directory.
3.  Build the project using the following command:

    ```bash
    dotnet build TranslationServiceCli.sln
    ```

## Usage

The CLI uses the following general structure:

```bash
dotnet run -- [global options] [command] [command options]
```

### Global Options

*   `--api-endpoint`: The base URL for the Translation Service API. This option is required for all commands.

    *   Example: `--api-endpoint "http://localhost:5000"`

### Commands

#### 1. `payment`

Processes a payment.

**Options:**

*   `--user-email`: Email of the user.
*   `--amount`: The payment amount.
*   `--service`: The service being paid for.
*   `--user-id`: The ID of the user.
*   `--synthesized-audio`: Indicates if audio was synthesized (true or false).

**Example:**

```bash
dotnet run -- --api-endpoint "http://localhost:5000" payment --user-email "test@example.com" --amount 10.0 --service "translation" --user-id "user123" --synthesized-audio true
```

**Output:**

```
Performing payment operation using API endpoint: http://localhost:5000
Payment processed successfully:
  Message: Payment successful
  User ID: user123
```

#### 2. `upload`

Uploads an audio file.

**Options:**

*   `--file`: Path to the audio file.
*   `--lang-in`: The input language code (e.g., "en").
*   `--lang-out`: The output language code (e.g., "es").
*   `--user-id`: The ID of the user.

**Example:**

```bash
dotnet run -- --api-endpoint "http://localhost:5000" upload --file "./test.wav" --lang-in "en" --lang-out "es" --user-id "user123"
```

**Output:**

```
Performing upload operation using API endpoint: http://localhost:5000
Audio uploaded successfully:
  Message: Audio uploaded successfully
```

#### 3. `download`

Downloads an artifact.

**Options:**

*   `--container-name`: The name of the container.
*   `--upload-id`: The ID of the upload to download.

**Example:**

```bash
dotnet run -- --api-endpoint "http://localhost:5000" download --container-name "mycontainer" --upload-id "upload123"
```

**Output:**

```
Performing download operation using API endpoint: http://localhost:5000
Artifact downloaded successfully:
File saved to mycontainer_upload123.bin
```

The downloaded artifact will be saved to a file named `{containerName}_{uploadId}.bin` in the same directory where you run the command.

## Error Handling

The CLI includes basic error handling. If an error occurs during an API call, an error message will be printed to the console in red.

## Notes

*   Replace the example API endpoint and file paths with your actual values.
*   The `TranlsationService.dll` client library must be located at the specified relative path for the CLI to function correctly.
