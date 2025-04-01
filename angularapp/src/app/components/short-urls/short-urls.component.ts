import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BaseComponent } from 'src/app/base.component';
import { getFullShortUrlPathByShortCode } from 'src/app/helpers/functions/getShortUrlPathByShortCode';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { ShortUrl } from 'src/app/models/db/short-url';
import { ApiResult } from 'src/app/models/helpers/api-result';
import { ShortUrlService } from 'src/app/services/short-url.service';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-short-urls',
  templateUrl: './short-urls.component.html',
  styleUrls: ['./short-urls.component.css']
})
export class ShortUrlsComponent extends BaseComponent {
  constructor(
    private shortUrlService: ShortUrlService,
    private clipboard: Clipboard,
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(tokenManager, snackBar);
  }
  
  shortUrls!: ShortUrl[];

  pageIndex: number = 0;
  pageSize: number = 10;
  totalCount!: number;

  sortColumn: string = "id";
  sortOrder: "asc" | "desc" = "asc";
    
  displayedColumns = ['index', 'originalUrl', 'shortCode', 'actions'];

  ngOnInit() {
    this.loadData();
  }

  getFullShortUrlPathByShortCode = getFullShortUrlPathByShortCode;
  
  loadData() {
    this.shortUrlService.getShortUrls(this.pageIndex, this.pageSize, this.sortColumn, this.sortOrder, null, null)
      .pipe(this.catchError())
      .subscribe((apiResult: ApiResult<ShortUrl>) => {
        this.totalCount = apiResult.totalCount;
        this.pageIndex = apiResult.pageIndex;
        this.pageSize = apiResult.pageSize;

        this.shortUrls = apiResult.data;
      });
  }

  onSortChange() {
    this.loadData();  
  }

  onPageChange() {
    this.loadData();  
  }

  deleteShortUrl(id: number) {
    this.shortUrlService.deleteShortUrl(id)
      .pipe(this.catchError())
      .subscribe(() => {
        this.loadData();
        this.modelDeletedSuccessfully("Short url");
      })
  };

  originalUrl: string|null = null;
  shortenUrl(){
    if(this.originalUrl) {
      this.shortUrlService.isOriginalUrlUnique(this.originalUrl)
        .pipe(this.catchError())
        .subscribe((isUnique) => {
          if(isUnique) {
            this.shortUrlService.shortenUrl(this.originalUrl!)
              .pipe(this.catchError())
              .subscribe(() => {
                this.loadData();
                this.operationDoneSuccessfully("Url", "shortened");
              })
          }
          else {
            this.errorOccured("Such an original url already exists!");
          }

          this.originalUrl = null;
        })
      
    }
  }

  resolveUrl(shortCode: string) {
    this.shortUrlService.resolveUrl(shortCode)
      .pipe(this.catchError())
      .subscribe();
  }

  copyFullShortUrlPath(shortCode: string) {
    var fullShortUrlPath = getFullShortUrlPathByShortCode(shortCode);
    this.clipboard.copy(fullShortUrlPath);
  }
}
