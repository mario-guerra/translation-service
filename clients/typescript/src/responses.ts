// Licensed under the MIT License.

import { HttpResponse } from "@typespec/ts-http-runtime";
import {
  PaymentResponseOutput,
  PaymentFailureErrorOutput,
  SuccessResponseOutput,
  InvalidFileErrorOutput,
} from "./outputModels.js";

/** Details the outcome of a payment process, including user's ID and a descriptive message, with an HTTP 200 status. */
export interface ProcessPayment200Response extends HttpResponse {
  status: "200";
  body: PaymentResponseOutput;
}

/** The server could not understand the request due to invalid syntax. */
export interface ProcessPayment400Response extends HttpResponse {
  status: "400";
  body: PaymentFailureErrorOutput;
}

/** Represents a successful result with a predefined HTTP 200 status code, used when the request and processing complete without errors. */
export interface UploadAudio200Response extends HttpResponse {
  status: "200";
  body: SuccessResponseOutput;
}

/** The server could not understand the request due to invalid syntax. */
export interface UploadAudio400Response extends HttpResponse {
  status: "400";
  body: InvalidFileErrorOutput;
}

/** The request has succeeded. */
export interface DownloadArtifact200Response extends HttpResponse {
  status: "200";
  body: string;
}
