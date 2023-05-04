import { Time } from "@angular/common";
import { AddressResponse } from "./address-response";

export default class TripsResponse {
  office: boolean;
  outStation: boolean;
  passengerEmailId: string;
  passengerUserId: string;
  vehicleType: number;
  vehicleTypeDesc: string;
  pickUpAddress: AddressResponse;
  dropAddress: AddressResponse;
  dropTime: Time;
  pickUpTime: Time;
  pickUpDate: Date | string;
  dropDate: Date | string;
  status: number;
  statusDesc: string;
  email: string;
  fullName: string;
}
