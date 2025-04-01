import { DbModel } from "./db-model";

export interface ShortUrl extends DbModel {
    originalUrl: string;
    shortCode: string;

    clickCount: number;
    createdDate: Date;

    isCreatedByYou: boolean;
}

