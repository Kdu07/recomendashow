import {
  loginWithEmail,
  loginWithGoogle,
  logout,
  registerWithEmail,
  deleteUserAccount,
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

<<<<<<< HEAD
export function signOut() {
  return logout();
=======
export function removeUserAccount(userId) {
  return deleteUserAccount(userId);
>>>>>>> develop
}
