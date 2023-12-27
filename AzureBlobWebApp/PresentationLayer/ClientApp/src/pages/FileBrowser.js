import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import { Spinner } from 'react-bootstrap'
import Fab from '@mui/material/Fab';
import AddIcon from '@mui/icons-material/Add';
import AzureBlobAPI from "../apis/AzureBlobAPI"
import { toastOptions } from "../utils/Utils"
import FileItem from "../components/FileItem/FileItem"
import VisuallyHiddenInput from "../components/VisuallyHiddenInput/VisuallyHiddenInput";
import RecycleBin from "../components/RecycleBin/RecycleBin";
import UploadModal from "../components/UploadModal/UploadModal";

const FileBrowser = () => {

    const [files, setFiles] = useState([])
    const [numDeleted, setNumDeleted] = useState(0)
    const [loading, setLoading] = useState(false)
    const [openModal, setOpenModal] = useState(false)

    const toggleModal = () => setOpenModal(!openModal)

    useEffect(() => {
        fetchData()
    }, [])

    async function fetchData() {
        try {
            setLoading(true)
            let response = await AzureBlobAPI.getAllBlobs()
            setLoading(false)
            const validFiles = response.filter((f) => !f.isDeleted)
            const numDeleted = response.length - validFiles.length
            setFiles(validFiles)
            setNumDeleted(numDeleted)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
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
            toast.info("File successfully uploaded", toastOptions)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response.data || error?.message, toastOptions)
        }
        await fetchData()
    }

    const onDelete = async (guid) => {
        try {
            setLoading(true)
            await AzureBlobAPI.delete(guid)
            setLoading(false)
            toast.info("File moved to Recently Deleted", toastOptions)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
        await fetchData()
    }

    const onDownload = async (guid, filename) => {
        try {
            setLoading(true)
            let file = await AzureBlobAPI.download(guid)
            setLoading(false)
            const element = document.createElement("a");
            element.href = URL.createObjectURL(file);
            element.download = filename;// simulate link click
            document.body.appendChild(element); // Required for this to work in FireFox
            element.click();
            // clean up "a" element & remove ObjectURL
            document.body.removeChild(element);
            URL.revokeObjectURL(file);
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
        
    }

    return (
        <>
            <div className="d-flex flex-col w-100 vh-100 flex-wrap">
                {(files === null || files.length === 0) ? (
                    <p className="d-flex flex-row justify-content-center vh-100 w-100 fs-1 align-items-center">No files found</p>
                ) : (
                    <>
                        {files.map((file, index) => 
                            <FileItem key={index} fileItem={file} onDelete={onDelete} onDownload={onDownload} />
                        )}
                        <RecycleBin number={numDeleted} />                   
                    </>                   
                )}
                <Fab color="primary" component="label" onClick={toggleModal} aria-label="add" sx={{
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
                            <UploadModal open={openModal} toggleModal={toggleModal} onUpload={onUpload} />
                            {/* <VisuallyHiddenInput type="file" onChange={onUpload} /> */}
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