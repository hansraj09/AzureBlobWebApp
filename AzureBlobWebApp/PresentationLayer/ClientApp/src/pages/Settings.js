import React, { useEffect, useState } from 'react'
import Button from '@mui/material/Button';
import InputAdornment from '@mui/material/InputAdornment';
import FormHelperText from '@mui/material/FormHelperText';
import FormControl from '@mui/material/FormControl';
import OutlinedInput from '@mui/material/OutlinedInput';
import MultiChipSelectorBox from '../components/MultiChipSelectorBox';
import { toastOptions } from "../utils/Utils"
import { toast, ToastContainer } from 'react-toastify';
import { useNavigate } from 'react-router-dom';

const Settings = () => {

    const DEFAULT_MAX_SIZE = 20

    const [maxAllowedSizeInMB, setMaxAllowedSizeInMB] = useState(DEFAULT_MAX_SIZE)
    const [types, setTypes] = useState([]);

    const navigate = useNavigate()

    useEffect(() => {
        // get user configurations
        // const userConfig = await ...
        //setMaxAllowedSizeInMB(userConfig.maxSize)
        //setTypes(userConfig.allowedTypes)
    }, [])

    const typeList = [
        'text',
        'image',
        'video',
        'all'
    ]

    const onSizeChanged = (e) => {
        // test for numerical value only
        const input = e.target.value
        if (/^\d+$/.test(input) || (input === "")) {
            setMaxAllowedSizeInMB(input)
        }
    }

    const onSave = () => {
        if (maxAllowedSizeInMB === "") {
            toast.error("Max allowed size cannot be empty", toastOptions)
        } else {
            // save configuration
            navigate('/home')
        }
    }

    const onCancel = () => {
        navigate('/home')
    }

  return (
    <div className='d-flex flex-column align-items-center'>
        <h1 className='mt-2 fw-bold mb-5'>Settings</h1>
        <div className='d-flex flex-column w-100 px-4 fs-5 my-5'>
            <div className='d-flex flex-row justify-content-around'>
                <p className='d-flex fw-bold align-items-center'>Allowed types:</p>
                <MultiChipSelectorBox selected={types} setSelected={setTypes} listItems={typeList} />
            </div>
            <div className='d-flex flex-row justify-content-around'>
                <p className='d-flex fw-bold align-items-center'>Maximum file size:</p>
                <FormControl error={maxAllowedSizeInMB === ''} sx={{ m: 1, width: '25ch' }} variant="outlined">
                    <OutlinedInput
                        id="outlined-adornment-size"
                        endAdornment={<InputAdornment position="end">MB</InputAdornment>}
                        aria-describedby="outlined-size-helper-text"
                        inputProps={{
                        'aria-label': 'max-size',
                        }}
                        value={maxAllowedSizeInMB}
                        onChange={onSizeChanged}
                    />
                    <FormHelperText id="outlined-size-helper-text">Max size</FormHelperText>
                </FormControl>
            </div>
        </div>
        <div className='d-flex flex-row align-items-center justify-content-end w-100'>
            <Button variant="contained" onClick={onSave}>Save</Button>
            <Button variant="outlined" color='error' onClick={onCancel} sx={{mx: 5}}>Cancel</Button>
        </div>
        <ToastContainer />
    </div>
  )
}

export default Settings