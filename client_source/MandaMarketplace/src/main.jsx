import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'

// leave warning for mobile users that site does not support mobile
if (/Mobi|Android/i.test(navigator.userAgent)) {
  alert("Warning: Manda Marketplace is not optimized for mobile devices. For the best experience, please use a desktop or laptop computer.")
}

createRoot(document.getElementById('root')).render(
  <App />
)
