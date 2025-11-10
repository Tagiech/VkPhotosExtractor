const BASE_URL = `${import.meta.env.VITE_API_URL}Auth/`;

export interface AuthParamsResponse{
    authUri: string;
}
export async function apiGetAuthUri(): Promise<AuthParamsResponse> {
    const response = await fetch(`${BASE_URL}params`, {
        method: 'GET',
        credentials: 'include',
    });
    
    if (!response.ok) {
        throw new Error(`Error fetching auth URL: ${response.statusText}`);
    }
    
    return (await response.json()) as AuthParamsResponse;
}