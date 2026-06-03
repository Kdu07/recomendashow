import { getAuthToken } from './tokenStorageService';

export const API_BASE = global.__DEV__
  ? 'http://172.20.10.3:5253'
  : 'http://localhost:5253';

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

  return fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });
}
