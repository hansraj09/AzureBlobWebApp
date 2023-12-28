import React, { useEffect, useState } from 'react'
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Spinner } from 'react-bootstrap'
import { toast, ToastContainer } from 'react-toastify';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { DEFAULT_MAX_SIZE, typeList } from '../../pages/Settings';
import ConfigurationAPI from '../../apis/ConfigurationAPI';
import { toastOptions } from '../../utils/Utils';
import './UploadModal.css'

const UploadModal = (props) => {

    const [file, setFile] = useState(null);   
    const [dragActive, setDragActive] = useState(false);
    const [msg, setMsg] = useState("");
    const [maxAllowedSizeInMB, setMaxAllowedSizeInMB] = useState(DEFAULT_MAX_SIZE)
    const [types, setTypes] = useState([]);
    const [loading, setLoading] = useState(false)

    const fetchConfig = async () => {
      try {
          setLoading(true)
          let response = await ConfigurationAPI.getConfigs()
          setLoading(false)
          if (response.length >= 2) {
              setMaxAllowedSizeInMB(response[0].configValue)
              const deserializedTypes = JSON.parse(response[1].configValue)
              setTypes(deserializedTypes)
          }
      } catch (error) {
          setLoading(false)
          toast.error(error?.response?.data || error.message, toastOptions)
      }
  }

    useEffect(() => {
      fetchConfig()
    }, [])

    const checkType = (type) => {
      if (types.includes('all')) {
        return true
      }
      let result = false
      typeList.map((allowedType) => {
        if (types.includes(allowedType) && type.includes(allowedType)) {
          result = true
        }
      })
      return result
    }

    const checkFileType = (e, eventType) => {
      let type;
  
      if (eventType === "drop") {
        type = e.dataTransfer.files[0].type
      } else {
        type = e.target.files[0].type
      }

      if (checkType(type)) {
        eventType !== "drop"
          ? setFile(e.target.files[0])
          : setFile(e.dataTransfer.files[0]);
        setMsg("");
      } else {
        setFile( null );
        setMsg(`${type} is not allowed.`);
      }
    }
  
    const checkSize = (e, eventType) => {
      let size;
      if (eventType === "drop") {
        size = e.dataTransfer.files[0].size;
      } else {
        size = e.target.files[0].size;
      }
  
      if (size <= (maxAllowedSizeInMB * 1024 * 1024)) {
        checkFileType(e, eventType);
      } else {
        setMsg(`Size should be less than ${maxAllowedSizeInMB}MB`);
      }
    };
  
    const chooseFile = (e) => {
      e.preventDefault();
      e.stopPropagation();
      if (e.target.files && e.target.files[0]) {
        checkSize(e);
      }
    };
  
    const handleDrag = (e) => {
      e.preventDefault();
      e.stopPropagation();
      if (e.type === "dragenter" || e.type === "dragover") {
        setDragActive(true);
      } else if (e.type === "dragleave") {
        setDragActive(false);
      }
    };
  
    const handleDrop = (e) => {
      e.preventDefault();
      e.stopPropagation();
      setDragActive(false);
      if (e.dataTransfer.files && e.dataTransfer.files[0]) {
        checkSize(e, "drop");
      }
    };

  return (
    <Modal isOpen={props.open} toggle={props.toggleModal} backdrop='static'>
        <ModalHeader toggle={() => {
          setFile(null)
          setMsg("")
          props.toggleModal()
        }}>
          Upload File
        </ModalHeader>
        <ModalBody>
            <div className="FirstTab">
                <form
                    className="uploadBox"
                    onDragEnter={handleDrag}
                    onDragLeave={handleDrag}
                    onDragOver={handleDrag}
                    onDrop={handleDrop}
                    onSubmit={(e) => e.preventDefault()}
                >
                    {file !== null ? (
                      <p className="filename">{file.name}</p>
                    ) : msg !== "" ? (
                      msg
                    ) : (
                      <CloudUploadIcon />
                    )}

                      <div className="drag">
                          Drop your file here or
                          <input className='border border-primary' type='file' onChange={chooseFile} />
                      </div>


                    <p className="info">{`Max Size: ${maxAllowedSizeInMB}, Supported types: ${types.toString()}`}</p>
                </form>
            </div>
        </ModalBody>
        <ModalFooter>
            <Button color="primary" disabled={loading || file === null} onClick={() => {
                props.toggleModal()
                props.onUpload(file)
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
                      <span>Upload</span>
                    )}
            </Button>{' '}
            <Button color="secondary" onClick={() => {
              setFile(null)
              setMsg("")
              props.toggleModal()
            }} >
                Cancel
            </Button>
        </ModalFooter>
        <ToastContainer />
    </Modal>
  )
}

export default UploadModal