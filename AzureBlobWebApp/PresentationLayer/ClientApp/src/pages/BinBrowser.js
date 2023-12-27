import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import { Spinner } from 'react-bootstrap'
import Fab from '@mui/material/Fab';
import DoneIcon from '@mui/icons-material/Done';
import AzureBlobAPI from "../apis/AzureBlobAPI"
import { toastOptions } from "../utils/Utils"
import BinItem from "../components/BinItem/BinItem";
import { useNavigate } from "react-router-dom";

const BinBrowser = () => {

    const [files, setFiles] = useState([])
    const [loading, setLoading] = useState(false)

    const navigate = useNavigate()

    useEffect(() => {
        fetchData()
    }, [])

    async function fetchData() {
        try {
            setLoading(true)
            let response = await AzureBlobAPI.getAllBlobs()
            setLoading(false)
            const validFiles = response.filter((f) => f.isDeleted)
            setFiles(validFiles)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
    }

    const onPermanentDelete = async (guid) => {
        try {
            setLoading(true)
            await AzureBlobAPI.permanentDelete(guid)
            setLoading(false)
            toast.info("File successfully deleted", toastOptions)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
        await fetchData()
    }

    const onRestore = async (guid) => {
        try {
            setLoading(true)
            await AzureBlobAPI.restore(guid)
            setLoading(false)
            toast.info("File successfully restored", toastOptions)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
        await fetchData()
    }

    return (
        <>
            <div className="d-flex flex-col w-100 vh-100 flex-wrap">
                {(files === null || files.length === 0) ? (
                    <p className="d-flex flex-row justify-content-center vh-100 w-100 fs-1 align-items-center">No files found</p>
                ) : (
                    files.map((file, index) => 
                        <BinItem key={index} fileItem={file} onDelete={onPermanentDelete} onRestore={onRestore} />
                    )                
                )}
                <Fab color="primary" component="label" aria-label="add" disabled={loading} onClick={() => navigate("/home")} sx={{
                    position: "fixed",
                    bottom: (theme) => theme.spacing(2),
                    right: (theme) => theme.spacing(2)
                }}>
                    {loading ? (
                        <Spinner
                            as="span"
                            variant="warning"
                            size="sm"
                            role="status"
                            aria-hidden="true"
                            animation="border"/>
                    ) : (
                        <DoneIcon />                      
                    )}
                </Fab>
            </div>
            <ToastContainer />
        </>
    )
}

export default BinBrowser