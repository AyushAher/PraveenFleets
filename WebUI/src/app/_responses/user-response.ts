import { LoginResponse } from "./login-response";

export interface UserResponse {
    id: string;
    userName: string;
    firstName: string;
    lastName: string;
    fullName: string;
    email: string;
    emailConfirmed: string;
    isActive: string;
    phoneNumber: string;
    profilePictureURL: string;
    token: LoginResponse;
}