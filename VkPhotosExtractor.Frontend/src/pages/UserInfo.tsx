import { type FC, useEffect, useState } from "react";
import type { User } from "src/models/User.ts";
import { apiGetUser } from "src/authApiClient.ts";

export const UserInfo: FC = () => {
    const [user, setUser] = useState<User | null>(null);
    useEffect(() => {
        apiGetUser()
            .then(setUser)
            .catch((error: Error) => {
                console.error("Failed to fetch user info:", error);
            });
    }, []);
    
    if (!user) {
        return (
            <div>
                Loading user info...
            </div>
        )
    }
    const updatedPhotoUrl = updatePhotoSize(user.photoUrl, 50);
    
    return (
        <div>
            <h1>User Info</h1>
            <p><strong>Name:</strong> {user.firstName}</p>
            <p><strong>Last name:</strong> {user.lastName}</p>
            <img src={updatedPhotoUrl}
                 alt="User Photo"
                 style={{
                     width: 50,
                     height: 50,
                     objectFit: "contain",
                     maxWidth: "100%",
                     maxHeight: "100%",
                     display: "block"
                 }}
            />
        </div>
    )
}

const updatePhotoSize = (url: string, size: number): string => {
    if (!url) return url;

    try {
        const parsedUrl = new URL(url);
        parsedUrl.searchParams.delete("cs");
        parsedUrl.searchParams.append("cs", `${size}x${size}`);

        return parsedUrl.toString();
    } catch (error) {
        console.error(error);

        return url;
    }
}