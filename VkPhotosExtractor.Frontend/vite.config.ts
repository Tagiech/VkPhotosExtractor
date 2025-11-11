import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()], 
    resolve:{
        alias: {
            'src': path.resolve(__dirname, 'src'),
        },
    },
    build: {
        rollupOptions: {
            output: {
                entryFileNames: `app.[hash].js`,
                chunkFileNames: `chunks/[name].[hash].js`,
                assetFileNames: `assets/[name].[hash][extname]`,
            }
        }
    }
});
