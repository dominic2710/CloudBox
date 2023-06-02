import { Byte } from "@angular/compiler/src/util";

export interface LoginResponse {
    id: number;
    userName: string;
    phoneNumber: string;
    email: string;
    status: string;
    accessToken: string;
    expiredsAt: Date;
    sercurityGroupId: number;
    avatar: Byte[];
}