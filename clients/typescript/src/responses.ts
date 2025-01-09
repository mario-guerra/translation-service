// Licensed under the MIT License.

import { HttpResponse } from "@typespec/ts-http-runtime";
import {
  UserOutput,
  PaymentOutput,
  AudioUploadOutput,
  TranslationJobOutput,
} from "./outputModels.js";

/** The request has succeeded. */
export interface Register200Response extends HttpResponse {
  status: "200";
  body: UserOutput;
}

/** The request has succeeded. */
export interface Login200Response extends HttpResponse {
  status: "200";
  body: string;
}

/** The request has succeeded. */
export interface ProcessPayment200Response extends HttpResponse {
  status: "200";
  body: PaymentOutput;
}

/** The request has succeeded. */
export interface UploadAudio200Response extends HttpResponse {
  status: "200";
  body: AudioUploadOutput;
}

/** The request has succeeded. */
export interface StartTranslation200Response extends HttpResponse {
  status: "200";
  body: TranslationJobOutput;
}

/** The request has succeeded. */
export interface CheckStatus200Response extends HttpResponse {
  status: "200";
  body: TranslationJobOutput;
}

/** The request has succeeded. */
export interface DownloadArtifact200Response extends HttpResponse {
  status: "200";
  body: string;
}

/** The request has succeeded. */
export interface GetUserProfile200Response extends HttpResponse {
  status: "200";
  body: UserOutput;
}

/** The request has succeeded. */
export interface UpdateUserProfile200Response extends HttpResponse {
  status: "200";
  body: string;
}

/** The request has succeeded. */
export interface GetUploadDetails200Response extends HttpResponse {
  status: "200";
  body: AudioUploadOutput;
}

/** The request has succeeded. */
export interface DeleteUpload200Response extends HttpResponse {
  status: "200";
  body: string;
}

/** The request has succeeded. */
export interface ListJobs200Response extends HttpResponse {
  status: "200";
  body: Array<TranslationJobOutput>;
}

/** The request has succeeded. */
export interface CancelJob200Response extends HttpResponse {
  status: "200";
  body: string;
}

/** The request has succeeded. */
export interface ManageNotifications200Response extends HttpResponse {
  status: "200";
  body: string;
}
