import {BrowserRouter, Routes, Route} from 'react-router';
import {useEffect, useState} from 'react';
import Cookies from 'js-cookie';
import Home from './Pages/home';
import Login from './Pages/login';
import Browse from './Pages/browse';
import Settings from './Pages/settings';
import Item from './Pages/item';
import Signup from './Pages/signup';
import AddFunds from './Pages/addfunds';
import './css/app.css';
import axios from 'axios';
import AuthContext from './context/authContext';

function App() {
  return (
      <BrowserRouter>
        <Routes>
          <Route index element={<Home />} />
          <Route path="/browse" element={<Browse />} />
          <Route path="/login" element={<Login />} />
          <Route path="/settings" element={<Settings />} />
          <Route path="/item" element={<Item />}  />
          <Route path="/signup" element={<Signup />} />
          <Route path="/addfunds" element={<AddFunds />} />
        </Routes>
      </BrowserRouter>
  )
}

export default App
