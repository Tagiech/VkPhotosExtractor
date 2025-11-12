import { useLocation, useNavigate } from "react-router-dom";
import { apiCallback } from "src/authApiClient.ts";
import {useEffect, useState} from "react";

export function AuthCallback(){
    const location = useLocation();
    const navigate = useNavigate();
    
    const [authSuccess, setAuthSuccess] = useState<boolean>(false);
    const [message, setMessage] = useState<string>("");
    const [timer, setTimer] = useState<number>(5);
    
    useEffect(() => {
        apiCallback(location.search)
            .then(() => {
                setMessage("Authentication successful! You will be redirected in {timer} seconds.");
                setAuthSuccess(true);
            })
            .catch((error: Error) => {
                console.error(error);
                setAuthSuccess(false);
                setMessage("Authentication failed. Please try again, you will be redirected back to login page in {timer} seconds.");
            })
    }, [location.search]);
    
    useEffect(() => {
        if (message === "") return;
        
        if (timer === 0) {
            if (authSuccess){
                navigate('/userinfo') //TODO: create userinfo page
            } else {
                navigate('/');
            }
            return;
        }
        
        const id = setTimeout(() => setTimer(t => t - 1), 1000);
        return () => clearTimeout(id);
    }, [timer, message, navigate]);
    
    return (
        <div>
            {message.replace('{timer}', timer.toString())}
        </div>
    )
}