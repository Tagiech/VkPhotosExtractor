interface VKIDSDK{
    Auth: any;
    Config: any;
    ConfigAuthMode: any;
    ConfigResponseMode: any;
    ConfigSource: any;
    FloatingOneTap: any;
    OneTap: any;
}

declare global {
    interface Window {
        VKIDSDK?: VKIDSDK;
    }
}

export {};
