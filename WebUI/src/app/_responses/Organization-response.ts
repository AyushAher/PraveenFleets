import { Guid } from "guid-typescript";
import { AddressResponse } from "./address-response";
import { UserResponse } from "./user-response";

export default class OrganizationResponse {
    id: Guid;
    name: string;
    gstNumber: string;
    address: AddressResponse;
    adminDetails: UserResponse;
    addressId: Guid;
    adminId: Guid;
}