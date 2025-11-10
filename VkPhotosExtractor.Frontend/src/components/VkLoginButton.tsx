import {useEffect, useRef, useState} from "react";

interface Props {
    onClick: () => void;
    ClientId: number;
    RedirectUrl: string;
    State: string;
    CodeChallenge: string;
}

export function VkLoginButton(props: Props) {
    const btnRef = useRef<HTMLDivElement>(null);
    const [sdkReady, setSdkReady] = useState(false);

    useEffect(() => {
        const checkSDK = () => {
            if (window.VKIDSDK) {
                setSdkReady(true);
            } else {
                setTimeout(checkSDK, 100);
            }
        };
        checkSDK();
    }, []);

    useEffect(() => {
        if (!btnRef.current || !sdkReady) return;

        window.VKIDSDK?.Config.init({
            app: props.ClientId,
            redirectUrl: props.RedirectUrl,
            state: props.State,
            codeChallenge: props.CodeChallenge,
            scope: 'email phone',
        });
    }, [sdkReady]);

    return (
        <div onClick={props.onClick}>
            <div ref={btnRef}></div>
        </div>
    );
}
