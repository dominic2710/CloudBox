import { Byte } from "@angular/compiler/src/util";

export interface User {
    id: number;
    userName: string;
    phoneNumber: string;
    email: string;
    status: string;
    sercurityGroupId?: number;
    avatar: Byte[];
}