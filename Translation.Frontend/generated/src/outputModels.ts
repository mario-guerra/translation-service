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
