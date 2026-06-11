import { clearAuthStorage, getAuthToken } from './tokenStorageService';
import { handleSessionExpired } from '../navigation/navigationRef';

export const API_BASE = __DEV__
  ? 'https://10.254.19.78:7151'
  : 'https://localhost:7151';

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
