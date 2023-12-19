import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import { Spinner } from 'react-bootstrap'
import Fab from '@mui/material/Fab';
import AddIcon from '@mui/icons-material/Add';
import AzureBlobAPI from "../apis/AzureBlobAPI"
import { toastOptions } from "../utils/Utils"
import FileItem from "../components/FileItem"
import VisuallyHiddenInput from "../components/VisuallyHiddenInput";

const FileBrowser = () => {

    const [files, setFiles] = useState([])
    const [loading, setLoading] = useState(false)

    useEffect(() => {
        fetchData()
    }, [])

    async function fetchData() {
        try {
            setLoading(true)
            let response = await AzureBlobAPI.getAllBlobs()
            setLoading(false)
            setFiles(response)
        } catch (error) {
            setLoading(false)
            toast.error(error.response.data || error.message, toastOptions)
        }
    }

    const onUpload = async (e) => {
        const fileToUpload = e.target.files[0]
        const formdata = new FormData();
        formdata.append("file", fileToUpload)
        try {
            setLoading(true)
            await AzureBlobAPI.upload(formdata)
            setLoading(false)
        } catch (error) {
            setLoading(false)
            toast.error(error.response.data || error.message, toastOptions)
        }
        await fetchData()
    }

    const onDelete = async (filename) => {
        try {
            setLoading(true)
            await AzureBlobAPI.delete(filename)
            setLoading(false)
        } catch (error) {
            setLoading(false)
            toast.error(error.response.data || error.message, toastOptions)
        }
        await fetchData()
    }

    return (
        <>
            <div className="d-flex flex-col w-100 vh-100">
                {(files === null || files.length === 0) ? (
                    <p className="d-flex flex-row justify-content-center vh-100 w-100 fs-1 align-items-center">No files found</p>
                ) : (
                    files.map((file) => 
                        <FileItem key={file} name={file.name} type={file.contentType} uri={file.uri} onDelete={onDelete} />
                    )                   
                )}
                <Fab color="primary" component="label" aria-label="add" sx={{
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
                        <>
                            <AddIcon />
                            <VisuallyHiddenInput type="file" onChange={onUpload} />
                        </>
                        
                    )}
                </Fab>
            </div>
            <ToastContainer />
        </>
    )
}

export default FileBrowser

// add to input to only accept types: accept=".jpg, .png"