// Licensed under the MIT License.

import {
  RegisterParameters,
  LoginParameters,
  ProcessPaymentParameters,
  UploadAudioParameters,
  StartTranslationParameters,
  CheckStatusParameters,
  DownloadArtifactParameters,
  GetUserProfileParameters,
  UpdateUserProfileParameters,
  GetUploadDetailsParameters,
  DeleteUploadParameters,
  ListJobsParameters,
  CancelJobParameters,
  ManageNotificationsParameters,
} from "./parameters.js";
import {
  Register200Response,
  Login200Response,
  ProcessPayment200Response,
  UploadAudio200Response,
  StartTranslation200Response,
  CheckStatus200Response,
  DownloadArtifact200Response,
  GetUserProfile200Response,
  UpdateUserProfile200Response,
  GetUploadDetails200Response,
  DeleteUpload200Response,
  ListJobs200Response,
  CancelJob200Response,
  ManageNotifications200Response,
} from "./responses.js";
import { Client, StreamableMethod } from "@typespec/ts-http-runtime";

export interface Register {
  post(options: RegisterParameters): StreamableMethod<Register200Response>;
}

export interface Login {
  post(options: LoginParameters): StreamableMethod<Login200Response>;
}

export interface ProcessPayment {
  post(
    options: ProcessPaymentParameters,
  ): StreamableMethod<ProcessPayment200Response>;
}

export interface UploadAudio {
  post(
    options: UploadAudioParameters,
  ): StreamableMethod<UploadAudio200Response>;
}

export interface StartTranslation {
  post(
    options: StartTranslationParameters,
  ): StreamableMethod<StartTranslation200Response>;
}

export interface CheckStatus {
  get(
    options?: CheckStatusParameters,
  ): StreamableMethod<CheckStatus200Response>;
}

export interface DownloadArtifact {
  get(
    options?: DownloadArtifactParameters,
  ): StreamableMethod<DownloadArtifact200Response>;
}

export interface GetUserProfile {
  get(
    options?: GetUserProfileParameters,
  ): StreamableMethod<GetUserProfile200Response>;
  put(
    options: UpdateUserProfileParameters,
  ): StreamableMethod<UpdateUserProfile200Response>;
}

export interface GetUploadDetails {
  get(
    options?: GetUploadDetailsParameters,
  ): StreamableMethod<GetUploadDetails200Response>;
  delete(
    options?: DeleteUploadParameters,
  ): StreamableMethod<DeleteUpload200Response>;
}

export interface ListJobs {
  get(options?: ListJobsParameters): StreamableMethod<ListJobs200Response>;
}

export interface CancelJob {
  delete(options?: CancelJobParameters): StreamableMethod<CancelJob200Response>;
}

export interface ManageNotifications {
  post(
    options: ManageNotificationsParameters,
  ): StreamableMethod<ManageNotifications200Response>;
}

export interface Routes {
  /** Resource for '/register' has methods for the following verbs: post */
  (path: "/register"): Register;
  /** Resource for '/login' has methods for the following verbs: post */
  (path: "/login"): Login;
  /** Resource for '/payment' has methods for the following verbs: post */
  (path: "/payment"): ProcessPayment;
  /** Resource for '/upload-audio' has methods for the following verbs: post */
  (path: "/upload-audio"): UploadAudio;
  /** Resource for '/translate' has methods for the following verbs: post */
  (path: "/translate"): StartTranslation;
  /** Resource for '/status/\{jobId\}' has methods for the following verbs: get */
  (path: "/status/{jobId}", jobId: string): CheckStatus;
  /** Resource for '/download/\{jobId\}' has methods for the following verbs: get */
  (path: "/download/{jobId}", jobId: string): DownloadArtifact;
  /** Resource for '/user/profile' has methods for the following verbs: get, put */
  (path: "/user/profile"): GetUserProfile;
  /** Resource for '/upload/\{uploadId\}' has methods for the following verbs: get, delete */
  (path: "/upload/{uploadId}", uploadId: string): GetUploadDetails;
  /** Resource for '/jobs' has methods for the following verbs: get */
  (path: "/jobs"): ListJobs;
  /** Resource for '/job/\{jobId\}' has methods for the following verbs: delete */
  (path: "/job/{jobId}", jobId: string): CancelJob;
  /** Resource for '/notifications' has methods for the following verbs: post */
  (path: "/notifications"): ManageNotifications;
}

export type AudioTranslationServiceClient = Client & {
  path: Routes;
};
