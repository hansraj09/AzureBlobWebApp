/* eslint-disable jsx-a11y/anchor-is-valid */
import React, { useEffect, useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, UncontrolledDropdown, DropdownToggle, DropdownItem, DropdownMenu } from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import Avatar from '@mui/material/Avatar';
import { GetUsernameFromToken, stringAvatar } from '../utils/Utils';
import './NavMenu.css';

const NavMenu = () => {

  const [collapsed, setCollapsed] = useState(true)
  const [signedIn, setSignedIn] = useState(false)

  const navigate = useNavigate()

  useEffect(() => {
    setSignedIn(checkSignedIn())
  }, [])

  function toggleNavbar () {
    setCollapsed(!collapsed)
  }

  const getUsername = () =>  GetUsernameFromToken().toString()
  const checkSignedIn = () => sessionStorage.getItem("JWTtoken") !== null

  const handleSignOut = (e) => {
    e.preventDefault()
    sessionStorage.removeItem("JWTtoken")
    navigate('/login')
  }

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
        <NavbarBrand tag={Link} to="/home">Azure Blob Web App</NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2" />
        {signedIn && (
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
            <UncontrolledDropdown nav inNavbar>
              <DropdownToggle nav>
                  <Avatar {...stringAvatar(getUsername())} />
              </DropdownToggle>
              <DropdownMenu right>
                <DropdownItem>Settings</DropdownItem>
                <DropdownItem divider />
                <DropdownItem className='text-danger' onClick={handleSignOut}>Sign Out</DropdownItem>
              </DropdownMenu>
            </UncontrolledDropdown>               
          </Collapse>
        )}
      </Navbar>
    </header>
  );
}

export default NavMenu
