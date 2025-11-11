
interface Props {
    onClick: () => void;
}

export function VkLoginButton(props: Props) {
    return (
        <button type="button" onClick={props.onClick}>Войти через VK ID</button>
    );
}
