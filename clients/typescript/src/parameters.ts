// Licensed under the MIT License.

import { RequestParameters } from "@typespec/ts-http-runtime";
import {
  User,
  Payment,
  AudioUpload,
  TranslationJob,
  NotificationPreferences,
} from "./models.js";

export interface RegisterBodyParam {
  body: User;
}

export type RegisterParameters = RegisterBodyParam & RequestParameters;

export interface LoginBodyParam {
  body: User;
}

export type LoginParameters = LoginBodyParam & RequestParameters;

export interface ProcessPaymentBodyParam {
  body: Payment;
}

export type ProcessPaymentParameters = ProcessPaymentBodyParam &
  RequestParameters;

export interface UploadAudioBodyParam {
  body: AudioUpload;
}

export type UploadAudioParameters = UploadAudioBodyParam & RequestParameters;

export interface StartTranslationBodyParam {
  body: TranslationJob;
}

export type StartTranslationParameters = StartTranslationBodyParam &
  RequestParameters;
export type CheckStatusParameters = RequestParameters;
export type DownloadArtifactParameters = RequestParameters;
export type GetUserProfileParameters = RequestParameters;

export interface UpdateUserProfileBodyParam {
  body: User;
}

export type UpdateUserProfileParameters = UpdateUserProfileBodyParam &
  RequestParameters;
export type GetUploadDetailsParameters = RequestParameters;
export type DeleteUploadParameters = RequestParameters;
export type ListJobsParameters = RequestParameters;
export type CancelJobParameters = RequestParameters;

export interface ManageNotificationsBodyParam {
  body: NotificationPreferences;
}

export type ManageNotificationsParameters = ManageNotificationsBodyParam &
  RequestParameters;
