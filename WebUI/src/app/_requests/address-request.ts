import { Guid } from "guid-typescript";

export class AddressRequest {
    id: Guid;
    parentId: Guid;
    attention: string;
    contactNumber: string;
    addressLine1: string;
    addressLine2: string;
    city: string;
    pinCode: string;
    state: string;
    country: string;
}
