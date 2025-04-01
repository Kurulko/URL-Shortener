import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { ModelsService } from './models.service';
import { ShortUrl } from '../models/db/short-url';
import { ApiResult } from '../models/helpers/api-result';

@Injectable({
    providedIn: 'root'
})
export class ShortUrlService extends ModelsService {
    constructor(httpClient: HttpClient) {
        super(httpClient, 'short-urls');
    }
    
    resolveUrl(shortCode: string): Observable<Object> {
        return this.webClient.get(`resolve/${shortCode}`);
    }

    shortenUrl(originalUrl:string): Observable<ShortUrl>{
        return this.webClient.post<ShortUrl>(`shorten`, { originalUrl });
    }

    getShortUrlById(id: number): Observable<ShortUrl> {
        return this.webClient.get<ShortUrl>(id.toString());
    }

    getShortUrlByShortCode(shortCode: string): Observable<ShortUrl> {
        return this.webClient.get<ShortUrl>(`by-shortcode/${shortCode}`);
    }

    getShortUrls( pageIndex:number, pageSize:number, sortColumn:string, sortOrder:string, filterColumn:string|null, filterQuery:string|null): Observable<ApiResult<ShortUrl>> {
        return this.webClient.get<ApiResult<ShortUrl>>(this.emptyPath, 
            this.getApiResultHttpParams( pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery));
    }

    deleteShortUrl(id: number): Observable<Object> {
        return this.webClient.delete(id.toString());
    }

    deleteShortUrlByShortCode(shortCode: string): Observable<Object> {
        return this.webClient.delete(`by-shortcode/${shortCode}`);
    }

    isOriginalUrlUnique(originalUrl: string): Observable<boolean> {
        var originalUrlParams =  new HttpParams()
            .set("originalUrl", originalUrl);
        return this.webClient.get<boolean>(`is-unique`, originalUrlParams);
    }
}
