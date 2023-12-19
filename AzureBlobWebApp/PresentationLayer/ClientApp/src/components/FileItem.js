import React from 'react'
import ArrowCircleDownIcon from '@mui/icons-material/ArrowCircleDown';
import AudioFileIcon from '@mui/icons-material/AudioFile';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import ImageIcon from '@mui/icons-material/Image';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import ShareIcon from '@mui/icons-material/Share';
import TextSnippetIcon from '@mui/icons-material/TextSnippet';
import VideoCameraBackIcon from '@mui/icons-material/VideoCameraBack';
import IconButton from '@mui/material/IconButton';
import './FileItem.css'

function IconType(type) {
    if (type === 'img') {
        return <ImageIcon sx={{ width: 1, height: 1}} />
    } else if (type === 'video') {
        return <VideoCameraBackIcon sx={{ width: 1, height: 1}} />
    } else if (type === 'text') {
        return <TextSnippetIcon sx={{ width: 1, height: 1}} />
    } else if (type === 'audio') {
        return <AudioFileIcon sx={{ width: 1, height: 1}} />
    } else {
        return <InsertDriveFileIcon sx={{ width: 1, height: 1}} />
    }
}

const FileItem = (props) => {
  return (
    <div className='d-flex flex-column rounded-3 m-4 bg-main'>
        <div className='d-flex align-items-center justify-content-center'>
            {IconType(props.type)}
        </div>
        <div className='d-flex align-items-center justify-content-center'>
            <p className='fs-3'>{props.name}</p>
        </div>
        <div className='d-flex flex-row align-items-center justify-content-between m-3 icon'>
            <IconButton color="secondary" aria-label="delete file">
                <DeleteForeverIcon />
            </IconButton>
            <IconButton color="primary" aria-label="download file">
                <ArrowCircleDownIcon />
            </IconButton>
            <IconButton color="primary" aria-label="share file">
                <ShareIcon />
            </IconButton>
        </div>
    </div>
  )
}

export default FileItem