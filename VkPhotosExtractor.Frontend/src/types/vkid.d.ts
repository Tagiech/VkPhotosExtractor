interface VKID {
    AuthButton: (config: any) => void;
}

declare global {
    interface Window {
        VKID: VKID;
    }
}

export {};
