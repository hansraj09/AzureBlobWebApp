import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const Register = () => {

    const [Username, setUsername] = useState('')
    const [Password, setPassword] = useState('')
    const [Email, setEmail] = useState('')

    const navigate = useNavigate()

    const IsValidate = () => {
        let valid = true;
        let errormessage = 'Please enter the value in ';
        if (Username === null || Username === '') {
            valid = false;
            errormessage += ' Username';
        }
        if (Password === null || Password === '') {
            valid = false;
            errormessage += ' Password';
        }
        if (Email === null || Email === '') {
            valid = false;
            errormessage += ' Email';
        }

        if(!valid){
            toast.warning(errormessage)
        } else if (!(/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(Email))) {
            valid = false;
            toast.warning('Please enter the valid email')
        }
        return valid;
    }

    const handleSubmit = (e) => {
        // e.preventDefault();
        // let regobj = { id, name, password, email, phone, country, address, gender };
        // if (IsValidate()) {
        // //console.log(regobj);
        // fetch("http://localhost:8000/user", {
        //     method: "POST",
        //     headers: { 'content-type': 'application/json' },
        //     body: JSON.stringify(regobj)
        // }).then((res) => {
        //     toast.success('Registered successfully.')
        //     navigate('/login');
        // }).catch((err) => {
        //     toast.error('Failed :' + err.message);
        // });
    }

    return (
        <div>
            <div className="offset-lg-3 col-lg-6">
                <form className="container" onSubmit={handleSubmit}>
                    <div className="card">
                        <div className="card-header">
                            <h1>User Registeration</h1>
                        </div>
                        <div className="card-body">

                            <div className="row">
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>User Name <span className="errmsg">*</span></label>
                                        <input value={Username} onChange={e => setUsername(e.target.value)} className="form-control"></input>
                                    </div>
                                </div>
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>Password <span className="errmsg">*</span></label>
                                        <input value={Password} onChange={e => setPassword(e.target.value)} type="password" className="form-control"></input>
                                    </div>
                                </div>
                                <div className="col-lg-6">
                                    <div className="form-group">
                                        <label>Email <span className="errmsg">*</span></label>
                                        <input value={Email} onChange={e => setEmail(e.target.value)} className="form-control"></input>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="card-footer">
                            <button type="submit" className="btn btn-primary">Register</button> |
                            <Link to={'/login'} className="btn btn-danger">Login</Link>
                        </div>
                    </div>
                </form>
            </div>


        </div>
    );

}

export default Register;