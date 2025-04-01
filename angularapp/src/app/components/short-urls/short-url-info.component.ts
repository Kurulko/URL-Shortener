import { Component } from '@angular/core';
import { ShortUrl } from 'src/app/models/db/short-url';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { BaseComponent } from 'src/app/base.component';
import { ShortUrlService } from 'src/app/services/short-url.service';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { getFullShortUrlPathByShortCode } from 'src/app/helpers/functions/getShortUrlPathByShortCode';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-short-url-info',
  templateUrl: './short-url-info.component.html',
  styleUrls: ['./short-url-info.component.css']
})
export class ShortUrlInfoComponent extends BaseComponent {
  shortUrl!: ShortUrl;

  constructor(
    private activatedRoute: ActivatedRoute, 
    private clipboard: Clipboard,
    private router: Router,
    private shortUrlService: ShortUrlService, 
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(tokenManager, snackBar);
  }
  
  getFullShortUrlPathByShortCode = getFullShortUrlPathByShortCode;
  
  ngOnInit(): void {
    this.loadShortUrl();
  } 

  loadShortUrl() {
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    var id = idParam ? +idParam : 0;

    if (id) {
      this.shortUrlService.getShortUrlById(id)
        .pipe(catchError((errorResponse: HttpErrorResponse) => {
          console.error(`Error occurred: ${errorResponse.message} - ${errorResponse.status}`);

          if (errorResponse.status === HttpStatusCode.NotFound) {
            this.router.navigate(['short-urls']);
          }

          this.showSnackbar(errorResponse.message);
          return throwError(() => errorResponse);
        }))
        .subscribe((result: ShortUrl) => {
          this.shortUrl = result;
        });
    } 
    else {
      this.router.navigate(['/short-urls']);
    }
  }

  copyFullShortUrlPath(shortCode: string) {
    var fullShortUrlPath = getFullShortUrlPathByShortCode(shortCode);
    this.clipboard.copy(fullShortUrlPath);
  }
}
