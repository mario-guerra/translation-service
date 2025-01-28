import "./models.tsp";
import "./routes.tsp";
import "./errors.tsp";
import "./security.tsp";

import "@typespec/http";

using TypeSpec.Http;

@service({
  title: "Audio Translation Service",
})
namespace AudioTranslationService;