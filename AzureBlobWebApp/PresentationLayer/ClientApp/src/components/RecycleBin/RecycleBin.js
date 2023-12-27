import React from 'react'
import { useNavigate } from 'react-router-dom';
import AutoDeleteIcon from '@mui/icons-material/AutoDelete';
import './RecycleBin.css'


const RecycleBin = (props) => {

    const navigate = useNavigate()

    const handleClick = (e) => {
        // on double click only
        if (e.detail === 2) {
            navigate('/bin')
        }
    }

  return (
    <div tabIndex={"1"} className='d-flex flex-column rounded-3 m-4 bg-main' style={{ height:"fit-content" }} onClick={handleClick}>
        <div className='d-flex align-items-center justify-content-center'>
            <AutoDeleteIcon color='error' sx={{ width: 1, height: 1}} />
        </div>
        <div className='d-flex flex-column align-items-center justify-content-center'>
            <p className='fs-3'>Recently Deleted</p>
            {props.number === 1 ? (
                <p>1 file</p>
            ) : (
                <p>{props.number} files</p>
            )}
        </div>
    </div>
  )
}

export default RecycleBin