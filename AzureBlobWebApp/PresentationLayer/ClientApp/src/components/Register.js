import { useState } from "react";
import { Button, Spinner } from 'react-bootstrap'
import { Link, useNavigate } from "react-router-dom";
import { ToastContainer, toast } from "react-toastify";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-toastify/dist/ReactToastify.css';
import LoginAPI from "../apis/LoginAPI";

const Register = () => {

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [email, setEmail] = useState('')
    const [loading, setLoading] = useState(false)

    const navigate = useNavigate()

    const toastOptions = {
        position: toast.POSITION.TOP_CENTER,
        autoClose: 3000, //3 seconds
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
    }

    const IsValid = () => {
        let valid = true;
        let errormessage = 'Please enter the value in ';
        if (username === null || username === '') {
            valid = false;
            errormessage += ' Username';
        }
        if (password === null || password === '') {
            valid = false;
            errormessage += ' Password';
        }
        if (email === null || email === '') {
            valid = false;
            errormessage += ' Email';
        }

        if (!valid) {
            toast.warning(errormessage, toastOptions)
        } else if (!(/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(email))) {
            valid = false;
            toast.warning('Please enter a valid email', toastOptions)
        }
        return valid;
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (IsValid()) {
            const userInfo = { username: username, password: password, email: email } 
            try {
                setLoading(true)
                await LoginAPI.register(userInfo)
                setLoading(false)
                navigate('/login')
            } catch (error) {
                setLoading(false)
                toast.error(error.response.data, toastOptions)
            }         
        }
    }

    return (
        <div>
            <div className="offset-lg-3 col-lg-6">
                <form className="container">
                    <div className="card">
                        <div className="card-header">
                            <h1>User Registration</h1>
                        </div>
                        <div className="card-body">

                            <div className="row">
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>Username <span className="errmsg">*</span></label>
                                        <input value={username} onChange={e => setUsername(e.target.value)} className="form-control"></input>
                                    </div>
                                </div>
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>Password <span className="errmsg">*</span></label>
                                        <input value={password} onChange={e => setPassword(e.target.value)} type="password" className="form-control"></input>
                                    </div>
                                </div>
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>Email <span className="errmsg">*</span></label>
                                        <input value={email} onChange={e => setEmail(e.target.value)} className="form-control"></input>
                                    </div>
                                </div>
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
                                        <span>Register</span>
                                    )}
                                </Button> &nbsp;
                            <Link to={'/login'} className="btn btn-danger">Login</Link>
                        </div>
                    </div>
                </form>
            </div>
            <ToastContainer />
        </div>
    );

}

export default Register;