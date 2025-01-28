import "@typespec/http";
using TypeSpec.Http;

namespace AudioTranslationService.Errors {
    @error
    model InvalidFileError {
      @statusCode statusCode:400;
      message: "The uploaded file is invalid.";
    }
  
    @error
    model PaymentFailureError {
      @statusCode statusCode:400;
      message: "Payment processing failed.";
    }
  }