import { TokenModel } from "../helpers/token.model";

export interface AuthResult {
  success: boolean;
  message: string;
  token: TokenModel|null;
}