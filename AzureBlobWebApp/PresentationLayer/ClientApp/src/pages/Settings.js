import React, { useEffect, useState } from 'react'
import Button from '@mui/material/Button';
import InputAdornment from '@mui/material/InputAdornment';
import FormHelperText from '@mui/material/FormHelperText';
import FormControl from '@mui/material/FormControl';
import OutlinedInput from '@mui/material/OutlinedInput';
import MultiChipSelectorBox from '../components/MultiChipSelectorBox/MultiChipSelectorBox';
import { toastOptions } from "../utils/Utils"
import { Spinner } from 'react-bootstrap'
import { toast, ToastContainer } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import ConfigurationAPI from '../apis/ConfigurationAPI';

const Settings = () => {

    const DEFAULT_MAX_SIZE = 20

    const [maxAllowedSizeInMB, setMaxAllowedSizeInMB] = useState(DEFAULT_MAX_SIZE)
    const [types, setTypes] = useState([]);
    const [loading, setLoading] = useState(false)

    const navigate = useNavigate()

    useEffect(() => {
        fetchConfig()
    }, [])

    async function fetchConfig() {
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

    const onSave = async () => {
        if (maxAllowedSizeInMB === "") {
            toast.error("Max allowed size cannot be empty", toastOptions)
        } else if (types.length === 0 || types === null) {
            toast.error("Please select at least one option for allowed types", toastOptions)
        } else {
            try {
                const serializedAllowedTypes = JSON.stringify(types)
                await ConfigurationAPI.setConfigs(maxAllowedSizeInMB, serializedAllowedTypes)
                navigate('/home')
            } catch(error) {
                toast.error(error?.response?.data || error.message, toastOptions)
            }
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
        
            <Button variant="contained" onClick={onSave} disabled={loading}>
            {loading ? (
                <Spinner
                    as="span"
                    variant="warning"
                    size="sm"
                    role="status"
                    aria-hidden="true"
                    animation="border"/>
            ) : (
                <span>Save</span>
            )}
            </Button>
            <Button variant="outlined" color='error' onClick={onCancel} sx={{mx: 5}}>Cancel</Button>
        </div>
        <ToastContainer />
    </div>
  )
}

export default Settings