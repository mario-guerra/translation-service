import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;

namespace AudioTranslationService.Security {
    model RateLimiting {
      limit: int32;
      window: string;
    }
  
    model InputValidation {
      rules: string[];
    }
  
    model SecureStorage {
      encryption: string;
    }
  }