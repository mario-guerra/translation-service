// Licensed under the MIT License.

export interface UserOutput {
  userId: string;
  name: string;
  email: string;
  password: string;
}

export interface PaymentOutput {
  paymentId: string;
  userId: string;
  amount: number;
  languageOptions: string[];
  service: string;
}

export interface AudioUploadOutput {
  uploadId: string;
  fileName: string;
  fileSize: number;
  uploadDate: string;
}

export interface TranslationJobOutput {
  jobId: string;
  uploadId: string;
  status: string;
  progress: number;
  createdAt: string;
}
