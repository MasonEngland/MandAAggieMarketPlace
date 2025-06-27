import {BrowserRouter, Routes, Route} from 'react-router';
import {useEffect, useState} from 'react';
import Home from './Pages/home';
import Login from './Pages/login';
import Browse from './Pages/browse';
import Settings from './Pages/settings';
import Item from './Pages/item';
import Signup from './Pages/signup';
import AddFunds from './Pages/addfunds';
import ChangePassword from './Pages/changePassword';
import './css/app.css';
import AuthContext from './context/authContext';
import verifyAccount from './util/verifyAccount';
import { Navigate } from 'react-router';

function App() {
  let [user, setUser] = useState({authenticated: false});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    verifyAccount().then((data) => {
      console.log(data);
      setUser(data);
    })
    .catch((err) => {
      console.log(err);
      setUser({authenticated: false});
    })
    .finally(() => {
      setLoading(false);
    });

  }, []);

  const PrivateRoute = ({ children }) => {
    if (loading) {
      return <div>Loading...</div>;
    }
    return user.authenticated ? children : <Navigate to="/login" />;
  };

  return (
    <AuthContext.Provider value={user}>
      <BrowserRouter>
        <Routes>
          <Route index element={<Home />} />
          <Route path="/browse" element={<Browse />} />
          <Route path="/login" element={<Login />} />
          <Route path="/settings" element={
            <PrivateRoute>
              <Settings />
            </PrivateRoute>
          } />
          <Route path="/item" element={<Item />}  />
          <Route path="/signup" element={<Signup />} />
          <Route path="/addfunds" element={<PrivateRoute><AddFunds /></PrivateRoute>} />
          <Route path="/changepassword" element={<PrivateRoute><ChangePassword /></PrivateRoute>} />
        </Routes>
      </BrowserRouter>
    </AuthContext.Provider>
  )
}
export default App
