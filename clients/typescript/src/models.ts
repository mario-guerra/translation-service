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
