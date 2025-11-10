import { useEffect, useRef } from "react";

interface Props {
    onClick: () => void;
}

export function VkLoginButton({ onClick }: Props) {
    const btnRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!btnRef.current) return;

        if (window.VKID) {
            window.VKID.AuthButton({
                container: btnRef.current,
                scheme: "dark",
                theme: "primary",
            });
        }
    }, []);

    return (
        <div onClick={onClick}>
            <div ref={btnRef}></div>
        </div>
    );
}
