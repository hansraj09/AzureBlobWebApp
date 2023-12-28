import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import { Spinner } from 'react-bootstrap'
import Fab from '@mui/material/Fab';
import TextField from "@mui/material/TextField";
import AddIcon from '@mui/icons-material/Add';
import AzureBlobAPI from "../apis/AzureBlobAPI"
import { toastOptions } from "../utils/Utils"
import FileItem from "../components/FileItem/FileItem"
import RecycleBin from "../components/RecycleBin/RecycleBin";
import UploadModal from "../components/UploadModal/UploadModal";

const FileBrowser = () => {

    const [files, setFiles] = useState([])
    const [filteredFiles, setFilteredFiles] = useState([])
    const [numDeleted, setNumDeleted] = useState(0)
    const [loading, setLoading] = useState(false)
    const [openModal, setOpenModal] = useState(false)
    const [search, setSearch] = useState("")

    const toggleModal = () => setOpenModal(!openModal)

    useEffect(() => {
        fetchData()
    }, [])

    useEffect(() => {
        const searchedFiles = files.filter((f) => {
            if (search === "") return true
            return f.fileName.toLowerCase().includes(search)
        })
        setFilteredFiles(searchedFiles)
    }, [search])

    async function fetchData() {
        try {
            setLoading(true)
            let response = await AzureBlobAPI.getAllBlobs()
            setLoading(false)
            const validFiles = response.filter((f) => !f.isDeleted)
            const numDeleted = response.length - validFiles.length
            setFiles(validFiles)
            setFilteredFiles(validFiles)
            setNumDeleted(numDeleted)
        } catch (error) {
            setLoading(false)
            toast.error(error?.response?.data || error.message, toastOptions)
        }
    }

    const onSearch = (e) => {
        //convert input text to lower case
        var lowerCase = e.target.value.toLowerCase();
        setSearch(lowerCase);
    };

    const onUpload = async (file, description) => {
        const formdata = new FormData();
        if (description === "") {
            description = "No description"
        }
        formdata.append("file", file)
        formdata.append("description", description)
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
                <div className="d-flex flex-row justify-content-center w-100 mx-5">
                    <TextField
                        id="outlined-basic"
                        variant="outlined"
                        fullWidth
                        label="Search"
                        onChange={onSearch}
                        sx={{width: '50%', height: 'auto'}}
                    />
                </div>
                {(files === null || files.length === 0) ? (
                    <p className="d-flex flex-row justify-content-center vh-100 w-100 fs-1 align-items-center">No files found</p>
                ) : (                       
                        <>
                            {filteredFiles.map((file, index) => 
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
                            <UploadModal 
                                open={openModal} 
                                toggleModal={toggleModal} 
                                onUpload={onUpload} />
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