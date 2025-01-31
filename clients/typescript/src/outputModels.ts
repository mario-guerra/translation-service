// Licensed under the MIT License.

/** Details the outcome of a payment process, including user's ID and a descriptive message, with an HTTP 200 status. */
export interface PaymentResponseOutput {
  /** A descriptive message indicating the result of the payment. */
  message: string;
  /** Corresponds to the user who initiated the payment, uniquely identifying them. */
  userId: string;
}

export interface PaymentFailureErrorOutput {
  message: "Payment processing failed.";
}

/** Represents a successful result with a predefined HTTP 200 status code, used when the request and processing complete without errors. */
export interface SuccessResponseOutput {
  /** A brief message providing additional information about the successful operation. */
  message: string;
}

export interface InvalidFileErrorOutput {
  message: "The uploaded file is invalid.";
}
