// Licensed under the MIT License.

import { RequestParameters } from "@typespec/ts-http-runtime";
import { PaymentMethod, Payment, AudioUpload } from "./models.js";

export interface ProcessPaymentBodyParam {
  /** Represents the base payment details gathered from the user. */
  body: Payment;
}

export interface ProcessPaymentQueryParamProperties {
  /** The method used for paying, e.g., PayPal, Stripe, or CreditCard. */
  method: PaymentMethod;
  /** Optional callback URL for receiving payment status updates from the gateway. */
  callbackUrl?: string;
}

export interface ProcessPaymentQueryParam {
  queryParameters: ProcessPaymentQueryParamProperties;
}

export type ProcessPaymentParameters = ProcessPaymentQueryParam &
  ProcessPaymentBodyParam &
  RequestParameters;

export interface UploadAudioBodyParam {
  /** Holds the audio data and metadata (e.g., input and output languages). */
  body: AudioUpload;
}

export interface UploadAudioMediaTypesParam {
  /** Specifies the Content-Type header as multipart/form-data. */
  contentType: "multipart/form-data";
}

export type UploadAudioParameters = UploadAudioMediaTypesParam &
  UploadAudioBodyParam &
  RequestParameters;

export interface DownloadArtifactQueryParamProperties {
  /** Identifies the storage container for the artifact. */
  ContainerName: string;
  /** Specifies the upload identifier for the target artifact. */
  uploadId: string;
}

export interface DownloadArtifactQueryParam {
  queryParameters: DownloadArtifactQueryParamProperties;
}

export type DownloadArtifactParameters = DownloadArtifactQueryParam &
  RequestParameters;
