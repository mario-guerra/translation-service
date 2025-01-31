// Licensed under the MIT License.

import {
  ProcessPaymentParameters,
  UploadAudioParameters,
  DownloadArtifactParameters,
} from "./parameters.js";
import {
  ProcessPayment200Response,
  ProcessPayment400Response,
  UploadAudio200Response,
  UploadAudio400Response,
  DownloadArtifact200Response,
} from "./responses.js";
import { Client, StreamableMethod } from "@typespec/ts-http-runtime";

export interface ProcessPayment {
  /** Submits a payment using the specified payment method and returns success or a payment failure error. */
  post(
    options: ProcessPaymentParameters,
  ): StreamableMethod<ProcessPayment200Response | ProcessPayment400Response>;
}

export interface UploadAudio {
  /** Uploads audio content using multipart/form-data, returning a success response or an invalid file error. */
  post(
    options: UploadAudioParameters,
  ): StreamableMethod<UploadAudio200Response | UploadAudio400Response>;
}

export interface DownloadArtifact {
  /** Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes. */
  get(
    options: DownloadArtifactParameters,
  ): StreamableMethod<DownloadArtifact200Response>;
}

export interface Routes {
  /** Resource for '/payment' has methods for the following verbs: post */
  (path: "/payment"): ProcessPayment;
  /** Resource for '/upload' has methods for the following verbs: post */
  (path: "/upload"): UploadAudio;
  /** Resource for '/download' has methods for the following verbs: get */
  (path: "/download"): DownloadArtifact;
}

export type AudioTranslationServiceClient = Client & {
  path: Routes;
};
