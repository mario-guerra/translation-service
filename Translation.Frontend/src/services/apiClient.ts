// Translation.Frontend/src/services/apiClient.ts
//import createClient from "@translation-client/index";
import createClient from "../generated/dist/index";

const endpoint = "https://tsp-e2e-demo-gfa7gtancxg5fcak.eastus-01.azurewebsites.net";
//const endpoint = "http://localhost:5000";
const client = createClient(endpoint, {
    allowInsecureConnection: true,
});

export default client;