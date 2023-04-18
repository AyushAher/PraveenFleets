import { AddressRequest } from "./address-request";

export class RegisterUserRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    phoneNumber: string;
    emailVerified: boolean;
    profilePictureUrl: string;
}


export class RegisterOrganizationRequest {

    name: string;
    gstNumber: string;
    addressRequest: AddressRequest;
    adminDetailsRequest: RegisterUserRequest;
}



