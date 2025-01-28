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
  
    model ContainerName {
      name: string;
    }

    model AudioUpload {
      file: bytes;
      userId: string;
      LangIn: string;
      LangOut: string;
    }
  
    model NotificationPreferences {
      userId: string;
      emailNotifications: boolean;
    }

    model OperationResponse {
      status: string;
    }

    model SuccessResponse {
      @statusCode statusCode:200;
      message: string;
    }
        
    model PaymentResponse {
      @statusCode statusCode:200;
      message: string;
      userId: string;
    }
  }