import {BrowserRouter, Routes, Route} from 'react-router-dom'
import Home from './Pages/home'
import Login from './Pages/login'
import Browse from './Pages/browse'
import Settings from './Pages/settings'
import Item from './Pages/item'
import './css/app.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route index element={<Home />} />
        <Route path="/browse" element={<Browse />} />
        <Route path="/login" element={<Login />} />
        <Route path="/settings" element={<Settings />} />
        <Route path="/item" element={<Item />}  />
      </Routes>
    </BrowserRouter>
  )
}

export default App
