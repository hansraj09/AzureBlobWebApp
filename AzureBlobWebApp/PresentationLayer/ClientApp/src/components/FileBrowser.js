import { useState } from "react"

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