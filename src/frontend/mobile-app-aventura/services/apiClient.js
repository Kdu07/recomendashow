import { clearAuthStorage, getAuthToken } from './tokenStorageService';
import { handleSessionExpired } from '../navigation/navigationRef';

export const API_BASE = __DEV__
  ? 'http://163.176.204.208:5253'
  : 'http://localhost:5253';

const AUTH_PATHS = ['/api/auth/login', '/api/auth/register'];

function isAuthPath(path) {
  return AUTH_PATHS.some((authPath) => path.startsWith(authPath));
}

async function handleUnauthorized(path, hadToken) {
  if (!hadToken || isAuthPath(path)) {
    return;
  }

  await clearAuthStorage();
  handleSessionExpired();
}

export async function apiFetch(path, options = {}) {
  const token = await getAuthToken();
  const headers = {
    ...options.headers,
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  if (options.body && !headers['Content-Type']) {
    headers['Content-Type'] = 'application/json';
  }

  const response = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  if (response.status === 401) {
    await handleUnauthorized(path, Boolean(token));
  }

  return response;
}
