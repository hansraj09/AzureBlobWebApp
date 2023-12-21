import { api } from "./configs/axiosConfig";
import { defineCancelApiObject } from "./configs/axiosUtils";


const AzureBlobAPI = {
    upload: async function (formfile, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "blob/upload",
            method: "POST",
            headers: {"Authorization" : `Bearer ${token}`, 'Content-Type': 'multipart/form-data'},
            data: formfile,
            // retrieving the signal value by using the property name
            signal: cancel ? cancelApiObject[this.upload.name].handleRequestCancellation().signal : undefined,
        })
       return response.data 
    },
    download: async function (filename, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "blob/download",
            method: "POST",
            headers: {"Authorization" : `Bearer ${token}`, 'Content-Type': 'application/json'},
            responseType: 'blob',
            data: filename,
            signal: cancel ? cancelApiObject[this.download.name].handleRequestCancellation().signal : undefined,
        })
        return response.data
    },
    delete: async function (filename, cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "blob/delete",
            method: "DELETE",
            headers: {"Authorization" : `Bearer ${token}`, 'Content-Type': 'application/json'},
            data: filename,
            signal: cancel ? cancelApiObject[this.delete.name].handleRequestCancellation().signal : undefined,
        })
        return response.data
    },
    getAllBlobs: async function (cancel = false) {
        const token = sessionStorage.getItem('JWTtoken')
        const response = await api.request({
            url: "blob/getblobs",
            method: "GET",
            headers: {"Authorization" : `Bearer ${token}`},
            signal: cancel ? cancelApiObject[this.getAllBlobs.name].handleRequestCancellation().signal : undefined,
        })
        return response.data
    }
}

const cancelApiObject = defineCancelApiObject(AzureBlobAPI)

export default AzureBlobAPI