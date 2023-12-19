import React, { Component } from 'react';
import { Outlet, Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';
import Login from './components/Login';
import Register from './components/Register';
import NavMenu from './components/NavMenu';

const App = () => {
  return (
      <Routes>
        <Route path='/' element={<Login />} />
        <Route path='/login' element={<Login />} />
        <Route path='/register' element={<Register />} />
        <Route element={
          <>
            <NavMenu />
            <Outlet />
          </>
        }>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Route>
      </Routes>
  );
}

export default App