import { useEffect, useState } from "react"
import { Button, Spinner } from 'react-bootstrap'
import { Link, useNavigate } from "react-router-dom"
import { ToastContainer, toast } from 'react-toastify'
import LoginAPI from "../apis/LoginAPI"
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-toastify/dist/ReactToastify.css';

const Login = () => {

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [loading, setLoading] = useState(false)

    const navigate = useNavigate();

    const toastOptions = {
        position: toast.POSITION.TOP_CENTER,
        autoClose: 3000, //3 seconds
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
    }

    useEffect(() => {
        sessionStorage.removeItem('JWTtoken')
    }, [])

    const validate = () => {
        let valid = true;
        if (username === '' || username === null) {
            valid = false;
            toast.warning('Please Enter Username', toastOptions);
        }
        if (password === '' || password === null) {
            valid = false;
            toast.warning('Please Enter Password', toastOptions);
        }
        return valid;
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (validate()) {
            const creds = { username: username, password: password } 
            try {
                setLoading(true)
                let response = await LoginAPI.authenticate(creds);
                setLoading(false)
                sessionStorage.setItem('JWTtoken', response.jwtToken)
                navigate('/home')
            } catch (error) {
                setLoading(false)
                toast.error(error.response.data, toastOptions)
            }         
        }
    }

    return (
        <>
            <div className="row">
                <div className="offset-lg-3 col-lg-6" style={{ marginTop: '100px' }}>
                    <form className="container">
                        <div className="card">
                            <div className="card-header">
                                <h2>User Login</h2>
                            </div>
                            <div className="card-body">
                                <div className="form-group">
                                    <label>User Name <span className="errmsg">*</span></label>
                                    <input value={username} onChange={e => setUsername(e.target.value)} className="form-control"></input>
                                </div>
                                <div className="form-group">
                                    <label>Password <span className="errmsg">*</span></label>
                                    <input type="password" value={password} onChange={e => setPassword(e.target.value)} className="form-control"></input>
                                </div>
                            </div>
                            <div className="card-footer">
                                <Button variant="primary" onClick={handleSubmit}>
                                    {loading ? (
                                        <Spinner
                                            as="span"
                                            variant="warning"
                                            size="sm"
                                            role="status"
                                            aria-hidden="true"
                                            animation="grow"/>
                                    ) : (
                                        <span>Login</span>
                                    )}
                                </Button> &nbsp;
                                <Link className="btn btn-success" to={'/register'}>New User</Link>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
            <ToastContainer />
        </>
        
    )
}

export default Login;