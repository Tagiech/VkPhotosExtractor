import type { User } from "src/models/User.ts";


export interface AuthParamsResponse{
    vkAppId: number;
    redirectUrl: string;
    state: string;
    codeChallenge: string;
    authRequestUri: string;
}

export async function apiGetAuthUri(): Promise<AuthParamsResponse> {
    const BASE_URL = window.APP_CONFIG.apiUrl;
    const response = await fetch(`${BASE_URL}/Auth/params`, {
        method: 'GET',
        credentials: 'include',
    });
    
    if (!response.ok) {
        throw new Error(`Error fetching auth URL: ${response.statusText}`);
    }
    
    return (await response.json()) as AuthParamsResponse;
}

export async function apiCallback(query: string): Promise<void> {
    const BASE_URL = window.APP_CONFIG.apiUrl;
    const response = await fetch(`${BASE_URL}/Auth/callback${query}`, {
        method: 'GET',
        credentials: 'include',
    });
    
    if (!response.ok) {
        throw new Error(`Error during auth callback: ${response.statusText}`);
    }
}

export async function apiGetUser(): Promise<User>{
    const BASE_URL = window.APP_CONFIG.apiUrl;
    const response = await fetch(`${BASE_URL}/Auth/userinfo`, {
        method: 'GET',
        credentials: 'include',
    });
    
    if (!response.ok) {
        throw new Error(`Error fetching user info: ${response.statusText}`);
    }
    
    return (await response.json()) as User;
}