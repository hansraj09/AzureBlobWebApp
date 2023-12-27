import React, { useState } from 'react'
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import AudioFileIcon from '@mui/icons-material/AudioFile';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import ImageIcon from '@mui/icons-material/Image';
import InfoIcon from '@mui/icons-material/Info';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import RestoreIcon from '@mui/icons-material/Restore';
import TextSnippetIcon from '@mui/icons-material/TextSnippet';
import VideoCameraBackIcon from '@mui/icons-material/VideoCameraBack';
import IconButton from '@mui/material/IconButton';
import './BinItem.css'


const BinItem = (props) => {

    const [open, setOpen] = useState(false)

    const toggleModal = () => setOpen(!open)

    function IconType(type) {
        if (type.includes('img')) {
            return <ImageIcon sx={{ width: 1, height: 1}} />
        } else if (type.includes('video')) {
            return <VideoCameraBackIcon sx={{ width: 1, height: 1}} />
        } else if (type.includes('text')) {
            return <TextSnippetIcon sx={{ width: 1, height: 1}} />
        } else if (type.includes('audio')) {
            return <AudioFileIcon sx={{ width: 1, height: 1}} />
        } else {
            return <InsertDriveFileIcon sx={{ width: 1, height: 1}} />
        }
    }


  return (
    <div className='d-flex flex-column rounded-3 m-4 bg-main' style={{ height:"fit-content" }}>
        <div className='d-flex align-items-center justify-content-center'>
            {IconType(props.fileItem.type)}
        </div>
        <div className='d-flex align-items-center justify-content-center'>
            <p className='fs-3'>{props.fileItem.fileName}</p>
        </div>
        <div className='d-flex flex-row align-items-center justify-content-between m-3 icon'>
            <IconButton color="secondary" aria-label="delete file" onClick={toggleModal}>
                <DeleteForeverIcon />
            </IconButton>
            <IconButton color="primary" aria-label="details" onClick={() => {}}>
                <InfoIcon />
            </IconButton>
            <IconButton color="primary" aria-label="restore file" onClick={() => props.onRestore(props.fileItem.guid)}>
                <RestoreIcon />
            </IconButton>
        </div>
        <Modal isOpen={open} toggle={toggleModal}>
            <ModalHeader toggle={toggleModal}>Confirm Permanent Delete</ModalHeader>
            <ModalBody>
            Are you sure you want to permanently delete the file {props.fileItem.fileName}?
            </ModalBody>
            <ModalFooter>
            <Button color="danger" onClick={() => {
                toggleModal()
                props.onDelete(props.fileItem.guid)
            }}>
                Delete
            </Button>{' '}
            <Button color="secondary" onClick={toggleModal}>
                Cancel
            </Button>
            </ModalFooter>
        </Modal>
    </div>
  )
}

export default BinItem