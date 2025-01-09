namespace AudioTranslationService.Errors {
    model ErrorResponse {
      errorCode: string;
      errorMessage: string;
    }
  
    @error
    model InvalidFileError extends ErrorResponse {
      errorCode: "INVALID_FILE";
      errorMessage: "The uploaded file is invalid.";
    }
  
    @error
    model PaymentFailureError extends ErrorResponse {
      errorCode: "PAYMENT_FAILURE";
      errorMessage: "Payment processing failed.";
    }
  }