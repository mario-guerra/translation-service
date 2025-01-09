import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;

namespace AudioTranslationService.Operations {
  @route("/user/profile")
  @get
  op getUserProfile(): Models.User;

  @route("/user/profile")
  @put
  op updateUserProfile(@body Models:User): string;

  @route("/upload/{uploadId}")
  @get
  op getUploadDetails(@path uploadId: string): Models.AudioUpload;

  @route("/upload/{uploadId}")
  @delete
  op deleteUpload(@path uploadId: string): string;

  @route("/jobs")
  @get
  op listJobs(): Models.TranslationJob[];

  @route("/job/{jobId}")
  @delete
  op cancelJob(@path jobId: string): string;
}