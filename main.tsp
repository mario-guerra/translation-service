import "./models.tsp";
import "./routes.tsp";
import "./operations.tsp";
import "./errors.tsp";
import "./security.tsp";
import "./notifications.tsp";

import "@typespec/http";

using TypeSpec.Http;

@service({
  title: "Audio Translation Service",
})
namespace AudioTranslationService;