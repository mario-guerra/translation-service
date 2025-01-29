\generated\src\audioTranslationServiceClient.ts:
```
// Licensed under the MIT License.

import { getClient, ClientOptions } from "@typespec/ts-http-runtime";
import { AudioTranslationServiceClient } from "./clientDefinitions.js";

/** The optional parameters for the client */
export interface AudioTranslationServiceClientOptions extends ClientOptions {}

/**
 * Initialize a new instance of `AudioTranslationServiceClient`
 * @param endpointParam - The parameter endpointParam
 * @param options - the parameter for all optional parameters
 */
export default function createClient(
  endpointParam: string,
  options: AudioTranslationServiceClientOptions = {},
): AudioTranslationServiceClient {
  const endpointUrl = options.endpoint ?? `${endpointParam}`;
  const userAgentInfo = `azsdk-js-AudioTranslationService-rest/1.0.0-beta.1`;
  const userAgentPrefix =
    options.userAgentOptions && options.userAgentOptions.userAgentPrefix
      ? `${options.userAgentOptions.userAgentPrefix} ${userAgentInfo}`
      : `${userAgentInfo}`;
  options = {
    ...options,
    userAgentOptions: {
      userAgentPrefix,
    },
  };
  const client = getClient(
    endpointUrl,
    options,
  ) as AudioTranslationServiceClient;

  client.pipeline.removePolicy({ name: "ApiVersionPolicy" });

  return client;
}

```

\generated\src\clientDefinitions.ts:
```
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
  post(
    options: ProcessPaymentParameters,
  ): StreamableMethod<ProcessPayment200Response | ProcessPayment400Response>;
}

export interface UploadAudio {
  post(
    options: UploadAudioParameters,
  ): StreamableMethod<UploadAudio200Response | UploadAudio400Response>;
}

export interface DownloadArtifact {
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

```

\generated\src\index.ts:
```
// Licensed under the MIT License.

import AudioTranslationServiceClient from "./audioTranslationServiceClient.js";

export * from "./audioTranslationServiceClient.js";
export * from "./parameters.js";
export * from "./responses.js";
export * from "./clientDefinitions.js";
export * from "./models.js";
export * from "./outputModels.js";

export default AudioTranslationServiceClient;

```

\generated\src\models.ts:
```
// Licensed under the MIT License.

export interface Payment {
  userEmail: string;
  amount: number;
  service: string;
  userId: string;
  synthesizedAudio: boolean;
}

export interface AudioUploadFilePartDescriptor {
  name: "file";
  body:
    | string
    | Uint8Array
    | ReadableStream<Uint8Array>
    | NodeJS.ReadableStream
    | File;
  filename?: string;
  contentType?: string;
}

export interface AudioUploadUserIdPartDescriptor {
  name: "userId";
  body: string;
}

export interface AudioUploadLangInPartDescriptor {
  name: "LangIn";
  body: string;
}

export interface AudioUploadLangOutPartDescriptor {
  name: "LangOut";
  body: string;
}

export type AudioUpload =
  | FormData
  | Array<
      | AudioUploadFilePartDescriptor
      | AudioUploadUserIdPartDescriptor
      | AudioUploadLangInPartDescriptor
      | AudioUploadLangOutPartDescriptor
    >;

```

\generated\src\outputModels.ts:
```
// Licensed under the MIT License.

export interface PaymentResponseOutput {
  message: string;
  userId: string;
}

export interface PaymentFailureErrorOutput {
  message: "Payment processing failed.";
}

export interface SuccessResponseOutput {
  message: string;
}

export interface InvalidFileErrorOutput {
  message: "The uploaded file is invalid.";
}

```

\generated\src\parameters.ts:
```
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

```

\generated\src\responses.ts:
```
// Licensed under the MIT License.

import { HttpResponse } from "@typespec/ts-http-runtime";
import {
  PaymentResponseOutput,
  PaymentFailureErrorOutput,
  SuccessResponseOutput,
  InvalidFileErrorOutput,
} from "./outputModels.js";

/** The request has succeeded. */
export interface ProcessPayment200Response extends HttpResponse {
  status: "200";
  body: PaymentResponseOutput;
}

/** The server could not understand the request due to invalid syntax. */
export interface ProcessPayment400Response extends HttpResponse {
  status: "400";
  body: PaymentFailureErrorOutput;
}

/** The request has succeeded. */
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

```

