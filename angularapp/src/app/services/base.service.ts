import { WebClient } from '../helpers/web-client';
import { HttpClient } from '@angular/common/http';

export abstract class BaseService {
    protected readonly pathBase:string = "api";
    protected readonly webClient: WebClient;
    protected readonly emptyPath:string = '';

    constructor(httpClient: HttpClient, controllerName: string) {
        this.webClient = new WebClient(httpClient, `${this.pathBase}/${controllerName}`);
    }
}