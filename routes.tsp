import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;
using AudioTranslationService.Errors;

@doc("Specifies various supported payment methods, such as PayPal, Stripe, or a credit card.")
enum PaymentMethod {
  "PayPal",
  "Stripe",
  "CreditCard"
}

@doc("Defines the routes for processing payments and handling file operations.")
namespace AudioTranslationService.Routes {
  @doc("Submits a payment using the specified payment method and returns success or a payment failure error.")
  @route("/payment")
  @post
  op processPayment(
    @doc("The method used for paying, e.g., PayPal, Stripe, or CreditCard.")
    @query method: PaymentMethod,
    @doc("Optional callback URL for receiving payment status updates from the gateway.")
    @query callbackUrl?: string,
    @doc("Represents the base payment details gathered from the user.")
    @body payment: Payment
  ): PaymentResponse | PaymentFailureError;
  

  @doc("Uploads audio content using multipart/form-data, returning a success response or an invalid file error.")
  @route("/upload")
  @post
  op uploadAudio(
    @doc("Specifies the Content-Type header as multipart/form-data.")
    @header contentType: "multipart/form-data",
    @doc("Holds the audio data and metadata (e.g., input and output languages).")
    @body audioupload: AudioUpload
  ): SuccessResponse | InvalidFileError;

  @doc("Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes.")
  @route("/download")
  @get
  op downloadArtifact(
    @doc("Identifies the storage container for the artifact.")
    @query ContainerName: string,
    @doc("Specifies the upload identifier for the target artifact.")
    @query uploadId: string
  ): bytes;
}