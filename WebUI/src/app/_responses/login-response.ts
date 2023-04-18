export interface LoginResponse {
    token: string;
    refreshToken: string;
    userImageUrl: string;
    userId: string;
    refreshTokenExpiryTime: Date;
}
