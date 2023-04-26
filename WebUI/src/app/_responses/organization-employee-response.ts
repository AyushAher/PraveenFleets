import { AddressResponse } from "./address-response";
import { LoginResponse } from "./login-response";
import { UserResponse } from "./user-response";

export default class OrganizationEmployeeResponse implements UserResponse {
    id: string;
    email: string;
    firstName: string;
    fullName: string;
    userName: string;
    lastName: string;
    emailConfirmed: string;
    role: string;
    isActive: string;
    phoneNumber: string;
    profilePictureURL: string;
    token: LoginResponse;
    parentEntityId: string;
    userType: number;

    UserId: string;
    AddressId: string;
    Address: AddressResponse;

} 