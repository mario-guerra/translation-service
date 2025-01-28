// Licensed under the MIT License.

import { RequestParameters } from "@typespec/ts-http-runtime";
import { Payment, AudioUpload } from "./models.js";

export interface ProcessPaymentBodyParam {
  body: Payment;
}

export type ProcessPaymentParameters = ProcessPaymentBodyParam &
  RequestParameters;

export interface UploadAudioBodyParam {
  body: AudioUpload;
}

export interface UploadAudioMediaTypesParam {
  contentType: "multipart/form-data";
}

export type UploadAudioParameters = UploadAudioMediaTypesParam &
  UploadAudioBodyParam &
  RequestParameters;

export interface DownloadArtifactQueryParamProperties {
  ContainerName: string;
  uploadId: string;
}

export interface DownloadArtifactQueryParam {
  queryParameters: DownloadArtifactQueryParamProperties;
}

export type DownloadArtifactParameters = DownloadArtifactQueryParam &
  RequestParameters;
