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
