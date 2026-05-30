import {
  loginWithEmail,
  loginWithGoogle,
  registerWithEmail,
} from '../services/authService';

export function authenticateUser(credentials) {
  return loginWithEmail(credentials);
}

export function createUserAccount(accountData) {
  return registerWithEmail(accountData);
}

export function authenticateWithGoogle() {
  return loginWithGoogle();
}
