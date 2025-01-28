### Documentation: Generating SAS Tokens for Upload and Read-Only Download Access

This documentation explains how to generate SAS tokens for upload and read-only download access to an Azure Blob Storage container, including sample code in C#.

#### Prerequisites
- Azure Storage account name and key
- Azure.Storage.Blobs NuGet package

### 1. Setting Up Your Environment

Install the `Azure.Storage.Blobs` package via NuGet:
```sh
Install-Package Azure.Storage.Blobs
```

### 2. Generating a SAS Token for Upload

#### Step-by-Step Guide

1. **Create a StorageSharedKeyCredential**:
   ```csharp
   var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
   ```

2. **Create a BlobServiceClient**:
   ```csharp
   var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), sharedKeyCredential);
   ```

3. **Get a Reference to the Container**:
   ```csharp
   var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
   ```

4. **Generate the SAS Token**:
   ```csharp
   var sasBuilder = new BlobSasBuilder
   {
       BlobContainerName = containerClient.Name,
       Resource = "c", // "c" for container
       ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Set the expiry time
   };

   sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create);

   var sasToken = sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();
   var sasUri = new Uri($"{containerClient.Uri}?{sasToken}");
   ```

### 3. Generating a Read-Only SAS Token for Download

#### Step-by-Step Guide

1. **Create a StorageSharedKeyCredential**:
   ```csharp
   var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
   ```

2. **Create a BlobServiceClient**:
   ```csharp
   var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), sharedKeyCredential);
   ```

3. **Get a Reference to the Container**:
   ```csharp
   var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
   ```

4. **Generate the SAS Token**:
   ```csharp
   var sasBuilder = new BlobSasBuilder
   {
       BlobContainerName = containerClient.Name,
       Resource = "c", // "c" for container
       ExpiresOn = DateTimeOffset.UtcNow.AddDays(10) // Set the expiry time to 10 days
   };

   sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

   var sasToken = sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();
   var sasUri = new Uri($"{containerClient.Uri}?{sasToken}");
   ```

### 4. Sample Code

Here’s a complete example that includes both upload and read-only download SAS token generation:

```csharp
using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

public class SasTokenService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public SasTokenService(string accountName, string accountKey, string containerName)
    {
        var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), sharedKeyCredential);
        _containerName = containerName;
    }

    public Uri GenerateUploadSasToken()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            Resource = "c", // "c" for container
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Set the expiry time
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, _blobServiceClient.AccountKey)).ToString();

        return new Uri($"{containerClient.Uri}?{sasToken}");
    }

    public Uri GenerateReadOnlySasToken()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            Resource = "c", // "c" for container
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(10) // Set the expiry time to 10 days
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, _blobServiceClient.AccountKey)).ToString();

        return new Uri($"{containerClient.Uri}?{sasToken}");
    }
}
```

### Best Practices
- **Use HTTPS**: Ensure all communications use HTTPS to encrypt data in transit.
- **Limit Permissions**: Grant only the necessary permissions (write for upload, read for download).
- **Set Expiry Times**: Use short-lived tokens to minimize security risks.
- **Monitor and Audit**: Regularly audit access logs and monitor for any unusual activity.

This documentation should help you securely manage upload and download access to your blob storage container.