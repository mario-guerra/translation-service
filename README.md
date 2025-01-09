# Audio Translation Service

This repository contains the backend implementation for an audio translation service. The backend is built using ASP.NET Core and supports user registration, payment processing, audio file uploads, and translation services. The project uses in-memory storage for simplicity and testing purposes.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Testing the User Journey](#testing-the-user-journey)
- [Prompts for AI Assistance](#prompts-for-ai-assistance)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Thunder Client Extension for VS Code](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client)
- [MailKit](https://www.nuget.org/packages/MailKit/) for email notifications

## Project Structure

```
Translation.Backend/
├── Translation.Backend.csproj
├── Controllers/
│   ├── NotificationsOperationsController.cs
│   ├── OperationsOperationsController.cs
│   ├── RoutesOperationsController.cs
├── Data/
│   ├── InMemoryStore.cs
├── Operations/
│   ├── NotificationsOperations.cs
│   ├── OperationsOperations.cs
│   ├── RoutesOperations.cs
├── Services/
│   ├── EmailService.cs
├── Program.cs
├── Startup.cs
└── generated/
    ├── controllers/
    │   ├── NotificationsOperationsControllerBase.cs
    │   ├── OperationsOperationsControllerBase.cs
    │   ├── RoutesOperationsControllerBase.cs
    ├── lib/
    │   ├── ArrayConstraintAttribute.cs
    │   ├── Base64UrlConverter.cs
    │   ├── NumericArrayConstraintAttribute.cs
    │   ├── NumericConstraintAttribute.cs
    │   ├── StringArrayConstraintAttribute.cs
    │   ├── StringConstraintAttribute.cs
    │   ├── TimeSpanDurationConverter.cs
    │   ├── UnixEpochDateTimeConverter.cs
    │   ├── UnixEpochDateTimeOffsetConverter.cs
    ├── models/
    │   ├── AudioUpload.cs
    │   ├── ErrorResponse.cs
    │   ├── InputValidation.cs
    │   ├── InvalidFileError.cs
    │   ├── NotificationPreferences.cs
    │   ├── Payment.cs
    │   ├── PaymentFailureError.cs
    │   ├── RateLimiting.cs
    │   ├── SecureStorage.cs
    │   ├── TranslationJob.cs
    │   ├── User.cs
    ├── operations/
    │   ├── INotificationsOperations.cs
    │   ├── IOperationsOperations.cs
    │   ├── IRoutesOperations.cs
    ├── source.md
    └── sourcefiles.py
```

## Setup Instructions

### 1. Clone the Repository

```sh
git clone https://github.com/yourusername/translation-service.git
cd translation-service/Translation.Backend
```

### 2. Create the Project and Solution Files

```sh
dotnet new sln -n AudioTranslationService
dotnet new webapi -n Translation.Backend
dotnet sln AudioTranslationService.sln add Translation.Backend/Translation.Backend.csproj
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
dotnet add package MailKit
dotnet restore
```

### 3. Add the Generated Code

Place all the pre-generated code in the `generated` folder as per the project structure.

### 4. Implement the Scaffolding Code with AI Assistance

## Prompts for AI Assistance

### 1. Create Concrete Implementations for Interfaces

**Prompt:**
```
Please create concrete implementations for the following interfaces: INotificationsOperations, IOperationsOperations, and IRoutesOperations. These implementations should interact with an in-memory data store to perform CRUD operations. The in-memory data store should be represented by a static class named InMemoryStore. The implementations should be placed in the Operations folder.
```

### 2. Create In-Memory Data Store

**Prompt:**
```
Please create a static class named InMemoryStore in the Data folder. This class should contain static lists to store User, AudioUpload, TranslationJob, NotificationPreferences, and Payment objects.
```

### 3. Create Controllers

**Prompt:**
```
Please create controllers for NotificationsOperations, OperationsOperations, and RoutesOperations. These controllers should inherit from the base controllers provided in the generated/controllers folder. The controllers should be placed in the Controllers folder and should initialize the concrete implementations created in the Operations folder.
```

### 4. Create Email Service

**Prompt:**
```
Please create an EmailService class in the Services folder. This class should use the MailKit library to send emails. The class should have a method named SendEmailAsync that takes parameters for the recipient's email address, subject, and body of the email.
```

### 5. Update Startup Configuration

**Prompt:**
```
Please update the Startup.cs file to configure services and middleware. The ConfigureServices method should add controllers and register the concrete implementations and the EmailService for dependency injection. The Configure method should set up routing and endpoints.
```

### 6. Create Program Entry Point

**Prompt:**
```
Please create a Program.cs file to start the web server. This file should use the default host builder and configure the web host to use the Startup class.
```

### 7. Support File Upload

**Prompt:**
```
Please update the RoutesOperations class to support file uploads. The UploadAudioAsync method should accept an IFormFile parameter and store the file content in the in-memory data store. The controller should be updated to handle file uploads.
```

### 8. Create Thunder Client Requests

**Prompt:**
```
Please provide Thunder Client requests to test the following user journey:
1. User inputs their information (name, address, email, phone number).
2. Payment gateway for user to pay for a translation into a different language.
3. Securely upload a single audio recording in MP3 or WAV format.
4. Start the translation process.
5. Check the status of the translation job.
6. Download the translation artifact.
```

### Example Thunder Client Requests

#### Register User
```json
{
  "method": "POST",
  "url": "${baseUrl}/register",
  "body": {
    "type": "json",
    "raw": {
      "UserId": "1",
      "Name": "John Doe",
      "Email": "john.doe@example.com",
      "Password": "password123"
    }
  }
}
```

#### Process Payment
```json
{
  "method": "POST",
  "url": "${baseUrl}/payment",
  "body": {
    "type": "json",
    "raw": {
      "PaymentId": "1",
      "UserId": "1",
      "Amount": 100.0,
      "LanguageOptions": ["transcription", "voice"],
      "Service": "translation"
    }
  }
}
```

#### Upload Audio
```json
{
  "method": "POST",
  "url": "${baseUrl}/upload-audio",
  "body": {
    "type": "form-data",
    "form": [
      {
        "key": "file",
        "value": "path/to/audio.mp3",
        "type": "file"
      },
      {
        "key": "userId",
        "value": "1",
        "type": "text"
      }
    ]
  }
}
```

#### Start Translation
```json
{
  "method": "POST",
  "url": "${baseUrl}/translate",
  "body": {
    "type": "json",
    "raw": {
      "JobId": "1",
      "UploadId": "1",
      "Status": "Pending",
      "Progress": 0,
      "CreatedAt": "2023-10-01"
    }
  }
}
```

#### Check Status
```json
{
  "method": "GET",
  "url": "${baseUrl}/status/1"
}
```

#### Download Artifact
```json
{
  "method": "GET",
  "url": "${baseUrl}/download/1"
}
```

### 9. View Registered Users

**Prompt:**
```
Please create an endpoint to view details of users who have registered. This endpoint should return a list of all registered users. The endpoint should be added to the OperationsOperationsController.
```

#### \Controllers\OperationsOperationsController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Data;

namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsOperationsController : OperationsOperationsControllerBase
    {
        internal override IOperationsOperations OperationsOperationsImpl { get; } = new OperationsOperations();

        [HttpGet]
        [Route("users")]
        public IActionResult GetRegisteredUsers()
        {
            var users = InMemoryStore.Users;
            return Ok(users);
        }
    }
}
```

### 10. Thunder Client Request to View Registered Users

#### View Registered Users
```json
{
  "method": "GET",
  "url": "${baseUrl}/operations/users"
}
```

## Testing the User Journey

### Step 1: User Inputs Their Information

1. Select the "Register" request.
2. Click "Send".
3. Verify that the response indicates the user was registered successfully.

### Step 2: Payment Gateway

1. Select the "Payment" request.
2. Click "Send".
3. Verify that the response indicates the payment was processed successfully.

### Step 3: Securely Upload Audio

1. Select the "Upload Audio" request.
2. Click "Send".
3. Verify that the response indicates the audio was uploaded successfully.

### Step 4: Start Translation

1. Select the "Start Translation" request.
2. Click "Send".
3. Verify that the response indicates the translation job was started successfully.

### Step 5: Check Status

1. Select the "Check Status" request.
2. Click "Send".
3. Verify that the response indicates the status of the translation job.

### Step 6: Download Artifact

1. Select the "Download Artifact" request.
2. Click "Send".
3. Verify that the response indicates the artifact was downloaded successfully.

### Step 7: View Registered Users

1. Select the "View Registered Users" request.
2. Click "Send".
3. Verify that the response returns a list of all registered users.

## Prompts for AI Assistance

### 1. Create Concrete Implementations for Interfaces

**Prompt:**
```
Please create concrete implementations for the following interfaces: INotificationsOperations, IOperationsOperations, and IRoutesOperations. These implementations should interact with an in-memory data store to perform CRUD operations. The in-memory data store should be represented by a static class named InMemoryStore. The implementations should be placed in the Operations folder.
```

### 2. Create In-Memory Data Store

**Prompt:**
```
Please create a static class named InMemoryStore in the Data folder. This class should contain static lists to store User, AudioUpload, TranslationJob, NotificationPreferences, and Payment objects.
```

### 3. Create Controllers

**Prompt:**
```
Please create controllers for NotificationsOperations, OperationsOperations, and RoutesOperations. These controllers should inherit from the base controllers provided in the generated/controllers folder. The controllers should be placed in the Controllers folder and should initialize the concrete implementations created in the Operations folder.
```

### 4. Create Email Service

**Prompt:**
```
Please create an EmailService class in the Services folder. This class should use the MailKit library to send emails. The class should have a method named SendEmailAsync that takes parameters for the recipient's email address, subject, and body of the email.
```

### 5. Update Startup Configuration

**Prompt:**
```
Please update the Startup.cs file to configure services and middleware. The ConfigureServices method should add controllers and register the concrete implementations and the EmailService for dependency injection. The Configure method should set up routing and endpoints.
```

### 6. Create Program Entry Point

**Prompt:**
```
Please create a Program.cs file to start the web server. This file should use the default host builder and configure the web host to use the Startup class.
```

### 7. Support File Upload

**Prompt:**
```
Please update the RoutesOperations class to support file uploads. The UploadAudioAsync method should accept an IFormFile parameter and store the file content in the in-memory data store. The controller should be updated to handle file uploads.
```

### 8. Create Thunder Client Requests

**Prompt:**
```
Please provide Thunder Client requests to test the following user journey:
1. User inputs their information (name, address, email, phone number).
2. Payment gateway for user to pay for a translation into a different language.
3. Securely upload a single audio recording in MP3 or WAV format.
4. Start the translation process.
5. Check the status of the translation job.
6. Download the translation artifact.
```

### 9. View Registered Users

**Prompt:**
```
Please create an endpoint to view details of users who have registered. This endpoint should return a list of all registered users. The endpoint should be added to the OperationsOperationsController.
```

### Example Thunder Client Requests

#### Register User
```json
{
  "method": "POST",
  "url": "${baseUrl}/register",
  "body": {
    "type": "json",
    "raw": {
      "UserId": "1",
      "Name": "John Doe",
      "Email": "john.doe@example.com",
      "Password": "password123"
    }
  }
}
```

#### Process Payment
```json
{
  "method": "POST",
  "url": "${baseUrl}/payment",
  "body": {
    "type": "json",
    "raw": {
      "PaymentId": "1",
      "UserId": "1",
      "Amount": 100.0,
      "LanguageOptions": ["transcription", "voice"],
      "Service": "translation"
    }
  }
}
```

#### Upload Audio
```json
{
  "method": "POST",
  "url": "${baseUrl}/upload-audio",
  "body": {
    "type": "form-data",
    "form": [
      {
        "key": "file",
        "value": "path/to/audio.mp3",
        "type": "file"
      },
      {
        "key": "userId",
        "value": "1",
        "type": "text"
      }
    ]
  }
}
```

#### Start Translation
```json
{
  "method": "POST",
  "url": "${baseUrl}/translate",
  "body": {
    "type": "json",
    "raw": {
      "JobId": "1",
      "UploadId": "1",
      "Status": "Pending",
      "Progress": 0,
      "CreatedAt": "2023-10-01"
    }
  }
}
```

#### Check Status
```json
{
  "method": "GET",
  "url": "${baseUrl}/status/1"
}
```

#### Download Artifact
```json
{
  "method": "GET",
  "url": "${baseUrl}/download/1"
}
```

#### View Registered Users
```json
{
  "method": "GET",
  "url": "${baseUrl}/operations/users"
}
```

## Summary

By following the steps outlined in this README, you can set up and run the Audio Translation Service backend. The provided prompts can be used to generate the necessary scaffolding code and project files with the help of an AI agent. The Thunder Client requests allow you to test the user journey, including user registration, payment processing, audio file uploads, translation status checks, and artifact downloads.

