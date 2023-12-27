import Box from '@mui/material/Box';
import { formatDate } from '../../utils/Utils';

const DrawerContent = (fileItem) => {
  return (
    <Box sx={{ width: 'auto' }} role="presentation">
        <div className='d-flex flex-column align-items-center justify-content-center'>
            <div className='d-flex m-5'>
                <h3 className='fw-bold'>File Details</h3>
            </div>
            <div className='d-flex flex-column m-3'>
                <div className='d-flex justify-content-between m-1'>
                    <p className='me-3'>File Name:&nbsp;</p>
                    <p>{fileItem.fileName}</p>
                </div>
                <div className='d-flex justify-content-between m-1'>
                    <p className='me-3'>Type:&nbsp;</p>
                    <p>{fileItem.type}</p>
                </div>
                <div className='d-flex justify-content-between m-1'>
                    <p className='me-3'>Size:&nbsp;</p>
                    <p>{fileItem.size} B</p>
                </div>
                <div className='d-flex justify-content-between m-1'>
                    <p className='me-3'>Last Modified:&nbsp;</p>
                    <p>{formatDate(fileItem.lastModified)}</p>
                </div>
                <div className='d-flex justify-content-between m-1'>
                    <p className='me-3'>Description:&nbsp;</p>
                    <p>{fileItem.description || "No description"}</p>
                </div>
            </div>
        </div>
    </Box>
  )
}

export default DrawerContent