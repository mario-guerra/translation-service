import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;
using AudioTranslationService.Errors;

namespace AudioTranslationService.Routes {
  @route("/payment")
  @post
  op processPayment(@body payment: Payment): PaymentResponse | PaymentFailureError;

  @route("/upload")
  @post
  op uploadAudio(
    @header contentType: "multipart/form-data",
    @body audioupload: AudioUpload
  ): SuccessResponse | InvalidFileError;

  @route("/download")
  @get
  op downloadArtifact(
    @query ContainerName: string,
    @query uploadId: string
  ): bytes;
}