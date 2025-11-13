import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'src/index.css'
import App from 'src/App.tsx'

async function bootstrap() {
    window.APP_CONFIG = await fetch('/config.json').then(r => r.json());
    createRoot(document.getElementById('root')!).render(
        <StrictMode>
            <App />
        </StrictMode>,
    );
}

bootstrap();
