import { useEffect, useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import { toast } from 'react-toastify'
import axios from "axios"

const Login = () => {

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')

    const navigate = useNavigate();

    useEffect(() => {
        sessionStorage.removeItem('JWTtoken')
    }, [])

    const validate = () => {
        let valid = true;
        if (username === '' || username === null) {
            valid = false;
            toast.warning('Please Enter Username');
        }
        if (password === '' || password === null) {
            valid = false;
            toast.warning('Please Enter Password');
        }
        return valid;
    }

    const handleSubmit = (e) => {
        e.preventDefault()
        if (validate) {
           const creds = { username: username, password: password } 
           // axios call
        }
    }

    return (
        <div className="row">
            <div className="offset-lg-3 col-lg-6" style={{ marginTop: '100px' }}>
                <form className="container" onSubmit={handleSubmit}>
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
                            <button type="submit" className="btn btn-primary">Login</button> |
                            <Link className="btn btn-success" to={'/register'}>New User</Link>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default Login;