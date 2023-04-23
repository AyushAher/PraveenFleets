import { UserResponse } from "../_responses/user-response";

export default class CreateOrganizationRolesRequest {
    RoleName: string;
    OrganizationId: string;
    User?: UserResponse;
}