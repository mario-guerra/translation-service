import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;

namespace AudioTranslationService.Routes {
  @route("/register")
  @post
  op register(@body user: User): User;

  @route("/login")
  @post
  op login(@body user: User): string;

  @route("/payment")
  @post
  op processPayment(@body payment: Payment): Payment;

  @route("/upload-audio")
  @post
  op uploadAudio(@body audioupload: AudioUpload): AudioUpload;

  @route("/translate")
  @post
  op startTranslation(@body translationjob: TranslationJob): TranslationJob;

  @route("/status/{jobId}")
  @get
  op checkStatus(@path jobId: string): TranslationJob;

  @route("/download/{jobId}")
  @get
  op downloadArtifact(@path jobId: string): bytes;
}