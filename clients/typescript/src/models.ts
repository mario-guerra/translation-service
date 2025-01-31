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
  body: HttpPart;
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

export interface HttpPart {}

/** Specifies various supported payment methods, such as PayPal, Stripe, or a credit card. */
export type PaymentMethod = "PayPal" | "Stripe" | "CreditCard";
/** Encapsulates data needed for audio file uploads, including file content, user identification, and translation languages. */
export type AudioUpload =
  | FormData
  | Array<
      | AudioUploadFilePartDescriptor
      | AudioUploadUserIdPartDescriptor
      | AudioUploadLangInPartDescriptor
      | AudioUploadLangOutPartDescriptor
    >;
