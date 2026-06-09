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

function calculateAge(dateString) {
  const [day, month, year] = dateString.split('/').map(Number);
  const birth = new Date(year, month - 1, day);
  const today = new Date();
  let age = today.getFullYear() - birth.getFullYear();
  const hasHadBirthday =
    today.getMonth() > birth.getMonth() ||
    (today.getMonth() === birth.getMonth() && today.getDate() >= birth.getDate());
  return hasHadBirthday ? age : age - 1;
}

async function post(path, body) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });

  const json = await response.json();

  if (!response.ok) {
    throw new Error(json.detail || json.title || `Erro ${response.status}`);
  }

  return json;
}

export async function loginWithEmail({ email, password }) {
  const data = await post('/api/auth/login', {
    Email: email,
    Senha: password,
  });
  return {
    user: { id: data.usuarioId, name: data.nome, email: data.email },
  };
}

export async function registerWithEmail({
  name,
  email,
  password,
  birthDate,
  cep,
  street,
  number,
  complement,
  neighborhood,
  city,
  consentimentoAceito,
}) {
  const data = await post('/api/auth/register', {
    Nome: name,
    Email: email,
    Senha: password,
    Idade: calculateAge(birthDate),
    Cep: cep,
    Logradouro: street,
    Numero: number,
    Complemento: complement || null,
    Bairro: neighborhood,
    Cidade: city,
    ConsentimentoAceito: consentimentoAceito,
  });
  return {
    user: { id: data.usuarioId, name: data.nome, email: data.email },
  };
}

export async function deleteUserAccount(userId) {
  const response = await fetch(`${API_BASE_URL}/api/usuario/${userId}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    const json = await response.json().catch(() => ({}));
    throw new Error(json.detail || json.title || `Erro ${response.status}`);
  }
}

export async function loginWithGoogle() {
  throw new Error('Login com Google ainda não implementado.');
}
