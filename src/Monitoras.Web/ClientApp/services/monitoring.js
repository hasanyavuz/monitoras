import { http } from "utils/http";

const MonitoringService = {
    async list(){
        var result = await http.get("/api/monitoring");
        if (result.status == 200) {
            return result.data;
          } else {
            console.error(result.error);
            throw result.error;
          }
    },
    async save(value){
        var result = await http.post("/api/monitoring", value);
        if (result.status == 200) {
            return result.data;
          } else {
            console.error(result.error);
            throw result.error;
          }
    }
}

export default MonitoringService;