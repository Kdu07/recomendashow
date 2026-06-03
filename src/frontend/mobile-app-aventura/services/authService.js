import { createAuthUser } from '../models/authModels';
import { API_BASE } from './apiClient';
import { clearAuthStorage, saveAuthData } from './tokenStorageService';

function normalizeAuthResponse(data) {
  return {
    usuarioId: data.usuarioId ?? data.UsuarioId,
    nome: data.nome ?? data.Nome,
    email: data.email ?? data.Email,
    token: data.token ?? data.Token,
  };
}

export async function persistAuthResponse(data) {
  const auth = normalizeAuthResponse(data);

  if (!auth.token) {
    throw new Error('Resposta de autenticação sem token.');
  }

  await saveAuthData(auth);
  return auth;
}

async function postAuth(path, payload) {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Autenticação falhou: ${res.status} ${text}`);
  }

  return res.json();
}

export async function loginWithEmail({ email, password }) {
  try {
    const data = await postAuth('/api/auth/login', {
      Email: email,
      Senha: password,
    });
    return persistAuthResponse(data);
  } catch (error) {
    console.error('loginWithEmail error:', error);
    throw error;
  }
}

export async function logout() {
  await clearAuthStorage();
}

export async function registerWithEmail({ name, email }) {
    await waitForMockResponse();

  return {
    user: createAuthUser({
      id: 'new-user',
      name,
      email,
    }),
    token: 'mock-register-token',
  };
}

export async function loginWithGoogle() {
  await waitForMockResponse();

  return {
    user: MOCK_USER,
    token: 'mock-google-token',
  };
}
