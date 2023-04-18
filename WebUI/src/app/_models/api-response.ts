export interface ApiResponse<T> {
    data: T;
    failed: boolean;
    messages: string[];
    statusCode: number;
}
