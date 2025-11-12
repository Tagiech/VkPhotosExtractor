import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css'
import { Login } from "src/pages/Login.tsx";
import { AuthCallback } from "src/pages/AuthCallback.tsx";
import { UserInfo } from "src/pages/UserInfo.tsx";

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/Auth/callback" element={<AuthCallback />} />
                <Route path="/userinfo" element={<UserInfo />} />
            </Routes>
        </BrowserRouter>
    );
}