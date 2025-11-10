import {useEffect, useState} from 'react'
import { VkLoginButton } from './components/VkLoginButton'
import { apiGetAuthUri } from "./authApiClient.ts";
import './App.css'

export default function App() {
    const [authUri, setAuthUri] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        async function loadAuthUri(){
            try{
                const data = await apiGetAuthUri();
                setAuthUri(data.authUri);
            } catch (e){
                console.error("Failed to fetch auth URI:", e);
            } finally {
                setLoading(false);
            }
        }
        
        loadAuthUri().catch(console.error);
    }, [])

    function handleLogin() {
        if (!authUri) return;
        window.location.href = authUri;
    }

    if (loading) {
        return <p style={{ textAlign: "center" }}>Загрузка...</p>;
    }

    return (
        <div style={{margin: "40px auto", textAlign: "center"}}>
            <h1>VK Photos Extractor</h1>
            <p>Загрузчик альбомов и изображений из VK</p>
            
            {authUri && (
                <VkLoginButton onClick={handleLogin}/>
            )}
        </div>
    );
}