import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './app.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)


document.querySelectorAll("textarea").forEach(function (textarea) {
    textarea.style.height = textarea.scrollHeight + "px";
    textarea.style.overflowY = "hidden";

    textarea.addEventListener("input", function () {
        this.style.height = "auto";
        this.style.height = this.scrollHeight + "px";
    });
});