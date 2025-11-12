const BASE_URL = `${import.meta.env.VITE_API_URL}`;

export interface AuthParamsResponse{
    vkAppId: number;
    redirectUrl: string;
    state: string;
    codeChallenge: string;
    authRequestUri: string;
}

export async function apiGetAuthUri(): Promise<AuthParamsResponse> {
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
    const response = await fetch(`${BASE_URL}/Auth/callback${query}`, {
        method: 'GET',
        credentials: 'include',
    });
    
    if (!response.ok) {
        throw new Error(`Error during auth callback: ${response.statusText}`);
    }
}