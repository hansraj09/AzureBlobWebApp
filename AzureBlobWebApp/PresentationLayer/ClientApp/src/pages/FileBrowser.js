import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import AzureBlobAPI from "../apis/AzureBlobAPI"
import { toastOptions } from "../utils/Utils"
import FileItem from "../components/FileItem"

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
            <div className="d-flex flex-col w-1 h-1">
                {(files === null || files.length === 0) ? (
                    <p className="flex justify-content-center align-items-center">No files found</p>
                ) : (
                    files.map((file) => 
                        <FileItem key={file} name={file.name} type={file.contentType} uri={file.uri} onDelete={onDelete} />
                    )
                )}
            </div>
            <ToastContainer />
        </>
    )

}

export default FileBrowser

const FileUpload = () => {
    const [file, setFile] = useState()
    //const [filename, setFilename] = useState()

    const saveFile = (e) => {
        setFile(e.target.files[0])
        //setFilename(e.target.files[0].name)
    }

    const uploadFile = async (e) => {
        //console.log(file)
        const formdata = new FormData();
        formdata.append("file", file)
        //formdata.append("fileName", filename)
        // axios call
    }

    return (
        <>
            <input type="file" onChange={saveFile} />
            <input type="button" value="upload" onClick={uploadFile} />
        </>
    )
}

// add to input to only accept types: accept=".jpg, .png"