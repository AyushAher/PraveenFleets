import { LoginResponse } from "./login-response";

export interface UserResponse {
    id: string;
    email: string;
    firstName: string;
    fullName: string;
    userName: string;
    lastName: string;
    emailConfirmed: string;
    isActive: string;
    phoneNumber: string;
    profilePictureURL: string;
    token: LoginResponse;
    parentEntityId: string;
    userType: number;
}