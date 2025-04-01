import { environment } from "src/environments/environment.prod";

export function getFullShortUrlPathByShortCode(shortCode: string) {
    const shortUrlPath = "api/short-urls/resolve/";
    return  environment.baseUrl + shortUrlPath + shortCode;
}