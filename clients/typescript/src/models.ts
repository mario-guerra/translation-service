// Licensed under the MIT License.

export interface User {
  userId: string;
  name: string;
  email: string;
  password: string;
}

export interface Payment {
  paymentId: string;
  userId: string;
  amount: number;
  languageOptions: string[];
  service: string;
}

export interface AudioUpload {
  uploadId: string;
  fileName: string;
  fileSize: number;
  uploadDate: string;
}

export interface TranslationJob {
  jobId: string;
  uploadId: string;
  status: string;
  progress: number;
  createdAt: string;
}

export interface NotificationPreferences {
  userId: string;
  emailNotifications: boolean;
}
