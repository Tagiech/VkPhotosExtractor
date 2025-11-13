import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'src/index.css'
import App from 'src/App.tsx'

async function bootstrap() {
    try {
        window.APP_CONFIG = await fetch('/config.json').then(r => {
            if (!r.ok) throw new Error(`Failed to load config: ${r.statusText}`);
            return r.json();
        });
    } catch (error) {
        console.error('Failed to load application config:', error);
        return;
    }
    createRoot(document.getElementById('root')!).render(
        <StrictMode>
            <App />
        </StrictMode>,
    );
}

bootstrap();
