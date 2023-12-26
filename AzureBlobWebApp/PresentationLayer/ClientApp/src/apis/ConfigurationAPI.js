import { api } from "./configs/axiosConfig";
import { defineCancelApiObject } from "./configs/axiosUtils";


const ConfigurationAPI = {
    getConfigs: async function (cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "configuration/getconfigs",
            method: "GET",
            headers: {"Authorization" : `Bearer ${token}`},
            signal: cancel ? cancelApiObject[this.getConfigs.name].handleRequestCancellation().signal : undefined,
        })
       return response.data 
    },
    setConfigs: async function (maxSize, allowedTypes, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "configuration/setconfigs",
            method: "PUT",
            headers: {"Authorization" : `Bearer ${token}`, 'Content-Type': 'application/json'},
            data: {maxSize: maxSize, allowedTypes: allowedTypes},
            signal: cancel ? cancelApiObject[this.getConfigs.name].handleRequestCancellation().signal : undefined,
        })
       return response.data
    }
}

const cancelApiObject = defineCancelApiObject(ConfigurationAPI)

export default ConfigurationAPI