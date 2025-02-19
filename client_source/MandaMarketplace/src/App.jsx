import {BrowserRouter, Routes, Route} from 'react-router-dom'
import Home from './Pages/home'
import Login from './Pages/login'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route index element={<Home />} />
        <Route path="/login" element={<Login />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
