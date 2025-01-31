import "@typespec/http";
using TypeSpec.Http;

namespace AudioTranslationService.Models {
  
    model Payment {
      userEmail: string;
      amount: float32;
      service: string;
      userId: string;
      synthesizedAudio: boolean;
    }
  @doc("Stores the name of a container used for grouping or referencing data, such as in blob storage. Each container has a unique name.")
  model ContainerName {
    @doc("A unique identifier for the container, ensuring distinct data groupings.")
    name: string;
  }
  
  @doc("Encapsulates data needed for audio file uploads, including file content, user identification, and translation languages.")
  model AudioUpload {
    @doc("The raw audio file content to be uploaded, typically in a binary format.")
    file: HttpPart<File>;
  
    @doc("The unique identifier associated with the user uploading the file.")
    userId: string;
  
    @doc("The source language of the audio content for translation or transcription.")
    LangIn: string;
  
    @doc("The target language for translation or other post-processing of the audio content.")
    LangOut: string;
  }
  
  @doc("Describes a user's email notification preferences, used to determine if notifications should be delivered.")
  model NotificationPreferences {
    @doc("A unique identifier for the user whose preferences are stored.")
    userId: string;
  
    @doc("Indicates whether the user wants to receive email notifications.")
    emailNotifications: boolean;
  }
  
  @doc("Represents the status details of an operation, including refresh or completed states.")
  model OperationResponse {
    @doc("Indicates the current status text of the operation, e.g., 'In Progress' or 'Completed'.")
    status: string;
  }
  
  @doc("Represents a successful result with a predefined HTTP 200 status code, used when the request and processing complete without errors.")
  model SuccessResponse {
    @statusCode statusCode: 200;
    @doc("A brief message providing additional information about the successful operation.")
    message: string;
  }
  
  @doc("Details the outcome of a payment process, including user's ID and a descriptive message, with an HTTP 200 status.")
  model PaymentResponse {
    @statusCode statusCode: 200;
    @doc("A descriptive message indicating the result of the payment.")
    message: string;
  
    @doc("Corresponds to the user who initiated the payment, uniquely identifying them.")
    userId: string;
  }
}