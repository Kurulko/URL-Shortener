import { Component, OnDestroy } from '@angular/core';
import { catchError, OperatorFunction, Subject, takeUntil, throwError } from 'rxjs';
import { TokenManager } from './helpers/managers/token-manager';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';

@Component({
    template: '',
})
export abstract class BaseComponent implements OnDestroy {
  private destroySubject = new Subject();

  isLoggedIn: boolean = false;
  isAdmin: boolean = false;

  constructor (
    protected tokenManager: TokenManager,
    private snackBar: MatSnackBar
  ) 
  {
    this.subscribeManagers();
    this.initializeData();
  }
  
  subscribeManagers() {
    this.tokenManager.isAuthenticationChanged()
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
      });

    this.tokenManager.isAdminChanged()
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isAdmin = result;
      });
  };

  initializeData(): void {
    this.isLoggedIn = this.tokenManager.isAuthenticated();
    this.isAdmin = this.tokenManager.isAdmin();
  }

  protected errorOccured(error:string) {
    this.showSnackbar(`ERROR: ${error}`)
  }

  protected operationDoneSuccessfully(modelName:string, actionName:string) {
    this.showSnackbar(`${modelName} ${actionName} successfully`)
  }

  protected modelDeletedSuccessfully(modelName:string){
    this.operationDoneSuccessfully(modelName, 'deleted');
  }

  protected modelUpdatedSuccessfully(modelName:string){
    this.operationDoneSuccessfully(modelName, 'updated');
  }

  protected modelAddedSuccessfully(modelName:string){
    this.operationDoneSuccessfully(modelName, 'added');
  }
  
  protected showSnackbar(message: string): void {
    const config = new MatSnackBarConfig();
    config.duration = 1500;
    config.verticalPosition = 'top';

    this.snackBar.open(message, 'Close', config);
  }

  protected catchError<T>() : OperatorFunction<T, T> {
    return catchError((errorResponse: HttpErrorResponse) => {
      console.error(`Error occurred: ${errorResponse.message} - ${errorResponse.status}`);

      let errorMessage = 'An unknown error occurred';

      if(errorResponse.status === HttpStatusCode.InternalServerError) {
        errorMessage = 'Internal error occurred';
      }
      else if (errorResponse.error) {
        if(typeof errorResponse.error === 'string') {
            errorMessage = errorResponse.error!;
        }
        else {
            errorMessage = errorResponse.error.message || JSON.stringify(errorResponse.error);
        }
      } 

      this.showSnackbar(errorMessage);
      return throwError(() => errorResponse);
    });
  }
  
  ngOnDestroy() {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }
}