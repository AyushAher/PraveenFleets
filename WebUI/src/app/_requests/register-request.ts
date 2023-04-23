import EnumResponse from "../_responses/enum-response";
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
    weeklyOffs: number[];
  gender: any;
  salutation: any;
}


export class RegisterOrganizationRequest {

    name: string;
    gstNumber: string;
    addressRequest: AddressRequest;
    adminDetailsRequest: RegisterUserRequest;
}



