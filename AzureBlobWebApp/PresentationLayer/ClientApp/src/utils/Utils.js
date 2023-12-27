import jwt_decode from "jwt-decode"; 
import { toast } from 'react-toastify'

export const GetUsernameFromToken = () => {
    const token = sessionStorage.getItem('JWTtoken')
    if (token !== null) {
        const decoded = jwt_decode(token)
        //console.log(decoded)
        return decoded.unique_name
    }
    return ''
}

export const GetRolesFromToken = () => {
  const token = sessionStorage.getItem('JWTtoken')
  if (token !== null) {
    const decoded = jwt_decode(token)
    return decoded.role
  }
}

function stringToColor(string) {
    let hash = 0;
    let i;
  
    /* eslint-disable no-bitwise */
    for (i = 0; i < string.length; i += 1) {
      hash = string.charCodeAt(i) + ((hash << 5) - hash);
    }
  
    let color = '#';
  
    for (i = 0; i < 3; i += 1) {
      const value = (hash >> (i * 8)) & 0xff;
      color += `00${value.toString(16)}`.slice(-2);
    }
    /* eslint-enable no-bitwise */
  
    return color;
}

export function stringAvatar(name) {
    return {
      sx: {
        bgcolor: stringToColor(name),
      },
      children: `${name.split(' ')[0][0]}`,
    };
}

export const toastOptions = {
	position: toast.POSITION.TOP_CENTER,
	autoClose: 3000, //3 seconds
	hideProgressBar: false,
	closeOnClick: true,
	pauseOnHover: true,
	draggable: true,
}

export const formatDate = (dateString) => {
  const options = { year: "numeric", month: "long", day: "numeric", hour: 'numeric', minute: 'numeric'}
  return new Date(dateString).toLocaleDateString(undefined, options)
}