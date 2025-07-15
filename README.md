# Audio Translation Service

This repository contains a full-stack audio translation service with backend (ASP.NET Core), frontend (React/TypeScript), CLI, and SDKs. It supports user registration, payment, audio uploads, translation, and notifications, and is designed for deployment on Azure.

## Table of Contents

- [Project Overview](#project-overview)
- [Azure Architecture](#azure-architecture)
- [Required Azure Resources](#required-azure-resources)
- [Configuration](#configuration)
- [Deployment Instructions](#deployment-instructions)
- [Frontend Setup](#frontend-setup)
- [Security Best Practices](#security-best-practices)
- [Troubleshooting](#troubleshooting)
- [Testing the User Journey](#testing-the-user-journey)

## Project Overview

- **Backend:** ASP.NET Core API for user, payment, audio upload, translation, notifications.
- **Frontend:** React/TypeScript web app.
- **Azure Integration:** Blob Storage, Cognitive Services (Speech/Translation), Communication Services (email).

## Azure Architecture

- **App Service:** Hosts backend API.
- **Blob Storage:** Stores uploaded audio and translation artifacts.
- **Cognitive Services:** Speech/Translation for audio processing.
- **Communication Services:** Email notifications.
- **Static Web Apps/App Service:** Hosts frontend.

## Required Azure Resources

Provision the following in your Azure subscription:
- **Azure Blob Storage Account**
- **Azure Cognitive Services (Speech/Translation)**
- **Azure Communication Services**
- **Azure App Service (for backend)**
- **Azure Static Web Apps or App Service (for frontend)**
- *(Optional)* **Azure Key Vault** for secrets

## Configuration

### Backend (appsettings.json or environment variables)

```json
{
  "BlobStorage": {
    "AccountName": "<your-storage-account-name>",
    "AccountKey": "<your-storage-account-key>"
  },
  "CognitiveServices": {
    "SpeechSubscriptionKey": "<your-cognitive-services-key>",
    "SpeechRegion": "<your-cognitive-services-region>",
    "TranslatorEndpoint": "https://api.cognitive.microsofttranslator.com",
    "TranslatorApiKey": "<your-translator-api-key>"
  },
  "AzureCommunicationServices": {
    "ConnectionString": "<your-communication-services-connection-string>",
    "MailFromAddress": "<your-mail-from-address>",
    "BaseUrl": "<your-backend-app-service-url>" // The public URL of your backend App Service, used for links in notification emails
  }
}
```

Do NOT commit or upload appsettings.json to the public repo.

**How to set secrets for production:**
- Go to your Azure App Service in the Azure Portal.
- Under "Configuration" > "Application settings", add each key/value from appsettings.json as an environment variable.
- The GitHub Actions workflow will deploy your code, but secrets must be set in Azure, not in the repo or workflow.

If you want to automate secret management, use Azure CLI or GitHub Actions to set environment variables, but never commit secrets to source control.

**Note:**  
- `BaseUrl` in `AzureCommunicationServices` should be set to your backend App Service public URL (e.g., `https://your-backend-app-service.azurewebsites.net`).  
  This value is used by the backend to generate user-facing links to API endpoints (such as status pages or download endpoints) in notification emails and other communications.  
  If downloadable artifacts are served directly from Azure Blob Storage, the backend will generate SAS URLs pointing to your Blob Storage account, not to `BaseUrl`. In that case, users receive direct blob URLs for downloads.
- The Communication Services `ConnectionString` is only for sending emails, not for API endpoints.

### Frontend

Set the backend API endpoint in `src/services/apiClient.ts`:
```ts
const endpoint = "https://<your-backend-app-service>.azurewebsites.net";
```
For production, use environment variables or build-time config.

## Deployment Instructions

### Backend (ASP.NET Core)

1. **Build and publish:**
   ```sh
   dotnet publish -c Release
   ```
2. **Deploy to Azure App Service:**
   - Create an App Service.
   - Set environment variables or upload `appsettings.json` (do not commit secrets).
   - Deploy published files via Azure Portal, CLI, or GitHub Actions.

### Frontend (React/TypeScript)

1. **Build:**
   ```sh
   npm install
   npm run build
   ```
2. **Deploy to Azure Static Web Apps or App Service:**
   - For Static Web Apps: Connect to GitHub repo, configure build.
   - For App Service: Upload build output (`build/` folder).

## Security Best Practices

- **Use Azure Key Vault** for secrets.
- **Restrict permissions** on storage and services.
- **Use HTTPS** for all endpoints.
- **Set short-lived SAS tokens** for Blob Storage.
- **Audit and monitor** access.

## Troubleshooting

- **Missing environment variables:** App will fail to start; check Azure App Service config.
- **Blob Storage errors:** Verify account name/key and container permissions.
- **Cognitive Services errors:** Check subscription key/region.
- **Email issues:** Confirm Communication Services connection string and sender address.
- **Frontend API errors:** Ensure endpoint matches deployed backend URL.

## Testing the User Journey

Use Thunder Client or Postman to test:
1. **Register User**
2. **Process Payment**
3. **Upload Audio**
4. **Start Translation**
5. **Check Status**
6. **Download Artifact**
7. **View Registered Users**

See example requests in previous README sections.

---

**This project is ready for Azure deployment. Configure all required Azure resources, set environment variables, and deploy backend and frontend as described above.**
