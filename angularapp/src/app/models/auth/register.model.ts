import { AuthModel } from './auth.model';

export interface RegisterModel extends AuthModel {
    passwordConfirm: string;
}