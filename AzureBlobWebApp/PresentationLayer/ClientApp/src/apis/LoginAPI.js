import { api } from "./configs/axiosConfig";
import { defineCancelApiObject } from "./configs/axiosUtils";


const LoginAPI = {
    authenticate: async function (creds, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "authenticate",
            method: "POST",
            headers: {"Authorization" : `Bearer ${token}`},
            data: creds,
            // retrieving the signal value by using the property name
            signal: cancel ? cancelApiObject[this.authenticate.name].handleRequestCancellation().signal : undefined,
        })
       return response.data 
    },
    refresh: async function (cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "refresh",
            method: "POST",
            headers: {"Authorization" : `Bearer ${token}`},
            signal: cancel ? cancelApiObject[this.refresh.name].handleRequestCancellation().signal : undefined,
        })
        return response.data
    },
    register: async function (userData, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "register",
            method: "POST",
            headers: {"Authorization" : `Bearer ${token}`},
            data: userData,
            signal: cancel ? cancelApiObject[this.register.name].handleRequestCancellation().signal : undefined,
        })
        return response.data
    }
}

const cancelApiObject = defineCancelApiObject(LoginAPI)

export default LoginAPI