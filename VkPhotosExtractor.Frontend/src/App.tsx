import {useEffect, useState} from 'react'
import { VkLoginButton } from 'src/components/VkLoginButton'
import {apiGetAuthUri, type AuthParamsResponse } from "src/authApiClient.ts";
import './App.css'

export default function App() {
    const [authParams, setAuthParams] = useState<AuthParamsResponse | null>(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        async function loadAuthUri(){
            try{
                const authParamsResponse = await apiGetAuthUri();
                setAuthParams(authParamsResponse);
            } catch (e){
                console.error("Failed to fetch auth URI:", e);
            } finally {
                setLoading(false);
            }
        }
        
        loadAuthUri().catch(console.error);
    }, [])

    function handleLogin() {
        if (!authParams) return;
        window.location.href = authParams.authRequestUri;
    }

    if (loading) {
        return <p style={{ textAlign: "center" }}>Загрузка...</p>;
    }

    return (
        <div style={{margin: "40px auto", textAlign: "center"}}>
            <h1>VK Photos Extractor</h1>
            <p>Загрузчик альбомов и изображений из VK</p>
            
            {authParams && (
                <VkLoginButton
                    onClick={handleLogin}
                    ClientId={ authParams.vkAppId }
                    RedirectUrl={ authParams.returnUri }
                    State={ authParams.state }
                    CodeChallenge={ authParams.codeChallenge}/>
            )}
        </div>
    );
}