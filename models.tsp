namespace AudioTranslationService.Models {
    model User {
      userId: string;
      name: string;
      email: string;
      password: string;
    }
  
    model Payment {
      paymentId: string;
      userId: string;
      amount: float32;
      languageOptions: string[];
      service: string;
    }
  
    model AudioUpload {
      uploadId: string;
      fileName: string;
      fileSize: float32;
      uploadDate: string;
    }
  
    model TranslationJob {
      jobId: string;
      uploadId: string;
      status: string;
      progress: float32;
      createdAt: string;
    }
  
    model NotificationPreferences {
      userId: string;
      emailNotifications: boolean;
    }
  }